import { createApp } from 'vue'
import App from './App.vue'
import { store } from './store/index'
import router from './router/index'

import 'bootstrap/dist/css/bootstrap.css'
import './style.css'

// Enhanced navigation guard with store integration
router.beforeEach((to: any, _from: any, next: any) => {
  const isAuthenticated = store.getters['auth/isAuthenticated'] || localStorage.getItem('zeus_auth') === 'true'
  
  if (to.meta?.requiresAuth && !isAuthenticated) {
    next('/login')
  } else if (to.path === '/login' && isAuthenticated) {
    next('/dashboard')
  } else {
    next()
  }
})

const app = createApp(App)
app.use(store)
app.use(router)

// Initialize auth state from localStorage if available
store.dispatch('auth/initializeAuth')

app.mount('#app')