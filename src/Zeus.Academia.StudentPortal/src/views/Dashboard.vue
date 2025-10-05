<template>
  <div class="dashboard-page">
    <div class="container-fluid py-4">
      <div class="row">
        <div class="col-12">
          <div class="d-flex justify-content-between align-items-center mb-4">
            <h1>Welcome back, {{ studentName }}!</h1>
            <small class="text-muted">{{ currentDate }}</small>
          </div>
        </div>
      </div>

      <div class="row g-4">
        <!-- Quick Stats -->
        <div class="col-md-3">
          <div class="card bg-primary text-white">
            <div class="card-body">
              <div class="d-flex align-items-center">
                <div class="flex-grow-1">
                  <h6 class="card-title">Enrolled Courses</h6>
                  <h3 class="mb-0">{{ enrolledCoursesCount }}</h3>
                </div>
                <i
                  class="bi bi-book-fill"
                  style="font-size: 2rem; opacity: 0.7"
                ></i>
              </div>
            </div>
          </div>
        </div>

        <div class="col-md-3">
          <div class="card bg-success text-white">
            <div class="card-body">
              <div class="d-flex align-items-center">
                <div class="flex-grow-1">
                  <h6 class="card-title">Current GPA</h6>
                  <h3 class="mb-0">{{ currentGPA }}</h3>
                </div>
                <i
                  class="bi bi-graph-up"
                  style="font-size: 2rem; opacity: 0.7"
                ></i>
              </div>
            </div>
          </div>
        </div>

        <div class="col-md-3">
          <div class="card bg-info text-white">
            <div class="card-body">
              <div class="d-flex align-items-center">
                <div class="flex-grow-1">
                  <h6 class="card-title">Completed Credits</h6>
                  <h3 class="mb-0">{{ completedCredits }}</h3>
                </div>
                <i
                  class="bi bi-award-fill"
                  style="font-size: 2rem; opacity: 0.7"
                ></i>
              </div>
            </div>
          </div>
        </div>

        <div class="col-md-3">
          <div class="card bg-warning text-dark">
            <div class="card-body">
              <div class="d-flex align-items-center">
                <div class="flex-grow-1">
                  <h6 class="card-title">Upcoming Deadlines</h6>
                  <h3 class="mb-0">{{ upcomingDeadlines }}</h3>
                </div>
                <i
                  class="bi bi-clock-fill"
                  style="font-size: 2rem; opacity: 0.7"
                ></i>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="row g-4 mt-2">
        <!-- Current Courses -->
        <div class="col-lg-8">
          <div class="card">
            <div
              class="card-header d-flex justify-content-between align-items-center"
            >
              <h5 class="mb-0">Current Courses</h5>
              <router-link to="/courses" class="btn btn-sm btn-outline-primary"
                >View All</router-link
              >
            </div>
            <div class="card-body">
              <div v-if="isLoading" class="text-center py-4">
                <div class="spinner-border text-primary" role="status">
                  <span class="visually-hidden">Loading...</span>
                </div>
              </div>

              <div
                v-else-if="currentCourses.length === 0"
                class="text-center py-4 text-muted"
              >
                <i class="bi bi-book" style="font-size: 3rem"></i>
                <p class="mt-2">No courses enrolled yet.</p>
                <router-link to="/courses" class="btn btn-primary"
                  >Browse Courses</router-link
                >
              </div>

              <div v-else class="row g-3">
                <div
                  v-for="course in currentCourses.slice(0, 3)"
                  :key="course.id"
                  class="col-12"
                >
                  <div class="border rounded p-3">
                    <div
                      class="d-flex justify-content-between align-items-start"
                    >
                      <div>
                        <h6 class="mb-1">
                          {{ course.code }} - {{ course.name }}
                        </h6>
                        <p class="text-muted mb-2">
                          <i class="bi bi-person me-1"></i>
                          {{ getInstructorDisplay(course.instructor) }}
                        </p>
                        <small class="text-muted"
                          >{{ course.credits }} credits</small
                        >
                      </div>
                      <span
                        class="badge"
                        :class="getStatusBadgeClass(course.enrollmentStatus)"
                      >
                        {{ course.enrollmentStatus }}
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Recent Activity -->
        <div class="col-lg-4">
          <div class="card">
            <div class="card-header">
              <h5 class="mb-0">Recent Activity</h5>
            </div>
            <div class="card-body">
              <div class="list-group list-group-flush">
                <div class="list-group-item border-0 px-0">
                  <div class="d-flex align-items-start">
                    <i class="bi bi-book-fill text-primary me-3 mt-1"></i>
                    <div class="flex-grow-1">
                      <small class="text-muted">Course enrollment</small>
                      <p class="mb-0 small">Enrolled in Advanced Mathematics</p>
                    </div>
                  </div>
                </div>

                <div class="list-group-item border-0 px-0">
                  <div class="d-flex align-items-start">
                    <i
                      class="bi bi-check-circle-fill text-success me-3 mt-1"
                    ></i>
                    <div class="flex-grow-1">
                      <small class="text-muted">Assignment submitted</small>
                      <p class="mb-0 small">Physics Lab Report submitted</p>
                    </div>
                  </div>
                </div>

                <div class="list-group-item border-0 px-0">
                  <div class="d-flex align-items-start">
                    <i class="bi bi-star-fill text-warning me-3 mt-1"></i>
                    <div class="flex-grow-1">
                      <small class="text-muted">Grade received</small>
                      <p class="mb-0 small">Chemistry Exam: A-</p>
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
</template>

