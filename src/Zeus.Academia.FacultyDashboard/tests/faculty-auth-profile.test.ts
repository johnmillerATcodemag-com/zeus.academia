/**
 * Task 2 Tests: Faculty Authentication and Profile Management
 * Testing hierarchical permissions, complete faculty profiles, office hours scheduling,
 * professional information management, and administrative role indicators
 */
import { describe, it, expect, beforeEach, vi } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useAuthStore } from '../src/stores/auth'
import { useFacultyProfileStore } from '../src/stores/facultyProfile'
import type { FacultyUser, FacultyRole, Permission, FacultyProfile, OfficeHours, Publication, Committee } from '../src/types'

// Mock API calls
vi.mock('../src/services/AuthService', () => ({
  AuthService: {
    login: vi.fn(),
    logout: vi.fn(),
    refreshToken: vi.fn(),
    updateProfile: vi.fn(),
    uploadProfileImage: vi.fn()
  }
}))

vi.mock('../src/services/FacultyProfileService', () => ({
  FacultyProfileService: {
    getProfile: vi.fn(),
    updateProfile: vi.fn(),
    uploadCV: vi.fn(),
    addPublication: vi.fn(),
    updatePublication: vi.fn(),
    deletePublication: vi.fn(),
    updateOfficeHours: vi.fn(),
    addCommitteeAssignment: vi.fn(),
    updateCommitteeRole: vi.fn(),
    removeCommitteeAssignment: vi.fn()
  }
}))

