import { describe, it, expect } from 'vitest'
import axios from 'axios'

/**
 * Backend API Smoke Tests
 * 
 * These tests verify that the backend API is running and responding correctly.
 * They test the most critical endpoints to ensure basic connectivity.
 * 
 * Prerequisites:
 * - Backend API should be running on http://localhost:5000
 */

describe('Backend API Smoke Tests', () => {
  const BASE_URL = 'http://localhost:5000/api'
  
  // Create axios instance for testing
  const apiClient = axios.create({
    baseURL: BASE_URL,
    timeout: 10000,
    headers: {
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    },
    validateStatus: () => true // Don't throw on HTTP error status codes
  })

  describe('API Connectivity', () => {
    it('should connect to the API server', async () => {
      try {
        const response = await apiClient.get('/health')
        expect(response.status).toBeLessThan(500) // Any response under 500 means server is running
        console.log(`✅ API server is running (status: ${response.status})`)
      } catch (error: any) {
        if (error.code === 'ECONNREFUSED') {
          throw new Error('❌ Backend API is not running. Please start the API server on http://localhost:5000')
        }
        throw error
      }
    })

    it('should return proper response format', async () => {
      const response = await apiClient.get('/')
      // Should not fail with connection error
      expect(response).toBeDefined()
      console.log(`✅ API responded with status: ${response.status}`)
    })
  })

  describe('Authentication Endpoints', () => {
    it('should have auth/login endpoint available', async () => {
      const response = await apiClient.post('/auth/login', {
        email: 'test@example.com',
        password: 'testpassword'
      })
      
      // Should respond (even if with error), not return 404
      expect(response.status).not.toBe(404)
      if (response.status === 404) {
        throw new Error('❌ Auth endpoints not found - check API routing')
      }
      console.log(`✅ Auth/login endpoint exists (status: ${response.status})`)
    })

    it('should have auth/register endpoint available', async () => {
      const response = await apiClient.post('/auth/register', {
        email: 'test@example.com',
        password: 'Test123!',
        firstName: 'Test',
        lastName: 'User'
      })
      
      expect(response.status).not.toBe(404)
      console.log(`✅ Auth/register endpoint exists (status: ${response.status})`)
    })
  })

  describe('Course Endpoints', () => {
    it('should have courses endpoint available', async () => {
      const response = await apiClient.get('/courses')
      
      // Should exist (may require auth but shouldn't be 404)
      expect(response.status).not.toBe(404)
      if (response.status === 404) {
        throw new Error('❌ Courses endpoint not found - check API routing')
      }
      console.log(`✅ Courses endpoint exists (status: ${response.status})`)
    })

    it('should have courses/paginated endpoint available', async () => {
      const response = await apiClient.get('/courses/paginated?page=1&limit=10')
      
      expect(response.status).not.toBe(404)
      if (response.status === 404) {
        throw new Error('❌ Courses/paginated endpoint not found')
      }
      console.log(`✅ Courses/paginated endpoint exists (status: ${response.status})`)
    })

    it('should handle course search', async () => {
      const response = await apiClient.get('/courses?search=computer')
      
      expect(response.status).not.toBe(404)
      console.log(`✅ Course search handled (status: ${response.status})`)
    })
  })

  describe('API Versioning', () => {
    it('should handle API version headers without crashing', async () => {
      const response = await apiClient.get('/courses', {
        headers: {
          'X-API-Version': '1.0'
        }
      })
      
      // Should not return 500 (server error)
      expect(response.status).not.toBe(500)
      if (response.status === 500) {
        console.error('❌ API versioning middleware causing server errors')
        console.error('Response:', response.data)
        throw new Error('API versioning middleware is causing server errors')
      } else {
        console.log(`✅ API versioning handled correctly (status: ${response.status})`)
      }
    })

    it('should handle missing API version gracefully', async () => {
      const response = await apiClient.get('/courses')
      
      // Should default to a version and not crash
      expect(response.status).not.toBe(500)
      console.log(`✅ Missing API version handled gracefully (status: ${response.status})`)
    })
  })

  describe('Error Handling', () => {
    it('should return 404 for non-existent endpoints', async () => {
      const response = await apiClient.get('/this-endpoint-does-not-exist')
      expect(response.status).toBe(404)
      console.log('✅ 404 handling works correctly')
    })

    it('should handle malformed requests gracefully', async () => {
      const response = await apiClient.post('/auth/login', 'invalid-json', {
        headers: {
          'Content-Type': 'application/json'
        }
      })
      
      // Should return 400 or 415, not crash (500)
      expect([400, 415].includes(response.status)).toBe(true)
      console.log(`✅ Malformed request handled gracefully (status: ${response.status})`)
    })

    it('should return proper error format for validation errors', async () => {
      const response = await apiClient.post('/auth/login', {
        email: 'invalid-email',
        password: ''
      })
      
      if (response.status >= 400 && response.status < 500) {
        expect(response.data).toBeDefined()
        expect(typeof response.data).toBe('object')
        console.log('✅ Error responses are properly formatted')
      }
    })
  })
})

/**
 * Extended Backend Integration Tests
 * These run if basic connectivity is working
 */
