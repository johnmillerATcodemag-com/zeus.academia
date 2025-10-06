<template>
  <div class="password-reset-security">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2>Password Reset & Security Tools</h2>
      <div class="action-buttons">
        <button
          class="btn btn-outline-warning me-2"
          @click="showBulkResetModal = true"
        >
          <i class="bi bi-shield-exclamation"></i>
          Bulk Reset
        </button>
        <button
          class="btn btn-outline-danger me-2"
          @click="showSecurityIncidentModal = true"
        >
          <i class="bi bi-exclamation-triangle"></i>
          Security Incident
        </button>
        <button
          class="btn btn-primary"
          @click="refreshData"
          :disabled="loading"
        >
          <i class="bi bi-arrow-clockwise"></i>
          Refresh
        </button>
      </div>
    </div>

    <!-- Security Alerts -->
    <div v-if="securityAlerts.length > 0" class="alert alert-warning mb-4">
      <h5 class="alert-heading">
        <i class="bi bi-exclamation-triangle me-2"></i>
        Security Alerts
      </h5>
      <ul class="mb-0">
        <li v-for="alert in securityAlerts" :key="alert.id">
          {{ alert.message }}
          <button
            class="btn btn-sm btn-outline-danger ms-2"
            @click="handleSecurityAlert(alert)"
          >
            Take Action
          </button>
        </li>
      </ul>
    </div>

    <!-- Statistics Cards -->
    <div class="row mb-4">
      <div class="col-md-3">
        <div class="card bg-info text-white">
          <div class="card-body">
            <div class="d-flex justify-content-between">
              <div>
                <h4>{{ stats.pendingResets }}</h4>
                <p class="mb-0">Pending Resets</p>
              </div>
              <i class="bi bi-key display-4"></i>
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card bg-warning text-white">
          <div class="card-body">
            <div class="d-flex justify-content-between">
              <div>
                <h4>{{ stats.expiringSoon }}</h4>
                <p class="mb-0">Expiring Soon</p>
              </div>
              <i class="bi bi-clock display-4"></i>
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card bg-danger text-white">
          <div class="card-body">
            <div class="d-flex justify-content-between">
              <div>
                <h4>{{ stats.compromised }}</h4>
                <p class="mb-0">Compromised</p>
              </div>
              <i class="bi bi-shield-x display-4"></i>
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card bg-success text-white">
          <div class="card-body">
            <div class="d-flex justify-content-between">
              <div>
                <h4>{{ stats.mfaEnabled }}</h4>
                <p class="mb-0">MFA Enabled</p>
              </div>
              <i class="bi bi-shield-check display-4"></i>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters -->
    <div class="card mb-4">
      <div class="card-body">
        <div class="row">
          <div class="col-md-2">
            <label class="form-label">Reset Status</label>
            <select
              v-model="filters.resetStatus"
              class="form-select"
              @change="applyFilters"
            >
              <option value="">All Status</option>
              <option value="pending">Pending</option>
              <option value="completed">Completed</option>
              <option value="expired">Expired</option>
            </select>
          </div>
          <div class="col-md-2">
            <label class="form-label">Security Level</label>
            <select
              v-model="filters.securityLevel"
              class="form-select"
              @change="applyFilters"
            >
              <option value="">All Levels</option>
              <option value="normal">Normal</option>
              <option value="elevated">Elevated</option>
              <option value="high">High</option>
              <option value="critical">Critical</option>
            </select>
          </div>
          <div class="col-md-2">
            <label class="form-label">MFA Status</label>
            <select
              v-model="filters.mfaStatus"
              class="form-select"
              @change="applyFilters"
            >
              <option value="">All</option>
              <option value="enabled">MFA Enabled</option>
              <option value="disabled">MFA Disabled</option>
              <option value="required">MFA Required</option>
            </select>
          </div>
          <div class="col-md-4">
            <label class="form-label">Search Users</label>
            <input
              v-model="filters.search"
              class="form-control"
              placeholder="Search by name or email..."
              @input="applyFilters"
            />
          </div>
          <div class="col-md-2">
            <label class="form-label">&nbsp;</label>
            <button
              class="btn btn-outline-secondary w-100"
              @click="clearFilters"
            >
              Clear
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Users Table -->
    <div class="card">
      <div class="card-header">
        <div class="d-flex justify-content-between align-items-center">
          <h5 class="mb-0">
            <i class="bi bi-people me-2"></i>
            Users Password Security ({{ filteredUsers.length }})
          </h5>
          <div class="form-check">
            <input
              v-model="selectAll"
              class="form-check-input"
              type="checkbox"
              @change="toggleSelectAll"
            />
            <label class="form-check-label"> Select All </label>
          </div>
        </div>
      </div>
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-striped mb-0">
            <thead>
              <tr>
                <th width="50px">
                  <input
                    type="checkbox"
                    class="form-check-input"
                    :checked="selectAll"
                    @change="toggleSelectAll"
                  />
                </th>
                <th>User</th>
                <th>Security Level</th>
                <th>MFA Status</th>
                <th>Last Password Change</th>
                <th>Password Expires</th>
                <th>Reset Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="user in paginatedUsers"
                :key="user.id"
                :class="{
                  'table-danger': user.securityLevel === 'critical',
                  'table-warning': user.securityLevel === 'high',
                  'table-info': user.securityLevel === 'elevated',
                }"
              >
                <td>
                  <input
                    v-model="selectedUserIds"
                    class="form-check-input"
                    type="checkbox"
                    :value="user.id"
                  />
                </td>
                <td>
                  <div class="user-info">
                    <div class="fw-bold">
                      {{ user.firstName }} {{ user.lastName }}
                    </div>
                    <small class="text-muted">{{ user.email }}</small>
                    <div
                      v-if="
                        user.riskIndicators && user.riskIndicators.length > 0
                      "
                    >
                      <small class="text-danger">
                        <i class="bi bi-exclamation-triangle me-1"></i>
                        {{ user.riskIndicators.join(", ") }}
                      </small>
                    </div>
                  </div>
                </td>
                <td>
                  <span
                    class="badge"
                    :class="{
                      'bg-success': user.securityLevel === 'normal',
                      'bg-info': user.securityLevel === 'elevated',
                      'bg-warning': user.securityLevel === 'high',
                      'bg-danger': user.securityLevel === 'critical',
                    }"
                  >
                    {{ user.securityLevel }}
                  </span>
                </td>
                <td>
                  <div class="d-flex align-items-center">
                    <span
                      class="badge me-2"
                      :class="{
                        'bg-success': user.mfaEnabled,
                        'bg-warning': user.mfaRequired && !user.mfaEnabled,
                        'bg-secondary': !user.mfaEnabled && !user.mfaRequired,
                      }"
                    >
                      {{ getMfaStatus(user) }}
                    </span>
                    <button
                      v-if="!user.mfaEnabled"
                      class="btn btn-sm btn-outline-warning"
                      @click="requireMfa(user)"
                      title="Require MFA"
                    >
                      <i class="bi bi-shield-lock"></i>
                    </button>
                  </div>
                </td>
                <td>
                  <small v-if="user.lastPasswordChange">
                    {{ formatDate(user.lastPasswordChange) }}
                  </small>
                  <span v-else class="text-muted">Never</span>
                </td>
                <td>
                  <small v-if="user.passwordExpiresAt">
                    <span
                      :class="{
                        'text-danger': isPasswordExpiringSoon(
                          user.passwordExpiresAt
                        ),
                        'text-warning': isPasswordExpiringMidTerm(
                          user.passwordExpiresAt
                        ),
                      }"
                    >
                      {{ formatDate(user.passwordExpiresAt) }}
                    </span>
                  </small>
                  <span v-else class="text-muted">No Expiry</span>
                </td>
                <td>
                  <span
                    v-if="user.resetStatus"
                    class="badge"
                    :class="{
                      'bg-warning': user.resetStatus === 'pending',
                      'bg-success': user.resetStatus === 'completed',
                      'bg-danger': user.resetStatus === 'expired',
                    }"
                  >
                    {{ user.resetStatus }}
                  </span>
                  <span v-else class="text-muted">None</span>
                </td>
                <td>
                  <div class="btn-group btn-group-sm" role="group">
                    <button
                      class="btn btn-outline-primary"
                      @click="showResetModal(user)"
                      title="Reset Password"
                    >
                      <i class="bi bi-key"></i>
                    </button>
                    <button
                      class="btn btn-outline-warning"
                      @click="showSecurityReviewModal(user)"
                      title="Security Review"
                    >
                      <i class="bi bi-shield-exclamation"></i>
                    </button>
                    <button
                      class="btn btn-outline-info"
                      @click="showPasswordHistory(user)"
                      title="Password History"
                    >
                      <i class="bi bi-clock-history"></i>
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
      <div class="card-footer">
        <nav>
          <ul class="pagination pagination-sm mb-0">
            <li class="page-item" :class="{ disabled: currentPage === 1 }">
              <button
                class="page-link"
                @click="currentPage = 1"
                :disabled="currentPage === 1"
              >
                First
              </button>
            </li>
            <li class="page-item" :class="{ disabled: currentPage === 1 }">
              <button
                class="page-link"
                @click="currentPage--"
                :disabled="currentPage === 1"
              >
                Previous
              </button>
            </li>
            <li
              v-for="page in visiblePages"
              :key="page"
              class="page-item"
              :class="{ active: currentPage === page }"
            >
              <button class="page-link" @click="currentPage = page">
                {{ page }}
              </button>
            </li>
            <li
              class="page-item"
              :class="{ disabled: currentPage === totalPages }"
            >
              <button
                class="page-link"
                @click="currentPage++"
                :disabled="currentPage === totalPages"
              >
                Next
              </button>
            </li>
            <li
              class="page-item"
              :class="{ disabled: currentPage === totalPages }"
            >
              <button
                class="page-link"
                @click="currentPage = totalPages"
                :disabled="currentPage === totalPages"
              >
                Last
              </button>
            </li>
          </ul>
        </nav>
      </div>
    </div>

    <!-- Individual Password Reset Modal -->
    <div
      class="modal fade"
      :class="{ show: showResetUserModal }"
      :style="{ display: showResetUserModal ? 'block' : 'none' }"
      tabindex="-1"
    >
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Reset User Password</h5>
            <button
              type="button"
              class="btn-close"
              @click="showResetUserModal = false"
            ></button>
          </div>
          <div class="modal-body">
            <div v-if="modalUser" class="mb-3">
              <strong>User:</strong> {{ modalUser.firstName }}
              {{ modalUser.lastName }} ({{ modalUser.email }})
            </div>

            <form @submit.prevent="processPasswordReset">
              <div class="mb-3">
                <label class="form-label">Reset Type *</label>
                <select
                  v-model="resetForm.resetType"
                  class="form-select"
                  required
                >
                  <option value="">Select reset type...</option>
                  <option value="admin_reset">Administrative Reset</option>
                  <option value="security_incident">Security Incident</option>
                  <option value="forgot_password">Forgot Password</option>
                </select>
              </div>

              <div class="mb-3">
                <label class="form-label">Reason for Reset *</label>
                <textarea
                  v-model="resetForm.reason"
                  class="form-control"
                  rows="3"
                  placeholder="Provide the reason for this password reset..."
                  required
                ></textarea>
              </div>

              <div class="row mb-3">
                <div class="col-md-6">
                  <div class="form-check">
                    <input
                      v-model="resetForm.generateTemporary"
                      class="form-check-input"
                      type="checkbox"
                      id="generateTemporary"
                    />
                    <label class="form-check-label" for="generateTemporary">
                      Generate temporary password
                    </label>
                  </div>
                </div>
                <div class="col-md-6">
                  <div class="form-check">
                    <input
                      v-model="resetForm.requirePasswordChange"
                      class="form-check-input"
                      type="checkbox"
                      id="requirePasswordChange"
                      checked
                    />
                    <label class="form-check-label" for="requirePasswordChange">
                      Require password change on next login
                    </label>
                  </div>
                </div>
              </div>

              <div v-if="!resetForm.generateTemporary" class="mb-3">
                <label class="form-label">New Password</label>
                <div class="input-group">
                  <input
                    v-model="resetForm.newPassword"
                    :type="showPassword ? 'text' : 'password'"
                    class="form-control"
                    :placeholder="
                      resetForm.generateTemporary
                        ? 'Will be generated automatically'
                        : 'Enter new password'
                    "
                    :disabled="resetForm.generateTemporary"
                  />
                  <button
                    class="btn btn-outline-secondary"
                    type="button"
                    @click="showPassword = !showPassword"
                  >
                    <i :class="showPassword ? 'bi-eye-slash' : 'bi-eye'"></i>
                  </button>
                  <button
                    class="btn btn-outline-primary"
                    type="button"
                    @click="generateSecurePassword"
                  >
                    Generate
                  </button>
                </div>
                <div class="form-text">
                  Password must be at least 8 characters with uppercase,
                  lowercase, number, and special character.
                </div>
              </div>

              <div class="mb-3">
                <div class="form-check">
                  <input
                    v-model="resetForm.notifyUser"
                    class="form-check-input"
                    type="checkbox"
                    id="notifyUser"
                    checked
                  />
                  <label class="form-check-label" for="notifyUser">
                    Send password reset notification to user
                  </label>
                </div>
              </div>

              <div
                v-if="resetForm.resetType === 'security_incident'"
                class="mb-3"
              >
                <div class="alert alert-warning">
                  <h6>Security Incident Reset</h6>
                  <div class="form-check">
                    <input
                      v-model="resetForm.requireMfa"
                      class="form-check-input"
                      type="checkbox"
                      id="requireMfa"
                    />
                    <label class="form-check-label" for="requireMfa">
                      Force MFA setup before next login
                    </label>
                  </div>
                  <div class="form-check">
                    <input
                      v-model="resetForm.invalidateAllSessions"
                      class="form-check-input"
                      type="checkbox"
                      id="invalidateAllSessions"
                      checked
                    />
                    <label class="form-check-label" for="invalidateAllSessions">
                      Invalidate all existing sessions
                    </label>
                  </div>
                </div>
              </div>
            </form>
          </div>
          <div class="modal-footer">
            <button
              type="button"
              class="btn btn-secondary"
              @click="showResetUserModal = false"
            >
              Cancel
            </button>
            <button
              type="button"
              class="btn btn-primary"
              @click="processPasswordReset"
              :disabled="
                !resetForm.resetType || !resetForm.reason || processing
              "
            >
              Reset Password
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Loading Overlay -->
    <div v-if="loading" class="loading-overlay">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>

    <!-- Modal backdrop -->
    <div
      v-if="
        showResetUserModal || showBulkResetModal || showSecurityIncidentModal
      "
      class="modal-backdrop fade show"
    ></div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, reactive, onMounted } from "vue";
