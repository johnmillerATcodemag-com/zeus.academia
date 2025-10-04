<template>
  <nav class="navbar navbar-expand-lg navbar-dark bg-zeus-primary">
    <div class="container">
      <router-link class="navbar-brand" to="/">
        <strong>Zeus Academia</strong>
      </router-link>

      <button
        class="navbar-toggler"
        type="button"
        data-bs-toggle="collapse"
        data-bs-target="#navbarNav"
        aria-controls="navbarNav"
        aria-expanded="false"
        aria-label="Toggle navigation"
      >
        <span class="navbar-toggler-icon"></span>
      </button>

      <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav ms-auto">
          <!-- Public navigation -->
          <li v-if="!isAuthenticated" class="nav-item">
            <router-link class="nav-link" to="/">Home</router-link>
          </li>

          <!-- Authenticated navigation -->
          <template v-if="isAuthenticated">
            <li class="nav-item">
              <router-link class="nav-link" to="/dashboard"
                >Dashboard</router-link
              >
            </li>
            <li class="nav-item">
              <router-link class="nav-link" to="/courses">Courses</router-link>
            </li>
            <li class="nav-item dropdown">
              <a
                class="nav-link dropdown-toggle"
                href="#"
                id="navbarDropdown"
                role="button"
                data-bs-toggle="dropdown"
                aria-expanded="false"
              >
                {{ studentName }}
              </a>
              <ul class="dropdown-menu" aria-labelledby="navbarDropdown">
                <li>
                  <router-link class="dropdown-item" to="/profile"
                    >Profile</router-link
                  >
                </li>
                <li><hr class="dropdown-divider" /></li>
                <li>
                  <button class="dropdown-item" @click="handleLogout">
                    Logout
                  </button>
                </li>
              </ul>
            </li>
          </template>

          <!-- Login button -->
          <li v-if="!isAuthenticated" class="nav-item">
            <router-link class="btn btn-outline-light ms-2" to="/login"
              >Login</router-link
            >
          </li>
        </ul>
      </div>
    </div>
  </nav>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useRouter } from "vue-router";
import { useStore } from "vuex";

const router = useRouter();
const store = useStore();

// Computed properties from Vuex store
const isAuthenticated = computed(() => store.getters["auth/isAuthenticated"]);
const studentName = computed(() => {
  const student = store.getters["auth/currentStudent"];
  if (student) {
    return `${student.firstName} ${student.lastName}`;
  }
  return "Student";
});

const handleLogout = () => {
  // Use Vuex store logout action
  store.dispatch("auth/logout");
  router.push("/");
};
</script>

<style scoped>
.navbar-brand {
  font-size: 1.5rem;
}

.nav-link {
  font-weight: 500;
}

.dropdown-item {
  cursor: pointer;
}
</style>