<script setup lang="ts">
import { computed, onMounted } from "vue";
import { useStore } from "vuex";

const store = useStore();

// Get student data from Vuex store
const student = computed(() => store.getters["auth/currentStudent"]);
const courses = computed(() => store.getters["courses/enrolledCourses"]);

// Loading state from store
const isLoading = computed(() => store.getters.isLoading("courses"));

// Current courses with null safety
const currentCourses = computed(() => courses.value || []);

const studentName = computed(() => {
  if (student.value) {
    return `${student.value.firstName} ${student.value.lastName}`;
  }
  return "Test Student";
});

const currentDate = computed(() => {
  return new Date().toLocaleDateString("en-US", {
    weekday: "long",
    year: "numeric",
    month: "long",
    day: "numeric",
  });
});

// Initialize course data on component mount
onMounted(async () => {
  try {
    store.dispatch("setLoading", { courses: true });
    await store.dispatch("courses/fetchEnrolledCourses");
  } catch (error) {
    console.error("Error fetching enrolled courses:", error);
  } finally {
    store.dispatch("setLoading", { courses: false });
  }
});

// Student statistics from store
const enrolledCoursesCount = computed(
  () => (courses.value && courses.value.length) || 0
);
const currentGPA = computed(() => student.value?.gpa?.toString() || "3.85");
const completedCredits = computed(() => 45); // This could come from student profile
const upcomingDeadlines = computed(() => 2); // This could be calculated from course data

const getStatusBadgeClass = (status: string) => {
  switch (status) {
    case "Enrolled":
      return "bg-success";
    case "Waitlisted":
      return "bg-warning";
    case "Completed":
      return "bg-primary";
    case "Dropped":
      return "bg-secondary";
    default:
      return "bg-secondary";
  }
};

// Helper function to display instructor information with fallbacks
const getInstructorDisplay = (instructor: string | undefined) => {
  if (
    !instructor ||
    instructor === "TBA" ||
    instructor === "Instructor to be announced"
  ) {
    return "Instructor to be announced";
  }
  return instructor;
};
</script>

<style scoped>
.card {
  border: none;
  box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075);
}

.list-group-item:last-child {
  border-bottom: none;
}
</style>
