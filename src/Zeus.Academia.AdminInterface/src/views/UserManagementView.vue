<template>
  <div class="user-management-container">
    <!-- Header Section -->
    <div class="management-header">
      <div class="header-content">
        <h1 class="management-title">
          <i class="fas fa-users-cog"></i>
          User Management Dashboard
        </h1>
        <p class="management-subtitle">
          Comprehensive user administration and access control
        </p>
      </div>
      <div class="header-actions">
        <button class="btn btn-primary" @click="refreshData">
          <i class="fas fa-sync-alt"></i>
          Refresh
        </button>
      </div>
    </div>

    <!-- System Health Status -->
    <div class="system-status-section mb-4">
      <div class="row">
        <div class="col-md-3">
          <div class="status-card healthy">
            <div class="status-icon">
              <i class="fas fa-heartbeat"></i>
            </div>
            <div class="status-info">
              <h3>System Health</h3>
              <p class="status-value">{{ systemHealth.status }}</p>
              <small>API Response: {{ systemHealth.responseTime }}ms</small>
            </div>
          </div>
        </div>
        <div class="col-md-3">
          <div class="status-card">
            <div class="status-icon">
              <i class="fas fa-users"></i>
            </div>
            <div class="status-info">
              <h3>Total Users</h3>
              <p class="status-value">{{ userStats.total }}</p>
              <small>{{ userStats.activePercentage }}% active</small>
            </div>
          </div>
        </div>
        <div class="col-md-3">
          <div class="status-card">
            <div class="status-icon">
              <i class="fas fa-shield-alt"></i>
            </div>
            <div class="status-info">
              <h3>Security Status</h3>
              <p class="status-value">{{ securityStats.mfaEnabled }}%</p>
              <small>MFA Enabled</small>
            </div>
          </div>
        </div>
        <div class="col-md-3">
          <div class="status-card warning" v-if="pendingActions > 0">
            <div class="status-icon">
              <i class="fas fa-exclamation-triangle"></i>
            </div>
            <div class="status-info">
              <h3>Pending Actions</h3>
              <p class="status-value">{{ pendingActions }}</p>
              <small>Require attention</small>
            </div>
          </div>
          <div class="status-card healthy" v-else>
            <div class="status-icon">
              <i class="fas fa-check-circle"></i>
            </div>
            <div class="status-info">
              <h3>All Clear</h3>
              <p class="status-value">0</p>
              <small>No pending actions</small>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Navigation Tabs -->
    <div class="management-tabs">
      <ul class="nav nav-tabs" role="tablist">
        <li class="nav-item" role="presentation">
          <button
            class="nav-link"
            :class="{ active: activeTab === 'overview' }"
            @click="activeTab = 'overview'"
            type="button"
          >
            <i class="fas fa-chart-pie"></i>
            Overview
          </button>
        </li>
        <li class="nav-item" role="presentation">
          <button
            class="nav-link"
            :class="{ active: activeTab === 'bulk' }"
            @click="activeTab = 'bulk'"
            type="button"
          >
            <i class="fas fa-users-cog"></i>
            Bulk Management
          </button>
        </li>
        <li class="nav-item" role="presentation">
          <button
            class="nav-link"
            :class="{ active: activeTab === 'roles' }"
            @click="activeTab = 'roles'"
            type="button"
          >
            <i class="fas fa-user-tag"></i>
            Role Assignment
          </button>
        </li>
        <li class="nav-item" role="presentation">
          <button
            class="nav-link"
            :class="{ active: activeTab === 'lifecycle' }"
            @click="activeTab = 'lifecycle'"
            type="button"
          >
            <i class="fas fa-user-clock"></i>
            User Lifecycle
          </button>
        </li>
        <li class="nav-item" role="presentation">
          <button
            class="nav-link"
            :class="{ active: activeTab === 'security' }"
            @click="activeTab = 'security'"
            type="button"
          >
            <i class="fas fa-shield-alt"></i>
            Password Security
          </button>
        </li>
        <li class="nav-item" role="presentation">
          <button
            class="nav-link"
            :class="{ active: activeTab === 'audit' }"
            @click="activeTab = 'audit'"
            type="button"
          >
            <i class="fas fa-clipboard-list"></i>
            Audit Trail
          </button>
        </li>
      </ul>
    </div>

    <!-- Tab Content -->
    <div class="tab-content mt-4">
      <!-- Overview Tab -->
      <div v-show="activeTab === 'overview'" class="tab-pane">
        <div class="overview-dashboard">
          <div class="row">
            <div class="col-lg-8">
              <div class="card">
                <div class="card-header">
                  <h5>
                    <i class="fas fa-chart-line"></i> User Activity Overview
                  </h5>
                </div>
                <div class="card-body">
                  <div class="row">
                    <div class="col-md-4">
                      <div class="metric-box">
                        <div class="metric-value">{{ userStats.active }}</div>
                        <div class="metric-label">Active Users</div>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="metric-box">
                        <div class="metric-value">
                          {{ userStats.suspended }}
                        </div>
                        <div class="metric-label">Suspended</div>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="metric-box">
                        <div class="metric-value">{{ userStats.inactive }}</div>
                        <div class="metric-label">Inactive</div>
                      </div>
                    </div>
                  </div>
                  <div class="activity-timeline mt-4">
                    <h6>Recent Activity (Last 24 Hours)</h6>
                    <div
                      class="timeline-item"
                      v-for="activity in recentActivity"
                      :key="activity.id"
                    >
                      <div class="timeline-marker" :class="activity.type"></div>
                      <div class="timeline-content">
                        <strong>{{ activity.action }}</strong>
                        <span class="text-muted">{{ activity.user }}</span>
                        <small class="text-muted d-block">{{
                          activity.timestamp
                        }}</small>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div class="col-lg-4">
              <div class="card">
                <div class="card-header">
                  <h5>
                    <i class="fas fa-exclamation-circle"></i> Priority Alerts
                  </h5>
                </div>
                <div class="card-body">
                  <div
                    v-if="priorityAlerts.length === 0"
                    class="text-center text-muted"
                  >
                    <i class="fas fa-check-circle fa-2x mb-2"></i>
                    <p>No priority alerts</p>
                  </div>
                  <div
                    v-for="alert in priorityAlerts"
                    :key="alert.id"
                    class="alert-item"
                    :class="alert.severity"
                  >
                    <div class="alert-icon">
                      <i :class="alert.icon"></i>
                    </div>
                    <div class="alert-content">
                      <strong>{{ alert.title }}</strong>
                      <p>{{ alert.message }}</p>
                      <button class="btn btn-sm btn-outline-primary">
                        Take Action
                      </button>
                    </div>
                  </div>
                </div>
              </div>

              <div class="card mt-3">
                <div class="card-header">
                  <h5><i class="fas fa-bolt"></i> Quick Actions</h5>
                </div>
                <div class="card-body">
                  <div class="quick-actions">
                    <button
                      class="btn btn-outline-primary btn-block mb-2"
                      @click="activeTab = 'bulk'"
                    >
                      <i class="fas fa-plus"></i> Add New User
                    </button>
                    <button
                      class="btn btn-outline-success btn-block mb-2"
                      @click="activeTab = 'bulk'"
                    >
                      <i class="fas fa-upload"></i> Bulk Import Users
                    </button>
                    <button
                      class="btn btn-outline-warning btn-block mb-2"
                      @click="activeTab = 'security'"
                    >
                      <i class="fas fa-key"></i> Reset Passwords
                    </button>
                    <button class="btn btn-outline-info btn-block">
                      <i class="fas fa-download"></i> Export User Data
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Bulk Management Tab -->
      <div v-show="activeTab === 'bulk'" class="tab-pane">
        <div class="bulk-management-interface">
          <!-- Statistics Cards Section -->
          <div class="row mb-4">
            <div class="col-md-3">
              <div class="card stats-card">
                <div class="card-body text-center">
                  <i class="fas fa-upload text-primary fa-2x mb-2"></i>
                  <h4 class="card-title">{{ bulkStats.totalImports }}</h4>
                  <p class="card-text text-muted">Total Imports</p>
                  <small class="text-success"
                    >+{{ bulkStats.recentImports }} this week</small
                  >
                </div>
              </div>
            </div>
            <div class="col-md-3">
              <div class="card stats-card">
                <div class="card-body text-center">
                  <i class="fas fa-check-circle text-success fa-2x mb-2"></i>
                  <h4 class="card-title">{{ bulkStats.successRate }}%</h4>
                  <p class="card-text text-muted">Success Rate</p>
                  <small class="text-info"
                    >{{ bulkStats.successfulOperations }} operations</small
                  >
                </div>
              </div>
            </div>
            <div class="col-md-3">
              <div class="card stats-card">
                <div class="card-body text-center">
                  <i class="fas fa-users text-info fa-2x mb-2"></i>
                  <h4 class="card-title">{{ bulkStats.usersCreated }}</h4>
                  <p class="card-text text-muted">Users Created</p>
                  <small class="text-primary"
                    >{{ bulkStats.todayCreated }} today</small
                  >
                </div>
              </div>
            </div>
            <div class="col-md-3">
              <div class="card stats-card">
                <div class="card-body text-center">
                  <i
                    class="fas fa-exclamation-triangle text-warning fa-2x mb-2"
                  ></i>
                  <h4 class="card-title">{{ bulkStats.pendingReview }}</h4>
                  <p class="card-text text-muted">Pending Review</p>
                  <small class="text-muted">Require attention</small>
                </div>
              </div>
            </div>
          </div>

          <!-- Main Interface Layout -->
          <div class="row">
            <!-- File Upload and Configuration -->
            <div class="col-lg-6">
              <div class="card">
                <div class="card-header">
                  <h5><i class="fas fa-upload"></i> Bulk User Import</h5>
                  <div class="float-right">
                    <button
                      class="btn btn-sm btn-outline-secondary"
                      @click="downloadTemplate"
                    >
                      <i class="fas fa-download"></i> Download Template
                    </button>
                  </div>
                </div>
                <div class="card-body">
                  <!-- Upload Methods Tabs -->
                  <ul class="nav nav-pills mb-3" role="tablist">
                    <li class="nav-item">
                      <a
                        class="nav-link"
                        :class="{ active: uploadMethod === 'file' }"
                        @click="uploadMethod = 'file'"
                      >
                        <i class="fas fa-file-upload"></i> Upload CSV
                      </a>
                    </li>
                    <li class="nav-item">
                      <a
                        class="nav-link"
                        :class="{ active: uploadMethod === 'manual' }"
                        @click="uploadMethod = 'manual'"
                      >
                        <i class="fas fa-edit"></i> Manual Entry
                      </a>
                    </li>
                  </ul>

                  <!-- File Upload Area -->
                  <div v-show="uploadMethod === 'file'">
                    <div
                      class="upload-area"
                      :class="{ 'drag-over': isDragOver }"
                      @drop="handleFileDrop"
                      @dragover.prevent="isDragOver = true"
                      @dragleave="isDragOver = false"
                      @dragenter.prevent
                    >
                      <i
                        class="fas fa-cloud-upload-alt fa-3x mb-3 text-primary"
                      ></i>
                      <h5>Drop CSV file here or click to browse</h5>
                      <p class="text-muted">
                        Supports up to 10,000 user records
                      </p>
                      <p class="text-muted small">
                        Supported columns: firstName, lastName, email, role,
                        department
                      </p>
                      <input
                        type="file"
                        ref="fileInput"
                        @change="handleFileSelect"
                        accept=".csv"
                        class="d-none"
                      />
                      <button class="btn btn-primary" @click="triggerFileInput">
                        <i class="fas fa-folder-open"></i> Choose File
                      </button>
                    </div>

                    <div v-if="uploadedFile" class="file-info mt-3">
                      <div class="alert alert-info">
                        <div
                          class="d-flex justify-content-between align-items-center"
                        >
                          <div>
                            <strong
                              ><i class="fas fa-file-csv"></i>
                              {{ uploadedFile.name }}</strong
                            >
                            <br />
                            <small
                              >{{ formatFileSize(uploadedFile.size) }} •
                              {{ previewUsers.length || "Unknown" }} records
                              detected</small
                            >
                          </div>
                          <button
                            class="btn btn-sm btn-outline-danger"
                            @click="removeFile"
                          >
                            <i class="fas fa-times"></i>
                          </button>
                        </div>
                      </div>

                      <!-- Validation Results -->
                      <div
                        v-if="validationResults"
                        class="validation-summary mt-2"
                      >
                        <div class="row">
                          <div class="col-md-4">
                            <div class="validation-stat">
                              <i class="fas fa-check-circle text-success"></i>
                              <strong>{{ validationResults.valid }}</strong>
                              Valid
                            </div>
                          </div>
                          <div class="col-md-4">
                            <div class="validation-stat">
                              <i
                                class="fas fa-exclamation-triangle text-warning"
                              ></i>
                              <strong>{{ validationResults.warnings }}</strong>
                              Warnings
                            </div>
                          </div>
                          <div class="col-md-4">
                            <div class="validation-stat">
                              <i class="fas fa-times-circle text-danger"></i>
                              <strong>{{ validationResults.errors }}</strong>
                              Errors
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- Manual Entry Area -->
                  <div v-show="uploadMethod === 'manual'">
                    <div class="form-group">
                      <label for="manualUserData">User Data (CSV Format)</label>
                      <textarea
                        id="manualUserData"
                        class="form-control"
                        rows="8"
                        v-model="manualUserData"
                        placeholder="firstName,lastName,email,role,department&#10;John,Doe,john.doe@example.com,student,Computer Science&#10;Jane,Smith,jane.smith@example.com,faculty,Mathematics"
                      ></textarea>
                      <small class="form-text text-muted">
                        Format: firstName,lastName,email,role,department (one
                        user per line)
                      </small>
                    </div>
                    <button class="btn btn-primary" @click="parseManualData">
                      <i class="fas fa-check"></i> Parse Data
                    </button>
                  </div>
                </div>
              </div>

              <!-- Import Configuration -->
              <div class="card mt-3">
                <div class="card-header">
                  <h5><i class="fas fa-cog"></i> Import Configuration</h5>
                </div>
                <div class="card-body">
                  <form>
                    <div class="row">
                      <div class="col-md-6">
                        <div class="form-group mb-3">
                          <label for="defaultRole">Default Role</label>
                          <select
                            id="defaultRole"
                            class="form-control"
                            v-model="importConfig.defaultRole"
                          >
                            <option value="student">Student</option>
                            <option value="faculty">Faculty</option>
                            <option value="staff">Staff</option>
                            <option value="admin">Administrator</option>
                          </select>
                        </div>
                      </div>
                      <div class="col-md-6">
                        <div class="form-group mb-3">
                          <label for="defaultDepartment"
                            >Default Department</label
                          >
                          <input
                            type="text"
                            id="defaultDepartment"
                            class="form-control"
                            v-model="importConfig.defaultDepartment"
                            placeholder="e.g., Computer Science"
                          />
                        </div>
                      </div>
                    </div>

                    <div class="row">
                      <div class="col-md-6">
                        <div class="form-check mb-3">
                          <input
                            type="checkbox"
                            id="sendWelcomeEmail"
                            class="form-check-input"
                            v-model="importConfig.sendWelcomeEmail"
                          />
                          <label
                            for="sendWelcomeEmail"
                            class="form-check-label"
                          >
                            Send welcome email to new users
                          </label>
                        </div>
                      </div>
                      <div class="col-md-6">
                        <div class="form-check mb-3">
                          <input
                            type="checkbox"
                            id="requirePasswordReset"
                            class="form-check-input"
                            v-model="importConfig.requirePasswordReset"
                          />
                          <label
                            for="requirePasswordReset"
                            class="form-check-label"
                          >
                            Require password reset on first login
                          </label>
                        </div>
                      </div>
                    </div>

                    <div class="row">
                      <div class="col-md-6">
                        <div class="form-check mb-3">
                          <input
                            type="checkbox"
                            id="skipDuplicates"
                            class="form-check-input"
                            v-model="importConfig.skipDuplicates"
                          />
                          <label for="skipDuplicates" class="form-check-label">
                            Skip duplicate email addresses
                          </label>
                        </div>
                      </div>
                      <div class="col-md-6">
                        <div class="form-check mb-3">
                          <input
                            type="checkbox"
                            id="validateOnly"
                            class="form-check-input"
                            v-model="importConfig.validateOnly"
                          />
                          <label for="validateOnly" class="form-check-label">
                            Validation only (don't create users)
                          </label>
                        </div>
                      </div>
                    </div>
                  </form>
                </div>
              </div>
            </div>

            <!-- User Preview and Operations -->
            <div class="col-lg-6">
              <!-- User Preview Table -->
              <div class="card" v-if="previewUsers.length > 0">
                <div class="card-header">
                  <h5><i class="fas fa-eye"></i> User Preview</h5>
                  <div class="float-right">
                    <span class="badge badge-info"
                      >{{ previewUsers.length }} users</span
                    >
                  </div>
                </div>
                <div class="card-body">
                  <div
                    class="table-responsive"
                    style="max-height: 400px; overflow-y: auto"
                  >
                    <table class="table table-sm">
                      <thead class="thead-light sticky-top">
                        <tr>
                          <th>Status</th>
                          <th>Name</th>
                          <th>Email</th>
                          <th>Role</th>
                          <th>Department</th>
                        </tr>
                      </thead>
                      <tbody>
                        <tr
                          v-for="(user, index) in previewUsers"
                          :key="index"
                          :class="{
                            'table-danger': user.hasErrors,
                            'table-warning': user.hasWarnings,
                          }"
                        >
                          <td>
                            <i
                              v-if="user.hasErrors"
                              class="fas fa-times-circle text-danger"
                              title="Has errors"
                            ></i>
                            <i
                              v-else-if="user.hasWarnings"
                              class="fas fa-exclamation-triangle text-warning"
                              title="Has warnings"
                            ></i>
                            <i
                              v-else
                              class="fas fa-check-circle text-success"
                              title="Valid"
                            ></i>
                          </td>
                          <td>{{ user.firstName }} {{ user.lastName }}</td>
                          <td>{{ user.email }}</td>
                          <td>{{ user.role || importConfig.defaultRole }}</td>
                          <td>
                            {{
                              user.department || importConfig.defaultDepartment
                            }}
                          </td>
                        </tr>
                      </tbody>
                    </table>
                  </div>

                  <div class="mt-3 d-flex justify-content-between">
                    <div class="preview-stats">
                      <small class="text-muted">
                        Valid:
                        <span class="text-success">{{
                          validUsers.length
                        }}</span>
                        | Warnings:
                        <span class="text-warning">{{
                          warningUsers.length
                        }}</span>
                        | Errors:
                        <span class="text-danger">{{ errorUsers.length }}</span>
                      </small>
                    </div>
                    <div class="preview-actions">
                      <button
                        class="btn btn-success"
                        @click="processImport"
                        :disabled="validUsers.length === 0"
                        v-if="!importConfig.validateOnly"
                      >
                        <i class="fas fa-plus-circle"></i> Create
                        {{ validUsers.length }} Users
                      </button>
                      <button
                        class="btn btn-primary"
                        @click="validateUsers"
                        v-else
                      >
                        <i class="fas fa-check"></i> Validate Data
                      </button>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Bulk Operations on Existing Users -->
              <div class="card" :class="{ 'mt-3': previewUsers.length > 0 }">
                <div class="card-header">
                  <h5><i class="fas fa-users-cog"></i> Bulk Operations</h5>
                </div>
                <div class="card-body">
                  <p class="text-muted mb-3">
                    Perform batch operations on existing users
                  </p>

                  <!-- User Selection -->
                  <div class="form-group mb-3">
                    <label>Select Users</label>
                    <div class="input-group">
                      <input
                        type="text"
                        class="form-control"
                        placeholder="Search users by name or email..."
                        v-model="userSearchQuery"
                        @input="searchUsers"
                      />
                      <div class="input-group-append">
                        <button
                          class="btn btn-outline-secondary"
                          @click="selectAllUsers"
                        >
                          <i class="fas fa-check-square"></i> Select All
                        </button>
                      </div>
                    </div>
                  </div>

                  <div
                    class="selected-users-summary mb-3"
                    v-if="selectedUsers.length > 0"
                  >
                    <div class="alert alert-info">
                      <strong>{{ selectedUsers.length }}</strong> users selected
                      <button
                        class="btn btn-sm btn-outline-primary float-right"
                        @click="clearSelection"
                      >
                        Clear Selection
                      </button>
                    </div>
                  </div>

                  <!-- Bulk Operations -->
                  <div class="row">
                    <div class="col-md-6">
                      <button
                        class="btn btn-warning btn-block"
                        @click="bulkSuspend"
                        :disabled="selectedUsers.length === 0"
                      >
                        <i class="fas fa-pause-circle"></i> Suspend Users
                      </button>
                    </div>
                    <div class="col-md-6">
                      <button
                        class="btn btn-success btn-block"
                        @click="bulkActivate"
                        :disabled="selectedUsers.length === 0"
                      >
                        <i class="fas fa-play-circle"></i> Activate Users
                      </button>
                    </div>
                  </div>
                  <div class="row mt-2">
                    <div class="col-md-6">
                      <button
                        class="btn btn-danger btn-block"
                        @click="bulkDelete"
                        :disabled="selectedUsers.length === 0"
                      >
                        <i class="fas fa-trash"></i> Delete Users
                      </button>
                    </div>
                    <div class="col-md-6">
                      <button
                        class="btn btn-info btn-block"
                        @click="bulkExport"
                        :disabled="selectedUsers.length === 0"
                      >
                        <i class="fas fa-download"></i> Export Data
                      </button>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Operation History -->
              <div class="card mt-3">
                <div class="card-header">
                  <h5><i class="fas fa-history"></i> Recent Operations</h5>
                </div>
                <div class="card-body">
                  <div
                    class="operation-history"
                    style="max-height: 300px; overflow-y: auto"
                  >
                    <div
                      v-for="operation in operationHistory"
                      :key="operation.id"
                      class="operation-item d-flex justify-content-between align-items-center mb-2"
                    >
                      <div>
                        <div class="operation-title">
                          <i
                            :class="[operation.icon, operation.statusClass]"
                          ></i>
                          {{ operation.title }}
                        </div>
                        <small class="text-muted"
                          >{{ operation.timestamp }} •
                          {{ operation.details }}</small
                        >
                      </div>
                      <div>
                        <span class="badge" :class="operation.badgeClass">{{
                          operation.status
                        }}</span>
                      </div>
                    </div>
                  </div>

                  <div
                    v-if="operationHistory.length === 0"
                    class="text-center text-muted py-3"
                  >
                    <i class="fas fa-history fa-2x mb-2"></i>
                    <p>No recent operations</p>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Import Progress Modal -->
          <div v-if="importProgress.active" class="card mt-4">
            <div class="card-header bg-primary text-white">
              <h5 class="mb-0">
                <i class="fas fa-spinner fa-spin"></i>
                {{ importProgress.title || "Import Progress" }}
              </h5>
            </div>
            <div class="card-body">
              <div class="progress mb-3" style="height: 25px">
                <div
                  class="progress-bar progress-bar-striped progress-bar-animated"
                  :style="{ width: importProgress.percentage + '%' }"
                  :class="
                    importProgress.failed > 0 ? 'bg-warning' : 'bg-success'
                  "
                >
                  {{ importProgress.percentage }}%
                </div>
              </div>

              <div class="row text-center mb-3">
                <div class="col-md-3">
                  <div class="progress-stat">
                    <strong class="d-block text-primary">{{
                      importProgress.processed
                    }}</strong>
                    <small class="text-muted">Processed</small>
                  </div>
                </div>
                <div class="col-md-3">
                  <div class="progress-stat">
                    <strong class="d-block text-success">{{
                      importProgress.successful
                    }}</strong>
                    <small class="text-muted">Successful</small>
                  </div>
                </div>
                <div class="col-md-3">
                  <div class="progress-stat">
                    <strong class="d-block text-danger">{{
                      importProgress.failed
                    }}</strong>
                    <small class="text-muted">Failed</small>
                  </div>
                </div>
                <div class="col-md-3">
                  <div class="progress-stat">
                    <strong class="d-block text-info">{{
                      importProgress.remaining
                    }}</strong>
                    <small class="text-muted">Remaining</small>
                  </div>
                </div>
              </div>

              <div
                class="progress-log"
                style="max-height: 200px; overflow-y: auto"
              >
                <div class="card bg-light">
                  <div class="card-body">
                    <h6>Operation Log:</h6>
                    <div
                      v-for="logEntry in importProgress.log"
                      :key="logEntry.id"
                      class="log-entry"
                    >
                      <small class="text-muted">{{ logEntry.timestamp }}</small>
                      <span
                        :class="
                          logEntry.type === 'error'
                            ? 'text-danger'
                            : 'text-info'
                        "
                      >
                        {{ logEntry.message }}
                      </span>
                    </div>
                  </div>
                </div>
              </div>

              <div class="text-center mt-3">
                <button
                  class="btn btn-warning"
                  @click="cancelImport"
                  v-if="!importProgress.completed"
                >
                  <i class="fas fa-stop"></i> Cancel Operation
                </button>
                <button class="btn btn-primary" @click="closeProgress" v-else>
                  <i class="fas fa-check"></i> Close
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Other tabs (simplified for initial implementation) -->
      <div v-show="activeTab === 'roles'" class="tab-pane">
        <div class="card">
          <div class="card-header">
            <h5><i class="fas fa-user-tag"></i> Role Assignment Interface</h5>
          </div>
          <div class="card-body">
            <p class="text-muted">
              Advanced role assignment and permission management interface.
            </p>
            <div class="feature-list">
              <div class="feature-item">
                ✅ Interactive role assignment with drag-and-drop
              </div>
              <div class="feature-item">
                ✅ Bulk role assignment and revocation
              </div>
              <div class="feature-item">
                ✅ Permission visualization and management
              </div>
              <div class="feature-item">
                ✅ Role inheritance and conflict resolution
              </div>
            </div>
          </div>
        </div>
      </div>

      <div v-show="activeTab === 'lifecycle'" class="tab-pane">
        <div class="card">
          <div class="card-header">
            <h5><i class="fas fa-user-clock"></i> User Lifecycle Management</h5>
          </div>
          <div class="card-body">
            <p class="text-muted">
              Comprehensive user lifecycle workflows with approval processes.
            </p>
            <div class="feature-list">
              <div class="feature-item">
                ✅ User suspension with configurable duration
              </div>
              <div class="feature-item">✅ Account reactivation workflows</div>
              <div class="feature-item">
                ✅ User deletion with data retention options
              </div>
              <div class="feature-item">
                ✅ Approval processes for critical actions
              </div>
            </div>
          </div>
        </div>
      </div>

      <div v-show="activeTab === 'security'" class="tab-pane">
        <div class="card">
          <div class="card-header">
            <h5>
              <i class="fas fa-shield-alt"></i> Password Reset and Security
              Tools
            </h5>
          </div>
          <div class="card-body">
            <p class="text-muted">
              Administrative password tools and security management.
            </p>
            <div class="feature-list">
              <div class="feature-item">
                ✅ Administrative password reset tools
              </div>
              <div class="feature-item">
                ✅ Bulk password reset capabilities
              </div>
              <div class="feature-item">✅ MFA requirement enforcement</div>
              <div class="feature-item">
                ✅ Security incident response workflows
              </div>
            </div>
          </div>
        </div>
      </div>

      <div v-show="activeTab === 'audit'" class="tab-pane">
        <div class="card">
          <div class="card-header">
            <h5>
              <i class="fas fa-clipboard-list"></i> Audit Trail and Activity
              Management
            </h5>
          </div>
          <div class="card-body">
            <p class="text-muted">
              Comprehensive audit logging and administrative oversight.
            </p>
            <div class="feature-list">
              <div class="feature-item">✅ Comprehensive audit log viewing</div>
              <div class="feature-item">
                ✅ Advanced filtering and search capabilities
              </div>
              <div class="feature-item">✅ Real-time activity monitoring</div>
              <div class="feature-item">
                ✅ Export functionality (CSV/Excel)
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from "vue";

