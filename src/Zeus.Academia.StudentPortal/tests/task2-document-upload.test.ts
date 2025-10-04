import { describe, it, expect, beforeEach, vi } from 'vitest'
import { AuthService } from '../src/services/AuthService'
import { ApiService } from '../src/services/ApiService'
import type { Document, ApiResponse } from '../src/types'

describe('Task 2: Document Upload and Management System', () => {
  beforeEach(() => {
    localStorage.clear()
    vi.clearAllMocks()
  })

  describe('Profile Photo Upload', () => {
    it('should upload profile photo with validation', async () => {
      const mockFile = new File(['photo content'], 'profile.jpg', { type: 'image/jpeg' })
      
      const mockApiResponse: ApiResponse<{ photoUrl: string }> = {
        success: true,
        data: {
          photoUrl: 'https://api.example.com/uploads/photos/profile-123.jpg'
        },
        message: 'Profile photo uploaded successfully'
      }

      const postSpy = vi.spyOn(ApiService, 'post')
      postSpy.mockResolvedValue(mockApiResponse)

      const result = await AuthService.uploadProfilePhoto(mockFile)

      expect(result.success).toBe(true)
      expect(result.data?.photoUrl).toBe('https://api.example.com/uploads/photos/profile-123.jpg')
      expect(result.message).toBe('Profile photo uploaded successfully')
      
      // Verify the FormData was created correctly
      expect(postSpy).toHaveBeenCalledWith('/auth/profile/photo', expect.any(Object))
      
      const formDataCall = postSpy.mock.calls[0][1] as FormData
      expect(formDataCall.get('photo')).toBe(mockFile)
    })

    it('should validate photo file type and size', async () => {
      const invalidFile = new File(['invalid content'], 'document.pdf', { type: 'application/pdf' })
      
      const mockErrorResponse: ApiResponse<any> = {
        success: false,
        message: 'Invalid file type',
        errors: [
          'Only image files (JPEG, PNG, GIF) are allowed',
          'File size must be less than 5MB'
        ]
      }

      const postSpy = vi.spyOn(ApiService, 'post')
      postSpy.mockResolvedValue(mockErrorResponse)

      const result = await AuthService.uploadProfilePhoto(invalidFile)

      expect(result.success).toBe(false)
      expect(result.errors).toContain('Only image files (JPEG, PNG, GIF) are allowed')
      expect(result.errors).toContain('File size must be less than 5MB')
    })

    it('should handle photo upload failures gracefully', async () => {
      const mockFile = new File(['photo content'], 'profile.jpg', { type: 'image/jpeg' })
      
      const postSpy = vi.spyOn(ApiService, 'post')
      postSpy.mockRejectedValue(new Error('Network error'))

      await expect(AuthService.uploadProfilePhoto(mockFile)).rejects.toThrow('Network error')
    })
  })

  describe('Document Management with File Type Validation', () => {
    it('should upload document with proper validation', async () => {
      const mockFile = new File(['document content'], 'transcript.pdf', { type: 'application/pdf' })
      const documentType = 'transcript'
      
      const mockApiResponse: ApiResponse<{ documentId: string; documentUrl: string }> = {
        success: true,
        data: {
          documentId: 'doc-123',
          documentUrl: 'https://api.example.com/uploads/documents/transcript-123.pdf'
        },
        message: 'Document uploaded successfully'
      }

      const postSpy = vi.spyOn(ApiService, 'post')
      postSpy.mockResolvedValue(mockApiResponse)

      const result = await AuthService.uploadDocument(mockFile, documentType)

      expect(result.success).toBe(true)
      expect(result.data?.documentId).toBe('doc-123')
      expect(result.data?.documentUrl).toBe('https://api.example.com/uploads/documents/transcript-123.pdf')
      
      // Verify FormData was created correctly
      expect(postSpy).toHaveBeenCalledWith('/auth/documents', expect.any(Object))
      
      const formDataCall = postSpy.mock.calls[0][1] as FormData
      expect(formDataCall.get('document')).toBe(mockFile)
      expect(formDataCall.get('type')).toBe(documentType)
    })

    it('should validate document file types', async () => {
      const invalidFile = new File(['executable content'], 'virus.exe', { type: 'application/x-msdownload' })
      const documentType = 'other'
      
      const mockErrorResponse: ApiResponse<any> = {
        success: false,
        message: 'Invalid file type',
        errors: [
          'File type not allowed. Allowed types: PDF, DOC, DOCX, JPG, PNG',
          'File appears to be executable and is not permitted'
        ]
      }

      const postSpy = vi.spyOn(ApiService, 'post')
      postSpy.mockResolvedValue(mockErrorResponse)

      const result = await AuthService.uploadDocument(invalidFile, documentType)

      expect(result.success).toBe(false)
      expect(result.errors).toContain('File type not allowed. Allowed types: PDF, DOC, DOCX, JPG, PNG')
      expect(result.errors).toContain('File appears to be executable and is not permitted')
    })

    it('should validate document size limits', async () => {
      // Create a mock large file
      const largeFile = new File(['x'.repeat(11 * 1024 * 1024)], 'large-document.pdf', { type: 'application/pdf' })
      const documentType = 'transcript'
      
      const mockErrorResponse: ApiResponse<any> = {
        success: false,
        message: 'File too large',
        errors: [
          'File size exceeds maximum limit of 10MB',
          'Please compress the file or upload a smaller version'
        ]
      }

      const postSpy = vi.spyOn(ApiService, 'post')
      postSpy.mockResolvedValue(mockErrorResponse)

      const result = await AuthService.uploadDocument(largeFile, documentType)

      expect(result.success).toBe(false)
      expect(result.errors).toContain('File size exceeds maximum limit of 10MB')
      expect(result.errors).toContain('Please compress the file or upload a smaller version')
    })

    it('should list all uploaded documents', async () => {
      const mockDocuments: Document[] = [
        {
          id: 'doc-1',
          name: 'transcript.pdf',
          type: 'transcript',
          url: 'https://api.example.com/uploads/documents/transcript-123.pdf',
          uploadDate: '2023-10-01T10:00:00Z',
          size: 1024000
        },
        {
          id: 'doc-2',
          name: 'resume.pdf',
          type: 'resume',
          url: 'https://api.example.com/uploads/documents/resume-456.pdf',
          uploadDate: '2023-10-02T14:30:00Z',
          size: 512000
        }
      ]

      const mockApiResponse: ApiResponse<Document[]> = {
        success: true,
        data: mockDocuments
      }

      const getSpy = vi.spyOn(ApiService, 'get')
      getSpy.mockResolvedValue(mockApiResponse)

      const result = await AuthService.getDocuments()

      expect(result.success).toBe(true)
      expect(result.data).toHaveLength(2)
      expect(result.data?.[0].name).toBe('transcript.pdf')
      expect(result.data?.[0].type).toBe('transcript')
      expect(result.data?.[1].name).toBe('resume.pdf')
      expect(result.data?.[1].type).toBe('resume')
      expect(getSpy).toHaveBeenCalledWith('/auth/documents')
    })

    it('should delete document by ID', async () => {
      const documentId = 'doc-123'
      
      const mockApiResponse: ApiResponse<void> = {
        success: true,
        message: 'Document deleted successfully'
      }

      const deleteSpy = vi.spyOn(ApiService, 'delete')
      deleteSpy.mockResolvedValue(mockApiResponse)

      const result = await AuthService.deleteDocument(documentId)

      expect(result.success).toBe(true)
      expect(result.message).toBe('Document deleted successfully')
      expect(deleteSpy).toHaveBeenCalledWith(`/auth/documents/${documentId}`)
    })

    it('should handle document upload with virus scanning', async () => {
      const suspiciousFile = new File(['suspicious content'], 'suspicious.pdf', { type: 'application/pdf' })
      const documentType = 'other'
      
      const mockErrorResponse: ApiResponse<any> = {
        success: false,
        message: 'Security scan failed',
        errors: [
          'File failed security scan',
          'Potentially malicious content detected',
          'Please contact support if you believe this is an error'
        ]
      }

      const postSpy = vi.spyOn(ApiService, 'post')
      postSpy.mockResolvedValue(mockErrorResponse)

      const result = await AuthService.uploadDocument(suspiciousFile, documentType)

      expect(result.success).toBe(false)
      expect(result.errors).toContain('File failed security scan')
      expect(result.errors).toContain('Potentially malicious content detected')
      expect(result.errors).toContain('Please contact support if you believe this is an error')
    })

    it('should validate required document types for student profile', async () => {
      const validFile = new File(['document content'], 'id.jpg', { type: 'image/jpeg' })
      const invalidDocumentType = 'invalid-type'
      
      const mockErrorResponse: ApiResponse<any> = {
        success: false,
        message: 'Invalid document type',
        errors: [
          'Document type "invalid-type" is not recognized',
          'Valid types: transcript, id, insurance, immunization, resume, other'
        ]
      }

      const postSpy = vi.spyOn(ApiService, 'post')
      postSpy.mockResolvedValue(mockErrorResponse)

      const result = await AuthService.uploadDocument(validFile, invalidDocumentType)

      expect(result.success).toBe(false)
      expect(result.errors).toContain('Document type "invalid-type" is not recognized')
      expect(result.errors).toContain('Valid types: transcript, id, insurance, immunization, resume, other')
    })

    it('should handle concurrent document uploads', async () => {
      const file1 = new File(['doc1 content'], 'doc1.pdf', { type: 'application/pdf' })
      const file2 = new File(['doc2 content'], 'doc2.pdf', { type: 'application/pdf' })
      
      const mockResponse1: ApiResponse<{ documentId: string; documentUrl: string }> = {
        success: true,
        data: { documentId: 'doc-1', documentUrl: 'url1' }
      }
      
      const mockResponse2: ApiResponse<{ documentId: string; documentUrl: string }> = {
        success: true,
        data: { documentId: 'doc-2', documentUrl: 'url2' }
      }

      const postSpy = vi.spyOn(ApiService, 'post')
      postSpy.mockResolvedValueOnce(mockResponse1)
      postSpy.mockResolvedValueOnce(mockResponse2)

      const [result1, result2] = await Promise.all([
        AuthService.uploadDocument(file1, 'transcript'),
        AuthService.uploadDocument(file2, 'resume')
      ])

      expect(result1.success).toBe(true)
      expect(result1.data?.documentId).toBe('doc-1')
      expect(result2.success).toBe(true)
      expect(result2.data?.documentId).toBe('doc-2')
      expect(postSpy).toHaveBeenCalledTimes(2)
    })
  })
})