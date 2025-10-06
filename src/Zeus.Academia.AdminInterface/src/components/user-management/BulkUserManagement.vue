<template>
  <div class="bulk-user-management">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2>Bulk User Management</h2>
      <div class="action-buttons">
        <button
          class="btn btn-outline-primary me-2"
          @click="showUploadModal = true"
        >
          <i class="bi bi-upload"></i>
          Import CSV
        </button>
        <button class="btn btn-primary" @click="showCreateModal = true">
          <i class="bi bi-person-plus"></i>
          Bulk Create
        </button>
      </div>
    </div>

    <!-- Progress Card -->
    <div v-if="operationInProgress" class="card mb-4">
      <div class="card-header bg-primary text-white">
        <h5 class="mb-0">
          <i class="bi bi-gear-fill me-2"></i>
          Operation in Progress
        </h5>
      </div>
      <div class="card-body">
        <div class="d-flex justify-content-between align-items-center mb-2">
          <span>{{ currentOperation }}</span>
          <span class="text-muted">{{ progressPercent }}%</span>
        </div>
        <div class="progress mb-3">
          <div
            class="progress-bar"
            :style="`width: ${progressPercent}%`"
            role="progressbar"
          ></div>
        </div>
        <div class="row">
          <div class="col-4 text-center">
            <div class="fw-bold text-success">{{ operationStats.success }}</div>
            <small class="text-muted">Success</small>
          </div>
          <div class="col-4 text-center">
            <div class="fw-bold text-warning">{{ operationStats.pending }}</div>
            <small class="text-muted">Pending</small>
          </div>
          <div class="col-4 text-center">
            <div class="fw-bold text-danger">{{ operationStats.failed }}</div>
            <small class="text-muted">Failed</small>
          </div>
        </div>
      </div>
    </div>

    <!-- Results Table -->
    <div v-if="lastOperationResults" class="card">
      <div class="card-header">
        <h5 class="mb-0">
          <i class="bi bi-list-check me-2"></i>
          Last Operation Results
        </h5>
      </div>
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-striped">
            <thead>
              <tr>
                <th>Email</th>
                <th>Name</th>
                <th>Role</th>
                <th>Status</th>
                <th>Message</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="result in lastOperationResults"
                :key="result.email || result.id"
                :class="{
                  'table-success': result.status === 'success',
                  'table-danger': result.status === 'failed',
                  'table-warning': result.status === 'partial',
                }"
              >
                <td>{{ result.email }}</td>
                <td>{{ result.firstName }} {{ result.lastName }}</td>
                <td>
                  <span class="badge bg-secondary">{{ result.role }}</span>
                </td>
                <td>
                  <span
                    class="badge"
                    :class="{
                      'bg-success': result.status === 'success',
                      'bg-danger': result.status === 'failed',
                      'bg-warning': result.status === 'partial',
                    }"
                  >
                    {{ result.status }}
                  </span>
                </td>
                <td>{{ result.message }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- CSV Upload Modal -->
    <div
      class="modal fade"
      :class="{ show: showUploadModal }"
      :style="{ display: showUploadModal ? 'block' : 'none' }"
      tabindex="-1"
    >
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Import Users from CSV</h5>
            <button
              type="button"
              class="btn-close"
              @click="showUploadModal = false"
            ></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">CSV File</label>
              <input
                type="file"
                class="form-control"
                accept=".csv"
                @change="handleFileUpload"
                ref="fileInput"
              />
              <div class="form-text">
                CSV should contain columns: email, firstName, lastName, role,
                department
                <a href="#" @click="downloadTemplate" class="ms-2"
                  >Download Template</a
                >
              </div>
            </div>

            <div v-if="csvPreview.length > 0" class="mb-3">
              <h6>Preview (first 5 rows)</h6>
              <div class="table-responsive">
                <table class="table table-sm table-bordered">
                  <thead>
                    <tr>
                      <th v-for="header in csvHeaders" :key="header">
                        {{ header }}
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr
                      v-for="(row, index) in csvPreview.slice(0, 5)"
                      :key="index"
                    >
                      <td v-for="(value, key) in row" :key="key">
                        {{ value }}
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
              <div class="text-muted">{{ csvPreview.length }} total rows</div>
            </div>

            <div
              v-if="csvValidationErrors.length > 0"
              class="alert alert-warning"
            >
              <h6>Validation Issues:</h6>
              <ul class="mb-0">
                <li v-for="error in csvValidationErrors" :key="error">
                  {{ error }}
                </li>
              </ul>
            </div>
          </div>
          <div class="modal-footer">
            <button
              type="button"
              class="btn btn-secondary"
              @click="showUploadModal = false"
            >
              Cancel
            </button>
            <button
              type="button"
              class="btn btn-primary"
              @click="processCSVImport"
              :disabled="
                csvPreview.length === 0 || csvValidationErrors.length > 0
              "
            >
              Import {{ csvPreview.length }} Users
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Bulk Create Modal -->
    <div
      class="modal fade"
      :class="{ show: showCreateModal }"
      :style="{ display: showCreateModal ? 'block' : 'none' }"
      tabindex="-1"
    >
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Bulk Create Users</h5>
            <button
              type="button"
              class="btn-close"
              @click="showCreateModal = false"
            ></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="processBulkCreate">
              <div class="row mb-3">
                <div class="col-md-6">
                  <label class="form-label">Default Role</label>
                  <select
                    v-model="bulkCreateForm.defaultRole"
                    class="form-select"
                    required
                  >
                    <option value="">Select Role</option>
                    <option value="student">Student</option>
                    <option value="faculty">Faculty</option>
                    <option value="staff">Staff</option>
                  </select>
                </div>
                <div class="col-md-6">
                  <label class="form-label">Default Department</label>
                  <select
                    v-model="bulkCreateForm.defaultDepartment"
                    class="form-select"
                  >
                    <option value="">Select Department</option>
                    <option
                      v-for="dept in departments"
                      :key="dept.id"
                      :value="dept.id"
                    >
                      {{ dept.name }}
                    </option>
                  </select>
                </div>
              </div>

              <div class="mb-3">
                <label class="form-label"
                  >Users (one per line: email,firstName,lastName)</label
                >
                <textarea
                  v-model="bulkCreateForm.userList"
                  class="form-control"
                  rows="10"
                  placeholder="john.doe@university.edu,John,Doe&#10;jane.smith@university.edu,Jane,Smith"
                  required
                ></textarea>
                <div class="form-text">
                  Format: email,firstName,lastName (one user per line)
                </div>
              </div>

              <div class="row mb-3">
                <div class="col-md-6">
                  <div class="form-check">
                    <input
                      v-model="bulkCreateForm.sendWelcomeEmail"
                      class="form-check-input"
                      type="checkbox"
                      id="sendWelcome"
                    />
                    <label class="form-check-label" for="sendWelcome">
                      Send welcome emails
                    </label>
                  </div>
                </div>
                <div class="col-md-6">
                  <div class="form-check">
                    <input
                      v-model="bulkCreateForm.requirePasswordReset"
                      class="form-check-input"
                      type="checkbox"
                      id="requirePasswordChange"
                    />
                    <label class="form-check-label" for="requirePasswordChange">
                      Require password change on first login
                    </label>
                  </div>
                </div>
              </div>
            </form>

            <div v-if="bulkCreatePreview.length > 0" class="mt-4">
              <h6>Preview</h6>
              <div class="table-responsive">
                <table class="table table-sm">
                  <thead>
                    <tr>
                      <th>Email</th>
                      <th>Name</th>
                      <th>Role</th>
                      <th>Department</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr
                      v-for="user in bulkCreatePreview.slice(0, 5)"
                      :key="user.email"
                    >
                      <td>{{ user.email }}</td>
                      <td>{{ user.firstName }} {{ user.lastName }}</td>
                      <td>{{ user.role }}</td>
                      <td>{{ user.department }}</td>
                    </tr>
                  </tbody>
                </table>
              </div>
              <div class="text-muted">
                {{ bulkCreatePreview.length }} users ready to create
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button
              type="button"
              class="btn btn-secondary"
              @click="showCreateModal = false"
            >
              Cancel
            </button>
            <button
              type="button"
              class="btn btn-primary"
              @click="processBulkCreate"
              :disabled="bulkCreatePreview.length === 0"
            >
              Create {{ bulkCreatePreview.length }} Users
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal backdrop -->
    <div
      v-if="showUploadModal || showCreateModal"
      class="modal-backdrop fade show"
      @click="
        showUploadModal = false;
        showCreateModal = false;
      "
    ></div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, reactive, watch, onMounted } from "vue";
