import { ApiService } from './ApiService'
import type { 
  LoginRequest, 
  LoginResponse, 
  Student, 
  ApiResponse,
  EmergencyContact,
  Document
} from '../types'

class AuthServiceClass {
  private readonly TOKEN_KEY = 'zeus_token'
  private readonly REFRESH_TOKEN_KEY = 'zeus_refresh_token'
  private readonly USER_KEY = 'zeus_user'

  // Login user - Updated to match backend API
  async login(loginRequest: LoginRequest): Promise<ApiResponse<LoginResponse>> {
    const response = await ApiService.post<any>('/auth/login', loginRequest)
    
    if (response.success && response.data) {
      // Backend returns: { success: true, token: "...", user: {...}, expiresAt: "...", message: "..." }
      const loginData: LoginResponse = {
        token: response.data.token,
        refreshToken: response.data.refreshToken || `refresh-${Date.now()}`,
        student: this.mapUserToStudent(response.data.user),
        expiresAt: response.data.expiresAt
      }

      // Store tokens
      this.setToken(loginData.token)
      this.setRefreshToken(loginData.refreshToken)
      
      // Store user data
      localStorage.setItem(this.USER_KEY, JSON.stringify(loginData.student))
      
      return { success: true, data: loginData }
    }
    
    return response
  }

  // Logout user
  async logout(): Promise<ApiResponse<void>> {
    try {
      // Call API to invalidate tokens on server
      await ApiService.post('/auth/logout')
    } catch (error) {
      console.warn('Logout API call failed:', error)
    } finally {
      // Always clear local storage
      this.clearToken()
      this.clearRefreshToken()
      this.clearUser()
    }

    return { success: true }
  }

  // Get current user from API - Updated to use student profile endpoint
  async getCurrentUser(): Promise<ApiResponse<Student>> {
    const response = await ApiService.get<any>('/student/profile')
    
    if (response.success && response.data) {
      const student = this.mapProfileToStudent(response.data)
      localStorage.setItem(this.USER_KEY, JSON.stringify(student))
      return { success: true, data: student }
    }
    
    return response
  }

  // Update user profile - Updated to use student profile endpoint
  async updateProfile(profileData: Partial<Student>): Promise<ApiResponse<Student>> {
    const updateData = this.mapStudentToProfileUpdate(profileData)
    const response = await ApiService.put<any>('/student/profile', updateData)
    
    if (response.success && response.data) {
      const student = this.mapProfileToStudent(response.data)
      localStorage.setItem(this.USER_KEY, JSON.stringify(student))
      return { success: true, data: student }
    }
    
    return response
  }

  // Helper method to map backend user data to frontend Student interface
  private mapUserToStudent(user: any): Student {
    return {
      id: user.id?.toString() || '1',
      email: user.email || user.username + '@academia.edu',
      firstName: user.firstName || 'Student',
      lastName: user.lastName || 'User',
      studentId: user.studentId || 'STU' + user.id,
      enrollmentDate: user.enrollmentDate || new Date().toISOString().split('T')[0],
      gpa: user.gpa || 0.0,
      phone: user.phone || '',
      dateOfBirth: user.dateOfBirth || '2000-01-01',
      address: user.address || {
        street: '',
        city: '',
        state: '',
        zipCode: '',
        country: 'United States'
      },
      emergencyContact: user.emergencyContact
    }
  }

  // Helper method to map backend profile data to frontend Student interface
  private mapProfileToStudent(profile: any): Student {
    return {
      id: profile.studentId?.toString() || '1',
      email: profile.email || '',
      firstName: profile.firstName || '',
      lastName: profile.lastName || '',
      studentId: profile.studentId || '',
      enrollmentDate: profile.enrollmentDate || new Date().toISOString().split('T')[0],
      gpa: profile.gpa || 0.0,
      phone: profile.phone || '',
      dateOfBirth: profile.dateOfBirth || '2000-01-01',
      address: profile.address || {
        street: profile.address?.street || '',
        city: profile.address?.city || '',
        state: profile.address?.state || '',
        zipCode: profile.address?.zipCode || '',
        country: profile.address?.country || 'United States'
      },
      emergencyContact: profile.emergencyContact
    }
  }

  // Helper method to map frontend Student data to backend profile update format
  private mapStudentToProfileUpdate(student: Partial<Student>): any {
    return {
      firstName: student.firstName,
      lastName: student.lastName,
      email: student.email,
      phone: student.phone,
      dateOfBirth: student.dateOfBirth,
      address: student.address
    }
  }

  // Change password
  async changePassword(currentPassword: string, newPassword: string): Promise<ApiResponse<void>> {
    return await ApiService.post('/auth/change-password', {
      currentPassword,
      newPassword
    })
  }

  // Refresh token
  async refreshToken(refreshToken: string): Promise<ApiResponse<{ token: string; refreshToken?: string }>> {
    const response = await ApiService.post('/auth/refresh', { refreshToken })
    
    if (response.success && response.data) {
      this.setToken(response.data.token)
      if (response.data.refreshToken) {
        this.setRefreshToken(response.data.refreshToken)
      }
    }
    
    return response
  }

  // Request password reset
  async requestPasswordReset(email: string): Promise<ApiResponse<void>> {
    return await ApiService.post('/auth/password-reset/request', { email })
  }

