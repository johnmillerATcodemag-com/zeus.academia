<template>
  <div
    class="modal fade show d-block"
    tabindex="-1"
    role="dialog"
    style="background-color: rgba(0, 0, 0, 0.5)"
  >
    <div class="modal-dialog modal-xl" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">
            <i class="fas fa-balance-scale me-2"></i>
            Course Comparison
          </h5>
          <button
            type="button"
            class="btn-close"
            @click="$emit('close')"
          ></button>
        </div>
        <div class="modal-body">
          <div v-if="courses.length === 0" class="text-center py-4">
            <p class="text-muted">No courses selected for comparison.</p>
          </div>
          <div v-else class="comparison-table-container">
            <div class="table-responsive">
              <table class="table table-bordered comparison-table">
                <thead class="table-light">
                  <tr>
                    <th class="attribute-column">Attribute</th>
                    <th
                      v-for="course in courses"
                      :key="course.id"
                      class="course-column"
                    >
                      <div class="course-header">
                        <h6 class="mb-1">{{ course.code }}</h6>
                        <small class="text-muted">{{ course.name }}</small>
                      </div>
                    </th>
                  </tr>
                </thead>
                <tbody>
                  <!-- Basic Information -->
                  <tr>
                    <td class="attribute-label">
                      <strong>
                        <i class="fas fa-info-circle me-1"></i>
                        Department
                      </strong>
                    </td>
                    <td v-for="course in courses" :key="`dept-${course.id}`">
                      {{ course.department }}
                    </td>
                  </tr>
                  <tr>
                    <td class="attribute-label">
                      <strong>
                        <i class="fas fa-credit-card me-1"></i>
                        Credits
                      </strong>
                    </td>
                    <td v-for="course in courses" :key="`credits-${course.id}`">
                      <span class="badge bg-info"
                        >{{ course.credits }} Credits</span
                      >
                    </td>
                  </tr>
                  <tr>
                    <td class="attribute-label">
                      <strong>
                        <i class="fas fa-user me-1"></i>
                        Instructor
                      </strong>
                    </td>
                    <td
                      v-for="course in courses"
                      :key="`instructor-${course.id}`"
                    >
                      {{ course.instructor }}
                    </td>
                  </tr>

                  <!-- Schedule Information -->
                  <tr>
                    <td class="attribute-label">
                      <strong>
                        <i class="fas fa-calendar me-1"></i>
                        Schedule
                      </strong>
                    </td>
                    <td
                      v-for="course in courses"
                      :key="`schedule-${course.id}`"
                    >
                      <div v-if="course.schedule && course.schedule.length > 0">
                        <div
                          v-for="(session, index) in course.schedule"
                          :key="index"
                          class="mb-1"
                        >
                          <div class="small">
                            <strong>{{ session.dayOfWeek }}</strong>
                          </div>
                          <div class="small text-muted">
                            {{ session.startTime }} - {{ session.endTime }}
                          </div>
                        </div>
                      </div>
                      <div v-else class="text-muted small">TBA</div>
                    </td>
                  </tr>
                  <tr>
                    <td class="attribute-label">
                      <strong>
                        <i class="fas fa-map-marker-alt me-1"></i>
                        Location
                      </strong>
                    </td>
                    <td
                      v-for="course in courses"
                      :key="`location-${course.id}`"
                    >
                      {{ course.schedule?.[0]?.location || "TBA" }}
                    </td>
                  </tr>

                  <!-- Prerequisites -->
                  <tr>
                    <td class="attribute-label">
                      <strong>
                        <i class="fas fa-list-check me-1"></i>
                        Prerequisites
                      </strong>
                    </td>
                    <td v-for="course in courses" :key="`prereq-${course.id}`">
                      <div
                        v-if="
                          course.prerequisites &&
                          course.prerequisites.length > 0
                        "
                      >
                        <span
                          v-for="prereq in course.prerequisites"
                          :key="prereq.code"
                          class="badge bg-secondary me-1 mb-1"
                        >
                          {{ prereq.code }}
                        </span>
                      </div>
                      <div v-else class="text-muted small">None</div>
                    </td>
                  </tr>

                  <!-- Enrollment Information -->
                  <tr>
                    <td class="attribute-label">
                      <strong>
                        <i class="fas fa-users me-1"></i>
                        Enrollment
                      </strong>
                    </td>
                    <td
                      v-for="course in courses"
                      :key="`enrollment-${course.id}`"
                    >
                      <div class="enrollment-info">
                        <div class="mb-1">
                          <span
                            class="badge"
                            :class="
                              getEnrollmentStatusClass(course.enrollmentStatus)
                            "
                          >
                            {{
                              getEnrollmentStatusText(course.enrollmentStatus)
                            }}
                          </span>
                        </div>
                        <div class="small text-muted">
                          {{ course.enrolledStudents || 0 }}/{{
                            course.maxEnrollment || 0
                          }}
                          enrolled
                        </div>
                        <div
                          v-if="course.waitlistCount > 0"
                          class="small text-muted"
                        >
                          {{ course.waitlistCount }} on waitlist
                        </div>
                      </div>
                    </td>
                  </tr>

                  <!-- Difficulty and Workload -->
                  <tr v-if="courses.some((c: Course) => c.difficulty)">
                    <td class="attribute-label">
                      <strong>
                        <i class="fas fa-chart-line me-1"></i>
                        Difficulty
                      </strong>
                    </td>
                    <td
                      v-for="course in courses"
                      :key="`difficulty-${course.id}`"
                    >
                      <div v-if="course.difficulty">
                        <div class="difficulty-rating">
                          <span
                            v-for="i in 5"
                            :key="i"
                            class="star"
                            :class="{ filled: i <= (course.difficulty || 0) }"
                          >
                            <i class="fas fa-star"></i>
                          </span>
                        </div>
                        <div class="small text-muted">
                          {{ course.difficulty }}/5
                        </div>
                      </div>
                      <div v-else class="text-muted small">Not rated</div>
                    </td>
                  </tr>

                  <tr v-if="courses.some((c: Course) => c.weeklyWorkload)">
                    <td class="attribute-label">
                      <strong>
                        <i class="fas fa-clock me-1"></i>
                        Weekly Workload
                      </strong>
                    </td>
                    <td
                      v-for="course in courses"
                      :key="`workload-${course.id}`"
                    >
                      <div v-if="course.weeklyWorkload">
                        {{ course.weeklyWorkload }} hours/week
                      </div>
                      <div v-else class="text-muted small">Not specified</div>
                    </td>
                  </tr>

                  <!-- Description -->
                  <tr>
                    <td class="attribute-label">
                      <strong>
                        <i class="fas fa-file-alt me-1"></i>
                        Description
                      </strong>
                    </td>
                    <td
                      v-for="course in courses"
                      :key="`desc-${course.id}`"
                      class="description-cell"
                    >
                      <div class="description-text">
                        {{ course.description }}
                      </div>
                    </td>
                  </tr>

                  <!-- Actions -->
                  <tr>
                    <td class="attribute-label">
                      <strong>
                        <i class="fas fa-cog me-1"></i>
                        Actions
                      </strong>
                    </td>
                    <td v-for="course in courses" :key="`actions-${course.id}`">
                      <div class="d-grid gap-2">
                        <button
                          class="btn btn-primary btn-sm"
                          @click="$emit('enroll', course)"
                          :disabled="!canEnroll(course)"
                          v-if="course.enrollmentStatus !== 'Enrolled'"
                        >
                          <i class="fas fa-plus me-1"></i>
                          <span v-if="course.enrollmentStatus === 'available'"
                            >Enroll Now</span
                          >
                          <span
                            v-else-if="course.enrollmentStatus === 'waitlist'"
                            >Join Waitlist</span
                          >
                          <span v-else>Unavailable</span>
                        </button>

                        <button v-else class="btn btn-success btn-sm" disabled>
                          <i class="fas fa-check me-1"></i>
                          Enrolled
                        </button>

                        <button
                          class="btn btn-outline-info btn-sm"
                          @click="viewDetails(course)"
                        >
                          <i class="fas fa-info-circle me-1"></i>
                          View Details
                        </button>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- Comparison Summary -->
          <div v-if="courses.length > 1" class="comparison-summary mt-4">
            <h6>
              <i class="fas fa-lightbulb me-2"></i>
              Comparison Summary
            </h6>
            <div class="row g-3">
              <div class="col-md-4">
                <div class="summary-card card border-info">
                  <div class="card-body text-center">
                    <i class="fas fa-award fa-2x text-info mb-2"></i>
                    <h6>Lowest Difficulty</h6>
                    <p class="mb-0">
                      {{ getLowestDifficultyCourse()?.code || "N/A" }}
                    </p>
                  </div>
                </div>
              </div>
              <div class="col-md-4">
                <div class="summary-card card border-success">
                  <div class="card-body text-center">
                    <i class="fas fa-clock fa-2x text-success mb-2"></i>
                    <h6>Least Workload</h6>
                    <p class="mb-0">
                      {{ getLeastWorkloadCourse()?.code || "N/A" }}
                    </p>
                  </div>
                </div>
              </div>
              <div class="col-md-4">
                <div class="summary-card card border-primary">
                  <div class="card-body text-center">
                    <i class="fas fa-users fa-2x text-primary mb-2"></i>
                    <h6>Most Available</h6>
                    <p class="mb-0">
                      {{ getMostAvailableCourse()?.code || "N/A" }}
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button
            type="button"
            class="btn btn-secondary"
            @click="$emit('close')"
          >
            Close Comparison
          </button>
          <button
            v-if="courses.length > 0"
            type="button"
            class="btn btn-primary"
            @click="enrollInRecommended"
            :disabled="!getRecommendedCourse()"
          >
            <i class="fas fa-star me-1"></i>
            Enroll in Recommended
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Course } from "../types";

