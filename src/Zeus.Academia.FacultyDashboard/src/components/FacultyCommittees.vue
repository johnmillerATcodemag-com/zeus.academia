<!-- Faculty Committees Component -->
<template>
  <div class="faculty-committees">
    <div class="card">
      <div
        class="card-header d-flex justify-content-between align-items-center"
      >
        <h5 class="mb-0">
          <i class="fas fa-users me-2"></i>Committee Assignments
        </h5>
        <button
          v-if="canEdit"
          class="btn btn-sm btn-primary"
          @click="showAddCommittee = true"
        >
          <i class="fas fa-plus me-1"></i>Add Assignment
        </button>
      </div>
      <div class="card-body">
        <!-- Active Committees -->
        <div v-if="activeCommittees.length > 0" class="committees-list">
          <h6 class="text-success mb-3">
            <i class="fas fa-check-circle me-2"></i>Active Assignments ({{
              activeCommittees.length
            }})
          </h6>
          <div class="row">
            <div
              v-for="committee in activeCommittees"
              :key="committee.id"
              class="col-md-6 col-lg-4 mb-3"
            >
              <div class="committee-card border rounded p-3">
                <div class="d-flex justify-content-between align-items-start">
                  <div class="flex-grow-1">
                    <h6 class="committee-name text-primary mb-2">
                      {{ committee.name }}
                    </h6>
                    <div class="committee-details">
                      <div class="role-display mb-2">
                        <span
                          class="badge"
                          :class="{
                            'bg-success': committee.role === 'chair',
                            'bg-info': committee.role === 'vice_chair',
                            'bg-primary': committee.role === 'member',
                            'bg-warning': committee.role === 'secretary',
                          }"
                        >
                          {{ formatRole(committee.role) }}
                        </span>
                      </div>
                      <div class="committee-type small text-muted mb-1">
                        <i class="fas fa-tag me-1"></i>
                        {{ formatType(committee.type) }}
                      </div>
                      <div class="committee-dates small text-muted mb-2">
                        <i class="fas fa-calendar me-1"></i>
                        Started: {{ formatDate(committee.startDate) }}
                        <span v-if="committee.endDate">
                          • Ends: {{ formatDate(committee.endDate) }}
                        </span>
                      </div>
                      <div
                        v-if="committee.description"
                        class="committee-description small"
                      >
                        <i class="fas fa-info-circle me-1"></i>
                        {{ committee.description }}
                      </div>
                      <div
                        v-if="
                          committee.responsibilities &&
                          committee.responsibilities.length > 0
                        "
                        class="responsibilities mt-2"
                      >
                        <div class="small text-muted mb-1">
                          <i class="fas fa-tasks me-1"></i>Key Responsibilities:
                        </div>
                        <ul class="small list-unstyled ms-3">
                          <li
                            v-for="resp in committee.responsibilities"
                            :key="resp"
                            class="mb-1"
                          >
                            <i class="fas fa-arrow-right me-1 text-muted"></i
                            >{{ resp }}
                          </li>
                        </ul>
                      </div>
                      <div
                        v-if="committee.meetingSchedule"
                        class="meeting-schedule small mt-2"
                      >
                        <i class="fas fa-clock me-1"></i>
                        <strong>Meetings:</strong>
                        {{ committee.meetingSchedule }}
                      </div>
                    </div>
                  </div>
                  <div v-if="canEdit" class="ms-2">
                    <div class="btn-group-vertical btn-group-sm">
                      <button
                        class="btn btn-outline-primary"
                        @click="editCommittee(committee)"
                        title="Edit"
                      >
                        <i class="fas fa-edit"></i>
                      </button>
                      <button
                        class="btn btn-outline-danger"
                        @click="removeCommittee(committee.id)"
                        title="Remove"
                      >
                        <i class="fas fa-trash"></i>
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Past Committees -->
        <div v-if="pastCommittees.length > 0" class="mt-4">
          <h6 class="text-muted mb-3">
            <i class="fas fa-history me-2"></i>Past Assignments ({{
              pastCommittees.length
            }})
          </h6>
          <div class="row">
            <div
              v-for="committee in pastCommittees"
              :key="committee.id"
              class="col-md-6 col-lg-4 mb-3"
            >
              <div class="committee-card border rounded p-3 opacity-75">
                <div class="d-flex justify-content-between align-items-start">
                  <div class="flex-grow-1">
                    <h6 class="committee-name text-muted mb-2">
                      {{ committee.name }}
                    </h6>
                    <div class="committee-details">
                      <div class="role-display mb-2">
                        <span class="badge bg-secondary">
                          {{ formatRole(committee.role) }}
                        </span>
                      </div>
                      <div class="committee-type small text-muted mb-1">
                        <i class="fas fa-tag me-1"></i>
                        {{ formatType(committee.type) }}
                      </div>
                      <div class="committee-dates small text-muted mb-2">
                        <i class="fas fa-calendar me-1"></i>
                        {{ formatDate(committee.startDate) }} -
                        {{ formatDate(committee.endDate!) }}
                      </div>
                      <div
                        v-if="
                          committee.achievements &&
                          committee.achievements.length > 0
                        "
                        class="achievements mt-2"
                      >
                        <div class="small text-muted mb-1">
                          <i class="fas fa-trophy me-1"></i>Key Achievements:
                        </div>
                        <ul class="small list-unstyled ms-3">
                          <li
                            v-for="achievement in committee.achievements"
                            :key="achievement"
                            class="mb-1"
                          >
                            <i class="fas fa-arrow-right me-1 text-muted"></i
                            >{{ achievement }}
                          </li>
                        </ul>
                      </div>
                    </div>
                  </div>
                  <div v-if="canEdit" class="ms-2">
                    <button
                      class="btn btn-sm btn-outline-primary"
                      @click="editCommittee(committee)"
                      title="Edit"
                    >
                      <i class="fas fa-edit"></i>
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Empty State -->
        <div v-if="committees.length === 0" class="text-center text-muted py-5">
          <i class="fas fa-users fa-3x mb-3 opacity-50"></i>
          <h6>No Committee Assignments</h6>
          <p class="mb-3">
            Add committee assignments to showcase service contributions and
            leadership roles.
          </p>
          <button
            v-if="canEdit"
            class="btn btn-primary"
            @click="showAddCommittee = true"
          >
            <i class="fas fa-plus me-2"></i>Add Committee Assignment
          </button>
        </div>
      </div>
    </div>

    <!-- Committee Statistics -->
    <div v-if="committees.length > 0" class="card mt-4">
      <div class="card-header">
        <h5 class="mb-0">
          <i class="fas fa-chart-bar me-2"></i>Service Summary
        </h5>
      </div>
      <div class="card-body">
        <div class="row text-center">
          <div class="col-md-3">
            <div class="stat-item">
              <div class="stat-number text-primary">
                {{ committees.length }}
              </div>
              <div class="stat-label text-muted">Total Assignments</div>
            </div>
          </div>
          <div class="col-md-3">
            <div class="stat-item">
              <div class="stat-number text-success">
                {{ activeCommittees.length }}
              </div>
              <div class="stat-label text-muted">Currently Active</div>
            </div>
          </div>
          <div class="col-md-3">
            <div class="stat-item">
              <div class="stat-number text-warning">{{ leadershipRoles }}</div>
              <div class="stat-label text-muted">Leadership Roles</div>
            </div>
          </div>
          <div class="col-md-3">
            <div class="stat-item">
              <div class="stat-number text-info">{{ totalYearsService }}</div>
              <div class="stat-label text-muted">Years of Service</div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Add/Edit Committee Modal -->
    <div
      v-if="showAddCommittee || editingCommittee"
      class="modal d-block"
      tabindex="-1"
      style="background-color: rgba(0, 0, 0, 0.5)"
    >
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              {{
                editingCommittee
                  ? "Edit Committee Assignment"
                  : "Add Committee Assignment"
              }}
            </h5>
            <button
              type="button"
              class="btn-close"
              @click="closeModal"
            ></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="saveCommittee">
              <div class="row">
                <div class="col-md-8 mb-3">
                  <label class="form-label">Committee Name *</label>
                  <input
                    v-model="committeeForm.name"
                    type="text"
                    class="form-control"
                    placeholder="e.g., Faculty Senate, Curriculum Committee"
                    required
                  />
                </div>
                <div class="col-md-4 mb-3">
                  <label class="form-label">Your Role *</label>
                  <select
                    v-model="committeeForm.role"
                    class="form-select"
                    required
                  >
                    <option value="">Select Role</option>
                    <option value="member">Member</option>
                    <option value="chair">Chair</option>
                    <option value="vice_chair">Vice Chair</option>
                    <option value="secretary">Secretary</option>
                  </select>
                </div>
              </div>

              <div class="row">
                <div class="col-md-6 mb-3">
                  <label class="form-label">Committee Type *</label>
                  <select
                    v-model="committeeForm.type"
                    class="form-select"
                    required
                  >
                    <option value="">Select Type</option>
                    <option value="academic">Academic</option>
                    <option value="administrative">Administrative</option>
                    <option value="search">Search Committee</option>
                    <option value="curriculum">Curriculum</option>
                    <option value="student_affairs">Student Affairs</option>
                    <option value="research">Research</option>
                    <option value="external">External/Professional</option>
                    <option value="other">Other</option>
                  </select>
                </div>
                <div class="col-md-6 mb-3">
                  <label class="form-label">Status</label>
                  <select v-model="committeeForm.status" class="form-select">
                    <option value="active">Active</option>
                    <option value="completed">Completed</option>
                    <option value="on_hold">On Hold</option>
                  </select>
                </div>
              </div>

              <div class="row">
                <div class="col-md-6 mb-3">
                  <label class="form-label">Start Date *</label>
                  <input
                    v-model="committeeForm.startDate"
                    type="date"
                    class="form-control"
                    required
                  />
                </div>
                <div class="col-md-6 mb-3">
                  <label class="form-label">End Date</label>
                  <input
                    v-model="committeeForm.endDate"
                    type="date"
                    class="form-control"
                  />
                  <div class="form-text">Leave blank if ongoing</div>
                </div>
              </div>

              <div class="mb-3">
                <label class="form-label">Description</label>
                <textarea
                  v-model="committeeForm.description"
                  class="form-control"
                  rows="2"
                  placeholder="Brief description of the committee's purpose..."
                ></textarea>
              </div>

              <div class="mb-3">
                <label class="form-label">Meeting Schedule</label>
                <input
                  v-model="committeeForm.meetingSchedule"
                  type="text"
                  class="form-control"
                  placeholder="e.g., Monthly, First Tuesday of each month, As needed"
                />
              </div>

              <div class="mb-3">
                <label class="form-label">Key Responsibilities</label>
                <div class="input-group mb-2">
                  <input
                    v-model="newResponsibility"
                    type="text"
                    class="form-control"
                    placeholder="Add a responsibility..."
                    @keyup.enter="addResponsibility"
                  />
                  <button
                    type="button"
                    class="btn btn-outline-primary"
                    @click="addResponsibility"
                  >
                    <i class="fas fa-plus"></i>
                  </button>
                </div>
                <div
                  v-if="committeeForm.responsibilities.length > 0"
                  class="responsibility-list"
                >
                  <div
                    v-for="(resp, index) in committeeForm.responsibilities"
                    :key="index"
                    class="d-flex align-items-center mb-2"
                  >
                    <small class="flex-grow-1 bg-light p-2 rounded">{{
                      resp
                    }}</small>
                    <button
                      type="button"
                      class="btn btn-sm btn-outline-danger ms-2"
                      @click="removeResponsibility(index)"
                    >
                      <i class="fas fa-times"></i>
                    </button>
                  </div>
                </div>
              </div>

              <div v-if="committeeForm.status === 'completed'" class="mb-3">
                <label class="form-label">Key Achievements</label>
                <div class="input-group mb-2">
                  <input
                    v-model="newAchievement"
                    type="text"
                    class="form-control"
                    placeholder="Add an achievement..."
                    @keyup.enter="addAchievement"
                  />
                  <button
                    type="button"
                    class="btn btn-outline-success"
                    @click="addAchievement"
                  >
                    <i class="fas fa-plus"></i>
                  </button>
                </div>
                <div
                  v-if="committeeForm.achievements.length > 0"
                  class="achievement-list"
                >
                  <div
                    v-for="(achievement, index) in committeeForm.achievements"
                    :key="index"
                    class="d-flex align-items-center mb-2"
                  >
                    <small class="flex-grow-1 bg-light p-2 rounded">{{
                      achievement
                    }}</small>
                    <button
                      type="button"
                      class="btn btn-sm btn-outline-danger ms-2"
                      @click="removeAchievement(index)"
                    >
                      <i class="fas fa-times"></i>
                    </button>
                  </div>
                </div>
              </div>
            </form>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="closeModal">
              Cancel
            </button>
            <button
              type="button"
              class="btn btn-primary"
              @click="saveCommittee"
            >
              {{ editingCommittee ? "Update" : "Save" }} Assignment
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import { useFacultyProfileStore } from "@/stores/facultyProfile";
import type { Committee } from "@/types";

