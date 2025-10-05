<!-- Faculty Profile View -->
<template>
  <div class="faculty-profile-view">
    <!-- Profile Header -->
    <div class="profile-header bg-light rounded-3 p-4 mb-4">
      <div class="row align-items-center">
        <div class="col-md-3 text-center">
          <div class="profile-image-container position-relative">
            <img
              :src="profileImage"
              :alt="`${user?.firstName} ${user?.lastName}`"
              class="profile-image rounded-circle img-fluid mb-3"
              style="width: 150px; height: 150px; object-fit: cover"
            />
            <button
              v-if="canEdit"
              class="btn btn-sm btn-outline-primary position-absolute bottom-0 end-0 rounded-circle"
              @click="showImageUpload = true"
              title="Change Profile Photo"
            >
              <i class="fas fa-camera"></i>
            </button>
          </div>
        </div>
        <div class="col-md-9">
          <div class="d-flex justify-content-between align-items-start">
            <div>
              <h1 class="h2 mb-1">{{ fullName }}</h1>
              <p class="text-muted mb-2">{{ user?.title }}</p>
              <div class="role-badges mb-2">
                <span
                  :class="`badge bg-${authStore.getRoleColor(
                    user?.role || 'lecturer'
                  )} me-2`"
                >
                  {{ authStore.getRoleDisplayName(user?.role || "lecturer") }}
                </span>
                <span
                  v-if="
                    authStore.isAdministrativeRole(user?.role || 'lecturer')
                  "
                  class="badge bg-warning"
                >
                  <i class="fas fa-crown me-1"></i>
                  Administrative Role
                </span>
              </div>
              <div class="contact-info">
                <p class="mb-1">
                  <i class="fas fa-building me-2"></i>
                  {{ user?.department }}
                </p>
                <p class="mb-1" v-if="user?.officeLocation">
                  <i class="fas fa-map-marker-alt me-2"></i>
                  {{ user?.officeLocation }}
                </p>
                <p class="mb-1">
                  <i class="fas fa-envelope me-2"></i>
                  {{ user?.email }}
                </p>
                <p class="mb-0" v-if="user?.phoneNumber">
                  <i class="fas fa-phone me-2"></i>
                  {{ user?.phoneNumber }}
                </p>
              </div>
            </div>
            <div v-if="canEdit">
              <button class="btn btn-primary" @click="editMode = !editMode">
                <i :class="editMode ? 'fas fa-times' : 'fas fa-edit'"></i>
                {{ editMode ? "Cancel" : "Edit Profile" }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Navigation Tabs -->
    <ul class="nav nav-tabs mb-4" id="profileTabs" role="tablist">
      <li class="nav-item" role="presentation">
        <button
          class="nav-link active"
          id="overview-tab"
          data-bs-toggle="tab"
          data-bs-target="#overview"
          type="button"
          role="tab"
        >
          <i class="fas fa-user me-2"></i>Overview
        </button>
      </li>
      <li class="nav-item" role="presentation">
        <button
          class="nav-link"
          id="education-tab"
          data-bs-toggle="tab"
          data-bs-target="#education"
          type="button"
          role="tab"
        >
          <i class="fas fa-graduation-cap me-2"></i>Education
        </button>
      </li>
      <li class="nav-item" role="presentation">
        <button
          class="nav-link"
          id="publications-tab"
          data-bs-toggle="tab"
          data-bs-target="#publications"
          type="button"
          role="tab"
        >
          <i class="fas fa-book me-2"></i>Publications
        </button>
      </li>
      <li class="nav-item" role="presentation">
        <button
          class="nav-link"
          id="office-hours-tab"
          data-bs-toggle="tab"
          data-bs-target="#office-hours"
          type="button"
          role="tab"
        >
          <i class="fas fa-clock me-2"></i>Office Hours
        </button>
      </li>
      <li class="nav-item" role="presentation">
        <button
          class="nav-link"
          id="committees-tab"
          data-bs-toggle="tab"
          data-bs-target="#committees"
          type="button"
          role="tab"
        >
          <i class="fas fa-users me-2"></i>Committees
        </button>
      </li>
    </ul>

    <!-- Tab Content -->
    <div class="tab-content" id="profileTabContent">
      <!-- Overview Tab -->
      <div class="tab-pane fade show active" id="overview" role="tabpanel">
        <div class="row">
          <div class="col-md-8">
            <!-- Bio Section -->
            <div class="card mb-4">
              <div
                class="card-header d-flex justify-content-between align-items-center"
              >
                <h5 class="mb-0">
                  <i class="fas fa-user-circle me-2"></i>Biography
                </h5>
                <button
                  v-if="canEdit && !editMode"
                  class="btn btn-sm btn-outline-primary"
                  @click="editBio = true"
                >
                  <i class="fas fa-edit"></i>
                </button>
              </div>
              <div class="card-body">
                <div v-if="!editBio">
                  <p class="mb-0">
                    {{ profile?.bio || "No biography available." }}
                  </p>
                </div>
                <div v-else>
                  <textarea
                    v-model="editedBio"
                    class="form-control mb-3"
                    rows="5"
                    placeholder="Enter your biography..."
                  ></textarea>
                  <div class="d-flex gap-2">
                    <button class="btn btn-primary btn-sm" @click="saveBio">
                      <i class="fas fa-save me-1"></i>Save
                    </button>
                    <button
                      class="btn btn-secondary btn-sm"
                      @click="editBio = false"
                    >
                      Cancel
                    </button>
                  </div>
                </div>
              </div>
            </div>

            <!-- Research Areas -->
            <div class="card mb-4">
              <div class="card-header">
                <h5 class="mb-0">
                  <i class="fas fa-microscope me-2"></i>Research Areas
                </h5>
              </div>
              <div class="card-body">
                <div v-if="profile?.researchAreas?.length">
                  <span
                    v-for="area in profile.researchAreas"
                    :key="area"
                    class="badge bg-info text-dark me-2 mb-2"
                  >
                    {{ area }}
                  </span>
                </div>
                <p v-else class="text-muted mb-0">
                  No research areas specified.
                </p>
              </div>
            </div>

            <!-- Awards -->
            <div v-if="profile?.awards?.length" class="card mb-4">
              <div class="card-header">
                <h5 class="mb-0">
                  <i class="fas fa-trophy me-2"></i>Awards & Honors
                </h5>
              </div>
              <div class="card-body">
                <div
                  v-for="award in profile.awards"
                  :key="award.name"
                  class="mb-3 border-bottom pb-3"
                >
                  <h6 class="mb-1">{{ award.name }}</h6>
                  <p class="text-muted mb-1">
                    {{ award.institution }} • {{ award.year }}
                  </p>
                  <p v-if="award.description" class="mb-0 small">
                    {{ award.description }}
                  </p>
                </div>
              </div>
            </div>
          </div>

          <div class="col-md-4">
            <!-- Quick Stats -->
            <div class="card mb-4">
              <div class="card-header">
                <h5 class="mb-0">
                  <i class="fas fa-chart-bar me-2"></i>Academic Metrics
                </h5>
              </div>
              <div class="card-body">
                <div class="row text-center">
                  <div class="col-6">
                    <div class="stat-item">
                      <h4 class="mb-0 text-primary">
                        {{ profileStore.publications.length }}
                      </h4>
                      <small class="text-muted">Publications</small>
                    </div>
                  </div>
                  <div class="col-6">
                    <div class="stat-item">
                      <h4 class="mb-0 text-success">
                        {{ profileStore.totalCitations }}
                      </h4>
                      <small class="text-muted">Citations</small>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- CV Download -->
            <div class="card mb-4">
              <div class="card-header">
                <h5 class="mb-0">
                  <i class="fas fa-file-pdf me-2"></i>Curriculum Vitae
                </h5>
              </div>
              <div class="card-body text-center">
                <div v-if="profile?.cvUrl">
                  <button class="btn btn-outline-primary mb-2">
                    <i class="fas fa-download me-2"></i>Download CV
                  </button>
                  <p class="small text-muted mb-0">
                    Last updated: {{ formatDate(profile.lastUpdated) }}
                  </p>
                </div>
                <div v-else>
                  <p class="text-muted mb-2">No CV uploaded</p>
                  <button
                    v-if="canEdit"
                    class="btn btn-primary"
                    @click="showCVUpload = true"
                  >
                    <i class="fas fa-upload me-2"></i>Upload CV
                  </button>
                </div>
              </div>
            </div>

            <!-- Professional Links -->
            <div class="card mb-4">
              <div class="card-header">
                <h5 class="mb-0">
                  <i class="fas fa-link me-2"></i>Professional Links
                </h5>
              </div>
              <div class="card-body">
                <div v-if="profile?.website" class="mb-2">
                  <a
                    :href="profile.website"
                    target="_blank"
                    class="btn btn-sm btn-outline-primary w-100"
                  >
                    <i class="fas fa-globe me-2"></i>Personal Website
                  </a>
                </div>
                <div v-if="profile?.linkedinUrl" class="mb-2">
                  <a
                    :href="profile.linkedinUrl"
                    target="_blank"
                    class="btn btn-sm btn-outline-info w-100"
                  >
                    <i class="fab fa-linkedin me-2"></i>LinkedIn Profile
                  </a>
                </div>
                <div v-if="profile?.orcidId" class="mb-2">
                  <a
                    :href="`https://orcid.org/${profile.orcidId}`"
                    target="_blank"
                    class="btn btn-sm btn-outline-success w-100"
                  >
                    <i class="fas fa-id-card me-2"></i>ORCID Profile
                  </a>
                </div>
                <div v-if="profile?.googleScholarId">
                  <a
                    :href="`https://scholar.google.com/citations?user=${profile.googleScholarId}`"
                    target="_blank"
                    class="btn btn-sm btn-outline-secondary w-100"
                  >
                    <i class="fas fa-graduation-cap me-2"></i>Google Scholar
                  </a>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Education Tab -->
      <div class="tab-pane fade" id="education" role="tabpanel">
        <div class="card">
          <div
            class="card-header d-flex justify-content-between align-items-center"
          >
            <h5 class="mb-0">
              <i class="fas fa-graduation-cap me-2"></i>Education History
            </h5>
            <button
              v-if="canEdit"
              class="btn btn-sm btn-primary"
              @click="showAddEducation = true"
            >
              <i class="fas fa-plus me-1"></i>Add Education
            </button>
          </div>
          <div class="card-body">
            <div v-if="profile?.education?.length">
              <div
                v-for="edu in profile.education"
                :key="`${edu.degree}-${edu.institution}`"
                class="education-item border-bottom pb-3 mb-3"
              >
                <div class="d-flex justify-content-between align-items-start">
                  <div>
                    <h6 class="mb-1">{{ edu.degree }}</h6>
                    <p class="text-primary mb-1">{{ edu.institution }}</p>
                    <p class="text-muted mb-1">
                      {{ edu.fieldOfStudy }} • {{ edu.year }}
                    </p>
                    <p v-if="edu.gpa" class="small mb-0">GPA: {{ edu.gpa }}</p>
                    <p v-if="edu.honors" class="small text-success mb-0">
                      {{ edu.honors }}
                    </p>
                  </div>
                  <button
                    v-if="canEdit"
                    class="btn btn-sm btn-outline-danger"
                    @click="removeEducation(edu)"
                  >
                    <i class="fas fa-trash"></i>
                  </button>
                </div>
              </div>
            </div>
            <div v-else class="text-center text-muted py-4">
              <i class="fas fa-graduation-cap fa-3x mb-3 opacity-50"></i>
              <p>No education history available</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Publications Tab -->
      <div class="tab-pane fade" id="publications" role="tabpanel">
        <div class="card">
          <div
            class="card-header d-flex justify-content-between align-items-center"
          >
            <h5 class="mb-0"><i class="fas fa-book me-2"></i>Publications</h5>
            <div class="d-flex gap-2">
              <button
                v-if="canEdit"
                class="btn btn-sm btn-outline-info"
                @click="importPublications"
              >
                <i class="fas fa-download me-1"></i>Import
              </button>
              <button
                v-if="canEdit"
                class="btn btn-sm btn-primary"
                @click="showAddPublication = true"
              >
                <i class="fas fa-plus me-1"></i>Add Publication
              </button>
            </div>
          </div>
          <div class="card-body">
            <div v-if="profileStore.publications.length">
              <div
                v-for="pub in profileStore.publications"
                :key="pub.id"
                class="publication-item border-bottom pb-3 mb-3"
              >
                <div class="d-flex justify-content-between align-items-start">
                  <div class="flex-grow-1">
                    <h6 class="mb-1">{{ pub.title }}</h6>
                    <p class="text-muted mb-1">{{ pub.authors.join(", ") }}</p>
                    <p class="small mb-1">
                      <span
                        :class="`badge bg-${getPublicationTypeColor(
                          pub.type
                        )} me-2`"
                      >
                        {{ pub.type.toUpperCase() }}
                      </span>
                      {{ pub.journal || pub.conference || pub.book }} •
                      {{ pub.year }}
                    </p>
                    <div
                      v-if="pub.citationCount > 0"
                      class="small text-success"
                    >
                      <i class="fas fa-quote-right me-1"></i>
                      {{ pub.citationCount }} citations
                    </div>
                  </div>
                  <div v-if="canEdit" class="ms-3">
                    <button
                      class="btn btn-sm btn-outline-danger"
                      @click="removePublication(pub.id)"
                    >
                      <i class="fas fa-trash"></i>
                    </button>
                  </div>
                </div>
              </div>
            </div>
            <div v-else class="text-center text-muted py-4">
              <i class="fas fa-book fa-3x mb-3 opacity-50"></i>
              <p>No publications available</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Office Hours Tab -->
      <div class="tab-pane fade" id="office-hours" role="tabpanel">
        <FacultyOfficeHours :faculty-id="user?.id" :can-edit="canEdit" />
      </div>

      <!-- Committees Tab -->
      <div class="tab-pane fade" id="committees" role="tabpanel">
        <FacultyCommittees :faculty-id="user?.id" :can-edit="canEdit" />
      </div>
    </div>

    <!-- Modals and overlays would go here -->
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from "vue";
import { useRoute } from "vue-router";
import { useAuthStore } from "@/stores/auth";
import { useFacultyProfileStore } from "@/stores/facultyProfile";
import FacultyOfficeHours from "../components/FacultyOfficeHours.vue";
import FacultyCommittees from "../components/FacultyCommittees.vue";

// Store instances
const authStore = useAuthStore();
const profileStore = useFacultyProfileStore();
const route = useRoute();

// Component state
const editMode = ref(false);
const editBio = ref(false);
const editedBio = ref("");
const showImageUpload = ref(false);
const showCVUpload = ref(false);
const showAddEducation = ref(false);
const showAddPublication = ref(false);

// Computed properties
const user = computed(() => authStore.user);
const profile = computed(() => profileStore.profile);
const fullName = computed(() =>
  user.value ? `${user.value.firstName} ${user.value.lastName}` : ""
);
const profileImage = computed(
  () => user.value?.profileImage || "/images/default-faculty.png"
);
const canEdit = computed(() => {
  const userId = (route.params.id as string) || user.value?.id;
  return userId ? profileStore.canEditProfile(userId) : false;
});

// Methods
const formatDate = (date: Date) => {
  return new Intl.DateTimeFormat("en-US", {
    year: "numeric",
    month: "long",
    day: "numeric",
  }).format(new Date(date));
};

const getPublicationTypeColor = (type: string) => {
  const colors: Record<string, string> = {
    journal: "primary",
    conference: "success",
    book: "info",
    chapter: "warning",
    preprint: "secondary",
  };
  return colors[type] || "secondary";
};

const saveBio = async () => {
  try {
    await profileStore.updateProfile({ bio: editedBio.value });
    editBio.value = false;
  } catch (error) {
    console.error("Failed to save bio:", error);
  }
};

const removeEducation = async (education: any) => {
  if (confirm("Are you sure you want to remove this education entry?")) {
    const updatedEducation =
      profile.value?.education?.filter(
        (edu) =>
          edu.degree !== education.degree ||
          edu.institution !== education.institution
      ) || [];
    await profileStore.updateProfile({ education: updatedEducation });
  }
};

const removePublication = async (publicationId: string) => {
  if (confirm("Are you sure you want to remove this publication?")) {
    // Implementation would call profile store method to remove publication
    console.log("Remove publication:", publicationId);
  }
};

const importPublications = () => {
  // Implementation for importing publications from ORCID/Google Scholar
  console.log("Import publications");
};

// Lifecycle
onMounted(async () => {
  const userId = (route.params.id as string) || user.value?.id;
  if (userId) {
    await profileStore.loadProfile(userId);
  }
});

watch(
  () => profile.value?.bio,
  (newBio) => {
    editedBio.value = newBio || "";
  }
);
</script>

<style scoped>
.profile-image-container {
  width: 150px;
  height: 150px;
  margin: 0 auto;
}

.stat-item {
  padding: 1rem 0;
}

.education-item:last-child,
.publication-item:last-child {
  border-bottom: none !important;
  margin-bottom: 0 !important;
  padding-bottom: 0 !important;
}

.role-badges .badge {
  font-size: 0.875rem;
}

.nav-tabs .nav-link {
  color: #6c757d;
  border-color: transparent;
}

.nav-tabs .nav-link.active {
  color: #0d6efd;
  border-color: #dee2e6 #dee2e6 #fff;
}

.tab-content {
  min-height: 400px;
}
</style>
