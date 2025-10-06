<template>
  <div class="audit-trail-management">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2>Audit Trail Management</h2>
      <div class="action-buttons">
        <button
          class="btn btn-outline-success me-2"
          @click="exportAuditLog"
          :disabled="loading || filteredEntries.length === 0"
        >
          <i class="bi bi-download"></i>
          Export Log
        </button>
        <button
          class="btn btn-outline-warning me-2"
          @click="showArchiveModal = true"
        >
          <i class="bi bi-archive"></i>
          Archive Old Logs
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

    <!-- Real-time Status Bar -->
    <div class="alert alert-info mb-4">
      <div class="d-flex justify-content-between align-items-center">
        <div>
          <i class="bi bi-activity me-2"></i>
          <strong>Real-time Monitoring:</strong>
          <span class="badge bg-success ms-2">{{ realtimeStatus }}</span>
          <small class="ms-2">Last update: {{ formatTime(lastUpdate) }}</small>
        </div>
        <div>
          <span class="badge bg-primary me-2"
            >{{ filteredEntries.length }} entries</span
          >
          <span class="badge bg-warning me-2">{{ todayCount }} today</span>
          <span class="badge bg-danger">{{ errorCount }} errors</span>
        </div>
      </div>
    </div>

    <!-- Statistics Cards -->
    <div class="row mb-4">
      <div class="col-md-3">
        <div class="card bg-primary text-white">
          <div class="card-body">
            <div class="d-flex justify-content-between">
              <div>
                <h4>{{ stats.totalEntries.toLocaleString() }}</h4>
                <p class="mb-0">Total Entries</p>
              </div>
              <i class="bi bi-journal-text display-4"></i>
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card bg-success text-white">
          <div class="card-body">
            <div class="d-flex justify-content-between">
              <div>
                <h4>{{ stats.uniqueUsers }}</h4>
                <p class="mb-0">Active Users</p>
              </div>
              <i class="bi bi-people display-4"></i>
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card bg-warning text-white">
          <div class="card-body">
            <div class="d-flex justify-content-between">
              <div>
                <h4>{{ stats.criticalActions }}</h4>
                <p class="mb-0">Critical Actions</p>
              </div>
              <i class="bi bi-exclamation-triangle display-4"></i>
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card bg-danger text-white">
          <div class="card-body">
            <div class="d-flex justify-content-between">
              <div>
                <h4>{{ stats.failedActions }}</h4>
                <p class="mb-0">Failed Actions</p>
              </div>
              <i class="bi bi-x-circle display-4"></i>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Advanced Filters -->
    <div class="card mb-4">
      <div class="card-header">
        <div class="d-flex justify-content-between align-items-center">
          <h5 class="mb-0">
            <i class="bi bi-funnel me-2"></i>
            Advanced Filters
          </h5>
          <button
            class="btn btn-sm btn-outline-secondary"
            @click="toggleFiltersExpanded"
          >
            <i
              :class="filtersExpanded ? 'bi-chevron-up' : 'bi-chevron-down'"
            ></i>
            {{ filtersExpanded ? "Collapse" : "Expand" }}
          </button>
        </div>
      </div>
      <div class="card-body" :class="{ 'd-none': !filtersExpanded }">
        <div class="row mb-3">
          <div class="col-md-2">
            <label class="form-label">Action Type</label>
            <select
              v-model="filters.actionType"
              class="form-select"
              @change="applyFilters"
            >
              <option value="">All Actions</option>
              <option value="authentication">Authentication</option>
              <option value="user_management">User Management</option>
              <option value="role_management">Role Management</option>
              <option value="data_access">Data Access</option>
              <option value="system_config">System Config</option>
              <option value="security">Security</option>
            </select>
          </div>
          <div class="col-md-2">
            <label class="form-label">Result Status</label>
            <select
              v-model="filters.resultStatus"
              class="form-select"
              @change="applyFilters"
            >
              <option value="">All Results</option>
              <option value="success">Success</option>
              <option value="failure">Failure</option>
              <option value="warning">Warning</option>
              <option value="error">Error</option>
            </select>
          </div>
          <div class="col-md-2">
            <label class="form-label">Severity Level</label>
            <select
              v-model="filters.severityLevel"
              class="form-select"
              @change="applyFilters"
            >
              <option value="">All Levels</option>
              <option value="low">Low</option>
              <option value="medium">Medium</option>
              <option value="high">High</option>
              <option value="critical">Critical</option>
            </select>
          </div>
          <div class="col-md-3">
            <label class="form-label">Date Range</label>
            <div class="input-group">
              <input
                v-model="filters.startDate"
                type="date"
                class="form-control"
                @change="applyFilters"
              />
              <input
                v-model="filters.endDate"
                type="date"
                class="form-control"
                @change="applyFilters"
              />
            </div>
          </div>
          <div class="col-md-3">
            <label class="form-label">Search</label>
            <div class="input-group">
              <input
                v-model="filters.search"
                class="form-control"
                placeholder="Search user, action, IP..."
                @input="debounceSearch"
              />
              <button class="btn btn-outline-secondary" @click="clearFilters">
                <i class="bi bi-x"></i>
              </button>
            </div>
          </div>
        </div>

        <div class="row">
          <div class="col-md-3">
            <label class="form-label">User</label>
            <select
              v-model="filters.userId"
              class="form-select"
              @change="applyFilters"
            >
              <option value="">All Users</option>
              <option
                v-for="user in uniqueUsers"
                :key="user.id"
                :value="user.id"
              >
                {{ user.name }} ({{ user.email }})
              </option>
            </select>
          </div>
          <div class="col-md-3">
            <label class="form-label">IP Address</label>
            <input
              v-model="filters.ipAddress"
              class="form-control"
              placeholder="e.g., 192.168.1.1"
              @input="applyFilters"
            />
          </div>
          <div class="col-md-3">
            <label class="form-label">Resource Type</label>
            <select
              v-model="filters.resourceType"
              class="form-select"
              @change="applyFilters"
            >
              <option value="">All Resources</option>
              <option value="user">User</option>
              <option value="role">Role</option>
              <option value="course">Course</option>
              <option value="enrollment">Enrollment</option>
              <option value="grade">Grade</option>
              <option value="system">System</option>
            </select>
          </div>
          <div class="col-md-3">
            <label class="form-label">Quick Filters</label>
            <div class="btn-group w-100" role="group">
              <button
                class="btn btn-sm btn-outline-primary"
                :class="{ active: quickFilter === 'today' }"
                @click="applyQuickFilter('today')"
              >
                Today
              </button>
              <button
                class="btn btn-sm btn-outline-primary"
                :class="{ active: quickFilter === 'errors' }"
                @click="applyQuickFilter('errors')"
              >
                Errors
              </button>
              <button
                class="btn btn-sm btn-outline-primary"
                :class="{ active: quickFilter === 'critical' }"
                @click="applyQuickFilter('critical')"
              >
                Critical
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Audit Entries Table -->
    <div class="card">
      <div class="card-header">
        <div class="d-flex justify-content-between align-items-center">
          <h5 class="mb-0">
            <i class="bi bi-list-ul me-2"></i>
            Audit Entries ({{ filteredEntries.length }})
          </h5>
          <div class="d-flex align-items-center">
            <label class="form-label me-2 mb-0">Per page:</label>
            <select
              v-model="pageSize"
              class="form-select form-select-sm"
              style="width: auto"
              @change="currentPage = 1"
            >
              <option value="10">10</option>
              <option value="25">25</option>
              <option value="50">50</option>
              <option value="100">100</option>
            </select>
          </div>
        </div>
      </div>
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-striped table-hover mb-0">
            <thead class="table-dark">
              <tr>
                <th width="180px">
                  <button
                    class="btn btn-sm btn-link text-white p-0 text-decoration-none"
                    @click="sortBy('timestamp')"
                  >
                    Timestamp
                    <i
                      v-if="sortField === 'timestamp'"
                      :class="
                        sortDirection === 'asc'
                          ? 'bi-arrow-up'
                          : 'bi-arrow-down'
                      "
                      class="ms-1"
                    ></i>
                  </button>
                </th>
                <th width="120px">
                  <button
                    class="btn btn-sm btn-link text-white p-0 text-decoration-none"
                    @click="sortBy('actionType')"
                  >
                    Action Type
                    <i
                      v-if="sortField === 'actionType'"
                      :class="
                        sortDirection === 'asc'
                          ? 'bi-arrow-up'
                          : 'bi-arrow-down'
                      "
                      class="ms-1"
                    ></i>
                  </button>
                </th>
                <th width="200px">User</th>
                <th>Action Details</th>
                <th width="100px">Result</th>
                <th width="80px">Severity</th>
                <th width="120px">IP Address</th>
                <th width="100px">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="entry in paginatedEntries"
                :key="entry.id"
                :class="{
                  'table-danger': entry.severityLevel === 'critical',
                  'table-warning': entry.severityLevel === 'high',
                  'table-info': entry.severityLevel === 'medium',
                }"
              >
                <td>
                  <div class="small">
                    <div>{{ formatDateTime(entry.timestamp) }}</div>
                    <span class="text-muted">{{
                      formatTime(entry.timestamp)
                    }}</span>
                  </div>
                </td>
                <td>
                  <span
                    class="badge"
                    :class="getActionTypeBadgeClass(entry.actionType)"
                  >
                    {{ entry.actionType }}
                  </span>
                </td>
                <td>
                  <div v-if="entry.user" class="user-info">
                    <div class="fw-bold">{{ entry.user.name }}</div>
                    <small class="text-muted">{{ entry.user.email }}</small>
                    <div v-if="entry.user.role" class="small">
                      <span class="badge bg-secondary">{{
                        entry.user.role
                      }}</span>
                    </div>
                  </div>
                  <span v-else class="text-muted">System</span>
                </td>
                <td>
                  <div class="action-details">
                    <div class="fw-bold">{{ entry.actionDescription }}</div>
                    <div
                      v-if="entry.resourceType && entry.resourceId"
                      class="small text-muted"
                    >
                      {{ entry.resourceType }}: {{ entry.resourceId }}
                    </div>
                    <div
                      v-if="
                        entry.details && Object.keys(entry.details).length > 0
                      "
                      class="small"
                    >
                      <button
                        class="btn btn-sm btn-link p-0"
                        @click="showDetailsModal(entry)"
                      >
                        View Details
                      </button>
                    </div>
                  </div>
                </td>
                <td>
                  <span
                    class="badge"
                    :class="getResultBadgeClass(entry.resultStatus)"
                  >
                    {{ entry.resultStatus }}
                  </span>
                </td>
                <td>
                  <span
                    class="badge"
                    :class="getSeverityBadgeClass(entry.severityLevel)"
                  >
                    {{ entry.severityLevel }}
                  </span>
                </td>
                <td>
                  <div class="small">
                    <div>{{ entry.ipAddress }}</div>
                    <div v-if="entry.userAgent" class="text-muted">
                      {{ truncateUserAgent(entry.userAgent) }}
                    </div>
                  </div>
                </td>
                <td>
                  <div class="btn-group btn-group-sm" role="group">
                    <button
                      class="btn btn-outline-primary"
                      @click="showDetailsModal(entry)"
                      title="View Details"
                    >
                      <i class="bi bi-eye"></i>
                    </button>
                    <button
                      v-if="entry.userId"
                      class="btn btn-outline-info"
                      @click="filterByUser(entry.userId)"
                      title="Filter by User"
                    >
                      <i class="bi bi-person"></i>
                    </button>
                    <button
                      v-if="entry.severityLevel === 'critical'"
                      class="btn btn-outline-danger"
                      @click="createIncidentReport(entry)"
                      title="Create Incident Report"
                    >
                      <i class="bi bi-exclamation-triangle"></i>
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

    <!-- Entry Details Modal -->
    <div
      class="modal fade"
      :class="{ show: showDetailsEntryModal }"
      :style="{ display: showDetailsEntryModal ? 'block' : 'none' }"
      tabindex="-1"
    >
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Audit Entry Details</h5>
            <button
              type="button"
              class="btn-close"
              @click="showDetailsEntryModal = false"
            ></button>
          </div>
          <div class="modal-body">
            <div v-if="selectedEntry">
              <div class="row mb-3">
                <div class="col-md-6">
                  <strong>Timestamp:</strong>
                  <div>{{ formatDateTime(selectedEntry.timestamp) }}</div>
                </div>
                <div class="col-md-6">
                  <strong>Action Type:</strong>
                  <span
                    class="badge ms-2"
                    :class="getActionTypeBadgeClass(selectedEntry.actionType)"
                  >
                    {{ selectedEntry.actionType }}
                  </span>
                </div>
              </div>

              <div class="row mb-3" v-if="selectedEntry.user">
                <div class="col-md-12">
                  <strong>User:</strong>
                  <div>
                    {{ selectedEntry.user.name }} ({{
                      selectedEntry.user.email
                    }})
                  </div>
                  <div v-if="selectedEntry.user.role">
                    <span class="badge bg-secondary">{{
                      selectedEntry.user.role
                    }}</span>
                  </div>
                </div>
              </div>

              <div class="row mb-3">
                <div class="col-md-12">
                  <strong>Action Description:</strong>
                  <div>{{ selectedEntry.actionDescription }}</div>
                </div>
              </div>

              <div class="row mb-3" v-if="selectedEntry.resourceType">
                <div class="col-md-6">
                  <strong>Resource Type:</strong>
                  <div>{{ selectedEntry.resourceType }}</div>
                </div>
                <div class="col-md-6" v-if="selectedEntry.resourceId">
                  <strong>Resource ID:</strong>
                  <div>{{ selectedEntry.resourceId }}</div>
                </div>
              </div>

              <div class="row mb-3">
                <div class="col-md-4">
                  <strong>Result:</strong>
                  <span
                    class="badge ms-2"
                    :class="getResultBadgeClass(selectedEntry.resultStatus)"
                  >
                    {{ selectedEntry.resultStatus }}
                  </span>
                </div>
                <div class="col-md-4">
                  <strong>Severity:</strong>
                  <span
                    class="badge ms-2"
                    :class="getSeverityBadgeClass(selectedEntry.severityLevel)"
                  >
                    {{ selectedEntry.severityLevel }}
                  </span>
                </div>
                <div class="col-md-4">
                  <strong>IP Address:</strong>
                  <div>{{ selectedEntry.ipAddress }}</div>
                </div>
              </div>

              <div v-if="selectedEntry.userAgent" class="mb-3">
                <strong>User Agent:</strong>
                <div class="small text-muted">
                  {{ selectedEntry.userAgent }}
                </div>
              </div>

              <div
                v-if="
                  selectedEntry.details &&
                  Object.keys(selectedEntry.details).length > 0
                "
                class="mb-3"
              >
                <strong>Additional Details:</strong>
                <pre class="bg-light p-3 rounded small">{{
                  JSON.stringify(selectedEntry.details, null, 2)
                }}</pre>
              </div>

              <div v-if="selectedEntry.errorMessage" class="mb-3">
                <strong>Error Message:</strong>
                <div class="alert alert-danger">
                  {{ selectedEntry.errorMessage }}
                </div>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button
              type="button"
              class="btn btn-secondary"
              @click="showDetailsEntryModal = false"
            >
              Close
            </button>
            <button
              v-if="selectedEntry?.userId"
              type="button"
              class="btn btn-primary"
              @click="
                filterByUser(selectedEntry.userId);
                showDetailsEntryModal = false;
              "
            >
              Filter by This User
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
      v-if="showDetailsEntryModal || showArchiveModal"
      class="modal-backdrop fade show"
    ></div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, reactive, onMounted, onUnmounted } from "vue";
