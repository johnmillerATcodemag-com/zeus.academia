<!-- Faculty Office Hours Component -->
<template>
  <div class="faculty-office-hours">
    <div class="card">
      <div
        class="card-header d-flex justify-content-between align-items-center"
      >
        <h5 class="mb-0">
          <i class="fas fa-clock me-2"></i>Office Hours Schedule
        </h5>
        <button
          v-if="canEdit"
          class="btn btn-sm btn-primary"
          @click="showAddHours = true"
        >
          <i class="fas fa-plus me-1"></i>Add Hours
        </button>
      </div>
      <div class="card-body">
        <!-- Weekly Schedule View -->
        <div v-if="officeHours.length > 0" class="office-hours-schedule">
          <div class="row">
            <div
              v-for="day in weekDays"
              :key="day.key"
              class="col-md-6 col-lg-4 mb-3"
            >
              <div class="day-schedule">
                <h6 class="day-header text-primary mb-2">
                  <i class="fas fa-calendar-day me-2"></i>{{ day.name }}
                </h6>
                <div
                  v-for="hours in getHoursForDay(day.key)"
                  :key="hours.id"
                  class="office-hours-block border rounded p-3 mb-2"
                  :class="{
                    'border-success': hours.type === 'office_hours',
                    'border-info': hours.type === 'virtual_hours',
                  }"
                >
                  <div class="d-flex justify-content-between align-items-start">
                    <div class="flex-grow-1">
                      <div class="time-display fw-bold">
                        {{ formatTime(hours.startTime) }} -
                        {{ formatTime(hours.endTime) }}
                      </div>
                      <div class="location-display text-muted small">
                        <i
                          :class="
                            hours.type === 'virtual_hours'
                              ? 'fas fa-video'
                              : 'fas fa-map-marker-alt'
                          "
                        ></i>
                        {{ hours.location }}
                      </div>
                      <div class="capacity-display small text-success mt-1">
                        <i class="fas fa-users me-1"></i>
                        Up to {{ hours.maxAppointments }} appointments ({{
                          hours.appointmentDuration
                        }}min each)
                      </div>
                      <div
                        v-if="
                          hours.meetingUrl && hours.type === 'virtual_hours'
                        "
                        class="virtual-link small mt-1"
                      >
                        <a
                          :href="hours.meetingUrl"
                          target="_blank"
                          class="text-decoration-none"
                        >
                          <i class="fas fa-external-link-alt me-1"></i>Join
                          Meeting
                        </a>
                      </div>
                    </div>
                    <div v-if="canEdit" class="ms-2">
                      <div class="btn-group-vertical btn-group-sm">
                        <button
                          class="btn btn-outline-primary"
                          @click="editHours(hours)"
                          title="Edit"
                        >
                          <i class="fas fa-edit"></i>
                        </button>
                        <button
                          class="btn btn-outline-danger"
                          @click="deleteHours(hours.id)"
                          title="Delete"
                        >
                          <i class="fas fa-trash"></i>
                        </button>
                      </div>
                    </div>
                  </div>
                </div>
                <div
                  v-if="getHoursForDay(day.key).length === 0"
                  class="text-muted text-center py-3"
                >
                  <i class="fas fa-calendar-times opacity-50"></i>
                  <div class="small">No office hours</div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Empty State -->
        <div v-else class="text-center text-muted py-5">
          <i class="fas fa-clock fa-3x mb-3 opacity-50"></i>
          <h6>No Office Hours Configured</h6>
          <p class="mb-3">
            Set up your office hours schedule to allow students to book
            appointments.
          </p>
          <button
            v-if="canEdit"
            class="btn btn-primary"
            @click="showAddHours = true"
          >
            <i class="fas fa-plus me-2"></i>Add Office Hours
          </button>
        </div>
      </div>
    </div>

    <!-- Upcoming Appointments -->
    <div v-if="upcomingAppointments.length > 0" class="card mt-4">
      <div class="card-header">
        <h5 class="mb-0">
          <i class="fas fa-calendar-check me-2"></i>Upcoming Appointments
        </h5>
      </div>
      <div class="card-body">
        <div class="row">
          <div
            v-for="appointment in upcomingAppointments"
            :key="appointment.id"
            class="col-md-6 mb-3"
          >
            <div class="appointment-card border rounded p-3">
              <div class="d-flex justify-content-between align-items-start">
                <div>
                  <h6 class="mb-1">{{ appointment.studentName }}</h6>
                  <p class="text-muted small mb-1">
                    {{ appointment.studentEmail }}
                  </p>
                  <p class="small mb-1">
                    <i class="fas fa-calendar me-1"></i>
                    {{ formatAppointmentDate(appointment.appointmentDate) }}
                  </p>
                  <p class="small mb-1">
                    <i class="fas fa-clock me-1"></i>
                    {{ appointment.startTime }} - {{ appointment.endTime }}
                  </p>
                  <p class="small mb-2">
                    <i class="fas fa-comment me-1"></i>
                    {{ appointment.purpose }}
                  </p>
                  <span
                    class="badge"
                    :class="{
                      'bg-success': appointment.status === 'confirmed',
                      'bg-warning': appointment.status === 'pending',
                      'bg-danger': appointment.status === 'cancelled',
                    }"
                  >
                    {{ appointment.status.toUpperCase() }}
                  </span>
                </div>
                <div class="btn-group-vertical btn-group-sm">
                  <button
                    v-if="appointment.status === 'pending'"
                    class="btn btn-outline-success"
                    @click="confirmAppointment(appointment.id)"
                    title="Confirm"
                  >
                    <i class="fas fa-check"></i>
                  </button>
                  <button
                    class="btn btn-outline-danger"
                    @click="cancelAppointment(appointment.id)"
                    title="Cancel"
                  >
                    <i class="fas fa-times"></i>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Add/Edit Office Hours Modal -->
    <div
      v-if="showAddHours || editingHours"
      class="modal d-block"
      tabindex="-1"
      style="background-color: rgba(0, 0, 0, 0.5)"
    >
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              {{ editingHours ? "Edit Office Hours" : "Add Office Hours" }}
            </h5>
            <button
              type="button"
              class="btn-close"
              @click="closeModal"
            ></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="saveHours">
              <div class="row">
                <div class="col-md-6 mb-3">
                  <label class="form-label">Day of Week</label>
                  <select
                    v-model="hoursForm.dayOfWeek"
                    class="form-select"
                    required
                  >
                    <option value="">Select Day</option>
                    <option
                      v-for="day in weekDays"
                      :key="day.key"
                      :value="day.key"
                    >
                      {{ day.name }}
                    </option>
                  </select>
                </div>
                <div class="col-md-6 mb-3">
                  <label class="form-label">Type</label>
                  <select v-model="hoursForm.type" class="form-select" required>
                    <option value="office_hours">In-Person Office Hours</option>
                    <option value="virtual_hours">Virtual Office Hours</option>
                    <option value="by_appointment">By Appointment Only</option>
                  </select>
                </div>
              </div>

              <div class="row">
                <div class="col-md-6 mb-3">
                  <label class="form-label">Start Time</label>
                  <input
                    v-model="hoursForm.startTime"
                    type="time"
                    class="form-control"
                    required
                  />
                </div>
                <div class="col-md-6 mb-3">
                  <label class="form-label">End Time</label>
                  <input
                    v-model="hoursForm.endTime"
                    type="time"
                    class="form-control"
                    required
                  />
                </div>
              </div>

              <div class="mb-3">
                <label class="form-label">Location</label>
                <input
                  v-model="hoursForm.location"
                  type="text"
                  class="form-control"
                  placeholder="e.g., Engineering Building Room 301"
                  required
                />
              </div>

              <div v-if="hoursForm.type === 'virtual_hours'" class="mb-3">
                <label class="form-label">Meeting URL</label>
                <input
                  v-model="hoursForm.meetingUrl"
                  type="url"
                  class="form-control"
                  placeholder="https://zoom.us/j/..."
                />
              </div>

              <div class="row">
                <div class="col-md-6 mb-3">
                  <label class="form-label">Max Appointments</label>
                  <input
                    v-model.number="hoursForm.maxAppointments"
                    type="number"
                    class="form-control"
                    min="1"
                    max="20"
                    required
                  />
                </div>
                <div class="col-md-6 mb-3">
                  <label class="form-label"
                    >Appointment Duration (minutes)</label
                  >
                  <select
                    v-model.number="hoursForm.appointmentDuration"
                    class="form-select"
                    required
                  >
                    <option value="15">15 minutes</option>
                    <option value="20">20 minutes</option>
                    <option value="30">30 minutes</option>
                    <option value="45">45 minutes</option>
                    <option value="60">60 minutes</option>
                  </select>
                </div>
              </div>

              <div class="mb-3">
                <div class="form-check">
                  <input
                    v-model="hoursForm.isRecurring"
                    class="form-check-input"
                    type="checkbox"
                    id="isRecurring"
                  />
                  <label class="form-check-label" for="isRecurring">
                    Recurring weekly schedule
                  </label>
                </div>
              </div>

              <div class="mb-3">
                <label class="form-label">Notes (Optional)</label>
                <textarea
                  v-model="hoursForm.notes"
                  class="form-control"
                  rows="2"
                  placeholder="Any additional information for students..."
                ></textarea>
              </div>
            </form>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="closeModal">
              Cancel
            </button>
            <button type="button" class="btn btn-primary" @click="saveHours">
              {{ editingHours ? "Update" : "Save" }} Office Hours
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
import type { OfficeHours } from "@/types";

