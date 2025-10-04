import { describe, it, expect, beforeEach, vi } from 'vitest'
import { AuthService } from '../src/services/AuthService'
import { ApiService } from '../src/services/ApiService'
import type { Student, ApiResponse } from '../src/types'

describe('Task 2: Profile Management System', () => {
  beforeEach(() => {
    localStorage.clear()
    vi.clearAllMocks()
  })

  describe('Complete Student Profile Editing', () => {
    it('should update profile with validation and data persistence', async () => {
      const mockStudentUpdate: Partial<Student> = {
        firstName: 'John',
        lastName: 'Smith',
        email: 'john.smith@updated.com',
        phone: '555-123-4567',
        dateOfBirth: '1995-05-15',
        address: {
          street: '123 Main St',
          city: 'Springfield',
          state: 'IL',
          zipCode: '62701',
          country: 'USA'
        },
        emergencyContact: {
          name: 'Jane Smith',
          relationship: 'Mother',
          phone: '555-987-6543',
          email: 'jane.smith@example.com'
        }
      }

      const mockUpdatedStudent: Student = {
        id: '1',
        email: 'john.smith@updated.com',
        firstName: 'John',
        lastName: 'Smith',
        studentId: 'STU001',
        enrollmentDate: '2023-09-01',
        phone: '555-123-4567',
        dateOfBirth: '1995-05-15',
        address: {
          street: '123 Main St',
          city: 'Springfield',
          state: 'IL',
          zipCode: '62701',
          country: 'USA'
        },
        emergencyContact: {
          name: 'Jane Smith',
          relationship: 'Mother',
          phone: '555-987-6543',
          email: 'jane.smith@example.com'
        }
      }

      const mockApiResponse: ApiResponse<Student> = {
        success: true,
        data: mockUpdatedStudent
      }

      const putSpy = vi.spyOn(ApiService, 'put')
      putSpy.mockResolvedValue(mockApiResponse)

      const result = await AuthService.updateProfile(mockStudentUpdate)

      expect(result.success).toBe(true)
      expect(result.data).toEqual(mockUpdatedStudent)
      expect(AuthService.getStoredUser()).toEqual(mockUpdatedStudent)
      expect(putSpy).toHaveBeenCalledWith('/auth/profile', mockStudentUpdate)
    })

    it('should validate required fields before updating profile', async () => {
      const invalidProfileUpdate = {
        firstName: '', // Invalid: empty first name
        lastName: 'Smith',
        email: 'invalid-email' // Invalid: malformed email
      }

      const mockErrorResponse: ApiResponse<Student> = {
        success: false,
        message: 'Validation failed',
        errors: [
          'First name is required',
          'Email format is invalid'
        ]
      }

      const putSpy = vi.spyOn(ApiService, 'put')
      putSpy.mockResolvedValue(mockErrorResponse)

      const result = await AuthService.updateProfile(invalidProfileUpdate)

      expect(result.success).toBe(false)
      expect(result.errors).toContain('First name is required')
      expect(result.errors).toContain('Email format is invalid')
      expect(putSpy).toHaveBeenCalledWith('/auth/profile', invalidProfileUpdate)
    })

    it('should handle profile update failures gracefully', async () => {
      const profileUpdate = {
        firstName: 'John',
        lastName: 'Smith'
      }

      const putSpy = vi.spyOn(ApiService, 'put')
      putSpy.mockRejectedValue(new Error('Network error'))

      await expect(AuthService.updateProfile(profileUpdate)).rejects.toThrow('Network error')
      
      // Ensure stored user data is not modified on failure
      const storedUser = AuthService.getStoredUser()
      expect(storedUser).toBeNull() // Should remain null since no successful login occurred
    })

    it('should update only provided fields while preserving others', async () => {
      // Set up existing user data
      const existingUser: Student = {
        id: '1',
        email: 'john@example.com',
        firstName: 'John',
        lastName: 'Doe',
        studentId: 'STU001',
        enrollmentDate: '2023-09-01',
        phone: '555-000-0000'
      }

      localStorage.setItem('zeus_user', JSON.stringify(existingUser))

      const partialUpdate = {
        firstName: 'Johnny',
        phone: '555-111-2222'
      }

      const expectedUpdatedUser: Student = {
        ...existingUser,
        firstName: 'Johnny',
        phone: '555-111-2222'
      }

      const mockApiResponse: ApiResponse<Student> = {
        success: true,
        data: expectedUpdatedUser
      }

      const putSpy = vi.spyOn(ApiService, 'put')
      putSpy.mockResolvedValue(mockApiResponse)

      const result = await AuthService.updateProfile(partialUpdate)

      expect(result.success).toBe(true)
      expect(result.data?.firstName).toBe('Johnny')
      expect(result.data?.phone).toBe('555-111-2222')
      expect(result.data?.lastName).toBe('Doe') // Should preserve existing value
      expect(result.data?.email).toBe('john@example.com') // Should preserve existing value
    })
  })

  describe('Password Change with Security Requirements', () => {
    it('should change password with proper validation', async () => {
      const currentPassword = 'oldPassword123!'
      const newPassword = 'newSecurePassword456@'

      const mockApiResponse: ApiResponse<void> = {
        success: true,
        message: 'Password changed successfully'
      }

      const postSpy = vi.spyOn(ApiService, 'post')
      postSpy.mockResolvedValue(mockApiResponse)

      const result = await AuthService.changePassword(currentPassword, newPassword)

      expect(result.success).toBe(true)
      expect(result.message).toBe('Password changed successfully')
      expect(postSpy).toHaveBeenCalledWith('/auth/change-password', {
        currentPassword,
        newPassword
      })
    })

    it('should validate password strength requirements', async () => {
      const currentPassword = 'oldPassword123!'
      const weakPassword = '123' // Too weak

      const mockErrorResponse: ApiResponse<void> = {
        success: false,
        message: 'Password does not meet security requirements',
        errors: [
          'Password must be at least 8 characters long',
          'Password must contain at least one uppercase letter',
          'Password must contain at least one special character'
        ]
      }

      const postSpy = vi.spyOn(ApiService, 'post')
      postSpy.mockResolvedValue(mockErrorResponse)

      const result = await AuthService.changePassword(currentPassword, weakPassword)

      expect(result.success).toBe(false)
      expect(result.errors).toContain('Password must be at least 8 characters long')
      expect(result.errors).toContain('Password must contain at least one uppercase letter')
      expect(result.errors).toContain('Password must contain at least one special character')
    })

    it('should validate current password before allowing change', async () => {
      const incorrectCurrentPassword = 'wrongPassword'
      const newPassword = 'newSecurePassword456@'

      const mockErrorResponse: ApiResponse<void> = {
        success: false,
        message: 'Current password is incorrect',
        errors: ['Current password verification failed']
      }

      const postSpy = vi.spyOn(ApiService, 'post')
      postSpy.mockResolvedValue(mockErrorResponse)

      const result = await AuthService.changePassword(incorrectCurrentPassword, newPassword)

      expect(result.success).toBe(false)
      expect(result.message).toBe('Current password is incorrect')
      expect(result.errors).toContain('Current password verification failed')
    })
  })

  describe('Emergency Contact CRUD Operations', () => {
    it('should add emergency contact with validation', async () => {
      const emergencyContact = {
        name: 'Jane Smith',
        relationship: 'Mother',
        phone: '555-987-6543',
        email: 'jane.smith@example.com',
        address: {
          street: '456 Oak Ave',
          city: 'Springfield',
          state: 'IL',
          zipCode: '62701',
          country: 'USA'
        }
      }

      const mockApiResponse: ApiResponse<any> = {
        success: true,
        data: { id: '1', ...emergencyContact },
        message: 'Emergency contact added successfully'
      }

      const postSpy = vi.spyOn(ApiService, 'post')
      postSpy.mockResolvedValue(mockApiResponse)

      const result = await AuthService.addEmergencyContact(emergencyContact)

      expect(result.success).toBe(true)
      expect(result.data?.name).toBe('Jane Smith')
      expect(result.data?.relationship).toBe('Mother')
      expect(postSpy).toHaveBeenCalledWith('/auth/emergency-contacts', emergencyContact)
    })

    it('should update emergency contact information', async () => {
      const contactId = '1'
      const updatedContact = {
        name: 'Jane Smith-Johnson',
        phone: '555-999-8888',
        email: 'jane.johnson@example.com'
      }

      const mockApiResponse: ApiResponse<any> = {
        success: true,
        data: { id: contactId, ...updatedContact },
        message: 'Emergency contact updated successfully'
      }

      const putSpy = vi.spyOn(ApiService, 'put')
      putSpy.mockResolvedValue(mockApiResponse)

      const result = await AuthService.updateEmergencyContact(contactId, updatedContact)

      expect(result.success).toBe(true)
      expect(result.data?.name).toBe('Jane Smith-Johnson')
      expect(result.data?.phone).toBe('555-999-8888')
      expect(putSpy).toHaveBeenCalledWith(`/auth/emergency-contacts/${contactId}`, updatedContact)
    })

    it('should delete emergency contact', async () => {
      const contactId = '1'

      const mockApiResponse: ApiResponse<void> = {
        success: true,
        message: 'Emergency contact deleted successfully'
      }

      const deleteSpy = vi.spyOn(ApiService, 'delete')
      deleteSpy.mockResolvedValue(mockApiResponse)

      const result = await AuthService.deleteEmergencyContact(contactId)

      expect(result.success).toBe(true)
      expect(result.message).toBe('Emergency contact deleted successfully')
      expect(deleteSpy).toHaveBeenCalledWith(`/auth/emergency-contacts/${contactId}`)
    })

    it('should list all emergency contacts', async () => {
      const mockContacts = [
        {
          id: '1',
          name: 'Jane Smith',
          relationship: 'Mother',
          phone: '555-987-6543',
          email: 'jane.smith@example.com'
        },
        {
          id: '2',
          name: 'Bob Smith',
          relationship: 'Father',
          phone: '555-987-6544',
          email: 'bob.smith@example.com'
        }
      ]

      const mockApiResponse: ApiResponse<any[]> = {
        success: true,
        data: mockContacts
      }

      const getSpy = vi.spyOn(ApiService, 'get')
      getSpy.mockResolvedValue(mockApiResponse)

      const result = await AuthService.getEmergencyContacts()

      expect(result.success).toBe(true)
      expect(result.data).toHaveLength(2)
      expect(result.data?.[0].name).toBe('Jane Smith')
      expect(result.data?.[1].name).toBe('Bob Smith')
      expect(getSpy).toHaveBeenCalledWith('/auth/emergency-contacts')
    })

    it('should validate emergency contact required fields', async () => {
      const invalidContact = {
        name: '', // Missing required field
        relationship: 'Mother',
        phone: 'invalid-phone', // Invalid format
        email: 'invalid-email' // Invalid format
      }

      const mockErrorResponse: ApiResponse<any> = {
        success: false,
        message: 'Validation failed',
        errors: [
          'Name is required',
          'Phone number format is invalid',
          'Email format is invalid'
        ]
      }

      const postSpy = vi.spyOn(ApiService, 'post')
      postSpy.mockResolvedValue(mockErrorResponse)

      const result = await AuthService.addEmergencyContact(invalidContact)

      expect(result.success).toBe(false)
      expect(result.errors).toContain('Name is required')
      expect(result.errors).toContain('Phone number format is invalid')
      expect(result.errors).toContain('Email format is invalid')
    })
  })
})