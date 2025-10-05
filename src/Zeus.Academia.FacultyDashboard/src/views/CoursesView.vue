<template>
  <div class="faculty-layout">
    <FacultySidebar />
    <FacultyHeader />

    <main class="faculty-main">
      <div class="courses-container">
        <!-- Course Detail View -->
        <div v-if="courseId && selectedCourse" class="course-detail">
          <div class="d-flex justify-content-between align-items-center mb-4">
            <div>
              <h1>{{ selectedCourse.name }}</h1>
              <p class="text-muted">
                {{ selectedCourse.code }} - {{ selectedCourse.section }}
              </p>
            </div>
            <router-link to="/courses" class="btn btn-outline-secondary">
              ← Back to All Courses
            </router-link>
          </div>

          <!-- Course Management Tabs -->
          <div class="faculty-card mb-4">
            <div class="card-header p-0">
              <ul class="nav nav-tabs card-header-tabs" role="tablist">
                <li class="nav-item" role="presentation">
                  <button
                    class="nav-link"
                    :class="{ active: activeTab === 'overview' }"
                    @click="activeTab = 'overview'"
                    type="button"
                  >
                    📊 Overview
                  </button>
                </li>
                <li class="nav-item" role="presentation">
                  <button
                    class="nav-link"
                    :class="{ active: activeTab === 'content' }"
                    @click="activeTab = 'content'"
                    type="button"
                  >
                    📚 Content Management
                  </button>
                </li>
                <li class="nav-item" role="presentation">
                  <button
                    class="nav-link"
                    :class="{ active: activeTab === 'assignments' }"
                    @click="activeTab = 'assignments'"
                    type="button"
                  >
                    📝 Assignments
                  </button>
                </li>
                <li class="nav-item" role="presentation">
                  <button
                    class="nav-link"
                    :class="{ active: activeTab === 'roster' }"
                    @click="activeTab = 'roster'"
                    type="button"
                  >
                    👥 Student Roster
                  </button>
                </li>
                <li class="nav-item" role="presentation">
                  <button
                    class="nav-link"
                    :class="{ active: activeTab === 'calendar' }"
                    @click="activeTab = 'calendar'"
                    type="button"
                  >
                    📅 Course Calendar
                  </button>
                </li>
              </ul>
            </div>

            <div class="card-body">
              <!-- Overview Tab -->
              <div v-if="activeTab === 'overview'" class="tab-content">
                <div class="row">
                  <div class="col-md-8">
                    <div class="faculty-card mb-4">
                      <div class="card-header">
                        <h3>Course Information</h3>
                      </div>
                      <div class="card-body">
                        <div class="row">
                          <div class="col-sm-6">
                            <p>
                              <strong>Credits:</strong>
                              {{ selectedCourse.credits }}
                            </p>
                            <p>
                              <strong>Semester:</strong>
                              {{ selectedCourse.semester }}
                              {{ selectedCourse.year }}
                            </p>
                            <p>
                              <strong>Status:</strong>
                              {{ selectedCourse.status }}
                            </p>
                          </div>
                          <div class="col-sm-6">
                            <p>
                              <strong>Enrollment:</strong>
                              {{ selectedCourse.enrollmentCount }}/{{
                                selectedCourse.maxEnrollment
                              }}
                            </p>
                            <p>
                              <strong>Section:</strong>
                              {{ selectedCourse.section }}
                            </p>
                            <p><strong>Meeting Times:</strong> TBA</p>
                          </div>
                        </div>
                        <div class="mt-3">
                          <p><strong>Description:</strong></p>
                          <p>{{ selectedCourse.description }}</p>
                        </div>
                      </div>
                    </div>

                    <div class="faculty-card">
                      <div class="card-header">
                        <h3>Quick Actions</h3>
                      </div>
                      <div class="card-body">
                        <div class="d-flex gap-3 flex-wrap">
                          <router-link
                            :to="`/gradebook/${selectedCourse.id}`"
                            class="btn btn-primary"
                          >
                            📊 Open Gradebook
                          </router-link>
                          <button
                            class="btn btn-outline-primary"
                            @click="activeTab = 'assignments'"
                          >
                            📝 Manage Assignments
                          </button>
                          <button
                            class="btn btn-outline-primary"
                            @click="activeTab = 'roster'"
                          >
                            👥 View Roster
                          </button>
                          <button
                            class="btn btn-outline-primary"
                            @click="activeTab = 'content'"
                          >
                            📚 Manage Content
                          </button>
                        </div>
                      </div>
                    </div>
                  </div>

                  <div class="col-md-4">
                    <div class="faculty-card mb-4">
                      <div class="card-header">
                        <h4>Course Metrics</h4>
                      </div>
                      <div class="card-body">
                        <div class="metric-item">
                          <div class="metric-value">
                            {{ selectedCourse.enrollmentCount }}
                          </div>
                          <div class="metric-label">Total Students</div>
                        </div>
                        <div class="metric-item">
                          <div class="metric-value">
                            {{ selectedCourse.credits }}
                          </div>
                          <div class="metric-label">Credits</div>
                        </div>
                        <div class="metric-item">
                          <div class="metric-value">
                            {{
                              Math.round(
                                (selectedCourse.enrollmentCount /
                                  selectedCourse.maxEnrollment) *
                                  100
                              )
                            }}%
                          </div>
                          <div class="metric-label">Enrollment Rate</div>
                        </div>
                      </div>
                    </div>

                    <div class="faculty-card">
                      <div class="card-header">
                        <h4>Recent Activity</h4>
                      </div>
                      <div class="card-body">
                        <div class="text-muted">
                          No recent activity to display
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Content Management Tab -->
              <div v-if="activeTab === 'content'" class="tab-content">
                <div class="row">
                  <div class="col-md-8">
                    <div
                      class="d-flex justify-content-between align-items-center mb-4"
                    >
                      <h3>Course Materials</h3>
                      <button
                        class="btn btn-primary"
                        @click="showUploadModal = true"
                      >
                        📤 Upload New Content
                      </button>
                    </div>

                    <!-- File Categories -->
                    <div class="row mb-4">
                      <div class="col-md-4">
                        <div class="faculty-card text-center">
                          <div class="card-body">
                            <div class="display-4 text-primary mb-2">📖</div>
                            <h5>Lectures</h5>
                            <p class="text-muted">
                              {{ getLectureCount() }} files
                            </p>
                            <button
                              class="btn btn-outline-primary btn-sm"
                              @click="filterContent('lecture')"
                            >
                              View All
                            </button>
                          </div>
                        </div>
                      </div>
                      <div class="col-md-4">
                        <div class="faculty-card text-center">
                          <div class="card-body">
                            <div class="display-4 text-success mb-2">📝</div>
                            <h5>Assignments</h5>
                            <p class="text-muted">
                              {{ getAssignmentCount() }} files
                            </p>
                            <button
                              class="btn btn-outline-success btn-sm"
                              @click="filterContent('assignment')"
                            >
                              View All
                            </button>
                          </div>
                        </div>
                      </div>
                      <div class="col-md-4">
                        <div class="faculty-card text-center">
                          <div class="card-body">
                            <div class="display-4 text-info mb-2">📚</div>
                            <h5>Resources</h5>
                            <p class="text-muted">
                              {{ getResourceCount() }} files
                            </p>
                            <button
                              class="btn btn-outline-info btn-sm"
                              @click="filterContent('resource')"
                            >
                              View All
                            </button>
                          </div>
                        </div>
                      </div>
                    </div>

                    <!-- File List -->
                    <div class="faculty-card">
                      <div
                        class="card-header d-flex justify-content-between align-items-center"
                      >
                        <h4>{{ getContentTitle() }}</h4>
                        <div class="d-flex gap-2">
                          <select
                            v-model="contentFilter"
                            class="form-select form-select-sm"
                            style="width: auto"
                          >
                            <option value="all">All Content</option>
                            <option value="lecture">Lectures</option>
                            <option value="assignment">Assignments</option>
                            <option value="resource">Resources</option>
                          </select>
                          <button
                            class="btn btn-outline-secondary btn-sm"
                            @click="refreshContent"
                          >
                            🔄 Refresh
                          </button>
                        </div>
                      </div>
                      <div class="card-body">
                        <div
                          v-if="filteredContent.length === 0"
                          class="text-center py-4"
                        >
                          <div class="text-muted">
                            <div class="display-1 mb-3">📁</div>
                            <p>
                              No
                              {{
                                contentFilter === "all"
                                  ? "content"
                                  : contentFilter + "s"
                              }}
                              found
                            </p>
                            <button
                              class="btn btn-primary"
                              @click="showUploadModal = true"
                            >
                              Upload First File
                            </button>
                          </div>
                        </div>
                        <div v-else class="table-responsive">
                          <table class="table table-hover">
                            <thead>
                              <tr>
                                <th>Name</th>
                                <th>Type</th>
                                <th>Size</th>
                                <th>Uploaded</th>
                                <th>Actions</th>
                              </tr>
                            </thead>
                            <tbody>
                              <tr
                                v-for="file in filteredContent"
                                :key="file.id"
                              >
                                <td>
                                  <div class="d-flex align-items-center">
                                    <span class="me-2">{{
                                      getFileIcon(file.type)
                                    }}</span>
                                    <div>
                                      <div class="fw-semibold">
                                        {{ file.name }}
                                      </div>
                                      <small class="text-muted">{{
                                        file.description || "No description"
                                      }}</small>
                                    </div>
                                  </div>
                                </td>
                                <td>
                                  <span
                                    class="badge"
                                    :class="getTypeBadgeClass(file.type)"
                                  >
                                    {{ file.type }}
                                  </span>
                                </td>
                                <td class="text-muted">
                                  {{ formatFileSize(file.size) }}
                                </td>
                                <td class="text-muted">
                                  {{ formatDate(file.uploadedAt) }}
                                </td>
                                <td>
                                  <div class="d-flex gap-1">
                                    <button
                                      class="btn btn-outline-primary btn-sm"
                                      @click="downloadFile(file)"
                                    >
                                      📥
                                    </button>
                                    <button
                                      class="btn btn-outline-secondary btn-sm"
                                      @click="editFile(file)"
                                    >
                                      ✏️
                                    </button>
                                    <button
                                      class="btn btn-outline-danger btn-sm"
                                      @click="deleteFile(file)"
                                    >
                                      🗑️
                                    </button>
                                  </div>
                                </td>
                              </tr>
                            </tbody>
                          </table>
                        </div>
                      </div>
                    </div>
                  </div>

                  <div class="col-md-4">
                    <!-- Course Announcements -->
                    <div class="faculty-card mb-4">
                      <div
                        class="card-header d-flex justify-content-between align-items-center"
                      >
                        <h4>Announcements</h4>
                        <button
                          class="btn btn-outline-primary btn-sm"
                          @click="createAnnouncement"
                        >
                          📢 New
                        </button>
                      </div>
                      <div class="card-body">
                        <div
                          v-if="announcements.length === 0"
                          class="text-muted text-center py-3"
                        >
                          No announcements yet
                        </div>
                        <div v-else>
                          <div
                            v-for="announcement in announcements.slice(0, 3)"
                            :key="announcement.id"
                            class="announcement-item mb-3"
                          >
                            <h6 class="mb-1">{{ announcement.title }}</h6>
                            <p class="text-muted small mb-1">
                              {{ announcement.content }}
                            </p>
                            <small class="text-muted">{{
                              formatDate(announcement.publishedAt)
                            }}</small>
                          </div>
                          <button
                            v-if="announcements.length > 3"
                            class="btn btn-outline-secondary btn-sm w-100"
                          >
                            View All Announcements
                          </button>
                        </div>
                      </div>
                    </div>

                    <!-- Content Statistics -->
                    <div class="faculty-card">
                      <div class="card-header">
                        <h4>Content Statistics</h4>
                      </div>
                      <div class="card-body">
                        <div class="metric-item">
                          <div class="metric-value">
                            {{ getTotalContentCount() }}
                          </div>
                          <div class="metric-label">Total Files</div>
                        </div>
                        <div class="metric-item">
                          <div class="metric-value">
                            {{ formatFileSize(getTotalContentSize()) }}
                          </div>
                          <div class="metric-label">Total Size</div>
                        </div>
                        <div class="metric-item">
                          <div class="metric-value">
                            {{ getRecentUploadsCount() }}
                          </div>
                          <div class="metric-label">
                            Recent Uploads (7 days)
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Assignments Tab -->
              <div v-if="activeTab === 'assignments'" class="tab-content">
                <div class="text-center py-5">
                  <div class="display-1 text-muted mb-3">📝</div>
                  <h3>Assignment Management</h3>
                  <p class="text-muted">
                    Assignment creation and management features will be
                    implemented here.
                  </p>
                  <button class="btn btn-primary" @click="createAssignment">
                    Create New Assignment
                  </button>
                </div>
              </div>

              <!-- Student Roster Tab -->
              <div v-if="activeTab === 'roster'" class="tab-content">
                <div class="text-center py-5">
                  <div class="display-1 text-muted mb-3">👥</div>
                  <h3>Student Roster</h3>
                  <p class="text-muted">
                    Student roster and enrollment management features will be
                    implemented here.
                  </p>
                  <p class="text-muted">
                    Current enrollment: {{ selectedCourse.enrollmentCount }}/{{
                      selectedCourse.maxEnrollment
                    }}
                  </p>
                </div>
              </div>

              <!-- Course Calendar Tab -->
              <div v-if="activeTab === 'calendar'" class="tab-content">
                <div class="text-center py-5">
                  <div class="display-1 text-muted mb-3">📅</div>
                  <h3>Course Calendar</h3>
                  <p class="text-muted">
                    Course calendar and event management features will be
                    implemented here.
                  </p>
                  <button class="btn btn-primary" @click="createEvent">
                    Create New Event
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Courses List View -->
        <div v-else class="courses-list">
          <h1>My Courses</h1>
          <div v-if="loading" class="text-center py-4">
            <div class="spinner-border text-primary" role="status">
              <span class="visually-hidden">Loading courses...</span>
            </div>
          </div>
          <div v-else-if="error" class="alert alert-danger">
            {{ error }}
          </div>
          <div v-else-if="courses.length === 0" class="text-center py-4">
            <p class="text-muted">No courses found.</p>
          </div>
          <div v-else class="row">
            <div
              v-for="course in courses"
              :key="course.id"
              class="col-md-6 col-lg-4 mb-4"
            >
              <div class="faculty-card course-card h-100">
                <div class="card-body">
                  <h5 class="card-title">{{ course.name }}</h5>
                  <p class="card-text text-muted">
                    {{ course.code }} - {{ course.section }}
                  </p>
                  <p class="card-text">{{ course.description }}</p>
                  <div class="mt-auto">
                    <small class="text-muted">
                      {{ course.enrollmentCount }}/{{ course.maxEnrollment }}
                      students
                    </small>
                  </div>
                </div>
                <div class="card-footer">
                  <div class="d-flex gap-2">
                    <router-link
                      :to="`/courses/${course.id}`"
                      class="btn btn-primary btn-sm"
                    >
                      View Details
                    </router-link>
                    <router-link
                      :to="`/gradebook/${course.id}`"
                      class="btn btn-outline-primary btn-sm"
                    >
                      Gradebook
                    </router-link>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { useGradebookStore } from "@/stores/gradebook";
