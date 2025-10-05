import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router/index'

// Bootstrap CSS and JavaScript
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap'

// Custom styles
import './styles/main.scss'

// Create Pinia store
const pinia = createPinia()

// Create Vue app
const app = createApp(App)

// Use plugins
app.use(pinia)
app.use(router)

// Enhanced navigation guard with faculty-specific authentication
router.beforeEach((to, _from, next) => {
  const isAuthenticated = localStorage.getItem('zeus_faculty_auth') === 'true'
  const userRole = localStorage.getItem('zeus_faculty_role')
  
  if (to.meta?.requiresAuth && !isAuthenticated) {
    next('/login')
  } else if (to.meta?.requiresRole && to.meta.requiresRole !== userRole) {
    next('/unauthorized')
  } else if (to.path === '/login' && isAuthenticated) {
    next('/dashboard')
  } else {
    next()
  }
})

// Mount the app
app.mount('#app')

// Development-only features
if (import.meta.env.DEV) {
  console.log('🎓 Zeus Academia Faculty Dashboard - Development Mode')
  console.log('📊 Pinia DevTools:', pinia)
  console.log('🛣️ Router:', router)
}