import { AdminApiService } from "@/services/AdminApiService";
import type { BulkUserCreationData } from "@/types";

// Reactive state
const showUploadModal = ref(false);
const showCreateModal = ref(false);
const operationInProgress = ref(false);
const currentOperation = ref("");
const csvPreview = ref<any[]>([]);
const csvHeaders = ref<string[]>([]);
const csvValidationErrors = ref<string[]>([]);
const lastOperationResults = ref<any[] | null>(null);
const departments = ref<any[]>([]);
const fileInput = ref<HTMLInputElement>();

// Operation progress tracking
const operationStats = reactive({
  success: 0,
  pending: 0,
  failed: 0,
  total: 0,
});

const progressPercent = computed(() => {
  if (operationStats.total === 0) return 0;
  return Math.round(
    ((operationStats.success + operationStats.failed) / operationStats.total) *
      100
  );
});

// Bulk create form
const bulkCreateForm = reactive({
  defaultRole: "",
  defaultDepartment: "",
  userList: "",
  sendWelcomeEmail: true,
  requirePasswordReset: true,
  defaultPassword: "",
});

const bulkCreatePreview = computed(() => {
  if (!bulkCreateForm.userList.trim()) return [];

  const lines = bulkCreateForm.userList.trim().split("\n");
  return lines
    .map((line) => {
      const [email, firstName, lastName] = line.split(",").map((s) => s.trim());
      return {
        email,
        firstName,
        lastName,
        role: bulkCreateForm.defaultRole,
        department: bulkCreateForm.defaultDepartment,
      };
    })
    .filter((user) => user.email && user.firstName && user.lastName);
});