import FacultySidebar from "@/components/FacultySidebar.vue";
import FacultyHeader from "@/components/FacultyHeader.vue";

// Interfaces for content management
interface CourseFile {
  id: string;
  name: string;
  type: "lecture" | "assignment" | "resource";
  size: number;
  uploadedAt: Date;
  description?: string;
}

interface Announcement {
  id: string;
  title: string;
  content: string;
  publishedAt: Date;
}

// Props
const props = defineProps<{
  courseId?: string;
}>();

// Composables
const gradebookStore = useGradebookStore();

// Reactive data
const activeTab = ref<
  "overview" | "content" | "assignments" | "roster" | "calendar"
>("overview");
const contentFilter = ref<"all" | "lecture" | "assignment" | "resource">("all");
const showUploadModal = ref(false);

// Mock data for content management
const courseFiles = ref<CourseFile[]>([
  {
    id: "1",
    name: "Introduction to Programming.pdf",
    type: "lecture",
    size: 2048576,
    uploadedAt: new Date("2024-01-15"),
    description: "Week 1 lecture slides",
  },
  {
    id: "2",
    name: "Assignment 1 - Variables.docx",
    type: "assignment",
    size: 1024000,
    uploadedAt: new Date("2024-01-20"),
    description: "Programming fundamentals assignment",
  },
  {
    id: "3",
    name: "Python Documentation.pdf",
    type: "resource",
    size: 5120000,
    uploadedAt: new Date("2024-01-10"),
    description: "Python language reference",
  },
]);

