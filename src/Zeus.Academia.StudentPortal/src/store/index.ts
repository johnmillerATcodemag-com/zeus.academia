import { createStore } from 'vuex'
import { authModule } from './modules/auth'
import { coursesModule } from './modules/courses'
import type { LoadingState } from '../types'

// Root state interface
export interface State {
  loading: LoadingState
  notification: {
    show: boolean
    type: 'info' | 'success' | 'warning' | 'error'
    title: string
    message: string
  } | null
}

// Root state
const state: State = {
  loading: {},
  notification: null
}

// Root getters
const getters = {
  isLoading: (state: State) => (key: string) => state.loading[key] || false,
  hasAnyLoading: (state: State) => Object.values(state.loading).some(loading => loading),
  notification: (state: State) => state.notification
}

// Root mutations
const mutations = {
  SET_LOADING(state: State, payload: { [key: string]: boolean } | boolean) {
    if (typeof payload === 'boolean') {
      // Set global loading state
      state.loading.global = payload
    } else {
      // Set specific loading states
      Object.assign(state.loading, payload)
    }
  },

  CLEAR_LOADING(state: State) {
    state.loading = {}
  },

  SET_NOTIFICATION(state: State, notification: State['notification']) {
    state.notification = notification
  },

  CLEAR_NOTIFICATION(state: State) {
    state.notification = null
  }
}

// Root actions
const actions = {
  setLoading({ commit }: any, payload: { [key: string]: boolean } | boolean) {
    commit('SET_LOADING', payload)
  },

  clearLoading({ commit }: any) {
    commit('CLEAR_LOADING')
  },

  showNotification({ commit }: any, notification: { 
    type: 'info' | 'success' | 'warning' | 'error', 
    title: string, 
    message: string 
  }) {
    commit('SET_NOTIFICATION', { ...notification, show: true })
  },

  clearNotification({ commit }: any) {
    commit('CLEAR_NOTIFICATION')
  },

  // Global initialization action
  async initializeApp({ dispatch }: any) {
    try {
      dispatch('setLoading', { app: true })
      
      // Initialize authentication
      await dispatch('auth/initializeAuth')
      
      // If authenticated, load initial data
      const isAuthenticated = (this as any).getters['auth/isAuthenticated']
      if (isAuthenticated) {
        await Promise.all([
          dispatch('courses/fetchCourses'),
          dispatch('courses/fetchEnrollments')
        ])
      }
      
    } catch (error) {
      console.error('Failed to initialize app:', error)
    } finally {
      dispatch('setLoading', { app: false })
    }
  },

  // Global error handling
  handleError({ dispatch }: any, error: { message: string; code?: string }) {
    console.error('Global error:', error)
    
    // Handle specific error codes
    if (error.code === 'UNAUTHORIZED' || error.code === '401') {
      dispatch('auth/logout')
    }
  }
}

// Create and export the store
export const store = createStore({
  state,
  getters,
  mutations,
  actions,
  modules: {
    auth: authModule,
    courses: coursesModule
  },
  
  // Enable strict mode in development
  strict: process.env.NODE_ENV !== 'production'
})

// Note: Hot module replacement is handled automatically by Vite

export default store