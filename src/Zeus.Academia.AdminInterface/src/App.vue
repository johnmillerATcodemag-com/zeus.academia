<template>
  <div id="app" class="admin-app">
    <div v-if="isLoading" class="loading-overlay">
      <div class="d-flex justify-content-center align-items-center vh-100">
        <div class="spinner-border text-primary" role="status">
          <span class="visually-hidden">Loading...</span>
        </div>
      </div>
    </div>

    <router-view v-else />

    <!-- Global toast notifications -->
    <div class="toast-container position-fixed bottom-0 end-0 p-3">
      <!-- Toasts will be rendered here -->
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from "vue";
import { useAuthStore } from "./stores/auth";

const authStore = useAuthStore();
const isLoading = ref(true);

// Session timeout monitoring
let sessionTimeoutInterval: number | null = null;

onMounted(async () => {
  try {
    // Initialize authentication state
    await authStore.initializeAuth();

    // Set up session timeout monitoring
    sessionTimeoutInterval = window.setInterval(() => {
      if (authStore.isAuthenticated && authStore.checkSessionTimeout()) {
        authStore.logout();
      }
    }, 60000); // Check every minute
  } catch (error) {
    console.error("Failed to initialize app:", error);
  } finally {
    isLoading.value = false;
  }
});

onUnmounted(() => {
  if (sessionTimeoutInterval) {
    clearInterval(sessionTimeoutInterval);
  }
});
</script>

<style lang="scss">
// Global styles for the administrative application
.admin-app {
  min-height: 100vh;
  font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
  background-color: #f8f9fa;
}

.loading-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(255, 255, 255, 0.9);
  z-index: 9999;
}

// Zeus Academia Admin Theme
:root {
  --zeus-primary: #2c3e50;
  --zeus-secondary: #34495e;
  --zeus-success: #27ae60;
  --zeus-danger: #e74c3c;
  --zeus-warning: #f39c12;
  --zeus-info: #3498db;
  --zeus-light: #ecf0f1;
  --zeus-dark: #2c3e50;

  --zeus-gradient-primary: linear-gradient(135deg, #2c3e50 0%, #34495e 100%);
  --zeus-gradient-secondary: linear-gradient(135deg, #3498db 0%, #2980b9 100%);

  --zeus-border-radius: 0.375rem;
  --zeus-box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075);
  --zeus-box-shadow-lg: 0 0.5rem 1rem rgba(0, 0, 0, 0.15);
}

// Bootstrap theme overrides
.btn-primary {
  background: var(--zeus-gradient-primary);
  border-color: var(--zeus-primary);

  &:hover {
    background: var(--zeus-secondary);
    border-color: var(--zeus-secondary);
    transform: translateY(-1px);
    box-shadow: var(--zeus-box-shadow-lg);
  }
}

.navbar-dark .navbar-brand {
  color: white;
  font-weight: 600;
}

.card {
  border: none;
  box-shadow: var(--zeus-box-shadow);
  border-radius: var(--zeus-border-radius);

  &:hover {
    box-shadow: var(--zeus-box-shadow-lg);
    transform: translateY(-2px);
  }
}

// Data grid styles
.admin-data-grid {
  .table {
    margin-bottom: 0;

    th {
      background-color: var(--zeus-light);
      border-bottom: 2px solid var(--zeus-primary);
      font-weight: 600;
      color: var(--zeus-dark);
    }

    tbody tr {
      transition: background-color 0.15s ease-in-out;

      &:hover {
        background-color: rgba(52, 73, 94, 0.05);
      }

      &.selected {
        background-color: rgba(52, 152, 219, 0.1);
      }
    }
  }
}

// Status badges
.badge {
  &.status-active {
    background-color: var(--zeus-success);
  }

  &.status-inactive {
    background-color: var(--zeus-warning);
  }

  &.status-blocked {
    background-color: var(--zeus-danger);
  }

  &.status-pending {
    background-color: var(--zeus-info);
  }
}

// Security level indicators
.security-level {
  &.basic {
    color: var(--zeus-success);
  }

  &.elevated {
    color: var(--zeus-warning);
  }

  &.critical {
    color: var(--zeus-danger);
  }
}

// Animation utilities
.fade-in {
  animation: fadeIn 0.3s ease-in-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.slide-in {
  animation: slideIn 0.3s ease-in-out;
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateX(-20px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

// Responsive utilities
@media (max-width: 768px) {
  .admin-app {
    .card {
      margin-bottom: 1rem;
    }
  }
}

// Print styles
@media print {
  .admin-app {
    .no-print {
      display: none !important;
    }

    .card {
      box-shadow: none;
      border: 1px solid #dee2e6;
    }
  }
}
</style>
