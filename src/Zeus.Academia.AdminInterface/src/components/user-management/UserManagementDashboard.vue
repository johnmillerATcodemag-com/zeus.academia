<template>
  <div class="user-management-dashboard">
    <!-- Header with Navigation -->
    <div class="dashboard-header mb-4">
      <div class="d-flex justify-content-between align-items-center">
        <div>
          <h1 class="mb-2">User Management Dashboard</h1>
          <nav aria-label="breadcrumb">
            <ol class="breadcrumb">
              <li class="breadcrumb-item">
                <router-link to="/admin">Administration</router-link>
              </li>
              <li class="breadcrumb-item active">User Management</li>
            </ol>
          </nav>
        </div>
        <div class="dashboard-actions">
          <button
            class="btn btn-outline-primary me-2"
            @click="refreshAllData"
            :disabled="loading"
          >
            <i class="bi bi-arrow-clockwise"></i>
            Refresh All
          </button>
          <div class="dropdown">
            <button
              class="btn btn-primary dropdown-toggle"
              type="button"
              data-bs-toggle="dropdown"
            >
              <i class="bi bi-plus-circle"></i>
              Quick Actions
            </button>
            <ul class="dropdown-menu">
              <li>
                <a class="dropdown-item" href="#" @click="showQuickCreateUser">
                  <i class="bi bi-person-plus me-2"></i>
                  Create New User
                </a>
              </li>
              <li>
                <a
                  class="dropdown-item"
                  href="#"
                  @click="setActiveView('bulk-management')"
                >
                  <i class="bi bi-people me-2"></i>
                  Bulk Operations
                </a>
              </li>
              <li>
                <a class="dropdown-item" href="#" @click="showEmergencyReset">
                  <i class="bi bi-shield-exclamation me-2"></i>
                  Emergency Reset
                </a>
              </li>
              <li><hr class="dropdown-divider" /></li>
              <li>
                <a class="dropdown-item" href="#" @click="exportUserReport">
                  <i class="bi bi-download me-2"></i>
                  Export User Report
                </a>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </div>

    <!-- Key Metrics Row -->
    <div class="row mb-4">
      <div class="col-md-2">
        <div class="metric-card card bg-primary text-white">
          <div class="card-body">
            <div class="d-flex justify-content-between align-items-center">
              <div>
                <h3 class="mb-0">{{ metrics.totalUsers.toLocaleString() }}</h3>
                <small>Total Users</small>
              </div>
              <i class="bi bi-people display-6"></i>
            </div>
            <div class="progress mt-2" style="height: 4px">
              <div
                class="progress-bar bg-white"
                :style="{
                  width: `${Math.min(
                    100,
                    (metrics.totalUsers / metrics.userLimit) * 100
                  )}%`,
                }"
              ></div>
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-2">
        <div class="metric-card card bg-success text-white">
          <div class="card-body">
            <div class="d-flex justify-content-between align-items-center">
              <div>
                <h3 class="mb-0">{{ metrics.activeUsers }}</h3>
                <small>Active Users</small>
              </div>
              <i class="bi bi-person-check display-6"></i>
            </div>
            <div class="mt-1">
              <small
                >{{
                  ((metrics.activeUsers / metrics.totalUsers) * 100).toFixed(1)
                }}% of total</small
              >
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-2">
        <div class="metric-card card bg-warning text-white">
          <div class="card-body">
            <div class="d-flex justify-content-between align-items-center">
              <div>
                <h3 class="mb-0">{{ metrics.pendingApprovals }}</h3>
                <small>Pending</small>
              </div>
              <i class="bi bi-clock display-6"></i>
            </div>
            <div class="mt-1">
              <small>Requires attention</small>
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-2">
        <div class="metric-card card bg-danger text-white">
          <div class="card-body">
            <div class="d-flex justify-content-between align-items-center">
              <div>
                <h3 class="mb-0">{{ metrics.securityAlerts }}</h3>
                <small>Security Alerts</small>
              </div>
              <i class="bi bi-shield-exclamation display-6"></i>
            </div>
            <div class="mt-1">
              <small>Critical: {{ metrics.criticalAlerts }}</small>
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-2">
        <div class="metric-card card bg-info text-white">
          <div class="card-body">
            <div class="d-flex justify-content-between align-items-center">
              <div>
                <h3 class="mb-0">{{ metrics.mfaEnabled }}</h3>
                <small>MFA Enabled</small>
              </div>
              <i class="bi bi-shield-check display-6"></i>
            </div>
            <div class="mt-1">
              <small
                >{{
                  ((metrics.mfaEnabled / metrics.totalUsers) * 100).toFixed(1)
                }}% coverage</small
              >
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-2">
        <div class="metric-card card bg-secondary text-white">
          <div class="card-body">
            <div class="d-flex justify-content-between align-items-center">
              <div>
                <h3 class="mb-0">{{ metrics.recentActivity }}</h3>
                <small>Recent Activity</small>
              </div>
              <i class="bi bi-activity display-6"></i>
            </div>
            <div class="mt-1">
              <small>Past 24 hours</small>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Priority Alerts -->
    <div v-if="priorityAlerts.length > 0" class="alert alert-warning mb-4">
      <h5 class="alert-heading">
        <i class="bi bi-exclamation-triangle me-2"></i>
        Priority Alerts
      </h5>
      <div class="row">
        <div class="col-md-8">
          <ul class="mb-0">
            <li v-for="alert in priorityAlerts" :key="alert.id">
              {{ alert.message }}
              <button
                class="btn btn-sm btn-outline-dark ms-2"
                @click="handleAlert(alert)"
              >
                {{ alert.actionText }}
              </button>
            </li>
          </ul>
        </div>
        <div class="col-md-4 text-end">
          <button
            class="btn btn-sm btn-outline-danger"
            @click="dismissAllAlerts"
          >
            Dismiss All
          </button>
        </div>
      </div>
    </div>

    <!-- Navigation Tabs -->
    <div class="dashboard-navigation mb-4">
      <ul class="nav nav-tabs nav-fill">
        <li class="nav-item">
          <button
            class="nav-link"
            :class="{ active: activeView === 'overview' }"
            @click="setActiveView('overview')"
          >
            <i class="bi bi-house me-2"></i>
            Overview
          </button>
        </li>
        <li class="nav-item">
          <button
            class="nav-link"
            :class="{ active: activeView === 'bulk-management' }"
            @click="setActiveView('bulk-management')"
          >
            <i class="bi bi-people me-2"></i>
            Bulk Management
          </button>
        </li>
        <li class="nav-item">
          <button
            class="nav-link"
            :class="{ active: activeView === 'role-assignment' }"
            @click="setActiveView('role-assignment')"
          >
            <i class="bi bi-person-badge me-2"></i>
            Role Assignment
          </button>
        </li>
        <li class="nav-item">
          <button
            class="nav-link"
            :class="{ active: activeView === 'lifecycle-management' }"
            @click="setActiveView('lifecycle-management')"
          >
            <i class="bi bi-arrow-repeat me-2"></i>
            User Lifecycle
          </button>
        </li>
        <li class="nav-item">
          <button
            class="nav-link"
            :class="{ active: activeView === 'password-security' }"
            @click="setActiveView('password-security')"
          >
            <i class="bi bi-shield-lock me-2"></i>
            Password Security
          </button>
        </li>
        <li class="nav-item">
          <button
            class="nav-link"
            :class="{ active: activeView === 'audit-trail' }"
            @click="setActiveView('audit-trail')"
          >
            <i class="bi bi-list-check me-2"></i>
            Audit Trail
          </button>
        </li>
      </ul>
    </div>

    <!-- Overview Dashboard -->
    <div v-if="activeView === 'overview'" class="overview-section">
      <!-- Recent Activity Timeline -->
      <div class="row mb-4">
        <div class="col-md-8">
          <div class="card">
            <div class="card-header">
              <h5 class="mb-0">
                <i class="bi bi-clock-history me-2"></i>
                Recent Activity Timeline
              </h5>
            </div>
            <div class="card-body">
              <div class="timeline">
                <div
                  v-for="activity in recentActivities"
                  :key="activity.id"
                  class="timeline-item"
                >
                  <div
                    class="timeline-marker"
                    :class="getActivityMarkerClass(activity.type)"
                  >
                    <i :class="getActivityIcon(activity.type)"></i>
                  </div>
                  <div class="timeline-content">
                    <div class="timeline-header">
                      <strong>{{ activity.title }}</strong>
                      <small class="text-muted ms-2">{{
                        formatRelativeTime(activity.timestamp)
                      }}</small>
                    </div>
                    <div class="timeline-description">
                      {{ activity.description }}
                    </div>
                    <div v-if="activity.user" class="timeline-user">
                      <small class="text-muted"
                        >by {{ activity.user.name }}</small
                      >
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div class="card-footer">
              <button
                class="btn btn-sm btn-outline-primary"
                @click="setActiveView('audit-trail')"
              >
                View Full Audit Trail
              </button>
            </div>
          </div>
        </div>
        <div class="col-md-4">
          <!-- Quick Stats -->
          <div class="card mb-3">
            <div class="card-header">
              <h5 class="mb-0">
                <i class="bi bi-bar-chart me-2"></i>
                Quick Statistics
              </h5>
            </div>
            <div class="card-body">
              <div class="stat-item mb-3">
                <div class="d-flex justify-content-between">
                  <span>New Users (7 days)</span>
                  <strong>{{ quickStats.newUsers7Days }}</strong>
                </div>
                <div class="progress" style="height: 6px">
                  <div class="progress-bar bg-success" style="width: 75%"></div>
                </div>
              </div>
              <div class="stat-item mb-3">
                <div class="d-flex justify-content-between">
                  <span>Password Resets (7 days)</span>
                  <strong>{{ quickStats.passwordResets7Days }}</strong>
                </div>
                <div class="progress" style="height: 6px">
                  <div class="progress-bar bg-warning" style="width: 45%"></div>
                </div>
              </div>
              <div class="stat-item mb-3">
                <div class="d-flex justify-content-between">
                  <span>Failed Logins (24h)</span>
                  <strong>{{ quickStats.failedLogins24h }}</strong>
                </div>
                <div class="progress" style="height: 6px">
                  <div class="progress-bar bg-danger" style="width: 20%"></div>
                </div>
              </div>
              <div class="stat-item">
                <div class="d-flex justify-content-between">
                  <span>Average Session Duration</span>
                  <strong>{{ quickStats.avgSessionDuration }}</strong>
                </div>
              </div>
            </div>
          </div>

          <!-- Quick Actions -->
          <div class="card">
            <div class="card-header">
              <h5 class="mb-0">
                <i class="bi bi-lightning me-2"></i>
                Quick Actions
              </h5>
            </div>
            <div class="card-body">
              <div class="d-grid gap-2">
                <button
                  class="btn btn-outline-primary btn-sm"
                  @click="showQuickCreateUser"
                >
                  <i class="bi bi-person-plus me-2"></i>
                  Create User
                </button>
                <button
                  class="btn btn-outline-warning btn-sm"
                  @click="setActiveView('bulk-management')"
                >
                  <i class="bi bi-upload me-2"></i>
                  Bulk Import
                </button>
                <button
                  class="btn btn-outline-danger btn-sm"
                  @click="showEmergencyReset"
                >
                  <i class="bi bi-shield-exclamation me-2"></i>
                  Emergency Actions
                </button>
                <button
                  class="btn btn-outline-info btn-sm"
                  @click="exportUserReport"
                >
                  <i class="bi bi-download me-2"></i>
                  Export Report
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- User Distribution Charts -->
      <div class="row">
        <div class="col-md-6">
          <div class="card">
            <div class="card-header">
              <h5 class="mb-0">User Status Distribution</h5>
            </div>
            <div class="card-body">
              <div class="chart-placeholder">
                <div class="row text-center">
                  <div class="col">
                    <div
                      class="chart-segment bg-success"
                      style="height: 100px; border-radius: 8px"
                    >
                      <div class="chart-label text-white pt-3">
                        <h4>{{ userDistribution.active }}</h4>
                        <small>Active</small>
                      </div>
                    </div>
                  </div>
                  <div class="col">
                    <div
                      class="chart-segment bg-warning"
                      style="height: 80px; border-radius: 8px"
                    >
                      <div class="chart-label text-white pt-3">
                        <h5>{{ userDistribution.suspended }}</h5>
                        <small>Suspended</small>
                      </div>
                    </div>
                  </div>
                  <div class="col">
                    <div
                      class="chart-segment bg-secondary"
                      style="height: 60px; border-radius: 8px"
                    >
                      <div class="chart-label text-white pt-2">
                        <h6>{{ userDistribution.inactive }}</h6>
                        <small>Inactive</small>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="col-md-6">
          <div class="card">
            <div class="card-header">
              <h5 class="mb-0">Role Distribution</h5>
            </div>
            <div class="card-body">
              <div class="role-distribution">
                <div
                  v-for="role in roleDistribution"
                  :key="role.name"
                  class="role-item mb-2"
                >
                  <div
                    class="d-flex justify-content-between align-items-center"
                  >
                    <div>
                      <span class="badge" :class="getRoleBadgeClass(role.name)">
                        {{ role.name }}
                      </span>
                      <span class="ms-2">{{ role.count }} users</span>
                    </div>
                    <div style="width: 100px">
                      <div class="progress" style="height: 8px">
                        <div
                          class="progress-bar"
                          :class="getRoleProgressClass(role.name)"
                          :style="{
                            width: `${
                              (role.count / metrics.totalUsers) * 100
                            }%`,
                          }"
                        ></div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Component Views -->
    <div v-else class="component-view">
      <BulkUserManagement v-if="activeView === 'bulk-management'" />
      <RoleAssignmentInterface v-if="activeView === 'role-assignment'" />
      <UserLifecycleManagement v-if="activeView === 'lifecycle-management'" />
      <PasswordResetSecurity v-if="activeView === 'password-security'" />
      <AuditTrailManagement v-if="activeView === 'audit-trail'" />
    </div>

    <!-- Loading Overlay -->
    <div v-if="loading" class="loading-overlay">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from "vue";