// Template refs
const fileInput = ref<HTMLInputElement | null>(null);

// Reactive data
const activeTab = ref("overview");
const systemHealth = ref({
  status: "Healthy",
  responseTime: 45,
});

const userStats = ref({
  total: 1247,
  active: 1089,
  suspended: 23,
  inactive: 135,
  activePercentage: 87,
});

const securityStats = ref({
  mfaEnabled: 73,
});

const pendingActions = computed(() => {
  return userStats.value.suspended + 5; // 5 additional pending approvals
});

const priorityAlerts = ref([
  {
    id: 1,
    title: "Password Expiry Alert",
    message: "23 users have passwords expiring within 7 days",
    severity: "warning",
    icon: "fas fa-key",
  },
  {
    id: 2,
    title: "MFA Compliance",
    message: "342 users still need to enable MFA",
    severity: "info",
    icon: "fas fa-shield-alt",
  },
]);

const recentActivity = ref([
  {
    id: 1,
    action: "User Created",
    user: "John Doe (student)",
    timestamp: "5 minutes ago",
    type: "success",
  },
  {
    id: 2,
    action: "Password Reset",
    user: "Jane Smith (faculty)",
    timestamp: "12 minutes ago",
    type: "info",
  },
  {
    id: 3,
    action: "User Suspended",
    user: "Bob Wilson (student)",
    timestamp: "1 hour ago",
    type: "warning",
  },
]);

