import type { AxiosInstance } from 'axios'
import type { Student, EmergencyContact, Document, LoginResponse } from '../types'

// Mock data for demonstration
const mockStudent: Student = {
  id: '1',
  email: 'student@zeus.edu',
  firstName: 'John',
  lastName: 'Doe',
  studentId: 'STU001',
  enrollmentDate: '2024-01-15',
  gpa: 3.85,
  phone: '555-123-4567',
  dateOfBirth: '2000-05-15',
  address: {
    street: '123 College Avenue',
    city: 'University City',
    state: 'CA',
    zipCode: '90210',
    country: 'United States'
  },
  emergencyContact: {
    id: '1',
    name: 'Jane Doe',
    relationship: 'Mother',
    phone: '555-987-6543',
    email: 'jane.doe@email.com',
    address: {
      street: '456 Parent Street',
      city: 'Hometown',
      state: 'CA',
      zipCode: '90211',
      country: 'United States'
    }
  }
}

const mockEmergencyContacts: EmergencyContact[] = [
  {
    id: '1',
    name: 'Jane Doe',
    relationship: 'Mother',
    phone: '555-987-6543',
    email: 'jane.doe@email.com',
    address: {
      street: '456 Parent Street',
      city: 'Hometown',
      state: 'CA',
      zipCode: '90211',
      country: 'United States'
    }
  },
  {
    id: '2',
    name: 'Bob Smith',
    relationship: 'Father',
    phone: '555-555-1234',
    email: 'bob.smith@email.com'
  }
]

const mockDocuments: Document[] = [
  {
    id: '1',
    name: 'Academic Transcript.pdf',
    type: 'transcript',
    url: '/api/documents/1/download',
    uploadDate: '2024-09-01',
    size: 245760
  },
  {
    id: '2',
    name: 'Student ID.jpg',
    type: 'id',
    url: '/api/documents/2/download',
    uploadDate: '2024-08-15',
    size: 102400
  }
]

// Simulate network delay
const delay = (ms: number = 500) => new Promise(resolve => setTimeout(resolve, ms))

// Mock API responses
const mockResponses: { [key: string]: any } = {
  // Authentication endpoints
  'POST /auth/login': async () => {
    await delay(1000)
    const loginResponse: LoginResponse = {
      token: 'mock-jwt-token-' + Date.now(),
      refreshToken: 'mock-refresh-token-' + Date.now(),
      student: mockStudent,
      expiresAt: new Date(Date.now() + 60 * 60 * 1000).toISOString() // 1 hour from now
    }
    return { data: loginResponse }
  },

  'POST /auth/logout': async () => {
    await delay(500)
    return { data: { message: 'Logged out successfully' } }
  },

  'GET /auth/me': async () => {
    await delay(300)
    return { data: mockStudent }
  },

  'PUT /auth/profile': async (data: any) => {
    await delay(800)
    // Merge the updated data with mock student
    const updatedStudent = { ...mockStudent, ...data }
    Object.assign(mockStudent, updatedStudent)
    return { data: updatedStudent }
  },

  'POST /auth/change-password': async () => {
    await delay(600)
    return { data: { message: 'Password changed successfully' } }
  },

  'POST /auth/refresh': async () => {
    await delay(400)
    return { 
      data: { 
        token: 'new-mock-jwt-token-' + Date.now(),
        refreshToken: 'new-mock-refresh-token-' + Date.now()
      } 
    }
  },

  // Emergency contacts endpoints
  'GET /emergency-contacts': async () => {
    await delay(300)
    return { data: mockEmergencyContacts }
  },

  'POST /emergency-contacts': async (data: any) => {
    await delay(600)
    const newContact = { ...data, id: Date.now().toString() }
    mockEmergencyContacts.push(newContact)
    return { data: newContact }
  },

  'PUT /emergency-contacts/:id': async (data: any, id: string) => {
    await delay(500)
    const index = mockEmergencyContacts.findIndex(c => c.id === id)
    if (index >= 0) {
      mockEmergencyContacts[index] = { ...mockEmergencyContacts[index], ...data }
      return { data: mockEmergencyContacts[index] }
    }
    throw new Error('Contact not found')
  },

  'DELETE /emergency-contacts/:id': async (_data: any, id: string) => {
    await delay(400)
    const index = mockEmergencyContacts.findIndex(c => c.id === id)
    if (index >= 0) {
      mockEmergencyContacts.splice(index, 1)
      return { data: { message: 'Contact deleted successfully' } }
    }
    throw new Error('Contact not found')
  },

  // Documents endpoints
  'GET /documents': async () => {
    await delay(300)
    return { data: mockDocuments }
  },

  'POST /documents/upload': async () => {
    await delay(2000) // Simulate file upload delay
    const newDoc: Document = {
      id: Date.now().toString(),
      name: 'Uploaded Document.pdf',
      type: 'other',
      url: '/api/documents/' + Date.now() + '/download',
      uploadDate: new Date().toISOString().split('T')[0],
      size: Math.floor(Math.random() * 1000000)
    }
    mockDocuments.push(newDoc)
    return { data: newDoc }
  },

  'POST /profile/photo': async () => {
    await delay(1500) // Simulate photo upload delay
    return { 
      data: { 
        photoUrl: '/api/profile/photo.jpg?t=' + Date.now(),
        message: 'Profile photo updated successfully'
      } 
    }
  },

  'DELETE /documents/:id': async (_data: any, id: string) => {
    await delay(400)
    const index = mockDocuments.findIndex(d => d.id === id)
    if (index >= 0) {
      mockDocuments.splice(index, 1)
      return { data: { message: 'Document deleted successfully' } }
    }
    throw new Error('Document not found')
  }
}

