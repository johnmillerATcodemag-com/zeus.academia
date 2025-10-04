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

                      <div class="col-md-6">
                        <label for="phone" class="form-label"
                          >Phone Number</label
                        >
                        <input
                          id="phone"
                          v-model="profile.phone"
                          type="tel"
                          class="form-control"
                          :disabled="!isEditing"
                          placeholder="(555) 123-4567"
                        />
                      </div>

                      <div class="col-md-6">
                        <label for="dateOfBirth" class="form-label"
                          >Date of Birth</label
                        >
                        <input
                          id="dateOfBirth"
                          v-model="profile.dateOfBirth"
                          type="date"
                          class="form-control"
                          :disabled="!isEditing"
                        />
                      </div>

                      <!-- Address Section -->
                      <div class="col-12">
                        <h6 class="mt-3 mb-2">Address Information</h6>
                      </div>

                      <div class="col-12">
                        <label for="street" class="form-label"
                          >Street Address</label
                        >
                        <input
                          id="street"
                          v-model="profile.address.street"
                          type="text"
                          class="form-control"
                          :disabled="!isEditing"
                          placeholder="123 Main Street"
                        />
                      </div>

                      <div class="col-md-6">
                        <label for="city" class="form-label">City</label>
                        <input
                          id="city"
                          v-model="profile.address.city"
                          type="text"
                          class="form-control"
                          :disabled="!isEditing"
                          placeholder="Springfield"
                        />
                      </div>

                      <div class="col-md-3">
                        <label for="state" class="form-label">State</label>
                        <input
                          id="state"
                          v-model="profile.address.state"
                          type="text"
                          class="form-control"
                          :disabled="!isEditing"
                          placeholder="IL"
                        />
                      </div>

                      <div class="col-md-3">
                        <label for="zipCode" class="form-label">ZIP Code</label>
                        <input
                          id="zipCode"
                          v-model="profile.address.zipCode"
                          type="text"
                          class="form-control"
                          :disabled="!isEditing"
                          placeholder="62701"
                        />
                      </div>

                      <div class="col-md-6">
                        <label for="country" class="form-label">Country</label>
                        <select
                          id="country"
                          v-model="profile.address.country"
                          class="form-select"
                          :disabled="!isEditing"
                        >
                          <option value="">Select Country</option>
                          <option value="USA">United States</option>
                          <option value="CA">Canada</option>
                          <option value="MX">Mexico</option>
                          <option value="UK">United Kingdom</option>
                          <option value="FR">France</option>
                          <option value="DE">Germany</option>
                          <option value="JP">Japan</option>
                          <option value="AU">Australia</option>
                        </select>
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
                          @click="showPasswordModal = true"
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

          <!-- Emergency Contact Section -->
          <div class="card mt-4">
            <div
              class="card-header d-flex justify-content-between align-items-center"
            >
              <h5 class="mb-0">Emergency Contact</h5>
              <button
                v-if="!isEditingEmergencyContact"
                type="button"
                class="btn btn-sm btn-outline-primary"
                @click="startEditingEmergencyContact"
              >
                <i class="bi bi-pencil me-1"></i>
                Edit
              </button>
            </div>
            <div class="card-body">
              <form @submit.prevent="updateEmergencyContact">
                <div class="row g-3">
                  <div class="col-md-6">
                    <label for="emergencyName" class="form-label"
                      >Full Name</label
                    >
                    <input
                      id="emergencyName"
                      v-model="emergencyContact.name"
                      type="text"
                      class="form-control"
                      :disabled="!isEditingEmergencyContact"
                      placeholder="Jane Smith"
                    />
                  </div>

                  <div class="col-md-6">
                    <label for="relationship" class="form-label"
                      >Relationship</label
                    >
                    <select
                      id="relationship"
                      v-model="emergencyContact.relationship"
                      class="form-select"
                      :disabled="!isEditingEmergencyContact"
                    >
                      <option value="">Select Relationship</option>
                      <option value="Parent">Parent</option>
                      <option value="Guardian">Guardian</option>
                      <option value="Spouse">Spouse</option>
                      <option value="Sibling">Sibling</option>
                      <option value="Relative">Relative</option>
                      <option value="Friend">Friend</option>
                      <option value="Other">Other</option>
                    </select>
                  </div>

                  <div class="col-md-6">
                    <label for="emergencyPhone" class="form-label"
                      >Phone Number</label
                    >
                    <input
                      id="emergencyPhone"
                      v-model="emergencyContact.phone"
                      type="tel"
                      class="form-control"
                      :disabled="!isEditingEmergencyContact"
                      placeholder="(555) 987-6543"
                    />
                  </div>

                  <div class="col-md-6">
                    <label for="emergencyEmail" class="form-label"
                      >Email Address</label
                    >
                    <input
                      id="emergencyEmail"
                      v-model="emergencyContact.email"
                      type="email"
                      class="form-control"
                      :disabled="!isEditingEmergencyContact"
                      placeholder="jane.smith@example.com"
                    />
                  </div>

                  <div v-if="isEditingEmergencyContact" class="col-12">
                    <div class="d-flex gap-2">
                      <button
                        type="submit"
                        class="btn btn-success"
                        :disabled="isUpdatingEmergencyContact"
                      >
                        <span
                          v-if="isUpdatingEmergencyContact"
                          class="spinner-border spinner-border-sm me-2"
                          role="status"
                        ></span>
                        <i v-else class="bi bi-check me-2"></i>
                        {{
                          isUpdatingEmergencyContact
                            ? "Saving..."
                            : "Save Changes"
                        }}
                      </button>
                      <button
                        type="button"
                        class="btn btn-secondary"
                        @click="cancelEditingEmergencyContact"
                      >
                        <i class="bi bi-x me-2"></i>
                        Cancel
                      </button>
                    </div>
                  </div>
                </div>
              </form>
            </div>
          </div>

          <!-- Profile Photo Section -->
          <div class="card mt-4">
            <div class="card-header">
              <h5 class="mb-0">Profile Photo</h5>
            </div>
            <div class="card-body">
              <div class="row">
                <div class="col-md-4 text-center">
                  <div class="profile-photo mb-3">
                    <img
                      v-if="profilePhotoUrl"
                      :src="profilePhotoUrl"
                      alt="Profile Photo"
                      class="img-fluid rounded-circle"
                      style="width: 150px; height: 150px; object-fit: cover"
                    />
                    <i
                      v-else
                      class="bi bi-person-circle"
                      style="font-size: 150px; color: var(--zeus-primary)"
                    ></i>
                  </div>
                </div>
                <div class="col-md-8">
                  <div class="mb-3">
                    <label for="photoUpload" class="form-label"
                      >Upload New Photo</label
                    >
                    <input
                      id="photoUpload"
                      ref="photoInput"
                      type="file"
                      class="form-control"
                      accept="image/jpeg,image/png,image/gif"
                      @change="handlePhotoUpload"
                    />
                    <div class="form-text">
                      Accepted formats: JPEG, PNG, GIF. Maximum size: 5MB.
                    </div>
                  </div>
                  <button
                    v-if="selectedPhoto"
                    type="button"
                    class="btn btn-primary"
                    :disabled="isUploadingPhoto"
                    @click="uploadPhoto"
                  >
                    <span
                      v-if="isUploadingPhoto"
                      class="spinner-border spinner-border-sm me-2"
                      role="status"
                    ></span>
                    <i v-else class="bi bi-upload me-2"></i>
                    {{ isUploadingPhoto ? "Uploading..." : "Upload Photo" }}
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Documents Section -->
          <div class="card mt-4">
            <div
              class="card-header d-flex justify-content-between align-items-center"
            >
              <h5 class="mb-0">Documents</h5>
              <button
                type="button"
                class="btn btn-sm btn-outline-primary"
                @click="showDocumentUpload = !showDocumentUpload"
              >
                <i class="bi bi-plus me-1"></i>
                Upload Document
              </button>
            </div>
            <div class="card-body">
              <!-- Document Upload Form -->
              <div v-if="showDocumentUpload" class="mb-4 p-3 bg-light rounded">
                <form @submit.prevent="uploadDocument">
                  <div class="row g-3">
                    <div class="col-md-6">
                      <label for="documentUpload" class="form-label"
                        >Select Document</label
                      >
                      <input
                        id="documentUpload"
                        ref="documentInput"
                        type="file"
                        class="form-control"
                        accept=".pdf,.doc,.docx,.jpg,.jpeg,.png"
                        @change="handleDocumentUpload"
                      />
                      <div class="form-text">
                        Accepted formats: PDF, DOC, DOCX, JPG, PNG. Maximum
                        size: 10MB.
                      </div>
                    </div>
                    <div class="col-md-6">
                      <label for="documentType" class="form-label"
                        >Document Type</label
                      >
                      <select
                        id="documentType"
                        v-model="selectedDocumentType"
                        class="form-select"
                      >
                        <option value="">Select Type</option>
                        <option value="transcript">Official Transcript</option>
                        <option value="id">ID Document</option>
                        <option value="insurance">Insurance Card</option>
                        <option value="immunization">
                          Immunization Records
                        </option>
                        <option value="resume">Resume/CV</option>
                        <option value="other">Other</option>
                      </select>
                    </div>
                    <div class="col-12">
                      <div class="d-flex gap-2">
                        <button
                          type="submit"
                          class="btn btn-success"
                          :disabled="
                            !selectedDocument ||
                            !selectedDocumentType ||
                            isUploadingDocument
                          "
                        >
                          <span
                            v-if="isUploadingDocument"
                            class="spinner-border spinner-border-sm me-2"
                            role="status"
                          ></span>
                          <i v-else class="bi bi-upload me-2"></i>
                          {{
                            isUploadingDocument
                              ? "Uploading..."
                              : "Upload Document"
                          }}
                        </button>
                        <button
                          type="button"
                          class="btn btn-secondary"
                          @click="cancelDocumentUpload"
                        >
                          Cancel
                        </button>
                      </div>
                    </div>
                  </div>
                </form>
              </div>

              <!-- Documents List -->
              <div v-if="documents.length > 0">
                <div class="table-responsive">
                  <table class="table table-hover">
                    <thead>
                      <tr>
                        <th>Document Name</th>
                        <th>Type</th>
                        <th>Upload Date</th>
                        <th>Size</th>
                        <th>Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="document in documents" :key="document.id">
                        <td>{{ document.name }}</td>
                        <td>
                          <span class="badge bg-secondary">{{
                            document.type
                          }}</span>
                        </td>
                        <td>{{ formatDate(document.uploadDate) }}</td>
                        <td>{{ formatFileSize(document.size) }}</td>
                        <td>
                          <div class="btn-group btn-group-sm">
                            <button
                              type="button"
                              class="btn btn-outline-primary"
                              @click="downloadDocument(document)"
                            >
                              <i class="bi bi-download"></i>
                            </button>
                            <button
                              type="button"
                              class="btn btn-outline-danger"
                              @click="deleteDocument(document.id)"
                            >
                              <i class="bi bi-trash"></i>
                            </button>
                          </div>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
              <div v-else class="text-center text-muted">
                <i class="bi bi-file-earmark" style="font-size: 3rem"></i>
                <p class="mt-2">No documents uploaded yet.</p>
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

    <!-- Password Change Modal -->
    <div
      v-if="showPasswordModal"
      class="modal d-block"
      tabindex="-1"
      style="background-color: rgba(0, 0, 0, 0.5)"
    >
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Change Password</h5>
            <button
              type="button"
              class="btn-close"
              @click="closePasswordModal"
            ></button>
          </div>
          <form @submit.prevent="changePassword">
            <div class="modal-body">
              <div class="mb-3">
                <label for="currentPassword" class="form-label"
                  >Current Password</label
                >
                <input
                  id="currentPassword"
                  v-model="passwordForm.currentPassword"
                  type="password"
                  class="form-control"
                  required
                />
              </div>
              <div class="mb-3">
                <label for="newPassword" class="form-label">New Password</label>
                <input
                  id="newPassword"
                  v-model="passwordForm.newPassword"
                  type="password"
                  class="form-control"
                  required
                  minlength="8"
                />
                <div class="form-text">
                  Password must be at least 8 characters long and contain
                  uppercase, lowercase, numbers, and special characters.
                </div>
              </div>
              <div class="mb-3">
                <label for="confirmPassword" class="form-label"
                  >Confirm New Password</label
                >
                <input
                  id="confirmPassword"
                  v-model="passwordForm.confirmPassword"
                  type="password"
                  class="form-control"
                  required
                />
              </div>
            </div>
            <div class="modal-footer">
              <button
                type="button"
                class="btn btn-secondary"
                @click="closePasswordModal"
              >
                Cancel
              </button>
              <button
                type="submit"
                class="btn btn-primary"
                :disabled="isChangingPassword || !isPasswordFormValid"
              >
                <span
                  v-if="isChangingPassword"
                  class="spinner-border spinner-border-sm me-2"
                  role="status"
                ></span>
                {{ isChangingPassword ? "Changing..." : "Change Password" }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, reactive, onMounted } from "vue";
