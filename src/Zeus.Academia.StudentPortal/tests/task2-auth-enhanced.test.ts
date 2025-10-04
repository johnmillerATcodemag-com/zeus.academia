import { describe, it, expect, beforeEach, vi } from 'vitest'
import { AuthService } from '../src/services/AuthService'
import { ApiService } from '../src/services/ApiService'
import type { LoginRequest, LoginResponse, ApiResponse } from '../src/types'

describe('Task 2: Enhanced Authentication System', () => {
  beforeEach(() => {
    // Clear localStorage before each test
    localStorage.clear()
    // Clear all mocks
    vi.clearAllMocks()
  })

  describe('JWT Token Management with Auto-Refresh', () => {
    it('should login with valid credentials and store tokens securely', async () => {
      const mockLoginRequest: LoginRequest = {
        email: 'student@test.com',
        password: 'password123'
      }

      const mockLoginResponse: LoginResponse = {
        token: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiZXhwIjoxNzI4MDczNjAwLCJyb2xlcyI6WyJzdHVkZW50Il19.test',
        refreshToken: 'refresh_token_123',
        student: {
          id: '1',
          email: 'student@test.com',
          firstName: 'John',
          lastName: 'Doe',
          studentId: 'STU001',
          enrollmentDate: '2023-09-01'
        },
        expiresAt: '2024-10-05T00:00:00Z'
      }

      const mockApiResponse: ApiResponse<LoginResponse> = {
        success: true,
        data: mockLoginResponse
      }

      const postSpy = vi.spyOn(ApiService, 'post')
      postSpy.mockResolvedValue(mockApiResponse)

      const result = await AuthService.login(mockLoginRequest)

      expect(result.success).toBe(true)
      expect(result.data).toEqual(mockLoginResponse)
      expect(AuthService.getToken()).toBe(mockLoginResponse.token)
      expect(AuthService.getRefreshToken()).toBe(mockLoginResponse.refreshToken)
      expect(AuthService.getStoredUser()).toEqual(mockLoginResponse.student)
      expect(postSpy).toHaveBeenCalledWith('/auth/login', mockLoginRequest)
    })

    it('should automatically refresh expired tokens', async () => {
      // Set up an expired token
      const expiredToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiZXhwIjoxNjAwMDAwMDAwfQ.test'
      const refreshToken = 'refresh_token_123'
      
      AuthService.setToken(expiredToken)
      AuthService.setRefreshToken(refreshToken)

      const mockNewToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiZXhwIjoxNzI4MDczNjAwfQ.new_test'
      const mockRefreshResponse: ApiResponse<{ token: string; refreshToken?: string }> = {
        success: true,
        data: {
          token: mockNewToken,
          refreshToken: 'new_refresh_token_123'
        }
      }

      const postSpy = vi.spyOn(ApiService, 'post')
      postSpy.mockResolvedValue(mockRefreshResponse)

      const result = await AuthService.refreshToken(refreshToken)

      expect(result.success).toBe(true)
      expect(AuthService.getToken()).toBe(mockNewToken)
      expect(AuthService.getRefreshToken()).toBe('new_refresh_token_123')
      expect(postSpy).toHaveBeenCalledWith('/auth/refresh', { refreshToken })
    })

    it('should detect token expiration correctly', () => {
      // Valid token (expires in far future - year 2030)
      const validToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiZXhwIjoxODkzNDU2MDAwfQ.test'
      AuthService.setToken(validToken)
      expect(AuthService.isAuthenticated()).toBe(true)
      expect(AuthService.isTokenExpired()).toBe(false)

      // Expired token (expired in 2020)
      const expiredToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiZXhwIjoxNjAwMDAwMDAwfQ.test'
      AuthService.setToken(expiredToken)
      expect(AuthService.isAuthenticated()).toBe(false)
      expect(AuthService.isTokenExpired()).toBe(true)
    })

    it('should maintain session persistence across browser sessions', () => {
      const token = 'test_token'
      const refreshToken = 'test_refresh_token'
      const user = {
        id: '1',
        email: 'test@example.com',
        firstName: 'Test',
        lastName: 'User',
        studentId: 'STU001',
        enrollmentDate: '2023-09-01'
      }

      AuthService.setToken(token)
      AuthService.setRefreshToken(refreshToken)
      localStorage.setItem('zeus_user', JSON.stringify(user))

      // Simulate browser reload by creating new AuthService instance
      expect(AuthService.getToken()).toBe(token)
      expect(AuthService.getRefreshToken()).toBe(refreshToken)
      expect(AuthService.getStoredUser()).toEqual(user)
    })

    it('should handle token refresh failure gracefully', async () => {
      const refreshToken = 'invalid_refresh_token'
      
      const mockErrorResponse: ApiResponse<any> = {
        success: false,
        message: 'Invalid refresh token',
        errors: ['Token has expired']
      }

      const postSpy = vi.spyOn(ApiService, 'post')
      postSpy.mockResolvedValue(mockErrorResponse)

      const result = await AuthService.refreshToken(refreshToken)

      expect(result.success).toBe(false)
      expect(result.message).toBe('Invalid refresh token')
      // Token should not be updated on failure
      expect(AuthService.getToken()).toBeNull()
    })

    it('should extract user roles and permissions from JWT token', () => {
      const tokenWithRoles = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiZXhwIjoxNzI4MDczNjAwLCJyb2xlcyI6WyJzdHVkZW50IiwidGVzdGVyIl19.test'
      
      AuthService.setToken(tokenWithRoles)

      expect(AuthService.getUserRoles()).toEqual(['student', 'tester'])
      expect(AuthService.hasRole('student')).toBe(true)
      expect(AuthService.hasRole('admin')).toBe(false)
      expect(AuthService.hasAnyRole(['student', 'admin'])).toBe(true)
      expect(AuthService.hasAnyRole(['admin', 'faculty'])).toBe(false)
    })

    it('should securely logout and clear all authentication data', async () => {
      // Set up authenticated state
      AuthService.setToken('test_token')
      AuthService.setRefreshToken('test_refresh_token')
      localStorage.setItem('zeus_user', JSON.stringify({ id: '1', name: 'Test User' }))

      const mockLogoutResponse: ApiResponse<void> = {
        success: true
      }

      const postSpy = vi.spyOn(ApiService, 'post')
      postSpy.mockResolvedValue(mockLogoutResponse)

      const result = await AuthService.logout()

      expect(result.success).toBe(true)
      expect(AuthService.getToken()).toBeNull()
      expect(AuthService.getRefreshToken()).toBeNull()
      expect(AuthService.getStoredUser()).toBeNull()
      expect(postSpy).toHaveBeenCalledWith('/auth/logout')
    })

    it('should clear local data even if logout API call fails', async () => {
      // Set up authenticated state
      AuthService.setToken('test_token')
      AuthService.setRefreshToken('test_refresh_token')
      localStorage.setItem('zeus_user', JSON.stringify({ id: '1', name: 'Test User' }))

      // Mock API failure
      const postSpy = vi.spyOn(ApiService, 'post')
      postSpy.mockRejectedValue(new Error('Network error'))

      const result = await AuthService.logout()

      // Should still succeed and clear local data
      expect(result.success).toBe(true)
      expect(AuthService.getToken()).toBeNull()
      expect(AuthService.getRefreshToken()).toBeNull()  
      expect(AuthService.getStoredUser()).toBeNull()
    })
  })
})