import axios, { type AxiosInstance, type AxiosResponse } from 'axios'
import type { ApiResponse, Course, Student, Assignment, Grade } from '@/types'

class GradebookServiceClass {
  private apiClient: AxiosInstance

  constructor() {
    this.apiClient = axios.create({
      baseURL: (import.meta as any).env?.VITE_API_BASE_URL || 'http://localhost:5000/api',
      timeout: 15000,
      headers: {
        'Content-Type': 'application/json',
      },
    })

    // Add request interceptor for auth token
    this.apiClient.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('zeus_faculty_token')
        if (token) {
          config.headers.Authorization = `Bearer ${token}`
        }
        return config
      },
      (error) => Promise.reject(error)
    )
  }

  async getCourses(facultyId: string): Promise<ApiResponse<Course[]>> {
    try {
      const response: AxiosResponse<any> = await this.apiClient.get(`/faculty/${facultyId}/courses`)
      
      // Handle nested API response structure
      if (response.data.success && response.data.data) {
        return {
          success: true,
          data: response.data.data, // Extract the actual courses array
        }
      } else {
        return {
          success: false,
          data: [],
          error: response.data.error || 'API returned success=false',
        }
      }
    } catch (error: any) {
      return {
        success: false,
        data: [],
        error: error.response?.data?.message || 'Failed to load courses',
      }
    }
  }

  async getStudents(courseId: string): Promise<ApiResponse<Student[]>> {
    try {
      const response: AxiosResponse<Student[]> = await this.apiClient.get(`/faculty/courses/${courseId}/students`)
      return {
        success: true,
        data: response.data,
      }
    } catch (error: any) {
      return {
        success: false,
        data: [],
        error: error.response?.data?.message || 'Failed to load students',
      }
    }
  }

  async getAssignments(courseId: string): Promise<ApiResponse<Assignment[]>> {
    try {
      const response: AxiosResponse<Assignment[]> = await this.apiClient.get(`/faculty/courses/${courseId}/assignments`)
      return {
        success: true,
        data: response.data,
      }
    } catch (error: any) {
      return {
        success: false,
        data: [],
        error: error.response?.data?.message || 'Failed to load assignments',
      }
    }
  }

  async getGrades(courseId: string): Promise<ApiResponse<Grade[]>> {
    try {
      const response: AxiosResponse<Grade[]> = await this.apiClient.get(`/faculty/courses/${courseId}/grades`)
      return {
        success: true,
        data: response.data,
      }
    } catch (error: any) {
      return {
        success: false,
        data: [],
        error: error.response?.data?.message || 'Failed to load grades',
      }
    }
  }

  async updateGrade(gradeId: string, score: number, feedback?: string): Promise<ApiResponse<Grade>> {
    try {
      const response: AxiosResponse<Grade> = await this.apiClient.put(`/faculty/grades/${gradeId}`, {
        score,
        feedback,
      })
      return {
        success: true,
        data: response.data,
      }
    } catch (error: any) {
      return {
        success: false,
        data: {} as Grade,
        error: error.response?.data?.message || 'Failed to update grade',
      }
    }
  }

  async bulkUpdateGrades(updates: Array<{ gradeId: string; score: number; feedback?: string }>): Promise<ApiResponse<Grade[]>> {
    try {
      const response: AxiosResponse<Grade[]> = await this.apiClient.put('/faculty/grades/bulk', {
        updates,
      })
      return {
        success: true,
        data: response.data,
      }
    } catch (error: any) {
      return {
        success: false,
        data: [],
        error: error.response?.data?.message || 'Failed to update grades',
      }
    }
  }

  async calculateWeightedGrades(courseId: string): Promise<ApiResponse<any>> {
    try {
      const response = await this.apiClient.post(`/faculty/courses/${courseId}/calculate-grades`)
      return {
        success: true,
        data: response.data,
      }
    } catch (error: any) {
      return {
        success: false,
        data: null,
        error: error.response?.data?.message || 'Failed to calculate grades',
      }
    }
  }

  async exportGradebook(courseId: string, format: 'csv' | 'xlsx' | 'pdf'): Promise<ApiResponse<{ downloadUrl: string }>> {
    try {
      const response: AxiosResponse<{ downloadUrl: string }> = await this.apiClient.post(`/faculty/courses/${courseId}/export`, {
        format,
      })
      return {
        success: true,
        data: response.data,
      }
    } catch (error: any) {
      return {
        success: false,
        data: { downloadUrl: '' },
        error: error.response?.data?.message || 'Failed to export gradebook',
      }
    }
  }

  async createAssignment(courseId: string, assignment: Omit<Assignment, 'id' | 'courseId'>): Promise<ApiResponse<Assignment>> {
    try {
      const response: AxiosResponse<Assignment> = await this.apiClient.post(`/faculty/courses/${courseId}/assignments`, assignment)
      return {
        success: true,
        data: response.data,
      }
    } catch (error: any) {
      return {
        success: false,
        data: {} as Assignment,
        error: error.response?.data?.message || 'Failed to create assignment',
      }
    }
  }

  async updateAssignment(assignmentId: string, assignment: Partial<Assignment>): Promise<ApiResponse<Assignment>> {
    try {
      const response: AxiosResponse<Assignment> = await this.apiClient.put(`/faculty/assignments/${assignmentId}`, assignment)
      return {
        success: true,
        data: response.data,
      }
    } catch (error: any) {
      return {
        success: false,
        data: {} as Assignment,
        error: error.response?.data?.message || 'Failed to update assignment',
      }
    }
  }

  async deleteAssignment(assignmentId: string): Promise<ApiResponse<void>> {
    try {
      await this.apiClient.delete(`/faculty/assignments/${assignmentId}`)
      return {
        success: true,
        data: undefined,
      }
    } catch (error: any) {
      return {
        success: false,
        data: undefined,
        error: error.response?.data?.message || 'Failed to delete assignment',
      }
    }
  }
}

export const GradebookService = new GradebookServiceClass()