// Props and Emits
const props = defineProps<{
  courses: Course[];
}>();

const emit = defineEmits<{
  close: [];
  enroll: [course: Course];
}>();

// Methods
const getEnrollmentStatusClass = (status: any) => {
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

const getEnrollmentStatusText = (status: any) => {
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
      return "Unknown";
  }
};

const canEnroll = (course: Course) => {
  return (
    (course.enrollmentStatus as any) === "available" ||
    (course.enrollmentStatus as any) === "waitlist"
  );
};

const viewDetails = (course: Course) => {
  // This would open the course details in a new modal or navigate to details page
  window.open(`/courses/${course.id}`, "_blank");
};

// Comparison analysis methods
const getLowestDifficultyCourse = () => {
  return props.courses
    .filter((c: Course) => c.difficulty)
    .sort(
      (a: Course, b: Course) => (a.difficulty || 0) - (b.difficulty || 0)
    )[0];
};

const getLeastWorkloadCourse = () => {
  return props.courses
    .filter((c: Course) => c.weeklyWorkload)
    .sort(
      (a: Course, b: Course) =>
        (a.weeklyWorkload || 0) - (b.weeklyWorkload || 0)
    )[0];
};

const getMostAvailableCourse = () => {
  return props.courses
    .filter((c: Course) => (c.enrollmentStatus as any) === "available")
    .sort((a: Course, b: Course) => {
      const aAvailable = (a.maxEnrollment || 0) - (a.enrolledStudents || 0);
      const bAvailable = (b.maxEnrollment || 0) - (b.enrolledStudents || 0);
      return bAvailable - aAvailable;
    })[0];
};

