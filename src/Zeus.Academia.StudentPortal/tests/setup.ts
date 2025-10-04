import { config } from '@vue/test-utils'
import 'vitest/globals'

// Mock Bootstrap Vue components for testing
config.global.stubs = {
  'b-navbar': true,
  'b-navbar-brand': true,
  'b-navbar-nav': true,
  'b-nav-item': true,
  'b-nav-item-dropdown': true,
  'b-dropdown-item': true,
  'b-button': true,
  'b-form': true,
  'b-form-group': true,
  'b-form-input': true,
  'b-form-select': true,
  'b-card': true,
  'b-card-header': true,
  'b-card-body': true,
  'b-table': true,
  'b-pagination': true,
  'b-alert': true,
  'b-spinner': true,
  'b-container': true,
  'b-row': true,
  'b-col': true
}

// Mock router-link and router-view
if (config.global.stubs) {
  Object.assign(config.global.stubs, {
    'router-link': { template: '<a><slot /></a>' },
    'router-view': { template: '<div><slot /></div>' }
  })
}

// Global test helpers
// Add global flushPromises helper for tests
(global as any).flushPromises = () => new Promise(resolve => setTimeout(resolve, 0))