import { useStore } from "vuex";
import { AuthService } from "../services/AuthService";
import type { EmergencyContact, Document } from "../types";

const store = useStore();

// Profile editing state
const isEditing = ref(false);
const isUpdating = ref(false);

// Emergency contact state
const isEditingEmergencyContact = ref(false);
const isUpdatingEmergencyContact = ref(false);

// Password change state
const showPasswordModal = ref(false);
const isChangingPassword = ref(false);

// Photo upload state
const selectedPhoto = ref<File | null>(null);
const isUploadingPhoto = ref(false);
const profilePhotoUrl = ref<string | null>(null);

// Document upload state
const showDocumentUpload = ref(false);
const selectedDocument = ref<File | null>(null);
const selectedDocumentType = ref("");
const isUploadingDocument = ref(false);
const documents = ref<Document[]>([]);

// Get student data from Vuex store
const student = computed(() => store.getters["auth/currentStudent"]);

// Reactive profile form data
const profile = reactive({
  firstName: "",
  lastName: "",
  email: "",
  phone: "",
  dateOfBirth: "",
  gpa: 0,
  address: {
    street: "",
    city: "",
    state: "",
    zipCode: "",
    country: "",
  },
});

// Emergency contact form data
const emergencyContact = reactive<EmergencyContact>({
  name: "",
  relationship: "",
  phone: "",
  email: "",
});

