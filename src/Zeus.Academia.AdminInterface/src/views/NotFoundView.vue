<template>
  <div class="container text-center mt-5">
    <div class="row justify-content-center">
      <div class="col-md-6">
        <div class="error-page">
          <h1 class="display-1 text-muted">404</h1>
          <h2>Page Not Found</h2>
          <p class="lead">
            The page you are looking for doesn't exist or has been moved.
          </p>
          <router-link to="/dashboard" class="btn btn-primary">
            <i class="fas fa-home me-2"></i>
            Go to Dashboard
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Not Found View
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
