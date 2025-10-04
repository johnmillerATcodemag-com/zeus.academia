export { ApiService, default as ApiServiceDefault } from './ApiService'
export { AuthService, default as AuthServiceDefault } from './AuthService'
export { CourseService, default as CourseServiceDefault } from './CourseService'

// Re-export types for convenience
export type {
  CourseSearchParams,
  CoursePaginationParams
} from './CourseService'