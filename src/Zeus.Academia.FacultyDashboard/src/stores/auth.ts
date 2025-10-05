import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { FacultyUser, AuthState, FacultyRole, Permission } from '@/types'
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

  // Task 2: Enhanced Auth Methods for Hierarchical Permissions and Role Management
  const roleHierarchy: Record<FacultyRole, number> = {
    'lecturer': 0,
    'assistant_professor': 1,
    'associate_professor': 2,
    'professor': 3,
    'chair': 4,
    'dean': 5,
    'admin': 6
  }

  const setUser = (newUser: FacultyUser) => {
    user.value = newUser
  }

  const setRole = (role: FacultyRole) => {
    if (!user.value) return
    
    const validRoles: FacultyRole[] = ['lecturer', 'assistant_professor', 'associate_professor', 'professor', 'chair', 'dean', 'admin']
    if (!validRoles.includes(role)) {
      throw new Error('Invalid faculty role')
    }
    
    user.value.role = role
  }

  const addPermission = (permission: Permission) => {
    if (!user.value) return
    
    const validPermissions: Permission[] = [
      'view_courses', 'manage_grades', 'view_students', 'manage_students',
      'create_assignments', 'manage_course_content', 'view_analytics',
      'manage_faculty', 'view_department', 'manage_department',
      'view_college', 'manage_college', 'system_admin'
    ]
    
    if (!validPermissions.includes(permission)) {
      throw new Error('Invalid permission')
    }
    
    if (!user.value.permissions.includes(permission)) {
      user.value.permissions.push(permission)
    }
  }

  const getRoleHierarchy = (): FacultyRole => {
    return userRole.value || 'lecturer'
  }

  const getRoleLevel = (role: FacultyRole): number => {
    return roleHierarchy[role] || 0
  }

  const canAccessRole = (currentRole: FacultyRole, targetRole: FacultyRole): boolean => {
    return getRoleLevel(currentRole) >= getRoleLevel(targetRole)
  }

  const getRoleDisplayName = (role: FacultyRole): string => {
    const displayNames: Record<FacultyRole, string> = {
      'lecturer': 'Lecturer',
      'assistant_professor': 'Assistant Professor',
      'associate_professor': 'Associate Professor',
      'professor': 'Professor',
      'chair': 'Department Chair',
      'dean': 'Dean',
      'admin': 'Administrator'
    }
    return displayNames[role] || role
  }

  const isAdministrativeRole = (role: FacultyRole): boolean => {
    return ['chair', 'dean', 'admin'].includes(role)
  }

  const getRoleColor = (role: FacultyRole): string => {
    const colors: Record<FacultyRole, string> = {
      'lecturer': 'secondary',
      'assistant_professor': 'info',
      'associate_professor': 'info',
      'professor': 'primary',
      'chair': 'warning',
      'dean': 'danger',
      'admin': 'dark'
    }
    return colors[role] || 'secondary'
  }

  const canManageFaculty = (): boolean => {
    return hasPermission('manage_faculty')
  }

  const canManageDepartment = (): boolean => {
    return hasPermission('manage_department')
  }

  const canManageCollege = (): boolean => {
    return hasPermission('manage_college')
  }

  const canAccessSubordinate = (subordinateRole: FacultyRole): boolean => {
    if (!userRole.value) return false
    return canAccessRole(userRole.value, subordinateRole)
  }

  const canInitiateWorkflow = (workflowType: string): boolean => {
    const workflowPermissions: Record<string, Permission[]> = {
      'faculty_hiring': ['manage_faculty'],
      'promotion_review': ['manage_faculty', 'manage_department'],
      'tenure_review': ['manage_faculty', 'manage_department'],
      'budget_approval': ['manage_college'],
      'course_approval': ['manage_department'],
      'student_discipline': ['manage_college']
    }

    const requiredPermissions = workflowPermissions[workflowType]
    if (!requiredPermissions) return false

    return requiredPermissions.some(permission => hasPermission(permission))
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
    refreshToken,

    // Task 2: Enhanced Auth Methods
    setUser,
    setRole,
    addPermission,
    getRoleHierarchy,
    getRoleLevel,
    canAccessRole,
    getRoleDisplayName,
    isAdministrativeRole,
    getRoleColor,
    canManageFaculty,
    canManageDepartment,
    canManageCollege,
    canAccessSubordinate,
    canInitiateWorkflow
  }
})