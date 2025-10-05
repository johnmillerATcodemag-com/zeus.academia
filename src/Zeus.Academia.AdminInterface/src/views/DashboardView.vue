<template>
  <div class="dashboard-layout">
    <!-- Navigation Bar -->
    <nav class="navbar navbar-expand-lg navbar-dark bg-zeus-primary">
      <div class="container-fluid">
        <span class="navbar-brand fw-bold">Zeus Academia - Admin</span>

        <div class="navbar-nav ms-auto">
          <div class="nav-item dropdown">
            <a
              class="nav-link dropdown-toggle d-flex align-items-center"
              href="#"
              role="button"
              data-bs-toggle="dropdown"
            >
              <div class="user-avatar me-2">
                {{ userInitials }}
              </div>
              {{ authStore.userFullName }}
            </a>
            <ul class="dropdown-menu dropdown-menu-end">
              <li>
                <router-link class="dropdown-item" to="/profile">
                  <i class="bi bi-person-circle me-2"></i>Profile
                </router-link>
              </li>
              <li><hr class="dropdown-divider" /></li>
              <li>
                <a class="dropdown-item" href="#" @click="handleLogout">
                  <i class="bi bi-box-arrow-right me-2"></i>Logout
                </a>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </nav>

    <!-- Main Content -->
    <div class="container-fluid mt-4">
      <!-- Welcome Section -->
      <div class="row mb-4">
        <div class="col-12">
          <div class="card">
            <div class="card-body">
              <h1 class="card-title mb-3">
                Welcome, {{ authStore.userFullName }}
              </h1>
              <p class="card-text text-muted">
                Role:
                <span class="badge bg-primary">{{ roleDisplayName }}</span>
                Security Level:
                <span
                  :class="'badge security-level ' + authStore.securityLevel"
                  >{{ authStore.securityLevel.toUpperCase() }}</span
                >
              </p>
              <p class="small text-muted">
                Last Login: {{ formatDate(new Date()) }}
              </p>
            </div>
          </div>
        </div>
      </div>

      <!-- Role-Based Dashboard Widgets -->
      <div class="row">
        <!-- System Admin Widgets -->
        <template v-if="authStore.isSystemAdmin">
          <div class="col-lg-3 col-md-6 mb-4">
            <div class="card text-white bg-primary">
              <div class="card-body">
                <div class="d-flex justify-content-between">
                  <div>
                    <h5 class="card-title">Total Users</h5>
                    <h2 class="mb-0">{{ systemMetrics.totalUsers || 0 }}</h2>
                  </div>
                  <div class="card-icon">
                    <i class="bi bi-people-fill"></i>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="col-lg-3 col-md-6 mb-4">
            <div class="card text-white bg-success">
              <div class="card-body">
                <div class="d-flex justify-content-between">
                  <div>
                    <h5 class="card-title">System Health</h5>
                    <h2 class="mb-0">{{ systemHealthStatus }}</h2>
                  </div>
                  <div class="card-icon">
                    <i class="bi bi-heart-pulse-fill"></i>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="col-lg-3 col-md-6 mb-4">
            <div class="card text-white bg-warning">
              <div class="card-body">
                <div class="d-flex justify-content-between">
                  <div>
                    <h5 class="card-title">Active Sessions</h5>
                    <h2 class="mb-0">
                      {{ systemMetrics.activeSessions || 0 }}
                    </h2>
                  </div>
                  <div class="card-icon">
                    <i class="bi bi-activity"></i>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="col-lg-3 col-md-6 mb-4">
            <div class="card text-white bg-danger">
              <div class="card-body">
                <div class="d-flex justify-content-between">
                  <div>
                    <h5 class="card-title">Critical Alerts</h5>
                    <h2 class="mb-0">
                      {{ systemMetrics.criticalAlerts || 0 }}
                    </h2>
                  </div>
                  <div class="card-icon">
                    <i class="bi bi-exclamation-triangle-fill"></i>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </template>

        <!-- Registrar Widgets -->
        <template v-if="authStore.isRegistrar">
          <div class="col-lg-3 col-md-6 mb-4">
            <div class="card text-white bg-info">
              <div class="card-body">
                <div class="d-flex justify-content-between">
                  <div>
                    <h5 class="card-title">Total Enrollments</h5>
                    <h2 class="mb-0">
                      {{ academicMetrics.totalEnrollments || 0 }}
                    </h2>
                  </div>
                  <div class="card-icon">
                    <i class="bi bi-journal-bookmark-fill"></i>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="col-lg-3 col-md-6 mb-4">
            <div class="card text-white bg-success">
              <div class="card-body">
                <div class="d-flex justify-content-between">
                  <div>
                    <h5 class="card-title">Active Courses</h5>
                    <h2 class="mb-0">
                      {{ academicMetrics.activeCourses || 0 }}
                    </h2>
                  </div>
                  <div class="card-icon">
                    <i class="bi bi-book-fill"></i>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </template>

        <!-- Academic Admin Widgets -->
        <template v-if="authStore.isAcademicAdmin">
          <div class="col-lg-3 col-md-6 mb-4">
            <div class="card text-white bg-primary">
              <div class="card-body">
                <div class="d-flex justify-content-between">
                  <div>
                    <h5 class="card-title">Faculty Members</h5>
                    <h2 class="mb-0">
                      {{ academicMetrics.facultyCount || 0 }}
                    </h2>
                  </div>
                  <div class="card-icon">
                    <i class="bi bi-mortarboard-fill"></i>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="col-lg-3 col-md-6 mb-4">
            <div class="card text-white bg-info">
              <div class="card-body">
                <div class="d-flex justify-content-between">
                  <div>
                    <h5 class="card-title">Departments</h5>
                    <h2 class="mb-0">
                      {{ academicMetrics.departmentCount || 0 }}
                    </h2>
                  </div>
                  <div class="card-icon">
                    <i class="bi bi-building"></i>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </template>
      </div>

      <!-- Quick Actions -->
      <div class="row">
        <div class="col-12">
          <div class="card">
            <div class="card-header">
              <h5 class="card-title mb-0">Quick Actions</h5>
            </div>
            <div class="card-body">
              <div class="row">
                <div
                  v-if="authStore.hasPermission('user_management')"
                  class="col-lg-2 col-md-4 col-sm-6 mb-3"
                >
                  <router-link
                    to="/users"
                    class="btn btn-outline-primary w-100 h-100 d-flex flex-column justify-content-center align-items-center py-3"
                  >
                    <i class="bi bi-people fs-1 mb-2"></i>
                    <span>Manage Users</span>
                  </router-link>
                </div>

                <div
                  v-if="authStore.hasPermission('system_configuration')"
                  class="col-lg-2 col-md-4 col-sm-6 mb-3"
                >
                  <router-link
                    to="/system"
                    class="btn btn-outline-secondary w-100 h-100 d-flex flex-column justify-content-center align-items-center py-3"
                  >
                    <i class="bi bi-gear fs-1 mb-2"></i>
                    <span>System Config</span>
                  </router-link>
                </div>

                <div
                  v-if="authStore.hasPermission('academic_calendar')"
                  class="col-lg-2 col-md-4 col-sm-6 mb-3"
                >
                  <router-link
                    to="/academic"
                    class="btn btn-outline-success w-100 h-100 d-flex flex-column justify-content-center align-items-center py-3"
                  >
                    <i class="bi bi-calendar-event fs-1 mb-2"></i>
                    <span>Academic</span>
                  </router-link>
                </div>

                <div
                  v-if="authStore.hasPermission('financial_management')"
                  class="col-lg-2 col-md-4 col-sm-6 mb-3"
                >
                  <router-link
                    to="/financial"
                    class="btn btn-outline-warning w-100 h-100 d-flex flex-column justify-content-center align-items-center py-3"
                  >
                    <i class="bi bi-currency-dollar fs-1 mb-2"></i>
                    <span>Financial</span>
                  </router-link>
                </div>

                <div
                  v-if="authStore.hasPermission('system_monitoring')"
                  class="col-lg-2 col-md-4 col-sm-6 mb-3"
                >
                  <router-link
                    to="/monitoring"
                    class="btn btn-outline-info w-100 h-100 d-flex flex-column justify-content-center align-items-center py-3"
                  >
                    <i class="bi bi-graph-up fs-1 mb-2"></i>
                    <span>Monitoring</span>
                  </router-link>
                </div>

                <div
                  v-if="authStore.hasPermission('audit_access')"
                  class="col-lg-2 col-md-4 col-sm-6 mb-3"
                >
                  <router-link
                    to="/audit"
                    class="btn btn-outline-danger w-100 h-100 d-flex flex-column justify-content-center align-items-center py-3"
                  >
                    <i class="bi bi-shield-check fs-1 mb-2"></i>
                    <span>Audit Logs</span>
                  </router-link>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "@/stores/auth";
