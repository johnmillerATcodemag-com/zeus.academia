import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { FacultyUser, AuthState } from '@/types'
import { AuthService } from '@/services/AuthService'

export const useAuthStore = defineStore('auth', () => {
  // State
  const user = ref<FacultyUser | null>(null)
  const token = ref<string | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Getters
  const isAuthenticated = computed(() => !!token.value && !!user.value)
  const userRole = computed(() => user.value?.role || null)
  const userPermissions = computed(() => user.value?.permissions || [])
  const userFullName = computed(() => 
    user.value ? `${user.value.firstName} ${user.value.lastName}` : ''
  )

  // Actions
  const login = async (email: string, password: string) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await AuthService.login(email, password)
      
      if (response.success) {
        user.value = response.data.user
        token.value = response.data.token
        
        // Store in localStorage for persistence
        localStorage.setItem('zeus_faculty_auth', 'true')
        localStorage.setItem('zeus_faculty_token', response.data.token)
        localStorage.setItem('zeus_faculty_role', response.data.user.role)
        localStorage.setItem('zeus_faculty_user', JSON.stringify(response.data.user))
        
        return { success: true }
      } else {
        error.value = response.error || 'Login failed'
        return { success: false, error: error.value }
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'An unexpected error occurred'
      return { success: false, error: error.value }
    } finally {
      loading.value = false
    }
  }

  const logout = async () => {
    loading.value = true
    
    try {
      if (token.value) {
        await AuthService.logout(token.value)
      }
    } catch (err) {
      console.warn('Logout request failed:', err)
    } finally {
      // Clear state regardless of API call success
      user.value = null
      token.value = null
      error.value = null
      
      // Clear localStorage
      localStorage.removeItem('zeus_faculty_auth')
      localStorage.removeItem('zeus_faculty_token')
      localStorage.removeItem('zeus_faculty_role')
      localStorage.removeItem('zeus_faculty_user')
      
      loading.value = false
    }
  }

  const initializeAuth = async () => {
    const storedToken = localStorage.getItem('zeus_faculty_token')
    const storedUser = localStorage.getItem('zeus_faculty_user')
    
    if (storedToken && storedUser) {
      try {
        token.value = storedToken
        user.value = JSON.parse(storedUser)
        
        // Validate token with server
        const isValid = await AuthService.validateToken(storedToken)
        if (!isValid) {
          await logout()
        }
      } catch (err) {
        console.error('Failed to initialize auth:', err)
        await logout()
      }
    }
  }

  const hasPermission = (permission: string): boolean => {
    return userPermissions.value.includes(permission as any)
  }

  const hasRole = (role: string): boolean => {
    return userRole.value === role
  }

  const hasAnyRole = (roles: string[]): boolean => {
    return roles.includes(userRole.value || '')
  }

  const refreshToken = async () => {
    if (!token.value) return false
    
    try {
      const response = await AuthService.refreshToken(token.value)
      if (response.success) {
        token.value = response.data.token
        localStorage.setItem('zeus_faculty_token', response.data.token)
        return true
      }
    } catch (err) {
      console.error('Token refresh failed:', err)
    }
    
    return false
  }

  return {
    // State
    user,
    token,
    loading,
    error,
    
    // Getters
    isAuthenticated,
    userRole,
    userPermissions,
    userFullName,
    
    // Actions
    login,
    logout,
    initializeAuth,
    hasPermission,
    hasRole,
    hasAnyRole,
    refreshToken
  }
})