import { AdminApiService } from "@/services/AdminApiService";
import type { PasswordResetRequest } from "@/types";

// Reactive state
const loading = ref(false);
const processing = ref(false);
const users = ref<any[]>([]);
const selectedUserIds = ref<string[]>([]);
const selectAll = ref(false);
const currentPage = ref(1);
const pageSize = ref(20);
const modalUser = ref<any | null>(null);
const showPassword = ref(false);

// Modal visibility
const showResetUserModal = ref(false);
const showBulkResetModal = ref(false);
const showSecurityIncidentModal = ref(false);

// Security alerts
const securityAlerts = ref<any[]>([
  {
    id: 1,
    message: "15 users have passwords expiring in the next 7 days",
    type: "password_expiry",
    severity: "warning",
  },
  {
    id: 2,
    message: "3 users detected with suspicious login patterns",
    type: "suspicious_activity",
    severity: "high",
  },
]);

// Filters
const filters = reactive({
  resetStatus: "",
  securityLevel: "",
  mfaStatus: "",
  search: "",
});

// Reset form
const resetForm = reactive({
  resetType: "",
  reason: "",
  generateTemporary: true,
  newPassword: "",
  requirePasswordChange: true,
  notifyUser: true,
  requireMfa: false,
  invalidateAllSessions: false,
});

