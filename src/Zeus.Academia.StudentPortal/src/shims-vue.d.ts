// Type declarations to help with module resolution issues
// These are workarounds for TypeScript language server issues

declare module 'vue' {
  export function createApp(options: any): any
  export function ref<T>(value: T): any
  export function reactive<T>(obj: T): T
  export function computed<T>(fn: () => T): any
  export function onMounted(fn: () => void): void
  export function watch(source: any, cb: any, options?: any): void
  export function watchEffect(fn: () => void): void
}

declare module 'vue-router' {
  export function createRouter(options: any): any
  export function createWebHistory(base?: string): any
  export function useRouter(): any
  export interface RouteRecordRaw {
    path: string
    name?: string
    component?: any
    meta?: any
    children?: RouteRecordRaw[]
  }
}

declare module 'vuex' {
  export function createStore(options: any): any
  export function useStore(): any
  export interface Store<S> {
    state: S
    getters: any
    dispatch: (type: string, payload?: any) => Promise<any>
    commit: (type: string, payload?: any) => void
  }
}

declare module 'axios' {
  export interface AxiosInstance {
    request<T = any>(config: any): Promise<AxiosResponse<T>>
    get<T = any>(url: string, config?: any): Promise<AxiosResponse<T>>
    post<T = any>(url: string, data?: any, config?: any): Promise<AxiosResponse<T>>
    put<T = any>(url: string, data?: any, config?: any): Promise<AxiosResponse<T>>
    patch<T = any>(url: string, data?: any, config?: any): Promise<AxiosResponse<T>>
    delete<T = any>(url: string, config?: any): Promise<AxiosResponse<T>>
    interceptors: {
      request: any
      response: any
    }
    defaults: any
    create(config?: any): AxiosInstance
  }
  
  export interface AxiosResponse<T = any> {
    data: T
    status: number
    statusText: string
    headers: any
    config: any
  }
  
  export interface AxiosError extends Error {
    config: any
    code?: string
    request?: any
    response?: AxiosResponse
  }
  
  export interface InternalAxiosRequestConfig {
    headers?: any
    [key: string]: any
  }
  
  export function create(config?: any): AxiosInstance
  const axios: AxiosInstance & {
    create(config?: any): AxiosInstance
  }
  export default axios
}

declare module 'vite' {
  export function defineConfig(config: any): any
  export interface UserConfig {
    [key: string]: any
  }
}

declare module '@vitejs/plugin-vue' {
  export default function vue(options?: any): any
}

// Build configuration type declarations
declare module 'vitest' {
  export function describe(name: string, fn: () => void): void
  export function it(name: string, fn: () => void | Promise<void>): void
  export function expect(value: any): any
  export function beforeEach(fn: () => void | Promise<void>): void
  export const vi: {
    fn(): any
    spyOn(obj: any, method: string): any
    clearAllMocks(): void
  }
  export interface UserConfig {
    [key: string]: any
  }
}

declare module '@vue/test-utils' {
  export function mount(component: any, options?: any): any
  export const config: {
    global: {
      stubs: Record<string, any>
      [key: string]: any
    }
  }
}