import { AdminApiService } from "@/services/AdminApiService";
import type { AuditEntry } from "@/types";

// Reactive state
const loading = ref(false);
const auditEntries = ref<AuditEntry[]>([]);
const selectedEntry = ref<AuditEntry | null>(null);
const currentPage = ref(1);
const pageSize = ref(25);
const filtersExpanded = ref(false);
const quickFilter = ref("");
const realtimeStatus = ref("Connected");
const lastUpdate = ref(new Date());

// Modal visibility
const showDetailsEntryModal = ref(false);
const showArchiveModal = ref(false);

// Sorting
const sortField = ref("timestamp");
const sortDirection = ref("desc");

// Filters
const filters = reactive({
  actionType: "",
  resultStatus: "",
  severityLevel: "",
  startDate: "",
  endDate: "",
  search: "",
  userId: "",
  ipAddress: "",
  resourceType: "",
});

// Search debounce
let searchTimeout: NodeJS.Timeout | null = null;

// Real-time updates
let realtimeInterval: NodeJS.Timeout | null = null;

// Computed properties
const filteredEntries = computed(() => {
  let filtered = auditEntries.value;

  if (filters.actionType) {
    filtered = filtered.filter(
      (entry: AuditEntry) => entry.action === filters.actionType
    );
  }

  if (filters.resultStatus) {
    filtered = filtered.filter(
      (entry: AuditEntry) => entry.outcome === filters.resultStatus
    );
  }

  if (filters.severityLevel) {
    filtered = filtered.filter(
      (entry: AuditEntry) => entry.risk.level === filters.severityLevel
    );
  }

  if (filters.startDate) {
    const startDate = new Date(filters.startDate);
    filtered = filtered.filter(
      (entry: AuditEntry) => new Date(entry.timestamp) >= startDate
    );
  }

  if (filters.endDate) {
    const endDate = new Date(filters.endDate);
    endDate.setHours(23, 59, 59, 999);
    filtered = filtered.filter(
      (entry: AuditEntry) => new Date(entry.timestamp) <= endDate
    );
  }

  if (filters.userId) {
    filtered = filtered.filter(
      (entry: AuditEntry) => entry.userId === filters.userId
    );
  }

  if (filters.ipAddress) {
    filtered = filtered.filter(
      (entry: AuditEntry) =>
        entry.details.ipAddress &&
        entry.details.ipAddress.includes(filters.ipAddress)
    );
  }

  if (filters.resourceType) {
    filtered = filtered.filter(
      (entry: AuditEntry) => entry.resource === filters.resourceType
    );
  }

  if (filters.search) {
    const search = filters.search.toLowerCase();
    filtered = filtered.filter(
      (entry: AuditEntry) =>
        entry.action.toLowerCase().includes(search) ||
        entry.userId.toLowerCase().includes(search) ||
        (entry.details.ipAddress && entry.details.ipAddress.includes(search)) ||
        entry.resource.toLowerCase().includes(search)
    );
  }

  // Apply sorting
  filtered.sort((a: AuditEntry, b: AuditEntry) => {
    if (sortField.value === "timestamp") {
      const aTime = new Date(a.timestamp).getTime();
      const bTime = new Date(b.timestamp).getTime();
      if (aTime < bTime) return sortDirection.value === "asc" ? -1 : 1;
      if (aTime > bTime) return sortDirection.value === "asc" ? 1 : -1;
      return 0;
    }

    const aVal = a[sortField.value as keyof AuditEntry];
    const bVal = b[sortField.value as keyof AuditEntry];

    if (aVal && bVal) {
      if (aVal < bVal) return sortDirection.value === "asc" ? -1 : 1;
      if (aVal > bVal) return sortDirection.value === "asc" ? 1 : -1;
    }
    return 0;
  });

  return filtered;
});

