// This file ensures Vuex types work correctly with TypeScript

declare module '@vue/runtime-core' {
  // Declare your own store states
  interface ComponentCustomProperties {
    $store: import('vuex').Store<import('./store/types').RootState>
  }
}

// Global Vuex module declaration
declare module 'vuex' {
  export function useStore<S = any>(): import('vuex').Store<S>
}