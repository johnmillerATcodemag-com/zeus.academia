/**
 * Administrative Interface Type Definitions
 * Zeus Academia System - Administrative Interface
 */

// Administrative role types
export type AdminRoleType = 'system_admin' | 'registrar' | 'academic_admin'

// Administrative user interface
export interface AdminUser {
  id: string
  email: string
  firstName: string
  lastName: string
  role: AdminRoleType
  permissions: AdminPermission[]
  department?: string
  title: string
  lastLogin?: Date
  isActive: boolean
  mfaEnabled: boolean
  trustedDevices: string[]
  securityLevel: SecurityLevel
}

// Administrative permissions
export type AdminPermission = 
  // System Administration
  | 'user_management'
  | 'system_configuration'
  | 'security_management'
  | 'audit_access'
  | 'backup_management'
  | 'system_monitoring'
  
  // Academic Administration  
  | 'academic_calendar'
  | 'enrollment_management'
  | 'academic_records'
  | 'course_scheduling'
  | 'graduation_management'
  | 'curriculum_oversight'
  
  // Faculty Administration
  | 'faculty_management'
  | 'academic_policy'
  | 'quality_assurance'
  | 'accreditation_support'
  
  // Financial Administration
  | 'financial_management'
  | 'tuition_management'
  | 'financial_aid'
  | 'billing_management'

// Security levels
export type SecurityLevel = 'basic' | 'elevated' | 'critical'

// Dashboard configuration
export interface DashboardConfig {
  role: AdminRoleType
  widgets: DashboardWidget[]
  dataViews: DataView[]
  actionPanels: ActionPanel[]
  layout: LayoutConfig
}

export interface DashboardWidget {
  id: string
  type: 'metric' | 'chart' | 'table' | 'alert' | 'status'
  title: string
  size: 'small' | 'medium' | 'large'
  refreshInterval?: number
  permissions: AdminPermission[]
  dataSource: string
  config: Record<string, any>
}

export interface DataView {
  id: string
  name: string
  type: 'grid' | 'chart' | 'report'
  dataSource: string
  filters: FilterConfig[]
  columns: ColumnConfig[]
  permissions: AdminPermission[]
}

export interface ActionPanel {
  id: string
  name: string
  actions: AdminAction[]
  permissions: AdminPermission[]
  context: 'global' | 'selection' | 'record'
}

export interface AdminAction {
  id: string
  name: string
  type: 'single' | 'bulk' | 'workflow'
  icon: string
  confirmation?: {
    required: boolean
    message: string
    riskLevel: 'low' | 'medium' | 'high'
  }
  permissions: AdminPermission[]
}

// Layout configuration
export interface LayoutConfig {
  columns: number
  rows: number
  gridSize: number
  responsive: boolean
  customizable: boolean
}

// Data grid interfaces
export interface GridConfig {
  name: string
  dataSource: string
  columns: GridColumn[]
  features: GridFeatures
  performance: GridPerformance
  security: GridSecurity
}

export interface GridColumn {
  field: string
  header: string
  type: 'text' | 'number' | 'date' | 'boolean' | 'enum' | 'actions'
  width?: number
  sortable: boolean
  filterable: boolean
  resizable: boolean
  editable?: boolean
  formatter?: string
  validator?: string
}

export interface GridFeatures {
  pagination: boolean
  sorting: boolean
  filtering: boolean
  grouping: boolean
  selection: 'single' | 'multiple' | 'none'
  export: string[]
  bulkActions: string[]
  search: boolean
  columnReorder: boolean
  columnResize: boolean
}

export interface GridPerformance {
  virtualScrolling: boolean
  lazyLoading: boolean
  caching: boolean
  maxRows: number
  pageSize: number
}

export interface GridSecurity {
  fieldLevelSecurity: boolean
  rowLevelSecurity: boolean
  auditChanges: boolean
  encryptSensitiveData: boolean
}

// Filter configurations
export interface FilterConfig {
  field: string
  type: 'text' | 'number' | 'date' | 'select' | 'multiselect' | 'range'
  operator: FilterOperator
  options?: FilterOption[]
  defaultValue?: any
}

export type FilterOperator = 
  | 'equals' | 'not_equals' | 'contains' | 'not_contains' 
  | 'starts_with' | 'ends_with' | 'greater_than' | 'less_than'
  | 'greater_equal' | 'less_equal' | 'between' | 'in' | 'not_in'

