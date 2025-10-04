<template>
  <div class="home-page">
    <div class="hero-section bg-zeus-primary text-white py-5">
      <div class="container">
        <div class="row align-items-center">
          <div class="col-lg-6">
            <h1 class="display-4 fw-bold mb-4">Welcome to Zeus Academia</h1>
            <p class="lead mb-4">
              Your gateway to academic excellence. Access your courses, track
              your progress, and connect with your educational journey.
            </p>
            <div class="d-flex gap-3">
              <router-link
                v-if="!isAuthenticated"
                to="/login"
                class="btn btn-light btn-lg"
              >
                Student Login
              </router-link>
              <router-link v-else to="/dashboard" class="btn btn-light btn-lg">
                Go to Dashboard
              </router-link>
            </div>
          </div>
          <div class="col-lg-6">
            <div
              class="hero-placeholder d-flex align-items-center justify-content-center bg-light rounded"
              style="height: 400px"
            >
              <i
                class="bi bi-mortarboard-fill text-muted"
                style="font-size: 6rem"
              ></i>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="features-section py-5">
      <div class="container">
        <div class="row text-center mb-5">
          <div class="col">
            <h2 class="mb-3">Everything You Need</h2>
            <p class="lead text-muted">
              Comprehensive tools for your academic success
            </p>
          </div>
        </div>

        <div class="row g-4">
          <div class="col-md-4">
            <div class="card h-100 border-0 shadow-sm">
              <div class="card-body text-center p-4">
                <div class="mb-3">
                  <i
                    class="bi bi-book-fill text-zeus-primary"
                    style="font-size: 3rem"
                  ></i>
                </div>
                <h5 class="card-title">Course Management</h5>
                <p class="card-text">
                  View your enrolled courses, access materials, and track your
                  academic progress.
                </p>
              </div>
            </div>
          </div>

          <div class="col-md-4">
            <div class="card h-100 border-0 shadow-sm">
              <div class="card-body text-center p-4">
                <div class="mb-3">
                  <i
                    class="bi bi-calendar-check-fill text-zeus-secondary"
                    style="font-size: 3rem"
                  ></i>
                </div>
                <h5 class="card-title">Schedule Planning</h5>
                <p class="card-text">
                  Organize your academic schedule and never miss important
                  deadlines.
                </p>
              </div>
            </div>
          </div>

          <div class="col-md-4">
            <div class="card h-100 border-0 shadow-sm">
              <div class="card-body text-center p-4">
                <div class="mb-3">
                  <i
                    class="bi bi-graph-up text-zeus-accent"
                    style="font-size: 3rem"
                  ></i>
                </div>
                <h5 class="card-title">Progress Tracking</h5>
                <p class="card-text">
                  Monitor your academic performance with detailed analytics and
                  insights.
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";

const isAuthenticated = ref(false);
</script>

<style scoped>
.hero-section {
  background: linear-gradient(
    135deg,
    var(--zeus-primary) 0%,
    var(--zeus-secondary) 100%
  );
}

.card {
  transition: transform 0.2s ease-in-out;
}

.card:hover {
  transform: translateY(-5px);
}
</style>
