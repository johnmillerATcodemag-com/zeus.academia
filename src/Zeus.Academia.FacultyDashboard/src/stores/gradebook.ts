import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { Course, Student, Assignment, Grade, GradebookState, GradebookFilters } from '@/types'
import { GradebookService } from '@/services/GradebookService'

export const useGradebookStore = defineStore('gradebook', () => {
  // State
  const courses = ref<Course[]>([])
  const selectedCourse = ref<Course | null>(null)
  const students = ref<Student[]>([])
  const assignments = ref<Assignment[]>([])
  const grades = ref<Grade[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)
  const bulkEditMode = ref(false)
  const view = ref<'grid' | 'list'>('grid')
  
  const filters = ref<GradebookFilters>({
    studentSearch: '',
    assignmentFilter: '',
    gradeFilter: 'all',
    sortBy: 'name',
    sortOrder: 'asc'
  })

  // Getters
  const filteredStudents = computed(() => {
    let filtered = [...students.value]
    
    if (filters.value.studentSearch) {
      const search = filters.value.studentSearch.toLowerCase()
      filtered = filtered.filter(student => 
        student.firstName.toLowerCase().includes(search) ||
        student.lastName.toLowerCase().includes(search) ||
        student.email.toLowerCase().includes(search) ||
        student.studentId.toLowerCase().includes(search)
      )
    }
    
    // Sort students
    filtered.sort((a, b) => {
      const aValue = filters.value.sortBy === 'name' 
        ? `${a.lastName}, ${a.firstName}`
        : a[filters.value.sortBy as keyof Student] as string
      const bValue = filters.value.sortBy === 'name'
        ? `${b.lastName}, ${b.firstName}`
        : b[filters.value.sortBy as keyof Student] as string
        
      const comparison = aValue.localeCompare(bValue.toString())
      return filters.value.sortOrder === 'asc' ? comparison : -comparison
    })
    
    return filtered
  })

  const filteredAssignments = computed(() => {
    let filtered = [...assignments.value]
    
    if (filters.value.assignmentFilter) {
      filtered = filtered.filter(assignment => 
        assignment.type === filters.value.assignmentFilter
      )
    }
    
    return filtered.sort((a, b) => a.dueDate.getTime() - b.dueDate.getTime())
  })

  const courseStats = computed(() => {
    if (!selectedCourse.value) return null
    
    const totalStudents = students.value.length
    const totalAssignments = assignments.value.length
    const totalGrades = grades.value.length
    const gradedCount = grades.value.filter(g => g.score !== null && g.score !== undefined).length
    const gradingProgress = totalGrades > 0 ? (gradedCount / totalGrades) * 100 : 0
    
    return {
      totalStudents,
      totalAssignments,
      totalGrades,
      gradedCount,
      gradingProgress
    }
  })

  // Actions
  const loadCourses = async () => {
    loading.value = true
    error.value = null
    
    try {
      const response = await GradebookService.getCourses()
      if (response.success) {
        courses.value = response.data
      } else {
        error.value = response.error || 'Failed to load courses'
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'An unexpected error occurred'
    } finally {
      loading.value = false
    }
  }

  const selectCourse = async (courseId: string) => {
    loading.value = true
    error.value = null
    
    try {
      const course = courses.value.find(c => c.id === courseId)
      if (!course) {
        throw new Error('Course not found')
      }
      
      selectedCourse.value = course
      
      // Load course data in parallel
      const [studentsResponse, assignmentsResponse, gradesResponse] = await Promise.all([
        GradebookService.getStudents(courseId),
        GradebookService.getAssignments(courseId),
        GradebookService.getGrades(courseId)
      ])
      
      if (studentsResponse.success) students.value = studentsResponse.data
      if (assignmentsResponse.success) assignments.value = assignmentsResponse.data
      if (gradesResponse.success) grades.value = gradesResponse.data
      
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load course data'
    } finally {
      loading.value = false
    }
  }

  const updateGrade = async (gradeId: string, score: number, feedback?: string) => {
    try {
      const response = await GradebookService.updateGrade(gradeId, score, feedback)
      if (response.success) {
        const gradeIndex = grades.value.findIndex(g => g.id === gradeId)
        if (gradeIndex !== -1) {
          grades.value[gradeIndex] = response.data
        }
        return { success: true }
      } else {
        return { success: false, error: response.error }
      }
    } catch (err) {
      return { success: false, error: err instanceof Error ? err.message : 'Update failed' }
    }
  }

  const bulkUpdateGrades = async (updates: Array<{ gradeId: string; score: number; feedback?: string }>) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await GradebookService.bulkUpdateGrades(updates)
      if (response.success) {
        // Update local grades
        response.data.forEach((updatedGrade: Grade) => {
          const index = grades.value.findIndex(g => g.id === updatedGrade.id)
          if (index !== -1) {
            grades.value[index] = updatedGrade
          }
        })
        return { success: true }
      } else {
        error.value = response.error || 'Bulk update failed'
        return { success: false, error: error.value }
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Bulk update failed'
      return { success: false, error: error.value }
    } finally {
      loading.value = false
    }
  }

  const calculateWeightedGrades = async (courseId: string) => {
    try {
      const response = await GradebookService.calculateWeightedGrades(courseId)
      if (response.success) {
        return response.data
      } else {
        throw new Error(response.error || 'Calculation failed')
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Grade calculation failed'
      throw err
    }
  }

  const exportGradebook = async (courseId: string, format: 'csv' | 'xlsx' | 'pdf') => {
    loading.value = true
    
    try {
      const response = await GradebookService.exportGradebook(courseId, format)
      if (response.success) {
        // Create download link
        const link = document.createElement('a')
        link.href = response.data.downloadUrl
        link.download = `gradebook-${courseId}.${format}`
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
        
        return { success: true }
      } else {
        return { success: false, error: response.error }
      }
    } catch (err) {
      return { success: false, error: err instanceof Error ? err.message : 'Export failed' }
    } finally {
      loading.value = false
    }
  }

  const resetFilters = () => {
    filters.value = {
      studentSearch: '',
      assignmentFilter: '',
      gradeFilter: 'all',
      sortBy: 'name',
      sortOrder: 'asc'
    }
  }

  const toggleBulkEditMode = () => {
    bulkEditMode.value = !bulkEditMode.value
  }

  const setView = (newView: 'grid' | 'list') => {
    view.value = newView
  }

  return {
    // State
    courses,
    selectedCourse,
    students,
    assignments,
    grades,
    loading,
    error,
    bulkEditMode,
    view,
    filters,
    
    // Getters
    filteredStudents,
    filteredAssignments,
    courseStats,
    
    // Actions
    loadCourses,
    selectCourse,
    updateGrade,
    bulkUpdateGrades,
    calculateWeightedGrades,
    exportGradebook,
    resetFilters,
    toggleBulkEditMode,
    setView
  }
})