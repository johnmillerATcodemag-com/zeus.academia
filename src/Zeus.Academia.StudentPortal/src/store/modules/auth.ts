import { AuthService } from '../../services/AuthService'
import type { AuthState, Student, LoginRequest, ApiResponse, LoginResponse } from '../../types'

export const authModule = {
  namespaced: true,
  
  state: (): AuthState => ({
    isAuthenticated: false,
    student: null,
    token: null,
    refreshToken: null
  }),

  getters: {
    isAuthenticated: (state: AuthState) => state.isAuthenticated,
    student: (state: AuthState) => state.student,
    studentName: (state: AuthState) => {
      return state.student ? `${state.student.firstName} ${state.student.lastName}` : 'Student'
    },
    hasToken: (state: AuthState) => !!state.token
  },

  mutations: {
    SET_AUTHENTICATED(state: AuthState, value: boolean) {
      state.isAuthenticated = value
    },

    SET_STUDENT(state: AuthState, student: Student | null) {
      state.student = student
    },

    SET_TOKEN(state: AuthState, token: string | null) {
      state.token = token
    },

    SET_REFRESH_TOKEN(state: AuthState, refreshToken: string | null) {
      state.refreshToken = refreshToken
    },

    CLEAR_AUTH(state: AuthState) {
      state.isAuthenticated = false
      state.student = null
      state.token = null
      state.refreshToken = null
    }
  },

  actions: {
    async login({ commit, dispatch }: any, loginRequest: LoginRequest): Promise<ApiResponse<LoginResponse>> {
      try {
        dispatch('setLoading', true, { root: true })
        
        const response = await AuthService.login(loginRequest)
        
        if (response.success && response.data) {
          const { token, refreshToken, student } = response.data
          
          // Store tokens
          AuthService.setToken(token)
          AuthService.setRefreshToken(refreshToken)
          
          // Update state
          commit('SET_TOKEN', token)
          commit('SET_REFRESH_TOKEN', refreshToken)
          commit('SET_STUDENT', student)
          commit('SET_AUTHENTICATED', true)
        }
        
        return response
      } catch (error) {
        console.error('Login error:', error)
        return {
          success: false,
          message: 'Login failed. Please try again.'
        }
      } finally {
        dispatch('setLoading', false, { root: true })
      }
    },

    async logout({ commit, dispatch }: any): Promise<void> {
      try {
        dispatch('setLoading', true, { root: true })
        
        await AuthService.logout()
        
        // Clear tokens
        AuthService.clearToken()
        
        // Clear state
        commit('CLEAR_AUTH')
      } catch (error) {
        console.error('Logout error:', error)
        // Clear local state even if API call fails
        AuthService.clearToken()
        commit('CLEAR_AUTH')
      } finally {
        dispatch('setLoading', false, { root: true })
      }
    },

    async getCurrentUser({ commit, dispatch }: any): Promise<ApiResponse<Student>> {
      try {
        dispatch('setLoading', true, { root: true })
        
        const response = await AuthService.getCurrentUser()
        
        if (response.success && response.data) {
          commit('SET_STUDENT', response.data)
          commit('SET_AUTHENTICATED', true)
        }
        
        return response
      } catch (error) {
        console.error('Get current user error:', error)
        return {
          success: false,
          message: 'Failed to get user information.'
        }
      } finally {
        dispatch('setLoading', false, { root: true })
      }
    },

    async updateProfile({ commit, dispatch }: any, profileData: Partial<Student>): Promise<ApiResponse<Student>> {
      try {
        dispatch('setLoading', true, { root: true })
        
        const response = await AuthService.updateProfile(profileData)
        
        if (response.success && response.data) {
          commit('SET_STUDENT', response.data)
        }
        
        return response
      } catch (error) {
        console.error('Update profile error:', error)
        return {
          success: false,
          message: 'Failed to update profile.'
        }
      } finally {
        dispatch('setLoading', false, { root: true })
      }
    },

    async refreshToken({ commit, state, dispatch }: any): Promise<boolean> {
      try {
        if (!state.refreshToken) {
          return false
        }

        const response = await AuthService.refreshToken(state.refreshToken)
        
        if (response.success && response.data) {
          const { token, refreshToken } = response.data
          
          AuthService.setToken(token)
          if (refreshToken) {
            AuthService.setRefreshToken(refreshToken)
            commit('SET_REFRESH_TOKEN', refreshToken)
          }
          
          commit('SET_TOKEN', token)
          return true
        }
        
        return false
      } catch (error) {
        console.error('Refresh token error:', error)
        // If refresh fails, logout the user
        dispatch('logout')
        return false
      }
    },

    async initializeAuth({ commit, dispatch }: any): Promise<void> {
      // Check for existing token from AuthService
      const token = AuthService.getToken()
      const refreshToken = AuthService.getRefreshToken()
      
      // Also check localStorage for backward compatibility
      const localAuth = localStorage.getItem('zeus_auth')
      const localUser = localStorage.getItem('zeus_user')
      
      if (token) {
        commit('SET_TOKEN', token)
        commit('SET_REFRESH_TOKEN', refreshToken)
        
        // Try to get current user to validate token
        const response = await dispatch('getCurrentUser')
        
        if (!response.success) {
          // Token might be expired, try to refresh
          const refreshed = await dispatch('refreshToken')
          
          if (refreshed) {
            // Retry getting current user
            await dispatch('getCurrentUser')
          } else {
            // Refresh failed, clear everything
            dispatch('logout')
          }
        }
      } else if (localAuth === 'true' && localUser) {
        // Legacy localStorage authentication - convert to store
        try {
          const userData = JSON.parse(localUser)
          commit('SET_STUDENT', {
            id: userData.id || '1',
            firstName: userData.name?.split(' ')[0] || 'Test',
            lastName: userData.name?.split(' ')[1] || 'Student',
            email: userData.email || 'student@zeus.edu',
            studentId: 'TS123456',
            gpa: 3.85,
            major: 'Computer Science',
            year: 'Sophomore'
          })
          commit('SET_AUTHENTICATED', true)
        } catch (error) {
          console.error('Error parsing localStorage user data:', error)
          localStorage.removeItem('zeus_auth')
          localStorage.removeItem('zeus_user')
        }
      }
    }
  }
}