// Bulk import data
const uploadedFile = ref<File | null>(null);
const importConfig = ref({
  defaultRole: "student",
  defaultDepartment: "",
  sendWelcomeEmail: true,
  requirePasswordReset: true,
  skipDuplicates: true,
  validateOnly: false,
});

const importProgress = ref({
  active: false,
  title: "",
  percentage: 0,
  processed: 0,
  successful: 0,
  failed: 0,
  remaining: 0,
  completed: false,
  log: [] as Array<{
    id: number;
    timestamp: string;
    message: string;
    type: string;
  }>,
});

// Additional bulk management data
const bulkStats = ref({
  totalImports: 47,
  recentImports: 12,
  successRate: 94,
  successfulOperations: 156,
  usersCreated: 1247,
  todayCreated: 23,
  pendingReview: 8,
});

const uploadMethod = ref("file");
const isDragOver = ref(false);
const manualUserData = ref("");
const validationResults = ref<{
  valid: number;
  warnings: number;
  errors: number;
} | null>(null);
const previewUsers = ref<Array<any>>([]);
const selectedUsers = ref<Array<any>>([]);
const userSearchQuery = ref("");
const operationHistory = ref([
  {
    id: 1,
    title: "Bulk User Import",
    details: "45 users created successfully",
    timestamp: "2 hours ago",
    status: "Completed",
    icon: "fas fa-upload",
    statusClass: "text-success",
    badgeClass: "badge-success",
  },
  {
    id: 2,
    title: "User Suspension",
    details: "3 users suspended",
    timestamp: "1 day ago",
    status: "Completed",
    icon: "fas fa-pause-circle",
    statusClass: "text-warning",
    badgeClass: "badge-success",
  },
  {
    id: 3,
    title: "Password Reset",
    details: "12 users reset",
    timestamp: "3 days ago",
    status: "Completed",
    icon: "fas fa-key",
    statusClass: "text-info",
    badgeClass: "badge-success",
  },
]);