// Props
const props = defineProps<{
  facultyId?: string;
  canEdit: boolean;
}>();

// Store
const profileStore = useFacultyProfileStore();

// Component state
const showAddHours = ref(false);
const editingHours = ref<OfficeHours | null>(null);
const hoursForm = ref({
  dayOfWeek: "",
  startTime: "",
  endTime: "",
  location: "",
  type: "office_hours",
  maxAppointments: 6,
  appointmentDuration: 20,
  isRecurring: true,
  meetingUrl: "",
  notes: "",
});

// Week days configuration
const weekDays = [
  { key: "monday", name: "Monday" },
  { key: "tuesday", name: "Tuesday" },
  { key: "wednesday", name: "Wednesday" },
  { key: "thursday", name: "Thursday" },
  { key: "friday", name: "Friday" },
  { key: "saturday", name: "Saturday" },
  { key: "sunday", name: "Sunday" },
];

// Computed properties
const officeHours = computed(() => profileStore.officeHours);
const upcomingAppointments = computed(() => profileStore.upcomingAppointments);

// Methods
const getHoursForDay = (day: string) => {
  return officeHours.value.filter((hours) => hours.dayOfWeek === day);
};

const formatTime = (time: string) => {
  const [hours, minutes] = time.split(":").map(Number);
  const period = hours >= 12 ? "PM" : "AM";
  const displayHours = hours % 12 || 12;
  return `${displayHours}:${minutes.toString().padStart(2, "0")} ${period}`;
};

