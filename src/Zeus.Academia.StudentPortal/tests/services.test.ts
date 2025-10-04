import { describe, it, expect, beforeEach, vi } from 'vitest'
import { AuthService } from '@/services/AuthService'
import { ApiService } from '@/services/ApiService'
import type { LoginRequest, LoginResponse, Student } from '@/types'

// Mock localStorage
const localStorageMock = {
  getItem: vi.fn(),
  setItem: vi.fn(),
  removeItem: vi.fn(),
  clear: vi.fn(),
}
global.localStorage = localStorageMock as any

describe('Service Layer Tests', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('AuthService', () => {
    it('should handle token storage', () => {
      const testToken = 'test-jwt-token'
      
      AuthService.setToken(testToken)
      expect(localStorage.setItem).toHaveBeenCalledWith('zeus_token', testToken)
      
      localStorageMock.getItem.mockReturnValue(testToken)
      expect(AuthService.getToken()).toBe(testToken)
      
      AuthService.clearToken()
      expect(localStorage.removeItem).toHaveBeenCalledWith('zeus_token')
      expect(localStorage.removeItem).toHaveBeenCalledWith('zeus_refresh_token')
    })

    it('should handle user login', async () => {
      const loginRequest: LoginRequest = {
        email: 'student@zeus.edu',
        password: 'password123'
      }

      const mockResponse: LoginResponse = {
        token: 'jwt-token',
        refreshToken: 'refresh-token',
        student: {
          id: '1',
          email: 'student@zeus.edu',
          firstName: 'John',
          lastName: 'Doe',
          studentId: 'STU001',
          enrollmentDate: '2024-01-15'
        },
        expiresAt: '2024-12-31T23:59:59Z'
      }

      // Mock ApiService.post
      vi.spyOn(ApiService, 'post').mockResolvedValue({
        success: true,
        data: mockResponse
      })

      const result = await AuthService.login(loginRequest)
      
      expect(result.success).toBe(true)
      expect(result.data).toEqual(mockResponse)
      expect(ApiService.post).toHaveBeenCalledWith('/auth/login', loginRequest)
    })

    it('should handle user logout', async () => {
      vi.spyOn(ApiService, 'post').mockResolvedValue({
        success: true
      })

      await AuthService.logout()
      
      expect(localStorage.removeItem).toHaveBeenCalledWith('zeus_token')
      expect(localStorage.removeItem).toHaveBeenCalledWith('zeus_refresh_token')
      expect(ApiService.post).toHaveBeenCalledWith('/auth/logout')
    })

    it('should get current user', async () => {
      const mockStudent: Student = {
        id: '1',
        email: 'student@zeus.edu',
        firstName: 'John',
        lastName: 'Doe',
        studentId: 'STU001',
        enrollmentDate: '2024-01-15'
      }

      vi.spyOn(ApiService, 'get').mockResolvedValue({
        success: true,
        data: mockStudent
      })

      const result = await AuthService.getCurrentUser()
      
      expect(result.success).toBe(true)
      expect(result.data).toEqual(mockStudent)
      expect(ApiService.get).toHaveBeenCalledWith('/auth/me')
    })
  })

  describe('ApiService', () => {
    it('should have axios instance configured', () => {
      expect(ApiService.axiosInstance).toBeDefined()
      expect(ApiService.axiosInstance.defaults.baseURL).toBeDefined()
    })

    it('should add authorization header when token exists', () => {
      const testToken = 'test-token'
      localStorageMock.getItem.mockReturnValue(testToken)
      
      // Test request interceptor
      const config = { headers: {} }
      const interceptor = ApiService.axiosInstance.interceptors.request.handlers[0]
      
      if (interceptor && interceptor.fulfilled) {
        const result = interceptor.fulfilled(config)
        expect(result.headers.Authorization).toBe(`Bearer ${testToken}`)
      }
    })

    it('should handle API responses', async () => {
      const mockData = { id: 1, name: 'Test' }
      
      vi.spyOn(ApiService.axiosInstance, 'get').mockResolvedValue({
        data: mockData,
        status: 200,
        statusText: 'OK',
        headers: {},
        config: {}
      })

      const result = await ApiService.get('/test')
      
      expect(result.success).toBe(true)
      expect(result.data).toEqual(mockData)
    })

    it('should handle API errors', async () => {
      const errorMessage = 'Network Error'
      
      vi.spyOn(ApiService.axiosInstance, 'get').mockRejectedValue(new Error(errorMessage))

      const result = await ApiService.get('/test')
      
      expect(result.success).toBe(false)
      expect(result.message).toBe(errorMessage)
    })
  })
})