/**
 * Faculty Profile Service
 * Handles API communication for faculty profile management, office hours, appointments, 
 * committees, and professional information
 */
import type { 
  FacultyProfile, 
  OfficeHours, 
  Appointment, 
  Committee, 
  ProfessionalMembership, 
  ProfessionalAffiliation, 
  Certification,
  Publication,
  ApiResponse 
} from '../types'

const API_BASE_URL = (import.meta as any).env?.VITE_API_BASE_URL || 'http://localhost:5000'

export class FacultyProfileService {
  private static async apiCall<T>(endpoint: string, options: RequestInit = {}): Promise<T> {
    const token = localStorage.getItem('zeus_auth_token')
    
    const defaultHeaders: HeadersInit = {
      'Content-Type': 'application/json',
    }
    
    if (token) {
      defaultHeaders['Authorization'] = `Bearer ${token}`
    }

    const config: RequestInit = {
      ...options,
      headers: {
        ...defaultHeaders,
        ...options.headers,
      },
    }

    try {
      const response = await fetch(`${API_BASE_URL}${endpoint}`, config)
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`)
      }

      const data: ApiResponse<T> = await response.json()
      
      if (!data.success) {
        throw new Error(data.error || 'API request failed')
      }

      return data.data
    } catch (error) {
      console.error('API call failed:', error)
      throw error
    }
  }

  /**
   * Profile Management
   */
  static async getProfile(userId: string): Promise<FacultyProfile> {
    // Use mock data in development
    if ((import.meta as any).env?.DEV) {
      return this.getMockProfile(userId)
    }
    return this.apiCall<FacultyProfile>(`/api/faculty/profile/${userId}`)
  }

  static async updateProfile(profileId: string, updates: Partial<FacultyProfile>): Promise<FacultyProfile> {
    return this.apiCall<FacultyProfile>(`/api/faculty/profile/${profileId}`, {
      method: 'PUT',
      body: JSON.stringify(updates),
    })
  }

  static async uploadCV(profileId: string, file: File): Promise<string> {
    const formData = new FormData()
    formData.append('cv', file)

    const token = localStorage.getItem('zeus_auth_token')
    const headers: HeadersInit = {}
    
    if (token) {
      headers['Authorization'] = `Bearer ${token}`
    }

    const response = await fetch(`${API_BASE_URL}/api/faculty/profile/${profileId}/cv`, {
      method: 'POST',
      headers,
      body: formData,
    })

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }

    const data: ApiResponse<{ cvUrl: string }> = await response.json()
    
    if (!data.success) {
      throw new Error(data.error || 'CV upload failed')
    }

    return data.data.cvUrl
  }

  /**
   * Office Hours Management
   */
  static async getOfficeHours(facultyId: string): Promise<OfficeHours[]> {
    // Use mock data in development
    if ((import.meta as any).env?.DEV) {
      return this.getMockOfficeHours(facultyId)
    }
    return this.apiCall<OfficeHours[]>(`/api/faculty/${facultyId}/office-hours`)
  }

  static async updateOfficeHours(facultyId: string, hours: OfficeHours[]): Promise<OfficeHours[]> {
    return this.apiCall<OfficeHours[]>(`/api/faculty/${facultyId}/office-hours`, {
      method: 'PUT',
      body: JSON.stringify({ officeHours: hours }),
    })
  }

  static async createOfficeHours(facultyId: string, hours: Omit<OfficeHours, 'id'>): Promise<OfficeHours> {
    return this.apiCall<OfficeHours>(`/api/faculty/${facultyId}/office-hours`, {
      method: 'POST',
      body: JSON.stringify(hours),
    })
  }

  static async deleteOfficeHours(facultyId: string, hoursId: string): Promise<void> {
    await this.apiCall<void>(`/api/faculty/${facultyId}/office-hours/${hoursId}`, {
      method: 'DELETE',
    })
  }

  /**
   * Appointment Management
   */
  static async getAppointments(facultyId: string): Promise<Appointment[]> {
    // Use mock data in development
    if ((import.meta as any).env?.DEV) {
      return this.getMockAppointments(facultyId)
    }
    return this.apiCall<Appointment[]>(`/api/faculty/${facultyId}/appointments`)
  }

  static async updateAppointment(appointmentId: string, updates: Partial<Appointment>): Promise<Appointment> {
    return this.apiCall<Appointment>(`/api/appointments/${appointmentId}`, {
      method: 'PUT',
      body: JSON.stringify(updates),
    })
  }

  static async cancelAppointment(appointmentId: string, reason?: string): Promise<void> {
    await this.apiCall<void>(`/api/appointments/${appointmentId}/cancel`, {
      method: 'POST',
      body: JSON.stringify({ reason }),
    })
  }

  static async confirmAppointment(appointmentId: string): Promise<Appointment> {
    return this.apiCall<Appointment>(`/api/appointments/${appointmentId}/confirm`, {
      method: 'POST',
    })
  }

  /**
   * Publication Management
   */
  static async getPublications(facultyId: string): Promise<Publication[]> {
    // Use mock data in development
    if ((import.meta as any).env?.DEV) {
      return this.getMockPublications(facultyId)
    }
    return this.apiCall<Publication[]>(`/api/faculty/${facultyId}/publications`)
  }

  static async addPublication(publication: Omit<Publication, 'id'>): Promise<Publication> {
    return this.apiCall<Publication>('/api/faculty/publications', {
      method: 'POST',
      body: JSON.stringify(publication),
    })
  }

  static async updatePublication(publicationId: string, updates: Partial<Publication>): Promise<Publication> {
    return this.apiCall<Publication>(`/api/faculty/publications/${publicationId}`, {
      method: 'PUT',
      body: JSON.stringify(updates),
    })
  }

  static async deletePublication(publicationId: string): Promise<void> {
    await this.apiCall<void>(`/api/faculty/publications/${publicationId}`, {
      method: 'DELETE',
    })
  }

  static async importPublicationsFromOrcid(orcidId: string): Promise<Publication[]> {
    return this.apiCall<Publication[]>(`/api/faculty/publications/import/orcid`, {
      method: 'POST',
      body: JSON.stringify({ orcidId }),
    })
  }

  static async importPublicationsFromGoogleScholar(scholarId: string): Promise<Publication[]> {
    return this.apiCall<Publication[]>(`/api/faculty/publications/import/google-scholar`, {
      method: 'POST',
      body: JSON.stringify({ scholarId }),
    })
  }

  /**
   * Committee Management
   */
  static async getCommittees(facultyId: string): Promise<Committee[]> {
    // Use mock data in development
    if ((import.meta as any).env?.DEV) {
      return this.getMockCommittees(facultyId)
    }
    return this.apiCall<Committee[]>(`/api/faculty/${facultyId}/committees`)
  }

  static async updateCommittees(facultyId: string, committees: Committee[]): Promise<Committee[]> {
    return this.apiCall<Committee[]>(`/api/faculty/${facultyId}/committees`, {
      method: 'PUT',
      body: JSON.stringify({ committees }),
    })
  }

  static async addCommitteeAssignment(facultyId: string, committee: Omit<Committee, 'id'>): Promise<Committee> {
    return this.apiCall<Committee>(`/api/faculty/${facultyId}/committees`, {
      method: 'POST',
      body: JSON.stringify(committee),
    })
  }

  static async updateCommitteeRole(committeeId: string, role: string, responsibilities: string[]): Promise<Committee> {
    return this.apiCall<Committee>(`/api/committees/${committeeId}/role`, {
      method: 'PUT',
      body: JSON.stringify({ role, responsibilities }),
    })
  }

  static async removeCommitteeAssignment(facultyId: string, committeeId: string): Promise<void> {
    await this.apiCall<void>(`/api/faculty/${facultyId}/committees/${committeeId}`, {
      method: 'DELETE',
    })
  }

  /**
   * Professional Information Management
   */
  static async getProfessionalInfo(facultyId: string): Promise<{
    memberships: ProfessionalMembership[]
    affiliations: ProfessionalAffiliation[]
    certifications: Certification[]
  }> {
    // Use mock data in development
    if ((import.meta as any).env?.DEV) {
      return this.getMockProfessionalInfo(facultyId)
    }
    return this.apiCall<{
      memberships: ProfessionalMembership[]
      affiliations: ProfessionalAffiliation[]
      certifications: Certification[]
    }>(`/api/faculty/${facultyId}/professional-info`)
  }

  static async updateProfessionalMemberships(
    facultyId: string, 
    memberships: ProfessionalMembership[]
  ): Promise<ProfessionalMembership[]> {
    return this.apiCall<ProfessionalMembership[]>(`/api/faculty/${facultyId}/professional-memberships`, {
      method: 'PUT',
      body: JSON.stringify({ memberships }),
    })
  }

  static async updateProfessionalAffiliations(
    facultyId: string, 
    affiliations: ProfessionalAffiliation[]
  ): Promise<ProfessionalAffiliation[]> {
    return this.apiCall<ProfessionalAffiliation[]>(`/api/faculty/${facultyId}/professional-affiliations`, {
      method: 'PUT',
      body: JSON.stringify({ affiliations }),
    })
  }

  static async updateCertifications(
    facultyId: string, 
    certifications: Certification[]
  ): Promise<Certification[]> {
    return this.apiCall<Certification[]>(`/api/faculty/${facultyId}/certifications`, {
      method: 'PUT',
      body: JSON.stringify({ certifications }),
    })
  }

  /**
   * Analytics and Reporting
   */
  static async getProfileAnalytics(facultyId: string): Promise<{
    totalPublications: number
    totalCitations: number
    hIndex: number
    publicationsByYear: Record<number, number>
    citationsByYear: Record<number, number>
    topKeywords: string[]
    collaborators: string[]
  }> {
    return this.apiCall(`/api/faculty/${facultyId}/analytics`)
  }

  static async getAppointmentStatistics(facultyId: string, dateRange?: {
    startDate: Date
    endDate: Date
  }): Promise<{
    totalAppointments: number
    confirmedAppointments: number
    cancelledAppointments: number
    noShowAppointments: number
    averageDuration: number
    popularTimeSlots: Array<{ time: string; count: number }>
    studentReturnRate: number
  }> {
    const params = dateRange ? new URLSearchParams({
      startDate: dateRange.startDate.toISOString(),
      endDate: dateRange.endDate.toISOString()
    }).toString() : ''
    
    return this.apiCall(`/api/faculty/${facultyId}/appointment-statistics${params ? `?${params}` : ''}`)
  }

  /**
   * Mock/Development Methods
   * These methods provide mock data for development and testing
   */
  static async getMockProfile(userId: string): Promise<FacultyProfile> {
    // Simulate API delay
    await new Promise(resolve => setTimeout(resolve, 500))
    
    return {
      id: userId,
      userId,
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
        }
      ],
      researchAreas: ['Artificial Intelligence', 'Machine Learning', 'Natural Language Processing'],
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
        }
      ],
      awards: [
        {
          name: 'Excellence in Teaching Award',
          institution: 'University of Technology',
          year: 2023,
          description: 'Recognized for outstanding contributions to undergraduate education'
        }
      ],
      professionalExperience: [
        {
          position: 'Professor',
          institution: 'University of Technology',
          startDate: new Date('2015-08-01'),
          endDate: null,
          description: 'Teaching undergraduate and graduate courses in AI and ML'
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
  }

  static async getMockOfficeHours(facultyId: string): Promise<OfficeHours[]> {
    await new Promise(resolve => setTimeout(resolve, 300))
    
    return [
      {
        id: '1',
        facultyId,
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
        facultyId,
        dayOfWeek: 'wednesday',
        startTime: '14:00',
        endTime: '16:00',
        location: 'Engineering Building 301',
        type: 'office_hours',
        isRecurring: true,
        maxAppointments: 6,
        appointmentDuration: 20
      }
    ]
  }

  static async getMockAppointments(_facultyId: string): Promise<Appointment[]> {
    await new Promise(resolve => setTimeout(resolve, 300))
    
    return [
      {
        id: '1',
        officeHoursId: '1',
        studentId: 'student123',
        studentName: 'John Student',
        studentEmail: 'john.student@university.edu',
        appointmentDate: new Date('2024-10-07'),
        startTime: '10:20',
        endTime: '10:40',
        status: 'confirmed',
        purpose: 'Discuss assignment questions',
        notes: 'Questions about machine learning project'
      }
    ]
  }

  static async getMockCommittees(_facultyId: string): Promise<Committee[]> {
    await new Promise(resolve => setTimeout(resolve, 300))
    
    return [
      {
        id: '1',
        name: 'Graduate Admissions Committee',
        department: 'Computer Science',
        role: 'member',
        startDate: new Date('2020-08-01'),
        endDate: null,
        responsibilities: ['Review graduate applications', 'Interview candidates'],
        meetingSchedule: 'Bi-weekly on Fridays',
        chairperson: 'Dr. Robert Smith'
      }
    ]
  }

  static async getMockProfessionalInfo(_facultyId: string): Promise<{
    memberships: ProfessionalMembership[]
    affiliations: ProfessionalAffiliation[]
    certifications: Certification[]
  }> {
    await new Promise(resolve => setTimeout(resolve, 300))
    
    return {
      memberships: [
        {
          organization: 'Association for Computing Machinery (ACM)',
          membershipType: 'Senior Member',
          startDate: new Date('2010-01-01'),
          endDate: null,
          membershipId: 'ACM12345'
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
  }

  static async getMockPublications(_facultyId: string): Promise<Publication[]> {
    await new Promise(resolve => setTimeout(resolve, 300))
    
    return [
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
    ]
  }
}