const formatAppointmentDate = (date: Date) => {
  return new Intl.DateTimeFormat("en-US", {
    weekday: "long",
    year: "numeric",
    month: "short",
    day: "numeric",
  }).format(new Date(date));
};

const editHours = (hours: OfficeHours) => {
  editingHours.value = hours;
  hoursForm.value = {
    dayOfWeek: hours.dayOfWeek,
    startTime: hours.startTime,
    endTime: hours.endTime,
    location: hours.location,
    type: hours.type,
    maxAppointments: hours.maxAppointments,
    appointmentDuration: hours.appointmentDuration,
    isRecurring: hours.isRecurring,
    meetingUrl: hours.meetingUrl || "",
    notes: hours.notes || "",
  };
};

const deleteHours = async (hoursId: string) => {
  if (confirm("Are you sure you want to delete these office hours?")) {
    try {
      const updatedHours = officeHours.value.filter((h) => h.id !== hoursId);
      await profileStore.updateOfficeHours(updatedHours);
    } catch (error) {
      console.error("Failed to delete office hours:", error);
    }
  }
};

const saveHours = async () => {
  try {
    const newHours: OfficeHours = {
      id: editingHours.value?.id || `hours_${Date.now()}`,
      facultyId: props.facultyId || "1",
      dayOfWeek: hoursForm.value.dayOfWeek as any,
      startTime: hoursForm.value.startTime,
      endTime: hoursForm.value.endTime,
      location: hoursForm.value.location,
      type: hoursForm.value.type as any,
      isRecurring: hoursForm.value.isRecurring,
      maxAppointments: hoursForm.value.maxAppointments,
      appointmentDuration: hoursForm.value.appointmentDuration,
      meetingUrl: hoursForm.value.meetingUrl || undefined,
      notes: hoursForm.value.notes || undefined,
    };

    let updatedHours: OfficeHours[];
    if (editingHours.value) {
      // Update existing
      updatedHours = officeHours.value.map((h) =>
        h.id === editingHours.value!.id ? newHours : h
      );
    } else {
      // Add new
      updatedHours = [...officeHours.value, newHours];
    }

    await profileStore.updateOfficeHours(updatedHours);
    closeModal();
  } catch (error) {
    console.error("Failed to save office hours:", error);
  }
};

const closeModal = () => {
  showAddHours.value = false;
  editingHours.value = null;
  hoursForm.value = {
    dayOfWeek: "",
    startTime: "",
    endTime: "",
    location: "",
    type: "office_hours",
    maxAppointments: 6,
    appointmentDuration: 20,
    isRecurring: true,
    meetingUrl: "",
    notes: "",
  };
};

const confirmAppointment = async (appointmentId: string) => {
  try {
    // Implementation would call profile store method
    console.log("Confirm appointment:", appointmentId);
  } catch (error) {
    console.error("Failed to confirm appointment:", error);
  }
};

const cancelAppointment = async (appointmentId: string) => {
  if (confirm("Are you sure you want to cancel this appointment?")) {
    try {
      // Implementation would call profile store method
      console.log("Cancel appointment:", appointmentId);
    } catch (error) {
      console.error("Failed to cancel appointment:", error);
    }
  }
};

// Lifecycle
onMounted(async () => {
  if (props.facultyId) {
    // Office hours and appointments are loaded with the profile
  }
});
</script>

<style scoped>
.office-hours-block {
  transition: all 0.2s ease;
}

.office-hours-block:hover {
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.day-header {
  border-bottom: 2px solid #e9ecef;
  padding-bottom: 0.5rem;
  margin-bottom: 1rem;
}

.time-display {
  font-size: 1.1rem;
  color: #0d6efd;
}

.appointment-card {
  transition: all 0.2s ease;
}

.appointment-card:hover {
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
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
</style>