const announcements = ref<Announcement[]>([
  {
    id: "1",
    title: "Assignment 2 Posted",
    content:
      "The second programming assignment is now available in the assignments section.",
    publishedAt: new Date("2024-01-25"),
  },
  {
    id: "2",
    title: "Office Hours Update",
    content: "Office hours this week will be moved to Thursday 2-4 PM.",
    publishedAt: new Date("2024-01-22"),
  },
]);

// Computed
const courses = computed(() => gradebookStore.courses);
const loading = computed(() => gradebookStore.loading);
const error = computed(() => gradebookStore.error);

const selectedCourse = computed(() => {
  if (!props.courseId) return null;
  return courses.value.find((course) => course.id === props.courseId);
});

const filteredContent = computed(() => {
  if (contentFilter.value === "all") {
    return courseFiles.value;
  }
  return courseFiles.value.filter((file) => file.type === contentFilter.value);
});

// Content management methods
const getLectureCount = () =>
  courseFiles.value.filter((f) => f.type === "lecture").length;
const getAssignmentCount = () =>
  courseFiles.value.filter((f) => f.type === "assignment").length;
const getResourceCount = () =>
  courseFiles.value.filter((f) => f.type === "resource").length;
const getTotalContentCount = () => courseFiles.value.length;

const getTotalContentSize = () => {
  return courseFiles.value.reduce((total, file) => total + file.size, 0);
};

