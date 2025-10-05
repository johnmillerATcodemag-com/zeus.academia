<template>
  <div class="container text-center mt-5">
    <div class="row justify-content-center">
      <div class="col-md-6">
        <div class="error-page">
          <h1 class="display-1 text-danger">403</h1>
          <h2>Access Denied</h2>
          <p class="lead">You don't have permission to access this resource.</p>
          <div class="mt-4">
            <p class="text-muted">
              <i class="fas fa-shield-alt me-2"></i>
              This area requires special administrative privileges.
            </p>
            <router-link to="/dashboard" class="btn btn-primary me-2">
              <i class="fas fa-home me-2"></i>
              Go to Dashboard
            </router-link>
            <button @click="$router.go(-1)" class="btn btn-secondary">
              <i class="fas fa-arrow-left me-2"></i>
              Go Back
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Unauthorized View
</script>

<style scoped>
.error-page {
  padding: 2rem;
}

.display-1 {
  font-size: 8rem;
  font-weight: 300;
}
</style>
