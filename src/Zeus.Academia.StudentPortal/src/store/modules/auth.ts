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
    currentStudent: (state: AuthState) => state.student, // Alias for template consistency
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

    async initializeAuth({ commit }: any): Promise<void> {
      // In development/demo mode, clear any stored tokens and use localStorage for demo auth
      const isDevelopment = import.meta.env.DEV || import.meta.env.NODE_ENV === 'development'
      
      if (isDevelopment) {
        // Clear any stored tokens to prevent API calls
        localStorage.removeItem('zeus_token')
        localStorage.removeItem('zeus_refresh_token')
        
        // Auto-enable demo auth for testing purposes
        commit('SET_STUDENT', {
          id: '1',
          firstName: 'John',
          lastName: 'Doe',
          email: 'john.doe@zeus.edu',
          studentId: 'STU-2024-001', // Updated to match API response
          gpa: 3.75, // Updated to match API response
          enrollmentDate: '2022-08-15', // Updated to match API response
          phone: '(555) 123-4567',
          dateOfBirth: '1998-05-15', // Updated to match API response
          address: {
            street: '123 College Ave',
            city: 'University City',
            state: 'CA',
            zipCode: '90210',
            country: 'USA'
          }
        })
        commit('SET_AUTHENTICATED', true)
        
        // Also check for localStorage demo mode flag for normal login flow
        const localAuth = localStorage.getItem('zeus_auth')
        const localUser = localStorage.getItem('zeus_user')
        
        if (localAuth === 'true' && localUser) {
          // Override with localStorage data if present
          try {
            const userData = JSON.parse(localUser)
            // Keep the updated demo data but use localStorage name if provided
            commit('SET_STUDENT', {
              id: userData.id || '1',
              firstName: userData.name?.split(' ')[0] || 'John',
              lastName: userData.name?.split(' ')[1] || 'Doe',
              email: userData.email || 'john.doe@zeus.edu',
              studentId: 'STU-2024-001', // Always use the correct student ID
              gpa: 3.75,
              enrollmentDate: '2022-08-15',
              phone: '(555) 123-4567',
              dateOfBirth: '1998-05-15',
              address: {
                street: '123 College Ave',
                city: 'University City',
                state: 'CA',
                zipCode: '90210',
                country: 'USA'
              }
            })
          } catch (error) {
            console.error('Error parsing localStorage user data:', error)
            localStorage.removeItem('zeus_auth')
            localStorage.removeItem('zeus_user')
          }
        }
      }
    }
  }
}