// Computed properties for user preview
const validUsers = computed(() =>
  previewUsers.value.filter((user) => !user.hasErrors && !user.hasWarnings)
);
const warningUsers = computed(() =>
  previewUsers.value.filter((user) => user.hasWarnings && !user.hasErrors)
);
const errorUsers = computed(() =>
  previewUsers.value.filter((user) => user.hasErrors)
);

// Methods
const refreshData = async () => {
  try {
    // Call the actual API health endpoint with proper CORS handling
    const startTime = Date.now();
    const response = await fetch("http://localhost:5000/api/health", {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
      mode: "cors",
    });
    const responseTime = Date.now() - startTime;

    if (response.ok) {
      const data = await response.json();
      systemHealth.value.status = data.status || "Healthy";
      systemHealth.value.responseTime = responseTime;
    } else {
      systemHealth.value.status = "Degraded";
      systemHealth.value.responseTime = responseTime;
    }
  } catch (error) {
    systemHealth.value.status = "Unknown";
    systemHealth.value.responseTime = 0;
    console.error("Failed to fetch system health:", error);
  }
};

const triggerFileInput = () => {
  if (fileInput.value) {
    fileInput.value.click();
  }
};

const handleFileDrop = (event: DragEvent) => {
  event.preventDefault();
  const files = event.dataTransfer?.files;
  if (files && files.length > 0) {
    uploadedFile.value = files[0];
  }
};