// Props
const props = defineProps<{
  facultyId?: string;
  canEdit: boolean;
}>();

// Store
const profileStore = useFacultyProfileStore();

// Component state
const showAddCommittee = ref(false);
const editingCommittee = ref<Committee | null>(null);
const newResponsibility = ref("");
const newAchievement = ref("");
const committeeForm = ref({
  name: "",
  type: "",
  role: "",
  status: "active",
  startDate: "",
  endDate: "",
  description: "",
  meetingSchedule: "",
  responsibilities: [] as string[],
  achievements: [] as string[],
});

// Computed properties
const committees = computed(() => profileStore.committees);
const activeCommittees = computed(() =>
  committees.value.filter((c) => c.status === "active")
);
const pastCommittees = computed(() =>
  committees.value.filter((c) => c.status === "completed")
);
const leadershipRoles = computed(
  () =>
    committees.value.filter((c) => ["chair", "vice_chair"].includes(c.role))
      .length
);
const totalYearsService = computed(() => {
  const now = new Date();
  return committees.value
    .reduce((total, committee) => {
      const start = new Date(committee.startDate);
      const end = committee.endDate ? new Date(committee.endDate) : now;
      const years =
        (end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24 * 365.25);
      return total + Math.max(0, years);
    }, 0)
    .toFixed(1);
});

