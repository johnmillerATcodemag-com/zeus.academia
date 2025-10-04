<template>
  <div class="profile-page">
    <div class="container py-4">
      <div class="row">
        <div class="col-lg-8 mx-auto">
          <div class="card">
            <div class="card-header">
              <h3 class="mb-0">Student Profile</h3>
            </div>
            <div class="card-body">
              <div class="row">
                <div class="col-md-4 text-center mb-4">
                  <div class="profile-avatar mb-3">
                    <i
                      class="bi bi-person-circle"
                      style="font-size: 8rem; color: var(--zeus-primary)"
                    ></i>
                  </div>
                  <h5>{{ fullName }}</h5>
                  <p class="text-muted">{{ student?.studentId }}</p>
                </div>

                <div class="col-md-8">
                  <form @submit.prevent="updateProfile">
                    <div class="row g-3">
                      <div class="col-md-6">
                        <label for="firstName" class="form-label"
                          >First Name</label
                        >
                        <input
                          id="firstName"
                          v-model="profile.firstName"
                          type="text"
                          class="form-control"
                          :disabled="!isEditing"
                        />
                      </div>

                      <div class="col-md-6">
                        <label for="lastName" class="form-label"
                          >Last Name</label
                        >
                        <input
                          id="lastName"
                          v-model="profile.lastName"
                          type="text"
                          class="form-control"
                          :disabled="!isEditing"
                        />
                      </div>

                      <div class="col-12">
                        <label for="email" class="form-label"
                          >Email Address</label
                        >
                        <input
                          id="email"
                          v-model="profile.email"
                          type="email"
                          class="form-control"
                          :disabled="!isEditing"
                        />
                      </div>

                      <div class="col-md-6">
                        <label for="studentId" class="form-label"
                          >Student ID</label
                        >
                        <input
                          id="studentId"
                          v-model="profile.studentId"
                          type="text"
                          class="form-control"
                          disabled
                        />
                      </div>

                      <div class="col-md-6">
                        <label for="enrollmentDate" class="form-label"
                          >Enrollment Date</label
                        >
                        <input
                          id="enrollmentDate"
                          :value="formattedEnrollmentDate"
                          type="text"
                          class="form-control"
                          disabled
                        />
                      </div>

                      <div class="col-md-6">
                        <label for="gpa" class="form-label">Current GPA</label>
                        <input
                          id="gpa"
                          :value="profile.gpa?.toFixed(2) || 'N/A'"
                          type="text"
                          class="form-control"
                          disabled
                        />
                      </div>
                    </div>

                    <div class="mt-4">
                      <div v-if="!isEditing" class="d-flex gap-2">
                        <button
                          type="button"
                          class="btn btn-primary"
                          @click="startEditing"
                        >
                          <i class="bi bi-pencil me-2"></i>
                          Edit Profile
                        </button>
                        <button
                          type="button"
                          class="btn btn-outline-secondary"
                          @click="changePassword"
                        >
                          <i class="bi bi-lock me-2"></i>
                          Change Password
                        </button>
                      </div>

                      <div v-else class="d-flex gap-2">
                        <button
                          type="submit"
                          class="btn btn-success"
                          :disabled="isUpdating"
                        >
                          <span
                            v-if="isUpdating"
                            class="spinner-border spinner-border-sm me-2"
                            role="status"
                          ></span>
                          <i v-else class="bi bi-check me-2"></i>
                          {{ isUpdating ? "Saving..." : "Save Changes" }}
                        </button>
                        <button
                          type="button"
                          class="btn btn-secondary"
                          @click="cancelEditing"
                        >
                          <i class="bi bi-x me-2"></i>
                          Cancel
                        </button>
                      </div>
                    </div>
                  </form>
                </div>
              </div>
            </div>
          </div>

          <!-- Academic Summary -->
          <div class="card mt-4">
            <div class="card-header">
              <h5 class="mb-0">Academic Summary</h5>
            </div>
            <div class="card-body">
              <div class="row g-4">
                <div class="col-md-4">
                  <div class="text-center">
                    <i
                      class="bi bi-book-fill text-primary"
                      style="font-size: 2rem"
                    ></i>
                    <h4 class="mt-2">{{ enrolledCoursesCount }}</h4>
                    <p class="text-muted mb-0">Enrolled Courses</p>
                  </div>
                </div>

                <div class="col-md-4">
                  <div class="text-center">
                    <i
                      class="bi bi-award-fill text-success"
                      style="font-size: 2rem"
                    ></i>
                    <h4 class="mt-2">{{ completedCredits }}</h4>
                    <p class="text-muted mb-0">Completed Credits</p>
                  </div>
                </div>

                <div class="col-md-4">
                  <div class="text-center">
                    <i
                      class="bi bi-graph-up text-info"
                      style="font-size: 2rem"
                    ></i>
                    <h4 class="mt-2">{{ currentGPA }}</h4>
                    <p class="text-muted mb-0">Current GPA</p>
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
import { ref, computed, reactive } from "vue";
import { useStore } from "vuex";

const store = useStore();
const isEditing = ref(false);
const isUpdating = ref(false);

// Get student data from Vuex store
const student = computed(() => store.getters["auth/currentStudent"]);

// Reactive profile form data
const profile = reactive({
  firstName: "",
  lastName: "",
  email: "",
  phone: "",
});

// Initialize profile data from store
const initializeProfile = () => {
  if (student.value) {
    profile.firstName = student.value.firstName;
    profile.lastName = student.value.lastName;
    profile.email = student.value.email;
    profile.phone = student.value.phone || "(555) 123-4567";
  }
};

// Watch for student data changes and initialize profile
if (student.value) {
  initializeProfile();
}

const fullName = computed(() => {
  if (student.value) {
    return `${student.value.firstName} ${student.value.lastName}`;
  }
  return "Test Student";
});

const formattedEnrollmentDate = computed(() => {
  return "September 1, 2023";
});

const enrolledCoursesCount = computed(() => {
  return 2; // Mock enrolled courses count
});

const completedCredits = computed(() => {
  return student.value?.totalCredits || 45;
});

const currentGPA = computed(() => {
  return student.value?.gpa?.toFixed(2) || "3.85";
});

const startEditing = () => {
  isEditing.value = true;
  initializeProfile();
};

const cancelEditing = () => {
  isEditing.value = false;
  initializeProfile();
};

const updateProfile = async () => {
  isUpdating.value = true;

  try {
    // Use Vuex store to update profile
    await store.dispatch("auth/updateProfile", {
      firstName: profile.firstName,
      lastName: profile.lastName,
      email: profile.email,
      phone: profile.phone,
    });

    isEditing.value = false;
  } catch (error) {
    console.error("Failed to update profile:", error);
  } finally {
    isUpdating.value = false;
  }
};

const changePassword = () => {
  // In a real app, this would open a password change modal
  alert("Password change functionality would be implemented here");
};

// Initialize profile on component setup
if (student.value) {
  initializeProfile();
}
</script>

<style scoped>
.profile-avatar {
  display: inline-block;
  position: relative;
}

.card {
  border: none;
  box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075);
}

.form-control:disabled {
  background-color: #f8f9fa;
  opacity: 1;
}
</style>