const getRecentUploadsCount = () => {
  const weekAgo = new Date();
  weekAgo.setDate(weekAgo.getDate() - 7);
  return courseFiles.value.filter((file) => file.uploadedAt > weekAgo).length;
};

const getContentTitle = () => {
  switch (contentFilter.value) {
    case "lecture":
      return "Lecture Materials";
    case "assignment":
      return "Assignment Files";
    case "resource":
      return "Course Resources";
    default:
      return "All Course Content";
  }
};

const filterContent = (type: "lecture" | "assignment" | "resource") => {
  contentFilter.value = type;
};

const refreshContent = () => {
  // TODO: Implement content refresh
  console.log("Refreshing content...");
};

const getFileIcon = (type: string) => {
  switch (type) {
    case "lecture":
      return "📖";
    case "assignment":
      return "📝";
    case "resource":
      return "📚";
    default:
      return "📄";
  }
};

const getTypeBadgeClass = (type: string) => {
  switch (type) {
    case "lecture":
      return "bg-primary";
    case "assignment":
      return "bg-success";
    case "resource":
      return "bg-info";
    default:
      return "bg-secondary";
  }
};

const formatFileSize = (bytes: number) => {
  if (bytes === 0) return "0 Bytes";
  const k = 1024;
  const sizes = ["Bytes", "KB", "MB", "GB"];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + " " + sizes[i];
};