import { AdminApiService } from "@/services/AdminApiService";
import BulkUserManagement from "./BulkUserManagement.vue";
import RoleAssignmentInterface from "./RoleAssignmentInterface.vue";
import UserLifecycleManagement from "./UserLifecycleManagement.vue";
import PasswordResetSecurity from "./PasswordResetSecurity.vue";
import AuditTrailManagement from "./AuditTrailManagement.vue";

// Reactive state
const loading = ref(false);
const activeView = ref("overview");

// Metrics data
const metrics = ref({
  totalUsers: 0,
  activeUsers: 0,
  pendingApprovals: 0,
  securityAlerts: 0,
  criticalAlerts: 0,
  mfaEnabled: 0,
  recentActivity: 0,
  userLimit: 10000,
});

// Priority alerts
const priorityAlerts = ref([
  {
    id: 1,
    message: "15 users have passwords expiring in the next 7 days",
    actionText: "Review Users",
    action: "password_expiry",
    severity: "warning",
  },
  {
    id: 2,
    message: "3 new user registrations pending approval",
    actionText: "Review Pending",
    action: "pending_approvals",
    severity: "info",
  },
  {
    id: 3,
    message: "2 failed login attempts detected from suspicious IPs",
    actionText: "Review Security",
    action: "security_review",
    severity: "high",
  },
]);

