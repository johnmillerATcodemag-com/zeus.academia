import { ApiService } from './ApiService'
import type { 
  Course, 
  Enrollment, 
  ApiResponse, 
  PaginatedResponse,
  EnrollmentStatus 
} from '../types'

export interface CourseSearchParams {
  query?: string
  instructor?: string
  credits?: number
  department?: string
  level?: string
  status?: EnrollmentStatus
}

export interface CoursePaginationParams {
  page?: number
  pageSize?: number
  sortBy?: string
  sortOrder?: 'asc' | 'desc'
}

class CourseServiceClass {
  // Get all courses - Updated to use backend paginated endpoint
  async getCourses(params?: CoursePaginationParams & { search?: string }): Promise<ApiResponse<Course[]>> {
    const response = await ApiService.get<any>('/courses/paginated', params)
    
    if (response.success && response.data) {
      const courses = this.mapBackendCoursesToFrontend(response.data.courses || response.data)
      return { success: true, data: courses }
    }
    
    return response
  }

  // Get paginated courses - Updated to match backend structure
  async getCoursesPaginated(params?: CoursePaginationParams & CourseSearchParams): Promise<PaginatedResponse<Course>> {
    const response = await ApiService.get<any>('/courses/paginated', params)
    
    if (response.success && response.data) {
      const courses = this.mapBackendCoursesToFrontend(response.data.courses || response.data)
      
      return {
        success: true,
        data: courses,
        totalCount: response.data.totalCount || courses.length,
        page: response.data.page || 1,
        pageSize: response.data.pageSize || courses.length,
        totalPages: response.data.totalPages || 1
      }
    }
    
    return {
      success: false,
      data: [],
      totalCount: 0,
      page: 1,
      pageSize: 10,
      totalPages: 0,
      message: response.message
    }
  }

  // Get course by ID - Updated endpoint structure
  async getCourseById(courseId: string): Promise<ApiResponse<Course>> {
    const response = await ApiService.get<any>(`/courses/${courseId}`)
    
    if (response.success && response.data) {
      const course = this.mapBackendCourseToFrontend(response.data)
      return { success: true, data: course }
    }
    
    return response
  }

  // Search courses - Updated to use backend search if available
  async searchCourses(query: string, filters?: CourseSearchParams): Promise<ApiResponse<Course[]>> {
    const params = {
      search: query,
      ...filters
    }
    
    // Use paginated endpoint with search parameters
    const response = await this.getCourses(params)
    return response
  }

  // Get student enrollments - Updated to use backend endpoint
  async getStudentEnrollments(): Promise<ApiResponse<Enrollment[]>> {
    const response = await ApiService.get<any>('/student/enrollments')
    
    if (response.success && response.data) {
      const enrollments = this.mapBackendEnrollmentsToFrontend(response.data.enrollments || response.data)
      return { success: true, data: enrollments }
    }
    
    return response
  }

  // Enroll in course - Updated to use backend endpoint
  async enrollInCourse(courseId: string): Promise<ApiResponse<Enrollment>> {
    const response = await ApiService.post<any>(`/student/enroll/${courseId}`)
    
    if (response.success && response.data) {
      const enrollment = this.mapBackendEnrollmentToFrontend(response.data)
      return { success: true, data: enrollment }
    }
    
    return response
  }

  // Drop course - Updated to use backend endpoint
  async dropCourse(courseId: string): Promise<ApiResponse<void>> {
    return await ApiService.delete(`/student/enroll/${courseId}`)
  }

  // Update enrollment status - if supported by backend
  async updateEnrollmentStatus(enrollmentId: string, status: EnrollmentStatus): Promise<ApiResponse<Enrollment>> {
    const response = await ApiService.patch<any>(`/enrollments/${enrollmentId}`, { status })
    
    if (response.success && response.data) {
      const enrollment = this.mapBackendEnrollmentToFrontend(response.data)
      return { success: true, data: enrollment }
    }
    
    return response
  }

  // Get enrollment by ID
  async getEnrollmentById(enrollmentId: string): Promise<ApiResponse<Enrollment>> {
    const response = await ApiService.get<any>(`/enrollments/${enrollmentId}`)
    
    if (response.success && response.data) {
      const enrollment = this.mapBackendEnrollmentToFrontend(response.data)
      return { success: true, data: enrollment }
    }
    
    return response
  }

  // Get courses by department - if supported
  async getCoursesByDepartment(department: string, params?: CoursePaginationParams): Promise<ApiResponse<Course[]>> {
    const searchParams = {
      department,
      ...params
    }
    return await this.getCourses(searchParams)
  }