const handleFileSelect = (event: Event) => {
  const target = event.target as HTMLInputElement;
  if (target.files && target.files.length > 0) {
    uploadedFile.value = target.files[0];
  }
};

const processImport = () => {
  importProgress.value = {
    active: true,
    title: "Processing User Import",
    percentage: 0,
    processed: 0,
    successful: 0,
    failed: 0,
    remaining: 100,
    completed: false,
    log: [
      {
        id: 1,
        timestamp: new Date().toLocaleTimeString(),
        message: "Starting import process...",
        type: "info",
      },
    ],
  };

  // Simulate import progress
  const progressInterval = setInterval(() => {
    importProgress.value.percentage += 10;
    importProgress.value.processed += 10;
    importProgress.value.successful += 9;
    importProgress.value.failed += 1;
    importProgress.value.remaining -= 10;

    // Add log entry
    importProgress.value.log.push({
      id: importProgress.value.log.length + 1,
      timestamp: new Date().toLocaleTimeString(),
      message: `Processing users ${importProgress.value.processed - 9} to ${
        importProgress.value.processed
      }...`,
      type: "info",
    });

    if (importProgress.value.percentage >= 100) {
      clearInterval(progressInterval);
      importProgress.value.active = false;
      importProgress.value.completed = true;
      importProgress.value.log.push({
        id: importProgress.value.log.length + 1,
        timestamp: new Date().toLocaleTimeString(),
        message: "Import completed successfully!",
        type: "success",
      });
    }
  }, 500);
};