// Password change form data
const passwordForm = reactive({
  currentPassword: "",
  newPassword: "",
  confirmPassword: "",
});

// Initialize profile data from store
const initializeProfile = () => {
  if (student.value) {
    profile.firstName = student.value.firstName || "";
    profile.lastName = student.value.lastName || "";
    profile.email = student.value.email || "";
    profile.phone = student.value.phone || "";
    profile.dateOfBirth = student.value.dateOfBirth || "";
    profile.gpa = student.value.gpa || 0;

    if (student.value.address) {
      profile.address = { ...student.value.address };
    }

    if (student.value.emergencyContact) {
      Object.assign(emergencyContact, student.value.emergencyContact);
    }
  }
};

// Initialize emergency contact data
const initializeEmergencyContact = () => {
  if (student.value?.emergencyContact) {
    Object.assign(emergencyContact, student.value.emergencyContact);
  }
};

// Computed properties
const fullName = computed(() => {
  if (student.value) {
    return `${student.value.firstName} ${student.value.lastName}`;
  }
  return "Test Student";
});

const formattedEnrollmentDate = computed(() => {
  if (student.value?.enrollmentDate) {
    return new Date(student.value.enrollmentDate).toLocaleDateString();
  }
  return "September 1, 2023";
});

const enrolledCoursesCount = computed(() => {
  return store.getters["courses/enrolledCourses"]?.length || 0;
});

