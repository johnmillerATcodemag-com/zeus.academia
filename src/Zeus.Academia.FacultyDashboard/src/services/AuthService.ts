import axios, { type AxiosInstance, type AxiosResponse } from 'axios'
import type { ApiResponse, LoginResponse, FacultyUser } from '@/types'

class AuthServiceClass {
  private apiClient: AxiosInstance

  constructor() {
    this.apiClient = axios.create({
      baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api',
      timeout: 10000,
      headers: {
        'Content-Type': 'application/json',
      },
    })

    // Add request interceptor for auth token
    this.apiClient.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('zeus_faculty_token')
        if (token) {
          config.headers.Authorization = `Bearer ${token}`
        }
        return config
      },
      (error) => Promise.reject(error)
    )

    // Add response interceptor for error handling
    this.apiClient.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          // Token expired or invalid
          localStorage.removeItem('zeus_faculty_auth')
          localStorage.removeItem('zeus_faculty_token')
          localStorage.removeItem('zeus_faculty_role')
          localStorage.removeItem('zeus_faculty_user')
          window.location.href = '/login'
        }
        return Promise.reject(error)
      }
    )
  }

  async login(email: string, password: string): Promise<ApiResponse<LoginResponse>> {
    try {
      const response: AxiosResponse<LoginResponse> = await this.apiClient.post('/auth/login', {
        usernameOrEmail: email,
        password,
      })

      return {
        success: true,
        data: response.data,
      }
    } catch (error: any) {
      return {
        success: false,
        data: {} as LoginResponse,
        error: error.response?.data?.message || 'Login failed',
      }
    }
  }

  async logout(token: string): Promise<ApiResponse<void>> {
    try {
      // API endpoint not implemented yet, mock successful logout
      console.log('Mock logout for token:', token)
      return { success: true, data: undefined }
    } catch (error: any) {
      return {
        success: false,
        data: undefined,
        error: error.response?.data?.message || 'Logout failed',
      }
    }
  }

  async validateToken(token: string): Promise<boolean> {
    try {
      // API endpoint not implemented yet, mock token validation
      console.log('Mock token validation for:', token)
      return token === 'mock-jwt-token-for-testing'
    } catch {
      return false
    }
  }

  async refreshToken(token: string): Promise<ApiResponse<{ token: string }>> {
    try {
      // API endpoint not implemented yet, mock token refresh
      console.log('Mock token refresh for:', token)
      return {
        success: true,
        data: { token: 'mock-refreshed-jwt-token-for-testing' },
      }
    } catch (error: any) {
      return {
        success: false,
        data: { token: '' },
        error: error.response?.data?.message || 'Token refresh failed',
      }
    }
  }

  async getCurrentUser(): Promise<ApiResponse<FacultyUser>> {
    try {
      const response: AxiosResponse<FacultyUser> = await this.apiClient.get('/auth/faculty/me')

      return {
        success: true,
        data: response.data,
      }
    } catch (error: any) {
      return {
        success: false,
        data: {} as FacultyUser,
        error: error.response?.data?.message || 'Failed to get user info',
      }
    }
  }

  async updateProfile(userData: Partial<FacultyUser>): Promise<ApiResponse<FacultyUser>> {
    try {
      const response: AxiosResponse<FacultyUser> = await this.apiClient.put('/auth/faculty/profile', userData)

      return {
        success: true,
        data: response.data,
      }
    } catch (error: any) {
      return {
        success: false,
        data: {} as FacultyUser,
        error: error.response?.data?.message || 'Profile update failed',
      }
    }
  }

  async changePassword(currentPassword: string, newPassword: string): Promise<ApiResponse<void>> {
    try {
      await this.apiClient.put('/auth/faculty/password', {
        currentPassword,
        newPassword,
      })

      return { success: true, data: undefined }
    } catch (error: any) {
      return {
        success: false,
        data: undefined,
        error: error.response?.data?.message || 'Password change failed',
      }
    }
  }
}

export const AuthService = new AuthServiceClass()