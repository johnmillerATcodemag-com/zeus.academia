<template>
  <div
    v-if="show"
    class="toast-container position-fixed top-0 end-0 p-3"
    style="z-index: 1055"
  >
    <div
      class="toast show"
      :class="toastClass"
      role="alert"
      aria-live="assertive"
      aria-atomic="true"
    >
      <div class="toast-header">
        <i :class="iconClass" class="me-2"></i>
        <strong class="me-auto">{{ displayTitle }}</strong>
        <button
          type="button"
          class="btn-close"
          @click="dismiss"
          aria-label="Close"
        ></button>
      </div>
      <div class="toast-body">
        {{ message }}
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted } from "vue";

interface Props {
  show: boolean;
  type?: "info" | "success" | "warning" | "error";
  title?: string;
  message: string;
  autoHide?: boolean;
  delay?: number;
}

const props = withDefaults(defineProps<Props>(), {
  type: "info",
  title: "",
  autoHide: true,
  delay: 5000,
});

// Computed property to provide default titles based on type
const displayTitle = computed(() => {
  if (props.title) return props.title;

  switch (props.type) {
    case "success":
      return "Success";
    case "error":
      return "Error";
    case "warning":
      return "Warning";
    case "info":
    default:
      return "Information";
  }
});

const emit = defineEmits<{
  dismiss: [];
}>();

const toastClass = computed(() => {
  switch (props.type) {
    case "success":
      return "text-bg-success";
    case "warning":
      return "text-bg-warning";
    case "error":
      return "text-bg-danger";
    default:
      return "text-bg-info";
  }
});

const iconClass = computed(() => {
  switch (props.type) {
    case "success":
      return "bi bi-check-circle-fill text-white";
    case "warning":
      return "bi bi-exclamation-triangle-fill text-dark";
    case "error":
      return "bi bi-x-circle-fill text-white";
    default:
      return "bi bi-info-circle-fill text-white";
  }
});

const dismiss = () => {
  emit("dismiss");
};

onMounted(() => {
  if (props.autoHide && props.show) {
    setTimeout(() => {
      dismiss();
    }, props.delay);
  }
});
</script>

<style scoped>
.toast {
  min-width: 300px;
}
</style>
