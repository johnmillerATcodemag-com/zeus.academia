import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { 
  DataState, 
  PaginationState, 
  Alert,
  BulkOperationResult
} from '@/types'
import { AdminApiService } from '@/services/AdminApiService'
import { useAuthStore } from './auth'

export const useDataStore = defineStore('data', () => {
  // Users state
  const users = ref<{
    data: any[]
    totalCount: number
    filters: Record<string, any>
    pagination: PaginationState
    loading: boolean
    error: string | null
  }>({
    data: [],
    totalCount: 0,
    filters: {},
    pagination: {
      page: 1,
      pageSize: 50,
      totalPages: 0,
      totalItems: 0
    },
    loading: false,
    error: null
  })

  // Courses state
  const courses = ref<{
    data: any[]
    totalCount: number
    byDepartment: Record<string, any[]>
    loading: boolean
    error: string | null
  }>({
    data: [],
    totalCount: 0,
    byDepartment: {},
    loading: false,
    error: null
  })

  // Enrollments state
  const enrollments = ref<{
    current: any[]
    historical: any[]
    statistics: Record<string, any>
    loading: boolean
    error: string | null
  }>({
    current: [],
    historical: [],
    statistics: {},
    loading: false,
    error: null
  })

  // System metrics state
  const systemMetrics = ref<{
    performance: Record<string, any>
    health: Record<string, any>
    alerts: Alert[]
    lastUpdated: Date | null
    loading: boolean
    error: string | null
  }>({
    performance: {},
    health: {},
    alerts: [],
    lastUpdated: null,
    loading: false,
    error: null
  })

  // Departments state
  const departments = ref<{
    data: any[]
    loading: boolean
    error: string | null
  }>({
    data: [],
    loading: false,
    error: null
  })

  // Roles state
  const roles = ref<{
    data: any[]
    loading: boolean
    error: string | null
  }>({
    data: [],
    loading: false,
    error: null
  })

  // Selected items for bulk operations
  const selectedUsers = ref<string[]>([])
  const selectedEnrollments = ref<string[]>([])

  // Getters
  const filteredUsers = computed(() => {
    if (!users.value.filters || Object.keys(users.value.filters).length === 0) {
      return users.value.data
    }
    
    return users.value.data.filter((user: any) => {
      return Object.entries(users.value.filters).every(([key, value]) => {
        if (!value) return true
        
        const userValue = user[key]
        if (typeof value === 'string') {
          return userValue?.toString().toLowerCase().includes(value.toLowerCase())
        }
        return userValue === value
      })
    })
  })

  const coursesByDepartment = computed(() => {
    const grouped: Record<string, any[]> = {}
    courses.value.data.forEach((course: any) => {
      const dept = course.department || 'Other'
      if (!grouped[dept]) grouped[dept] = []
      grouped[dept].push(course)
    })
    return grouped
  })

  const enrollmentStatistics = computed(() => {
    const stats = {
      totalCurrent: enrollments.value.current.length,
      totalHistorical: enrollments.value.historical.length,
      byStatus: {} as Record<string, number>,
      bySemester: {} as Record<string, number>
    }
    
    enrollments.value.current.forEach((enrollment: any) => {
      stats.byStatus[enrollment.status] = (stats.byStatus[enrollment.status] || 0) + 1
      stats.bySemester[enrollment.semester] = (stats.bySemester[enrollment.semester] || 0) + 1
    })
    
    return stats
  })

  const systemHealthStatus = computed(() => {
    const health = systemMetrics.value.health
    if (!health || Object.keys(health).length === 0) return 'unknown'
    
    const criticalIssues = systemMetrics.value.alerts.filter(alert => alert.severity === 'critical').length
    const highIssues = systemMetrics.value.alerts.filter(alert => alert.severity === 'high').length
    
    if (criticalIssues > 0) return 'critical'
    if (highIssues > 0) return 'warning'
    if (health.overallStatus === 'healthy') return 'healthy'
    return 'warning'
  })

  // Actions - Users
  const loadUsers = async (filters?: Record<string, any>, pagination?: Record<string, any>) => {
    users.value.loading = true
    users.value.error = null
    
    try {
      if (filters) users.value.filters = { ...filters }
      if (pagination) users.value.pagination = { ...users.value.pagination, ...pagination }
      
      const response = await AdminApiService.users.getAll(users.value.filters, users.value.pagination)
      
      if (response.success && response.data) {
        users.value.data = response.data.data
        users.value.totalCount = response.data.total
        users.value.pagination.totalItems = response.data.total
        users.value.pagination.totalPages = Math.ceil(response.data.total / users.value.pagination.pageSize)
      } else {
        throw new Error(response.message || 'Failed to load users')
      }
    } catch (error: any) {
      users.value.error = error.message
      console.error('Error loading users:', error)
    } finally {
      users.value.loading = false
    }
  }

  const createUser = async (userData: any) => {
    try {
      const response = await AdminApiService.users.create(userData)
      
      if (response.success && response.data) {
        // Refresh users list
        await loadUsers()
        return { success: true, data: response.data }
      } else {
        throw new Error(response.message || 'Failed to create user')
      }
    } catch (error: any) {
      return { success: false, error: error.message }
    }
  }

  const updateUser = async (id: string, userData: any) => {
    try {
      const response = await AdminApiService.users.update(id, userData)
      
      if (response.success && response.data) {
        // Update user in current data
        const userIndex = users.value.data.findIndex((user: any) => user.id === id)
        if (userIndex !== -1) {
          users.value.data[userIndex] = { ...users.value.data[userIndex], ...response.data }
        }
        return { success: true, data: response.data }
      } else {
        throw new Error(response.message || 'Failed to update user')
      }
    } catch (error: any) {
      return { success: false, error: error.message }
    }
  }

  const deleteUser = async (id: string) => {
    try {
      const response = await AdminApiService.users.delete(id)
      
      if (response.success) {
        // Remove user from current data
        users.value.data = users.value.data.filter((user: any) => user.id !== id)
        users.value.totalCount -= 1
        return { success: true }
      } else {
        throw new Error(response.message || 'Failed to delete user')
      }
    } catch (error: any) {
      return { success: false, error: error.message }
    }
  }

  const bulkUserOperation = async (operation: string, userIds: string[], params?: Record<string, any>): Promise<BulkOperationResult> => {
    try {
      const response = await AdminApiService.users.bulkOperation(operation, userIds, params)
      
      if (response.success && response.data) {
        // Refresh users list after bulk operation
        await loadUsers()
        return response.data
      } else {
        throw new Error(response.message || 'Bulk operation failed')
      }
    } catch (error: any) {
      return {
        success: 0,
        failed: userIds.length,
        errors: [error.message],
        details: {
          successIds: [],
          failedIds: userIds,
          warnings: []
        }
      }
    }
  }

  // Actions - Courses
  const loadCourses = async (departmentId?: string) => {
    courses.value.loading = true
    courses.value.error = null
    
    try {
      const filters = departmentId ? { departmentId } : undefined
      const response = await AdminApiService.academic.getCourses(filters)
      
      if (response.success && response.data) {
        courses.value.data = response.data.data
        courses.value.totalCount = response.data.total
        
        // Group by department
        courses.value.byDepartment = {}
        response.data.data.forEach((course: any) => {
          const dept = course.department || 'Other'
          if (!courses.value.byDepartment[dept]) {
            courses.value.byDepartment[dept] = []
          }
          courses.value.byDepartment[dept].push(course)
        })
      } else {
        throw new Error(response.message || 'Failed to load courses')
      }
    } catch (error: any) {
      courses.value.error = error.message
      console.error('Error loading courses:', error)
    } finally {
      courses.value.loading = false
    }
  }

  // Actions - Enrollments
  const loadEnrollmentData = async (semesterId?: string) => {
    enrollments.value.loading = true
    enrollments.value.error = null
    
    try {
      const filters = semesterId ? { semesterId } : undefined
      const response = await AdminApiService.academic.getEnrollments(filters)
      
      if (response.success && response.data) {
        enrollments.value.current = response.data.data
        
        // Calculate statistics
        const stats: Record<string, any> = {
          total: response.data.data.length,
          byStatus: {},
          bySemester: {},
          byDepartment: {}
        }
        
        response.data.data.forEach((enrollment: any) => {
          stats.byStatus[enrollment.status] = (stats.byStatus[enrollment.status] || 0) + 1
          stats.bySemester[enrollment.semester] = (stats.bySemester[enrollment.semester] || 0) + 1
          stats.byDepartment[enrollment.department] = (stats.byDepartment[enrollment.department] || 0) + 1
        })
        
        enrollments.value.statistics = stats
      } else {
        throw new Error(response.message || 'Failed to load enrollment data')
      }
    } catch (error: any) {
      enrollments.value.error = error.message
      console.error('Error loading enrollment data:', error)
    } finally {
      enrollments.value.loading = false
    }
  }

  const bulkEnrollmentOperation = async (operations: any[]): Promise<{ success: number; failed: number }> => {
    try {
      const response = await AdminApiService.academic.bulkEnrollment(operations)
      
      if (response.success && response.data) {
        // Refresh enrollment data
        await loadEnrollmentData()
        return response.data
      } else {
        throw new Error(response.message || 'Bulk enrollment operation failed')
      }
    } catch (error: any) {
      return {
        success: 0,
        failed: operations.length
      }
    }
  }

  // Actions - System Metrics
  const refreshSystemMetrics = async () => {
    systemMetrics.value.loading = true
    systemMetrics.value.error = null
    
    try {
      const [healthResponse, metricsResponse] = await Promise.all([
        AdminApiService.system.getHealth(),
        AdminApiService.system.getMetrics()
      ])
      
      if (healthResponse.success && healthResponse.data) {
        systemMetrics.value.health = healthResponse.data
      }
      
      if (metricsResponse.success && metricsResponse.data) {
        systemMetrics.value.performance = metricsResponse.data
      }
      
      // Mock alerts for now - in real implementation, these would come from the API
      systemMetrics.value.alerts = [
        {
          id: '1',
          type: 'warning',
          title: 'High Memory Usage',
          message: 'System memory usage is at 85%',
          timestamp: new Date(),
          severity: 'medium',
          category: 'system',
          acknowledged: false
        }
      ]
      
      systemMetrics.value.lastUpdated = new Date()
    } catch (error: any) {
      systemMetrics.value.error = error.message
      console.error('Error refreshing system metrics:', error)
    } finally {
      systemMetrics.value.loading = false
    }
  }

  // Actions - Departments
  const loadDepartments = async () => {
    departments.value.loading = true
    departments.value.error = null
    
    try {
      const response = await AdminApiService.academic.getDepartments()
      
      if (response.success && response.data) {
        departments.value.data = response.data
      } else {
        throw new Error(response.message || 'Failed to load departments')
      }
    } catch (error: any) {
      departments.value.error = error.message
      console.error('Error loading departments:', error)
    } finally {
      departments.value.loading = false
    }
  }

  // Actions - Roles
  const loadRoles = async () => {
    roles.value.loading = true
    roles.value.error = null
    
    try {
      const response = await AdminApiService.roles.getAll()
      
      if (response.success && response.data) {
        roles.value.data = response.data
      } else {
        throw new Error(response.message || 'Failed to load roles')
      }
    } catch (error: any) {
      roles.value.error = error.message
      console.error('Error loading roles:', error)
    } finally {
      roles.value.loading = false
    }
  }

  // Selection management
  const selectUser = (userId: string) => {
    if (!selectedUsers.value.includes(userId)) {
      selectedUsers.value.push(userId)
    }
  }

  const deselectUser = (userId: string) => {
    selectedUsers.value = selectedUsers.value.filter(id => id !== userId)
  }

  const selectAllUsers = () => {
    selectedUsers.value = users.value.data.map((user: any) => user.id)
  }

  const deselectAllUsers = () => {
    selectedUsers.value = []
  }

  const isUserSelected = (userId: string): boolean => {
    return selectedUsers.value.includes(userId)
  }

  // Clear methods
  const clearUsersData = () => {
    users.value.data = []
    users.value.totalCount = 0
    users.value.filters = {}
    users.value.pagination = {
      page: 1,
      pageSize: 50,
      totalPages: 0,
      totalItems: 0
    }
    selectedUsers.value = []
  }

  const clearCoursesData = () => {
    courses.value.data = []
    courses.value.totalCount = 0
    courses.value.byDepartment = {}
  }

  const clearAllData = () => {
    clearUsersData()
    clearCoursesData()
    enrollments.value = {
      current: [],
      historical: [],
      statistics: {},
      loading: false,
      error: null
    }
    systemMetrics.value = {
      performance: {},
      health: {},
      alerts: [],
      lastUpdated: null,
      loading: false,
      error: null
    }
    selectedEnrollments.value = []
  }

  return {
    // State
    users: computed(() => users.value),
    courses: computed(() => courses.value),
    enrollments: computed(() => enrollments.value),
    systemMetrics: computed(() => systemMetrics.value),
    departments: computed(() => departments.value),
    roles: computed(() => roles.value),
    selectedUsers: computed(() => selectedUsers.value),
    selectedEnrollments: computed(() => selectedEnrollments.value),
    
    // Getters
    filteredUsers,
    coursesByDepartment,
    enrollmentStatistics,
    systemHealthStatus,
    
    // Actions
    loadUsers,
    createUser,
    updateUser,
    deleteUser,
    bulkUserOperation,
    loadCourses,
    loadEnrollmentData,
    bulkEnrollmentOperation,
    refreshSystemMetrics,
    loadDepartments,
    loadRoles,
    
    // Selection management
    selectUser,
    deselectUser,
    selectAllUsers,
    deselectAllUsers,
    isUserSelected,
    
    // Clear methods
    clearUsersData,
    clearCoursesData,
    clearAllData
  }
})