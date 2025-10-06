import axios from 'axios'
import type { AxiosInstance, AxiosResponse } from 'axios'
import type { 
  ApiResponse, 
  AdminUser, 
  BulkOperationResult,
  SessionInfo,
  AuditEntry
} from '@/types'

/**
 * Administrative API Service
 * Handles all API communications for the administrative interface
 */
class AdminApiServiceClass {
  private api: AxiosInstance
  private baseURL: string

  constructor() {
    this.baseURL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'
    
    this.api = axios.create({
      baseURL: this.baseURL,
      timeout: 30000,
      headers: {
        'Content-Type': 'application/json',
      },
    })

    this.setupInterceptors()
  }

  private setupInterceptors() {
    // Request interceptor to add auth token
    this.api.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('zeus_admin_token')
        if (token) {
          config.headers.Authorization = `Bearer ${token}`
        }
        
        // Add request timestamp and ID for tracking
        config.headers['X-Request-ID'] = crypto.randomUUID()
        config.headers['X-Request-Timestamp'] = new Date().toISOString()
        
        return config
      },
      (error) => {
        return Promise.reject(error)
      }
    )

    // Response interceptor to handle errors and token refresh
    this.api.interceptors.response.use(
      (response) => {
        return response
      },
      async (error) => {
        const originalRequest = error.config

        if (error.response?.status === 401 && !originalRequest._retry) {
          originalRequest._retry = true

          try {
            const refreshToken = localStorage.getItem('zeus_admin_refresh_token')
            if (refreshToken) {
              const response = await this.auth.refreshToken(refreshToken)
              if (response.success && response.data) {
                localStorage.setItem('zeus_admin_token', response.data.token)
                localStorage.setItem('zeus_admin_refresh_token', response.data.refreshToken)
                originalRequest.headers.Authorization = `Bearer ${response.data.token}`
                return this.api(originalRequest)
              }
            }
          } catch (refreshError) {
            console.error('Token refresh failed:', refreshError)
            // Redirect to login or handle logout
            this.handleAuthFailure()
          }
        }

        return Promise.reject(error)
      }
    )
  }

  private handleAuthFailure() {
    // Clear stored auth data
    localStorage.removeItem('zeus_admin_token')
    localStorage.removeItem('zeus_admin_refresh_token')
    localStorage.removeItem('zeus_admin_user')
    
    // In a real app, this would trigger a redirect to login
    console.warn('Authentication failed, user should be redirected to login')
  }

  private async makeRequest<T>(
    method: 'get' | 'post' | 'put' | 'delete',
    url: string,
    data?: any,
    config?: any
  ): Promise<ApiResponse<T>> {
    try {
      const response: AxiosResponse<T> = await this.api[method](url, data, config)
      
      // Backend returns direct data, not wrapped in ApiResponse format
      // So we need to wrap it in the expected format
      return {
        success: true,
        data: response.data,
        message: 'Request successful'
      }
    } catch (error: any) {
      if (error.response?.data) {
        return {
          success: false,
          message: error.response.data.message || error.message || 'Request failed',
          errors: error.response.data.errors || [error.message || 'Network error']
        }
      }
      
      return {
        success: false,
        message: error.message || 'An unexpected error occurred',
        errors: [error.message || 'Network error']
      }
    }
  }

  // Authentication endpoints
  public auth = {
    login: async (credentials: {
      email: string
      password: string
      mfaCode?: string
      deviceInfo?: Record<string, any>
    }) => {
      return this.makeRequest<{
        token: string
        refreshToken: string
        user: AdminUser
        session: SessionInfo
      }>('post', '/api/auth/login', credentials)
    },

    logout: async () => {
      // Backend doesn't have logout endpoint, handle locally
      return Promise.resolve({ success: true })
    },

    refreshToken: async (refreshToken: string) => {
      return this.makeRequest<{
        token: string
        refreshToken: string
      }>('post', '/api/auth/refresh', { refreshToken })
    },

    validateToken: async () => {
      return this.makeRequest('get', '/api/student/profile')
    },

    changePassword: async (data: {
      currentPassword: string
      newPassword: string
      confirmPassword: string
    }) => {
      return this.makeRequest('put', '/api/student/profile', data)
    }
  }

  // User management endpoints
  public users = {
    getAll: async (filters?: Record<string, any>, pagination?: Record<string, any>) => {
      const params = new URLSearchParams()
      if (filters) {
        Object.entries(filters).forEach(([key, value]) => {
          if (value != null) params.append(key, String(value))
        })
      }
      if (pagination) {
        Object.entries(pagination).forEach(([key, value]) => {
          if (value != null) params.append(key, String(value))
        })
      }
      
      return this.makeRequest<{ data: any[]; total: number }>('get', `/admin/users?${params}`)
    },

    getById: async (id: string) => {
      return this.makeRequest<any>('get', `/admin/users/${id}`)
    },

    create: async (userData: any) => {
      return this.makeRequest<any>('post', '/admin/users', userData)
    },

    update: async (id: string, userData: any) => {
      return this.makeRequest<any>('put', `/admin/users/${id}`, userData)
    },

    delete: async (id: string) => {
      return this.makeRequest('delete', `/admin/users/${id}`)
    },

    bulkOperation: async (operation: string, ids: string[], params?: Record<string, any>) => {
      return this.makeRequest<BulkOperationResult>('post', '/admin/users/bulk', {
        operation,
        ids,
        parameters: params || {}
      })
    },

    export: async (format: 'csv' | 'xlsx' | 'json', filters?: Record<string, any>) => {
      const response = await this.api.post('/admin/users/export', 
        { format, filters }, 
        { responseType: 'blob' }
      )
      return response.data
    },

    bulkCreate: async (userData: any) => {
      return this.makeRequest<any>('post', '/admin/users/bulk-create', userData)
    },

    suspend: async (suspensionData: any) => {
      return this.makeRequest<any>('post', '/admin/users/suspend', suspensionData)
    },

    reactivate: async (reactivationData: any) => {
      return this.makeRequest<any>('post', '/admin/users/reactivate', reactivationData)
    },

    resetPassword: async (resetRequest: any) => {
      return this.makeRequest<any>('post', '/admin/users/reset-password', resetRequest)
    }
  }

  // Role management endpoints
  public roles = {
    getAll: async () => {
      return this.makeRequest<any[]>('get', '/admin/roles')
    },

    assign: async (userId: string, roleId: string) => {
      return this.makeRequest('post', `/admin/users/${userId}/roles/${roleId}`)
    },

    revoke: async (userId: string, roleId: string) => {
      return this.makeRequest('delete', `/admin/users/${userId}/roles/${roleId}`)
    },

    getPermissions: async (roleId: string) => {
      return this.makeRequest<string[]>('get', `/admin/roles/${roleId}/permissions`)
    },

    updatePermissions: async (roleId: string, permissions: string[]) => {
      return this.makeRequest('put', `/admin/roles/${roleId}/permissions`, { permissions })
    }
  }

  // Audit endpoints
  public audit = {
    getTrail: async (filters?: Record<string, any>) => {
      const params = new URLSearchParams()
      if (filters) {
        Object.entries(filters).forEach(([key, value]) => {
          if (value != null) params.append(key, String(value))
        })
      }
      return this.makeRequest<any[]>('get', `/admin/audit/trail?${params}`)
    },

    getUserActivity: async (filters?: Record<string, any>) => {
      const params = new URLSearchParams()
      if (filters) {
        Object.entries(filters).forEach(([key, value]) => {
          if (value != null) params.append(key, String(value))
        })
      }
      return this.makeRequest<any>('get', `/admin/audit/user-activity?${params}`)
    },

    exportAuditLog: async (exportRequest: any) => {
      return this.makeRequest<any>('post', '/admin/audit/export', exportRequest)
    }
  }

  // System monitoring endpoints
  public system = {
    getHealth: async () => {
      return this.makeRequest<Record<string, any>>('get', '/api/health')
    },

    getMetrics: async () => {
      // Return mock metrics since the endpoint doesn't exist yet
      return Promise.resolve({
        userCount: 1247,
        activeUsers: 1089,
        systemLoad: 0.45,
        memoryUsage: 0.67,
        responseTime: Math.floor(Math.random() * 100) + 20
      })
    },

    getAuditLogs: async (filters?: Record<string, any>) => {
      const params = new URLSearchParams()
      if (filters) {
        Object.entries(filters).forEach(([key, value]) => {
          if (value != null) params.append(key, String(value))
        })
      }
      
      return this.makeRequest<AuditEntry[]>('get', `/admin/system/audit-logs?${params}`)
    },

    backup: async (options?: Record<string, any>) => {
      return this.makeRequest<{ jobId: string }>('post', '/admin/system/backup', options)
    },

    getBackupStatus: async (jobId: string) => {
      return this.makeRequest<Record<string, any>>('get', `/admin/system/backup/${jobId}`)
    }
  }

  // Academic management endpoints
  public academic = {
    getCalendar: async () => {
      return this.makeRequest<any>('get', '/admin/academic/calendar')
    },

    updateCalendar: async (calendar: any) => {
      return this.makeRequest<any>('put', '/admin/academic/calendar', calendar)
    },

    getEnrollments: async (filters?: Record<string, any>) => {
      const params = new URLSearchParams()
      if (filters) {
        Object.entries(filters).forEach(([key, value]) => {
          if (value != null) params.append(key, String(value))
        })
      }
      
      return this.makeRequest<{ data: any[]; total: number }>('get', `/admin/academic/enrollments?${params}`)
    },

    bulkEnrollment: async (operations: any[]) => {
      return this.makeRequest<{ success: number; failed: number }>('post', '/admin/academic/enrollments/bulk', {
        operations
      })
    },

    getCourses: async (filters?: Record<string, any>) => {
      const params = new URLSearchParams()
      if (filters) {
        Object.entries(filters).forEach(([key, value]) => {
          if (value != null) params.append(key, String(value))
        })
      }
      
      return this.makeRequest<{ data: any[]; total: number }>('get', `/admin/academic/courses?${params}`)
    },

    getDepartments: async () => {
      return this.makeRequest<any[]>('get', '/admin/academic/departments')
    }
  }

  // Financial management endpoints
  public financial = {
    getTuitionStructure: async () => {
      return this.makeRequest<any>('get', '/admin/financial/tuition')
    },

    updateTuitionStructure: async (structure: any) => {
      return this.makeRequest<any>('put', '/admin/financial/tuition', structure)
    },

    getStudentAccounts: async (filters?: Record<string, any>) => {
      const params = new URLSearchParams()
      if (filters) {
        Object.entries(filters).forEach(([key, value]) => {
          if (value != null) params.append(key, String(value))
        })
      }
      
      return this.makeRequest<{ data: any[]; total: number }>('get', `/admin/financial/accounts?${params}`)
    },

    processPayment: async (accountId: string, payment: any) => {
      return this.makeRequest<any>('post', `/admin/financial/accounts/${accountId}/payments`, payment)
    },

    processRefund: async (accountId: string, refund: any) => {
      return this.makeRequest<any>('post', `/admin/financial/accounts/${accountId}/refunds`, refund)
    }
  }

  // Configuration endpoints
  public configuration = {
    getInstitutionProfile: async () => {
      return this.makeRequest<any>('get', '/admin/configuration/institution')
    },

    updateInstitutionProfile: async (profile: any) => {
      return this.makeRequest<any>('put', '/admin/configuration/institution', profile)
    },

    getSystemSettings: async (category?: string) => {
      const url = category ? `/admin/configuration/settings/${category}` : '/admin/configuration/settings'
      return this.makeRequest<Record<string, any>>('get', url)
    },

    updateSystemSettings: async (category: string, settings: Record<string, any>) => {
      return this.makeRequest<any>('put', `/admin/configuration/settings/${category}`, settings)
    },

    getEmailTemplates: async () => {
      return this.makeRequest<any[]>('get', '/admin/configuration/email-templates')
    },

    updateEmailTemplate: async (templateId: string, template: any) => {
      return this.makeRequest<any>('put', `/admin/configuration/email-templates/${templateId}`, template)
    }
  }

  // Analytics endpoints
  public analytics = {
    getDashboardMetrics: async (role: string) => {
      return this.makeRequest<Record<string, any>>('get', `/admin/analytics/dashboard/${role}`)
    },

    getEnrollmentStatistics: async (timeRange?: string) => {
      const params = timeRange ? `?timeRange=${timeRange}` : ''
      return this.makeRequest<Record<string, any>>('get', `/admin/analytics/enrollment${params}`)
    },

    getFinancialReports: async (reportType: string, parameters?: Record<string, any>) => {
      return this.makeRequest<any>('post', `/admin/analytics/financial/${reportType}`, parameters)
    },

    getUserActivityReport: async (filters?: Record<string, any>) => {
      return this.makeRequest<any>('post', '/admin/analytics/user-activity', filters)
    }
  }
}

// Create and export singleton instance
export const AdminApiService = new AdminApiServiceClass()