import { useDataStore } from "@/stores/data";

const router = useRouter();
const authStore = useAuthStore();
const dataStore = useDataStore();

// Mock metrics - in real implementation, these would come from the data store
const systemMetrics = ref({
  totalUsers: 1250,
  activeSessions: 42,
  criticalAlerts: 2,
});

const academicMetrics = ref({
  totalEnrollments: 3450,
  activeCourses: 120,
  facultyCount: 85,
  departmentCount: 12,
});

const userInitials = computed(() => {
  const user = authStore.user;
  if (!user) return "?";
  return `${user.firstName.charAt(0)}${user.lastName.charAt(0)}`.toUpperCase();
});

const roleDisplayName = computed(() => {
  const roleNames = {
    system_admin: "System Administrator",
    registrar: "Registrar",
    academic_admin: "Academic Administrator",
  };
  return roleNames[authStore.userRole || "academic_admin"] || "Administrator";
});

const systemHealthStatus = computed(() => {
  const status = dataStore.systemHealthStatus;
  const statusMap = {
    healthy: "Good",
    warning: "Warning",
    critical: "Critical",
    unknown: "Unknown",
  };
  return statusMap[status] || "Unknown";
});

const formatDate = (date: Date): string => {
  return date.toLocaleDateString("en-US", {
    year: "numeric",
    month: "long",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
};

const handleLogout = async () => {
  try {
    await authStore.logout();
    await router.push("/login");
  } catch (error) {
    console.error("Logout failed:", error);
  }
};

onMounted(async () => {
  // Load initial dashboard data
  try {
    if (authStore.hasPermission("system_monitoring")) {
      await dataStore.refreshSystemMetrics();
    }
  } catch (error) {
    console.error("Failed to load dashboard data:", error);
  }
});
</script>

<style lang="scss" scoped>
.dashboard-layout {
  min-height: 100vh;
  background-color: #f8f9fa;
}

.bg-zeus-primary {
  background: var(--zeus-gradient-primary) !important;
}

.user-avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.2);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.875rem;
  font-weight: 600;
}