// Recent activities
const recentActivities = ref([
  {
    id: 1,
    type: "user_created",
    title: "New User Created",
    description: "John Smith was added to the system with Student role",
    timestamp: new Date(Date.now() - 5 * 60000),
    user: { name: "Admin User" },
  },
  {
    id: 2,
    type: "password_reset",
    title: "Password Reset",
    description: "Password reset completed for jane.doe@university.edu",
    timestamp: new Date(Date.now() - 15 * 60000),
    user: { name: "Admin User" },
  },
  {
    id: 3,
    type: "role_assigned",
    title: "Role Assignment",
    description: "Faculty role assigned to 5 users in bulk operation",
    timestamp: new Date(Date.now() - 30 * 60000),
    user: { name: "Admin User" },
  },
  {
    id: 4,
    type: "security_alert",
    title: "Security Alert",
    description:
      "Suspicious login pattern detected for user mike.wilson@university.edu",
    timestamp: new Date(Date.now() - 45 * 60000),
    user: null,
  },
]);

// Quick stats
const quickStats = ref({
  newUsers7Days: 23,
  passwordResets7Days: 8,
  failedLogins24h: 12,
  avgSessionDuration: "24m",
});

// User distribution
const userDistribution = ref({
  active: 0,
  suspended: 0,
  inactive: 0,
});

