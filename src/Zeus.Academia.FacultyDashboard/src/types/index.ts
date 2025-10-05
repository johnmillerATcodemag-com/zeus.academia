/**
 * Faculty Authentication Types
 * Defines role-based access control and permissions for faculty users
 */
export interface FacultyUser {
  id: string
  email: string
  firstName: string
  lastName: string
  role: FacultyRole
  department: string
  title: string
  permissions: Permission[]
  profileImage?: string
  officeLocation?: string
  phoneNumber?: string
}

export type FacultyRole = 'professor' | 'associate_professor' | 'assistant_professor' | 'lecturer' | 'chair' | 'dean' | 'admin'

export type Permission = 
  | 'view_courses'
  | 'manage_grades' 
  | 'view_students'
  | 'manage_students'
  | 'create_assignments'
  | 'manage_course_content'
  | 'view_analytics'
  | 'manage_faculty'
  | 'view_department'
  | 'manage_department'
  | 'view_college'
  | 'manage_college'
  | 'system_admin'

export interface AuthState {
  user: FacultyUser | null
  token: string | null
  isAuthenticated: boolean
  loading: boolean
  error: string | null
}

/**
 * Course and Gradebook Types
 */
export interface Course {
  id: string
  name: string
  code: string
  section: string
  semester: string
  year: number
  credits: number
  description?: string
  syllabusUrl?: string
  enrollmentCount: number
  maxEnrollment: number
  status: 'active' | 'inactive' | 'completed'
}

export interface Student {
  id: string
  firstName: string
  lastName: string
  email: string
  studentId: string
  major: string
  year: number
  gpa?: number
  profileImage?: string
  enrollmentDate: Date
}

export interface Assignment {
  id: string
  courseId: string
  name: string
  description?: string
  type: 'homework' | 'quiz' | 'exam' | 'project' | 'participation'
  maxPoints: number
  weight: number
  dueDate: Date
  createdDate: Date
  isPublished: boolean
  rubricId?: string
}

export interface Grade {
  id: string
  studentId: string
  assignmentId: string
  score: number
  maxScore: number
  percentage: number
  letterGrade?: string
  submittedAt?: Date
  gradedAt?: Date
  feedback?: string
}

/**
 * Component Props and State Types
 */
export interface GradebookState {
  courses: Course[]
  selectedCourse: Course | null
  students: Student[]
  assignments: Assignment[]
  grades: Grade[]
  loading: boolean
  error: string | null
  filters: GradebookFilters
  view: 'grid' | 'list'
  bulkEditMode: boolean
}

export interface GradebookFilters {
  studentSearch: string
  assignmentFilter: string
  gradeFilter: 'all' | 'graded' | 'ungraded' | 'missing'
  sortBy: 'name' | 'grade' | 'date'
  sortOrder: 'asc' | 'desc'
}

/**
 * API Response Types
 */
export interface ApiResponse<T> {
  success: boolean
  data: T
  message?: string
  error?: string
}

export interface LoginResponse {
  user: FacultyUser
  token: string
  refreshToken: string
  expiresIn: number
}

/**
 * Responsive Design Types
 */
export type ScreenSize = 'xs' | 'sm' | 'md' | 'lg' | 'xl' | 'xxl'

export interface ResponsiveConfig {
  breakpoints: Record<ScreenSize, number>
  currentSize: ScreenSize
  isMobile: boolean
  isTablet: boolean
  isDesktop: boolean
}

/**
 * Performance and Analytics Types
 */
export interface PerformanceMetrics {
  loadTime: number
  renderTime: number
  memoryUsage: number
  apiCalls: number
  cacheHits: number
  cacheMisses: number
}