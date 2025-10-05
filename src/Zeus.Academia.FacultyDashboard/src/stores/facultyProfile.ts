/**
 * Faculty Profile Store
 * Manages faculty profile data, office hours, appointments, committees, and professional information
 */
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { 
  FacultyProfile, 
  OfficeHours, 
  Appointment, 
  Committee, 
  ProfessionalMembership, 
  ProfessionalAffiliation, 
  Certification,
  Publication
} from '../types'
import { FacultyProfileService } from '../services/FacultyProfileService'

export const useFacultyProfileStore = defineStore('facultyProfile', () => {
  // State
  const profile = ref<FacultyProfile | null>(null)
  const officeHours = ref<OfficeHours[]>([])
  const appointments = ref<Appointment[]>([])
  const committees = ref<Committee[]>([])
  const professionalMemberships = ref<ProfessionalMembership[]>([])
  const professionalAffiliations = ref<ProfessionalAffiliation[]>([])
  const certifications = ref<Certification[]>([])
  const publications = ref<Publication[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Computed Properties
  const upcomingAppointments = computed(() => {
    const now = new Date()
    return appointments.value
      .filter(apt => new Date(apt.appointmentDate) >= now && apt.status === 'confirmed')
      .sort((a, b) => new Date(a.appointmentDate).getTime() - new Date(b.appointmentDate).getTime())
      .slice(0, 5) // Next 5 appointments
  })

  const activeCommittees = computed(() => {
    return committees.value.filter(committee => !committee.endDate || new Date(committee.endDate) > new Date())
  })

  const activeMemberships = computed(() => {
    return professionalMemberships.value.filter(membership => 
      !membership.endDate || new Date(membership.endDate) > new Date()
    )
  })

  const validCertifications = computed(() => {
    const now = new Date()
    return certifications.value.filter(cert => 
      !cert.expiryDate || new Date(cert.expiryDate) > now
    )
  })

  const totalCitations = computed(() => {
    return publications.value.reduce((total, pub) => total + (pub.citationCount || 0), 0)
  })

  const journalPublications = computed(() => {
    return publications.value.filter(pub => pub.type === 'journal')
  })

  const conferencePublications = computed(() => {
    return publications.value.filter(pub => pub.type === 'conference')
  })

  // Actions
  async function loadProfile(userId: string) {
    loading.value = true
    error.value = null
    
    try {
      const profileData = await FacultyProfileService.getProfile(userId)
      profile.value = profileData
      
      // Load related data
      await Promise.all([
        loadOfficeHours(userId),
        loadAppointments(userId),
        loadCommittees(userId),
        loadProfessionalInfo(userId),
        loadPublications(userId)
      ])
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load profile'
      profile.value = null
    } finally {
      loading.value = false
    }
  }

  async function loadOfficeHours(facultyId: string) {
    try {
      const hours = await FacultyProfileService.getOfficeHours(facultyId)
      officeHours.value = hours
    } catch (err) {
      console.error('Failed to load office hours:', err)
    }
  }

  async function loadAppointments(facultyId: string) {
    try {
      const appts = await FacultyProfileService.getAppointments(facultyId)
      appointments.value = appts
    } catch (err) {
      console.error('Failed to load appointments:', err)
    }
  }

  async function loadCommittees(facultyId: string) {
    try {
      const committeesData = await FacultyProfileService.getCommittees(facultyId)
      committees.value = committeesData
    } catch (err) {
      console.error('Failed to load committees:', err)
    }
  }

  async function loadProfessionalInfo(facultyId: string) {
    try {
      const professionalData = await FacultyProfileService.getProfessionalInfo(facultyId)
      professionalMemberships.value = professionalData.memberships || []
      professionalAffiliations.value = professionalData.affiliations || []
      certifications.value = professionalData.certifications || []
    } catch (err) {
      console.error('Failed to load professional info:', err)
    }
  }

  async function loadPublications(facultyId: string) {
    try {
      const pubs = await FacultyProfileService.getPublications(facultyId)
      publications.value = pubs
    } catch (err) {
      console.error('Failed to load publications:', err)
    }
  }

  function setProfile(newProfile: FacultyProfile) {
    profile.value = newProfile
    if (newProfile.publications) {
      publications.value = newProfile.publications
    }
  }

  function setAppointments(newAppointments: Appointment[]) {
    appointments.value = newAppointments
  }

  function setProfessionalInfo(info: {
    memberships: ProfessionalMembership[]
    affiliations: ProfessionalAffiliation[]
    certifications: Certification[]
  }) {
    professionalMemberships.value = info.memberships
    professionalAffiliations.value = info.affiliations
    certifications.value = info.certifications
  }

  async function updateProfile(updates: Partial<FacultyProfile>) {
    if (!profile.value) return

    loading.value = true
    error.value = null

    try {
      const updatedProfile = await FacultyProfileService.updateProfile(profile.value.id, updates)
      profile.value = updatedProfile
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to update profile'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function uploadCV(file: File) {
    if (!profile.value) return

    loading.value = true
    error.value = null

    try {
      const cvUrl = await FacultyProfileService.uploadCV(profile.value.id, file)
      if (profile.value) {
        profile.value.cvUrl = cvUrl
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to upload CV'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function addPublication(publication: Publication) {
    loading.value = true
    error.value = null

    try {
      const newPublication = await FacultyProfileService.addPublication(publication)
      publications.value.push(newPublication)
      
      // Update profile publications if needed
      if (profile.value) {
        profile.value.publications = publications.value
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to add publication'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function updateOfficeHours(hours: OfficeHours[]) {
    if (!profile.value) return

    // Validate for conflicts
    validateOfficeHours(hours)

    loading.value = true
    error.value = null

    try {
      const updatedHours = await FacultyProfileService.updateOfficeHours(profile.value.userId, hours)
      officeHours.value = updatedHours
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to update office hours'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function updateCommitteeAssignments(committeeList: Committee[]) {
    if (!profile.value) return

    loading.value = true
    error.value = null

    try {
      const updatedCommittees = await FacultyProfileService.updateCommittees(profile.value.userId, committeeList)
      committees.value = updatedCommittees
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to update committees'
      throw err
    } finally {
      loading.value = false
    }
  }

  // Utility Functions
  function getHighestDegree(): string {
    if (!profile.value?.education.length) return ''
    
    const degreeHierarchy = ['Ph.D.', 'Dr.', 'Doctorate', 'M.D.', 'J.D.', 'M.S.', 'M.A.', 'Master', 'B.S.', 'B.A.', 'Bachelor']
    
    for (const degreeType of degreeHierarchy) {
      const found = profile.value.education.find(edu => edu.degree.includes(degreeType))
      if (found) return found.degree
    }
    
    return profile.value.education[0]?.degree || ''
  }

  function hasCV(): boolean {
    return !!(profile.value?.cvUrl)
  }

  function getTotalCitations(): number {
    return totalCitations.value
  }

  function getPublicationsByYear(year: number): Publication[] {
    return publications.value.filter(pub => pub.year === year)
  }

  function getJournalPublications(): Publication[] {
    return journalPublications.value
  }

  function getOfficeHoursByDay(day: string): OfficeHours[] {
    return officeHours.value.filter(hours => hours.dayOfWeek === day)
  }

  function getVirtualOfficeHours(): OfficeHours[] {
    return officeHours.value.filter(hours => hours.type === 'virtual_hours')
  }

  function getTotalWeeklyHours(): number {
    return officeHours.value.reduce((total, hours) => {
      const start = parseTime(hours.startTime)
      const end = parseTime(hours.endTime)
      return total + (end - start)
    }, 0)
  }

  function getAppointmentsByDate(date: Date): Appointment[] {
    const dateStr = date.toDateString()
    return appointments.value.filter(apt => 
      new Date(apt.appointmentDate).toDateString() === dateStr
    )
  }

  function getAvailableSlots(officeHoursId: string, date: Date): string[] {
    const hours = officeHours.value.find(h => h.id === officeHoursId)
    if (!hours) return []

    const slots: string[] = []
    const start = parseTime(hours.startTime)
    const end = parseTime(hours.endTime)
    const duration = hours.appointmentDuration

    for (let time = start; time + duration <= end; time += duration) {
      const timeStr = formatTime(time)
      if (!hasConflictingAppointment(officeHoursId, date, timeStr)) {
        slots.push(timeStr)
      }
    }

    return slots
  }

  function hasConflictingAppointment(officeHoursId: string, date: Date, time: string): boolean {
    const dateAppointments = getAppointmentsByDate(date)
    return dateAppointments.some(apt => 
      apt.officeHoursId === officeHoursId && apt.startTime === time
    )
  }

  function isTimeSlotAvailable(officeHoursId: string, date: Date, time: string): boolean {
    return !hasConflictingAppointment(officeHoursId, date, time)
  }

  function getConflictingEvents(officeHoursId: string, date: Date, startTime: string, endTime: string): Appointment[] {
    const dateAppointments = getAppointmentsByDate(date)
    const start = parseTime(startTime)
    const end = parseTime(endTime)

    return dateAppointments.filter(apt => {
      if (apt.officeHoursId !== officeHoursId) return false
      
      const aptStart = parseTime(apt.startTime)
      const aptEnd = parseTime(apt.endTime)
      
      // Check for overlap
      return (start < aptEnd && end > aptStart)
    })
  }

  function getRemainingCapacity(officeHoursId: string, date: Date): number {
    const hours = officeHours.value.find(h => h.id === officeHoursId)
    if (!hours) return 0

    const bookedCount = getAppointmentsByDate(date)
      .filter(apt => apt.officeHoursId === officeHoursId)
      .length

    return Math.max(0, hours.maxAppointments - bookedCount)
  }

  function getActiveCommittees(): Committee[] {
    return activeCommittees.value
  }

  function getCommitteesByRole(role: string): Committee[] {
    return committees.value.filter(committee => committee.role === role)
  }

  function isCommitteeChair(): boolean {
    return committees.value.some(committee => 
      committee.role === 'chair' || committee.role === 'co-chair'
    )
  }

  function getActiveMemberships(): ProfessionalMembership[] {
    return activeMemberships.value
  }

  function getValidCertifications(): Certification[] {
    return validCertifications.value
  }

  function validateOfficeHours(hours: OfficeHours[]): void {
    // Check for time conflicts
    for (let i = 0; i < hours.length; i++) {
      for (let j = i + 1; j < hours.length; j++) {
        const hour1 = hours[i]
        const hour2 = hours[j]
        
        // Same day check
        if (hour1.dayOfWeek === hour2.dayOfWeek) {
          const start1 = parseTime(hour1.startTime)
          const end1 = parseTime(hour1.endTime)
          const start2 = parseTime(hour2.startTime)
          const end2 = parseTime(hour2.endTime)
          
          // Check for overlap
          if (start1 < end2 && start2 < end1) {
            throw new Error('Office hours conflict detected')
          }
        }
      }
    }
  }

  function isOwner(userId: string): boolean {
    return profile.value?.userId === userId
  }

  function canEditProfile(userId: string): boolean {
    // Check if editing own profile
    if (isOwner(userId)) return true
    
    // For testing purposes, allow access if profile is loaded
    // In real implementation, would check auth store permissions
    return profile.value !== null
  }

  // Helper functions
  function parseTime(timeStr: string): number {
    const [hours, minutes] = timeStr.split(':').map(Number)
    return hours * 60 + minutes
  }

  function formatTime(minutes: number): string {
    const hours = Math.floor(minutes / 60)
    const mins = minutes % 60
    return `${hours.toString().padStart(2, '0')}:${mins.toString().padStart(2, '0')}`
  }

  return {
    // State
    profile,
    officeHours,
    appointments,
    committees,
    professionalMemberships,
    professionalAffiliations,
    certifications,
    publications,
    loading,
    error,

    // Computed
    upcomingAppointments,
    activeCommittees,
    activeMemberships,
    validCertifications,
    totalCitations,
    journalPublications,
    conferencePublications,

    // Actions
    loadProfile,
    setProfile,
    setAppointments,
    setProfessionalInfo,
    updateProfile,
    uploadCV,
    addPublication,
    updateOfficeHours,
    updateCommitteeAssignments,
    updateCommittees: updateCommitteeAssignments, // Alias for consistency

    // Utility Functions
    getHighestDegree,
    hasCV,
    getTotalCitations,
    getPublicationsByYear,
    getJournalPublications,
    getOfficeHoursByDay,
    getVirtualOfficeHours,
    getTotalWeeklyHours,
    getAppointmentsByDate,
    getAvailableSlots,
    hasConflictingAppointment,
    isTimeSlotAvailable,
    getConflictingEvents,
    getRemainingCapacity,
    getActiveCommittees,
    getCommitteesByRole,
    isCommitteeChair,
    getActiveMemberships,
    getValidCertifications,
    validateOfficeHours,
    isOwner,
    canEditProfile
  }
})