.card-icon {
  font-size: 2.5rem;
  opacity: 0.8;
}

.card {
  transition: all 0.3s ease;
  border: none;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);

  &:hover {
    transform: translateY(-2px);
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
  }
}

.btn-outline-primary,
.btn-outline-secondary,
.btn-outline-success,
.btn-outline-warning,
.btn-outline-info,
.btn-outline-danger {
  min-height: 120px;
  transition: all 0.3s ease;

  &:hover {
    transform: translateY(-2px);
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
  }

  i {
    transition: all 0.3s ease;
  }

  &:hover i {
    transform: scale(1.1);
  }
}

.security-level {
  &.basic {
    background-color: var(--zeus-success) !important;
  }

  &.elevated {
    background-color: var(--zeus-warning) !important;
  }

  &.critical {
    background-color: var(--zeus-danger) !important;
  }
}

// Card color variations
.bg-primary {
  background: linear-gradient(135deg, #007bff 0%, #0056b3 100%) !important;
}
.bg-success {
  background: linear-gradient(135deg, #28a745 0%, #1e7e34 100%) !important;
}
.bg-warning {
  background: linear-gradient(135deg, #ffc107 0%, #d39e00 100%) !important;
}
.bg-danger {
  background: linear-gradient(135deg, #dc3545 0%, #bd2130 100%) !important;
}
.bg-info {
  background: linear-gradient(135deg, #17a2b8 0%, #117a8b 100%) !important;
}

@media (max-width: 768px) {
  .card-body {
    padding: 1rem;

    h2 {
      font-size: 1.5rem;
    }
  }

  .card-icon {
    font-size: 2rem;
  }

  .btn-outline-primary,
  .btn-outline-secondary,
  .btn-outline-success,
  .btn-outline-warning,
  .btn-outline-info,
  .btn-outline-danger {
    min-height: 100px;

    i {
      font-size: 2rem !important;
    }
  }
}
</style>
