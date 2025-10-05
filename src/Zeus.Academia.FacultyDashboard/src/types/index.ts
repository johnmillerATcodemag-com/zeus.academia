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

/**
 * Task 2: Faculty Profile and Authentication Types
 */
export interface FacultyProfile {
  id: string
  userId: string
  bio?: string
  education: Education[]
  researchAreas: string[]
  publications: Publication[]
  awards: Award[]
  professionalExperience: ProfessionalExperience[]
  cvUrl?: string
  website?: string
  linkedinUrl?: string
  orcidId?: string
  googleScholarId?: string
  contactPreferences: ContactPreferences
  lastUpdated: Date
}

export interface Education {
  degree: string
  institution: string
  year: number
  fieldOfStudy: string
  gpa?: number
  honors?: string
}

export interface Publication {
  id: string
  title: string
  authors: string[]
  year: number
  type: 'journal' | 'conference' | 'book' | 'chapter' | 'preprint'
  journal?: string
  conference?: string
  book?: string
  volume?: string
  issue?: string
  pages?: string
  doi?: string
  url?: string
  abstract?: string
  keywords?: string[]
  citationCount: number
  impactFactor?: number
}

export interface Award {
  name: string
  institution: string
  year: number
  description?: string
  amount?: number
  category?: string
}

export interface ProfessionalExperience {
  position: string
  institution: string
  startDate: Date
  endDate: Date | null
  description?: string
  responsibilities?: string[]
}

export interface ContactPreferences {
  email: boolean
  phone: boolean
  officeVisit: boolean
  preferredContactMethod: 'email' | 'phone' | 'office'
}

/**
 * Office Hours and Appointment Types
 */
export interface OfficeHours {
  id: string
  facultyId: string
  dayOfWeek: 'monday' | 'tuesday' | 'wednesday' | 'thursday' | 'friday' | 'saturday' | 'sunday'
  startTime: string // HH:MM format
  endTime: string // HH:MM format
  location: string
  type: 'office_hours' | 'virtual_hours' | 'by_appointment'
  isRecurring: boolean
  maxAppointments: number
  appointmentDuration: number // minutes
  meetingUrl?: string // for virtual hours
  notes?: string
}

export interface Appointment {
  id: string
  officeHoursId: string
  studentId: string
  studentName: string
  studentEmail: string
  appointmentDate: Date
  startTime: string
  endTime: string
  status: 'pending' | 'confirmed' | 'cancelled' | 'completed' | 'no_show'
  purpose: string
  notes?: string
  meetingUrl?: string
  reminderSent?: boolean
}

/**
 * Committee and Administrative Types
 */
export interface Committee {
  id: string
  facultyId: string
  name: string
  type: 'academic' | 'administrative' | 'search' | 'curriculum' | 'student_affairs' | 'research' | 'external' | 'other'
  role: 'member' | 'chair' | 'vice_chair' | 'secretary'
  status: 'active' | 'completed' | 'on_hold'
  startDate: Date
  endDate?: Date
  description?: string
  meetingSchedule?: string
  responsibilities?: string[]
  achievements?: string[]
}

export interface ProfessionalMembership {
  organization: string
  membershipType: string
  startDate: Date
  endDate: Date | null
  membershipId?: string
  status?: 'active' | 'inactive' | 'expired'
}

export interface ProfessionalAffiliation {
  institution: string
  role: string
  startDate: Date
  endDate: Date | null
  description?: string
}

export interface Certification {
  name: string
  issuingOrganization: string
  issueDate: Date
  expiryDate?: Date
  credentialId?: string
  verificationUrl?: string
}

/**
 * Enhanced Auth Store State for Task 2
 */
export interface EnhancedAuthState extends AuthState {
  roleHierarchy: Record<FacultyRole, number>
  permissionHierarchy: Record<Permission, FacultyRole[]>
}

/**
 * Faculty Profile Store State
 */
export interface FacultyProfileState {
  profile: FacultyProfile | null
  officeHours: OfficeHours[]
  appointments: Appointment[]
  upcomingAppointments: Appointment[]
  committees: Committee[]
  professionalMemberships: ProfessionalMembership[]
  professionalAffiliations: ProfessionalAffiliation[]
  certifications: Certification[]
  publications: Publication[]
  loading: boolean
  error: string | null
}