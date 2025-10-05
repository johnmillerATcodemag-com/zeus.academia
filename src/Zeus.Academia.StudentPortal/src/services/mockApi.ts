import type { AxiosInstance } from 'axios'
import type { Student, EmergencyContact, Document, LoginResponse } from '../types'

// Mock data for demonstration
const mockStudent: Student = {
  id: '1',
  email: 'student@zeus.edu',
  firstName: 'John',
  lastName: 'Doe',
  studentId: 'STU-2024-001',
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
  },

  // Helper function to transform mock course data to Course interface
  transformCourse: (mockCourse: any) => {
    // Determine enrollment status based on capacity and enrollment
    let enrollmentStatus = 'Enrolled'
    if (mockCourse.enrolled >= mockCourse.capacity) {
      enrollmentStatus = mockCourse.waitlist > 0 ? 'Waitlisted' : 'Completed'
    }

    return {
      id: mockCourse.id,
      code: mockCourse.code,
      name: mockCourse.title, // Convert title to name
      description: mockCourse.description,
      credits: mockCourse.credits,
      instructor: mockCourse.instructor,
      department: mockCourse.department,
      schedule: mockCourse.schedule, // Keep as string for now
      maxEnrollment: mockCourse.capacity,
      enrolledStudents: mockCourse.enrolled,
      waitlistCount: mockCourse.waitlist,
      prerequisites: mockCourse.prerequisites || [],
      enrollmentStatus: enrollmentStatus,
      difficulty: typeof mockCourse.difficulty === 'string' ? 3 : mockCourse.difficulty || 3,
      weeklyWorkload: 8 // Default value
    }
  },

  // Course endpoints
  'GET /courses': async (params: any) => {
    await delay(300)
    
    // Mock course data
    const mockCourses = [
      {
        id: '1',
        code: 'CS101',
        title: 'Introduction to Computer Science',
        description: 'An introductory course covering fundamental concepts of computer science.',
        credits: 3,
        instructor: 'Dr. Smith',
        department: 'Computer Science',
        level: 'Undergraduate',
        semester: 'Fall 2024',
        schedule: 'MWF 10:00-11:00',
        room: 'CS Building 101',
        capacity: 30,
        enrolled: 25,
        waitlist: 2,
        prerequisites: [],
        difficulty: 'Beginner',
        rating: 4.5
      },
      {
        id: '2',
        code: 'CS201',
        title: 'Data Structures and Algorithms',
        description: 'Advanced study of data structures and algorithmic techniques.',
        credits: 4,
        instructor: 'Prof. Johnson',
        department: 'Computer Science',
        level: 'Undergraduate',
        semester: 'Fall 2024',
        schedule: 'TTh 14:00-16:00',
        room: 'CS Building 201',
        capacity: 25,
        enrolled: 25,
        waitlist: 5,
        prerequisites: ['CS101'],
        difficulty: 'Intermediate',
        rating: 4.2
      },
      {
        id: '3',
        code: 'MATH101',
        title: 'Calculus I',
        description: 'Introduction to differential and integral calculus.',
        credits: 4,
        instructor: 'Dr. Wilson',
        department: 'Mathematics',  
        level: 'Undergraduate',
        semester: 'Fall 2024',
        schedule: 'MWF 09:00-10:00',
        room: 'Math Building 105',
        capacity: 40,
        enrolled: 35,
        waitlist: 0,
        prerequisites: [],
        difficulty: 'Intermediate',
        rating: 4.0
      },
      {
        id: '4',
        code: 'ENG101',
        title: 'English Composition',
        description: 'Fundamentals of academic writing and composition.',
        credits: 3,
        instructor: 'Prof. Davis',
        department: 'English',
        level: 'Undergraduate', 
        semester: 'Fall 2024',
        schedule: 'TTh 11:00-12:30',
        room: 'Liberal Arts 210',
        capacity: 20,
        enrolled: 18,
        waitlist: 0,
        prerequisites: [],
        difficulty: 'Beginner',
        rating: 4.3
      },
      {
        id: '5',
        code: 'CS301',
        title: 'Database Systems',
        description: 'Design and implementation of database management systems.',
        credits: 3,
        instructor: 'Dr. Brown',
        department: 'Computer Science',
        level: 'Undergraduate',
        semester: 'Fall 2024',
        schedule: 'MWF 13:00-14:00',
        room: 'CS Building 301',
        capacity: 20,
        enrolled: 15,
        waitlist: 0,
        prerequisites: ['CS101', 'CS201'],
        difficulty: 'Advanced',
        rating: 4.1
      }
    ]

    // Apply filters based on query parameters
    let filteredCourses = [...mockCourses]
    
    if (params.search) {
      const searchTerm = params.search.toLowerCase()
      filteredCourses = filteredCourses.filter(course => 
        course.title.toLowerCase().includes(searchTerm) ||
        course.code.toLowerCase().includes(searchTerm) ||
        course.instructor.toLowerCase().includes(searchTerm) ||
        course.description.toLowerCase().includes(searchTerm)
      )
    }

    if (params.department) {
      filteredCourses = filteredCourses.filter(course => 
        course.department.toLowerCase() === params.department.toLowerCase()
      )
    }

    if (params.level) {
      filteredCourses = filteredCourses.filter(course => 
        course.level.toLowerCase() === params.level.toLowerCase()
      )
    }

    if (params.instructor) {
      filteredCourses = filteredCourses.filter(course => 
        course.instructor.toLowerCase().includes(params.instructor.toLowerCase())
      )
    }

    if (params.timeSlot) {
      filteredCourses = filteredCourses.filter(course => 
        course.schedule.toLowerCase().includes(params.timeSlot.toLowerCase())
      )
    }

    // Pagination
    const page = parseInt(params.page) || 1
    const limit = parseInt(params.limit) || 10
    const startIndex = (page - 1) * limit
    const endIndex = startIndex + limit
    const paginatedCourses = filteredCourses.slice(startIndex, endIndex)

    // Transform courses to match Course interface
    const transformedCourses = paginatedCourses.map(course => mockResponses.transformCourse(course))

    return {
      data: transformedCourses,
      pagination: {
        page: page,
        pageSize: limit,
        totalItems: filteredCourses.length,
        totalPages: Math.ceil(filteredCourses.length / limit)
      }
    }
  },

  'GET /courses/:id': async (_data: any, id: string) => {
    await delay(200)
    // Return detailed course information
    const courses = [
      {
        id: '1',
        code: 'CS101',
        title: 'Introduction to Computer Science',
        description: 'An introductory course covering fundamental concepts of computer science.',
        credits: 3,
        instructor: 'Dr. Smith',
        department: 'Computer Science',
        level: 'Undergraduate',
        semester: 'Fall 2024',
        schedule: 'MWF 10:00-11:00',
        room: 'CS Building 101',
        capacity: 30,
        enrolled: 25,
        waitlist: 2,
        prerequisites: [],
        difficulty: 'Beginner',
        rating: 4.5
      }
    ]
    
    const course = courses.find(c => c.id === id)
    if (course) {
      return { data: course }
    }
    throw new Error('Course not found')
  },

  'POST /courses/:id/enroll': async (_data: any, id: string) => {
    await delay(500)
    return { 
      data: { 
        message: `Successfully enrolled in course ${id}`,
        enrollmentId: 'ENROLL-' + Date.now(),
        courseId: id
      } 
    }
  },

  'POST /courses/:id/waitlist': async (_data: any, id: string) => {
    await delay(300)
    return { 
      data: { 
        message: `Added to waitlist for course ${id}`,
        position: Math.floor(Math.random() * 5) + 1,
        courseId: id
      } 
    }
  },

  // Paginated courses endpoint
  'GET /courses/paginated': async (params: any) => {
    await delay(300)
    
    // Mock course data
    const mockCourses = [
      {
        id: '1',
        code: 'CS101',
        title: 'Introduction to Computer Science',
        description: 'An introductory course covering fundamental concepts of computer science.',
        credits: 3,
        instructor: 'Dr. Smith',
        department: 'Computer Science',
        level: 'Undergraduate',
        semester: 'Fall 2024',
        schedule: 'MWF 10:00-11:00',
        room: 'CS Building 101',
        capacity: 30,
        enrolled: 25,
        waitlist: 2,
        prerequisites: [],
        difficulty: 'Beginner',
        rating: 4.5
      },
      {
        id: '2',
        code: 'CS201',
        title: 'Data Structures and Algorithms',
        description: 'Advanced study of data structures and algorithmic techniques.',
        credits: 4,
        instructor: 'Prof. Johnson',
        department: 'Computer Science',
        level: 'Undergraduate',
        semester: 'Fall 2024',
        schedule: 'TTh 14:00-16:00',
        room: 'CS Building 201',
        capacity: 25,
        enrolled: 25,
        waitlist: 5,
        prerequisites: ['CS101'],
        difficulty: 'Intermediate',
        rating: 4.2
      },
      {
        id: '3',
        code: 'MATH101',
        title: 'Calculus I',
        description: 'Introduction to differential and integral calculus.',
        credits: 4,
        instructor: 'Dr. Wilson',
        department: 'Mathematics',  
        level: 'Undergraduate',
        semester: 'Fall 2024',
        schedule: 'MWF 09:00-10:00',
        room: 'Math Building 105',
        capacity: 40,
        enrolled: 35,
        waitlist: 0,
        prerequisites: [],
        difficulty: 'Intermediate',
        rating: 4.0
      },
      {
        id: '4',
        code: 'ENG101',
        title: 'English Composition',
        description: 'Fundamentals of academic writing and composition.',
        credits: 3,
        instructor: 'Prof. Davis',
        department: 'English',
        level: 'Undergraduate', 
        semester: 'Fall 2024',
        schedule: 'TTh 11:00-12:30',
        room: 'Liberal Arts 210',
        capacity: 20,
        enrolled: 18,
        waitlist: 0,
        prerequisites: [],
        difficulty: 'Beginner',
        rating: 4.3
      },
      {
        id: '5',
        code: 'CS301',
        title: 'Database Systems',
        description: 'Design and implementation of database management systems.',
        credits: 3,
        instructor: 'Dr. Brown',
        department: 'Computer Science',
        level: 'Undergraduate',
        semester: 'Fall 2024',
        schedule: 'MWF 13:00-14:00',
        room: 'CS Building 301',
        capacity: 20,
        enrolled: 15,
        waitlist: 0,
        prerequisites: ['CS101', 'CS201'],
        difficulty: 'Advanced',
        rating: 4.1
      },
      {
        id: '6',
        code: 'PHYS101',
        title: 'Physics I',
        description: 'Introduction to mechanics and thermodynamics.',
        credits: 4,
        instructor: 'Dr. Anderson',
        department: 'Physics',
        level: 'Undergraduate',
        semester: 'Fall 2024',
        schedule: 'MWF 11:00-12:00',
        room: 'Science Building 201',
        capacity: 35,
        enrolled: 30,
        waitlist: 1,
        prerequisites: ['MATH101'],
        difficulty: 'Intermediate',
        rating: 4.4
      },
      {
        id: '7',
        code: 'HIST101',
        title: 'World History',
        description: 'Survey of world civilizations from ancient to modern times.',
        credits: 3,
        instructor: 'Prof. Martinez',
        department: 'History',
        level: 'Undergraduate',
        semester: 'Fall 2024',
        schedule: 'TTh 09:00-10:30',
        room: 'Humanities 150',
        capacity: 30,
        enrolled: 28,
        waitlist: 0,
        prerequisites: [],
        difficulty: 'Beginner',
        rating: 4.2
      },
      {
        id: '8',
        code: 'BIO101',
        title: 'Introduction to Biology',
        description: 'Basic principles of cellular and molecular biology.',
        credits: 4,
        instructor: 'Dr. Thompson',
        department: 'Biology',
        level: 'Undergraduate',
        semester: 'Fall 2024',
        schedule: 'MWF 14:00-15:00',
        room: 'Biology Lab 101',
        capacity: 25,
        enrolled: 25,
        waitlist: 3,
        prerequisites: [],
        difficulty: 'Intermediate',
        rating: 4.1
      }
    ]

    // Apply filters based on query parameters
    let filteredCourses = [...mockCourses]
    
    if (params.search) {
      const searchTerm = params.search.toLowerCase()
      filteredCourses = filteredCourses.filter(course => 
        course.title.toLowerCase().includes(searchTerm) ||
        course.code.toLowerCase().includes(searchTerm) ||
        course.instructor.toLowerCase().includes(searchTerm) ||
        course.description.toLowerCase().includes(searchTerm)
      )
    }

    if (params.department) {
      filteredCourses = filteredCourses.filter(course => 
        course.department.toLowerCase() === params.department.toLowerCase()
      )
    }

    if (params.level) {
      filteredCourses = filteredCourses.filter(course => 
        course.level.toLowerCase() === params.level.toLowerCase()
      )
    }

    if (params.instructor) {
      filteredCourses = filteredCourses.filter(course => 
        course.instructor.toLowerCase().includes(params.instructor.toLowerCase())
      )
    }

    if (params.timeSlot) {
      filteredCourses = filteredCourses.filter(course => 
        course.schedule.toLowerCase().includes(params.timeSlot.toLowerCase())
      )
    }

    // Pagination
    const page = parseInt(params.page) || 1
    const limit = parseInt(params.limit) || 10
    const startIndex = (page - 1) * limit
    const endIndex = startIndex + limit
    const paginatedCourses = filteredCourses.slice(startIndex, endIndex)

    // Transform courses to match Course interface
    const transformedCourses = paginatedCourses.map(course => mockResponses.transformCourse(course))

    return {
      data: transformedCourses,
      pagination: {
        page: page,
        pageSize: limit,
        totalItems: filteredCourses.length,
        totalPages: Math.ceil(filteredCourses.length / limit)
      }
    }
  }
}

// Setup mock interceptor
export const setupMockApi = (axiosInstance: AxiosInstance) => {
  // Only enable mocking when explicitly enabled via environment variable
  if (import.meta.env.VITE_MOCK_API === 'true') {
    console.log('🎭 Mock API enabled for development/demo purposes')
    
    // Store original adapter (may be undefined in newer axios versions)
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
      
      // If no mock found and original adapter exists, use it
      if (originalAdapter && typeof originalAdapter === 'function') {
        try {
          return await originalAdapter(config)
        } catch (error) {
          console.warn('🎭 Original adapter failed, falling back to mock error')
        }
      }
      
      // Fallback: simulate network error for unmocked endpoints
      return Promise.reject({
        message: `No mock found for ${key}`,
        code: 'MOCK_NOT_FOUND',
        config: config
      })
    }
  }
}

export default setupMockApi