const completedCredits = computed(() => {
  return student.value?.totalCredits || 45;
});

const currentGPA = computed(() => {
  return profile.gpa?.toFixed(2) || "3.85";
});

const isPasswordFormValid = computed(() => {
  return (
    passwordForm.currentPassword.length > 0 &&
    passwordForm.newPassword.length >= 8 &&
    passwordForm.newPassword === passwordForm.confirmPassword
  );
});

// Profile management methods
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
    const result = await AuthService.updateProfile({
      firstName: profile.firstName,
      lastName: profile.lastName,
      email: profile.email,
      phone: profile.phone,
      dateOfBirth: profile.dateOfBirth,
      address: profile.address,
    });

    if (result.success) {
      await store.dispatch("auth/updateProfile", result.data);
      store.dispatch("showNotification", {
        type: "success",
        message: "Profile updated successfully",
      });
      isEditing.value = false;
    } else {
      store.dispatch("showNotification", {
        type: "error",
        message: result.message || "Failed to update profile",
      });
    }
  } catch (error) {
    console.error("Failed to update profile:", error);
    store.dispatch("showNotification", {
      type: "error",
      message: "An error occurred while updating your profile",
    });
  } finally {
    isUpdating.value = false;
  }
};

// Emergency contact methods
const startEditingEmergencyContact = () => {
  isEditingEmergencyContact.value = true;
  initializeEmergencyContact();
};

