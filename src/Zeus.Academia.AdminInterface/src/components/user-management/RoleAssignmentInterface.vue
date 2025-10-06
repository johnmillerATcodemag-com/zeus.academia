<template>
  <div class="role-assignment-interface">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2>Role & Permission Management</h2>
      <div class="action-buttons">
        <button
          class="btn btn-outline-primary me-2"
          @click="showBulkAssignModal = true"
        >
          <i class="bi bi-people-fill"></i>
          Bulk Assignment
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

    <!-- Filters -->
    <div class="card mb-4">
      <div class="card-body">
        <div class="row">
          <div class="col-md-3">
            <label class="form-label">Filter by Role</label>
            <select
              v-model="filters.role"
              class="form-select"
              @change="applyFilters"
            >
              <option value="">All Roles</option>
              <option
                v-for="role in availableRoles"
                :key="role.id"
                :value="role.id"
              >
                {{ role.name }}
              </option>
            </select>
          </div>
          <div class="col-md-3">
            <label class="form-label">Filter by Department</label>
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

    <!-- Role Assignment Grid -->
    <div class="row">
      <!-- Users List -->
      <div class="col-md-6">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">
              <i class="bi bi-people me-2"></i>
              Users ({{ filteredUsers.length }})
            </h5>
          </div>
          <div class="card-body p-0">
            <div class="user-list" style="max-height: 600px; overflow-y: auto">
              <div
                v-for="user in filteredUsers"
                :key="user.id"
                class="user-item p-3 border-bottom"
                :class="{
                  selected: selectedUser?.id === user.id,
                  'bg-light': selectedUser?.id === user.id,
                }"
                @click="selectUser(user)"
                style="cursor: pointer"
              >
                <div class="d-flex justify-content-between align-items-start">
                  <div class="user-info">
                    <div class="fw-bold">
                      {{ user.firstName }} {{ user.lastName }}
                    </div>
                    <div class="text-muted small">{{ user.email }}</div>
                    <div class="text-muted small">{{ user.department }}</div>
                  </div>
                  <div class="user-roles">
                    <span
                      v-for="role in user.roles"
                      :key="role"
                      class="badge bg-secondary me-1 mb-1"
                    >
                      {{ getRoleName(role) }}
                    </span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Role Management Panel -->
      <div class="col-md-6">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">
              <i class="bi bi-shield-check me-2"></i>
              Role Management
              <span v-if="selectedUser" class="text-muted small ms-2">
                - {{ selectedUser.firstName }} {{ selectedUser.lastName }}
              </span>
            </h5>
          </div>
          <div class="card-body">
            <div v-if="!selectedUser" class="text-center text-muted py-5">
              <i class="bi bi-person-circle display-4 mb-3"></i>
              <p>Select a user to manage their roles and permissions</p>
            </div>

            <div v-else>
              <!-- Current Roles -->
              <div class="mb-4">
                <h6>Current Roles</h6>
                <div v-if="selectedUser.roles && selectedUser.roles.length > 0">
                  <div
                    v-for="roleId in selectedUser.roles"
                    :key="roleId"
                    class="d-flex justify-content-between align-items-center p-2 mb-2 bg-light rounded"
                  >
                    <div>
                      <span class="fw-bold">{{ getRoleName(roleId) }}</span>
                      <small class="text-muted d-block">{{
                        getRoleDescription(roleId)
                      }}</small>
                    </div>
                    <button
                      class="btn btn-sm btn-outline-danger"
                      @click="revokeRole(selectedUser.id, roleId)"
                      :disabled="processing"
                    >
                      <i class="bi bi-x"></i>
                    </button>
                  </div>
                </div>
                <div v-else class="text-muted">No roles assigned</div>
              </div>

              <!-- Available Roles -->
              <div class="mb-4">
                <h6>Available Roles</h6>
                <div class="available-roles">
                  <div
                    v-for="role in getAvailableRolesForUser(selectedUser)"
                    :key="role.id"
                    class="d-flex justify-content-between align-items-center p-2 mb-2 border rounded"
                    style="cursor: pointer"
                    @click="assignRole(selectedUser.id, role.id)"
                  >
                    <div>
                      <span class="fw-bold">{{ role.name }}</span>
                      <small class="text-muted d-block">{{
                        role.description
                      }}</small>
                      <div class="permissions-preview">
                        <small class="text-success">
                          Permissions:
                          {{ role.permissions?.join(", ") || "None" }}
                        </small>
                      </div>
                    </div>
                    <button
                      class="btn btn-sm btn-outline-primary"
                      :disabled="processing"
                    >
                      <i class="bi bi-plus"></i>
                    </button>
                  </div>
                </div>
              </div>

              <!-- Permission Details -->
              <div v-if="selectedUser.roles && selectedUser.roles.length > 0">
                <h6>Effective Permissions</h6>
                <div class="permissions-list">
                  <span
                    v-for="permission in getEffectivePermissions(selectedUser)"
                    :key="permission"
                    class="badge bg-success me-1 mb-1"
                  >
                    {{ permission }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Bulk Assignment Modal -->
    <div
      class="modal fade"
      :class="{ show: showBulkAssignModal }"
      :style="{ display: showBulkAssignModal ? 'block' : 'none' }"
      tabindex="-1"
    >
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Bulk Role Assignment</h5>
            <button
              type="button"
              class="btn-close"
              @click="showBulkAssignModal = false"
            ></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="processBulkAssignment">
              <div class="mb-3">
                <label class="form-label">Select Role to Assign</label>
                <select
                  v-model="bulkAssignForm.roleId"
                  class="form-select"
                  required
                >
                  <option value="">Choose a role...</option>
                  <option
                    v-for="role in availableRoles"
                    :key="role.id"
                    :value="role.id"
                  >
                    {{ role.name }}
                  </option>
                </select>
              </div>

              <div class="mb-3">
                <label class="form-label">Target Users</label>
                <div class="form-check">
                  <input
                    v-model="bulkAssignForm.targetType"
                    class="form-check-input"
                    type="radio"
                    value="selected"
                    id="targetSelected"
                  />
                  <label class="form-check-label" for="targetSelected">
                    Selected users from list
                  </label>
                </div>
                <div class="form-check">
                  <input
                    v-model="bulkAssignForm.targetType"
                    class="form-check-input"
                    type="radio"
                    value="department"
                    id="targetDepartment"
                  />
                  <label class="form-check-label" for="targetDepartment">
                    All users in department
                  </label>
                </div>
                <div class="form-check">
                  <input
                    v-model="bulkAssignForm.targetType"
                    class="form-check-input"
                    type="radio"
                    value="role"
                    id="targetRole"
                  />
                  <label class="form-check-label" for="targetRole">
                    All users with specific role
                  </label>
                </div>
              </div>

              <div
                v-if="bulkAssignForm.targetType === 'department'"
                class="mb-3"
              >
                <label class="form-label">Select Department</label>
                <select
                  v-model="bulkAssignForm.departmentId"
                  class="form-select"
                >
                  <option value="">Choose department...</option>
                  <option
                    v-for="dept in departments"
                    :key="dept.id"
                    :value="dept.id"
                  >
                    {{ dept.name }}
                  </option>
                </select>
              </div>

              <div v-if="bulkAssignForm.targetType === 'role'" class="mb-3">
                <label class="form-label">Select Existing Role</label>
                <select
                  v-model="bulkAssignForm.existingRoleId"
                  class="form-select"
                >
                  <option value="">Choose role...</option>
                  <option
                    v-for="role in availableRoles"
                    :key="role.id"
                    :value="role.id"
                  >
                    {{ role.name }}
                  </option>
                </select>
              </div>

              <div v-if="bulkAssignForm.targetType === 'selected'" class="mb-3">
                <label class="form-label">Selected Users</label>
                <div
                  class="selected-users-list"
                  style="max-height: 200px; overflow-y: auto"
                >
                  <div class="form-check" v-for="user in users" :key="user.id">
                    <input
                      v-model="bulkAssignForm.selectedUserIds"
                      class="form-check-input"
                      type="checkbox"
                      :value="user.id"
                      :id="`user-${user.id}`"
                    />
                    <label class="form-check-label" :for="`user-${user.id}`">
                      {{ user.firstName }} {{ user.lastName }} ({{
                        user.email
                      }})
                    </label>
                  </div>
                </div>
              </div>

              <div class="form-check mb-3">
                <input
                  v-model="bulkAssignForm.notifyUsers"
                  class="form-check-input"
                  type="checkbox"
                  id="notifyUsers"
                />
                <label class="form-check-label" for="notifyUsers">
                  Send notification emails to affected users
                </label>
              </div>
            </form>

            <div v-if="bulkAssignPreview.length > 0" class="alert alert-info">
              <h6>Preview:</h6>
              <p>
                This will assign the role
                <strong>{{ getRoleName(bulkAssignForm.roleId) }}</strong> to
                {{ bulkAssignPreview.length }} users.
              </p>
            </div>
          </div>
          <div class="modal-footer">
            <button
              type="button"
              class="btn btn-secondary"
              @click="showBulkAssignModal = false"
            >
              Cancel
            </button>
            <button
              type="button"
              class="btn btn-primary"
              @click="processBulkAssignment"
              :disabled="!canProcessBulkAssignment || processing"
            >
              Assign Role to {{ bulkAssignPreview.length }} Users
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
      v-if="showBulkAssignModal"
      class="modal-backdrop fade show"
      @click="showBulkAssignModal = false"
    ></div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, reactive, onMounted, watch } from "vue";
