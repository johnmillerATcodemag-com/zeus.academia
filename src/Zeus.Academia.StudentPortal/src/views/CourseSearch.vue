<template>
  <div class="course-search">
    <!-- Search Header -->
    <div class="search-header mb-4">
      <h2 class="mb-3">
        <i class="fas fa-search me-2"></i>
        Course Search & Enrollment
      </h2>
      <p class="text-muted">
        Find and enroll in courses for the upcoming semester
      </p>
    </div>

    <!-- Search Filters -->
    <div class="search-filters card mb-4">
      <div class="card-header">
        <h5 class="mb-0">
          <i class="fas fa-filter me-2"></i>
          Search Filters
        </h5>
      </div>
      <div class="card-body">
        <form @submit.prevent="searchCourses">
          <div class="row g-3">
            <!-- Text Search -->
            <div class="col-md-6">
              <label for="searchQuery" class="form-label"
                >Search Keywords</label
              >
              <input
                id="searchQuery"
                v-model="searchFilters.query"
                type="text"
                class="form-control"
                placeholder="Course name, code, or keywords..."
                @input="debounceSearch"
              />
            </div>

            <!-- Department -->
            <div class="col-md-6">
              <label for="department" class="form-label">Department</label>
              <select
                id="department"
                v-model="searchFilters.department"
                class="form-select"
                @change="searchCourses"
              >
                <option value="">All Departments</option>
                <option value="CS">Computer Science</option>
                <option value="MATH">Mathematics</option>
                <option value="ENG">English</option>
                <option value="HIST">History</option>
                <option value="PHYS">Physics</option>
                <option value="CHEM">Chemistry</option>
                <option value="BIO">Biology</option>
                <option value="ECON">Economics</option>
              </select>
            </div>

            <!-- Level -->
            <div class="col-md-3">
              <label for="level" class="form-label">Course Level</label>
              <select
                id="level"
                v-model="searchFilters.level"
                class="form-select"
                @change="searchCourses"
              >
                <option value="">All Levels</option>
                <option value="100">100 - Introductory</option>
                <option value="200">200 - Intermediate</option>
                <option value="300">300 - Advanced</option>
                <option value="400">400 - Senior</option>
                <option value="500">500+ - Graduate</option>
              </select>
            </div>

            <!-- Credits -->
            <div class="col-md-3">
              <label for="credits" class="form-label">Credits</label>
              <select
                id="credits"
                v-model="searchFilters.credits"
                class="form-select"
                @change="searchCourses"
              >
                <option value="">Any Credits</option>
                <option :value="1">1 Credit</option>
                <option :value="2">2 Credits</option>
                <option :value="3">3 Credits</option>
                <option :value="4">4 Credits</option>
                <option :value="5">5+ Credits</option>
              </select>
            </div>

            <!-- Instructor -->
            <div class="col-md-6">
              <label for="instructor" class="form-label">Instructor</label>
              <input
                id="instructor"
                v-model="searchFilters.instructor"
                type="text"
                class="form-control"
                placeholder="Instructor name..."
                @input="debounceSearch"
              />
            </div>

            <!-- Day of Week -->
            <div class="col-md-6">
              <label class="form-label">Days</label>
              <div class="d-flex gap-2 flex-wrap">
                <div
                  class="form-check"
                  v-for="day in daysOfWeek"
                  :key="day.value"
                >
                  <input
                    :id="`day-${day.value}`"
                    v-model="searchFilters.days"
                    :value="day.value"
                    class="form-check-input"
                    type="checkbox"
                    @change="searchCourses"
                  />
                  <label :for="`day-${day.value}`" class="form-check-label">
                    {{ day.label }}
                  </label>
                </div>
              </div>
            </div>

            <!-- Time Range -->
            <div class="col-md-6">
              <label class="form-label">Time Range</label>
              <div class="row g-2">
                <div class="col">
                  <input
                    v-model="searchFilters.startTime"
                    type="time"
                    class="form-control"
                    @change="searchCourses"
                  />
                </div>
                <div class="col-auto d-flex align-items-center">
                  <span class="text-muted">to</span>
                </div>
                <div class="col">
                  <input
                    v-model="searchFilters.endTime"
                    type="time"
                    class="form-control"
                    @change="searchCourses"
                  />
                </div>
              </div>
            </div>

            <!-- Enrollment Status -->
            <div class="col-md-6">
              <label for="enrollmentStatus" class="form-label"
                >Availability</label
              >
              <select
                id="enrollmentStatus"
                v-model="searchFilters.availability"
                class="form-select"
                @change="searchCourses"
              >
                <option value="">All Courses</option>
                <option value="available">Available to Enroll</option>
                <option value="waitlist">Waitlist Available</option>
                <option value="full">Full (No Waitlist)</option>
              </select>
            </div>

            <!-- Clear Filters -->
            <div class="col-12">
              <div class="d-flex gap-2">
                <button
                  type="button"
                  class="btn btn-outline-secondary"
                  @click="clearFilters"
                >
                  <i class="fas fa-times me-1"></i>
                  Clear Filters
                </button>
                <button
                  type="submit"
                  class="btn btn-primary"
                  :disabled="isLoading"
                >
                  <i class="fas fa-search me-1"></i>
                  <span v-if="isLoading">Searching...</span>
                  <span v-else>Search Courses</span>
                </button>
              </div>
            </div>
          </div>
        </form>
      </div>
    </div>

    <!-- Search Results Actions -->
    <div
      class="search-actions d-flex justify-content-between align-items-center mb-3"
      v-if="searchResults.length > 0"
    >
      <div class="results-info">
        <span class="text-muted">
          Showing {{ searchResults.length }} of {{ totalResults }} courses
        </span>
      </div>
      <div class="view-actions d-flex gap-2">
        <button
          class="btn btn-outline-primary btn-sm"
          :class="{ active: viewMode === 'list' }"
          @click="viewMode = 'list'"
        >
          <i class="fas fa-list"></i>
          List View
        </button>
        <button
          class="btn btn-outline-primary btn-sm"
          :class="{ active: viewMode === 'grid' }"
          @click="viewMode = 'grid'"
        >
          <i class="fas fa-th"></i>
          Grid View
        </button>
        <button
          class="btn btn-outline-success btn-sm"
          :class="{ active: showComparison }"
          @click="toggleComparison"
          v-if="selectedForComparison.length > 0"
        >
          <i class="fas fa-balance-scale"></i>
          Compare ({{ selectedForComparison.length }})
        </button>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="text-center py-4">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading courses...</span>
      </div>
      <p class="mt-2 text-muted">Searching for courses...</p>
    </div>

    <!-- Empty State -->
    <div
      v-else-if="searchResults.length === 0 && hasSearched"
      class="empty-state text-center py-5"
    >
      <i class="fas fa-search fa-3x text-muted mb-3"></i>
      <h5>No courses found</h5>
      <p class="text-muted">
        Try adjusting your search filters to find more courses.
      </p>
    </div>

    <!-- Search Results -->
    <div v-else-if="searchResults.length > 0" class="search-results">
      <!-- List View -->
      <div v-if="viewMode === 'list'" class="course-list">
        <div
          v-for="course in searchResults"
          :key="course.id"
          class="course-item card mb-3"
          :class="{
            'border-primary': selectedForComparison.includes(course.id),
          }"
        >
          <div class="card-body">
            <div class="row">
              <div class="col-md-8">
                <div
                  class="d-flex justify-content-between align-items-start mb-2"
                >
                  <div>
                    <h5 class="course-title mb-1">
                      <router-link
                        :to="`/courses/${course.id}`"
                        class="text-decoration-none"
                      >
                        {{ course.code }} - {{ course.title }}
                      </router-link>
                    </h5>
                    <p class="course-instructor text-muted mb-2">
                      <i class="fas fa-user me-1"></i>
                      {{ course.instructor || "Instructor to be announced" }}
                    </p>
                  </div>
                  <div class="course-actions">
                    <div class="form-check">
                      <input
                        :id="`compare-${course.id}`"
                        v-model="selectedForComparison"
                        :value="course.id"
                        class="form-check-input"
                        type="checkbox"
                      />
                      <label
                        :for="`compare-${course.id}`"
                        class="form-check-label text-sm"
                      >
                        Compare
                      </label>
                    </div>
                  </div>
                </div>

                <p class="course-description text-muted mb-2">
                  {{ course.description || getDerivedDescription(course) }}
                </p>

                <div class="course-details d-flex flex-wrap gap-3 mb-3">
                  <span class="badge bg-primary">
                    <i class="fas fa-building me-1"></i>
                    {{ getDepartment(course.code) }}
                  </span>
                  <span class="badge bg-info">
                    <i class="fas fa-credit-card me-1"></i>
                    {{ course.credits }} Credits
                  </span>
                  <span class="badge bg-secondary">
                    <i class="fas fa-layer-group me-1"></i>
                    {{ getAcademicLevel(course.code) }}
                  </span>
                  <span class="text-muted small">
                    <i class="fas fa-calendar me-1"></i>
                    {{ formatSchedule(course.schedule) }}
                  </span>
                  <span class="text-muted small">
                    <i class="fas fa-map-marker-alt me-1"></i>
                    {{
                      course.schedule?.[0]?.location ||
                      "Room assignment pending"
                    }}
                  </span>
                </div>
              </div>

              <div class="col-md-4 text-end">
                <div class="enrollment-status mb-3">
                  <span
                    class="badge"
                    :class="getEnrollmentStatusClass(course.enrollmentStatus)"
                  >
                    {{ getEnrollmentStatusText(course.enrollmentStatus) }}
                  </span>
                </div>

                <div class="course-actions d-flex flex-column gap-2">
                  <button
                    class="btn btn-primary btn-sm"
                    @click="enrollInCourse(course)"
                    :disabled="!canEnroll(course) || isEnrolling"
                    v-if="course.enrollmentStatus !== 'Enrolled'"
                  >
                    <i class="fas fa-plus me-1"></i>
                    <span v-if="course.enrollmentStatus === 'available'"
                      >Enroll Now</span
                    >
                    <span v-else-if="course.enrollmentStatus === 'waitlist'"
                      >Join Waitlist</span
                    >
                    <span v-else>Registration closed</span>
                  </button>

                  <button v-else class="btn btn-success btn-sm" disabled>
                    <i class="fas fa-check me-1"></i>
                    Enrolled
                  </button>

                  <button
                    class="btn btn-outline-info btn-sm"
                    @click="viewCourseDetails(course)"
                  >
                    <i class="fas fa-info-circle me-1"></i>
                    Details
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Grid View -->
      <div v-else class="course-grid">
        <div class="row g-3">
          <div
            v-for="course in searchResults"
            :key="course.id"
            class="col-lg-4 col-md-6"
          >
            <div
              class="course-card card h-100"
              :class="{
                'border-primary': selectedForComparison.includes(course.id),
              }"
            >
              <div class="card-header">
                <div class="d-flex justify-content-between align-items-center">
                  <h6 class="mb-0">{{ course.code }}</h6>
                  <div class="form-check">
                    <input
                      :id="`grid-compare-${course.id}`"
                      v-model="selectedForComparison"
                      :value="course.id"
                      class="form-check-input"
                      type="checkbox"
                    />
                    <label
                      :for="`grid-compare-${course.id}`"
                      class="form-check-label text-sm"
                    >
                      Compare
                    </label>
                  </div>
                </div>
              </div>
              <div class="card-body d-flex flex-column">
                <h6 class="card-title">{{ course.title }}</h6>
                <p class="card-text flex-grow-1 small">
                  {{ course.description || getDerivedDescription(course) }}
                </p>

                <div class="course-info mb-3">
                  <div class="small text-muted mb-1">
                    <i class="fas fa-user me-1"></i>
                    {{ course.instructor || "Instructor to be announced" }}
                  </div>
                  <div class="small text-muted mb-1">
                    <i class="fas fa-building me-1"></i>
                    {{ getDepartment(course.code) }}
                  </div>
                  <div class="small text-muted mb-1">
                    <i class="fas fa-credit-card me-1"></i>
                    {{ course.credits }} Credits
                  </div>
                  <div class="small text-muted">
                    <i class="fas fa-calendar me-1"></i>
                    {{ formatSchedule(course.schedule) }}
                  </div>
                </div>

                <div class="enrollment-actions">
                  <div
                    class="d-flex justify-content-between align-items-center mb-2"
                  >
                    <span
                      class="badge"
                      :class="getEnrollmentStatusClass(course.enrollmentStatus)"
                    >
                      {{ getEnrollmentStatusText(course.enrollmentStatus) }}
                    </span>
                  </div>

                  <div class="d-grid gap-2">
                    <button
                      class="btn btn-primary btn-sm"
                      @click="enrollInCourse(course)"
                      :disabled="!canEnroll(course) || isEnrolling"
                      v-if="course.enrollmentStatus !== 'Enrolled'"
                    >
                      <i class="fas fa-plus me-1"></i>
                      <span v-if="course.enrollmentStatus === 'available'"
                        >Enroll</span
                      >
                      <span v-else-if="course.enrollmentStatus === 'waitlist'"
                        >Waitlist</span
                      >
                      <span v-else>Closed</span>
                    </button>

                    <button v-else class="btn btn-success btn-sm" disabled>
                      <i class="fas fa-check me-1"></i>
                      Enrolled
                    </button>

                    <button
                      class="btn btn-outline-info btn-sm"
                      @click="viewCourseDetails(course)"
                    >
                      <i class="fas fa-info-circle me-1"></i>
                      Details
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Pagination -->
      <nav
        v-if="totalPages > 1"
        class="mt-4"
        aria-label="Course search pagination"
      >
        <ul class="pagination justify-content-center">
          <li class="page-item" :class="{ disabled: currentPage === 1 }">
            <button
              class="page-link"
              @click="goToPage(currentPage - 1)"
              :disabled="currentPage === 1"
            >
              Previous
            </button>
          </li>
          <li
            v-for="page in visiblePages"
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
            <button
              class="page-link"
              @click="goToPage(currentPage + 1)"
              :disabled="currentPage === totalPages"
            >
              Next
            </button>
          </li>
        </ul>
      </nav>
    </div>

    <!-- Course Comparison Modal -->
    <CourseComparison
      v-if="showComparison"
      :courses="comparisonCourses"
      @close="closeComparison"
      @enroll="enrollInCourse"
    />

    <!-- Course Details Modal -->
    <CourseDetails
      v-if="selectedCourse"
      :course="selectedCourse"
      @close="closeCourseDetails"
      @enroll="enrollInCourse"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from "vue";
