<template>
  <div class="courses-page">
    <div class="container-fluid py-4">
      <!-- Page Header -->
      <div class="row mb-4">
        <div class="col-12">
          <div class="d-flex justify-content-between align-items-center">
            <h1>My Courses</h1>
            <button class="btn btn-primary" @click="refreshCourses">
              <i class="bi bi-arrow-clockwise me-2"></i>
              Refresh
            </button>
          </div>
        </div>
      </div>

      <!-- Filter and Search -->
      <div class="row mb-4">
        <div class="col-md-8">
          <div class="input-group">
            <span class="input-group-text">
              <i class="bi bi-search"></i>
            </span>
            <input
              v-model="searchQuery"
              type="text"
              class="form-control"
              placeholder="Search courses by name, code, or instructor..."
            />
          </div>
        </div>
        <div class="col-md-4">
          <select v-model="statusFilter" class="form-select">
            <option value="">All Status</option>
            <option value="Enrolled">Enrolled</option>
            <option value="Completed">Completed</option>
            <option value="Waitlisted">Waitlisted</option>
            <option value="Dropped">Dropped</option>
          </select>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="isLoading" class="text-center py-5">
        <div class="spinner-border text-primary" role="status">
          <span class="visually-hidden">Loading courses...</span>
        </div>
        <p class="mt-2 text-muted">Loading your courses...</p>
      </div>

      <!-- No Courses State -->
      <div v-else-if="filteredCourses.length === 0" class="text-center py-5">
        <i class="bi bi-book" style="font-size: 4rem; color: #6c757d"></i>
        <h3 class="mt-3 text-muted">No courses found</h3>
        <p class="text-muted">
          {{
            searchQuery || statusFilter
              ? "Try adjusting your search criteria."
              : "You haven't enrolled in any courses yet."
          }}
        </p>
      </div>

      <!-- Courses Grid -->
      <div v-else class="row g-4">
        <div
          v-for="course in filteredCourses"
          :key="course.id"
          class="col-lg-6 col-xl-4"
        >
          <div class="card h-100 course-card">
            <div
              class="card-header d-flex justify-content-between align-items-center"
            >
              <div>
                <h6 class="mb-0">{{ course.code }}</h6>
                <small class="text-muted">{{ course.credits }} Credits</small>
              </div>
              <span
                class="badge"
                :class="getStatusBadgeClass(course.enrollmentStatus)"
              >
                {{ course.enrollmentStatus }}
              </span>
            </div>

            <div class="card-body">
              <h5 class="card-title">{{ course.name }}</h5>
              <p class="card-text text-muted">{{ course.description }}</p>

              <div class="mb-3">
                <small class="text-muted">
                  <i class="bi bi-person-fill me-1"></i>
                  {{ getInstructorDisplay(course.instructor) }}
                </small>
              </div>

              <!-- Schedule -->
              <div class="mb-3">
                <small class="text-muted d-block mb-1">Schedule:</small>
                <div v-if="course.schedule && course.schedule.length > 0">
                  <div
                    v-for="schedule in course.schedule"
                    :key="`${schedule.dayOfWeek}-${schedule.startTime}`"
                  >
                    <small class="text-dark">
                      {{ schedule.dayOfWeek }} {{ schedule.startTime }} -
                      {{ schedule.endTime }}
                      <br />
                      <span class="text-muted">
                        <i class="bi bi-geo-alt me-1"></i>
                        {{ getLocationDisplay(schedule.location) }}
                      </span>
                    </small>
                  </div>
                </div>
                <div v-else>
                  <small class="text-muted">
                    <i class="bi bi-calendar me-1"></i>
                    Schedule to be determined
                  </small>
                </div>
              </div>
            </div>

            <div class="card-footer bg-transparent">
              <div class="d-flex gap-2">
                <button
                  class="btn btn-outline-primary btn-sm flex-fill"
                  @click="viewCourseDetails(course)"
                >
                  <i class="bi bi-eye me-1"></i>
                  View Details
                </button>
                <button
                  v-if="course.enrollmentStatus === 'Enrolled'"
                  class="btn btn-outline-danger btn-sm"
                  @click="dropCourse(course)"
                >
                  <i class="bi bi-x-circle me-1"></i>
                  Drop
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Pagination -->
      <div v-if="totalPages > 1" class="row mt-4">
        <div class="col-12">
          <nav aria-label="Courses pagination">
            <ul class="pagination justify-content-center">
              <li class="page-item" :class="{ disabled: currentPage === 1 }">
                <button class="page-link" @click="goToPage(currentPage - 1)">
                  Previous
                </button>
              </li>
              <li
                v-for="page in totalPages"
                :key="page"
                class="page-item"
                :class="{ active: page === currentPage }"
              >
                <button class="page-link" @click="goToPage(page)">
                  {{ page }}
                </button>
              </li>
              <li
                class="page-item"
                :class="{ disabled: currentPage === totalPages }"
              >
                <button class="page-link" @click="goToPage(currentPage + 1)">
                  Next
                </button>
              </li>
            </ul>
          </nav>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import { useStore, type Store } from "vuex";

