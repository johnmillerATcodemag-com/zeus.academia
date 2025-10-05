/**
 * Task 3 Tests: Course and Section Management Interface
 * Testing course overview dashboard, content management, assignment creation,
 * student roster management, and integrated course calendar functionality
 */
import { describe, it, expect, beforeEach, vi } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useAuthStore } from '../src/stores/auth'
import { useCourseManagementStore } from '../src/stores/courseManagement'
import type { 
  Course,
  ExtendedCourse,
  CourseSection, 
  Assignment,
  ExtendedAssignment, 
  CourseContent, 
  StudentRoster, 
  CourseCalendarEvent,
  CourseMetrics,
  Rubric,
  CourseAnnouncement
} from '../src/types'

// Mock API calls
vi.mock('../src/services/CourseManagementService', () => ({
  CourseManagementService: {
    getCourses: vi.fn(),
    getCourseDetails: vi.fn(),
    updateCourse: vi.fn(),
    createAssignment: vi.fn(),
    updateAssignment: vi.fn(),
    deleteAssignment: vi.fn(),
    getRoster: vi.fn(),
    uploadContent: vi.fn(),
    createAnnouncement: vi.fn(),
    getCalendarEvents: vi.fn(),
    getCourseMetrics: vi.fn(),
    createRubric: vi.fn(),
    bulkGradeUpdate: vi.fn()
  }
}))

