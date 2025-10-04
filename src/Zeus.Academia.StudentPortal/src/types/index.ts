// Core entity types
export interface Student {
  id: string
  email: string
  firstName: string
  lastName: string
  studentId: string
  enrollmentDate: string
  gpa?: number
}

export interface Course {
  id: string
  code: string
  name: string
  description: string
  credits: number
  instructor: string
  schedule: CourseSchedule[]
  enrollmentStatus: EnrollmentStatus
}

export interface CourseSchedule {
  dayOfWeek: string
  startTime: string
  endTime: string
  location: string
}

export interface Enrollment {
  id: string
  studentId: string
  courseId: string
  enrollmentDate: string
  status: EnrollmentStatus
  grade?: string
}

// Enums
export enum EnrollmentStatus {
  Enrolled = 'Enrolled',
  Waitlisted = 'Waitlisted',
  Dropped = 'Dropped',
  Completed = 'Completed'
}

// API Response types
export interface ApiResponse<T = any> {
  success: boolean
  data?: T
  message?: string
  errors?: string[]
}

export interface PaginatedResponse<T> extends ApiResponse<T[]> {
  pagination: {
    page: number
    pageSize: number
    totalItems: number
    totalPages: number
  }
}

// Authentication types
export interface LoginRequest {
  email: string
  password: string
}

export interface LoginResponse {
  token: string
  refreshToken: string
  student: Student
  expiresAt: string
}

export interface AuthState {
  isAuthenticated: boolean
  student: Student | null
  token: string | null
  refreshToken: string | null
}

// Store state types
export interface RootState {
  auth: AuthState
  courses: CoursesState
  loading: LoadingState
}

export interface CoursesState {
  courses: Course[]
  enrollments: Enrollment[]
  selectedCourse: Course | null
}

export interface LoadingState {
  [key: string]: boolean
}