// Methods
const formatRole = (role: string) => {
  const roles: Record<string, string> = {
    member: "Member",
    chair: "Chair",
    vice_chair: "Vice Chair",
    secretary: "Secretary",
  };
  return roles[role] || role;
};

const formatType = (type: string) => {
  const types: Record<string, string> = {
    academic: "Academic Committee",
    administrative: "Administrative Committee",
    search: "Search Committee",
    curriculum: "Curriculum Committee",
    student_affairs: "Student Affairs",
    research: "Research Committee",
    external: "External/Professional",
    other: "Other",
  };
  return types[type] || type;
};

const formatDate = (date: string | Date) => {
  return new Intl.DateTimeFormat("en-US", {
    year: "numeric",
    month: "short",
  }).format(new Date(date));
};

const editCommittee = (committee: Committee) => {
  editingCommittee.value = committee;
  committeeForm.value = {
    name: committee.name,
    type: committee.type,
    role: committee.role,
    status: committee.status,
    startDate: committee.startDate.toString().split("T")[0],
    endDate: committee.endDate
      ? committee.endDate.toString().split("T")[0]
      : "",
    description: committee.description || "",
    meetingSchedule: committee.meetingSchedule || "",
    responsibilities: [...(committee.responsibilities || [])],
    achievements: [...(committee.achievements || [])],
  };
};