import { CourseService } from "../services/CourseService";
import type { Course, ApiResponse } from "../types";
import CourseComparison from "../components/CourseComparison.vue";
import CourseDetails from "../components/CourseDetails.vue";
import { useToast } from "../composables/useToast";

// Reactive state
const searchFilters = ref({
  query: "",
  department: "",
  level: "",
  credits: null as number | null,
  instructor: "",
  days: [] as string[],
  startTime: "",
  endTime: "",
  availability: "",
});

const searchResults = ref<Course[]>([]);
const totalResults = ref(0);
const totalPages = ref(1);
const currentPage = ref(1);
const pageSize = ref(12);
const isLoading = ref(false);
const hasSearched = ref(false);
const viewMode = ref<"list" | "grid">("list");
const selectedForComparison = ref<string[]>([]);
const showComparison = ref(false);
const selectedCourse = ref<Course | null>(null);
const isEnrolling = ref(false);

// Days of the week for filter
const daysOfWeek = [
  { value: "Monday", label: "Mon" },
  { value: "Tuesday", label: "Tue" },
  { value: "Wednesday", label: "Wed" },
  { value: "Thursday", label: "Thu" },
  { value: "Friday", label: "Fri" },
  { value: "Saturday", label: "Sat" },
  { value: "Sunday", label: "Sun" },
];