// Role distribution
const roleDistribution = ref([
  { name: "Student", count: 0 },
  { name: "Faculty", count: 0 },
  { name: "Administrator", count: 0 },
  { name: "Staff", count: 0 },
]);

// Auto-refresh interval
let refreshInterval: NodeJS.Timeout | null = null;

// Methods
const loadDashboardData = async () => {
  loading.value = true;
  try {
    // Load all necessary data
    const [usersResponse, auditResponse] = await Promise.all([
      AdminApiService.users.getAll(),
      AdminApiService.audit.getTrail({ limit: 50 }),
    ]);

    const users = usersResponse.data?.data || [];

    // Calculate metrics
    metrics.value = {
      totalUsers: users.length,
      activeUsers: users.filter((u: any) => u.status === "active").length,
      pendingApprovals: users.filter((u: any) => u.status === "pending").length,
      securityAlerts: 5, // This would come from security service
      criticalAlerts: 2,
      mfaEnabled: users.filter((u: any) => u.mfaEnabled).length,
      recentActivity: auditResponse.data?.length || 0,
      userLimit: 10000,
    };

    // Calculate user distribution
    userDistribution.value = {
      active: users.filter((u: any) => u.status === "active").length,
      suspended: users.filter((u: any) => u.status === "suspended").length,
      inactive: users.filter((u: any) => u.status === "inactive").length,
    };

    // Calculate role distribution
    const roleCounts = users.reduce((acc: any, user: any) => {
      const role = user.roles?.[0] || "Student";
      acc[role] = (acc[role] || 0) + 1;
      return acc;
    }, {});

    roleDistribution.value = [
      { name: "Student", count: roleCounts.Student || 0 },
      { name: "Faculty", count: roleCounts.Faculty || 0 },
      { name: "Administrator", count: roleCounts.Administrator || 0 },
      { name: "Staff", count: roleCounts.Staff || 0 },
    ];
  } catch (error) {
    console.error("Failed to load dashboard data:", error);
  } finally {
    loading.value = false;
  }
};

