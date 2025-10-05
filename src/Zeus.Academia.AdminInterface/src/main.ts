import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router/index'

// Bootstrap CSS and JavaScript
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap'

// Create Pinia store
const pinia = createPinia()

// Create Vue app
const app = createApp(App)

// Use plugins
app.use(pinia)
app.use(router)

// Enhanced navigation guard with administrative authentication
router.beforeEach((to, _from, next) => {
  const isAuthenticated = localStorage.getItem('zeus_admin_token') && localStorage.getItem('zeus_admin_user')
  
  if (to.meta?.requiresAuth && !isAuthenticated) {
    next('/login')
  } else if (to.path === '/login' && isAuthenticated) {
    next('/dashboard')
  } else {
    next()
  }
})

// Global error handler
app.config.errorHandler = (err, instance, info) => {
  console.error('Global error:', err)
  console.error('Component:', instance)
  console.error('Info:', info)
}

// Mount the app
app.mount('#app')