// Toast composable
const { showToast } = useToast();

// Computed properties
const comparisonCourses = computed(() =>
  searchResults.value.filter((course: any) =>
    selectedForComparison.value.includes(course.id)
  )
);

const visiblePages = computed(() => {
  const delta = 2;
  const range = [];
  const start = Math.max(1, currentPage.value - delta);
  const end = Math.min(totalPages.value, currentPage.value + delta);

  for (let i = start; i <= end; i++) {
    range.push(i);
  }

  return range;
});

// Search debouncing
let searchTimeout: NodeJS.Timeout | null = null;

const debounceSearch = () => {
  if (searchTimeout) {
    clearTimeout(searchTimeout);
  }
  searchTimeout = setTimeout(() => {
    searchCourses();
  }, 300);
};

// Methods
const searchCourses = async (pageOrEvent: number | Event = 1) => {
  // Handle both page number and event parameters
  const page = typeof pageOrEvent === "number" ? pageOrEvent : 1;

  isLoading.value = true;
  hasSearched.value = true;
  currentPage.value = page;

  try {
    const params = {
      query: searchFilters.value.query,
      department: searchFilters.value.department,
      level: searchFilters.value.level,
      credits: searchFilters.value.credits,
      instructor: searchFilters.value.instructor,
      page: currentPage.value,
      pageSize: pageSize.value,
    };

    // Remove empty values
    Object.keys(params).forEach((key) => {
      if (
        params[key as keyof typeof params] === "" ||
        params[key as keyof typeof params] === null
      ) {
        delete params[key as keyof typeof params];
      }
    });

    const response: ApiResponse<Course[]> =
      await CourseService.getCoursesPaginated(params);

    console.log("Course search response:", response);

    if (response.success && response.data) {
      // Handle paginated response structure
      const paginatedData = response.data as any;
      console.log("Paginated data:", paginatedData);

      if (paginatedData.items && Array.isArray(paginatedData.items)) {
        // API returns paginated structure with items array
        let filteredResults = paginatedData.items;

        // Temporary client-side filtering since API search isn't working properly
        if (searchFilters.value.query) {
          const query = searchFilters.value.query.toLowerCase();
          filteredResults = paginatedData.items.filter(
            (course: any) =>
              course.title?.toLowerCase().includes(query) ||
              course.code?.toLowerCase().includes(query) ||
              course.description?.toLowerCase().includes(query)
          );
        }

        searchResults.value = filteredResults;
        totalResults.value = filteredResults.length;
        totalPages.value =
          Math.ceil(filteredResults.length / pageSize.value) || 1;
        console.log("Set search results:", searchResults.value);
      } else if (Array.isArray(response.data)) {
        // API returns direct array
        searchResults.value = response.data;
        totalResults.value = response.data.length;
        totalPages.value = 1;
      } else {
        // Fallback
        searchResults.value = [];
        totalResults.value = 0;
        totalPages.value = 1;
      }
    } else {
      searchResults.value = [];
      totalResults.value = 0;
      totalPages.value = 1;
      showToast("Failed to search courses", "error");
    }
  } catch (error) {
    console.error("Search error:", error);
    searchResults.value = [];
    totalResults.value = 0;
    totalPages.value = 1;
    showToast("Error searching courses", "error");
  } finally {
    isLoading.value = false;
  }
};