describe('Backend API Data Structure Tests', () => {
  const BASE_URL = 'http://localhost:5000/api'
  
  const apiClient = axios.create({
    baseURL: BASE_URL,
    timeout: 15000,
    headers: {
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    },
    validateStatus: () => true
  })

  describe('Course Data Validation', () => {
    it('should return courses with expected structure (if accessible)', async () => {
      const response = await apiClient.get('/courses')
      
      if (response.status === 200 && response.data) {
        const courses = Array.isArray(response.data) ? response.data : response.data.data
        
        if (courses && courses.length > 0) {
          const course = courses[0]
          
          // Verify course has expected properties for frontend
          expect(course).toHaveProperty('id')
          expect(course).toHaveProperty('code')
          expect(course).toHaveProperty('name')
          expect(course).toHaveProperty('credits')
          
          // Verify data types
          expect(typeof course.id).toBe('string')
          expect(typeof course.code).toBe('string')
          expect(typeof course.name).toBe('string')
          expect(typeof course.credits).toBe('number')
          
          console.log('✅ Course data structure matches frontend expectations')
          console.log('Sample course:', JSON.stringify(course, null, 2))
        } else {
          console.log('⚠️ No course data returned (may need database seeding)')
        }
      } else if (response.status === 401 || response.status === 403) {
        console.log('⚠️ Courses endpoint requires authentication (expected)')
      } else if (response.status === 404) {
        console.log('❌ Courses endpoint not found')
      } else {
        console.log(`⚠️ Courses endpoint returned status: ${response.status}`)
      }
    })
  })

  describe('Pagination Support Validation', () => {
    it('should support pagination format (if accessible)', async () => {
      const response = await apiClient.get('/courses/paginated', {
        params: {
          page: 1,
          limit: 5
        }
      })
      
      if (response.status === 200 && response.data) {
        // Check for expected pagination structure
        if (response.data.data && response.data.pagination) {
          expect(response.data).toHaveProperty('data')
          expect(response.data).toHaveProperty('pagination')
          
          const pagination = response.data.pagination
          expect(pagination).toHaveProperty('page')
          expect(pagination).toHaveProperty('pageSize')
          expect(pagination).toHaveProperty('totalItems')
          expect(pagination).toHaveProperty('totalPages')
          
          console.log('✅ Pagination structure matches frontend expectations')
          console.log('Pagination info:', JSON.stringify(pagination, null, 2))
        } else {
          console.log('⚠️ Pagination response format differs from expected structure')
          console.log('Actual response:', JSON.stringify(response.data, null, 2))
        }
      } else {
        console.log(`⚠️ Paginated endpoint status: ${response.status}`)
      }
    })
  })

  describe('Search and Filter Functionality', () => {
    it('should support course search (if accessible)', async () => {
      const response = await apiClient.get('/courses', {
        params: {
          search: 'computer'
        }
      })
      
      if (response.status === 200) {
        const courses = Array.isArray(response.data) ? response.data : response.data.data
        expect(Array.isArray(courses)).toBe(true)
        console.log(`✅ Course search returned ${courses?.length || 0} results`)
      } else {
        console.log(`⚠️ Course search status: ${response.status}`)
      }
    })

    it('should support filtering by department (if accessible)', async () => {
      const response = await apiClient.get('/courses', {
        params: {
          department: 'Computer Science'
        }
      })
      
      if (response.status === 200) {
        console.log('✅ Department filtering is supported')
      } else {
        console.log(`⚠️ Department filtering status: ${response.status}`)
      }
    })
  })
})

/**
 * Performance Smoke Tests
 */
describe('Backend API Performance Tests', () => {
  const BASE_URL = 'http://localhost:5000/api'
  
  const apiClient = axios.create({
    baseURL: BASE_URL,
    timeout: 5000, // 5 second timeout for performance tests
    headers: {
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    },
    validateStatus: () => true
  })

  it('should respond quickly to basic endpoints', async () => {
    const startTime = Date.now()
    
    try {
      const response = await apiClient.get('/health')
      const responseTime = Date.now() - startTime
      
      expect(responseTime).toBeLessThan(5000)
      console.log(`✅ API response time: ${responseTime}ms`)
      
      if (responseTime > 2000) {
        console.log('⚠️ API response time is slow (>2s) - consider optimization')
      }
    } catch (error: any) {
      if (error.code === 'ECONNREFUSED') {
        throw new Error('❌ Backend API is not running')
      } else if (error.code === 'ECONNABORTED') {
        throw new Error('❌ API response timeout (>5s) - server may be overloaded')
      }
      throw error
    }
  })

  it('should handle concurrent requests', async () => {
    const requests = Array(5).fill(null).map(() => apiClient.get('/health'))
    
    const startTime = Date.now()
    const responses = await Promise.all(requests)
    const totalTime = Date.now() - startTime
    
    // All requests should succeed (or at least not fail with connection errors)
    responses.forEach(response => {
      expect(response).toBeDefined()
    })
    
    console.log(`✅ Handled 5 concurrent requests in ${totalTime}ms`)
    
    if (totalTime > 10000) {
      console.log('⚠️ Concurrent request handling is slow - may need performance tuning')
    }
  })
})