const totalPages = computed(() => {
  return Math.ceil(filteredEntries.value.length / pageSize.value);
});

const paginatedEntries = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value;
  const end = start + pageSize.value;
  return filteredEntries.value.slice(start, end);
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

const uniqueUsers = computed(() => {
  const users = new Map();
  auditEntries.value.forEach((entry: AuditEntry) => {
    if (entry.userId && !users.has(entry.userId)) {
      users.set(entry.userId, {
        id: entry.userId,
        name: entry.userId, // Using userId as name since user object doesn't exist
        email: entry.userId + "@university.edu", // Placeholder email
      });
    }
  });
  return Array.from(users.values());
});

const stats = computed(() => {
  const today = new Date();
  today.setHours(0, 0, 0, 0);

  return {
    totalEntries: auditEntries.value.length,
    uniqueUsers: uniqueUsers.value.length,
    criticalActions: auditEntries.value.filter(
      (e: AuditEntry) => e.risk.level === "critical"
    ).length,
    failedActions: auditEntries.value.filter(
      (e: AuditEntry) => e.outcome === "failure"
    ).length,
  };
});

const todayCount = computed(() => {
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  return auditEntries.value.filter(
    (entry) => new Date(entry.timestamp) >= today
  ).length;
});

const errorCount = computed(() => {
  return auditEntries.value.filter(
    (entry: AuditEntry) => entry.outcome === "failure"
  ).length;
});