const formatDate = (date: Date) => {
  return date.toLocaleDateString("en-US", {
    year: "numeric",
    month: "short",
    day: "numeric",
  });
};

const downloadFile = (file: CourseFile) => {
  console.log("Downloading file:", file.name);
  // TODO: Implement file download
};

const editFile = (file: CourseFile) => {
  console.log("Editing file:", file.name);
  // TODO: Implement file editing
};

const deleteFile = (file: CourseFile) => {
  console.log("Deleting file:", file.name);
  // TODO: Implement file deletion
};

const createAnnouncement = () => {
  console.log("Creating new announcement");
  // TODO: Implement announcement creation
};

const createAssignment = () => {
  console.log("Creating new assignment");
  // TODO: Implement assignment creation
};

const createEvent = () => {
  console.log("Creating new calendar event");
  // TODO: Implement event creation
};

// Lifecycle
onMounted(async () => {
  if (courses.value.length === 0) {
    await gradebookStore.loadCourses();
  }
});
</script>

<style scoped>
.course-card {
  transition: transform 0.2s ease-in-out;
}

.course-card:hover {
  transform: translateY(-2px);
}

.metric-item {
  text-align: center;
  margin-bottom: 1rem;
}

.metric-value {
  font-size: 2rem;
  font-weight: bold;
  color: var(--faculty-primary);
}

.metric-label {
  font-size: 0.9rem;
  color: var(--faculty-text-secondary);
}

.activity-item {
  border-bottom: 1px solid #eee;
  padding-bottom: 0.5rem;
  margin-bottom: 0.5rem;
}

.activity-item:last-child {
  border-bottom: none;
  margin-bottom: 0;
}

/* Tab content styles */
.tab-content {
  min-height: 400px;
}

.nav-tabs .nav-link {
  color: var(--faculty-text-secondary);
  border: none;
  border-bottom: 2px solid transparent;
  border-radius: 0;
  padding: 1rem 1.5rem;
}

.nav-tabs .nav-link:hover {
  color: var(--faculty-primary);
  border-bottom-color: var(--faculty-primary);
  background-color: transparent;
}

.nav-tabs .nav-link.active {
  color: var(--faculty-primary);
  background-color: transparent;
  border-bottom-color: var(--faculty-primary);
  font-weight: 600;
}

/* Content management styles */
.announcement-item {
  border-bottom: 1px solid #f0f0f0;
  padding-bottom: 0.75rem;
}

.announcement-item:last-child {
  border-bottom: none;
  margin-bottom: 0;
}

/* File table hover effects */
.table-hover tbody tr:hover {
  background-color: rgba(var(--faculty-primary-rgb), 0.05);
}

/* Badge variations */
.badge.bg-primary {
  background-color: var(--faculty-primary) !important;
}

.badge.bg-success {
  background-color: #28a745 !important;
}

.badge.bg-info {
  background-color: #17a2b8 !important;
}

/* Metric cards in content section */
.faculty-card .display-4 {
  font-size: 3rem;
}
</style>