import { AdminApiService } from "@/services/AdminApiService";
import type { UserRoleAssignment } from "@/types";

// Reactive state
const loading = ref(false);
const processing = ref(false);
const showBulkAssignModal = ref(false);
const users = ref<any[]>([]);
const availableRoles = ref<any[]>([]);
const departments = ref<any[]>([]);
const selectedUser = ref<any | null>(null);

// Filters
const filters = reactive({
  role: "",
  department: "",
  search: "",
});

// Bulk assignment form
const bulkAssignForm = reactive({
  roleId: "",
  targetType: "selected",
  departmentId: "",
  existingRoleId: "",
  selectedUserIds: [] as string[],
  notifyUsers: true,
});

// Computed properties
const filteredUsers = computed(() => {
  let filtered = users.value;

  if (filters.role) {
    filtered = filtered.filter((user) => user.roles?.includes(filters.role));
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

const bulkAssignPreview = computed(() => {
  if (!bulkAssignForm.roleId) return [];

  switch (bulkAssignForm.targetType) {
    case "selected":
      return users.value.filter((user) =>
        bulkAssignForm.selectedUserIds.includes(user.id)
      );
    case "department":
      return bulkAssignForm.departmentId
        ? users.value.filter(
            (user) => user.department === bulkAssignForm.departmentId
          )
        : [];
    case "role":
      return bulkAssignForm.existingRoleId
        ? users.value.filter((user) =>
            user.roles?.includes(bulkAssignForm.existingRoleId)
          )
        : [];
    default:
      return [];
  }
});

const canProcessBulkAssignment = computed(() => {
  return bulkAssignForm.roleId && bulkAssignPreview.value.length > 0;
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

const loadRoles = async () => {
  try {
    const response = await AdminApiService.roles.getAll();
    availableRoles.value = response.data || [];
  } catch (error) {
    console.error("Failed to load roles:", error);
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

const selectUser = (user: any) => {
  selectedUser.value = user;
};

const getRoleName = (roleId: string) => {
  const role = availableRoles.value.find((r) => r.id === roleId);
  return role?.name || roleId;
};

const getRoleDescription = (roleId: string) => {
  const role = availableRoles.value.find((r) => r.id === roleId);
  return role?.description || "";
};

const getAvailableRolesForUser = (user: any) => {
  const userRoles = user.roles || [];
  return availableRoles.value.filter((role) => !userRoles.includes(role.id));
};

const getEffectivePermissions = (user: any) => {
  const permissions = new Set<string>();
  const userRoles = user.roles || [];

  userRoles.forEach((roleId: string) => {
    const role = availableRoles.value.find((r) => r.id === roleId);
    if (role && role.permissions) {
      role.permissions.forEach((permission: string) =>
        permissions.add(permission)
      );
    }
  });

  return Array.from(permissions).sort();
};

const assignRole = async (userId: string, roleId: string) => {
  processing.value = true;
  try {
    await AdminApiService.roles.assign(userId, roleId);

    // Update local user data
    const user = users.value.find((u) => u.id === userId);
    if (user) {
      user.roles = user.roles || [];
      if (!user.roles.includes(roleId)) {
        user.roles.push(roleId);
      }
    }

    // Update selected user if it's the same
    if (selectedUser.value?.id === userId) {
      selectedUser.value = { ...user };
    }
  } catch (error) {
    console.error("Failed to assign role:", error);
  } finally {
    processing.value = false;
  }
};

const revokeRole = async (userId: string, roleId: string) => {
  processing.value = true;
  try {
    await AdminApiService.roles.revoke(userId, roleId);

    // Update local user data
    const user = users.value.find((u) => u.id === userId);
    if (user && user.roles) {
      user.roles = user.roles.filter((r: string) => r !== roleId);
    }

    // Update selected user if it's the same
    if (selectedUser.value?.id === userId) {
      selectedUser.value = { ...user };
    }
  } catch (error) {
    console.error("Failed to revoke role:", error);
  } finally {
    processing.value = false;
  }
};

const processBulkAssignment = async () => {
  processing.value = true;
  showBulkAssignModal.value = false;

  try {
    // Process assignments using the correct API method signature
    for (const user of bulkAssignPreview.value) {
      await AdminApiService.roles.assign(user.id, bulkAssignForm.roleId);

      // Update local data
      const localUser = users.value.find((u) => u.id === user.id);
      if (localUser) {
        localUser.roles = localUser.roles || [];
        if (!localUser.roles.includes(bulkAssignForm.roleId)) {
          localUser.roles.push(bulkAssignForm.roleId);
        }
      }
    }

    // Reset form
    Object.assign(bulkAssignForm, {
      roleId: "",
      targetType: "selected",
      departmentId: "",
      existingRoleId: "",
      selectedUserIds: [],
      notifyUsers: true,
    });
  } catch (error) {
    console.error("Bulk assignment failed:", error);
  } finally {
    processing.value = false;
  }
};

const applyFilters = () => {
  // Filters are reactive, so this just triggers the computed property
};

const clearFilters = () => {
  filters.role = "";
  filters.department = "";
  filters.search = "";
};

const refreshData = async () => {
  await Promise.all([loadUsers(), loadRoles(), loadDepartments()]);
};

// Load data on mount
onMounted(async () => {
  await refreshData();
});
</script>

<style scoped>
.role-assignment-interface {
  padding: 1rem;
}

.user-item {
  transition: background-color 0.2s ease;
}

.user-item:hover {
  background-color: #f8f9fa !important;
}

.user-item.selected {
  border-left: 4px solid #0d6efd;
}

.permissions-preview {
  margin-top: 0.25rem;
}

.available-roles > div:hover {
  background-color: #f8f9fa;
  cursor: pointer;
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

.permissions-list {
  max-height: 150px;
  overflow-y: auto;
}

.selected-users-list {
  border: 1px solid #dee2e6;
  border-radius: 0.375rem;
  padding: 0.5rem;
}

.modal {
  background-color: rgba(0, 0, 0, 0.5);
}

@media (max-width: 768px) {
  .row > .col-md-6 {
    margin-bottom: 1rem;
  }

  .user-item {
    padding: 1rem !important;
  }

  .user-info {
    margin-bottom: 0.5rem;
  }
}
</style>