  // Get courses by instructor - if supported
  async getCoursesByInstructor(instructor: string, params?: CoursePaginationParams): Promise<ApiResponse<Course[]>> {
    const searchParams = {
      instructor,
      ...params
    }
    return await this.getCourses(searchParams)
  }

  // Helper method to map backend course data to frontend Course interface
  private mapBackendCourseToFrontend(backendCourse: any): Course {
    return {
      id: backendCourse.courseId?.toString() || backendCourse.id?.toString() || '',
      courseCode: backendCourse.courseCode || '',
      title: backendCourse.title || backendCourse.name || '',
      description: backendCourse.description || '',
      credits: backendCourse.credits || 3,
      instructor: backendCourse.instructor || 'TBA',
      department: backendCourse.department || 'General',
      level: backendCourse.level || 'Undergraduate',
      prerequisites: backendCourse.prerequisites || [],
      schedule: backendCourse.schedule || {
        days: ['TBA'],
        time: 'TBA',
        location: 'TBA'
      },
      capacity: backendCourse.capacity || 30,
      enrolled: backendCourse.enrolled || 0,
      status: backendCourse.status || 'Active',
      semester: backendCourse.semester || 'Fall 2024',
      year: backendCourse.year || 2024
    }
  }

  // Helper method to map multiple backend courses
  private mapBackendCoursesToFrontend(backendCourses: any[]): Course[] {
    if (!Array.isArray(backendCourses)) {
      return []
    }
    return backendCourses.map(course => this.mapBackendCourseToFrontend(course))
  }

  // Helper method to map backend enrollment data to frontend Enrollment interface
  private mapBackendEnrollmentToFrontend(backendEnrollment: any): Enrollment {
    return {
      id: backendEnrollment.enrollmentId?.toString() || backendEnrollment.id?.toString() || '',
      studentId: backendEnrollment.studentId?.toString() || '1',
      courseId: backendEnrollment.courseId?.toString() || '',
      course: backendEnrollment.course ? this.mapBackendCourseToFrontend(backendEnrollment.course) : undefined,
      enrollmentDate: backendEnrollment.enrollmentDate || new Date().toISOString().split('T')[0],
      status: (backendEnrollment.status as EnrollmentStatus) || 'Enrolled',
      grade: backendEnrollment.grade || null,
      credits: backendEnrollment.credits || backendEnrollment.course?.credits || 3,
      semester: backendEnrollment.semester || 'Fall 2024',
      year: backendEnrollment.year || 2024
    }
  }

  // Helper method to map multiple backend enrollments
  private mapBackendEnrollmentsToFrontend(backendEnrollments: any[]): Enrollment[] {
    if (!Array.isArray(backendEnrollments)) {
      return []
    }
    return backendEnrollments.map(enrollment => this.mapBackendEnrollmentToFrontend(enrollment))
  }

  // Utility methods for frontend use
  
  // Calculate total credits for enrollments
  calculateTotalCredits(enrollments: Enrollment[]): number {
    return enrollments
      .filter(e => e.status === 'Enrolled' || e.status === 'Completed')
      .reduce((total, enrollment) => total + enrollment.credits, 0)
  }

  // Filter enrollments by status
  filterEnrollmentsByStatus(enrollments: Enrollment[], status: EnrollmentStatus): Enrollment[] {
    return enrollments.filter(e => e.status === status)
  }

  // Get current semester enrollments
  getCurrentSemesterEnrollments(enrollments: Enrollment[]): Enrollment[] {
    const currentYear = new Date().getFullYear()
    const currentMonth = new Date().getMonth() + 1
    const currentSemester = currentMonth >= 8 ? 'Fall' : currentMonth >= 5 ? 'Summer' : 'Spring'
    
    return enrollments.filter(e => 
      e.year === currentYear && 
      e.semester === `${currentSemester} ${currentYear}`
    )
  }

  // Check if student can enroll in course (basic validation)
  canEnrollInCourse(course: Course, currentEnrollments: Enrollment[]): { canEnroll: boolean; reason?: string } {
    // Check if already enrolled
    const isAlreadyEnrolled = currentEnrollments.some(e => 
      e.courseId === course.id && 
      (e.status === 'Enrolled' || e.status === 'Pending')
    )
    
    if (isAlreadyEnrolled) {
      return { canEnroll: false, reason: 'Already enrolled in this course' }
    }

    // Check capacity
    if (course.enrolled >= course.capacity) {
      return { canEnroll: false, reason: 'Course is full' }
    }

    // Check if course is active
    if (course.status !== 'Active') {
      return { canEnroll: false, reason: 'Course is not available for enrollment' }
    }

    return { canEnroll: true }
  }
}

// Export singleton instance
export const CourseService = new CourseServiceClass()
export default CourseService