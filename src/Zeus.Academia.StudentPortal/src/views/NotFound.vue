<template>
  <div class="not-found-page min-vh-100 d-flex align-items-center">
    <div class="container">
      <div class="row justify-content-center text-center">
        <div class="col-lg-6">
          <div class="error-content">
            <h1 class="display-1 fw-bold zeus-primary">404</h1>
            <h2 class="mb-4">Page Not Found</h2>
            <p class="lead text-muted mb-4">
              Sorry, the page you are looking for doesn't exist or has been
              moved.
            </p>
            <div
              class="d-flex flex-column flex-sm-row gap-3 justify-content-center"
            >
              <router-link to="/" class="btn btn-primary btn-lg">
                <i class="bi bi-house-fill me-2"></i>
                Go Home
              </router-link>
              <button class="btn btn-outline-secondary btn-lg" @click="goBack">
                <i class="bi bi-arrow-left me-2"></i>
                Go Back
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from "vue-router";

const router = useRouter();

const goBack = () => {
  if (window.history.length > 1) {
    router.go(-1);
  } else {
    router.push("/");
  }
};
</script>

<style scoped>
.not-found-page {
  background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
}

.error-content {
  animation: fadeInUp 0.6s ease-out;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.display-1 {
  font-size: 8rem;
  line-height: 1;
}

@media (max-width: 576px) {
  .display-1 {
    font-size: 6rem;
  }
}
</style>