interface Schedule {
  dayOfWeek: string;
  startTime: string;
  endTime: string;
  location: string;
}

interface Course {
  id: string | number;
  code: string;
  name?: string;
  title?: string;
  description: string;
  credits: number;
  instructor: string;
  enrollmentStatus: "Enrolled" | "Completed" | "Waitlisted" | "Dropped";
  schedule: Schedule[];
}

interface RootState {
  courses: {
    enrolledCourses: Course[];
    isLoading: boolean;
  };
}

const store: Store<RootState> = useStore();

const searchQuery = ref<string>("");
const statusFilter = ref<string>("");
const currentPage = ref<number>(1);
const pageSize = ref<number>(9);

// Get courses from Vuex store
const courses = computed<Course[]>(
  () => store.getters["courses/enrolledCourses"]
);
const isLoading = computed<boolean>(() => store.getters["courses/isLoading"]);

// Initialize courses on component mount
onMounted(() => {
  store.dispatch("courses/fetchEnrolledCourses");
});

const filteredCourses = computed<Course[]>(() => {
  let filtered: Course[] = courses.value;

  if (searchQuery.value) {
    const query: string = searchQuery.value.toLowerCase();
    filtered = filtered.filter(
      (course: Course) =>
        (course.title || course.name || "").toLowerCase().includes(query) ||
        course.code.toLowerCase().includes(query) ||
        course.instructor.toLowerCase().includes(query)
    );
  }

  if (statusFilter.value) {
    filtered = filtered.filter(
      (course: Course) => course.enrollmentStatus === statusFilter.value
    );
  }

  const startIndex: number = (currentPage.value - 1) * pageSize.value;
  return filtered.slice(startIndex, startIndex + pageSize.value);
});

const totalPages = computed<number>(() => {
  let filtered: Course[] = courses.value;

  if (searchQuery.value) {
    const query: string = searchQuery.value.toLowerCase();
    filtered = filtered.filter(
      (course: Course) =>
        (course.title || course.name || "").toLowerCase().includes(query) ||
        course.code.toLowerCase().includes(query) ||
        course.instructor.toLowerCase().includes(query)
    );
  }

  if (statusFilter.value) {
    filtered = filtered.filter(
      (course: Course) => course.enrollmentStatus === statusFilter.value
    );
  }

  return Math.ceil(filtered.length / pageSize.value);
});

const getStatusBadgeClass = (status: Course["enrollmentStatus"]): string => {
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

const refreshCourses = async (): Promise<void> => {
  await store.dispatch("courses/fetchEnrolledCourses");
};

const viewCourseDetails = (course: Course): void => {
  console.log("Viewing course details:", course);
  // In a real app, navigate to course details page
  console.log("Viewing course details:", course.name || course.title);
};

const dropCourse = async (course: Course): Promise<void> => {
  if (
    confirm(`Are you sure you want to drop "${course.name || course.title}"?`)
  ) {
    try {
      await store.dispatch("courses/dropCourse", course.id);
      // Refresh courses list after dropping
      await refreshCourses();
    } catch (error: unknown) {
      console.error("Failed to drop course:", error);
      alert("Failed to drop course. Please try again.");
    }
  }
};

const goToPage = (page: number): void => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page;
  }
};

// Helper functions for better data display
const getInstructorDisplay = (instructor: string | undefined): string => {
  if (
    !instructor ||
    instructor === "TBA" ||
    instructor === "Instructor to be announced"
  ) {
    return "Instructor to be announced";
  }
  return instructor;
};

const getLocationDisplay = (location: string | undefined): string => {
  if (!location || location === "TBA") {
    return "Room assignment pending";
  }
  return location;
};
</script>

<style scoped>
.course-card {
  border: none;
  box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075);
  transition: all 0.2s ease-in-out;
}

.course-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.15);
}

.page-link {
  color: var(--zeus-primary);
  border-color: #dee2e6;
}

.page-link:hover {
  color: var(--zeus-secondary);
  background-color: #e9ecef;
}

.page-item.active .page-link {
  background-color: var(--zeus-primary);
  border-color: var(--zeus-primary);
}
</style>