// Methods
const loadAuditEntries = async () => {
  loading.value = true;
  try {
    const response = await AdminApiService.audit.getTrail({
      limit: 10000, // Load all for client-side filtering
      sortBy: "timestamp",
      sortOrder: "desc",
    });

    auditEntries.value = response.data || [];
    lastUpdate.value = new Date();
  } catch (error) {
    console.error("Failed to load audit entries:", error);
    realtimeStatus.value = "Disconnected";
  } finally {
    loading.value = false;
  }
};

const showDetailsModal = (entry: AuditEntry) => {
  selectedEntry.value = entry;
  showDetailsEntryModal.value = true;
};

const sortBy = (field: string) => {
  if (sortField.value === field) {
    sortDirection.value = sortDirection.value === "asc" ? "desc" : "asc";
  } else {
    sortField.value = field;
    sortDirection.value = "desc";
  }
};

const applyFilters = () => {
  currentPage.value = 1;
  quickFilter.value = "";
};

const applyQuickFilter = (filter: string) => {
  quickFilter.value = filter;

  // Clear existing filters
  Object.assign(filters, {
    actionType: "",
    resultStatus: "",
    severityLevel: "",
    startDate: "",
    endDate: "",
    search: "",
    userId: "",
    ipAddress: "",
    resourceType: "",
  });

  const today = new Date();

  switch (filter) {
    case "today":
      filters.startDate = today.toISOString().split("T")[0];
      filters.endDate = today.toISOString().split("T")[0];
      break;
    case "errors":
      filters.resultStatus = "error";
      break;
    case "critical":
      filters.severityLevel = "critical";
      break;
  }

  currentPage.value = 1;
};

