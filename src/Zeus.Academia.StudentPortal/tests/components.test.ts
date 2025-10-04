import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import { createStore } from 'vuex'
import NavBar from '@/components/NavBar.vue'
import Footer from '@/components/Footer.vue'

describe('Component Tests', () => {
  const mockStore = createStore({
    state: {
      auth: {
        isAuthenticated: false,
        student: null,
        token: null,
        refreshToken: null
      }
    },
    modules: {
      auth: {
        namespaced: true,
        state: {
          isAuthenticated: false,
          student: null
        },
        mutations: {
          SET_AUTHENTICATED: (state: any, value: boolean) => {
            state.isAuthenticated = value
          }
        }
      }
    }
  })

  describe('NavBar Component', () => {
    it('should render navigation bar', () => {
      const wrapper = mount(NavBar, {
        global: {
          plugins: [mockStore],
          stubs: ['router-link']
        }
      })
      
      expect(wrapper.exists()).toBe(true)
      expect(wrapper.find('nav').exists()).toBe(true)
    })

    it('should show login link when not authenticated', () => {
      // Ensure the mock store returns false for isAuthenticated
      const mockStoreWithAuth = createStore({
        modules: {
          auth: {
            namespaced: true,
            state: { isAuthenticated: false, student: null, token: null, refreshToken: null },
            getters: {
              isAuthenticated: (state: any) => state.isAuthenticated,
              currentStudent: (state: any) => state.student
            }
          }
        }
      })
      
      const wrapper = mount(NavBar, {
        global: {
          plugins: [mockStoreWithAuth],
          stubs: {
            'router-link': {
              template: '<a :to="to"><slot /></a>',
              props: ['to']
            }
          }
        }
      })
      
      expect(wrapper.text()).toContain('Login')
    })
  })

  describe('Footer Component', () => {
    it('should render footer', () => {
      const wrapper = mount(Footer)
      
      expect(wrapper.exists()).toBe(true)
      expect(wrapper.find('footer').exists()).toBe(true)
    })

    it('should contain Zeus Academia branding', () => {
      const wrapper = mount(Footer)
      
      expect(wrapper.text()).toContain('Zeus Academia')
    })
  })
})