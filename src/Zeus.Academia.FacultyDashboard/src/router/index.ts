import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'

// Import views (will be created shortly)
import LoginView from '@/views/LoginView.vue'
import DashboardView from '@/views/DashboardView.vue'
import GradebookView from '@/views/GradebookView.vue'
import CoursesView from '@/views/CoursesView.vue'
import UnauthorizedView from '@/views/UnauthorizedView.vue'
import NotFoundView from '@/views/NotFoundView.vue'

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
      title: 'Faculty Login'
    }
  },
  {
    path: '/dashboard',
    name: 'Dashboard',
    component: DashboardView,
    meta: {
      requiresAuth: true,
      title: 'Faculty Dashboard'
    }
  },
  {
    path: '/courses',
    name: 'Courses',
    component: CoursesView,
    meta: {
      requiresAuth: true,
      title: 'My Courses'
    }
  },
  {
    path: '/gradebook/:courseId?',
    name: 'Gradebook',
    component: GradebookView,
    props: true,
    meta: {
      requiresAuth: true,
      requiresPermission: 'manage_grades',
      title: 'Gradebook'
    }
  },
  {
    path: '/unauthorized',
    name: 'Unauthorized',
    component: UnauthorizedView,
    meta: {
      title: 'Unauthorized Access'
    }
  },
  {
    path: '/:pathMatch(.*)*',
    name: 'NotFound',
    component: NotFoundView,
    meta: {
      title: 'Page Not Found'
    }
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior(to, from, savedPosition) {
    if (savedPosition) {
      return savedPosition
    } else if (to.hash) {
      return { el: to.hash }
    } else {
      return { top: 0 }
    }
  }
})

// Global route guard for authentication and authorization
router.beforeEach((to, from, next) => {
  // Set document title
  if (to.meta?.title) {
    document.title = `${to.meta.title} - Zeus Academia Faculty Dashboard`
  } else {
    document.title = 'Zeus Academia Faculty Dashboard'
  }

  // Check authentication
  const isAuthenticated = localStorage.getItem('zeus_faculty_auth') === 'true'
  const userRole = localStorage.getItem('zeus_faculty_role')
  const userPermissions = JSON.parse(localStorage.getItem('zeus_faculty_user') || '{}').permissions || []

  // Handle routes that require authentication
  if (to.meta?.requiresAuth && !isAuthenticated) {
    next('/login')
    return
  }

  // Handle login redirect
  if (to.path === '/login' && isAuthenticated) {
    next('/dashboard')
    return
  }

  // Handle role-based access
  if (to.meta?.requiresRole && to.meta.requiresRole !== userRole) {
    next('/unauthorized')
    return
  }

  // Handle permission-based access
  if (to.meta?.requiresPermission && !userPermissions.includes(to.meta.requiresPermission)) {
    next('/unauthorized')
    return
  }

  next()
})

export default router