/**
 * Course Management Store
 * Manages course overview, content, assignments, roster, and calendar functionality
 */
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { 
  ExtendedCourse,
  CourseSection, 
  ExtendedAssignment, 
  CourseContent, 
  CourseAnnouncement,
  StudentRoster, 
  RosterStudent,
  CourseCalendarEvent,
  CourseMetrics,
  Rubric,
  FileAttachment
} from '../types'
import { CourseManagementService } from '../services/CourseManagementService'

export const useCourseManagementStore = defineStore('courseManagement', () => {
  // State
  const courses = ref<ExtendedCourse[]>([])
  const currentCourse = ref<ExtendedCourse | null>(null)
  const assignments = ref<ExtendedAssignment[]>([])
  const courseContent = ref<CourseContent[]>([])
  const announcements = ref<CourseAnnouncement[]>([])
  const roster = ref<StudentRoster | null>(null)
  const calendarEvents = ref<CourseCalendarEvent[]>([])
  const campusEvents = ref<CourseCalendarEvent[]>([])
  const enrollmentAlerts = ref<string[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)
  const gradeMap = ref(new Map<string, number>())

  // Computed Properties
  const totalEnrollment = computed(() => {
    return courses.value.reduce((total, course) => {
      return total + (course.metrics?.totalEnrolled || 0)
    }, 0)
  })

  const averageEnrollmentRate = computed(() => {
    if (courses.value.length === 0) return 0
    const totalRate = courses.value.reduce((sum, course) => {
      return sum + (course.metrics?.enrollmentPercentage || 0)
    }, 0)
    return totalRate / courses.value.length
  })

  const upcomingAssignments = computed(() => {
    const now = new Date()
    return assignments.value
      .filter(assignment => assignment.dueDate >= now && assignment.isPublished)
      .sort((a, b) => a.dueDate.getTime() - b.dueDate.getTime())
  })

  const groupAssignments = computed(() => {
    return assignments.value.filter(assignment => assignment.isGroupAssignment)
  })

  const allEvents = computed(() => {
    return [...calendarEvents.value, ...campusEvents.value]
      .sort((a, b) => a.startDate.getTime() - b.startDate.getTime())
  })

  // Actions
  async function loadCourses(facultyId: string) {
    loading.value = true
    error.value = null
    
    try {
      const coursesData = await CourseManagementService.getCourses(facultyId)
      courses.value = coursesData || []
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load courses'
      courses.value = []
    } finally {
      loading.value = false
    }
  }

  function setCourses(newCourses: ExtendedCourse[]) {
    courses.value = newCourses
  }

  function getCourseMetrics(courseId: string): CourseMetrics | undefined {
    const course = courses.value.find(c => c.id === courseId)
    return course?.metrics
  }

  function getTotalEnrollment(): number {
    return totalEnrollment.value
  }

  function getAverageEnrollmentRate(): number {
    return averageEnrollmentRate.value
  }

  // Section Management
  function updateSection(section: CourseSection) {
    const course = courses.value.find(c => c.id === section.courseId)
    if (course) {
      const sectionIndex = course.sections.findIndex(s => s.id === section.id)
      if (sectionIndex >= 0) {
        course.sections[sectionIndex] = section
      } else {
        course.sections.push(section)
      }
    }
  }

  function getSectionEnrollment(sectionId: string): number {
    for (const course of courses.value) {
      const section = course.sections.find(s => s.id === sectionId)
      if (section) return section.enrolled
    }
    return 0
  }

  function getSectionCapacity(sectionId: string): number {
    for (const course of courses.value) {
      const section = course.sections.find(s => s.id === sectionId)
      if (section) return section.capacity
    }
    return 0
  }

  function getSectionWaitlist(sectionId: string): number {
    for (const course of courses.value) {
      const section = course.sections.find(s => s.id === sectionId)
      if (section) return section.waitlist
    }
    return 0
  }

  function isSectionFull(sectionId: string): boolean {
    const enrolled = getSectionEnrollment(sectionId)
    const capacity = getSectionCapacity(sectionId)
    return enrolled >= capacity
  }

  function getSectionAvailability(sectionId: string): number {
    const enrolled = getSectionEnrollment(sectionId)
    const capacity = getSectionCapacity(sectionId)
    return Math.max(0, capacity - enrolled)
  }

  async function updateEnrollment(sectionId: string, newEnrollment: number) {
    const capacity = getSectionCapacity(sectionId)
    
    // Update enrollment
    for (const course of courses.value) {
      const section = course.sections.find(s => s.id === sectionId)
      if (section) {
        section.enrolled = newEnrollment
        
        // Check for capacity alerts
        const utilizationRate = (newEnrollment / capacity) * 100
        if (utilizationRate >= 90) {
          const alertMessage = `Section ${sectionId} is approaching capacity`
          if (!enrollmentAlerts.value.includes(alertMessage)) {
            enrollmentAlerts.value.push(alertMessage)
          }
        }
        break
      }
    }
  }

  function canEnrollStudent(sectionId: string): boolean {
    return getSectionAvailability(sectionId) > 0
  }

  // Content Management
  async function addContent(content: CourseContent) {
    courseContent.value.push(content)
  }

  function getContentByType(courseId: string, type: string): CourseContent[] {
    return courseContent.value.filter(content => 
      content.courseId === courseId && content.type === type
    )
  }

  function isSyllabusPublished(courseId: string): boolean {
    const syllabus = courseContent.value.find(content => 
      content.courseId === courseId && content.type === 'syllabus'
    )
    return syllabus?.isPublished || false
  }

  async function uploadCourseFile(courseId: string, file: File, _contentType: string): Promise<FileAttachment> {
    // Mock implementation - in real app would upload to server
    const attachment: FileAttachment = {
      id: `file_${Date.now()}`,
      filename: file.name,
      size: file.size,
      type: file.type,
      uploadedAt: new Date(),
      url: `/content/${courseId}/${file.name}`
    }
    
    // Add to course content to make it retrievable
    const existingContent = courseContent.value.find(content => 
      content.courseId === courseId && content.type === 'resource'
    )
    
    if (existingContent) {
      if (!existingContent.attachments) {
        existingContent.attachments = []
      }
      existingContent.attachments.push(attachment)
    } else {
      courseContent.value.push({
        id: `content_${Date.now()}`,
        courseId,
        type: 'resource',
        title: 'Uploaded Files',
        content: '',
        isPublished: true,
        publishedAt: new Date(),
        lastModified: new Date(),
        createdBy: '1',
        attachments: [attachment]
      })
    }
    
    return attachment
  }

  function getCourseFiles(courseId: string): FileAttachment[] {
    return courseContent.value
      .filter(content => content.courseId === courseId)
      .flatMap(content => content.attachments || [])
  }

  // Announcement Management
  async function createAnnouncement(announcement: CourseAnnouncement) {
    announcements.value.push(announcement)
  }

  function getAnnouncementsByPriority(priority: string): CourseAnnouncement[] {
    return announcements.value.filter(announcement => announcement.priority === priority)
  }

  function getDeliveryRate(announcementId: string): number {
    const announcement = announcements.value.find(a => a.id === announcementId)
    if (!announcement?.deliveryStatus) return 0
    
    const { sent, read } = announcement.deliveryStatus
    return sent > 0 ? (read / sent) * 100 : 0
  }

  // Assignment Management
  async function createAssignment(assignment: ExtendedAssignment) {
    // Validate assignment before adding
    validateAssignment(assignment)
    assignments.value.push(assignment)
  }

  function addAssignment(assignment: ExtendedAssignment) {
    assignments.value.push(assignment)
  }

  function getAssignmentById(id: string): ExtendedAssignment | undefined {
    return assignments.value.find(assignment => assignment.id === id)
  }

  function getAssignmentsByType(type: string): ExtendedAssignment[] {
    return assignments.value.filter(assignment => assignment.type === type)
  }

  function getRubricById(id: string): Rubric | undefined {
    for (const assignment of assignments.value) {
      if (assignment.rubric?.id === id) {
        return assignment.rubric
      }
    }
    return undefined
  }

  function getUpcomingAssignments(): ExtendedAssignment[] {
    return upcomingAssignments.value
  }

  function getGroupAssignments(): ExtendedAssignment[] {
    return groupAssignments.value
  }

  function calculateLatePenalty(assignmentId: string, originalScore: number, submissionDate: Date): number {
    const assignment = getAssignmentById(assignmentId)
    if (!assignment || !assignment.allowLateSubmissions) return originalScore
    
    const daysLate = getDaysLate(assignmentId, submissionDate)
    if (daysLate <= 0) return originalScore
    
    const penalty = (assignment.latePenalty || 0) / 100
    const totalPenalty = penalty * daysLate
    const penalizedScore = originalScore * (1 - totalPenalty)
    
    return Math.max(0, penalizedScore)
  }

  function isAssignmentOverdue(assignmentId: string): boolean {
    const assignment = getAssignmentById(assignmentId)
    if (!assignment) return false
    
    return new Date() > assignment.dueDate
  }

  function getDaysLate(assignmentId: string, submissionDate: Date): number {
    const assignment = getAssignmentById(assignmentId)
    if (!assignment) return 0
    
    const timeDiff = submissionDate.getTime() - assignment.dueDate.getTime()
    return Math.max(0, Math.ceil(timeDiff / (24 * 60 * 60 * 1000)))
  }

  function validateAssignment(assignment: ExtendedAssignment): void {
    if (assignment.dueDate < new Date()) {
      throw new Error('Due date cannot be in the past')
    }
    
    if (assignment.maxPoints <= 0) {
      throw new Error('Max points must be positive')
    }
    
    if (assignment.isGroupAssignment && (!assignment.maxGroupSize || assignment.maxGroupSize <= 0)) {
      throw new Error('Group assignments must have a valid maximum group size')
    }
  }

  async function bulkCreateAssignments(courseId: string, assignmentData: Partial<ExtendedAssignment>[]): Promise<void> {
    const newAssignments = assignmentData.map((data, index) => ({
      id: `bulk_${courseId}_${Date.now()}_${index}`,
      courseId,
      title: data.title || 'New Assignment',
      type: data.type || 'homework',
      maxPoints: data.maxPoints || 100,
      dueDate: data.dueDate || new Date(),
      submissionTypes: ['file_upload'] as const,
      isPublished: true,
      publishedAt: new Date(),
      createdBy: '1', // Current user
      createdAt: new Date(),
      submissions: [],
      statistics: {
        submitted: 0,
        graded: 0,
        averageScore: 0,
        highestScore: 0,
        lowestScore: 0
      },
      ...data
    })) as ExtendedAssignment[]

    for (const assignment of newAssignments) {
      await createAssignment(assignment)
    }
  }

  // Roster Management
  async function loadRoster(courseId: string) {
    loading.value = true
    error.value = null
    
    try {
      const rosterData = await CourseManagementService.getRoster(courseId)
      roster.value = rosterData
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load roster'
      roster.value = null
    } finally {
      loading.value = false
    }
  }

  function setRoster(newRoster: StudentRoster) {
    roster.value = newRoster
  }

  function getStudentById(id: string): RosterStudent | undefined {
    return roster.value?.students.find(student => student.id === id)
  }

  function getStudentsByMajor(major: string): RosterStudent[] {
    return roster.value?.students.filter(student => student.academicInfo.major === major) || []
  }

  function getClassAverage(): number {
    if (!roster.value?.students.length) return 0
    
    const total = roster.value.students.reduce((sum, student) => {
      return sum + student.grades.currentPercentage
    }, 0)
    
    return total / roster.value.students.length
  }

  function filterStudentsByYear(year: string): RosterStudent[] {
    return roster.value?.students.filter(student => student.academicInfo.year === year) || []
  }

  function filterStudentsByGPA(minGPA: number, maxGPA: number): RosterStudent[] {
    return roster.value?.students.filter(student => {
      const gpa = student.academicInfo.gpa
      return gpa >= minGPA && gpa <= maxGPA
    }) || []
  }

  function sortStudentsBy(criteria: string, order: 'asc' | 'desc' = 'asc'): RosterStudent[] {
    if (!roster.value?.students) return []
    
    const students = [...roster.value.students]
    
    students.sort((a, b) => {
      let aValue: any, bValue: any
      
      switch (criteria) {
        case 'gpa':
          aValue = a.academicInfo.gpa
          bValue = b.academicInfo.gpa
          break
        case 'currentGrade':
          aValue = a.grades.currentPercentage
          bValue = b.grades.currentPercentage
          break
        case 'lastName':
          aValue = a.lastName
          bValue = b.lastName
          break
        default:
          return 0
      }
      
      if (order === 'desc') {
        return bValue > aValue ? 1 : -1
      } else {
        return aValue > bValue ? 1 : -1
      }
    })
    
    return students
  }

  function addStudentToRoster(student: RosterStudent) {
    if (roster.value) {
      roster.value.students.push(student)
      roster.value.totalEnrolled = roster.value.students.length
    }
  }

  function getStudentsAtRisk(): RosterStudent[] {
    return roster.value?.students.filter(student => {
      return student.grades.currentPercentage < 65 || 
             student.enrollmentInfo.attendanceRate < 70
    }) || []
  }

  function getAttendanceAlerts(): string[] {
    return roster.value?.students
      .filter(student => student.enrollmentInfo.attendanceRate < 70)
      .map(student => student.id) || []
  }

  function getGradeAlerts(): string[] {
    return roster.value?.students
      .filter(student => student.grades.currentPercentage < 65)
      .map(student => student.id) || []
  }

  // Calendar Management
  async function loadCalendarEvents(courseId: string) {
    loading.value = true
    error.value = null
    
    try {
      const events = await CourseManagementService.getCalendarEvents(courseId)
      calendarEvents.value = events
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load calendar events'
      calendarEvents.value = []
    } finally {
      loading.value = false
    }
  }

  function setCalendarEvents(events: CourseCalendarEvent[]) {
    calendarEvents.value = events
  }

  function setCampusEvents(events: CourseCalendarEvent[]) {
    campusEvents.value = events
  }

  function getEventsByType(type: string): CourseCalendarEvent[] {
    return calendarEvents.value.filter(event => event.type === type)
  }

  function getUpcomingEvents(days: number): CourseCalendarEvent[] {
    const cutoffDate = new Date()
    cutoffDate.setDate(cutoffDate.getDate() + days)
    
    return allEvents.value.filter(event => 
      event.startDate >= new Date() && event.startDate <= cutoffDate
    )
  }

  function getAllEvents(): CourseCalendarEvent[] {
    return allEvents.value
  }

  function getHolidays(): CourseCalendarEvent[] {
    return campusEvents.value.filter(event => event.type === 'holiday')
  }

  function getEventsByDateRange(startDate: Date, endDate: Date): CourseCalendarEvent[] {
    return allEvents.value.filter(event => 
      event.startDate >= startDate && event.startDate <= endDate
    )
  }

  function getEventsByPriority(priority: string): CourseCalendarEvent[] {
    return allEvents.value.filter(event => event.priority === priority)
  }

  function getEventsForDate(date: Date): CourseCalendarEvent[] {
    const dateStr = date.toDateString()
    return allEvents.value.filter(event => 
      event.startDate.toDateString() === dateStr
    )
  }

  // Permission and Access Control
  function canManageCourse(courseId: string, facultyId: string): boolean {
    if (!courses.value || !Array.isArray(courses.value)) return false
    const course = courses.value.find(c => c.id === courseId)
    return course?.facultyId === facultyId
  }

  function canViewRoster(courseId: string, facultyId: string): boolean {
    return canManageCourse(courseId, facultyId)
  }

  function canCreateAssignments(courseId: string, facultyId: string): boolean {
    return canManageCourse(courseId, facultyId)
  }

  // Grading and Analytics
  async function bulkUpdateGrades(gradeUpdates: Array<{
    studentId: string
    assignmentId: string
    score: number
  }>) {
    for (const update of gradeUpdates) {
      // In real implementation, would update via API
      const student = getStudentById(update.studentId)
      if (student) {
        const assignmentGrade = student.grades.assignments.find(
          grade => grade.assignmentId === update.assignmentId
        )
        if (assignmentGrade) {
          assignmentGrade.score = update.score
        }
      }
      
      // Also store in gradeMap for quick lookup
      const key = `${update.studentId}-${update.assignmentId}`
      gradeMap.value.set(key, update.score)
    }
  }

  function getStudentGrade(studentId: string, assignmentId: string): number | undefined {
    // Try from roster first
    const student = getStudentById(studentId)
    const assignmentGrade = student?.grades.assignments.find(
      grade => grade.assignmentId === assignmentId
    )
    if (assignmentGrade?.score !== undefined) {
      return assignmentGrade.score
    }
    
    // Fallback to gradeMap
    const key = `${studentId}-${assignmentId}`
    return gradeMap.value.get(key)
  }

  function getCourseAnalytics(_courseId: string) {
    return {
      enrollmentTrends: {},
      gradeDistribution: {},
      assignmentPerformance: {},
      attendanceMetrics: {}
    }
  }

  function getAssignmentCompletionRate(assignmentId: string): number {
    const assignment = getAssignmentById(assignmentId)
    if (!assignment || !roster.value) return 0
    
    const totalStudents = roster.value.students.length
    const completedSubmissions = assignment.statistics.submitted
    
    return totalStudents > 0 ? (completedSubmissions / totalStudents) * 100 : 0
  }

  function compareClassPerformance(_courseIds: string[]) {
    return {
      averageGrades: {},
      enrollmentRates: {}
    }
  }

  return {
    // State
    courses,
    currentCourse,
    assignments,
    courseContent,
    announcements,
    roster,
    calendarEvents,
    campusEvents,
    enrollmentAlerts,
    loading,
    error,

    // Computed
    totalEnrollment,
    averageEnrollmentRate,
    upcomingAssignments,
    groupAssignments,
    allEvents,

    // Actions
    loadCourses,
    setCourses,
    getCourseMetrics,
    getTotalEnrollment,
    getAverageEnrollmentRate,

    // Section Management
    updateSection,
    getSectionEnrollment,
    getSectionCapacity,
    getSectionWaitlist,
    isSectionFull,
    getSectionAvailability,
    updateEnrollment,
    canEnrollStudent,

    // Content Management
    addContent,
    getContentByType,
    isSyllabusPublished,
    uploadCourseFile,
    getCourseFiles,

    // Announcement Management
    createAnnouncement,
    getAnnouncementsByPriority,
    getDeliveryRate,

    // Assignment Management
    createAssignment,
    addAssignment,
    getAssignmentById,
    getAssignmentsByType,
    getRubricById,
    getUpcomingAssignments,
    getGroupAssignments,
    calculateLatePenalty,
    isAssignmentOverdue,
    getDaysLate,
    validateAssignment,
    bulkCreateAssignments,

    // Roster Management
    loadRoster,
    setRoster,
    getStudentById,
    getStudentsByMajor,
    getClassAverage,
    filterStudentsByYear,
    filterStudentsByGPA,
    sortStudentsBy,
    addStudentToRoster,
    getStudentsAtRisk,
    getAttendanceAlerts,
    getGradeAlerts,

    // Calendar Management
    loadCalendarEvents,
    setCalendarEvents,
    setCampusEvents,
    getEventsByType,
    getUpcomingEvents,
    getAllEvents,
    getHolidays,
    getEventsByDateRange,
    getEventsByPriority,
    getEventsForDate,

    // Permission and Access Control
    canManageCourse,
    canViewRoster,
    canCreateAssignments,

    // Grading and Analytics
    bulkUpdateGrades,
    getStudentGrade,
    getCourseAnalytics,
    getAssignmentCompletionRate,
    compareClassPerformance
  }
})