import { ref } from 'vue'

export interface Toast {
  id: string
  message: string
  type: 'success' | 'error' | 'warning' | 'info'
  duration?: number
}

const toasts = ref<Toast[]>([])
let toastId = 0

export function useToast() {
  const showToast = (message: string, type: Toast['type'] = 'info', duration = 5000) => {
    const id = (++toastId).toString()
    const toast: Toast = {
      id,
      message,
      type,
      duration
    }

    toasts.value.push(toast)

    // Auto remove toast after duration
    if (duration > 0) {
      setTimeout(() => {
        removeToast(id)
      }, duration)
    }

    return id
  }

  const removeToast = (id: string) => {
    const index = toasts.value.findIndex((toast: Toast) => toast.id === id)
    if (index > -1) {
      toasts.value.splice(index, 1)
    }
  }

  const clearAllToasts = () => {
    toasts.value = []
  }

  return {
    toasts: toasts,
    showToast,
    removeToast,
    clearAllToasts
  }
}