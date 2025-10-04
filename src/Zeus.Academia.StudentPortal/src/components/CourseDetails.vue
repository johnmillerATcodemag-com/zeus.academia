<template>
  <div
    class="modal fade show d-block"
    tabindex="-1"
    role="dialog"
    style="background-color: rgba(0, 0, 0, 0.5)"
  >
    <div class="modal-dialog modal-lg" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">
            <i class="fas fa-info-circle me-2"></i>
            Course Details
          </h5>
          <button
            type="button"
            class="btn-close"
            @click="$emit('close')"
          ></button>
        </div>
        <div class="modal-body">
          <div v-if="course" class="course-details">
            <!-- Course Header -->
            <div class="course-header mb-4">
              <div class="row">
                <div class="col-md-8">
                  <h3 class="course-title mb-2">
                    {{ course.code }} - {{ course.name }}
                  </h3>
                  <p class="course-instructor text-muted mb-2">
                    <i class="fas fa-user me-2"></i>
                    <strong>Instructor:</strong> {{ course.instructor }}
                  </p>
                  <div class="course-meta d-flex flex-wrap gap-3">
                    <span class="badge bg-info fs-6">
                      <i class="fas fa-credit-card me-1"></i>
                      {{ course.credits }} Credits
                    </span>
                    <span
                      v-if="course.department"
                      class="badge bg-secondary fs-6"
                    >
                      <i class="fas fa-building me-1"></i>
                      {{ course.department }}
                    </span>
                    <span
                      v-if="course.difficulty"
                      class="badge bg-warning fs-6"
                    >
                      <i class="fas fa-chart-line me-1"></i>
                      Difficulty: {{ course.difficulty }}/5
                    </span>
                  </div>
                </div>
                <div class="col-md-4 text-end">
                  <div class="enrollment-status mb-3">
                    <span
                      class="badge badge-lg"
                      :class="getEnrollmentStatusClass(course.enrollmentStatus)"
                    >
                      {{ getEnrollmentStatusText(course.enrollmentStatus) }}
                    </span>
                  </div>
                  <div v-if="course.maxEnrollment" class="enrollment-stats">
                    <div class="small text-muted">
                      {{ course.enrolledStudents || 0 }}/{{
                        course.maxEnrollment
                      }}
                      enrolled
                    </div>
                    <div
                      v-if="course.waitlistCount && course.waitlistCount > 0"
                      class="small text-muted"
                    >
                      {{ course.waitlistCount }} on waitlist
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Course Description -->
            <div class="course-description mb-4">
              <h5>
                <i class="fas fa-file-alt me-2"></i>
                Description
              </h5>
              <p class="description-text">{{ course.description }}</p>
            </div>

            <!-- Schedule Information -->
            <div
              class="schedule-info mb-4"
              v-if="course.schedule && course.schedule.length > 0"
            >
              <h5>
                <i class="fas fa-calendar me-2"></i>
                Schedule
              </h5>
              <div class="schedule-grid">
                <div
                  v-for="(session, index) in course.schedule"
                  :key="index"
                  class="schedule-item card"
                >
                  <div class="card-body">
                    <div
                      class="d-flex justify-content-between align-items-center"
                    >
                      <div>
                        <h6 class="mb-1">{{ session.dayOfWeek }}</h6>
                        <p class="mb-1 text-muted">
                          <i class="fas fa-clock me-1"></i>
                          {{ session.startTime }} - {{ session.endTime }}
                        </p>
                        <p class="mb-0 small text-muted">
                          <i class="fas fa-map-marker-alt me-1"></i>
                          {{ session.location }}
                        </p>
                      </div>
                      <div class="schedule-duration">
                        <span class="badge bg-light text-dark">
                          {{
                            calculateDuration(
                              session.startTime,
                              session.endTime
                            )
                          }}
                        </span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Prerequisites -->
            <div
              class="prerequisites mb-4"
              v-if="course.prerequisites && course.prerequisites.length > 0"
            >
              <h5>
                <i class="fas fa-list-check me-2"></i>
                Prerequisites
              </h5>
              <div class="prerequisites-list">
                <div class="alert alert-info">
                  <div class="d-flex align-items-start">
                    <i class="fas fa-info-circle me-2 mt-1"></i>
                    <div>
                      <p class="mb-2">
                        <strong>Required courses before enrollment:</strong>
                      </p>
                      <div class="prerequisite-items">
                        <span
                          v-for="prereq in course.prerequisites"
                          :key="prereq.code"
                          class="badge bg-primary me-2 mb-2"
                        >
                          {{ prereq.code }} - {{ prereq.name }}
                        </span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Course Statistics -->
            <div
              class="course-stats mb-4"
              v-if="course.difficulty || course.weeklyWorkload"
            >
              <h5>
                <i class="fas fa-chart-bar me-2"></i>
                Course Information
              </h5>
              <div class="row g-3">
                <div class="col-md-6" v-if="course.difficulty">
                  <div class="stat-card card">
                    <div class="card-body text-center">
                      <i class="fas fa-chart-line fa-2x text-warning mb-2"></i>
                      <h6>Difficulty Level</h6>
                      <div class="difficulty-rating mb-2">
                        <span
                          v-for="i in 5"
                          :key="i"
                          class="star"
                          :class="{ filled: i <= (course.difficulty || 0) }"
                        >
                          <i class="fas fa-star"></i>
                        </span>
                      </div>
                      <p class="mb-0 text-muted">{{ course.difficulty }}/5</p>
                    </div>
                  </div>
                </div>
                <div class="col-md-6" v-if="course.weeklyWorkload">
                  <div class="stat-card card">
                    <div class="card-body text-center">
                      <i class="fas fa-clock fa-2x text-info mb-2"></i>
                      <h6>Expected Workload</h6>
                      <h4 class="text-info mb-1">
                        {{ course.weeklyWorkload }}
                      </h4>
                      <p class="mb-0 text-muted">hours per week</p>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Enrollment Warnings/Information -->
            <div class="enrollment-info mb-4">
              <div v-if="!canEnroll" class="alert alert-warning">
                <div class="d-flex align-items-start">
                  <i class="fas fa-exclamation-triangle me-2 mt-1"></i>
                  <div>
                    <h6 class="alert-heading">Enrollment Not Available</h6>
                    <p class="mb-0">
                      This course is currently full and does not have a waitlist
                      available.
                    </p>
                  </div>
                </div>
              </div>

              <div
                v-else-if="course.enrollmentStatus === 'waitlist'"
                class="alert alert-info"
              >
                <div class="d-flex align-items-start">
                  <i class="fas fa-info-circle me-2 mt-1"></i>
                  <div>
                    <h6 class="alert-heading">Waitlist Available</h6>
                    <p class="mb-0">
                      This course is full, but you can join the waitlist. You'll
                      be automatically enrolled if a spot becomes available.
                    </p>
                  </div>
                </div>
              </div>

              <div
                v-else-if="course.enrollmentStatus === 'Enrolled'"
                class="alert alert-success"
              >
                <div class="d-flex align-items-start">
                  <i class="fas fa-check-circle me-2 mt-1"></i>
                  <div>
                    <h6 class="alert-heading">Already Enrolled</h6>
                    <p class="mb-0">
                      You are currently enrolled in this course.
                    </p>
                  </div>
                </div>
              </div>
            </div>

            <!-- Schedule Conflicts Warning -->
            <div v-if="hasScheduleConflict" class="alert alert-danger">
              <div class="d-flex align-items-start">
                <i class="fas fa-exclamation-circle me-2 mt-1"></i>
                <div>
                  <h6 class="alert-heading">Schedule Conflict Detected</h6>
                  <p class="mb-0">
                    This course conflicts with your current schedule. Please
                    review your enrolled courses before proceeding.
                  </p>
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
            Close
          </button>
          <button
            v-if="course && course.enrollmentStatus !== 'Enrolled'"
            type="button"
            class="btn"
            :class="
              course.enrollmentStatus === 'available'
                ? 'btn-primary'
                : 'btn-warning'
            "
            @click="handleEnrollment"
            :disabled="!canEnroll"
          >
            <i class="fas fa-plus me-1"></i>
            <span v-if="course.enrollmentStatus === 'available'"
              >Enroll Now</span
            >
            <span v-else-if="course.enrollmentStatus === 'waitlist'"
              >Join Waitlist</span
            >
            <span v-else>Unavailable</span>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";