const clearFilters = () => {
  searchFilters.value = {
    query: "",
    department: "",
    level: "",
    credits: null,
    instructor: "",
    days: [],
    startTime: "",
    endTime: "",
    availability: "",
  };
  selectedForComparison.value = [];
  searchCourses();
};

const goToPage = (page: number) => {
  if (page >= 1 && page <= totalPages.value) {
    searchCourses(page);
  }
};

const formatSchedule = (schedule: any[] | string) => {
  if (!schedule) return "Schedule pending";

  // Handle string format (like "MWF 10:00-11:00")
  if (typeof schedule === "string") {
    return schedule;
  }

  // Handle array format
  if (Array.isArray(schedule)) {
    if (schedule.length === 0) return "Schedule pending";

    const days = schedule.map((s) => s.dayOfWeek.substring(0, 3)).join(", ");
    const time = schedule[0]
      ? `${schedule[0].startTime} - ${schedule[0].endTime}`
      : "Time pending";

    return `${days} ${time}`;
  }

  return "Schedule pending";
};

// Helper function to derive department from course code
const getDepartment = (courseCode: string) => {
  if (!courseCode) return "General Studies";

  const departments: { [key: string]: string } = {
    CS: "Computer Science",
    MATH: "Mathematics",
    PHYS: "Physics",
    CHEM: "Chemistry",
    BIOL: "Biology",
    ENGL: "English",
    HIST: "History",
    PSYC: "Psychology",
    ECON: "Economics",
    BUSN: "Business",
  };

  const prefix = courseCode.match(/^([A-Z]+)/)?.[1] || "";
  return departments[prefix] || prefix || "General Studies";
};