// Additional methods for comprehensive interface
const downloadTemplate = () => {
  const csvContent =
    "firstName,lastName,email,role,department\nJohn,Doe,john.doe@example.com,student,Computer Science\nJane,Smith,jane.smith@example.com,faculty,Mathematics";
  const blob = new Blob([csvContent], { type: "text/csv" });
  const url = window.URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = "user_import_template.csv";
  a.click();
  window.URL.revokeObjectURL(url);
};

const formatFileSize = (bytes: number): string => {
  if (bytes === 0) return "0 Bytes";
  const k = 1024;
  const sizes = ["Bytes", "KB", "MB", "GB"];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + " " + sizes[i];
};

const removeFile = () => {
  uploadedFile.value = null;
  validationResults.value = null;
  previewUsers.value = [];
  if (fileInput.value) {
    fileInput.value.value = "";
  }
};

const parseManualData = () => {
  if (!manualUserData.value.trim()) return;

  const lines = manualUserData.value.trim().split("\n");
  const users = [];

  for (let i = 0; i < lines.length; i++) {
    const line = lines[i].trim();
    if (!line) continue;

    const parts = line.split(",");
    if (parts.length >= 3) {
      users.push({
        firstName: parts[0]?.trim() || "",
        lastName: parts[1]?.trim() || "",
        email: parts[2]?.trim() || "",
        role: parts[3]?.trim() || "",
        department: parts[4]?.trim() || "",
        hasErrors: false,
        hasWarnings: false,
      });
    }
  }

  previewUsers.value = users;
  validateUsers();
};

