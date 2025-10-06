<template>
  <div class="user-lifecycle-management">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2>User Lifecycle Management</h2>
      <div class="action-buttons">
        <button
          class="btn btn-outline-warning me-2"
          @click="showBulkSuspendModal = true"
        >
          <i class="bi bi-pause-circle"></i>
          Bulk Suspend
        </button>
        <button
          class="btn btn-outline-danger me-2"
          @click="showBulkDeleteModal = true"
        >
          <i class="bi bi-trash"></i>
          Bulk Delete
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

    <!-- Statistics Cards -->
    <div class="row mb-4">
      <div class="col-md-3">
        <div class="card bg-success text-white">
          <div class="card-body">
            <div class="d-flex justify-content-between">
              <div>
                <h4>{{ stats.active }}</h4>
                <p class="mb-0">Active Users</p>
              </div>
              <i class="bi bi-person-check display-4"></i>
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card bg-warning text-white">
          <div class="card-body">
            <div class="d-flex justify-content-between">
              <div>
                <h4>{{ stats.suspended }}</h4>
                <p class="mb-0">Suspended</p>
              </div>
              <i class="bi bi-person-dash display-4"></i>
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card bg-secondary text-white">
          <div class="card-body">
            <div class="d-flex justify-content-between">
              <div>
                <h4>{{ stats.inactive }}</h4>
                <p class="mb-0">Inactive</p>
              </div>
              <i class="bi bi-person-x display-4"></i>
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card bg-danger text-white">
          <div class="card-body">
            <div class="d-flex justify-content-between">
              <div>
                <h4>{{ stats.pendingDeletion }}</h4>
                <p class="mb-0">Pending Deletion</p>
              </div>
              <i class="bi bi-hourglass-split display-4"></i>
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
            <label class="form-label">Status</label>
            <select
              v-model="filters.status"
              class="form-select"
              @change="applyFilters"
            >
              <option value="">All Status</option>
              <option value="active">Active</option>
              <option value="suspended">Suspended</option>
              <option value="inactive">Inactive</option>
              <option value="pending_deletion">Pending Deletion</option>
            </select>
          </div>
          <div class="col-md-2">
            <label class="form-label">Role</label>
            <select
              v-model="filters.role"
              class="form-select"
              @change="applyFilters"
            >
              <option value="">All Roles</option>
              <option value="student">Student</option>
              <option value="faculty">Faculty</option>
              <option value="staff">Staff</option>
            </select>
          </div>
          <div class="col-md-2">
            <label class="form-label">Department</label>
            <select
              v-model="filters.department"
              class="form-select"
              @change="applyFilters"
            >
              <option value="">All Departments</option>
              <option
                v-for="dept in departments"
                :key="dept.id"
                :value="dept.id"
              >
                {{ dept.name }}
              </option>
            </select>
          </div>
          <div class="col-md-4">
            <label class="form-label">Search</label>
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
            Users ({{ filteredUsers.length }})
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
                <th>Role</th>
                <th>Department</th>
                <th>Status</th>
                <th>Last Login</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="user in paginatedUsers"
                :key="user.id"
                :class="{
                  'table-warning': user.status === 'suspended',
                  'table-secondary': user.status === 'inactive',
                  'table-danger': user.status === 'pending_deletion',
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
                  </div>
                </td>
                <td>
                  <span class="badge bg-secondary">{{ user.role }}</span>
                </td>
                <td>{{ user.department }}</td>
                <td>
                  <span
                    class="badge"
                    :class="{
                      'bg-success': user.status === 'active',
                      'bg-warning': user.status === 'suspended',
                      'bg-secondary': user.status === 'inactive',
                      'bg-danger': user.status === 'pending_deletion',
                    }"
                  >
                    {{ formatStatus(user.status) }}
                  </span>
                </td>
                <td>
                  <small v-if="user.lastLogin">
                    {{ formatDate(user.lastLogin) }}
                  </small>
                  <span v-else class="text-muted">Never</span>
                </td>
                <td>
                  <div class="btn-group btn-group-sm" role="group">
                    <button
                      v-if="user.status === 'active'"
                      class="btn btn-outline-warning"
                      @click="showSuspendModal(user)"
                      title="Suspend User"
                    >
                      <i class="bi bi-pause"></i>
                    </button>
                    <button
                      v-if="user.status === 'suspended'"
                      class="btn btn-outline-success"
                      @click="showReactivateModal(user)"
                      title="Reactivate User"
                    >
                      <i class="bi bi-play"></i>
                    </button>
                    <button
                      v-if="user.status !== 'pending_deletion'"
                      class="btn btn-outline-danger"
                      @click="showDeleteModal(user)"
                      title="Delete User"
                    >
                      <i class="bi bi-trash"></i>
                    </button>
                    <button
                      class="btn btn-outline-info"
                      @click="showUserHistory(user)"
                      title="View History"
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

    <!-- Suspend User Modal -->
    <div
      class="modal fade"
      :class="{ show: showSuspendUserModal }"
      :style="{ display: showSuspendUserModal ? 'block' : 'none' }"
      tabindex="-1"
    >
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Suspend User</h5>
            <button
              type="button"
              class="btn-close"
              @click="showSuspendUserModal = false"
            ></button>
          </div>
          <div class="modal-body">
            <div v-if="modalUser" class="mb-3">
              <strong>User:</strong> {{ modalUser.firstName }}
              {{ modalUser.lastName }} ({{ modalUser.email }})
            </div>

            <form @submit.prevent="processSuspension">
              <div class="mb-3">
                <label class="form-label">Reason for Suspension *</label>
                <select
                  v-model="suspensionForm.reason"
                  class="form-select"
                  required
                >
                  <option value="">Select reason...</option>
                  <option value="policy_violation">Policy Violation</option>
                  <option value="security_concern">Security Concern</option>
                  <option value="inactive_account">Inactive Account</option>
                  <option value="investigation">Under Investigation</option>
                  <option value="other">Other</option>
                </select>
              </div>

              <div class="mb-3">
                <label class="form-label">Additional Details</label>
                <textarea
                  v-model="suspensionForm.details"
                  class="form-control"
                  rows="3"
                  placeholder="Provide additional context for this suspension..."
                ></textarea>
              </div>

              <div class="mb-3">
                <label class="form-label">Suspension Duration</label>
                <select v-model="suspensionForm.duration" class="form-select">
                  <option value="indefinite">Indefinite</option>
                  <option value="7">7 days</option>
                  <option value="30">30 days</option>
                  <option value="90">90 days</option>
                  <option value="custom">Custom</option>
                </select>
              </div>

              <div v-if="suspensionForm.duration === 'custom'" class="mb-3">
                <label class="form-label">End Date</label>
                <input
                  v-model="suspensionForm.endDate"
                  type="date"
                  class="form-control"
                  :min="getTomorrowDate()"
                />
              </div>

              <div class="form-check mb-3">
                <input
                  v-model="suspensionForm.notifyUser"
                  class="form-check-input"
                  type="checkbox"
                  id="notifyUser"
                />
                <label class="form-check-label" for="notifyUser">
                  Send notification email to user
                </label>
              </div>
            </form>
          </div>
          <div class="modal-footer">
            <button
              type="button"
              class="btn btn-secondary"
              @click="showSuspendUserModal = false"
            >
              Cancel
            </button>
            <button
              type="button"
              class="btn btn-warning"
              @click="processSuspension"
              :disabled="!suspensionForm.reason || processing"
            >
              Suspend User
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Delete User Modal -->
    <div
      class="modal fade"
      :class="{ show: showDeleteUserModal }"
      :style="{ display: showDeleteUserModal ? 'block' : 'none' }"
      tabindex="-1"
    >
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header bg-danger text-white">
            <h5 class="modal-title">Delete User Account</h5>
            <button
              type="button"
              class="btn-close btn-close-white"
              @click="showDeleteUserModal = false"
            ></button>
          </div>
          <div class="modal-body">
            <div class="alert alert-danger">
              <i class="bi bi-exclamation-triangle me-2"></i>
              This action cannot be undone. Please review the data retention
              options carefully.
            </div>

            <div v-if="modalUser" class="mb-3">
              <strong>User:</strong> {{ modalUser.firstName }}
              {{ modalUser.lastName }} ({{ modalUser.email }})
            </div>

            <form @submit.prevent="processDeletion">
              <div class="mb-3">
                <label class="form-label">Reason for Deletion *</label>
                <select
                  v-model="deletionForm.reason"
                  class="form-select"
                  required
                >
                  <option value="">Select reason...</option>
                  <option value="user_request">User Requested</option>
                  <option value="data_compliance">
                    Data Compliance (GDPR)
                  </option>
                  <option value="account_closure">Account Closure</option>
                  <option value="duplicate_account">Duplicate Account</option>
                  <option value="security_breach">Security Breach</option>
                  <option value="other">Other</option>
                </select>
              </div>

              <div class="mb-3">
                <label class="form-label">Data Retention Options *</label>
                <div class="form-check">
                  <input
                    v-model="deletionForm.dataRetention"
                    class="form-check-input"
                    type="radio"
                    value="anonymize"
                    id="anonymize"
                  />
                  <label class="form-check-label" for="anonymize">
                    <strong>Anonymize Data</strong> - Remove personal
                    identifiers but keep academic records
                  </label>
                </div>
                <div class="form-check">
                  <input
                    v-model="deletionForm.dataRetention"
                    class="form-check-input"
                    type="radio"
                    value="archive"
                    id="archive"
                  />
                  <label class="form-check-label" for="archive">
                    <strong>Archive Account</strong> - Mark as deleted but
                    retain all data
                  </label>
                </div>
                <div class="form-check">
                  <input
                    v-model="deletionForm.dataRetention"
                    class="form-check-input"
                    type="radio"
                    value="purge"
                    id="purge"
                  />
                  <label class="form-check-label" for="purge">
                    <strong>Complete Purge</strong> - Permanently delete all
                    data (requires admin approval)
                  </label>
                </div>
              </div>

              <div class="mb-3">
                <label class="form-label">Additional Notes</label>
                <textarea
                  v-model="deletionForm.notes"
                  class="form-control"
                  rows="3"
                  placeholder="Document the reason and any special considerations..."
                ></textarea>
              </div>

              <div class="form-check mb-3">
                <input
                  v-model="deletionForm.requireApproval"
                  class="form-check-input"
                  type="checkbox"
                  id="requireApproval"
                />
                <label class="form-check-label" for="requireApproval">
                  Require supervisor approval before deletion
                </label>
              </div>
            </form>
          </div>
          <div class="modal-footer">
            <button
              type="button"
              class="btn btn-secondary"
              @click="showDeleteUserModal = false"
            >
              Cancel
            </button>
            <button
              type="button"
              class="btn btn-danger"
              @click="processDeletion"
              :disabled="
                !deletionForm.reason ||
                !deletionForm.dataRetention ||
                processing
              "
            >
              {{
                deletionForm.requireApproval
                  ? "Submit for Approval"
                  : "Delete User"
              }}
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
        showSuspendUserModal ||
        showDeleteUserModal ||
        showBulkSuspendModal ||
        showBulkDeleteModal
      "
      class="modal-backdrop fade show"
    ></div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, reactive, onMounted } from "vue";
