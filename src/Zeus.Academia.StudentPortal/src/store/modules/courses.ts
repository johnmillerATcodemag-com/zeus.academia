import { CourseService } from '../../services/CourseService'
import type { CoursesState, Course, Enrollment, ApiResponse } from '../../types'
import { EnrollmentStatus } from '../../types'

export const coursesModule = {
  namespaced: true,
  
  state: (): CoursesState => ({
    courses: [],
    enrollments: [],
    selectedCourse: null
  }),

  getters: {
    courses: (state: CoursesState) => state.courses,
    enrollments: (state: CoursesState) => state.enrollments,
    selectedCourse: (state: CoursesState) => state.selectedCourse,
    
    enrolledCourses: (state: CoursesState) => {
      return state.courses.filter(course => course.enrollmentStatus === EnrollmentStatus.Enrolled)
    },
    
    completedCourses: (state: CoursesState) => {
      return state.courses.filter(course => course.enrollmentStatus === EnrollmentStatus.Completed)
    },
    
    totalCredits: (state: CoursesState) => {
      return state.courses
        .filter(course => course.enrollmentStatus === EnrollmentStatus.Completed)
        .reduce((total, course) => total + course.credits, 0)
    },
    
    courseById: (state: CoursesState) => (id: string) => {
      return state.courses.find(course => course.id === id)
    }
  },

  mutations: {
    SET_COURSES(state: CoursesState, courses: Course[]) {
      state.courses = courses
    },

    ADD_COURSE(state: CoursesState, course: Course) {
      const existingIndex = state.courses.findIndex(c => c.id === course.id)
      if (existingIndex >= 0) {
        state.courses[existingIndex] = course
      } else {
        state.courses.push(course)
      }
    },

    UPDATE_COURSE(state: CoursesState, updatedCourse: Course) {
      const index = state.courses.findIndex(course => course.id === updatedCourse.id)
      if (index >= 0) {
        state.courses[index] = updatedCourse
      }
    },

    REMOVE_COURSE(state: CoursesState, courseId: string) {
      state.courses = state.courses.filter(course => course.id !== courseId)
    },

    SET_ENROLLMENTS(state: CoursesState, enrollments: Enrollment[]) {
      state.enrollments = enrollments
    },

    ADD_ENROLLMENT(state: CoursesState, enrollment: Enrollment) {
      const existingIndex = state.enrollments.findIndex(e => e.id === enrollment.id)
      if (existingIndex >= 0) {
        state.enrollments[existingIndex] = enrollment
      } else {
        state.enrollments.push(enrollment)
      }
    },

    UPDATE_ENROLLMENT(state: CoursesState, updatedEnrollment: Enrollment) {
      const index = state.enrollments.findIndex(enrollment => enrollment.id === updatedEnrollment.id)
      if (index >= 0) {
        state.enrollments[index] = updatedEnrollment
      }
    },

    REMOVE_ENROLLMENT(state: CoursesState, enrollmentId: string) {
      state.enrollments = state.enrollments.filter(enrollment => enrollment.id !== enrollmentId)
    },

    SET_SELECTED_COURSE(state: CoursesState, course: Course | null) {
      state.selectedCourse = course
    },

    CLEAR_COURSES(state: CoursesState) {
      state.courses = []
      state.enrollments = []
      state.selectedCourse = null
    }
  },

  actions: {
    async fetchCourses({ commit, dispatch }: any, params?: { page?: number; pageSize?: number; search?: string }): Promise<ApiResponse<Course[]>> {
      try {
        dispatch('setLoading', { courses: true }, { root: true })
        
        const response = await CourseService.getCourses(params)
        
        if (response.success && response.data) {
          commit('SET_COURSES', response.data)
        }
        
        return response
      } catch (error) {
        console.error('Fetch courses error:', error)
        return {
          success: false,
          message: 'Failed to fetch courses.'
        }
      } finally {
        dispatch('setLoading', { courses: false }, { root: true })
      }
    },

    async fetchCourseById({ commit, dispatch }: any, courseId: string): Promise<ApiResponse<Course>> {
      try {
        dispatch('setLoading', { courses: true }, { root: true })
        
        const response = await CourseService.getCourseById(courseId)
        
        if (response.success && response.data) {
          commit('ADD_COURSE', response.data)
          commit('SET_SELECTED_COURSE', response.data)
        }
        
        return response
      } catch (error) {
        console.error('Fetch course by ID error:', error)
        return {
          success: false,
          message: 'Failed to fetch course details.'
        }
      } finally {
        dispatch('setLoading', { courses: false }, { root: true })
      }
    },

    async enrollInCourse({ commit, dispatch }: any, courseId: string): Promise<ApiResponse<Enrollment>> {
      try {
        dispatch('setLoading', { courses: true }, { root: true })
        
        const response = await CourseService.enrollInCourse(courseId)
        
        if (response.success && response.data) {
          commit('ADD_ENROLLMENT', response.data)
          
          // Update course enrollment status
          const courseResponse = await CourseService.getCourseById(courseId)
          if (courseResponse.success && courseResponse.data) {
            commit('UPDATE_COURSE', courseResponse.data)
          }
        }
        
        return response
      } catch (error) {
        console.error('Enroll in course error:', error)
        return {
          success: false,
          message: 'Failed to enroll in course.'
        }
      } finally {
        dispatch('setLoading', { courses: false }, { root: true })
      }
    },

    async dropCourse({ commit, dispatch }: any, courseId: string): Promise<ApiResponse<void>> {
      try {
        dispatch('setLoading', { courses: true }, { root: true })
        
        const response = await CourseService.dropCourse(courseId)
        
        if (response.success) {
          // Update course enrollment status
          const courseResponse = await CourseService.getCourseById(courseId)
          if (courseResponse.success && courseResponse.data) {
            commit('UPDATE_COURSE', courseResponse.data)
          }
          
          // Remove enrollment
          const enrollment = await CourseService.getEnrollmentByCourseId(courseId)
          if (enrollment.success && enrollment.data) {
            commit('REMOVE_ENROLLMENT', enrollment.data.id)
          }
        }
        
        return response
      } catch (error) {
        console.error('Drop course error:', error)
        return {
          success: false,
          message: 'Failed to drop course.'
        }
      } finally {
        dispatch('setLoading', { courses: false }, { root: true })
      }
    },

    async fetchEnrollments({ commit, dispatch }: any): Promise<ApiResponse<Enrollment[]>> {
      try {
        dispatch('setLoading', { courses: true }, { root: true })
        
        const response = await CourseService.getEnrollments()
        
        if (response.success && response.data) {
          commit('SET_ENROLLMENTS', response.data)
        }
        
        return response
      } catch (error) {
        console.error('Fetch enrollments error:', error)
        return {
          success: false,
          message: 'Failed to fetch enrollments.'
        }
      } finally {
        dispatch('setLoading', { courses: false }, { root: true })
      }
    },

    async searchCourses({ commit, dispatch }: any, searchParams: { query: string; filters?: any }): Promise<ApiResponse<Course[]>> {
      try {
        dispatch('setLoading', { courses: true }, { root: true })
        
        const response = await CourseService.searchCourses(searchParams.query, searchParams.filters)
        
        if (response.success && response.data) {
          commit('SET_COURSES', response.data)
        }
        
        return response
      } catch (error) {
        console.error('Search courses error:', error)
        return {
          success: false,
          message: 'Failed to search courses.'
        }
      } finally {
        dispatch('setLoading', { courses: false }, { root: true })
      }
    },

    async fetchEnrolledCourses({ commit, dispatch }: any): Promise<ApiResponse<Course[]>> {
      try {
        dispatch('setLoading', { courses: true }, { root: true })
        
        // Try to fetch from API first
        let response: any
        
        try {
          response = await CourseService.getEnrolledCourses()
        } catch (apiError) {
          console.warn('API call failed, falling back to mock data:', apiError)
          response = { success: false, message: 'API unavailable' }
        }
        
        if (response.success && response.data) {
          // Handle the API response structure: { enrollments: [...], totalCredits: 7, semester: "Fall 2024" }
          const enrollmentData = response.data
          
          // Extract courses from enrollments and convert to Course objects
          const courses: Course[] = enrollmentData.enrollments?.map((enrollment: any) => ({
            id: enrollment.courseId.toString(),
            name: enrollment.course?.title || 'Course Title Pending',
            code: enrollment.course?.code || 'Course Code Pending',
            credits: enrollment.course?.credits || 0,
            instructor: enrollment.course?.instructor || 'Instructor to be announced',
            description: enrollment.course?.description || 'Course description will be available soon.',
            enrollmentStatus: enrollment.status || EnrollmentStatus.Enrolled,
            schedule: enrollment.course?.schedule || []
          })) || []
          
          commit('SET_COURSES', courses)
          commit('SET_ENROLLMENTS', enrollmentData.enrollments || [])
          
          return {
            success: true,
            data: courses,
            message: `Loaded ${courses.length} enrolled courses`
          }
        } else {
          // Fallback to mock data if API fails  
          console.debug('Using fallback mock data - API unavailable')
          
          // Show user-friendly notification about offline mode
          dispatch('showNotification', {
            type: 'info',
            title: 'Offline Mode',
            message: 'Unable to connect to server. Displaying sample data for demonstration.'
          }, { root: true })
          
          const mockCourses: Course[] = [
            {
              id: '1',
              name: 'Introduction to Computer Science',
              code: 'CS101',
              instructor: 'Dr. Smith',
              credits: 3,
              enrollmentStatus: EnrollmentStatus.Enrolled,
              description: 'Fundamentals of programming and computer science concepts.',
              schedule: [
                {
                  dayOfWeek: 'Monday',
                  startTime: '10:00 AM',
                  endTime: '11:30 AM',
                  location: 'Room 101'
                },
                {
                  dayOfWeek: 'Wednesday',
                  startTime: '10:00 AM',
                  endTime: '11:30 AM',
                  location: 'Room 101'
                }
              ]
            },
            {
              id: '2',
              name: 'Calculus I',
              code: 'MATH201',
              instructor: 'Prof. Johnson',
              credits: 4,
              enrollmentStatus: EnrollmentStatus.Enrolled,
              description: 'Differential and integral calculus with applications.',
              schedule: [
                {
                  dayOfWeek: 'Tuesday',
                  startTime: '2:00 PM',
                  endTime: '3:30 PM',
                  location: 'Room 205'
                },
                {
                  dayOfWeek: 'Thursday',
                  startTime: '2:00 PM',
                  endTime: '3:30 PM',
                  location: 'Room 205'
                }
              ]
            },
            {
              id: '3',
              name: 'World History',
              code: 'HIST150',
              instructor: 'Dr. Davis',
              credits: 3,
              enrollmentStatus: EnrollmentStatus.Completed,
              description: 'Survey of world civilizations from ancient to modern times.',
              schedule: [
                {
                  dayOfWeek: 'Monday',
                  startTime: '1:00 PM',
                  endTime: '2:30 PM',
                  location: 'Room 302'
                }
              ]
            }
          ]
          
          commit('SET_COURSES', mockCourses)
          
          return {
            success: true,
            data: mockCourses,
            message: 'Using mock data (API unavailable)'
          }
        }
      } catch (error) {
        console.error('Fetch enrolled courses error:', error)
        
        // Return comprehensive mock data on error
        const mockCourses: Course[] = [
          {
            id: '1',
            name: 'Introduction to Computer Science',
            code: 'CS101',
            instructor: 'Dr. Smith',
            credits: 3,
            enrollmentStatus: EnrollmentStatus.Enrolled,
            description: 'Fundamentals of programming and computer science concepts.',
            schedule: [
              {
                dayOfWeek: 'Monday',
                startTime: '10:00 AM',
                endTime: '11:30 AM',
                location: 'Room 101'
              },
              {
                dayOfWeek: 'Wednesday',
                startTime: '10:00 AM',
                endTime: '11:30 AM',
                location: 'Room 101'
              }
            ]
          },
          {
            id: '2',
            name: 'Calculus II',
            code: 'MATH202',
            instructor: 'Dr. Johnson',
            credits: 4,
            enrollmentStatus: EnrollmentStatus.Enrolled,
            description: 'Advanced calculus topics including integration techniques and series.',
            schedule: [
              {
                dayOfWeek: 'Tuesday',
                startTime: '9:00 AM',
                endTime: '10:30 AM',
                location: 'Room 205'
              },
              {
                dayOfWeek: 'Thursday',
                startTime: '9:00 AM',
                endTime: '10:30 AM',
                location: 'Room 205'
              }
            ]
          },
          {
            id: '3',
            name: 'Physics Lab',
            code: 'PHYS151L',
            instructor: 'Dr. Wilson',
            credits: 1,
            enrollmentStatus: EnrollmentStatus.Enrolled,
            description: 'Laboratory experiments in introductory physics.',
            schedule: [
              {
                dayOfWeek: 'Friday',
                startTime: '2:00 PM',
                endTime: '4:00 PM',
                location: 'Physics Lab'
              }
            ]
          }
        ]
        
        commit('SET_COURSES', mockCourses)
        
        return {
          success: true,
          data: mockCourses,
          message: 'Using mock data (Network error)'
        }
      } finally {
        dispatch('setLoading', { courses: false }, { root: true })
      }
    },

    clearCourses({ commit }: any) {
      commit('CLEAR_COURSES')
    }
  }
}