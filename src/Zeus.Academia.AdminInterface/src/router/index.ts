import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import type { AdminPermission } from '@/types'

// Lazy load components for better performance
const LoginView = () => import('@/views/LoginView.vue')
const DashboardView = () => import('@/views/DashboardView.vue')
const UserManagementView = () => import('@/views/UserManagementView.vue')
const SystemConfigurationView = () => import('@/views/SystemConfigurationView.vue')
const AcademicManagementView = () => import('@/views/AcademicManagementView.vue')
const FinancialManagementView = () => import('@/views/FinancialManagementView.vue')
const SystemMonitoringView = () => import('@/views/SystemMonitoringView.vue')
const AuditLogsView = () => import('@/views/AuditLogsView.vue')
const ProfileView = () => import('@/views/ProfileView.vue')
const NotFoundView = () => import('@/views/NotFoundView.vue')
const UnauthorizedView = () => import('@/views/UnauthorizedView.vue')

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    redirect: '/dashboard'
  },
  {
    path: '/login',
    name: 'Login',
    component: LoginView,
    meta: {
      requiresAuth: false,
      title: 'Login - Zeus Academia Admin'
    }
  },
  {
    path: '/dashboard',
    name: 'Dashboard',
    component: DashboardView,
    meta: {
      requiresAuth: true,
      permissions: [],
      title: 'Dashboard - Zeus Academia Admin'
    }
  },
  {
    path: '/users',
    name: 'UserManagement',
    component: UserManagementView,
    meta: {
      requiresAuth: true,
      permissions: [], // Temporarily removed user_management requirement for demo
      title: 'User Management - Zeus Academia Admin',
      breadcrumb: [
        { name: 'Dashboard', path: '/dashboard' },
        { name: 'User Management', path: '/users' }
      ]
    }
  },
  {
    path: '/system',
    name: 'SystemConfiguration',
    component: SystemConfigurationView,
    meta: {
      requiresAuth: true,
      permissions: ['system_configuration'],
      roles: ['system_admin'],
      title: 'System Configuration - Zeus Academia Admin',
      breadcrumb: [
        { name: 'Dashboard', path: '/dashboard' },
        { name: 'System Configuration', path: '/system' }
      ]
    }
  },
  {
    path: '/academic',
    name: 'AcademicManagement',
    component: AcademicManagementView,
    meta: {
      requiresAuth: true,
      permissions: ['academic_calendar', 'enrollment_management', 'academic_records'],
      title: 'Academic Management - Zeus Academia Admin',
      breadcrumb: [
        { name: 'Dashboard', path: '/dashboard' },
        { name: 'Academic Management', path: '/academic' }
      ]
    }
  },
  {
    path: '/financial',
    name: 'FinancialManagement',
    component: FinancialManagementView,
    meta: {
      requiresAuth: true,
      permissions: ['financial_management'],
      title: 'Financial Management - Zeus Academia Admin',
      breadcrumb: [
        { name: 'Dashboard', path: '/dashboard' },
        { name: 'Financial Management', path: '/financial' }
      ]
    }
  },
  {
    path: '/monitoring',
    name: 'SystemMonitoring',
    component: SystemMonitoringView,
    meta: {
      requiresAuth: true,
      permissions: ['system_monitoring'],
      title: 'System Monitoring - Zeus Academia Admin',
      breadcrumb: [
        { name: 'Dashboard', path: '/dashboard' },
        { name: 'System Monitoring', path: '/monitoring' }
      ]
    }
  },
  {
    path: '/audit',
    name: 'AuditLogs',
    component: AuditLogsView,
    meta: {
      requiresAuth: true,
      permissions: ['audit_access'],
      title: 'Audit Logs - Zeus Academia Admin',
      breadcrumb: [
        { name: 'Dashboard', path: '/dashboard' },
        { name: 'Audit Logs', path: '/audit' }
      ]
    }
  },
  {
    path: '/profile',
    name: 'Profile',
    component: ProfileView,
    meta: {
      requiresAuth: true,
      permissions: [],
      title: 'Profile - Zeus Academia Admin',
      breadcrumb: [
        { name: 'Dashboard', path: '/dashboard' },
        { name: 'Profile', path: '/profile' }
      ]
    }
  },
  {
    path: '/unauthorized',
    name: 'Unauthorized',
    component: UnauthorizedView,
    meta: {
      requiresAuth: false,
      title: 'Unauthorized - Zeus Academia Admin'
    }
  },
  {
    path: '/:pathMatch(.*)*',
    name: 'NotFound',
    component: NotFoundView,
    meta: {
      requiresAuth: false,
      title: 'Page Not Found - Zeus Academia Admin'
    }
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior(to, from, savedPosition) {
    if (savedPosition) {
      return savedPosition
    } else {
      return { top: 0 }
    }
  }
})

// Navigation guards
router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore()
  
  // Update page title
  if (to.meta?.title) {
    document.title = to.meta.title as string
  }
  
  // Check if route requires authentication
  if (to.meta?.requiresAuth) {
    if (!authStore.isAuthenticated) {
      next({
        path: '/login',
        query: { redirect: to.fullPath }
      })
      return
    }
    
    // Check session timeout
    if (authStore.checkSessionTimeout()) {
      await authStore.logout()
      next({
        path: '/login',
        query: { 
          redirect: to.fullPath,
          message: 'Session expired. Please log in again.'
        }
      })
      return
    }
    
    // Update last activity
    authStore.updateLastActivity()
    
    // Check role requirements
    if (to.meta?.roles && Array.isArray(to.meta.roles)) {
      const requiredRoles = to.meta.roles as string[]
      const userRole = authStore.userRole
      
      if (!userRole || !requiredRoles.includes(userRole)) {
        next('/unauthorized')
        return
      }
    }
    
    // Check permission requirements
    if (to.meta?.permissions && Array.isArray(to.meta.permissions)) {
      const requiredPermissions = to.meta.permissions as string[]
      
      if (requiredPermissions.length > 0 && !authStore.hasAnyPermission(requiredPermissions as AdminPermission[])) {
        next('/unauthorized')
        return
      }
    }
  }
  
  // Redirect to dashboard if already authenticated and trying to access login
  if (to.path === '/login' && authStore.isAuthenticated) {
    next('/dashboard')
    return
  }
  
  next()
})

// Global error handling for navigation
router.onError((error) => {
  console.error('Router error:', error)
  
  // Handle chunk load errors (when lazy-loaded components fail to load)
  if (error.message.includes('Loading chunk')) {
    window.location.reload()
  }
})

export default router