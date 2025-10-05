/**
 * Course Management Service
 * Handles API communication for course management, assignments, roster, and calendar functionality
 */
import type { 
  Course, 
  ExtendedCourse,
  ExtendedAssignment, 
  CourseContent, 
  CourseAnnouncement,
  StudentRoster, 
  CourseCalendarEvent,
  CourseMetrics,
  ApiResponse 
} from '../types'

const API_BASE_URL = (import.meta as any).env?.VITE_API_BASE_URL || 'http://localhost:5000'

export class CourseManagementService {
  private static async apiCall<T>(endpoint: string, options: RequestInit = {}): Promise<T> {
    const token = localStorage.getItem('zeus_auth_token')
    
    const defaultHeaders: HeadersInit = {
      'Content-Type': 'application/json',
    }
    
    if (token) {
      defaultHeaders['Authorization'] = `Bearer ${token}`
    }

    const config: RequestInit = {
      ...options,
      headers: {
        ...defaultHeaders,
        ...options.headers,
      },
    }

    try {
      const response = await fetch(`${API_BASE_URL}${endpoint}`, config)
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`)
      }

      const data: ApiResponse<T> = await response.json()
      
      if (!data.success) {
        throw new Error(data.error || 'API request failed')
      }

      return data.data
    } catch (error) {
      console.error('API call failed:', error)
      // For development, return mock data instead of throwing
      if ((import.meta as any).env?.DEV) {
        return this.getMockData<T>(endpoint)
      }
      throw error
    }
  }

  /**
   * Course Management
   */
  static async getCourses(facultyId: string): Promise<ExtendedCourse[]> {
    if ((import.meta as any).env?.DEV) {
      return this.getMockCourses(facultyId)
    }
    return this.apiCall<ExtendedCourse[]>(`/api/faculty/${facultyId}/courses`)
  }

  static async getCourseDetails(courseId: string): Promise<ExtendedCourse> {
    if ((import.meta as any).env?.DEV) {
      return this.getMockCourseDetails(courseId)
    }
    return this.apiCall<ExtendedCourse>(`/api/courses/${courseId}`)
  }

  static async updateCourse(courseId: string, updates: Partial<Course>): Promise<Course> {
    return this.apiCall<Course>(`/api/courses/${courseId}`, {
      method: 'PUT',
      body: JSON.stringify(updates),
    })
  }

  static async getCourseMetrics(courseId: string): Promise<CourseMetrics> {
    if ((import.meta as any).env?.DEV) {
      return this.getMockCourseMetrics(courseId)
    }
    return this.apiCall<CourseMetrics>(`/api/courses/${courseId}/metrics`)
  }

  /**
   * Assignment Management
   */
  static async getAssignments(courseId: string): Promise<ExtendedAssignment[]> {
    if ((import.meta as any).env?.DEV) {
      return this.getMockAssignments(courseId)
    }
    return this.apiCall<ExtendedAssignment[]>(`/api/courses/${courseId}/assignments`)
  }

  static async createAssignment(courseId: string, assignment: Omit<ExtendedAssignment, 'id'>): Promise<ExtendedAssignment> {
    return this.apiCall<ExtendedAssignment>(`/api/courses/${courseId}/assignments`, {
      method: 'POST',
      body: JSON.stringify(assignment),
    })
  }

  static async updateAssignment(assignmentId: string, updates: Partial<ExtendedAssignment>): Promise<ExtendedAssignment> {
    return this.apiCall<ExtendedAssignment>(`/api/assignments/${assignmentId}`, {
      method: 'PUT',
      body: JSON.stringify(updates),
    })
  }

  static async deleteAssignment(assignmentId: string): Promise<void> {
    await this.apiCall<void>(`/api/assignments/${assignmentId}`, {
      method: 'DELETE',
    })
  }

  /**
   * Content Management
   */
  static async getCourseContent(courseId: string): Promise<CourseContent[]> {
    if ((import.meta as any).env?.DEV) {
      return this.getMockCourseContent(courseId)
    }
    return this.apiCall<CourseContent[]>(`/api/courses/${courseId}/content`)
  }

  static async uploadContent(courseId: string, content: FormData): Promise<CourseContent> {
    const token = localStorage.getItem('zeus_auth_token')
    const headers: HeadersInit = {}
    
    if (token) {
      headers['Authorization'] = `Bearer ${token}`
    }

    const response = await fetch(`${API_BASE_URL}/api/courses/${courseId}/content`, {
      method: 'POST',
      headers,
      body: content,
    })

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }

    const data: ApiResponse<CourseContent> = await response.json()
    
    if (!data.success) {
      throw new Error(data.error || 'Content upload failed')
    }

    return data.data
  }

  static async createAnnouncement(courseId: string, announcement: Omit<CourseAnnouncement, 'id'>): Promise<CourseAnnouncement> {
    return this.apiCall<CourseAnnouncement>(`/api/courses/${courseId}/announcements`, {
      method: 'POST',
      body: JSON.stringify(announcement),
    })
  }

  /**
   * Roster Management
   */
  static async getRoster(courseId: string): Promise<StudentRoster> {
    if ((import.meta as any).env?.DEV) {
      return this.getMockRoster(courseId)
    }
    return this.apiCall<StudentRoster>(`/api/courses/${courseId}/roster`)
  }

  /**
   * Calendar Management
   */
  static async getCalendarEvents(courseId: string): Promise<CourseCalendarEvent[]> {
    if ((import.meta as any).env?.DEV) {
      return this.getMockCalendarEvents(courseId)
    }
    return this.apiCall<CourseCalendarEvent[]>(`/api/courses/${courseId}/calendar`)
  }

  static async createCalendarEvent(courseId: string, event: Omit<CourseCalendarEvent, 'id'>): Promise<CourseCalendarEvent> {
    return this.apiCall<CourseCalendarEvent>(`/api/courses/${courseId}/calendar`, {
      method: 'POST',
      body: JSON.stringify(event),
    })
  }

  /**
   * Grading and Analytics
   */
  static async bulkGradeUpdate(updates: Array<{
    studentId: string
    assignmentId: string
    score: number
  }>): Promise<void> {
    await this.apiCall<void>('/api/grades/bulk-update', {
      method: 'POST',
      body: JSON.stringify({ updates }),
    })
  }

  /**
   * Mock Data Methods for Development
   */
  private static getMockData<T>(endpoint: string): T {
    // Generic mock data handler
    console.log(`Returning mock data for endpoint: ${endpoint}`)
    return {} as T
  }

  private static async getMockCourses(facultyId: string): Promise<ExtendedCourse[]> {
    await new Promise(resolve => setTimeout(resolve, 500))
    
    if (facultyId === 'nonexistent') {
      throw new Error('Faculty not found')
    }
    
    return [
      {
        id: '1',
        name: 'Introduction to Computer Science',
        code: 'CS-101',
        section: '001',
        semester: 'Fall 2024',
        year: 2024,
        credits: 3,
        description: 'Fundamental concepts of computer science and programming.',
        enrollmentCount: 25,
        maxEnrollment: 30,
        status: 'active',
        // Extended properties for Task 3
        title: 'Introduction to Computer Science',
        facultyId,
        sections: [
          {
            id: '1-1',
            courseId: '1',
            sectionNumber: '001',
            meetingTimes: ['MWF 10:00-10:50'],
            location: 'Engineering 101',
            capacity: 30,
            enrolled: 25,
            waitlist: 3,
            status: 'active'
          }
        ],
        metrics: {
          totalEnrolled: 25,
          totalCapacity: 30,
          waitlistCount: 3,
          enrollmentPercentage: 83.3,
          averageGPA: 3.2,
          completionRate: 92.0,
          lastUpdated: new Date()
        }
      },
      {
        id: '2',
        name: 'Data Structures and Algorithms',
        code: 'CS-201',
        section: '001',
        semester: 'Fall 2024',
        year: 2024,
        credits: 4,
        description: 'Advanced programming concepts and algorithm design.',
        enrollmentCount: 22,
        maxEnrollment: 25,
        status: 'active',
        // Extended properties for Task 3
        title: 'Data Structures and Algorithms',
        facultyId,
        sections: [
          {
            id: '2-1',
            courseId: '2',
            sectionNumber: '001',
            meetingTimes: ['TR 14:00-15:30'],
            location: 'Engineering 201',
            capacity: 25,
            enrolled: 22,
            waitlist: 0,
            status: 'active'
          }
        ],
        metrics: {
          totalEnrolled: 22,
          totalCapacity: 25,
          waitlistCount: 0,
          enrollmentPercentage: 88.0,
          averageGPA: 3.5,
          completionRate: 95.0,
          lastUpdated: new Date()
        }
      }
    ] as ExtendedCourse[]
  }

  private static async getMockCourseDetails(courseId: string): Promise<ExtendedCourse> {
    const courses = await this.getMockCourses('1')
    const course = courses.find(c => c.id === courseId)
    if (!course) {
      throw new Error('Course not found')
    }
    return course
  }

  private static async getMockCourseMetrics(_courseId: string): Promise<CourseMetrics> {
    await new Promise(resolve => setTimeout(resolve, 300))
    
    return {
      totalEnrolled: 25,
      totalCapacity: 30,
      waitlistCount: 3,
      enrollmentPercentage: 83.3,
      averageGPA: 3.2,
      completionRate: 92.0,
      lastUpdated: new Date()
    }
  }

  private static async getMockAssignments(courseId: string): Promise<ExtendedAssignment[]> {
    await new Promise(resolve => setTimeout(resolve, 300))
    
    return [
      {
        id: '1',
        courseId,
        title: 'Programming Assignment 1',
        description: 'Implement a basic calculator using Python',
        type: 'programming',
        maxPoints: 100,
        dueDate: new Date('2024-10-15T23:59:00'),
        submissionTypes: ['file_upload'],
        allowedFileTypes: ['.py', '.txt'],
        isPublished: true,
        publishedAt: new Date(),
        createdBy: '1',
        createdAt: new Date(),
        submissions: [],
        statistics: {
          submitted: 0,
          graded: 0,
          averageScore: 0,
          highestScore: 0,
          lowestScore: 0
        }
      }
    ]
  }

  private static async getMockCourseContent(courseId: string): Promise<CourseContent[]> {
    await new Promise(resolve => setTimeout(resolve, 300))
    
    return [
      {
        id: '1',
        courseId,
        type: 'syllabus',
        title: 'Course Syllabus',
        description: 'Complete course syllabus with policies and schedule',
        isPublished: true,
        publishedAt: new Date(),
        lastModified: new Date(),
        createdBy: '1'
      }
    ]
  }

  private static async getMockRoster(courseId: string): Promise<StudentRoster> {
    await new Promise(resolve => setTimeout(resolve, 300))
    
    return {
      courseId,
      sectionId: '1-1',
      students: [
        {
          id: 'student-1',
          studentId: 'ST001',
          firstName: 'John',
          lastName: 'Doe',
          email: 'john.doe@student.university.edu',
          profilePhoto: '/images/students/john-doe.jpg',
          academicInfo: {
            major: 'Computer Science',
            year: 'Sophomore',
            gpa: 3.5,
            creditHours: 15,
            enrollmentStatus: 'full-time',
            advisorId: 'advisor-1'
          },
          enrollmentInfo: {
            enrolledDate: new Date('2024-08-15'),
            status: 'enrolled',
            attendanceRate: 95.0,
            participationScore: 85
          },
          grades: {
            currentGrade: 'B+',
            currentPercentage: 87.5,
            assignments: []
          }
        }
      ],
      totalEnrolled: 1,
      lastUpdated: new Date()
    }
  }

  private static async getMockCalendarEvents(courseId: string): Promise<CourseCalendarEvent[]> {
    await new Promise(resolve => setTimeout(resolve, 300))
    
    return [
      {
        id: 'event-1',
        courseId,
        title: 'Assignment 1 Due',
        description: 'Programming Assignment 1 submission deadline',
        type: 'assignment_due',
        startDate: new Date('2024-10-15T23:59:00'),
        endDate: new Date('2024-10-15T23:59:00'),
        isAllDay: false,
        priority: 'high',
        color: '#dc3545',
        reminders: []
      }
    ]
  }
}