describe('Task 2: Faculty Authentication and Profile Management', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  describe('1. Faculty login with hierarchical permissions', () => {
    it('should authenticate professor with appropriate permissions', async () => {
      const authStore = useAuthStore()
      const professorUser: FacultyUser = {
        id: '1',
        email: 'jane.smith@university.edu',
        firstName: 'Jane',
        lastName: 'Smith',
        role: 'professor',
        department: 'Computer Science',
        title: 'Professor of Computer Science',
        permissions: ['view_courses', 'manage_grades', 'view_students', 'create_assignments', 'manage_course_content'],
        profileImage: '/images/faculty/jane-smith.jpg',
        officeLocation: 'Engineering Building 301',
        phoneNumber: '(555) 123-4567'
      }

      await authStore.login('jane.smith@university.edu', 'password123')
      authStore.setUser(professorUser)

      expect(authStore.user).toEqual(professorUser)
      expect(authStore.hasPermission('manage_grades')).toBe(true)
      expect(authStore.hasPermission('manage_department')).toBe(false)
      expect(authStore.getRoleHierarchy()).toBe('professor')
    })

    it('should authenticate department chair with elevated permissions', async () => {
      const authStore = useAuthStore()
      const chairUser: FacultyUser = {
        id: '2',
        email: 'john.doe@university.edu',
        firstName: 'John',
        lastName: 'Doe',
        role: 'chair',
        department: 'Computer Science',
        title: 'Department Chair & Professor',
        permissions: [
          'view_courses', 'manage_grades', 'view_students', 'manage_students',
          'create_assignments', 'manage_course_content', 'view_analytics',
          'manage_faculty', 'view_department', 'manage_department'
        ],
        profileImage: '/images/faculty/john-doe.jpg',
        officeLocation: 'Engineering Building 401',
        phoneNumber: '(555) 123-4568'
      }

      await authStore.login('john.doe@university.edu', 'password123')
      authStore.setUser(chairUser)

      expect(authStore.user).toEqual(chairUser)
      expect(authStore.hasPermission('manage_department')).toBe(true)
      expect(authStore.hasPermission('manage_faculty')).toBe(true)
      expect(authStore.hasPermission('manage_college')).toBe(false)
      expect(authStore.getRoleHierarchy()).toBe('chair')
    })

    it('should authenticate dean with highest administrative permissions', async () => {
      const authStore = useAuthStore()
      const deanUser: FacultyUser = {
        id: '3',
        email: 'mary.johnson@university.edu',
        firstName: 'Mary',
        lastName: 'Johnson',
        role: 'dean',
        department: 'College of Engineering',
        title: 'Dean of Engineering',
        permissions: [
          'view_courses', 'manage_grades', 'view_students', 'manage_students',
          'create_assignments', 'manage_course_content', 'view_analytics',
          'manage_faculty', 'view_department', 'manage_department',
          'view_college', 'manage_college'
        ],
        profileImage: '/images/faculty/mary-johnson.jpg',
        officeLocation: 'Administration Building 501',
        phoneNumber: '(555) 123-4569'
      }

      await authStore.login('mary.johnson@university.edu', 'password123')
      authStore.setUser(deanUser)

      expect(authStore.user).toEqual(deanUser)
      expect(authStore.hasPermission('manage_college')).toBe(true)
      expect(authStore.hasPermission('system_admin')).toBe(false)
      expect(authStore.getRoleHierarchy()).toBe('dean')
    })

    it('should properly handle role hierarchy and permission inheritance', () => {
      const authStore = useAuthStore()
      
      // Test role hierarchy levels
      expect(authStore.getRoleLevel('assistant_professor')).toBe(1)
      expect(authStore.getRoleLevel('associate_professor')).toBe(2)
      expect(authStore.getRoleLevel('professor')).toBe(3)
      expect(authStore.getRoleLevel('chair')).toBe(4)
      expect(authStore.getRoleLevel('dean')).toBe(5)
      
      // Test permission inheritance
      expect(authStore.canAccessRole('chair', 'professor')).toBe(true)
      expect(authStore.canAccessRole('professor', 'chair')).toBe(false)
      expect(authStore.canAccessRole('dean', 'assistant_professor')).toBe(true)
    })
  })

  describe('2. Complete faculty profile with academic credentials', () => {
    it('should manage comprehensive faculty profile data', async () => {
      const profileStore = useFacultyProfileStore()
      const completeProfile: FacultyProfile = {
        id: '1',
        userId: '1',
        bio: 'Dr. Jane Smith is a Professor of Computer Science specializing in artificial intelligence and machine learning.',
        education: [
          {
            degree: 'Ph.D. in Computer Science',
            institution: 'Stanford University',
            year: 2005,
            fieldOfStudy: 'Artificial Intelligence'
          },
          {
            degree: 'M.S. in Computer Science', 
            institution: 'MIT',
            year: 2001,
            fieldOfStudy: 'Machine Learning'
          },
          {
            degree: 'B.S. in Computer Science',
            institution: 'UC Berkeley',
            year: 1999,
            fieldOfStudy: 'Computer Science'
          }
        ],
        researchAreas: ['Artificial Intelligence', 'Machine Learning', 'Natural Language Processing', 'Computer Vision'],
        publications: [
          {
            id: '1',
            title: 'Advanced Neural Networks in Natural Language Processing',
            authors: ['Jane Smith', 'Robert Chen'],
            journal: 'Journal of AI Research',
            year: 2023,
            doi: '10.1000/182',
            citationCount: 45,
            type: 'journal'
          },
          {
            id: '2', 
            title: 'Machine Learning Applications in Healthcare',
            authors: ['Jane Smith', 'Sarah Davis', 'Michael Brown'],
            conference: 'International Conference on Machine Learning',
            year: 2022,
            pages: '234-251',
            citationCount: 32,
            type: 'conference'
          }
        ],
        awards: [
          {
            name: 'Excellence in Teaching Award',
            institution: 'University of Technology',
            year: 2023,
            description: 'Recognized for outstanding contributions to undergraduate education'
          },
          {
            name: 'Best Paper Award',
            institution: 'ICML 2022',
            year: 2022,
            description: 'Best paper in machine learning applications track'
          }
        ],
        professionalExperience: [
          {
            position: 'Professor',
            institution: 'University of Technology',
            startDate: new Date('2015-08-01'),
            endDate: null,
            description: 'Teaching undergraduate and graduate courses in AI and ML'
          },
          {
            position: 'Associate Professor',
            institution: 'University of Technology', 
            startDate: new Date('2010-08-01'),
            endDate: new Date('2015-07-31'),
            description: 'Research and teaching in computer science department'
          }
        ],
        cvUrl: '/documents/cv/jane-smith-cv.pdf',
        website: 'https://cs.university.edu/~jsmith',
        linkedinUrl: 'https://linkedin.com/in/jane-smith-phd',
        orcidId: '0000-0002-1825-0097',
        googleScholarId: 'abc123def456',
        contactPreferences: {
          email: true,
          phone: true,
          officeVisit: true,
          preferredContactMethod: 'email'
        },
        lastUpdated: new Date()
      }

      await profileStore.loadProfile('1')
      profileStore.setProfile(completeProfile)

      expect(profileStore.profile).toEqual(completeProfile)
      expect(profileStore.profile?.education).toHaveLength(3)
      expect(profileStore.profile?.publications).toHaveLength(2)
      expect(profileStore.profile?.researchAreas).toContain('Artificial Intelligence')
      expect(profileStore.getHighestDegree()).toBe('Ph.D. in Computer Science')
    })

    it('should manage publications with academic metadata', async () => {
      const profileStore = useFacultyProfileStore()
      const newPublication: Publication = {
        id: '3',
        title: 'Deep Learning for Medical Image Analysis',
        authors: ['Jane Smith', 'Alice Wilson'],
        journal: 'Medical AI Journal',
        year: 2024,
        volume: '15',
        issue: '3',
        pages: '123-145',
        doi: '10.1000/medical-ai.2024.15.3.123',
        citationCount: 12,
        type: 'journal',
        abstract: 'This paper presents novel deep learning approaches for medical image analysis...',
        keywords: ['deep learning', 'medical imaging', 'convolutional neural networks'],
        impactFactor: 4.5
      }

      await profileStore.addPublication(newPublication)
      
      expect(profileStore.publications).toContainEqual(newPublication)
      expect(profileStore.getTotalCitations()).toBeGreaterThan(0)
      expect(profileStore.getPublicationsByYear(2024)).toHaveLength(1)
      expect(profileStore.getJournalPublications()).toContain(newPublication)
    })

    it('should handle CV upload and document management', async () => {
      const profileStore = useFacultyProfileStore()
      const cvFile = new File(['CV content'], 'jane-smith-cv.pdf', { type: 'application/pdf' })
      
      await profileStore.uploadCV(cvFile)
      
      expect(profileStore.profile?.cvUrl).toBeDefined()
      expect(profileStore.profile?.cvUrl).toMatch(/\.pdf$/)
      expect(profileStore.hasCV()).toBe(true)
    })
  })

  describe('3. Office hours scheduling with student appointment integration', () => {
    it('should manage office hours schedule', async () => {
      const profileStore = useFacultyProfileStore()
      const officeHours: OfficeHours[] = [
        {
          id: '1',
          facultyId: '1',
          dayOfWeek: 'monday',
          startTime: '10:00',
          endTime: '12:00',
          location: 'Engineering Building 301',
          type: 'office_hours',
          isRecurring: true,
          maxAppointments: 6,
          appointmentDuration: 20
        },
        {
          id: '2',
          facultyId: '1',
          dayOfWeek: 'wednesday',
          startTime: '14:00',
          endTime: '16:00',
          location: 'Engineering Building 301',
          type: 'office_hours',
          isRecurring: true,
          maxAppointments: 6,
          appointmentDuration: 20
        },
        {
          id: '3',
          facultyId: '1',
          dayOfWeek: 'friday',
          startTime: '09:00',
          endTime: '10:00',
          location: 'Virtual - Zoom',
          type: 'virtual_hours',
          isRecurring: true,
          maxAppointments: 3,
          appointmentDuration: 20,
          meetingUrl: 'https://zoom.us/j/123456789'
        }
      ]

      await profileStore.updateOfficeHours(officeHours)
      
      expect(profileStore.officeHours).toEqual(officeHours)
      expect(profileStore.getOfficeHoursByDay('monday')).toHaveLength(1)
      expect(profileStore.getVirtualOfficeHours()).toHaveLength(1)
      expect(profileStore.getTotalWeeklyHours()).toBe(5) // 2 + 2 + 1 hours
    })

    it('should handle student appointment booking integration', async () => {
      const profileStore = useFacultyProfileStore()
      const appointments = [
        {
          id: '1',
          officeHoursId: '1',
          studentId: 'student123',
          studentName: 'John Student',
          studentEmail: 'john.student@university.edu',
          appointmentDate: new Date('2024-10-07'), // Monday
          startTime: '10:20',
          endTime: '10:40',
          status: 'confirmed',
          purpose: 'Discuss assignment questions',
          notes: 'Questions about machine learning project'
        },
        {
          id: '2',
          officeHoursId: '1',
          studentId: 'student456',
          studentName: 'Sarah Johnson',
          studentEmail: 'sarah.johnson@university.edu',
          appointmentDate: new Date('2024-10-07'), // Monday
          startTime: '11:00',
          endTime: '11:20',
          status: 'pending',
          purpose: 'Career advice',
          notes: 'Graduate school applications discussion'
        }
      ]

      profileStore.setAppointments(appointments)
      
      expect(profileStore.upcomingAppointments).toHaveLength(2)
      expect(profileStore.getAppointmentsByDate(new Date('2024-10-07'))).toHaveLength(2)
      expect(profileStore.getAvailableSlots('1', new Date('2024-10-07'))).toBeDefined()
      expect(profileStore.hasConflictingAppointment('1', new Date('2024-10-07'), '10:30')).toBe(true)
    })

    it('should validate office hours availability and conflicts', () => {
      const profileStore = useFacultyProfileStore()
      
      // Test availability calculation
      expect(profileStore.isTimeSlotAvailable('1', new Date('2024-10-07'), '10:00')).toBe(true)
      expect(profileStore.isTimeSlotAvailable('1', new Date('2024-10-07'), '10:20')).toBe(false) // Booked
      
      // Test conflict detection
      expect(profileStore.getConflictingEvents('1', new Date('2024-10-07'), '10:15', '10:35')).toHaveLength(1)
      
      // Test capacity management
      expect(profileStore.getRemainingCapacity('1', new Date('2024-10-07'))).toBe(4) // 6 max - 2 booked
    })
  })

  describe('4. Professional information management and display', () => {
    it('should manage department and committee assignments', async () => {
      const profileStore = useFacultyProfileStore()
      const committees: Committee[] = [
        {
          id: '1',
          facultyId: '1',
          name: 'Graduate Admissions Committee',
          type: 'academic',
          role: 'member',
          status: 'active',
          startDate: new Date('2020-08-01'),
          endDate: undefined,
          responsibilities: ['Review graduate applications', 'Interview candidates'],
          meetingSchedule: 'Bi-weekly on Fridays',
          description: 'Committee for graduate student admissions'
        },
        {
          id: '2',
          facultyId: '1',
          name: 'Faculty Senate',
          type: 'administrative',
          role: 'chair',
          status: 'active',
          startDate: new Date('2023-01-01'),
          endDate: new Date('2024-12-31'),
          responsibilities: ['Lead senate meetings', 'Coordinate with administration'],
          meetingSchedule: 'Monthly',
          description: 'University-wide faculty governance'
        },
        {
          id: '3',
          facultyId: '1',
          name: 'Curriculum Committee',
          type: 'curriculum',
          role: 'vice_chair',
          status: 'active',
          startDate: new Date('2021-08-01'),
          endDate: undefined,
          responsibilities: ['Curriculum development', 'Course approval process'],
          meetingSchedule: 'Weekly during academic year',
          description: 'Department curriculum oversight'
        }
      ]

      await profileStore.updateCommitteeAssignments(committees)
      
      expect(profileStore.committees).toEqual(committees)
      expect(profileStore.getActiveCommittees()).toHaveLength(3)
      expect(profileStore.getCommitteesByRole('chair')).toHaveLength(1)
      expect(profileStore.getCommitteesByRole('member')).toHaveLength(1)
      expect(profileStore.isCommitteeChair()).toBe(true)
    })

    it('should display professional memberships and affiliations', async () => {
      const profileStore = useFacultyProfileStore()
      const professionalInfo = {
        memberships: [
          {
            organization: 'Association for Computing Machinery (ACM)',
            membershipType: 'Senior Member',
            startDate: new Date('2010-01-01'),
            endDate: null,
            membershipId: 'ACM12345'
          },
          {
            organization: 'IEEE Computer Society',
            membershipType: 'Member',
            startDate: new Date('2008-01-01'),
            endDate: null,
            membershipId: 'IEEE67890'
          }
        ],
        affiliations: [
          {
            institution: 'Stanford AI Lab',
            role: 'Visiting Researcher',
            startDate: new Date('2019-06-01'),
            endDate: new Date('2019-08-31'),
            description: 'Collaborative research on neural language models'
          }
        ],
        certifications: [
          {
            name: 'Certified Data Scientist',
            issuingOrganization: 'Data Science Institute',
            issueDate: new Date('2020-03-15'),
            expiryDate: new Date('2025-03-15'),
            credentialId: 'DSI-CDS-2020-031'
          }
        ]
      }

      profileStore.setProfessionalInfo(professionalInfo)
      
      expect(profileStore.professionalMemberships).toEqual(professionalInfo.memberships)
      expect(profileStore.professionalAffiliations).toEqual(professionalInfo.affiliations)
      expect(profileStore.certifications).toEqual(professionalInfo.certifications)
      expect(profileStore.getActiveMemberships()).toHaveLength(2)
      expect(profileStore.getValidCertifications()).toHaveLength(1)
    })
  })

  describe('5. Administrative role indicators and permissions', () => {
    it('should display role hierarchy and administrative indicators', () => {
      const authStore = useAuthStore()
      
      // Test role display utilities
      expect(authStore.getRoleDisplayName('professor')).toBe('Professor')
      expect(authStore.getRoleDisplayName('chair')).toBe('Department Chair')
      expect(authStore.getRoleDisplayName('dean')).toBe('Dean')
      
      // Test administrative role indicators
      expect(authStore.isAdministrativeRole('professor')).toBe(false)
      expect(authStore.isAdministrativeRole('chair')).toBe(true)
      expect(authStore.isAdministrativeRole('dean')).toBe(true)
      
      // Test role color coding for UI
      expect(authStore.getRoleColor('professor')).toBe('primary')
      expect(authStore.getRoleColor('chair')).toBe('warning')
      expect(authStore.getRoleColor('dean')).toBe('danger')
    })

    it('should manage hierarchical permissions correctly', () => {
      const authStore = useAuthStore()
      
      // Mock chair user
      const chairUser: FacultyUser = {
        id: '2',
        email: 'john.doe@university.edu',
        firstName: 'John',
        lastName: 'Doe',
        role: 'chair',
        department: 'Computer Science',
        title: 'Department Chair',
        permissions: ['view_department', 'manage_department', 'manage_faculty'],
        officeLocation: 'Engineering 401',
        phoneNumber: '(555) 123-4568'
      }
      
      authStore.setUser(chairUser)
      
      // Test permission checks
      expect(authStore.canManageFaculty()).toBe(true)
      expect(authStore.canManageDepartment()).toBe(true)
      expect(authStore.canManageCollege()).toBe(false)
      
      // Test subordinate access
      expect(authStore.canAccessSubordinate('professor')).toBe(true)
      expect(authStore.canAccessSubordinate('dean')).toBe(false)
    })

    it('should handle administrative workflow permissions', () => {
      const authStore = useAuthStore()
      
      // Test workflow permissions for different roles
      const workflows = [
        'faculty_hiring',
        'promotion_review', 
        'tenure_review',
        'budget_approval',
        'course_approval',
        'student_discipline'
      ]
      
      // Chair should have some but not all permissions
      const chairUser: FacultyUser = {
        id: '2',
        email: 'chair@university.edu',
        firstName: 'John',
        lastName: 'Chair',
        role: 'chair',
        department: 'Computer Science',
        title: 'Department Chair',
        permissions: ['manage_faculty', 'view_department', 'manage_department'],
        officeLocation: 'Engineering 401'
      }
      
      authStore.setUser(chairUser)
      
      expect(authStore.canInitiateWorkflow('faculty_hiring')).toBe(true)
      expect(authStore.canInitiateWorkflow('promotion_review')).toBe(true)
      expect(authStore.canInitiateWorkflow('budget_approval')).toBe(false) // Dean level
      
      // Dean should have more permissions
      const deanUser: FacultyUser = {
        id: '3',
        email: 'dean@university.edu',
        firstName: 'Mary',
        lastName: 'Dean',
        role: 'dean',
        department: 'College of Engineering',
        title: 'Dean',
        permissions: ['manage_college', 'manage_department', 'manage_faculty'],
        officeLocation: 'Admin 501'
      }
      
      authStore.setUser(deanUser)
      
      expect(authStore.canInitiateWorkflow('budget_approval')).toBe(true)
      expect(authStore.canInitiateWorkflow('student_discipline')).toBe(true)
    })
  })

  describe('Integration Tests', () => {
    it('should integrate authentication with profile management', async () => {
      const authStore = useAuthStore()
      const profileStore = useFacultyProfileStore()
      
      // Login user
      const user: FacultyUser = {
        id: '1',
        email: 'test@university.edu',
        firstName: 'Test',
        lastName: 'User',
        role: 'professor',
        department: 'Computer Science',
        title: 'Professor',
        permissions: ['view_courses', 'manage_grades'],
        officeLocation: 'Engineering 301'
      }
      
      authStore.setUser(user)
      
      // Load profile should use auth user ID
      await profileStore.loadProfile(authStore.user!.id)
      
      expect(profileStore.profile?.userId).toBe(authStore.user!.id)
      expect(profileStore.isOwner(authStore.user!.id)).toBe(true)
    })

    it('should handle permission-based profile access', () => {
      const authStore = useAuthStore()
      const profileStore = useFacultyProfileStore()
      
      // Regular professor
      const professorUser: FacultyUser = {
        id: '1',
        email: 'prof@university.edu',
        firstName: 'Professor',
        lastName: 'Smith',
        role: 'professor',
        department: 'Computer Science',
        title: 'Professor',
        permissions: ['view_courses'],
        officeLocation: 'Engineering 301'
      }
      
      authStore.setUser(professorUser)
      
      // Can edit own profile
      expect(profileStore.canEditProfile('1')).toBe(true)
      // Cannot edit other profiles without admin permissions
      expect(profileStore.canEditProfile('2')).toBe(false)
      
      // Chair with manage_faculty permission
      const chairUser: FacultyUser = {
        id: '2',
        email: 'chair@university.edu',
        firstName: 'Department',
        lastName: 'Chair',
        role: 'chair',
        department: 'Computer Science',
        title: 'Chair',
        permissions: ['manage_faculty'],
        officeLocation: 'Engineering 401'
      }
      
      authStore.setUser(chairUser)
      
      // Can edit subordinate profiles
      expect(profileStore.canEditProfile('1')).toBe(true)
      expect(profileStore.canEditProfile('2')).toBe(true)
    })
  })

  describe('Error Handling and Edge Cases', () => {
    it('should handle missing profile data gracefully', async () => {
      const profileStore = useFacultyProfileStore()
      
      await profileStore.loadProfile('nonexistent')
      
      expect(profileStore.profile).toBeNull()
      expect(profileStore.error).toBeDefined()
      expect(profileStore.hasCV()).toBe(false)
      expect(profileStore.getTotalCitations()).toBe(0)
    })

    it('should validate office hours time conflicts', () => {
      const profileStore = useFacultyProfileStore()
      
      const conflictingHours: OfficeHours[] = [
        {
          id: '1',
          facultyId: '1',
          dayOfWeek: 'monday',
          startTime: '10:00',
          endTime: '12:00',
          location: 'Engineering 301',
          type: 'office_hours',
          isRecurring: true,
          maxAppointments: 6,
          appointmentDuration: 20
        },
        {
          id: '2',
          facultyId: '1',
          dayOfWeek: 'monday',
          startTime: '11:00',
          endTime: '13:00',
          location: 'Engineering 301',
          type: 'office_hours',
          isRecurring: true,
          maxAppointments: 6,
          appointmentDuration: 20
        }
      ]
      
      expect(() => profileStore.validateOfficeHours(conflictingHours))
        .toThrow('Office hours conflict detected')
    })

    it('should handle invalid role assignments', () => {
      const authStore = useAuthStore()
      
      expect(() => authStore.setRole('invalid_role' as FacultyRole))
        .toThrow('Invalid faculty role')
      
      expect(() => authStore.addPermission('invalid_permission' as Permission))
        .toThrow('Invalid permission')
    })
  })
})