// Helper function to derive academic level from course code
const getAcademicLevel = (courseCode: string) => {
  if (!courseCode) return "Introductory";

  const number = courseCode.match(/(\d+)/)?.[1] || "";
  const firstDigit = parseInt(number.charAt(0));

  if (firstDigit >= 1 && firstDigit <= 2) return "Undergraduate";
  if (firstDigit >= 3 && firstDigit <= 4) return "Upper Division";
  if (firstDigit >= 5 && firstDigit <= 7) return "Graduate";
  if (firstDigit >= 8 && firstDigit <= 9) return "Advanced Graduate";

  return "Undergraduate";
};

// Helper function to provide more informative course descriptions
const getDerivedDescription = (course: any) => {
  if (!course) return "Course description not available.";

  const department = getDepartment(course.code);
  const level = getAcademicLevel(course.code);

  return `${level} course in ${department}. Detailed course description will be available upon enrollment.`;
};

const getEnrollmentStatusClass = (status: string | undefined) => {
  if (!status) return "bg-secondary";

  switch (status) {
    case "Enrolled":
      return "bg-success";
    case "available":
      return "bg-primary";
    case "waitlist":
      return "bg-warning";
    case "full":
      return "bg-danger";
    default:
      return "bg-secondary";
  }
};

const getEnrollmentStatusText = (status: string | undefined) => {
  if (!status) return "Registration pending";

  switch (status) {
    case "Enrolled":
      return "Enrolled";
    case "available":
      return "Available";
    case "waitlist":
      return "Waitlist";
    case "full":
      return "Full";
    default:
      return "Registration pending";
  }
};