export interface FilterOption {
  value: any
  label: string
  disabled?: boolean
}

export interface ColumnConfig {
  field: string
  header: string
  type: string
  width?: number
  visible: boolean
  sortable: boolean
  filterable: boolean
}

// State management interfaces
export interface AdminState {
  user: AdminUser | null
  token: string | null
  refreshToken: string | null
  permissions: AdminPermission[]
  dashboardConfig: DashboardConfig | null
  loading: boolean
  error: string | null
}

export interface DataState {
  users: {
    data: any[]
    totalCount: number
    filters: Record<string, any>
    pagination: PaginationState
    loading: boolean
    error: string | null
  }
  courses: {
    data: any[]
    totalCount: number
    byDepartment: Record<string, any[]>
    loading: boolean
    error: string | null
  }
  enrollments: {
    current: any[]
    historical: any[]
    statistics: Record<string, any>
    loading: boolean
    error: string | null
  }
  systemMetrics: {
    performance: Record<string, any>
    health: Record<string, any>
    alerts: Alert[]
    lastUpdated: Date | null
    loading: boolean
    error: string | null
  }
}

export interface PaginationState {
  page: number
  pageSize: number
  totalPages: number
  totalItems: number
}

// Security interfaces
export interface SecurityContext {
  user: AdminUser
  session: SessionInfo
  request: RequestInfo
  riskAssessment: RiskAssessment
}

export interface SessionInfo {
  id: string
  startTime: Date
  lastActivity: Date
  ipAddress: string
  userAgent: string
  location?: LocationInfo
  deviceFingerprint: string
}

export interface LocationInfo {
  country: string
  region: string
  city: string
  latitude?: number
  longitude?: number
}

export interface RequestInfo {
  resource: string
  action: string
  parameters: Record<string, any>
  timestamp: Date
  riskScore: number
}

export interface RiskAssessment {
  score: number
  factors: RiskFactor[]
  level: 'low' | 'medium' | 'high' | 'critical'
  recommendations: string[]
}

export interface RiskFactor {
  type: string
  description: string
  weight: number
  value: any
}

// Audit interfaces
export interface AuditEntry {
  id: string
  timestamp: Date
  userId: string
  userRole: AdminRoleType
  action: string
  resource: string
  details: AuditDetails
  session: SessionInfo
  risk: RiskAssessment
  outcome: 'success' | 'failure' | 'partial'
  errors?: string[]
}

export interface AuditDetails {
  before?: any
  after?: any
  parameters: Record<string, any>
  affectedRecords: number
  ipAddress: string
  userAgent: string
}

// API response interfaces
export interface ApiResponse<T = any> {
  success: boolean
  data?: T
  message?: string
  errors?: string[]
  metadata?: {
    total?: number
    page?: number
    pageSize?: number
    timestamp: Date
  }
}

export interface BulkOperationResult {
  success: number
  failed: number
  errors: string[]
  details: {
    successIds: string[]
    failedIds: string[]
    warnings: string[]
  }
}

// Notification interfaces
export interface Alert {
  id: string
  type: 'info' | 'warning' | 'error' | 'success'
  title: string
  message: string
  timestamp: Date
  severity: 'low' | 'medium' | 'high' | 'critical'
  category: string
  acknowledged: boolean
  actions?: AlertAction[]
}

export interface AlertAction {
  id: string
  label: string
  type: 'button' | 'link'
  action: string
  style: 'primary' | 'secondary' | 'success' | 'danger' | 'warning'
}

// Form interfaces
export interface FormField {
  name: string
  type: 'text' | 'email' | 'password' | 'number' | 'date' | 'select' | 'multiselect' | 'textarea' | 'checkbox' | 'radio'
  label: string
  placeholder?: string
  required: boolean
  validation?: ValidationRule[]
  options?: FormOption[]
  disabled?: boolean
  readonly?: boolean
  help?: string
}

export interface FormOption {
  value: any
  label: string
  disabled?: boolean
}

export interface ValidationRule {
  type: 'required' | 'email' | 'min' | 'max' | 'pattern' | 'custom'
  value?: any
  message: string
  validator?: (value: any) => boolean
}

// All types are already exported individually above