// Statistics
const stats = computed(() => {
  return {
    pendingResets: users.value.filter((u) => u.resetStatus === "pending")
      .length,
    expiringSoon: users.value.filter(
      (u) => u.passwordExpiresAt && isPasswordExpiringSoon(u.passwordExpiresAt)
    ).length,
    compromised: users.value.filter((u) => u.securityLevel === "critical")
      .length,
    mfaEnabled: users.value.filter((u) => u.mfaEnabled).length,
  };
});

// Computed properties
const filteredUsers = computed(() => {
  let filtered = users.value;

  if (filters.resetStatus) {
    filtered = filtered.filter(
      (user) => user.resetStatus === filters.resetStatus
    );
  }

  if (filters.securityLevel) {
    filtered = filtered.filter(
      (user) => user.securityLevel === filters.securityLevel
    );
  }

  if (filters.mfaStatus) {
    switch (filters.mfaStatus) {
      case "enabled":
        filtered = filtered.filter((user) => user.mfaEnabled);
        break;
      case "disabled":
        filtered = filtered.filter((user) => !user.mfaEnabled);
        break;
      case "required":
        filtered = filtered.filter((user) => user.mfaRequired);
        break;
    }
  }

  if (filters.search) {
    const search = filters.search.toLowerCase();
    filtered = filtered.filter(
      (user) =>
        user.firstName.toLowerCase().includes(search) ||
        user.lastName.toLowerCase().includes(search) ||
        user.email.toLowerCase().includes(search)
    );
  }

  return filtered;
});

