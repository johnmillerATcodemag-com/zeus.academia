<template>
  <div class="faculty-layout">
    <FacultySidebar />
    <FacultyHeader />

    <main class="faculty-main">
      <div class="dashboard-header mb-4">
        <h1 class="dashboard-title">Faculty Dashboard</h1>
        <p class="dashboard-subtitle">Welcome back, {{ userFullName }}</p>
      </div>

      <!-- Dashboard Summary Cards -->
      <div class="row mb-4">
        <div class="col-lg-3 col-md-6 mb-3">
          <div class="summary-card">
            <div class="summary-icon">📚</div>
            <div class="summary-value">{{ dashboardStats.totalCourses }}</div>
            <div class="summary-label">Active Courses</div>
            <div class="summary-change positive">+2 from last semester</div>
          </div>
        </div>

        <div class="col-lg-3 col-md-6 mb-3">
          <div class="summary-card">
            <div class="summary-icon">👥</div>
            <div class="summary-value">{{ dashboardStats.totalStudents }}</div>
            <div class="summary-label">Total Students</div>
            <div class="summary-change positive">+15 this semester</div>
          </div>
        </div>

        <div class="col-lg-3 col-md-6 mb-3">
          <div class="summary-card">
            <div class="summary-icon">📝</div>
            <div class="summary-value">{{ dashboardStats.pendingGrades }}</div>
            <div class="summary-label">Pending Grades</div>
            <div class="summary-change neutral">Due this week</div>
          </div>
        </div>

        <div class="col-lg-3 col-md-6 mb-3">
          <div class="summary-card">
            <div class="summary-icon">📊</div>
            <div class="summary-value">{{ dashboardStats.averageGrade }}%</div>
            <div class="summary-label">Class Average</div>
            <div class="summary-change positive">+3% improvement</div>
          </div>
        </div>
      </div>

      <!-- Recent Activity and Quick Actions -->
      <div class="row">
        <div class="col-lg-8">
          <div class="faculty-card">
            <div class="card-header">
              <h3 class="card-title">Recent Courses</h3>
              <p class="card-subtitle">Your active courses this semester</p>
            </div>
            <div class="card-body">
              <div v-if="loading" class="text-center py-4">
                <div class="spinner-border text-primary" role="status">
                  <span class="visually-hidden">Loading courses...</span>
                </div>
              </div>

              <div v-else-if="courses.length === 0" class="text-center py-4">
                <p class="text-muted">
                  No courses found. Contact administration to set up your
                  courses.
                </p>
              </div>

              <div v-else class="course-list">
                <div
                  v-for="course in courses.slice(0, 5)"
                  :key="course.id"
                  class="course-item d-flex justify-content-between align-items-center py-3 border-bottom"
                >
                  <div class="course-info">
                    <h5 class="course-name mb-1">
                      {{ course.code }} - {{ course.name }}
                    </h5>
                    <p class="course-details mb-0 text-muted">
                      {{ course.section }} • {{ course.enrollmentCount }}/{{
                        course.maxEnrollment
                      }}
                      students
                    </p>
                  </div>
                  <div class="course-actions">
                    <router-link
                      :to="`/gradebook/${course.id}`"
                      class="btn btn-outline-primary btn-sm me-2"
                    >
                      Gradebook
                    </router-link>
                    <router-link
                      :to="`/courses/${course.id}`"
                      class="btn btn-primary btn-sm"
                    >
                      View Course
                    </router-link>
                  </div>
                </div>
              </div>
            </div>
            <div class="card-footer">
              <div class="card-actions">
                <router-link to="/courses" class="btn btn-outline-primary">
                  View All Courses
                </router-link>
              </div>
            </div>
          </div>
        </div>

        <div class="col-lg-4">
          <div class="faculty-card">
            <div class="card-header">
              <h3 class="card-title">Quick Actions</h3>
            </div>
            <div class="card-body">
              <div class="d-grid gap-2">
                <router-link to="/gradebook" class="btn btn-primary">
                  Open Gradebook
                </router-link>
                <button
                  class="btn btn-outline-primary"
                  @click="createAssignment"
                >
                  Create Assignment
                </button>
                <button
                  class="btn btn-outline-secondary"
                  @click="sendAnnouncement"
                >
                  Send Announcement
                </button>
                <router-link to="/analytics" class="btn btn-outline-info">
                  View Analytics
                </router-link>
              </div>
            </div>
          </div>

          <!-- Upcoming deadlines -->
          <div class="faculty-card mt-4">
            <div class="card-header">
              <h3 class="card-title">Upcoming Deadlines</h3>
            </div>
            <div class="card-body">
              <div class="deadline-list">
                <div
                  class="deadline-item d-flex justify-content-between align-items-center py-2"
                >
                  <div>
                    <div class="deadline-title">Midterm Grades Due</div>
                    <small class="text-muted">CS 101 - Section A</small>
                  </div>
                  <span class="badge bg-warning">3 days</span>
                </div>
                <div
                  class="deadline-item d-flex justify-content-between align-items-center py-2"
                >
                  <div>
                    <div class="deadline-title">Assignment 3 Due</div>
                    <small class="text-muted">CS 201 - Section B</small>
                  </div>
                  <span class="badge bg-info">5 days</span>
                </div>
                <div
                  class="deadline-item d-flex justify-content-between align-items-center py-2"
                >
                  <div>
                    <div class="deadline-title">Final Project Due</div>
                    <small class="text-muted">CS 301 - All Sections</small>
                  </div>
                  <span class="badge bg-danger">1 week</span>
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
import { ref, computed, onMounted } from "vue";
import { useAuthStore } from "@/stores/auth";
import { useGradebookStore } from "@/stores/gradebook";
import FacultySidebar from "@/components/FacultySidebar.vue";
import FacultyHeader from "@/components/FacultyHeader.vue";