// Setup mock interceptor
export const setupMockApi = (axiosInstance: AxiosInstance) => {
  // Only enable mocking in development mode
  if (import.meta.env.DEV || import.meta.env.VITE_MOCK_API === 'true') {
    console.log('🎭 Mock API enabled for development/demo purposes')
    
    // Store original adapter
    const originalAdapter = axiosInstance.defaults.adapter
    
    // Override the adapter to intercept requests
    axiosInstance.defaults.adapter = async (config: any) => {
      const method = config.method?.toUpperCase()
      const url = config.url?.replace(/^.*\/api/, '') // Remove base URL and /api prefix
      const key = `${method} ${url}`
      
      // Handle parameterized URLs (e.g., /emergency-contacts/123)
      let matchedKey = key
      let pathParams: string | undefined
      
      // Check for exact match first
      if (!mockResponses[key]) {
        // Try to match parameterized routes
        for (const mockKey of Object.keys(mockResponses)) {
          const pattern = mockKey.replace(/:id/g, '([^/]+)')
          const regex = new RegExp(`^${pattern}$`)
          const match = key.match(regex)
          if (match) {
            matchedKey = mockKey
            pathParams = match[1] // First captured group (the ID)
            break
          }
        }
      }

      if (mockResponses[matchedKey]) {
        console.log(`🎭 Mock response for: ${key}`)
        
        try {
          const mockFn = mockResponses[matchedKey]
          const response = await mockFn(JSON.parse(config.data || '{}'), pathParams)
          
          return Promise.resolve({
            data: response.data,
            status: 200,
            statusText: 'OK',
            headers: { 'content-type': 'application/json' },
            config: config
          })
        } catch (error) {
          console.error('🎭 Mock API error:', error)
          return Promise.reject({
            response: {
              status: 500,
              data: { message: (error as Error).message || 'Mock API error' },
              headers: {},
              config: config
            }
          })
        }
      }
      
      // If no mock found, use original adapter (will fail to real API)
      if (originalAdapter) {
        return originalAdapter(config)
      }
      
      // Fallback: simulate network error
      return Promise.reject({
        message: 'Network Error',
        code: 'NETWORK_ERROR',
        config: config
      })
    }
  }
}

export default setupMockApi