const filterByUser = (userId: string) => {
  filters.userId = userId;
  applyFilters();
};

const clearFilters = () => {
  Object.assign(filters, {
    actionType: "",
    resultStatus: "",
    severityLevel: "",
    startDate: "",
    endDate: "",
    search: "",
    userId: "",
    ipAddress: "",
    resourceType: "",
  });
  quickFilter.value = "";
  currentPage.value = 1;
};

const debounceSearch = () => {
  if (searchTimeout) {
    clearTimeout(searchTimeout);
  }
  searchTimeout = setTimeout(() => {
    applyFilters();
  }, 300);
};

const toggleFiltersExpanded = () => {
  filtersExpanded.value = !filtersExpanded.value;
};

const exportAuditLog = async () => {
  try {
    const response = await AdminApiService.audit.exportAuditLog({
      format: "csv",
      filters: filters,
      entries: filteredEntries.value.map((e: AuditEntry) => e.id),
    });

    // Create download link
    const blob = new Blob([response.data], { type: "text/csv" });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = `audit-log-${new Date().toISOString().split("T")[0]}.csv`;
    link.click();
    window.URL.revokeObjectURL(url);
  } catch (error) {
    console.error("Failed to export audit log:", error);
  }
};

const createIncidentReport = (entry: AuditEntry) => {
  // This would open a modal to create an incident report
  console.log("Create incident report for:", entry);
};