const totalPages = computed(() => {
  return Math.ceil(filteredUsers.value.length / pageSize.value);
});

const paginatedUsers = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value;
  const end = start + pageSize.value;
  return filteredUsers.value.slice(start, end);
});

const visiblePages = computed(() => {
  const pages = [];
  const start = Math.max(1, currentPage.value - 2);
  const end = Math.min(totalPages.value, currentPage.value + 2);

  for (let i = start; i <= end; i++) {
    pages.push(i);
  }

  return pages;
});

// Methods
const loadUsers = async () => {
  loading.value = true;
  try {
    const response = await AdminApiService.users.getAll();
    users.value = (response.data?.data || []).map((user: any) => ({
      ...user,
      securityLevel: user.securityLevel || "normal",
      mfaEnabled: user.mfaEnabled || false,
      mfaRequired: user.mfaRequired || false,
      resetStatus: user.resetStatus || null,
      lastPasswordChange: user.lastPasswordChange || null,
      passwordExpiresAt: user.passwordExpiresAt || null,
      riskIndicators: user.riskIndicators || [],
    }));
  } catch (error) {
    console.error("Failed to load users:", error);
  } finally {
    loading.value = false;
  }
};

const showResetModal = (user: any) => {
  modalUser.value = user;
  showResetUserModal.value = true;

  // Reset form
  Object.assign(resetForm, {
    resetType: "",
    reason: "",
    generateTemporary: true,
    newPassword: "",
    requirePasswordChange: true,
    notifyUser: true,
    requireMfa: false,
    invalidateAllSessions: false,
  });
};