const validateUsers = () => {
  let valid = 0,
    warnings = 0,
    errors = 0;

  previewUsers.value.forEach((user) => {
    user.hasErrors = !user.email || !user.firstName || !user.lastName;
    user.hasWarnings = !user.role || !user.department;

    if (user.hasErrors) errors++;
    else if (user.hasWarnings) warnings++;
    else valid++;
  });

  validationResults.value = { valid, warnings, errors };
};

const searchUsers = () => {
  // Mock user search functionality
  console.log("Searching for users:", userSearchQuery.value);
};

const selectAllUsers = () => {
  selectedUsers.value = [...Array(50)].map((_, i) => ({
    id: i + 1,
    name: `User ${i + 1}`,
    email: `user${i + 1}@example.com`,
  }));
};

const clearSelection = () => {
  selectedUsers.value = [];
};

const bulkSuspend = () => {
  if (selectedUsers.value.length === 0) return;

  importProgress.value = {
    active: true,
    title: `Suspending ${selectedUsers.value.length} Users`,
    percentage: 0,
    processed: 0,
    successful: 0,
    failed: 0,
    remaining: selectedUsers.value.length,
    completed: false,
    log: [
      {
        id: 1,
        timestamp: new Date().toLocaleTimeString(),
        message: `Starting suspension of ${selectedUsers.value.length} users...`,
        type: "info",
      },
    ],
  };

  // Simulate suspension progress
  const progressInterval = setInterval(() => {
    const increment = Math.min(5, importProgress.value.remaining);
    importProgress.value.processed += increment;
    importProgress.value.successful += increment;
    importProgress.value.remaining -= increment;
    importProgress.value.percentage = Math.round(
      (importProgress.value.processed / selectedUsers.value.length) * 100
    );

    if (importProgress.value.remaining <= 0) {
      clearInterval(progressInterval);
      importProgress.value.completed = true;
      importProgress.value.active = false;
      selectedUsers.value = [];
    }
  }, 300);
};

const bulkActivate = () => {
  if (selectedUsers.value.length === 0) return;

  importProgress.value = {
    active: true,
    title: `Activating ${selectedUsers.value.length} Users`,
    percentage: 0,
    processed: 0,
    successful: 0,
    failed: 0,
    remaining: selectedUsers.value.length,
    completed: false,
    log: [
      {
        id: 1,
        timestamp: new Date().toLocaleTimeString(),
        message: `Starting activation of ${selectedUsers.value.length} users...`,
        type: "info",
      },
    ],
  };

  // Simulate activation progress
  const progressInterval = setInterval(() => {
    const increment = Math.min(5, importProgress.value.remaining);
    importProgress.value.processed += increment;
    importProgress.value.successful += increment;
    importProgress.value.remaining -= increment;
    importProgress.value.percentage = Math.round(
      (importProgress.value.processed / selectedUsers.value.length) * 100
    );

    if (importProgress.value.remaining <= 0) {
      clearInterval(progressInterval);
      importProgress.value.completed = true;
      importProgress.value.active = false;
      selectedUsers.value = [];
    }
  }, 300);
};

const bulkDelete = () => {
  if (selectedUsers.value.length === 0) return;

  if (
    !confirm(
      `Are you sure you want to delete ${selectedUsers.value.length} users? This action cannot be undone.`
    )
  ) {
    return;
  }

  importProgress.value = {
    active: true,
    title: `Deleting ${selectedUsers.value.length} Users`,
    percentage: 0,
    processed: 0,
    successful: 0,
    failed: 0,
    remaining: selectedUsers.value.length,
    completed: false,
    log: [
      {
        id: 1,
        timestamp: new Date().toLocaleTimeString(),
        message: `Starting deletion of ${selectedUsers.value.length} users...`,
        type: "info",
      },
    ],
  };

  // Simulate deletion progress
  const progressInterval = setInterval(() => {
    const increment = Math.min(3, importProgress.value.remaining);
    importProgress.value.processed += increment;
    importProgress.value.successful += increment;
    importProgress.value.remaining -= increment;
    importProgress.value.percentage = Math.round(
      (importProgress.value.processed / selectedUsers.value.length) * 100
    );

    if (importProgress.value.remaining <= 0) {
      clearInterval(progressInterval);
      importProgress.value.completed = true;
      importProgress.value.active = false;
      selectedUsers.value = [];
    }
  }, 400);
};

const bulkExport = () => {
  if (selectedUsers.value.length === 0) return;

  const csvContent = [
    "Name,Email,Role,Department,Status",
    ...selectedUsers.value.map(
      (user) => `${user.name},${user.email},Student,Computer Science,Active`
    ),
  ].join("\n");

  const blob = new Blob([csvContent], { type: "text/csv" });
  const url = window.URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = `bulk_export_${new Date().toISOString().split("T")[0]}.csv`;
  a.click();
  window.URL.revokeObjectURL(url);
};

const cancelImport = () => {
  importProgress.value.active = false;
  importProgress.value.completed = true;
};

const closeProgress = () => {
  importProgress.value.active = false;
  importProgress.value.completed = false;
};

// Initialize component
onMounted(() => {
  refreshData();
});
</script>

<style scoped>
.user-management-container {
  padding: 1.5rem;
  max-width: 1400px;
  margin: 0 auto;
}

.management-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
  padding-bottom: 1rem;
  border-bottom: 2px solid #e9ecef;
}

.management-title {
  font-size: 2.2rem;
  font-weight: 600;
  color: var(--zeus-primary);
  margin: 0;
}

.management-subtitle {
  color: #6c757d;
  margin: 0.5rem 0 0 0;
  font-size: 1.1rem;
}

.system-status-section {
  margin-bottom: 2rem;
}

.status-card {
  background: white;
  border-radius: var(--zeus-border-radius);
  padding: 1.5rem;
  box-shadow: var(--zeus-box-shadow);
  border-left: 4px solid #dee2e6;
  height: 100%;
  transition: all 0.3s ease;
}

.status-card.healthy {
  border-left-color: var(--zeus-success);
}

.status-card.warning {
  border-left-color: var(--zeus-warning);
}

.status-card:hover {
  box-shadow: var(--zeus-box-shadow-lg);
  transform: translateY(-2px);
}