const setActiveView = (view: string) => {
  activeView.value = view;
};

const refreshAllData = async () => {
  await loadDashboardData();
};

const handleAlert = (alert: any) => {
  switch (alert.action) {
    case "password_expiry":
      setActiveView("password-security");
      break;
    case "pending_approvals":
      setActiveView("lifecycle-management");
      break;
    case "security_review":
      setActiveView("audit-trail");
      break;
  }
};

const dismissAllAlerts = () => {
  priorityAlerts.value = [];
};

const showQuickCreateUser = () => {
  // This would open a quick create user modal
  console.log("Show quick create user modal");
};

const showEmergencyReset = () => {
  // This would open emergency reset modal
  console.log("Show emergency reset modal");
};

const exportUserReport = async () => {
  try {
    // This would generate and download a comprehensive user report
    console.log("Export user report");
  } catch (error) {
    console.error("Failed to export user report:", error);
  }
};

// Utility functions
const getActivityMarkerClass = (type: string) => {
  const classes = {
    user_created: "bg-success",
    password_reset: "bg-warning",
    role_assigned: "bg-info",
    security_alert: "bg-danger",
    user_suspended: "bg-secondary",
  };
  return classes[type as keyof typeof classes] || "bg-primary";
};

const getActivityIcon = (type: string) => {
  const icons = {
    user_created: "bi-person-plus",
    password_reset: "bi-key",
    role_assigned: "bi-person-badge",
    security_alert: "bi-shield-exclamation",
    user_suspended: "bi-pause-circle",
  };
  return icons[type as keyof typeof icons] || "bi-circle";
};

const getRoleBadgeClass = (role: string) => {
  const classes = {
    Student: "bg-primary",
    Faculty: "bg-success",
    Administrator: "bg-danger",
    Staff: "bg-warning",
  };
  return classes[role as keyof typeof classes] || "bg-secondary";
};