// File upload handling
const handleFileUpload = (event: Event) => {
  const file = (event.target as HTMLInputElement).files?.[0];
  if (!file) return;

  const reader = new FileReader();
  reader.onload = (e) => {
    const csv = e.target?.result as string;
    parseCSV(csv);
  };
  reader.readAsText(file);
};

const parseCSV = (csvText: string) => {
  const lines = csvText.trim().split("\n");
  const headers = lines[0].split(",").map((h) => h.trim());

  csvHeaders.value = headers;
  csvPreview.value = lines.slice(1).map((line) => {
    const values = line.split(",").map((v) => v.trim());
    const row: any = {};
    headers.forEach((header, index) => {
      row[header] = values[index] || "";
    });
    return row;
  });

  validateCSV();
};

const validateCSV = () => {
  csvValidationErrors.value = [];

  // Check required columns
  const requiredColumns = ["email", "firstName", "lastName", "role"];
  requiredColumns.forEach((col) => {
    if (!csvHeaders.value.includes(col)) {
      csvValidationErrors.value.push(`Missing required column: ${col}`);
    }
  });

  // Validate data
  csvPreview.value.forEach((row, index) => {
    if (!row.email || !isValidEmail(row.email)) {
      csvValidationErrors.value.push(`Row ${index + 2}: Invalid email`);
    }
    if (!row.firstName) {
      csvValidationErrors.value.push(`Row ${index + 2}: First name required`);
    }
    if (!row.lastName) {
      csvValidationErrors.value.push(`Row ${index + 2}: Last name required`);
    }
  });
};

const isValidEmail = (email: string) => {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
};

const downloadTemplate = () => {
  const template =
    "email,firstName,lastName,role,department\njohn.doe@university.edu,John,Doe,student,Computer Science\njane.smith@university.edu,Jane,Smith,faculty,Mathematics";
  const blob = new Blob([template], { type: "text/csv" });
  const url = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = "user-import-template.csv";
  a.click();
  URL.revokeObjectURL(url);
};

// Processing functions
const processCSVImport = async () => {
  showUploadModal.value = false;
  operationInProgress.value = true;
  currentOperation.value = "Importing users from CSV...";

  const userData: BulkUserCreationData = {
    users: csvPreview.value.map((row) => ({
      email: row.email,
      firstName: row.firstName,
      lastName: row.lastName,
      role: row.role,
      department: row.department || "",
    })),
    options: {
      sendWelcomeEmail: true,
      requirePasswordReset: true,
      defaultPassword: "",
    },
  };

  try {
    const response = await AdminApiService.users.bulkCreate(userData);
    lastOperationResults.value = response.data.results;
    updateOperationStats(response.data.results);
  } catch (error) {
    console.error("CSV import failed:", error);
    // Handle error
  } finally {
    operationInProgress.value = false;
    csvPreview.value = [];
    if (fileInput.value) {
      fileInput.value.value = "";
    }
  }
};

const processBulkCreate = async () => {
  showCreateModal.value = false;
  operationInProgress.value = true;
  currentOperation.value = "Creating users...";

  const userData: BulkUserCreationData = {
    users: bulkCreatePreview.value,
    options: {
      sendWelcomeEmail: bulkCreateForm.sendWelcomeEmail,
      requirePasswordReset: bulkCreateForm.requirePasswordReset,
      defaultPassword: bulkCreateForm.defaultPassword,
    },
  };

  try {
    const response = await AdminApiService.users.bulkCreate(userData);
    lastOperationResults.value = response.data.results;
    updateOperationStats(response.data.results);
  } catch (error) {
    console.error("Bulk create failed:", error);
    // Handle error
  } finally {
    operationInProgress.value = false;
    bulkCreateForm.userList = "";
  }
};

const updateOperationStats = (results: any[]) => {
  operationStats.total = results.length;
  operationStats.success = results.filter((r) => r.status === "success").length;
  operationStats.failed = results.filter((r) => r.status === "failed").length;
  operationStats.pending = results.filter((r) => r.status === "pending").length;
};

// Load departments on mount
onMounted(async () => {
  try {
    const response = await AdminApiService.academic.getDepartments();
    departments.value = response.data || [];
  } catch (error) {
    console.error("Failed to load departments:", error);
  }
});
</script>

<style scoped>
.bulk-user-management {
  padding: 1rem;
}

.modal {
  background-color: rgba(0, 0, 0, 0.5);
}

.action-buttons {
  display: flex;
  gap: 0.5rem;
}

.progress {
  height: 8px;
}

.table-responsive {
  max-height: 400px;
  overflow-y: auto;
}

textarea {
  font-family: monospace;
  font-size: 0.9rem;
}

.form-text a {
  text-decoration: none;
}

.form-text a:hover {
  text-decoration: underline;
}

@media (max-width: 768px) {
  .action-buttons {
    flex-direction: column;
  }

  .action-buttons .btn {
    margin-bottom: 0.5rem;
  }
}
</style>