const refreshData = async () => {
  await loadAuditEntries();
};

// Badge class helpers
const getActionTypeBadgeClass = (actionType: string) => {
  const classes = {
    authentication: "bg-primary",
    user_management: "bg-info",
    role_management: "bg-warning",
    data_access: "bg-success",
    system_config: "bg-secondary",
    security: "bg-danger",
  };
  return classes[actionType as keyof typeof classes] || "bg-secondary";
};

const getResultBadgeClass = (resultStatus: string) => {
  const classes = {
    success: "bg-success",
    failure: "bg-danger",
    warning: "bg-warning",
    error: "bg-danger",
  };
  return classes[resultStatus as keyof typeof classes] || "bg-secondary";
};

const getSeverityBadgeClass = (severityLevel: string) => {
  const classes = {
    low: "bg-secondary",
    medium: "bg-info",
    high: "bg-warning",
    critical: "bg-danger",
  };
  return classes[severityLevel as keyof typeof classes] || "bg-secondary";
};

// Utility functions
const formatDateTime = (date: string | Date) => {
  return new Date(date).toLocaleString();
};

const formatTime = (date: string | Date) => {
  return new Date(date).toLocaleTimeString();
};

const truncateUserAgent = (userAgent: string) => {
  return userAgent.length > 50 ? userAgent.substring(0, 50) + "..." : userAgent;
};