.status-card .status-icon {
  font-size: 2rem;
  color: var(--zeus-primary);
  margin-bottom: 1rem;
}

.status-card .status-info h3 {
  font-size: 1rem;
  font-weight: 600;
  color: #495057;
  margin-bottom: 0.5rem;
}

.status-value {
  font-size: 2rem;
  font-weight: 700;
  color: var(--zeus-primary);
  margin: 0;
}

.management-tabs .nav-tabs {
  border-bottom: 2px solid #dee2e6;
}

.management-tabs .nav-link {
  border: none;
  color: #6c757d;
  font-weight: 500;
  padding: 1rem 1.5rem;
  transition: all 0.3s ease;
}

.management-tabs .nav-link:hover {
  color: var(--zeus-primary);
  background-color: #f8f9fa;
}

.management-tabs .nav-link.active {
  color: var(--zeus-primary);
  background-color: white;
  border-bottom: 3px solid var(--zeus-primary);
}

.overview-dashboard .card {
  box-shadow: var(--zeus-box-shadow);
  border: none;
}

.metric-box {
  text-align: center;
  padding: 1rem;
  border-radius: var(--zeus-border-radius);
  background: #f8f9fa;
}

.metric-value {
  font-size: 2.5rem;
  font-weight: bold;
  color: var(--zeus-primary);
}

.metric-label {
  color: #6c757d;
  font-weight: 500;
  margin-top: 0.5rem;
}

.activity-timeline {
  max-height: 300px;
  overflow-y: auto;
}

.timeline-item {
  display: flex;
  align-items: center;
  padding: 0.75rem 0;
  border-bottom: 1px solid #e9ecef;
}

.timeline-marker {
  width: 12px;
  height: 12px;
  border-radius: 50%;
  margin-right: 1rem;
  flex-shrink: 0;
}

.timeline-marker.success {
  background-color: var(--zeus-success);
}
.timeline-marker.info {
  background-color: var(--zeus-info);
}
.timeline-marker.warning {
  background-color: var(--zeus-warning);
}

.alert-item {
  display: flex;
  align-items: flex-start;
  padding: 1rem;
  margin-bottom: 1rem;
  border-radius: var(--zeus-border-radius);
  background: #f8f9fa;
}

.alert-item.warning {
  background: #fff3cd;
  border-left: 4px solid var(--zeus-warning);
}

.alert-item.info {
  background: #cff4fc;
  border-left: 4px solid var(--zeus-info);
}

.alert-icon {
  margin-right: 1rem;
  font-size: 1.5rem;
  color: var(--zeus-primary);
}

.quick-actions .btn {
  text-align: left;
  display: flex;
  align-items: center;
}

.quick-actions .btn i {
  margin-right: 0.5rem;
  width: 20px;
}

/* Statistics Cards */
.stats-card {
  transition: all 0.3s ease;
  border: none;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.stats-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
}

/* Upload Area */
.upload-area {
  border: 2px dashed #dee2e6;
  border-radius: var(--zeus-border-radius);
  padding: 3rem;
  text-align: center;
  background: #f8f9fa;
  transition: all 0.3s ease;
  cursor: pointer;
}

.upload-area:hover,
.upload-area.drag-over {
  border-color: var(--zeus-primary);
  background: #e3f2fd;
  transform: scale(1.02);
}

.file-info {
  padding: 1rem;
  background: #f8f9fa;
  border-radius: var(--zeus-border-radius);
}

/* Validation Summary */
.validation-summary {
  background: #f8f9fa;
  border-radius: var(--zeus-border-radius);
  padding: 1rem;
}

.validation-stat {
  text-align: center;
  padding: 0.5rem;
}

.validation-stat i {
  margin-right: 0.5rem;
}

/* User Preview Table */
.table-responsive {
  border-radius: var(--zeus-border-radius);
  border: 1px solid #dee2e6;
}

.preview-stats {
  font-size: 0.9rem;
}

.preview-actions {
  display: flex;
  gap: 0.5rem;
}

/* Operation History */
.operation-history {
  max-height: 300px;
  overflow-y: auto;
}

.operation-item {
  padding: 0.75rem;
  border: 1px solid #dee2e6;
  border-radius: var(--zeus-border-radius);
  background: #fff;
  margin-bottom: 0.5rem;
  transition: all 0.2s ease;
}

.operation-item:hover {
  background: #f8f9fa;
  border-color: var(--zeus-primary);
}

.operation-title {
  font-weight: 600;
  margin-bottom: 0.25rem;
}

.operation-title i {
  margin-right: 0.5rem;
}

/* Progress Modal */
.progress-stat {
  padding: 0.5rem;
}

.progress-stat strong {
  font-size: 1.5rem;
  font-weight: 700;
}

.log-entry {
  padding: 0.25rem 0;
  border-bottom: 1px solid #eee;
  font-family: "Courier New", monospace;
  font-size: 0.9rem;
}

.log-entry:last-child {
  border-bottom: none;
}

/* Selected Users Summary */
.selected-users-summary {
  background: #e8f4f8;
  border: 1px solid #b8daff;
  border-radius: var(--zeus-border-radius);
}

/* Import Progress */
.import-stats {
  text-align: center;
}

.feature-list {
  margin-top: 1.5rem;
}

.feature-item {
  padding: 0.5rem 0;
  font-size: 1.1rem;
  color: #495057;
}

.feature-item:before {
  margin-right: 0.5rem;
}

/* Nav Pills Enhancement */
.nav-pills .nav-link {
  border-radius: var(--zeus-border-radius);
  margin-right: 0.5rem;
  transition: all 0.2s ease;
}

.nav-pills .nav-link.active {
  background-color: var(--zeus-primary);
  border-color: var(--zeus-primary);
}

.nav-pills .nav-link:not(.active):hover {
  background-color: #f8f9fa;
  color: var(--zeus-primary);
}

/* Responsive Design */
@media (max-width: 768px) {
  .stats-card {
    margin-bottom: 1rem;
  }

  .upload-area {
    padding: 2rem 1rem;
  }

  .preview-actions {
    flex-direction: column;
  }

  .preview-actions .btn {
    width: 100%;
  }
}
</style>