const getRecommendedCourse = () => {
  // Simple recommendation algorithm based on availability and difficulty
  const availableCourses = props.courses.filter((c: Course) => canEnroll(c));
  if (availableCourses.length === 0) return null;

  return availableCourses.sort((a: Course, b: Course) => {
    const aScore =
      ((a.enrollmentStatus as any) === "available" ? 10 : 5) -
      (a.difficulty || 0);
    const bScore =
      ((b.enrollmentStatus as any) === "available" ? 10 : 5) -
      (b.difficulty || 0);
    return bScore - aScore;
  })[0];
};

const enrollInRecommended = () => {
  const recommended = getRecommendedCourse();
  if (recommended) {
    emit("enroll", recommended);
  }
};
</script>

<style scoped>
.comparison-table-container {
  max-height: 70vh;
  overflow-y: auto;
}

.comparison-table {
  min-width: 800px;
}

.comparison-table th,
.comparison-table td {
  vertical-align: top;
  padding: 1rem;
}

.attribute-column {
  width: 200px;
  min-width: 200px;
  background-color: var(--bs-light);
}

.course-column {
  min-width: 250px;
}

.course-header {
  text-align: center;
}

.attribute-label {
  font-weight: 500;
  background-color: rgba(var(--bs-light-rgb), 0.5);
}

.description-cell {
  max-width: 300px;
}

.description-text {
  max-height: 100px;
  overflow-y: auto;
  font-size: 0.875rem;
  line-height: 1.4;
}

.enrollment-info {
  text-align: center;
}

.difficulty-rating .star {
  color: #ddd;
  margin-right: 2px;
}

.difficulty-rating .star.filled {
  color: #ffc107;
}

.summary-card {
  transition: transform 0.2s ease;
}

.summary-card:hover {
  transform: translateY(-2px);
}

.modal-xl {
  max-width: 95vw;
}

@media (max-width: 768px) {
  .modal-xl {
    max-width: 100vw;
    margin: 0;
  }

  .comparison-table-container {
    max-height: 50vh;
  }

  .attribute-column {
    width: 150px;
    min-width: 150px;
  }

  .course-column {
    min-width: 200px;
  }

  .comparison-table th,
  .comparison-table td {
    padding: 0.5rem;
    font-size: 0.875rem;
  }
}
</style>
