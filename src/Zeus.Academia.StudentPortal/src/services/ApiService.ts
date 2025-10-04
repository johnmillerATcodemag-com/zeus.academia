import axios from 'axios'
import type { AxiosInstance, AxiosResponse, AxiosError, InternalAxiosRequestConfig } from 'axios'
import type { ApiResponse } from '../types'
import { setupMockApi } from './mockApi'

// Create axios instance with default configuration
const createAxiosInstance = (): AxiosInstance => {
  const instance = axios.create({
    baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api',
    timeout: 10000,
    headers: {
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    }
  })

  // Request interceptor - Add auth token
  instance.interceptors.request.use(
    (config: InternalAxiosRequestConfig) => {
      const token = localStorage.getItem('zeus_token')
      if (token && config.headers) {
        config.headers.Authorization = `Bearer ${token}`
      }
      return config
    },
    (error: AxiosError) => {
      console.error('Request interceptor error:', error)
      return Promise.reject(error)
    }
  )

  // Response interceptor - Handle common responses and errors
  instance.interceptors.response.use(
    (response: AxiosResponse) => {
      return response
    },
    async (error: AxiosError) => {
      const originalRequest = error.config as any

      // Handle 401 Unauthorized - token expired
      if (error.response?.status === 401 && !originalRequest._retry) {
        originalRequest._retry = true

        try {
          // Try to refresh token
          const refreshToken = localStorage.getItem('zeus_refresh_token')
          if (refreshToken) {
            const refreshResponse = await instance.post('/auth/refresh', {
              refreshToken
            })

            if (refreshResponse.data.success) {
              const { token, refreshToken: newRefreshToken } = refreshResponse.data.data
              localStorage.setItem('zeus_token', token)
              if (newRefreshToken) {
                localStorage.setItem('zeus_refresh_token', newRefreshToken)
              }

              // Retry original request with new token
              if (originalRequest.headers) {
                originalRequest.headers.Authorization = `Bearer ${token}`
              }
              return instance.request(originalRequest)
            }
          }
        } catch (refreshError) {
          // Refresh failed, redirect to login
          localStorage.removeItem('zeus_token')
          localStorage.removeItem('zeus_refresh_token')
          window.location.href = '/login'
          return Promise.reject(refreshError)
        }
      }

      // Handle other errors - let the API service handle error logging
      return Promise.reject(error)
    }
  )

  // Setup mock API for development/demo
  setupMockApi(instance)

  return instance
}

class ApiServiceClass {
  public axiosInstance: AxiosInstance

  constructor() {
    this.axiosInstance = createAxiosInstance()
  }

  // Generic GET request
  async get<T = any>(url: string, params?: any): Promise<ApiResponse<T>> {
    try {
      const response = await this.axiosInstance.get<T>(url, { params })
      return {
        success: true,
        data: response.data
      }
    } catch (error) {
      return this.handleError<T>(error as AxiosError)
    }
  }

  // Generic POST request
  async post<T = any>(url: string, data?: any): Promise<ApiResponse<T>> {
    try {
      const response = await this.axiosInstance.post<T>(url, data)
      return {
        success: true,
        data: response.data
      }
    } catch (error) {
      return this.handleError<T>(error as AxiosError)
    }
  }

  // Generic PUT request
  async put<T = any>(url: string, data?: any): Promise<ApiResponse<T>> {
    try {
      const response = await this.axiosInstance.put<T>(url, data)
      return {
        success: true,
        data: response.data
      }
    } catch (error) {
      return this.handleError<T>(error as AxiosError)
    }
  }

  // Generic PATCH request
  async patch<T = any>(url: string, data?: any): Promise<ApiResponse<T>> {
    try {
      const response = await this.axiosInstance.patch<T>(url, data)
      return {
        success: true,
        data: response.data
      }
    } catch (error) {
      return this.handleError<T>(error as AxiosError)
    }
  }

  // Generic DELETE request
  async delete<T = any>(url: string): Promise<ApiResponse<T>> {
    try {
      const response = await this.axiosInstance.delete<T>(url)
      return {
        success: true,
        data: response.data
      }
    } catch (error) {
      return this.handleError<T>(error as AxiosError)
    }
  }

  // File upload
  async uploadFile<T = any>(url: string, file: File, progressCallback?: (progress: number) => void): Promise<ApiResponse<T>> {
    try {
      const formData = new FormData()
      formData.append('file', file)

      const response = await this.axiosInstance.post<T>(url, formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        },
        onUploadProgress: (progressEvent: any) => {
          if (progressCallback && progressEvent.total) {
            const progress = Math.round((progressEvent.loaded * 100) / progressEvent.total)
            progressCallback(progress)
          }
        }
      })

      return {
        success: true,
        data: response.data
      }
    } catch (error) {
      return this.handleError<T>(error as AxiosError)
    }
  }

  // Download file
  async downloadFile(url: string, filename?: string): Promise<ApiResponse<Blob>> {
    try {
      const response = await this.axiosInstance.get(url, {
        responseType: 'blob'
      })

      // Create download link
      const blob = response.data
      const downloadUrl = window.URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = downloadUrl
      link.download = filename || 'download'
      document.body.appendChild(link)
      link.click()
      document.body.removeChild(link)
      window.URL.revokeObjectURL(downloadUrl)

      return {
        success: true,
        data: blob
      }
    } catch (error) {
      return this.handleError<Blob>(error as AxiosError)
    }
  }

  // Error handler
  private handleError<T = any>(error: AxiosError): ApiResponse<T> {
    let message = 'An unexpected error occurred'
    let errors: string[] = []

    if (error.response) {
      // Server responded with error status
      const { status, data } = error.response
      
      if (data && typeof data === 'object') {
        const errorData = data as any
        message = errorData.message || errorData.error || `Server error: ${status}`
        errors = errorData.errors || errorData.validationErrors || []
      } else {
        message = `Server error: ${status}`
      }
    } else if (error.request) {
      // Request was made but no response received
      message = 'Network error: Unable to connect to server'
    } else {
      // Something else happened
      message = error.message || 'Request setup error'
    }

    // Only log non-network errors to avoid console spam
    if (error.code !== 'ERR_NETWORK' && error.message !== 'Network Error') {
      console.error('API Error:', { message, errors })
    } else {
      console.debug('Network connection unavailable - using fallback data')
    }

    return {
      success: false,
      message,
      errors: errors.length > 0 ? errors : undefined
    }
  }

  // Set base URL
  setBaseURL(baseURL: string): void {
    this.axiosInstance.defaults.baseURL = baseURL
  }

  // Set timeout
  setTimeout(timeout: number): void {
    this.axiosInstance.defaults.timeout = timeout
  }

  // Set default headers
  setDefaultHeaders(headers: Record<string, string>): void {
    Object.assign(this.axiosInstance.defaults.headers.common, headers)
  }
}

// Export singleton instance
export const ApiService = new ApiServiceClass()
export default ApiService