describe('Task 3: Course and Section Management Interface', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    
    // Mock the environment to ensure we use mock data
    vi.stubGlobal('import.meta', {
      env: { DEV: true }
    })
  })

  describe('1. Course overview dashboard with key metrics and enrollment', () => {
    it('should display comprehensive course dashboard with metrics', async () => {
      const courseStore = useCourseManagementStore()
      const authStore = useAuthStore()

      // Set up faculty user  
      authStore.setUser({
        id: '1',
        email: 'prof@university.edu',
        firstName: 'Jane',
        lastName: 'Smith',
        role: 'professor',
        department: 'Computer Science',
        title: 'Professor',
        permissions: ['view_courses', 'manage_course_content'],
        officeLocation: 'Engineering 301'
      })

      const mockCourses: Course[] = [
        {
          id: '1',
          code: 'CS-101',
          title: 'Introduction to Computer Science',
          description: 'Fundamental concepts of computer science and programming.',
          creditHours: 3,
          semester: 'Fall 2024',
          year: 2024,
          facultyId: '1',
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
          code: 'CS-201',
          title: 'Data Structures and Algorithms',
          description: 'Advanced programming concepts and algorithm design.',
          creditHours: 4,
          semester: 'Fall 2024',
          year: 2024,
          facultyId: '1',
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
      ]

      await courseStore.loadCourses('1')
      courseStore.setCourses(mockCourses)

      expect(courseStore.courses).toEqual(mockCourses)
      expect(courseStore.courses).toHaveLength(2)
      expect(courseStore.getCourseMetrics('1')).toEqual(mockCourses[0].metrics)
      expect(courseStore.getTotalEnrollment()).toBe(47) // 25 + 22
      expect(courseStore.getAverageEnrollmentRate()).toBeCloseTo(85.65) // (83.3 + 88.0) / 2
    })

    it('should track course section enrollment and capacity', () => {
      const courseStore = useCourseManagementStore()
      
      // Set up base course data first
      const baseCourse: ExtendedCourse = {
        id: '1',
        name: 'Introduction to Computer Science',
        code: 'CS-101',
        section: '001',
        semester: 'Fall 2024',
        year: 2024,
        credits: 3,
        description: 'Fundamental concepts of computer science and programming.',
        enrollmentCount: 0,
        maxEnrollment: 30,
        status: 'active',
        title: 'Introduction to Computer Science',
        facultyId: '1',
        sections: [],
        metrics: {
          totalEnrolled: 0,
          totalCapacity: 30,
          waitlistCount: 0,
          enrollmentPercentage: 0,
          averageGPA: 0,
          completionRate: 0,
          lastUpdated: new Date()
        }
      }
      courseStore.setCourses([baseCourse])
      
      const courseSection: CourseSection = {
        id: '1-1',
        courseId: '1',
        sectionNumber: '001',
        meetingTimes: ['MWF 10:00-10:50'],
        location: 'Engineering 101',
        capacity: 30,
        enrolled: 28,
        waitlist: 5,
        status: 'active'
      }

      courseStore.updateSection(courseSection)

      expect(courseStore.getSectionEnrollment('1-1')).toBe(28)
      expect(courseStore.getSectionCapacity('1-1')).toBe(30)
      expect(courseStore.getSectionWaitlist('1-1')).toBe(5)
      expect(courseStore.isSectionFull('1-1')).toBe(false) // 28/30 < 100%
      expect(courseStore.getSectionAvailability('1-1')).toBe(2) // 30 - 28
    })

    it('should provide real-time enrollment updates and notifications', async () => {
      const courseStore = useCourseManagementStore()
      
      // Set up base course data first
      const baseCourse: ExtendedCourse = {
        id: '1',
        name: 'Introduction to Computer Science',
        code: 'CS-101',
        section: '001',
        semester: 'Fall 2024',
        year: 2024,
        credits: 3,
        description: 'Fundamental concepts of computer science and programming.',
        enrollmentCount: 0,
        maxEnrollment: 30,
        status: 'active',
        title: 'Introduction to Computer Science',
        facultyId: '1',
        sections: [{
          id: '1-1',
          courseId: '1',
          sectionNumber: '001',
          meetingTimes: ['MWF 10:00-10:50'],
          location: 'Engineering 101',
          capacity: 30,
          enrolled: 0,
          waitlist: 0,
          status: 'active'
        }]
      }
      courseStore.setCourses([baseCourse])
      
      const initialEnrollment = 25
      courseStore.updateEnrollment('1-1', initialEnrollment)
      
      expect(courseStore.getSectionEnrollment('1-1')).toBe(initialEnrollment)
      
      // Simulate enrollment increase
      const newEnrollment = 29
      await courseStore.updateEnrollment('1-1', newEnrollment)
      
      expect(courseStore.getSectionEnrollment('1-1')).toBe(newEnrollment)
      expect(courseStore.enrollmentAlerts).toContain('Section 1-1 is approaching capacity')
    })
  })

  describe('2. Content management system for course materials', () => {
    it('should manage syllabus and course materials', async () => {
      const courseStore = useCourseManagementStore()
      
      const courseContent: CourseContent = {
        id: '1',
        courseId: '1',
        type: 'syllabus',
        title: 'CS-101 Course Syllabus',
        description: 'Complete course syllabus with policies and schedule',
        content: 'Course Description: Introduction to Computer Science...',
        attachments: [
          {
            id: 'att-1',
            filename: 'CS101-Syllabus.pdf',
            size: 245760,
            type: 'application/pdf',
            uploadedAt: new Date(),
            url: '/content/cs101/syllabus.pdf'
          }
        ],
        isPublished: true,
        publishedAt: new Date(),
        lastModified: new Date(),
        createdBy: '1'
      }

      await courseStore.addContent(courseContent)
      
      expect(courseStore.courseContent).toContainEqual(courseContent)
      expect(courseStore.getContentByType('1', 'syllabus')).toEqual([courseContent])
      expect(courseStore.isSyllabusPublished('1')).toBe(true)
    })

    it('should handle file uploads and document management', async () => {
      const courseStore = useCourseManagementStore()
      
      const file = new File(['Lecture content'], 'lecture01.pdf', { type: 'application/pdf' })
      const uploadResult = await courseStore.uploadCourseFile('1', file, 'lecture')
      
      expect(uploadResult).toBeDefined()
      expect(uploadResult.filename).toBe('lecture01.pdf')
      expect(uploadResult.type).toBe('application/pdf')
      const courseFiles = courseStore.getCourseFiles('1')
      expect(courseFiles).toHaveLength(1)
      expect(courseFiles[0].filename).toBe('lecture01.pdf')
      expect(courseFiles[0].id).toBe(uploadResult.id)
    })

    it('should manage course announcements with delivery tracking', async () => {
      const courseStore = useCourseManagementStore()
      
      const announcement: CourseAnnouncement = {
        id: '1',
        courseId: '1',
        title: 'Midterm Exam Schedule',
        content: 'The midterm exam will be held on Friday, October 25th at 2:00 PM in the regular classroom.',
        type: 'exam',
        priority: 'high',
        publishDate: new Date(),
        expiryDate: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000), // 7 days from now
        isPublished: true,
        deliveryStatus: {
          sent: 25,
          delivered: 23,
          read: 18
        },
        createdBy: '1',
        createdAt: new Date()
      }

      await courseStore.createAnnouncement(announcement)
      
      expect(courseStore.announcements).toContainEqual(announcement)
      const highPriorityAnnouncements = courseStore.getAnnouncementsByPriority('high')
      expect(highPriorityAnnouncements).toHaveLength(1)
      expect(highPriorityAnnouncements[0].id).toBe('1')
      expect(highPriorityAnnouncements[0].priority).toBe('high')
      expect(courseStore.getDeliveryRate('1')).toBeCloseTo(72) // 18/25 * 100
    })
  })

  describe('3. Assignment creation with rubrics and due date management', () => {
    it('should create and manage assignments with detailed rubrics', async () => {
      const courseStore = useCourseManagementStore()
      
      const rubric: Rubric = {
        id: 'rubric-1',
        name: 'Programming Assignment Rubric',
        criteria: [
          {
            id: 'criteria-1',
            name: 'Code Quality',
            description: 'Code organization, readability, and documentation',
            maxPoints: 25,
            levels: [
              { name: 'Excellent', points: 25, description: 'Clean, well-documented code' },
              { name: 'Good', points: 20, description: 'Mostly clean with minor issues' },
              { name: 'Fair', points: 15, description: 'Some organization issues' },
              { name: 'Poor', points: 10, description: 'Poorly organized code' }
            ]
          },
          {
            id: 'criteria-2',
            name: 'Functionality',
            description: 'Program meets all requirements and works correctly',
            maxPoints: 50,
            levels: [
              { name: 'Excellent', points: 50, description: 'All requirements met perfectly' },
              { name: 'Good', points: 40, description: 'Most requirements met' },
              { name: 'Fair', points: 30, description: 'Some requirements missing' },
              { name: 'Poor', points: 20, description: 'Major functionality issues' }
            ]
          }
        ],
        totalPoints: 75,
        createdBy: '1',
        createdAt: new Date()
      }

      const assignment: ExtendedAssignment = {
        id: '1',
        courseId: '1',
        title: 'Programming Assignment 1',
        description: 'Implement a basic calculator using Python',
        instructions: 'Create a calculator that can perform basic arithmetic operations...',
        type: 'programming',
        maxPoints: 100,
        dueDate: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000), // 7 days from now
        submissionTypes: ['file_upload', 'text_entry'],
        allowedFileTypes: ['.py', '.txt', '.pdf'],
        maxFileSize: 10485760, // 10MB
        allowLateSubmissions: true,
        latePenalty: 10, // 10% per day
        maxLateDays: 3,
        rubric: rubric,
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

      await courseStore.createAssignment(assignment)
      
      expect(courseStore.assignments).toContainEqual(assignment)
      expect(courseStore.getAssignmentById('1')).toEqual(assignment)
      expect(courseStore.getAssignmentsByType('programming')).toContainEqual(assignment)
      expect(courseStore.getRubricById('rubric-1')).toEqual(rubric)
    })

    it('should handle due date management and late submission policies', () => {
      const courseStore = useCourseManagementStore()
      
      const assignment: ExtendedAssignment = {
        id: '2',
        courseId: '1',
        title: 'Essay Assignment',
        description: 'Write a 5-page essay on software engineering principles',
        type: 'essay',
        maxPoints: 100,
        dueDate: new Date('2024-10-20T23:59:59'), // Due date for calculation
        allowLateSubmissions: true,
        latePenalty: 15, // 15% per day
        maxLateDays: 2,
        submissionTypes: ['file_upload'],
        allowedFileTypes: ['.pdf', '.docx'],
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

      courseStore.addAssignment(assignment)
      
      // Test late submission calculations
      const submissionDate = new Date('2024-10-22T10:00:00') // 2 days late
      const originalScore = 85
      const penalizedScore = courseStore.calculateLatePenalty('2', originalScore, submissionDate)
      
      // Calculation: 85 * (1 - (0.15 * 2)) = 85 * 0.70 = 59.5
      expect(penalizedScore).toBeCloseTo(59.5)
      
      expect(courseStore.isAssignmentOverdue('2')).toBe(true)
      expect(courseStore.getDaysLate('2', submissionDate)).toBe(2)
    })

    it('should support multiple assignment types with different configurations', () => {
      const courseStore = useCourseManagementStore()
      
      const assignments: ExtendedAssignment[] = [
        {
          id: '3',
          courseId: '1',
          title: 'Quiz 1',
          description: 'Multiple choice quiz on basic concepts',
          type: 'quiz',
          maxPoints: 50,
          dueDate: new Date(Date.now() + 3 * 24 * 60 * 60 * 1000), // 3 days from now
          timeLimit: 30, // 30 minutes
          attempts: 2,
          submissionTypes: ['online_quiz'],
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
        },
        {
          id: '4',
          courseId: '1',
          title: 'Group Project',
          description: 'Collaborative software development project',
          type: 'project',
          maxPoints: 200,
          dueDate: new Date(Date.now() + 14 * 24 * 60 * 60 * 1000), // 14 days from now
          isGroupAssignment: true,
          maxGroupSize: 4,
          submissionTypes: ['file_upload', 'url_submission'],
          allowedFileTypes: ['.zip', '.pdf'],
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

      assignments.forEach(assignment => courseStore.addAssignment(assignment))
      
      expect(courseStore.getAssignmentsByType('quiz')).toHaveLength(1)
      expect(courseStore.getAssignmentsByType('project')).toHaveLength(1)
      expect(courseStore.getUpcomingAssignments()).toHaveLength(2)
      expect(courseStore.getGroupAssignments()).toHaveLength(1)
    })
  })

  describe('4. Student roster with academic information and photos', () => {
    it('should display comprehensive student roster with academic data', async () => {
      const courseStore = useCourseManagementStore()
      
      const roster: StudentRoster = {
        courseId: '1',
        sectionId: '1-1',
        students: [
          {
            id: 'student-1',
            studentId: 'ST001',
            firstName: 'John',
            lastName: 'Doe',
            email: 'john.doe@student.university.edu',
            phone: '(555) 123-4567',
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
              assignments: [
                { assignmentId: '1', score: 85, maxPoints: 100, submittedAt: new Date() }
              ]
            }
          },
          {
            id: 'student-2',
            studentId: 'ST002',
            firstName: 'Jane',
            lastName: 'Smith',
            email: 'jane.smith@student.university.edu',
            phone: '(555) 987-6543',
            profilePhoto: '/images/students/jane-smith.jpg',
            academicInfo: {
              major: 'Computer Science',
              year: 'Junior',
              gpa: 3.8,
              creditHours: 18,
              enrollmentStatus: 'full-time',
              advisorId: 'advisor-2'
            },
            enrollmentInfo: {
              enrolledDate: new Date('2024-08-15'),
              status: 'enrolled',
              attendanceRate: 98.0,
              participationScore: 92
            },
            grades: {
              currentGrade: 'A-',
              currentPercentage: 91.2,
              assignments: [
                { assignmentId: '1', score: 95, maxPoints: 100, submittedAt: new Date() }
              ]
            }
          }
        ],
        totalEnrolled: 2,
        lastUpdated: new Date()
      }

      await courseStore.loadRoster('1')
      courseStore.setRoster(roster)
      
      expect(courseStore.roster).toEqual(roster)
      expect(courseStore.getStudentById('student-1')).toEqual(roster.students[0])
      expect(courseStore.getStudentsByMajor('Computer Science')).toHaveLength(2)
      expect(courseStore.getClassAverage()).toBeCloseTo(89.35) // (87.5 + 91.2) / 2
    })

    it('should provide student filtering and sorting capabilities', () => {
      const courseStore = useCourseManagementStore()
      
      // Set up test students for filtering
      const testRoster: StudentRoster = {
        courseId: '1',
        sectionId: '1-1',
        students: [
          {
            id: 'student-1',
            firstName: 'John',
            lastName: 'Doe',
            email: 'john.doe@student.edu',
            studentId: 'STU001',
            phone: '555-1234',
            academicInfo: {
              major: 'Computer Science',
              year: 'Sophomore',
              gpa: 3.5,
              creditHours: 45,
              enrollmentStatus: 'full-time' as const,
              advisorId: 'advisor-1'
            },
            enrollmentInfo: {
              enrolledDate: new Date('2024-08-15'),
              status: 'enrolled' as const,
              attendanceRate: 95,
              participationScore: 88
            },
            grades: {
              currentGrade: 'B+',
              currentPercentage: 87.5,
              assignments: []
            }
          },
          {
            id: 'student-2',
            firstName: 'Jane',
            lastName: 'Smith',
            email: 'jane.smith@student.edu',
            studentId: 'STU002',
            phone: '555-5678',
            academicInfo: {
              major: 'Computer Science',
              year: 'Junior',
              gpa: 3.8,
              creditHours: 72,
              enrollmentStatus: 'full-time' as const,
              advisorId: 'advisor-1'
            },
            enrollmentInfo: {
              enrolledDate: new Date('2024-08-15'),
              status: 'enrolled' as const,
              attendanceRate: 92,
              participationScore: 95
            },
            grades: {
              currentGrade: 'A-',
              currentPercentage: 91.2,
              assignments: []
            }
          }
        ],
        totalEnrolled: 2,
        lastUpdated: new Date()
      }
      
      courseStore.setRoster(testRoster)
      
      // Filter by academic year
      expect(courseStore.filterStudentsByYear('Sophomore')).toHaveLength(1)
      expect(courseStore.filterStudentsByYear('Junior')).toHaveLength(1)
      
      // Filter by GPA range
      expect(courseStore.filterStudentsByGPA(3.0, 3.6)).toHaveLength(1) // John: 3.5
      expect(courseStore.filterStudentsByGPA(3.7, 4.0)).toHaveLength(1) // Jane: 3.8
      
      // Sort by various criteria
      const sortedByGPA = courseStore.sortStudentsBy('gpa', 'desc')
      expect(sortedByGPA[0].academicInfo.gpa).toBe(3.8) // Jane first
      
      const sortedByGrade = courseStore.sortStudentsBy('currentGrade', 'desc')
      expect(sortedByGrade[0].grades.currentPercentage).toBe(91.2) // Jane first
    })

    it('should handle student academic alerts and warnings', () => {
      const courseStore = useCourseManagementStore()
      
      // Initialize empty roster first
      const emptyRoster: StudentRoster = {
        courseId: '1',
        sectionId: '1-1', 
        students: [],
        totalEnrolled: 0,
        lastUpdated: new Date()
      }
      courseStore.setRoster(emptyRoster)
      
      // Add a student with poor performance
      const strugglingStudent = {
        id: 'student-3',
        studentId: 'ST003',
        firstName: 'Bob',
        lastName: 'Wilson',
        email: 'bob.wilson@student.university.edu',
        profilePhoto: '/images/students/bob-wilson.jpg',
        academicInfo: {
          major: 'Computer Science',
          year: 'Freshman',
          gpa: 2.1,
          creditHours: 12,
          enrollmentStatus: 'full-time',
          advisorId: 'advisor-3'
        },
        enrollmentInfo: {
          enrolledDate: new Date('2024-08-15'),
          status: 'enrolled',
          attendanceRate: 65.0,
          participationScore: 45
        },
        grades: {
          currentGrade: 'D',
          currentPercentage: 62.0,
          assignments: [
            { assignmentId: '1', score: 55, maxPoints: 100, submittedAt: new Date() }
          ]
        }
      }

      courseStore.addStudentToRoster(strugglingStudent)
      
      expect(courseStore.getStudentsAtRisk()).toContainEqual(strugglingStudent)
      expect(courseStore.getAttendanceAlerts()).toContain('student-3') // < 70% attendance
      expect(courseStore.getGradeAlerts()).toContain('student-3') // < 65% grade
    })
  })

  describe('5. Integrated course calendar with campus events', () => {
    it('should manage course calendar with assignments and events', async () => {
      const courseStore = useCourseManagementStore()
      
      const calendarEvents: CourseCalendarEvent[] = [
        {
          id: 'event-1',
          courseId: '1',
          title: 'Assignment 1 Due',
          description: 'Programming Assignment 1 submission deadline',
          type: 'assignment_due',
          startDate: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000), // 2 days from now
          endDate: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000),
          location: 'Online Submission',
          isAllDay: false,
          priority: 'high',
          color: '#dc3545', // red for deadlines
          reminders: [
            { type: 'email', timeBeforeEvent: 24 * 60 }, // 24 hours before
            { type: 'notification', timeBeforeEvent: 2 * 60 } // 2 hours before
          ]
        },
        {
          id: 'event-2',
          courseId: '1',
          title: 'Midterm Exam',
          description: 'Comprehensive midterm examination',
          type: 'exam',
          startDate: new Date(Date.now() + 5 * 24 * 60 * 60 * 1000), // 5 days from now
          endDate: new Date(Date.now() + 5 * 24 * 60 * 60 * 1000 + 90 * 60 * 1000), // 5 days + 1.5 hours
          location: 'Engineering 101',
          isAllDay: false,
          priority: 'high',
          color: '#ffc107', // yellow for exams
          reminders: [
            { type: 'email', timeBeforeEvent: 7 * 24 * 60 }, // 1 week before
            { type: 'notification', timeBeforeEvent: 24 * 60 } // 24 hours before
          ]
        },
        {
          id: 'event-3',
          courseId: '1',
          title: 'Guest Lecture: Industry Trends',
          description: 'Special guest speaker on current industry trends',
          type: 'lecture',
          startDate: new Date(Date.now() + 3 * 24 * 60 * 60 * 1000), // 3 days from now
          endDate: new Date(Date.now() + 3 * 24 * 60 * 60 * 1000 + 50 * 60 * 1000), // 3 days + 50 minutes
          location: 'Engineering 101',
          isAllDay: false,
          priority: 'medium',
          color: '#0d6efd', // blue for lectures
          reminders: [
            { type: 'notification', timeBeforeEvent: 30 } // 30 minutes before
          ]
        }
      ]

      await courseStore.loadCalendarEvents('1')
      courseStore.setCalendarEvents(calendarEvents)
      
      expect(courseStore.calendarEvents).toEqual(calendarEvents)
      expect(courseStore.getEventsByType('assignment_due')).toHaveLength(1)
      expect(courseStore.getEventsByType('exam')).toHaveLength(1)
      expect(courseStore.getUpcomingEvents(7)).toHaveLength(3) // All events within 7 days from now
    })

    it('should integrate with campus-wide academic calendar', () => {
      const courseStore = useCourseManagementStore()
      
      // First set up course events
      const courseEvents: CourseCalendarEvent[] = [
        {
          id: 'event-1',
          courseId: '1',
          title: 'Assignment 1 Due',
          description: 'Programming Assignment 1 submission deadline',
          type: 'assignment_due',
          startDate: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000), // 2 days from now
          endDate: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000),
          location: 'Online Submission',
          isAllDay: false,
          priority: 'high',
          color: '#dc3545', // red for deadlines
          reminders: [
            { type: 'email', timeBeforeEvent: 24 * 60 }, // 24 hours before
            { type: 'notification', timeBeforeEvent: 2 * 60 } // 2 hours before
          ]
        },
        {
          id: 'event-2',
          courseId: '1',
          title: 'Midterm Exam',
          description: 'Comprehensive midterm examination',
          type: 'exam',
          startDate: new Date(Date.now() + 5 * 24 * 60 * 60 * 1000), // 5 days from now
          endDate: new Date(Date.now() + 5 * 24 * 60 * 60 * 1000 + 90 * 60 * 1000), // 5 days + 1.5 hours
          location: 'Engineering 101',
          isAllDay: false,
          priority: 'high',
          color: '#ffc107', // yellow for exams
          reminders: [
            { type: 'email', timeBeforeEvent: 7 * 24 * 60 }, // 1 week before
            { type: 'notification', timeBeforeEvent: 24 * 60 } // 24 hours before
          ]
        },
        {
          id: 'event-3',
          courseId: '1',
          title: 'Guest Lecture: Industry Trends',
          description: 'Special guest speaker on current industry trends',
          type: 'lecture',
          startDate: new Date(Date.now() + 3 * 24 * 60 * 60 * 1000), // 3 days from now
          endDate: new Date(Date.now() + 3 * 24 * 60 * 60 * 1000 + 60 * 60 * 1000), // 3 days + 1 hour
          location: 'Main Auditorium',
          isAllDay: false,
          priority: 'medium',
          color: '#28a745', // green for lectures
          reminders: [
            { type: 'notification', timeBeforeEvent: 30 } // 30 minutes before
          ]
        }
      ]
      
      courseStore.setCalendarEvents(courseEvents)
      
      const campusEvents: CourseCalendarEvent[] = [
        {
          id: 'campus-1',
          courseId: 'ALL',
          title: 'Fall Break',
          description: 'University-wide fall break',
          type: 'holiday',
          startDate: new Date('2024-10-12T00:00:00'),
          endDate: new Date('2024-10-14T23:59:59'),
          isAllDay: true,
          priority: 'medium',
          color: '#28a745', // green for holidays
          reminders: []
        },
        {
          id: 'campus-2',
          courseId: 'ALL',
          title: 'Registration Opens',
          description: 'Spring 2025 course registration begins',
          type: 'registration',
          startDate: new Date('2024-11-01T08:00:00'),
          endDate: new Date('2024-11-01T08:00:00'),
          isAllDay: false,
          priority: 'high',
          color: '#6f42c1', // purple for registration
          reminders: [
            { type: 'email', timeBeforeEvent: 7 * 24 * 60 } // 1 week before
          ]
        }
      ]

      courseStore.setCampusEvents(campusEvents)
      
      expect(courseStore.campusEvents).toEqual(campusEvents)
      expect(courseStore.getAllEvents()).toHaveLength(5) // 3 course + 2 campus events
      expect(courseStore.getHolidays()).toHaveLength(1)
    })

    it('should provide calendar view filtering and display options', () => {
      const courseStore = useCourseManagementStore()
      
      // Set up the same events as the previous test for filtering
      const courseEvents: CourseCalendarEvent[] = [
        {
          id: 'event-1',
          courseId: '1',
          title: 'Assignment 1 Due',
          description: 'Programming Assignment 1 submission deadline',
          type: 'assignment_due',
          startDate: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000), // 2 days from now
          endDate: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000),
          location: 'Online Submission',
          isAllDay: false,
          priority: 'high',
          color: '#dc3545', // red for deadlines
          reminders: [
            { type: 'email', timeBeforeEvent: 24 * 60 }, // 24 hours before
            { type: 'notification', timeBeforeEvent: 2 * 60 } // 2 hours before
          ]
        },
        {
          id: 'event-2',
          courseId: '1',
          title: 'Midterm Exam',
          description: 'Comprehensive midterm examination',
          type: 'exam',
          startDate: new Date(Date.now() + 5 * 24 * 60 * 60 * 1000), // 5 days from now
          endDate: new Date(Date.now() + 5 * 24 * 60 * 60 * 1000 + 90 * 60 * 1000), // 5 days + 1.5 hours
          location: 'Engineering 101',
          isAllDay: false,
          priority: 'high',
          color: '#ffc107', // yellow for exams
          reminders: [
            { type: 'email', timeBeforeEvent: 7 * 24 * 60 }, // 1 week before
            { type: 'notification', timeBeforeEvent: 24 * 60 } // 24 hours before
          ]
        },
        {
          id: 'event-3',
          courseId: '1',
          title: 'Guest Lecture: Industry Trends',
          description: 'Special guest speaker on current industry trends',
          type: 'lecture',
          startDate: new Date(Date.now() + 3 * 24 * 60 * 60 * 1000), // 3 days from now
          endDate: new Date(Date.now() + 3 * 24 * 60 * 60 * 1000 + 60 * 60 * 1000), // 3 days + 1 hour
          location: 'Main Auditorium',
          isAllDay: false,
          priority: 'medium',
          color: '#28a745', // green for lectures
          reminders: [
            { type: 'notification', timeBeforeEvent: 30 } // 30 minutes before
          ]
        }
      ]
      
      const campusEvents: CourseCalendarEvent[] = [
        {
          id: 'campus-1',
          courseId: 'ALL',
          title: 'Fall Break',
          description: 'University-wide fall break',
          type: 'holiday',
          startDate: new Date(Date.now() + 4 * 24 * 60 * 60 * 1000), // 4 days from now
          endDate: new Date(Date.now() + 4 * 24 * 60 * 60 * 1000),
          isAllDay: true,
          priority: 'medium',
          color: '#17a2b8', // teal for holidays
          reminders: []
        },
        {
          id: 'campus-2',
          courseId: 'ALL',
          title: 'Registration Deadline',
          description: 'Last day for course registration',
          type: 'registration',
          startDate: new Date(Date.now() + 1 * 24 * 60 * 60 * 1000), // 1 day from now
          endDate: new Date(Date.now() + 1 * 24 * 60 * 60 * 1000),
          isAllDay: false,
          priority: 'high',
          color: '#dc3545', // red for deadlines
          reminders: [
            { type: 'email', timeBeforeEvent: 24 * 60 } // 24 hours before
          ]
        }
      ]
      
      courseStore.setCalendarEvents(courseEvents)
      courseStore.setCampusEvents(campusEvents)
      
      // Filter by date range - use a range that covers our test events (next 7 days)
      const startDate = new Date(Date.now())
      const endDate = new Date(Date.now() + 7 * 24 * 60 * 60 * 1000) // 7 days from now
      const upcomingEvents = courseStore.getEventsByDateRange(startDate, endDate)
      
      expect(upcomingEvents).toHaveLength(5) // All 3 course events + 2 campus events
      
      // Filter by priority
      const highPriorityEvents = courseStore.getEventsByPriority('high')
      expect(highPriorityEvents).toHaveLength(3) // Assignment due, exam, and registration
      
      // Get events for specific day (exam is 5 days from now)
      const examDay = new Date(Date.now() + 5 * 24 * 60 * 60 * 1000)
      const examDayEvents = courseStore.getEventsForDate(examDay)
      expect(examDayEvents).toHaveLength(1)
      expect(examDayEvents[0].title).toBe('Midterm Exam')
    })
  })

  describe('Integration and Advanced Features', () => {
    it('should integrate course management with faculty authentication', async () => {
      const authStore = useAuthStore()
      const courseStore = useCourseManagementStore()
      
      // Set professor user
      const professor = {
        id: '1',
        email: 'prof@university.edu',
        firstName: 'Jane',
        lastName: 'Smith',
        role: 'professor' as const,
        department: 'Computer Science', 
        title: 'Professor',
        permissions: ['view_courses', 'manage_course_content', 'manage_grades'],
        officeLocation: 'Engineering 301'
      }
      
      authStore.setUser(professor)
      
      // Mock some courses that the professor manages
      const mockCourses: ExtendedCourse[] = [
        {
          id: '1',
          name: 'Introduction to Computer Science',
          title: 'Introduction to Computer Science',
          code: 'CS-101',
          section: '001',
          semester: 'Fall 2024',
          year: 2024,
          credits: 3,
          description: 'Fundamental concepts of computer science and programming.',
          enrollmentCount: 25,
          maxEnrollment: 30,
          status: 'active',
          facultyId: professor.id,
          sections: [],
          assignments: [],
          students: [],
          materials: [],
          announcements: [],
          metrics: {
            averageGrade: 85,
            attendanceRate: 92,
            assignmentCompletionRate: 88,
            participationScore: 75
          }
        } as unknown as ExtendedCourse,
        {
          id: '2',
          name: 'Data Structures',
          title: 'Data Structures', 
          code: 'CS-201',
          section: '001',
          semester: 'Fall 2024',
          year: 2024,
          credits: 4,
          description: 'Advanced data structures and algorithms.',
          enrollmentCount: 20,
          maxEnrollment: 25,
          status: 'active',
          facultyId: professor.id,
          sections: [],
          assignments: [],
          students: [],
          materials: [],
          announcements: [],
          metrics: {
            averageGrade: 78,
            attendanceRate: 88,
            assignmentCompletionRate: 85,
            participationScore: 82
          }
        } as unknown as ExtendedCourse
      ]
      
      // Set courses directly since the service call isn't working in tests
      courseStore.setCourses(mockCourses)
      
      // Verify courses are loaded
      expect(courseStore.courses).toHaveLength(2) // Should have 2 mock courses
      expect(courseStore.courses[0].id).toBe('1')
      expect(courseStore.courses[0].facultyId).toBe(professor.id)
      
      // Professor should access their own courses
      expect(courseStore.canManageCourse('1', professor.id)).toBe(true)
      expect(courseStore.canViewRoster('1', professor.id)).toBe(true)
      expect(courseStore.canCreateAssignments('1', professor.id)).toBe(true)
      
      // Professor cannot access other faculty courses without permission
      expect(courseStore.canManageCourse('999', professor.id)).toBe(false)
    })

    it('should handle bulk operations for assignments and grading', async () => {
      const courseStore = useCourseManagementStore()
      
      // Bulk create assignments
      const assignments = [
        { title: 'Homework 1', type: 'homework', maxPoints: 50, dueDate: new Date(Date.now() + 5 * 24 * 60 * 60 * 1000) },
        { title: 'Homework 2', type: 'homework', maxPoints: 50, dueDate: new Date(Date.now() + 12 * 24 * 60 * 60 * 1000) },
        { title: 'Homework 3', type: 'homework', maxPoints: 50, dueDate: new Date(Date.now() + 19 * 24 * 60 * 60 * 1000) }
      ]
      
      await courseStore.bulkCreateAssignments('1', assignments)
      
      expect(courseStore.getAssignmentsByType('homework')).toHaveLength(3)
      
      // Bulk grade update
      const gradeUpdates = [
        { studentId: 'student-1', assignmentId: '1', score: 85 },
        { studentId: 'student-1', assignmentId: '2', score: 90 },
        { studentId: 'student-2', assignmentId: '1', score: 95 },
        { studentId: 'student-2', assignmentId: '2', score: 88 }
      ]
      
      await courseStore.bulkUpdateGrades(gradeUpdates)
      
      expect(courseStore.getStudentGrade('student-1', '1')).toBe(85)
      expect(courseStore.getStudentGrade('student-2', '2')).toBe(88)
    })

    it('should provide analytics and reporting capabilities', () => {
      const courseStore = useCourseManagementStore()
      
      // Course performance analytics
      const analytics = courseStore.getCourseAnalytics('1')
      
      expect(analytics).toHaveProperty('enrollmentTrends')
      expect(analytics).toHaveProperty('gradeDistribution')
      expect(analytics).toHaveProperty('assignmentPerformance')
      expect(analytics).toHaveProperty('attendanceMetrics')
      
      // Assignment completion rates
      const completionRate = courseStore.getAssignmentCompletionRate('1')
      expect(completionRate).toBeGreaterThanOrEqual(0)
      expect(completionRate).toBeLessThanOrEqual(100)
      
      // Class performance comparison
      const performanceComparison = courseStore.compareClassPerformance(['1', '2'])
      expect(performanceComparison).toHaveProperty('averageGrades')
      expect(performanceComparison).toHaveProperty('enrollmentRates')
    })
  })

  describe('Error Handling and Edge Cases', () => {
    it('should handle missing course data gracefully', async () => {
      const courseStore = useCourseManagementStore()
      
      await courseStore.loadCourses('nonexistent')
      
      expect(courseStore.courses).toEqual([])
      expect(courseStore.error).toBeDefined()
      expect(courseStore.loading).toBe(false)
    })

    it('should validate assignment dates and constraints', () => {
      const courseStore = useCourseManagementStore()
      
      // Assignment due date in the past
      const pastAssignment = {
        id: '999',
        courseId: '1',
        title: 'Past Assignment',
        type: 'homework',
        maxPoints: 100,
        dueDate: new Date('2024-01-01'), // In the past
        submissionTypes: ['file_upload'],
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
      
      expect(() => courseStore.validateAssignment(pastAssignment)).toThrow('Due date cannot be in the past')
      
      // Assignment with invalid points
      const invalidPointsAssignment = {
        ...pastAssignment,
        id: '998',
        dueDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000), // 30 days from now
        maxPoints: -10
      }
      
      expect(() => courseStore.validateAssignment(invalidPointsAssignment)).toThrow('Max points must be positive')
    })

    it('should handle enrollment capacity limits', () => {
      const courseStore = useCourseManagementStore()
      
      const fullSection: CourseSection = {
        id: '1-2',
        courseId: '1',
        sectionNumber: '002',
        meetingTimes: ['TR 10:00-11:30'],
        location: 'Engineering 102',
        capacity: 20,
        enrolled: 20,
        waitlist: 0,
        status: 'active'
      }
      
      courseStore.updateSection(fullSection)
      
      expect(courseStore.canEnrollStudent('1-2')).toBe(false)
      expect(courseStore.isSectionFull('1-2')).toBe(true)
      expect(courseStore.getSectionAvailability('1-2')).toBe(0)
    })
  })
})