const canEnroll = (_course: Course) => {
  // For minimal API, courses don't have enrollment status, so assume they're not available for enrollment
  return false;
};

const enrollInCourse = async (course: Course) => {
  if (isEnrolling.value) return;

  isEnrolling.value = true;

  try {
    // Validate prerequisites first
    const prerequisiteCheck = await CourseService.validatePrerequisites(
      course.id
    );
    if (!prerequisiteCheck.success || !prerequisiteCheck.data?.valid) {
      const missingPrereqs =
        prerequisiteCheck.data?.missingPrerequisites
          ?.map((p) => p.code)
          .join(", ") || "prerequisite information unavailable";
      showToast(
        `Cannot enroll: Missing prerequisites: ${missingPrereqs}`,
        "error"
      );
      return;
    }

    // Check availability
    const availabilityCheck = await CourseService.checkCourseAvailability(
      course.id
    );
    if (!availabilityCheck.success) {
      showToast("Cannot check course availability", "error");
      return;
    }

    let response: ApiResponse<any>;

    if (availabilityCheck.data?.available) {
      // Direct enrollment
      response = await CourseService.enrollInCourse(course.id);
      if (response.success) {
        showToast(`Successfully enrolled in ${course.code}`, "success");
        course.enrollmentStatus = "Enrolled" as any;
      } else {
        showToast("Failed to enroll in course", "error");
      }
    } else if (availabilityCheck.data?.waitlistAvailable) {
      // Waitlist enrollment
      response = await CourseService.waitlistCourse(course.id);
      if (response.success) {
        showToast(`Added to waitlist for ${course.code}`, "info");
        course.enrollmentStatus = "waitlist" as any;
      } else {
        showToast("Failed to join waitlist", "error");
      }
    } else {
      showToast("Course is not available for enrollment", "error");
    }
  } catch (error) {
    console.error("Enrollment error:", error);
    showToast("Error during enrollment", "error");
  } finally {
    isEnrolling.value = false;
  }
};

const viewCourseDetails = (course: Course) => {
  selectedCourse.value = course;
};

const closeCourseDetails = () => {
  selectedCourse.value = null;
};

const toggleComparison = () => {
  showComparison.value = !showComparison.value;
};

const closeComparison = () => {
  showComparison.value = false;
};

// Lifecycle
onMounted(() => {
  searchCourses();
});

// Watch for filter changes (except for text inputs which use debounce)
watch(
  () => [
    searchFilters.value.department,
    searchFilters.value.level,
    searchFilters.value.credits,
    searchFilters.value.days,
    searchFilters.value.startTime,
    searchFilters.value.endTime,
    searchFilters.value.availability,
  ],
  () => {
    if (hasSearched.value) {
      searchCourses();
    }
  },
  { deep: true }
);
</script>

<style scoped>
.course-search {
  padding: 1rem;
}

.search-filters .card-header {
  background-color: var(--bs-light);
  border-bottom: 1px solid var(--bs-border-color);
}

.course-item {
  transition: all 0.3s ease;
}

.course-item:hover {
  box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.15);
}

.course-card {
  transition: all 0.3s ease;
}

.course-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.15);
}

.course-title a {
  color: var(--bs-primary);
}

.course-title a:hover {
  color: var(--bs-primary);
  text-decoration: underline !important;
}

.btn.active {
  background-color: var(--bs-primary);
  border-color: var(--bs-primary);
  color: white;
}

.empty-state i {
  opacity: 0.5;
}

.badge {
  font-size: 0.75em;
}

@media (max-width: 768px) {
  .course-search {
    padding: 0.5rem;
  }

  .search-filters .row > div {
    margin-bottom: 1rem;
  }

  .course-actions {
    margin-top: 1rem;
  }

  .view-actions {
    flex-wrap: wrap;
  }
}
</style>