const getRoleProgressClass = (role: string) => {
  const classes = {
    Student: "bg-primary",
    Faculty: "bg-success",
    Administrator: "bg-danger",
    Staff: "bg-warning",
  };
  return classes[role as keyof typeof classes] || "bg-secondary";
};

const formatRelativeTime = (date: Date) => {
  const now = new Date();
  const diffInMinutes = Math.floor((now.getTime() - date.getTime()) / 60000);

  if (diffInMinutes < 1) return "Just now";
  if (diffInMinutes < 60) return `${diffInMinutes}m ago`;
  if (diffInMinutes < 1440) return `${Math.floor(diffInMinutes / 60)}h ago`;
  return `${Math.floor(diffInMinutes / 1440)}d ago`;
};

// Auto-refresh functionality
const startAutoRefresh = () => {
  refreshInterval = setInterval(async () => {
    await loadDashboardData();
  }, 60000); // Refresh every minute
};

const stopAutoRefresh = () => {
  if (refreshInterval) {
    clearInterval(refreshInterval);
    refreshInterval = null;
  }
};

// Lifecycle hooks
onMounted(async () => {
  await loadDashboardData();
  startAutoRefresh();
});

onUnmounted(() => {
  stopAutoRefresh();
});
</script>

<style scoped>
.user-management-dashboard {
  padding: 1rem;
  min-height: 100vh;
  background-color: #f8f9fa;
}

.dashboard-header {
  background: white;
  border-radius: 8px;
  padding: 1.5rem;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.metric-card {
  transition: transform 0.2s, box-shadow 0.2s;
  border: none;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.metric-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
}

.dashboard-navigation .nav-tabs {
  background: white;
  border-radius: 8px;
  padding: 0.5rem;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.dashboard-navigation .nav-link {
  border: none;
  border-radius: 6px;
  margin: 0 0.25rem;
  transition: all 0.2s;
}

.dashboard-navigation .nav-link:hover {
  background-color: #e9ecef;
}

.dashboard-navigation .nav-link.active {
  background-color: #007bff;
  color: white;
}

.timeline {
  position: relative;
}

.timeline::before {
  content: "";
  position: absolute;
  left: 20px;
  top: 0;
  bottom: 0;
  width: 2px;
  background: #dee2e6;
}

.timeline-item {
  position: relative;
  padding-left: 50px;
  margin-bottom: 1.5rem;
}

.timeline-marker {
  position: absolute;
  left: 10px;
  top: 5px;
  width: 20px;
  height: 20px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 0.8rem;
}

.timeline-content {
  background: #f8f9fa;
  padding: 1rem;
  border-radius: 8px;
  border-left: 3px solid #007bff;
}

.timeline-header {
  display: flex;
  justify-content: between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.chart-placeholder {
  padding: 1rem;
}

.chart-segment {
  margin-bottom: 1rem;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: transform 0.2s;
}

.chart-segment:hover {
  transform: scale(1.05);
}

.role-distribution .role-item {
  padding: 0.5rem;
  background: #f8f9fa;
  border-radius: 6px;
  transition: background-color 0.2s;
}

.role-distribution .role-item:hover {
  background: #e9ecef;
}

.stat-item {
  padding: 0.75rem;
  background: #f8f9fa;
  border-radius: 6px;
  transition: background-color 0.2s;
}

.stat-item:hover {
  background: #e9ecef;
}

.loading-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(255, 255, 255, 0.8);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 9999;
}

.component-view {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  padding: 1rem;
}

@media (max-width: 768px) {
  .dashboard-header {
    padding: 1rem;
  }

  .dashboard-actions {
    margin-top: 1rem;
  }

  .metric-card {
    margin-bottom: 1rem;
  }

  .dashboard-navigation .nav-tabs {
    flex-direction: column;
  }

  .timeline-item {
    padding-left: 30px;
  }

  .timeline-marker {
    left: 5px;
    width: 15px;
    height: 15px;
  }

  .timeline::before {
    left: 12px;
  }
}

@media (max-width: 576px) {
  .user-management-dashboard {
    padding: 0.5rem;
  }

  .metric-card .card-body {
    padding: 1rem 0.75rem;
  }

  .metric-card h3 {
    font-size: 1.5rem;
  }

  .chart-segment {
    height: 60px !important;
  }
}
</style>