import type { Course } from "../types";

// Props and Emits
const props = defineProps<{
  course: Course | null;
}>();

const emit = defineEmits<{
  close: [];
  enroll: [course: Course];
}>();

// Computed properties
const canEnroll = computed(() => {
  if (!props.course) return false;
  return (
    (props.course.enrollmentStatus as any) === "available" ||
    (props.course.enrollmentStatus as any) === "waitlist"
  );
});

const hasScheduleConflict = computed(() => {
  // This would be calculated based on enrolled courses
  // For now, returning false as we don't have enrolled courses context
  return false;
});

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
      return "Waitlist Available";
    case "full":
      return "Full";
    default:
      return "Unknown";
  }
};

const calculateDuration = (startTime: string, endTime: string) => {
  try {
    const start = new Date(`2000-01-01 ${startTime}`);
    const end = new Date(`2000-01-01 ${endTime}`);
    const diff = end.getTime() - start.getTime();
    const hours = Math.floor(diff / (1000 * 60 * 60));
    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));

    if (hours > 0 && minutes > 0) {
      return `${hours}h ${minutes}m`;
    } else if (hours > 0) {
      return `${hours}h`;
    } else {
      return `${minutes}m`;
    }
  } catch {
    return "N/A";
  }
};

const handleEnrollment = () => {
  if (props.course) {
    emit("enroll", props.course);
  }
};
</script>

<style scoped>
.course-details {
  padding: 0.5rem;
}

.course-header {
  border-bottom: 1px solid var(--bs-border-color);
  padding-bottom: 1rem;
}

.course-title {
  color: var(--bs-primary);
  font-weight: 600;
}

.badge-lg {
  font-size: 1rem;
  padding: 0.5rem 0.75rem;
}

.description-text {
  font-size: 1rem;
  line-height: 1.6;
}

.schedule-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 1rem;
}

.schedule-item {
  border: 1px solid var(--bs-border-color);
  transition: transform 0.2s ease;
}

.schedule-item:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.prerequisites-list .badge {
  font-size: 0.875rem;
  padding: 0.5rem 0.75rem;
}

.stat-card {
  border: 1px solid var(--bs-border-color);
  transition: transform 0.2s ease;
  height: 100%;
}

.stat-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.difficulty-rating .star {
  color: #ddd;
  margin-right: 2px;
  font-size: 1.2rem;
}

.difficulty-rating .star.filled {
  color: #ffc107;
}

.alert {
  border: 1px solid;
  border-radius: 0.375rem;
}

.alert-heading {
  margin-bottom: 0.5rem;
  font-weight: 600;
}

.modal-dialog {
  max-width: 800px;
}

@media (max-width: 768px) {
  .modal-dialog {
    max-width: 95vw;
    margin: 0.5rem;
  }

  .course-details {
    padding: 0.25rem;
  }

  .schedule-grid {
    grid-template-columns: 1fr;
  }

  .course-meta {
    flex-direction: column;
    align-items: flex-start !important;
  }

  .badge {
    margin-bottom: 0.5rem;
  }
}
</style>
