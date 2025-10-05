<template>
  <div id="app" class="faculty-app">
    <div v-if="isLoading" class="loading-overlay">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>

    <router-view v-else />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useAuthStore } from "./stores/auth";

const authStore = useAuthStore();
const isLoading = ref(true);

onMounted(async () => {
  try {
    // Initialize authentication state
    await authStore.initializeAuth();
  } catch (error) {
    console.error("Failed to initialize app:", error);
  } finally {
    isLoading.value = false;
  }
});
</script>

<style lang="scss">
// Global styles for the faculty application
.faculty-app {
  min-height: 100vh;
  font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
  background-color: var(--bs-gray-50);
}

.loading-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(255, 255, 255, 0.9);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
}

// Faculty-specific color scheme
:root {
  --faculty-primary: #0d47a1;
  --faculty-secondary: #1976d2;
  --faculty-accent: #42a5f5;
  --faculty-success: #2e7d32;
  --faculty-warning: #f57c00;
  --faculty-danger: #d32f2f;
  --faculty-info: #0288d1;

  // Background colors
  --faculty-bg-primary: #fafafa;
  --faculty-bg-secondary: #ffffff;
  --faculty-bg-accent: #f5f5f5;

  // Text colors
  --faculty-text-primary: #212121;
  --faculty-text-secondary: #757575;
  --faculty-text-muted: #9e9e9e;
}

// Override Bootstrap primary color
.btn-primary {
  background-color: var(--faculty-primary);
  border-color: var(--faculty-primary);

  &:hover {
    background-color: var(--faculty-secondary);
    border-color: var(--faculty-secondary);
  }
}

.text-primary {
  color: var(--faculty-primary) !important;
}

// Faculty-specific utilities
.faculty-card {
  background: var(--faculty-bg-secondary);
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  transition: box-shadow 0.2s ease;

  &:hover {
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
  }
}

.faculty-header {
  background: linear-gradient(
    135deg,
    var(--faculty-primary),
    var(--faculty-secondary)
  );
  color: white;
  padding: 1rem 0;
}

// Responsive utilities for faculty workflows
@media (max-width: 768px) {
  .faculty-app {
    font-size: 14px;
  }
}

@media (min-width: 1200px) {
  .faculty-app {
    font-size: 16px;
  }
}
</style>
