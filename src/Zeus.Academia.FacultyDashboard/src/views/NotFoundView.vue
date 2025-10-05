<template>
  <div
    class="not-found-container d-flex align-items-center justify-content-center min-vh-100"
  >
    <div class="text-center">
      <div class="error-icon mb-4">
        <i
          class="fas fa-question-circle"
          style="font-size: 4rem; color: #6c757d"
        ></i>
      </div>
      <h1 class="error-title">404 - Page Not Found</h1>
      <p class="error-message">The page you're looking for doesn't exist.</p>
      <div class="error-actions mt-4">
        <button @click="goBack" class="btn btn-primary me-2">Go Back</button>
        <router-link to="/dashboard" class="btn btn-outline-primary"
          >Dashboard</router-link
        >
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from "vue-router";

const router = useRouter();

const goBack = () => {
  router.back();
};
</script>

<style scoped>
.not-found-container {
  background: #f8f9fa;
}
.error-title {
  font-size: 2rem;
  color: #495057;
}
.error-message {
  font-size: 1.1rem;
  color: #6c757d;
}
</style>