const cancelEditingEmergencyContact = () => {
  isEditingEmergencyContact.value = false;
  initializeEmergencyContact();
};

const updateEmergencyContact = async () => {
  isUpdatingEmergencyContact.value = true;

  try {
    let result;
    if (emergencyContact.id) {
      result = await AuthService.updateEmergencyContact(
        emergencyContact.id,
        emergencyContact
      );
    } else {
      result = await AuthService.addEmergencyContact(emergencyContact);
    }

    if (result.success) {
      store.dispatch("showNotification", {
        type: "success",
        message: "Emergency contact updated successfully",
      });
      isEditingEmergencyContact.value = false;
      // Update the local emergency contact data
      if (result.data) {
        Object.assign(emergencyContact, result.data);
      }
    } else {
      store.dispatch("showNotification", {
        type: "error",
        message: result.message || "Failed to update emergency contact",
      });
    }
  } catch (error) {
    console.error("Failed to update emergency contact:", error);
    store.dispatch("showNotification", {
      type: "error",
      message: "An error occurred while updating emergency contact",
    });
  } finally {
    isUpdatingEmergencyContact.value = false;
  }
};

// Password change methods
const closePasswordModal = () => {
  showPasswordModal.value = false;
  passwordForm.currentPassword = "";
  passwordForm.newPassword = "";
  passwordForm.confirmPassword = "";
};

const changePassword = async () => {
  if (!isPasswordFormValid.value) return;

  isChangingPassword.value = true;

  try {
    const result = await AuthService.changePassword(
      passwordForm.currentPassword,
      passwordForm.newPassword
    );

    if (result.success) {
      store.dispatch("showNotification", {
        type: "success",
        message: "Password changed successfully",
      });
      closePasswordModal();
    } else {
      store.dispatch("showNotification", {
        type: "error",
        message: result.message || "Failed to change password",
      });
    }
  } catch (error) {
    console.error("Failed to change password:", error);
    store.dispatch("showNotification", {
      type: "error",
      message: "An error occurred while changing password",
    });
  } finally {
    isChangingPassword.value = false;
  }
};

// Photo upload methods
const photoInput = ref<HTMLInputElement | null>(null);

const handlePhotoUpload = (event: Event) => {
  const target = event.target as HTMLInputElement;
  const file = target.files?.[0];

  if (file) {
    // Validate file type and size
    if (!file.type.startsWith("image/")) {
      store.dispatch("showNotification", {
        type: "error",
        message: "Please select a valid image file",
      });
      return;
    }

    if (file.size > 5 * 1024 * 1024) {
      // 5MB
      store.dispatch("showNotification", {
        type: "error",
        message: "File size must be less than 5MB",
      });
      return;
    }

    selectedPhoto.value = file;
  }
};

