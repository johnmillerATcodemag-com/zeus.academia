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
  // Get all courses for the authenticated student
  async getCourses(params?: CoursePaginationParams & { search?: string }): Promise<ApiResponse<Course[]>> {
    return await ApiService.get<Course[]>('/courses', params)
  }

  // Get paginated courses
  async getCoursesPaginated(params?: CoursePaginationParams & CourseSearchParams): Promise<PaginatedResponse<Course>> {
    return await ApiService.get<Course[]>('/courses/paginated', params) as PaginatedResponse<Course>
  }

  // Get course by ID
  async getCourseById(courseId: string): Promise<ApiResponse<Course>> {
    return await ApiService.get<Course>(`/courses/${courseId}`)
  }

  // Search courses
  async searchCourses(query: string, filters?: CourseSearchParams): Promise<ApiResponse<Course[]>> {
    const params = {
      query,
      ...filters
    }
    return await ApiService.get<Course[]>('/courses/search', params)
  }

  // Get available courses (not enrolled)
  async getAvailableCourses(params?: CoursePaginationParams & CourseSearchParams): Promise<ApiResponse<Course[]>> {
    return await ApiService.get<Course[]>('/courses/available', params)
  }

  // Get enrolled courses
  async getEnrolledCourses(params?: CoursePaginationParams): Promise<ApiResponse<Course[]>> {
    return await ApiService.get<Course[]>('/student/enrollments', params)
  }

  // Get completed courses
  async getCompletedCourses(params?: CoursePaginationParams): Promise<ApiResponse<Course[]>> {
    return await ApiService.get<Course[]>('/courses/completed', params)
  }

  // Get courses by department
  async getCoursesByDepartment(department: string, params?: CoursePaginationParams): Promise<ApiResponse<Course[]>> {
    return await ApiService.get<Course[]>(`/courses/department/${department}`, params)
  }

  // Get course prerequisites
  async getCoursePrerequisites(courseId: string): Promise<ApiResponse<Course[]>> {
    return await ApiService.get<Course[]>(`/courses/${courseId}/prerequisites`)
  }

  // Get course schedule
  async getCourseSchedule(courseId: string): Promise<ApiResponse<any>> {
    return await ApiService.get(`/courses/${courseId}/schedule`)
  }

  // Get course materials
  async getCourseMaterials(courseId: string): Promise<ApiResponse<any[]>> {
    return await ApiService.get<any[]>(`/courses/${courseId}/materials`)
  }

  // Get course announcements
  async getCourseAnnouncements(courseId: string): Promise<ApiResponse<any[]>> {
    return await ApiService.get<any[]>(`/courses/${courseId}/announcements`)
  }

  // Enrollment operations
  async enrollInCourse(courseId: string): Promise<ApiResponse<Enrollment>> {
    return await ApiService.post<Enrollment>(`/student/enroll/${courseId}`, {})
  }

  async dropCourse(courseId: string): Promise<ApiResponse<void>> {
    return await ApiService.delete(`/student/enroll/${courseId}`)
  }

  async waitlistCourse(courseId: string): Promise<ApiResponse<Enrollment>> {
    return await ApiService.post<Enrollment>(`/student/enroll/${courseId}`, { waitlist: true })
  }

  // Get student enrollments
  async getEnrollments(): Promise<ApiResponse<Enrollment[]>> {
    return await ApiService.get<Enrollment[]>('/student/enrollments')
  }

  async getEnrollmentById(enrollmentId: string): Promise<ApiResponse<Enrollment>> {
    return await ApiService.get<Enrollment>(`/enrollments/${enrollmentId}`)
  }

  async getEnrollmentByCourseId(courseId: string): Promise<ApiResponse<Enrollment>> {
    return await ApiService.get<Enrollment>(`/enrollments/course/${courseId}`)
  }

  // Get enrollment history
  async getEnrollmentHistory(): Promise<ApiResponse<Enrollment[]>> {
    return await ApiService.get<Enrollment[]>('/enrollments/history')
  }

  // Grade operations
  async getCourseGrades(courseId: string): Promise<ApiResponse<any[]>> {
    return await ApiService.get<any[]>(`/courses/${courseId}/grades`)
  }

  async getTranscript(): Promise<ApiResponse<any>> {
    return await ApiService.get('/student/transcript')
  }

  async downloadTranscript(): Promise<ApiResponse<Blob>> {
    return await ApiService.downloadFile('/student/transcript/download', 'transcript.pdf')
  }

  // Course evaluation and feedback
  async submitCourseEvaluation(courseId: string, evaluation: any): Promise<ApiResponse<void>> {
    return await ApiService.post(`/courses/${courseId}/evaluation`, evaluation)
  }

  async getCourseEvaluations(courseId: string): Promise<ApiResponse<any[]>> {
    return await ApiService.get<any[]>(`/courses/${courseId}/evaluations`)
  }

  // Academic planning
  async getPlannerCourses(semester: string, year: number): Promise<ApiResponse<Course[]>> {
    return await ApiService.get<Course[]>('/planner/courses', { semester, year })
  }

  async addToPlanner(courseId: string, semester: string, year: number): Promise<ApiResponse<void>> {
    return await ApiService.post('/planner/add', { courseId, semester, year })
  }

  async removeFromPlanner(courseId: string, semester: string, year: number): Promise<ApiResponse<void>> {
    return await ApiService.delete(`/planner/remove?courseId=${courseId}&semester=${semester}&year=${year}`)
  }

  // Course recommendations
  async getRecommendedCourses(): Promise<ApiResponse<Course[]>> {
    return await ApiService.get<Course[]>('/courses/recommendations')
  }

  // Academic progress
  async getAcademicProgress(): Promise<ApiResponse<any>> {
    return await ApiService.get('/student/progress')
  }

  async getDegreeAudit(): Promise<ApiResponse<any>> {
    return await ApiService.get('/student/degree-audit')
  }

  // Utility methods
  async checkCourseAvailability(courseId: string): Promise<ApiResponse<{ available: boolean; waitlistAvailable: boolean; message?: string }>> {
    return await ApiService.get<{ available: boolean; waitlistAvailable: boolean; message?: string }>(`/courses/${courseId}/availability`)
  }

  async validatePrerequisites(courseId: string): Promise<ApiResponse<{ valid: boolean; missingPrerequisites?: Course[] }>> {
    return await ApiService.get<{ valid: boolean; missingPrerequisites?: Course[] }>(`/courses/${courseId}/validate-prerequisites`)
  }

  // Bulk operations
  async enrollInMultipleCourses(courseIds: string[]): Promise<ApiResponse<Enrollment[]>> {
    return await ApiService.post<Enrollment[]>('/enrollments/bulk', { courseIds })
  }

  async dropMultipleCourses(courseIds: string[]): Promise<ApiResponse<void>> {
    return await ApiService.post('/enrollments/bulk-drop', { courseIds })
  }

  // Export data
  async exportCourseData(format: 'csv' | 'pdf' | 'excel' = 'csv'): Promise<ApiResponse<Blob>> {
    return await ApiService.downloadFile(`/courses/export?format=${format}`, `courses.${format}`)
  }

  async exportTranscript(format: 'pdf' | 'xml' = 'pdf'): Promise<ApiResponse<Blob>> {
    return await ApiService.downloadFile(`/student/transcript/export?format=${format}`, `transcript.${format}`)
  }
}

// Export singleton instance
export const CourseService = new CourseServiceClass()
export default CourseService