const showSecurityReviewModal = (user: any) => {
  // This would open a security review modal
  console.log("Show security review for:", user);
};

const showPasswordHistory = (user: any) => {
  // This would open a password history modal
  console.log("Show password history for:", user);
};

const processPasswordReset = async () => {
  if (!modalUser.value) return;

  processing.value = true;
  try {
    const resetRequest: PasswordResetRequest = {
      userId: modalUser.value.id,
      resetType: resetForm.resetType as
        | "admin_reset"
        | "security_incident"
        | "forgot_password",
      temporaryPassword: resetForm.generateTemporary
        ? undefined
        : resetForm.newPassword,
      requirePasswordChange: resetForm.requirePasswordChange,
      notifyUser: resetForm.notifyUser,
      resetBy: "current-admin", // Would come from auth context
      reason: resetForm.reason,
    };

    await AdminApiService.users.resetPassword(resetRequest);

    // Update local user data
    const user = users.value.find((u) => u.id === modalUser.value!.id);
    if (user) {
      user.resetStatus = "pending";
      user.lastPasswordChange = new Date();
    }

    showResetUserModal.value = false;
    modalUser.value = null;
  } catch (error) {
    console.error("Failed to reset password:", error);
  } finally {
    processing.value = false;
  }
};

const requireMfa = async (user: any) => {
  try {
    // This would call an API to require MFA for the user
    console.log("Requiring MFA for user:", user);
    user.mfaRequired = true;
  } catch (error) {
    console.error("Failed to require MFA:", error);
  }
};