  // Reset password with token
  async resetPassword(token: string, newPassword: string): Promise<ApiResponse<void>> {
    return await ApiService.post('/auth/password-reset/confirm', {
      token,
      newPassword
    })
  }

  // Verify email with token
  async verifyEmail(token: string): Promise<ApiResponse<void>> {
    return await ApiService.post('/auth/verify-email', { token })
  }

  // Resend email verification
  async resendEmailVerification(): Promise<ApiResponse<void>> {
    return await ApiService.post('/auth/verify-email/resend')
  }

  // Token management
  setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token)
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY)
  }

  clearToken(): void {
    localStorage.removeItem(this.TOKEN_KEY)
  }

  // Refresh token management
  setRefreshToken(refreshToken: string): void {
    localStorage.setItem(this.REFRESH_TOKEN_KEY, refreshToken)
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY)
  }

  clearRefreshToken(): void {
    localStorage.removeItem(this.REFRESH_TOKEN_KEY)
  }

  // User data management
  getStoredUser(): Student | null {
    const userData = localStorage.getItem(this.USER_KEY)
    return userData ? JSON.parse(userData) : null
  }

  clearUser(): void {
    localStorage.removeItem(this.USER_KEY)
  }

  // Authentication state checks
  isAuthenticated(): boolean {
    const token = this.getToken()
    if (!token) return false

    try {
      // Check if token is expired
      const payload = JSON.parse(atob(token.split('.')[1]))
      const currentTime = Date.now() / 1000
      
      return payload.exp > currentTime
    } catch (error) {
      console.error('Invalid token format:', error)
      return false
    }
  }

  isTokenExpired(): boolean {
    return !this.isAuthenticated()
  }

  // Get token expiration time
  getTokenExpiration(): Date | null {
    const token = this.getToken()
    if (!token) return null

    try {
      const payload = JSON.parse(atob(token.split('.')[1]))
      return new Date(payload.exp * 1000)
    } catch (error) {
      console.error('Invalid token format:', error)
      return null
    }
  }

  // Get user roles from token
  getUserRoles(): string[] {
    const token = this.getToken()
    if (!token) return []

    try {
      const payload = JSON.parse(atob(token.split('.')[1]))
      return payload.roles || []
    } catch (error) {
      console.error('Invalid token format:', error)
      return []
    }
  }

  // Check if user has specific role
  hasRole(role: string): boolean {
    const roles = this.getUserRoles()
    return roles.includes(role)
  }

  // Check if user has any of the specified roles
  hasAnyRole(roles: string[]): boolean {
    const userRoles = this.getUserRoles()
    return roles.some(role => userRoles.includes(role))
  }

  // Get user ID from token
  getUserId(): string | null {
    const token = this.getToken()
    if (!token) return null

    try {
      const payload = JSON.parse(atob(token.split('.')[1]))
      return payload.sub || payload.userId || null
    } catch (error) {
      console.error('Invalid token format:', error)
      return null
    }
  }

  // Emergency Contact Management
  async getEmergencyContacts(): Promise<ApiResponse<EmergencyContact[]>> {
    return await ApiService.get<EmergencyContact[]>('/auth/emergency-contacts')
  }

  async addEmergencyContact(contact: Omit<EmergencyContact, 'id'>): Promise<ApiResponse<EmergencyContact>> {
    return await ApiService.post<EmergencyContact>('/auth/emergency-contacts', contact)
  }

  async updateEmergencyContact(contactId: string, contact: Partial<EmergencyContact>): Promise<ApiResponse<EmergencyContact>> {
    return await ApiService.put<EmergencyContact>(`/auth/emergency-contacts/${contactId}`, contact)
  }

  async deleteEmergencyContact(contactId: string): Promise<ApiResponse<void>> {
    return await ApiService.delete(`/auth/emergency-contacts/${contactId}`)
  }

  // Document and Photo Management
  async uploadProfilePhoto(file: File): Promise<ApiResponse<{ photoUrl: string }>> {
    const formData = new FormData()
    formData.append('photo', file)
    
    return await ApiService.post<{ photoUrl: string }>('/auth/profile/photo', formData)
  }

  async uploadDocument(file: File, documentType: string): Promise<ApiResponse<{ documentId: string; documentUrl: string }>> {
    const formData = new FormData()
    formData.append('document', file)
    formData.append('type', documentType)
    
    return await ApiService.post<{ documentId: string; documentUrl: string }>('/auth/documents', formData)
  }

  async getDocuments(): Promise<ApiResponse<Document[]>> {
    const result = await ApiService.get<Document[]>('/auth/documents')
    
    // If API call failed (e.g., 404 for minimal API), return empty success response
    if (!result.success) {
      return {
        success: true,
        data: [],
        message: 'Documents feature not available in minimal API mode'
      }
    }
    
    return result
  }

  async deleteDocument(documentId: string): Promise<ApiResponse<void>> {
    return await ApiService.delete(`/auth/documents/${documentId}`)
  }

  // Clear all authentication data
  clearAll(): void {
    this.clearToken()
    this.clearRefreshToken()
    this.clearUser()
  }
}

// Export singleton instance
export const AuthService = new AuthServiceClass()
export default AuthService