import { AdminApiService } from "@/services/AdminApiService";
import type { UserLifecycleAction } from "@/types";

// Reactive state
const loading = ref(false);
const processing = ref(false);
const users = ref<any[]>([]);
const departments = ref<any[]>([]);
const selectedUserIds = ref<string[]>([]);
const selectAll = ref(false);
const currentPage = ref(1);
const pageSize = ref(20);
const modalUser = ref<any | null>(null);

// Modal visibility
const showSuspendUserModal = ref(false);
const showDeleteUserModal = ref(false);
const showBulkSuspendModal = ref(false);
const showBulkDeleteModal = ref(false);

// Filters
const filters = reactive({
  status: "",
  role: "",
  department: "",
  search: "",
});

// Forms
const suspensionForm = reactive({
  reason: "",
  details: "",
  duration: "indefinite",
  endDate: "",
  notifyUser: true,
});

const deletionForm = reactive({
  reason: "",
  dataRetention: "",
  notes: "",
  requireApproval: false,
});

// Statistics
const stats = computed(() => {
  const counts = {
    active: 0,
    suspended: 0,
    inactive: 0,
    pendingDeletion: 0,
  };

  users.value.forEach((user) => {
    switch (user.status) {
      case "active":
        counts.active++;
        break;
      case "suspended":
        counts.suspended++;
        break;
      case "inactive":
        counts.inactive++;
        break;
      case "pending_deletion":
        counts.pendingDeletion++;
        break;
    }
  });

  return counts;
});