const removeCommittee = async (committeeId: string) => {
  if (confirm("Are you sure you want to remove this committee assignment?")) {
    try {
      const updatedCommittees = committees.value.filter(
        (c) => c.id !== committeeId
      );
      await profileStore.updateCommitteeAssignments(updatedCommittees);
    } catch (error) {
      console.error("Failed to remove committee:", error);
    }
  }
};

const addResponsibility = () => {
  if (newResponsibility.value.trim()) {
    committeeForm.value.responsibilities.push(newResponsibility.value.trim());
    newResponsibility.value = "";
  }
};

const removeResponsibility = (index: number) => {
  committeeForm.value.responsibilities.splice(index, 1);
};

const addAchievement = () => {
  if (newAchievement.value.trim()) {
    committeeForm.value.achievements.push(newAchievement.value.trim());
    newAchievement.value = "";
  }
};

const removeAchievement = (index: number) => {
  committeeForm.value.achievements.splice(index, 1);
};

const saveCommittee = async () => {
  try {
    const newCommittee: Committee = {
      id: editingCommittee.value?.id || `committee_${Date.now()}`,
      facultyId: props.facultyId || "1",
      name: committeeForm.value.name,
      type: committeeForm.value.type as any,
      role: committeeForm.value.role as any,
      status: committeeForm.value.status as any,
      startDate: new Date(committeeForm.value.startDate),
      endDate: committeeForm.value.endDate
        ? new Date(committeeForm.value.endDate)
        : undefined,
      description: committeeForm.value.description || undefined,
      meetingSchedule: committeeForm.value.meetingSchedule || undefined,
      responsibilities:
        committeeForm.value.responsibilities.length > 0
          ? committeeForm.value.responsibilities
          : undefined,
      achievements:
        committeeForm.value.achievements.length > 0
          ? committeeForm.value.achievements
          : undefined,
    };

    let updatedCommittees: Committee[];
    if (editingCommittee.value) {
      // Update existing
      updatedCommittees = committees.value.map((c) =>
        c.id === editingCommittee.value!.id ? newCommittee : c
      );
    } else {
      // Add new
      updatedCommittees = [...committees.value, newCommittee];
    }

    await profileStore.updateCommitteeAssignments(updatedCommittees);
    closeModal();
  } catch (error) {
    console.error("Failed to save committee:", error);
  }
};

const closeModal = () => {
  showAddCommittee.value = false;
  editingCommittee.value = null;
  newResponsibility.value = "";
  newAchievement.value = "";
  committeeForm.value = {
    name: "",
    type: "",
    role: "",
    status: "active",
    startDate: "",
    endDate: "",
    description: "",
    meetingSchedule: "",
    responsibilities: [],
    achievements: [],
  };
};

// Lifecycle
onMounted(async () => {
  if (props.facultyId) {
    // Committees are loaded with the profile
  }
});
</script>

<style scoped>
.committee-card {
  transition: all 0.2s ease;
  height: 100%;
}

.committee-card:hover {
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.committee-name {
  font-size: 1.1rem;
  font-weight: 600;
}

.stat-item {
  padding: 1rem;
}

.stat-number {
  font-size: 2rem;
  font-weight: bold;
}

.stat-label {
  font-size: 0.875rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.modal {
  z-index: 1050;
}

.btn-group-vertical .btn {
  border-radius: 0.25rem !important;
  margin-bottom: 2px;
}

.btn-group-vertical .btn:last-child {
  margin-bottom: 0;
}

.responsibility-list,
.achievement-list {
  max-height: 200px;
  overflow-y: auto;
}
</style>
