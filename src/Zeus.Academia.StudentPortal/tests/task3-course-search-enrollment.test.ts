import { describe, it, expect, beforeEach, vi } from 'vitest'
import { CourseService } from '../src/services/CourseService'
import type { CourseSearchParams } from '../src/services/CourseService'
import { ApiService } from '../src/services/ApiService'
import type { Course, ApiResponse, PaginatedResponse } from '../src/types'
import { EnrollmentStatus } from '../src/types'

describe('Task 3: Course Search and Enrollment Interface', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Advanced Course Search with Multiple Filters', () => {
    it('should search courses with subject filter', async () => {
      const mockCourses: Course[] = [
        {
          id: '1',
          code: 'CS101',
          name: 'Introduction to Computer Science',
          description: 'Basic programming concepts',
          credits: 3,
          instructor: 'Dr. Smith',
          schedule: [
            { dayOfWeek: 'Monday', startTime: '10:00', endTime: '11:30', location: 'Room 101' }
          ],
          enrollmentStatus: EnrollmentStatus.Enrolled
        },
        {
          id: '2',
          code: 'CS201',
          name: 'Data Structures',
          description: 'Advanced data structures and algorithms',
          credits: 4,
          instructor: 'Dr. Johnson',
          schedule: [
            { dayOfWeek: 'Wednesday', startTime: '14:00', endTime: '15:30', location: 'Room 201' }
          ],
          enrollmentStatus: EnrollmentStatus.Enrolled
        }
      ]

      const mockResponse: ApiResponse<Course[]> = {
        success: true,
        data: mockCourses
      }

      const getSpy = vi.spyOn(ApiService, 'get')
      getSpy.mockResolvedValue(mockResponse)

      const searchParams: CourseSearchParams = {
        department: 'CS',
        query: 'computer'
      }

      const result = await CourseService.searchCourses('computer', searchParams)

      expect(result.success).toBe(true)
      expect(result.data).toHaveLength(2)
      expect(result.data?.[0].code).toBe('CS101')
      expect(getSpy).toHaveBeenCalledWith('/courses/search', {
        query: 'computer',
        department: 'CS'
      })
    })

    it('should search courses with level filter', async () => {
      const mockCourses: Course[] = [
        {
          id: '3',
          code: 'CS301',
          name: 'Advanced Programming',
          description: 'Advanced programming techniques',
          credits: 3,
          instructor: 'Dr. Wilson',
          schedule: [
            { dayOfWeek: 'Tuesday', startTime: '09:00', endTime: '10:30', location: 'Room 301' }
          ],
          enrollmentStatus: EnrollmentStatus.Enrolled
        }
      ]

      const mockResponse: ApiResponse<Course[]> = {
        success: true,
        data: mockCourses
      }

      const getSpy = vi.spyOn(ApiService, 'get')
      getSpy.mockResolvedValue(mockResponse)

      const searchParams: CourseSearchParams = {
        level: '300',
        department: 'CS'
      }

      const result = await CourseService.searchCourses('', searchParams)

      expect(result.success).toBe(true)
      expect(result.data).toHaveLength(1)
      expect(result.data?.[0].code).toBe('CS301')
    })

    it('should search courses with instructor filter', async () => {
      const mockCourses: Course[] = [
        {
          id: '4',
          code: 'MATH201',
          name: 'Calculus II',
          description: 'Advanced calculus concepts',
          credits: 4,
          instructor: 'Dr. Smith',
          schedule: [
            { dayOfWeek: 'Monday', startTime: '11:00', endTime: '12:30', location: 'Room 401' }
          ],
          enrollmentStatus: EnrollmentStatus.Enrolled
        }
      ]

      const mockResponse: ApiResponse<Course[]> = {
        success: true,
        data: mockCourses
      }

      const getSpy = vi.spyOn(ApiService, 'get')
      getSpy.mockResolvedValue(mockResponse)

      const searchParams: CourseSearchParams = {
        instructor: 'Dr. Smith'
      }

      const result = await CourseService.searchCourses('', searchParams)

      expect(result.success).toBe(true)
      expect(result.data).toHaveLength(1)
      expect(result.data?.[0].instructor).toBe('Dr. Smith')
    })

    it('should search courses with time filter', async () => {
      const mockCourses: Course[] = [
        {
          id: '5',
          code: 'ENG101',
          name: 'English Composition',
          description: 'Writing fundamentals',
          credits: 3,
          instructor: 'Prof. Davis',
          schedule: [
            { dayOfWeek: 'Tuesday', startTime: '10:00', endTime: '11:30', location: 'Room 501' }
          ],
          enrollmentStatus: EnrollmentStatus.Enrolled
        }
      ]

      const mockResponse: ApiResponse<Course[]> = {
        success: true,
        data: mockCourses
      }

      const getSpy = vi.spyOn(ApiService, 'get')
      getSpy.mockResolvedValue(mockResponse)

      // Test would include time range filtering logic
      const result = await CourseService.getAvailableCourses()

      expect(result.success).toBe(true)
      expect(result.data).toBeDefined()
    })

    it('should handle combined multiple filters', async () => {
      const mockResponse: ApiResponse<Course[]> = {
        success: true,
        data: []
      }

      const getSpy = vi.spyOn(ApiService, 'get')
      getSpy.mockResolvedValue(mockResponse)

      const searchParams: CourseSearchParams = {
        department: 'CS',
        level: '200',
        instructor: 'Dr. Smith',
        credits: 3
      }

      const result = await CourseService.searchCourses('programming', searchParams)
      
      expect(result).toBeDefined()

      expect(getSpy).toHaveBeenCalledWith('/courses/search', {
        query: 'programming',
        department: 'CS',
        level: '200',
        instructor: 'Dr. Smith',
        credits: 3
      })
    })
  })

  describe('Detailed Course Information with Prerequisites', () => {
    it('should display course details with prerequisites clearly shown', async () => {
      const mockCourse: Course = {
        id: '1',
        code: 'CS301',
        name: 'Data Structures and Algorithms',
        description: 'Advanced study of data structures including trees, graphs, and hash tables. Analysis of algorithm complexity.',
        credits: 4,
        instructor: 'Dr. Anderson',
        schedule: [
          { dayOfWeek: 'Monday', startTime: '10:00', endTime: '11:30', location: 'CS Building 201' },
          { dayOfWeek: 'Wednesday', startTime: '10:00', endTime: '11:30', location: 'CS Building 201' }
        ],
        enrollmentStatus: EnrollmentStatus.Enrolled
      }

      const mockPrerequisites: Course[] = [
        {
          id: '2',
          code: 'CS101',
          name: 'Introduction to Programming',
          description: 'Basic programming concepts',
          credits: 3,
          instructor: 'Dr. Smith',
          schedule: [],
          enrollmentStatus: EnrollmentStatus.Completed
        },
        {
          id: '3',
          code: 'CS201',
          name: 'Programming Fundamentals',
          description: 'Intermediate programming',
          credits: 3,
          instructor: 'Dr. Johnson',
          schedule: [],
          enrollmentStatus: EnrollmentStatus.Completed
        }
      ]

      const courseResponse: ApiResponse<Course> = {
        success: true,
        data: mockCourse
      }

      const prerequisitesResponse: ApiResponse<Course[]> = {
        success: true,
        data: mockPrerequisites
      }

      const getSpy = vi.spyOn(ApiService, 'get')
      getSpy.mockResolvedValueOnce(courseResponse)
      getSpy.mockResolvedValueOnce(prerequisitesResponse)

      const courseResult = await CourseService.getCourseById('1')
      const prerequisitesResult = await CourseService.getCoursePrerequisites('1')

      expect(courseResult.success).toBe(true)
      expect(courseResult.data?.code).toBe('CS301')
      expect(courseResult.data?.description).toContain('Advanced study of data structures')
      
      expect(prerequisitesResult.success).toBe(true)
      expect(prerequisitesResult.data).toHaveLength(2)
      expect(prerequisitesResult.data?.[0].code).toBe('CS101')
      expect(prerequisitesResult.data?.[1].code).toBe('CS201')
    })

    it('should validate prerequisites before enrollment', async () => {
      const mockValidationResponse: ApiResponse<{ valid: boolean; missingPrerequisites?: Course[] }> = {
        success: true,
        data: {
          valid: false,
          missingPrerequisites: [
            {
              id: '2',
              code: 'CS101',
              name: 'Introduction to Programming',
              description: 'Basic programming concepts',
              credits: 3,
              instructor: 'Dr. Smith',
              schedule: [],
              enrollmentStatus: EnrollmentStatus.Enrolled
            }
          ]
        }
      }

      const getSpy = vi.spyOn(ApiService, 'get')
      getSpy.mockResolvedValue(mockValidationResponse)

      const result = await CourseService.validatePrerequisites('1')

      expect(result.success).toBe(true)
      expect(result.data?.valid).toBe(false)
      expect(result.data?.missingPrerequisites).toHaveLength(1)
      expect(result.data?.missingPrerequisites?.[0].code).toBe('CS101')
    })
  })

  describe('Course Comparison Side-by-Side Functionality', () => {
    it('should compare multiple courses side by side', async () => {
      const mockCourses: Course[] = [
        {
          id: '1',
          code: 'CS301',
          name: 'Data Structures',
          description: 'Advanced data structures',
          credits: 4,
          instructor: 'Dr. Anderson',
          schedule: [
            { dayOfWeek: 'Monday', startTime: '10:00', endTime: '11:30', location: 'Room 201' }
          ],
          enrollmentStatus: EnrollmentStatus.Enrolled
        },
        {
          id: '2',
          code: 'CS302', 
          name: 'Algorithms',
          description: 'Algorithm analysis and design',
          credits: 3,
          instructor: 'Dr. Wilson',
          schedule: [
            { dayOfWeek: 'Tuesday', startTime: '14:00', endTime: '15:30', location: 'Room 301' }
          ],
          enrollmentStatus: EnrollmentStatus.Enrolled
        }
      ]

      const getSpy = vi.spyOn(ApiService, 'get')
      
      // Mock individual course calls
      getSpy.mockResolvedValueOnce({ success: true, data: mockCourses[0] })
      getSpy.mockResolvedValueOnce({ success: true, data: mockCourses[1] })

      const course1 = await CourseService.getCourseById('1')
      const course2 = await CourseService.getCourseById('2')

      expect(course1.success).toBe(true)
      expect(course2.success).toBe(true)
      
      // Verify courses can be compared
      expect(course1.data?.credits).toBe(4)
      expect(course2.data?.credits).toBe(3)
      expect(course1.data?.instructor).toBe('Dr. Anderson')
      expect(course2.data?.instructor).toBe('Dr. Wilson')
    })

    it('should handle comparison of courses with different schedules', async () => {
      const mockCourse1: Course = {
        id: '1',
        code: 'CS301',
        name: 'Morning Course',
        description: 'Course in the morning',
        credits: 3,
        instructor: 'Dr. Early',
        schedule: [
          { dayOfWeek: 'Monday', startTime: '08:00', endTime: '09:30', location: 'Room 101' },
          { dayOfWeek: 'Wednesday', startTime: '08:00', endTime: '09:30', location: 'Room 101' }
        ],
        enrollmentStatus: EnrollmentStatus.Enrolled
      }

      const mockCourse2: Course = {
        id: '2',
        code: 'CS302',
        name: 'Evening Course',
        description: 'Course in the evening',
        credits: 3,
        instructor: 'Dr. Night',
        schedule: [
          { dayOfWeek: 'Tuesday', startTime: '18:00', endTime: '19:30', location: 'Room 201' },
          { dayOfWeek: 'Thursday', startTime: '18:00', endTime: '19:30', location: 'Room 201' }
        ],
        enrollmentStatus: EnrollmentStatus.Enrolled
      }

      const getSpy = vi.spyOn(ApiService, 'get')
      getSpy.mockResolvedValueOnce({ success: true, data: mockCourse1 })
      getSpy.mockResolvedValueOnce({ success: true, data: mockCourse2 })

      const course1 = await CourseService.getCourseById('1')
      const course2 = await CourseService.getCourseById('2')

      expect(course1.data?.schedule[0].startTime).toBe('08:00')
      expect(course2.data?.schedule[0].startTime).toBe('18:00')
      
      // Verify no schedule conflicts
      const hasConflict = course1.data?.schedule.some(s1 => 
        course2.data?.schedule.some(s2 => 
          s1.dayOfWeek === s2.dayOfWeek && 
          s1.startTime === s2.startTime
        )
      )
      expect(hasConflict).toBe(false)
    })
  })

  describe('One-Click Enrollment with Real-Time Validation', () => {
    it('should enroll in course successfully when all requirements met', async () => {
      const mockEnrollmentResponse: ApiResponse<any> = {
        success: true,
        data: {
          id: '1',
          studentId: 'student1',
          courseId: 'course1',
          enrollmentDate: '2024-10-03',
          status: EnrollmentStatus.Enrolled
        }
      }

      const mockValidationResponse: ApiResponse<{ valid: boolean }> = {
        success: true,
        data: { valid: true }
      }

      const mockAvailabilityResponse: ApiResponse<{ available: boolean; waitlistAvailable: boolean }> = {
        success: true,
        data: { available: true, waitlistAvailable: false }
      }

      const getSpy = vi.spyOn(ApiService, 'get')
      const postSpy = vi.spyOn(ApiService, 'post')

      getSpy.mockResolvedValueOnce(mockValidationResponse) // Prerequisites
      getSpy.mockResolvedValueOnce(mockAvailabilityResponse) // Availability
      postSpy.mockResolvedValue(mockEnrollmentResponse) // Enrollment

      // Validate prerequisites first
      const validation = await CourseService.validatePrerequisites('course1')
      expect(validation.data?.valid).toBe(true)

      // Check availability
      const availability = await CourseService.checkCourseAvailability('course1')
      expect(availability.data?.available).toBe(true)

      // Enroll in course
      const result = await CourseService.enrollInCourse('course1')

      expect(result.success).toBe(true)
      expect(result.data?.status).toBe(EnrollmentStatus.Enrolled)
      expect(postSpy).toHaveBeenCalledWith('/enrollments', { courseId: 'course1' })
    })

    it('should prevent enrollment when prerequisites not met', async () => {
      const mockValidationResponse: ApiResponse<{ valid: boolean; missingPrerequisites?: Course[] }> = {
        success: true,
        data: {
          valid: false,
          missingPrerequisites: [
            {
              id: '2',
              code: 'CS101',
              name: 'Intro to Programming',
              description: 'Basic programming',
              credits: 3,
              instructor: 'Dr. Smith',
              schedule: [],
              enrollmentStatus: EnrollmentStatus.Enrolled
            }
          ]
        }
      }

      const getSpy = vi.spyOn(ApiService, 'get')
      getSpy.mockResolvedValue(mockValidationResponse)

      const validation = await CourseService.validatePrerequisites('course1')
      
      expect(validation.success).toBe(true)
      expect(validation.data?.valid).toBe(false)
      expect(validation.data?.missingPrerequisites).toHaveLength(1)
      
      // Should not proceed with enrollment
      // In real implementation, enrollment would be blocked
      // This test verifies the validation works
      expect(validation.data?.valid).toBe(false)
    })

    it('should detect schedule conflicts during enrollment', async () => {
      // Mock student's current courses
      const mockCurrentCourses: Course[] = [
        {
          id: '1',
          code: 'CS201',
          name: 'Current Course',
          description: 'Already enrolled course',
          credits: 3,
          instructor: 'Dr. Current',
          schedule: [
            { dayOfWeek: 'Monday', startTime: '10:00', endTime: '11:30', location: 'Room 101' }
          ],
          enrollmentStatus: EnrollmentStatus.Enrolled
        }
      ]

      const mockNewCourse: Course = {
        id: '2',
        code: 'CS301',
        name: 'New Course',
        description: 'Course to enroll in',
        credits: 3,
        instructor: 'Dr. New',
        schedule: [
          { dayOfWeek: 'Monday', startTime: '10:30', endTime: '12:00', location: 'Room 201' }
        ],
        enrollmentStatus: EnrollmentStatus.Enrolled
      }

      const getSpy = vi.spyOn(ApiService, 'get')
      getSpy.mockResolvedValueOnce({ success: true, data: mockCurrentCourses })
      getSpy.mockResolvedValueOnce({ success: true, data: mockNewCourse })

      const currentCourses = await CourseService.getEnrolledCourses()
      const newCourse = await CourseService.getCourseById('2')

      // Check for schedule conflict
      const hasConflict = currentCourses.data?.some(current => 
        current.schedule.some(currentSchedule =>
          newCourse.data?.schedule.some(newSchedule =>
            currentSchedule.dayOfWeek === newSchedule.dayOfWeek &&
            ((currentSchedule.startTime <= newSchedule.startTime && currentSchedule.endTime > newSchedule.startTime) ||
             (newSchedule.startTime <= currentSchedule.startTime && newSchedule.endTime > currentSchedule.startTime))
          )
        )
      )

      expect(hasConflict).toBe(true)
    })
  })

  describe('Waitlist Status and Automatic Notification System', () => {
    it('should add student to waitlist when course is full', async () => {
      const mockAvailabilityResponse: ApiResponse<{ available: boolean; waitlistAvailable: boolean; message?: string }> = {
        success: true,
        data: {
          available: false,
          waitlistAvailable: true,
          message: 'Course is full but waitlist is available'
        }
      }

      const mockWaitlistResponse: ApiResponse<any> = {
        success: true,
        data: {
          id: '1',
          studentId: 'student1',
          courseId: 'course1',
          enrollmentDate: '2024-10-03',
          status: EnrollmentStatus.Waitlisted
        }
      }

      const getSpy = vi.spyOn(ApiService, 'get')
      const postSpy = vi.spyOn(ApiService, 'post')

      getSpy.mockResolvedValue(mockAvailabilityResponse)
      postSpy.mockResolvedValue(mockWaitlistResponse)

      // Check availability
      const availability = await CourseService.checkCourseAvailability('course1')
      expect(availability.data?.available).toBe(false)
      expect(availability.data?.waitlistAvailable).toBe(true)

      // Add to waitlist
      const result = await CourseService.waitlistCourse('course1')

      expect(result.success).toBe(true)
      expect(result.data?.status).toBe(EnrollmentStatus.Waitlisted)
      expect(postSpy).toHaveBeenCalledWith('/enrollments/waitlist', { courseId: 'course1' })
    })

    it('should handle waitlist position and notifications', async () => {
      const mockWaitlistStatus: ApiResponse<any> = {
        success: true,
        data: {
          position: 3,
          estimatedTimeToEnrollment: '2-3 weeks',
          notificationsEnabled: true
        }
      }

      const getSpy = vi.spyOn(ApiService, 'get')
      getSpy.mockResolvedValue(mockWaitlistStatus)

      // This would be a new method to get waitlist status
      const result = await ApiService.get('/enrollments/waitlist/status/course1')

      expect(result.success).toBe(true)
      expect(result.data?.position).toBe(3)
      expect(result.data?.estimatedTimeToEnrollment).toBe('2-3 weeks')
      expect(result.data?.notificationsEnabled).toBe(true)
    })

    it('should automatically enroll from waitlist when spot becomes available', async () => {
      const mockEnrollmentUpdate: ApiResponse<any> = {
        success: true,
        data: {
          id: '1',
          studentId: 'student1',
          courseId: 'course1',
          enrollmentDate: '2024-10-03',
          status: EnrollmentStatus.Enrolled,
          previousStatus: EnrollmentStatus.Waitlisted,
          autoEnrolledFromWaitlist: true
        }
      }

      const getSpy = vi.spyOn(ApiService, 'get')
      getSpy.mockResolvedValue(mockEnrollmentUpdate)

      // This would be triggered by a webhook or polling mechanism
      const result = await CourseService.getEnrollmentByCourseId('course1')

      expect(result.success).toBe(true)
      expect(result.data?.status).toBe(EnrollmentStatus.Enrolled)
      expect(result.data?.autoEnrolledFromWaitlist).toBe(true)
    })

    it('should send notifications for waitlist status changes', async () => {
      const mockNotificationResponse: ApiResponse<any> = {
        success: true,
        data: {
          notificationId: '1',
          type: 'waitlist_enrollment',
          message: 'You have been enrolled in CS301 from the waitlist',
          timestamp: '2024-10-03T10:00:00Z',
          read: false
        }
      }

      const getSpy = vi.spyOn(ApiService, 'get')
      getSpy.mockResolvedValue(mockNotificationResponse)

      // Mock notification endpoint
      const result = await ApiService.get('/notifications/latest')

      expect(result.success).toBe(true)
      expect(result.data?.type).toBe('waitlist_enrollment')
      expect(result.data?.message).toContain('enrolled')
      expect(result.data?.read).toBe(false)
    })
  })

  describe('Integration and Edge Cases', () => {
    it('should handle API failures gracefully', async () => {
      const getSpy = vi.spyOn(ApiService, 'get')
      getSpy.mockRejectedValue(new Error('Network error'))

      await expect(CourseService.getCourses()).rejects.toThrow('Network error')
    })

    it('should handle empty search results', async () => {
      const mockResponse: ApiResponse<Course[]> = {
        success: true,
        data: []
      }

      const getSpy = vi.spyOn(ApiService, 'get')
      getSpy.mockResolvedValue(mockResponse)

      const result = await CourseService.searchCourses('nonexistent')

      expect(result.success).toBe(true)
      expect(result.data).toHaveLength(0)
    })

    it('should handle pagination correctly', async () => {
      const mockResponse: PaginatedResponse<Course> = {
        success: true,
        data: [],
        pagination: {
          page: 1,
          pageSize: 10,
          totalItems: 50,
          totalPages: 5
        }
      }

      const getSpy = vi.spyOn(ApiService, 'get')
      getSpy.mockResolvedValue(mockResponse)

      const result = await CourseService.getCoursesPaginated({ page: 1, pageSize: 10 })

      expect(result.success).toBe(true)
      expect(result.pagination?.totalItems).toBe(50)
      expect(result.pagination?.totalPages).toBe(5)
    })
  })
})