const uploadPhoto = async () => {
  if (!selectedPhoto.value) return;

  isUploadingPhoto.value = true;

  try {
    const result = await AuthService.uploadProfilePhoto(selectedPhoto.value);

    if (result.success && result.data) {
      profilePhotoUrl.value = result.data.photoUrl;
      store.dispatch("showNotification", {
        type: "success",
        message: "Profile photo uploaded successfully",
      });
      selectedPhoto.value = null;
      if (photoInput.value) {
        photoInput.value.value = "";
      }
    } else {
      store.dispatch("showNotification", {
        type: "error",
        message: result.message || "Failed to upload photo",
      });
    }
  } catch (error) {
    console.error("Failed to upload photo:", error);
    store.dispatch("showNotification", {
      type: "error",
      message: "An error occurred while uploading photo",
    });
  } finally {
    isUploadingPhoto.value = false;
  }
};

// Document upload methods
const documentInput = ref<HTMLInputElement | null>(null);

const handleDocumentUpload = (event: Event) => {
  const target = event.target as HTMLInputElement;
  const file = target.files?.[0];

  if (file) {
    // Validate file size
    if (file.size > 10 * 1024 * 1024) {
      // 10MB
      store.dispatch("showNotification", {
        type: "error",
        message: "File size must be less than 10MB",
      });
      return;
    }

    selectedDocument.value = file;
  }
};

const uploadDocument = async () => {
  if (!selectedDocument.value || !selectedDocumentType.value) return;

  isUploadingDocument.value = true;

  try {
    const result = await AuthService.uploadDocument(
      selectedDocument.value,
      selectedDocumentType.value
    );

    if (result.success) {
      store.dispatch("showNotification", {
        type: "success",
        message: "Document uploaded successfully",
      });
      await loadDocuments(); // Refresh documents list
      cancelDocumentUpload();
    } else {
      store.dispatch("showNotification", {
        type: "error",
        message: result.message || "Failed to upload document",
      });
    }
  } catch (error) {
    console.error("Failed to upload document:", error);
    store.dispatch("showNotification", {
      type: "error",
      message: "An error occurred while uploading document",
    });
  } finally {
    isUploadingDocument.value = false;
  }
};

const cancelDocumentUpload = () => {
  showDocumentUpload.value = false;
  selectedDocument.value = null;
  selectedDocumentType.value = "";
  if (documentInput.value) {
    documentInput.value.value = "";
  }
};

const loadDocuments = async () => {
  try {
    const result = await AuthService.getDocuments();
    if (result.success && result.data) {
      documents.value = result.data;
    }
  } catch (error) {
    console.error("Failed to load documents:", error);
  }
};

const deleteDocument = async (documentId: string) => {
  if (!confirm("Are you sure you want to delete this document?")) return;

  try {
    const result = await AuthService.deleteDocument(documentId);
    if (result.success) {
      store.dispatch("showNotification", {
        type: "success",
        message: "Document deleted successfully",
      });
      await loadDocuments(); // Refresh documents list
    } else {
      store.dispatch("showNotification", {
        type: "error",
        message: result.message || "Failed to delete document",
      });
    }
  } catch (error) {
    console.error("Failed to delete document:", error);
    store.dispatch("showNotification", {
      type: "error",
      message: "An error occurred while deleting document",
    });
  }
};

const downloadDocument = (document: Document) => {
  window.open(document.url, "_blank");
};

// Utility methods
const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString();
};

const formatFileSize = (bytes: number) => {
  if (bytes === 0) return "0 Bytes";
  const k = 1024;
  const sizes = ["Bytes", "KB", "MB", "GB"];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + " " + sizes[i];
};

// Initialize data on component mount
onMounted(() => {
  initializeProfile();
  initializeEmergencyContact();
  loadDocuments();
});
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
