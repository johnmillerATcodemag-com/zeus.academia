import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { 
  AdminUser, 
  AdminState, 
  AdminRoleType, 
  AdminPermission,
  SecurityContext,
  SessionInfo,
  RiskAssessment,
  SecurityLevel
} from '@/types'
import { AdminApiService } from '@/services/AdminApiService'

export const useAuthStore = defineStore('auth', () => {
  // State
  const user = ref<AdminUser | null>(null)
  const token = ref<string | null>(null)
  const refreshToken = ref<string | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)
  const sessionInfo = ref<SessionInfo | null>(null)
  const lastActivity = ref<Date>(new Date())

  // Getters
  const isAuthenticated = computed(() => !!token.value && !!user.value)
  const userRole = computed(() => user.value?.role || null)
  const userPermissions = computed(() => user.value?.permissions || [])
  const userFullName = computed(() => 
    user.value ? `${user.value.firstName} ${user.value.lastName}` : ''
  )
  const securityLevel = computed(() => user.value?.securityLevel || 'basic')
  const isMfaEnabled = computed(() => user.value?.mfaEnabled || false)

  // Role-based access checks
  const isSystemAdmin = computed(() => userRole.value === 'system_admin')
  const isRegistrar = computed(() => userRole.value === 'registrar')
  const isAcademicAdmin = computed(() => userRole.value === 'academic_admin')

  // Permission checks
  const hasPermission = (permission: AdminPermission): boolean => {
    if (!user.value) return false
    return user.value.permissions.includes(permission)
  }

  const hasAnyPermission = (permissions: AdminPermission[]): boolean => {
    if (!user.value) return false
    return permissions.some(permission => user.value!.permissions.includes(permission))
  }

  const hasAllPermissions = (permissions: AdminPermission[]): boolean => {
    if (!user.value) return false
    return permissions.every(permission => user.value!.permissions.includes(permission))
  }

  // Role hierarchy checks
  const canManageRole = (targetRole: AdminRoleType): boolean => {
    if (!user.value) return false
    
    const roleHierarchy: Record<AdminRoleType, number> = {
      'system_admin': 3,
      'registrar': 2,
      'academic_admin': 1
    }
    
    const currentRoleLevel = roleHierarchy[user.value.role]
    const targetRoleLevel = roleHierarchy[targetRole]
    
    return currentRoleLevel >= targetRoleLevel
  }

  // Security context
  const getSecurityContext = (): SecurityContext | null => {
    if (!user.value || !sessionInfo.value) return null
    
    return {
      user: user.value,
      session: sessionInfo.value,
      request: {
        resource: '',
        action: '',
        parameters: {},
        timestamp: new Date(),
        riskScore: 0
      },
      riskAssessment: {
        score: 0,
        factors: [],
        level: 'low',
        recommendations: []
      }
    }
  }

  // Risk assessment
  const assessRisk = (action: string, resource: string, parameters: Record<string, any> = {}): RiskAssessment => {
    let score = 0
    const factors = []
    
    // Base risk factors
    if (action.includes('delete')) {
      score += 0.3
      factors.push({ type: 'destructive_action', description: 'Delete operation', weight: 0.3, value: action })
    }
    
    if (action.includes('bulk')) {
      score += 0.4
      factors.push({ type: 'bulk_operation', description: 'Bulk operation', weight: 0.4, value: action })
    }
    
    if (resource === 'users' && action.includes('modify')) {
      score += 0.2
      factors.push({ type: 'user_modification', description: 'User account modification', weight: 0.2, value: resource })
    }
    
    // Session factors
    if (sessionInfo.value) {
      const sessionAge = Date.now() - sessionInfo.value.startTime.getTime()
      if (sessionAge > 3600000) { // 1 hour
        score += 0.1
        factors.push({ type: 'session_age', description: 'Old session', weight: 0.1, value: sessionAge })
      }
    }
    
    // Determine risk level
    let level: 'low' | 'medium' | 'high' | 'critical' = 'low'
    if (score >= 0.8) level = 'critical'
    else if (score >= 0.6) level = 'high'
    else if (score >= 0.3) level = 'medium'
    
    const recommendations = []
    if (level === 'high' || level === 'critical') {
      recommendations.push('Require additional authentication')
      recommendations.push('Log detailed audit information')
    }
    if (action.includes('bulk')) {
      recommendations.push('Confirm operation with user')
    }
    
    return {
      score,
      factors,
      level,
      recommendations
    }
  }

  // Actions
  const login = async (email: string, password: string, mfaCode?: string) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await AdminApiService.auth.login({
        email,
        password,
        mfaCode,
        deviceInfo: {
          userAgent: navigator.userAgent,
          platform: navigator.platform,
          language: navigator.language
        }
      })
      
      if (response.success && response.data) {
        token.value = response.data.token
        refreshToken.value = response.data.refreshToken
        
        // Map backend user to AdminUser format
        user.value = {
          id: response.data.user?.id || '1',
          email: response.data.user?.email || email,
          firstName: response.data.user?.firstName || 'Admin',
          lastName: response.data.user?.lastName || 'User',
          role: 'system_admin' as AdminRoleType,
          permissions: [
            'user_management',
            'system_configuration', 
            'security_management',
            'system_monitoring',
            'audit_access'
          ] as AdminPermission[],
          department: response.data.user?.department || 'Administration',
          title: response.data.user?.title || 'System Administrator',
          lastLogin: new Date(),
          isActive: true,
          mfaEnabled: false,
          trustedDevices: [],
          securityLevel: 'critical' as SecurityLevel
        }
        
        // Create mock session info
        sessionInfo.value = {
          id: crypto.randomUUID(),
          startTime: new Date(),
          lastActivity: new Date(),
          ipAddress: '127.0.0.1',
          userAgent: navigator.userAgent,
          deviceFingerprint: 'mock-device-fingerprint'
        }
        
        lastActivity.value = new Date()
        
        // Store in localStorage for persistence
        localStorage.setItem('zeus_admin_token', response.data.token)
        localStorage.setItem('zeus_admin_refresh_token', response.data.refreshToken)
        localStorage.setItem('zeus_admin_user', JSON.stringify(response.data.user))
        
        return { success: true }
      } else {
        throw new Error(response.message || 'Login failed')
      }
    } catch (err: any) {
      error.value = err.message || 'Login failed'
      return { success: false, error: error.value }
    } finally {
      loading.value = false
    }
  }

  const logout = async () => {
    loading.value = true
    
    try {
      if (token.value) {
        await AdminApiService.auth.logout()
      }
    } catch (err) {
      console.warn('Logout API call failed:', err)
    } finally {
      // Clear state regardless of API call success
      user.value = null
      token.value = null
      refreshToken.value = null
      sessionInfo.value = null
      error.value = null
      
      // Clear localStorage
      localStorage.removeItem('zeus_admin_token')
      localStorage.removeItem('zeus_admin_refresh_token')
      localStorage.removeItem('zeus_admin_user')
      
      loading.value = false
    }
  }

  const refreshAccessToken = async (): Promise<boolean> => {
    if (!refreshToken.value) return false
    
    try {
      const response = await AdminApiService.auth.refreshToken(refreshToken.value)
      
      if (response.success && response.data) {
        token.value = response.data.token
        refreshToken.value = response.data.refreshToken
        lastActivity.value = new Date()
        
        localStorage.setItem('zeus_admin_token', response.data.token)
        localStorage.setItem('zeus_admin_refresh_token', response.data.refreshToken)
        
        return true
      }
    } catch (err) {
      console.error('Token refresh failed:', err)
      await logout()
    }
    
    return false
  }

  const initializeAuth = async () => {
    loading.value = true
    
    try {
      const storedToken = localStorage.getItem('zeus_admin_token')
      const storedRefreshToken = localStorage.getItem('zeus_admin_refresh_token')
      const storedUser = localStorage.getItem('zeus_admin_user')
      
      if (storedToken && storedRefreshToken && storedUser) {
        token.value = storedToken
        refreshToken.value = storedRefreshToken
        user.value = JSON.parse(storedUser)
        lastActivity.value = new Date()
        
        // Validate token with API
        try {
          const response = await AdminApiService.auth.validateToken()
          if (!response.success) {
            // Try to refresh token
            const refreshSuccess = await refreshAccessToken()
            if (!refreshSuccess) {
              await logout()
            }
          }
        } catch (err) {
          console.warn('Token validation failed:', err)
          await logout()
        }
      }
    } catch (err) {
      console.error('Auth initialization failed:', err)
      await logout()
    } finally {
      loading.value = false
    }
  }

  const updateLastActivity = () => {
    lastActivity.value = new Date()
  }

  const checkSessionTimeout = (): boolean => {
    if (!sessionInfo.value) return false
    
    const sessionTimeout = 3600000 // 1 hour in milliseconds
    const sessionAge = Date.now() - sessionInfo.value.startTime.getTime()
    const inactivityTime = Date.now() - lastActivity.value.getTime()
    
    return sessionAge > sessionTimeout || inactivityTime > sessionTimeout
  }

  // Export store interface
  return {
    // State
    user: computed(() => user.value),
    token: computed(() => token.value),
    loading: computed(() => loading.value),
    error: computed(() => error.value),
    sessionInfo: computed(() => sessionInfo.value),
    lastActivity: computed(() => lastActivity.value),
    
    // Getters
    isAuthenticated,
    userRole,
    userPermissions,
    userFullName,
    securityLevel,
    isMfaEnabled,
    isSystemAdmin,
    isRegistrar,
    isAcademicAdmin,
    
    // Methods
    hasPermission,
    hasAnyPermission,
    hasAllPermissions,
    canManageRole,
    getSecurityContext,
    assessRisk,
    login,
    logout,
    refreshAccessToken,
    initializeAuth,
    updateLastActivity,
    checkSessionTimeout
  }
})