const handleSecurityAlert = (alert: any) => {
  // Handle different types of security alerts
  switch (alert.type) {
    case "password_expiry":
      // Open bulk reset modal with expiring users pre-selected
      break;
    case "suspicious_activity":
      // Open security incident modal
      break;
  }
};

const generateSecurePassword = () => {
  // Generate a secure password
  const chars =
    "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
  let password = "";

  // Ensure at least one of each required character type
  password += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[Math.floor(Math.random() * 26)];
  password += "abcdefghijklmnopqrstuvwxyz"[Math.floor(Math.random() * 26)];
  password += "0123456789"[Math.floor(Math.random() * 10)];
  password += "!@#$%^&*"[Math.floor(Math.random() * 8)];

  // Add random characters to reach desired length
  for (let i = 4; i < 12; i++) {
    password += chars[Math.floor(Math.random() * chars.length)];
  }

  // Shuffle the password
  resetForm.newPassword = password
    .split("")
    .sort(() => Math.random() - 0.5)
    .join("");
};

const getMfaStatus = (user: any) => {
  if (user.mfaEnabled) return "Enabled";
  if (user.mfaRequired) return "Required";
  return "Disabled";
};

const isPasswordExpiringSoon = (expiresAt: string | Date) => {
  const expiry = new Date(expiresAt);
  const now = new Date();
  const daysUntilExpiry = Math.ceil(
    (expiry.getTime() - now.getTime()) / (1000 * 60 * 60 * 24)
  );
  return daysUntilExpiry <= 7 && daysUntilExpiry > 0;
};

const isPasswordExpiringMidTerm = (expiresAt: string | Date) => {
  const expiry = new Date(expiresAt);
  const now = new Date();
  const daysUntilExpiry = Math.ceil(
    (expiry.getTime() - now.getTime()) / (1000 * 60 * 60 * 24)
  );
  return daysUntilExpiry <= 30 && daysUntilExpiry > 7;
};

const formatDate = (date: string | Date) => {
  return new Date(date).toLocaleDateString();
};

const toggleSelectAll = () => {
  if (selectAll.value) {
    selectedUserIds.value = paginatedUsers.value.map((user) => user.id);
  } else {
    selectedUserIds.value = [];
  }
};

const applyFilters = () => {
  currentPage.value = 1;
};

const clearFilters = () => {
  Object.assign(filters, {
    resetStatus: "",
    securityLevel: "",
    mfaStatus: "",
    search: "",
  });
  currentPage.value = 1;
};

const refreshData = async () => {
  await loadUsers();
};

// Load data on mount
onMounted(async () => {
  await refreshData();
});
</script>

<style scoped>
.password-reset-security {
  padding: 1rem;
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

.modal {
  background-color: rgba(0, 0, 0, 0.5);
}

.user-info {
  min-width: 200px;
}

.table-responsive {
  max-height: 600px;
  overflow-y: auto;
}

.btn-group-sm .btn {
  padding: 0.25rem 0.5rem;
}

.pagination {
  justify-content: center;
}

.alert ul {
  margin-bottom: 0;
}

.input-group .form-control:disabled {
  background-color: #f8f9fa;
}

@media (max-width: 768px) {
  .action-buttons {
    flex-direction: column;
    gap: 0.5rem;
  }

  .table-responsive {
    font-size: 0.9rem;
  }

  .btn-group-sm .btn {
    padding: 0.125rem 0.25rem;
  }

  .statistics-row .col-md-3 {
    margin-bottom: 1rem;
  }
}
</style>