// Computed properties
const filteredUsers = computed(() => {
  let filtered = users.value;

  if (filters.status) {
    filtered = filtered.filter((user) => user.status === filters.status);
  }

  if (filters.role) {
    filtered = filtered.filter((user) => user.role === filters.role);
  }

  if (filters.department) {
    filtered = filtered.filter(
      (user) => user.department === filters.department
    );
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
    users.value = response.data?.data || [];
  } catch (error) {
    console.error("Failed to load users:", error);
  } finally {
    loading.value = false;
  }
};

const loadDepartments = async () => {
  try {
    const response = await AdminApiService.academic.getDepartments();
    departments.value = response.data || [];
  } catch (error) {
    console.error("Failed to load departments:", error);
  }
};

const showSuspendModal = (user: any) => {
  modalUser.value = user;
  showSuspendUserModal.value = true;

  // Reset form
  Object.assign(suspensionForm, {
    reason: "",
    details: "",
    duration: "indefinite",
    endDate: "",
    notifyUser: true,
  });
};

const showDeleteModal = (user: any) => {
  modalUser.value = user;
  showDeleteUserModal.value = true;

  // Reset form
  Object.assign(deletionForm, {
    reason: "",
    dataRetention: "",
    notes: "",
    requireApproval: false,
  });
};

const showReactivateModal = async (user: any) => {
  if (
    confirm(
      `Are you sure you want to reactivate ${user.firstName} ${user.lastName}?`
    )
  ) {
    await processReactivation(user);
  }
};

const showUserHistory = (user: any) => {
  // This would open a history modal or navigate to a history page
  console.log("Show user history for:", user);
};

const processSuspension = async () => {
  if (!modalUser.value) return;

  processing.value = true;
  try {
    const suspensionData: UserLifecycleAction = {
      userId: modalUser.value.id,
      action: "suspend",
      reason: suspensionForm.reason,
      effectiveDate: new Date(),
      performedBy: "current-admin", // Would come from auth context
      notifyUser: suspensionForm.notifyUser,
      retainData: true,
      expirationDate: suspensionForm.endDate
        ? new Date(suspensionForm.endDate)
        : undefined,
    };

    await AdminApiService.users.suspend(suspensionData);

    // Update local user status
    const user = users.value.find((u) => u.id === modalUser.value!.id);
    if (user) {
      user.status = "suspended";
    }

    showSuspendUserModal.value = false;
    modalUser.value = null;
  } catch (error) {
    console.error("Failed to suspend user:", error);
  } finally {
    processing.value = false;
  }
};

const processReactivation = async (user: any) => {
  processing.value = true;
  try {
    const reactivationData: UserLifecycleAction = {
      userId: user.id,
      action: "reactivate",
      reason: "Manual reactivation by administrator",
      effectiveDate: new Date(),
      performedBy: "current-admin",
      notifyUser: true,
      retainData: true,
    };

    await AdminApiService.users.reactivate(reactivationData);

    // Update local user status
    const localUser = users.value.find((u) => u.id === user.id);
    if (localUser) {
      localUser.status = "active";
    }
  } catch (error) {
    console.error("Failed to reactivate user:", error);
  } finally {
    processing.value = false;
  }
};

const processDeletion = async () => {
  if (!modalUser.value) return;

  processing.value = true;
  try {
    // For now, we'll just use the simple delete API
    // In a real implementation, you'd want to send the full lifecycle data
    await AdminApiService.users.delete(modalUser.value.id);

    // Update local user status
    const user = users.value.find((u) => u.id === modalUser.value!.id);
    if (user) {
      if (deletionForm.requireApproval) {
        user.status = "pending_deletion";
      } else {
        // Remove from list if immediate deletion
        const index = users.value.findIndex(
          (u) => u.id === modalUser.value!.id
        );
        if (index > -1) {
          users.value.splice(index, 1);
        }
      }
    }

    showDeleteUserModal.value = false;
    modalUser.value = null;
  } catch (error) {
    console.error("Failed to delete user:", error);
  } finally {
    processing.value = false;
  }
};

const toggleSelectAll = () => {
  if (selectAll.value) {
    selectedUserIds.value = paginatedUsers.value.map((user) => user.id);
  } else {
    selectedUserIds.value = [];
  }
};

const formatStatus = (status: string) => {
  const statusMap: Record<string, string> = {
    active: "Active",
    suspended: "Suspended",
    inactive: "Inactive",
    pending_deletion: "Pending Deletion",
  };
  return statusMap[status] || status;
};

const formatDate = (date: string | Date) => {
  return new Date(date).toLocaleDateString();
};

const getTomorrowDate = () => {
  const tomorrow = new Date();
  tomorrow.setDate(tomorrow.getDate() + 1);
  return tomorrow.toISOString().split("T")[0];
};

const applyFilters = () => {
  currentPage.value = 1;
};

const clearFilters = () => {
  Object.assign(filters, {
    status: "",
    role: "",
    department: "",
    search: "",
  });
  currentPage.value = 1;
};

const refreshData = async () => {
  await Promise.all([loadUsers(), loadDepartments()]);
};

// Load data on mount
onMounted(async () => {
  await refreshData();
});
</script>

<style scoped>
.user-lifecycle-management {
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
}
</style>