// Real-time updates
const startRealtimeUpdates = () => {
  realtimeInterval = setInterval(async () => {
    try {
      await loadAuditEntries();
      realtimeStatus.value = "Connected";
    } catch (error) {
      realtimeStatus.value = "Disconnected";
    }
  }, 30000); // Update every 30 seconds
};

const stopRealtimeUpdates = () => {
  if (realtimeInterval) {
    clearInterval(realtimeInterval);
    realtimeInterval = null;
  }
};

// Lifecycle hooks
onMounted(async () => {
  await refreshData();
  startRealtimeUpdates();
});

onUnmounted(() => {
  stopRealtimeUpdates();
  if (searchTimeout) {
    clearTimeout(searchTimeout);
  }
});
</script>

<style scoped>
.audit-trail-management {
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
  min-width: 150px;
}

.action-details {
  min-width: 200px;
}

.table-responsive {
  max-height: 700px;
  overflow-y: auto;
}

.btn-group-sm .btn {
  padding: 0.25rem 0.5rem;
}

.pagination {
  justify-content: center;
}

.btn-link {
  text-decoration: none !important;
}

.btn-link:hover {
  text-decoration: underline !important;
}

pre {
  white-space: pre-wrap;
  word-wrap: break-word;
  max-height: 300px;
  overflow-y: auto;
}

.statistics-row .card {
  transition: transform 0.2s;
}

.statistics-row .card:hover {
  transform: translateY(-2px);
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

  .card-body .row .col-md-2,
  .card-body .row .col-md-3 {
    margin-bottom: 1rem;
  }
}
</style>