const authStore = useAuthStore();
const gradebookStore = useGradebookStore();

const loading = ref(true);

// Computed properties
const userFullName = computed(() => authStore.userFullName);
const courses = computed(() => gradebookStore.courses);

// Dashboard statistics (mock data for now)
const dashboardStats = ref({
  totalCourses: 4,
  totalStudents: 156,
  pendingGrades: 23,
  averageGrade: 87,
});

// Methods
const createAssignment = () => {
  // TODO: Implement assignment creation modal
  console.log("Create assignment clicked");
};

const sendAnnouncement = () => {
  // TODO: Implement announcement modal
  console.log("Send announcement clicked");
};

// Initialize dashboard data
onMounted(async () => {
  try {
    await gradebookStore.loadCourses();

    // Calculate real statistics
    dashboardStats.value = {
      totalCourses: courses.value.length,
      totalStudents: courses.value.reduce(
        (sum, course) => sum + course.enrollmentCount,
        0
      ),
      pendingGrades: 23, // This would come from actual grade data
      averageGrade: 87, // This would be calculated from actual grades
    };
  } catch (error) {
    console.error("Failed to load dashboard data:", error);
  } finally {
    loading.value = false;
  }
});
</script>

<style lang="scss" scoped>
.dashboard-header {
  .dashboard-title {
    font-size: 2rem;
    font-weight: 600;
    color: var(--faculty-text-primary);
    margin: 0;
  }

  .dashboard-subtitle {
    color: var(--faculty-text-secondary);
    font-size: 1.1rem;
    margin: 0.5rem 0 0;
  }
}

.course-item {
  &:last-child {
    border-bottom: none !important;
  }

  .course-name {
    color: var(--faculty-text-primary);
    font-weight: 600;
  }

  .course-details {
    font-size: 0.9rem;
  }
}

.deadline-item {
  .deadline-title {
    font-weight: 500;
    color: var(--faculty-text-primary);
  }
}
</style>
