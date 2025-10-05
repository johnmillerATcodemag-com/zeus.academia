<template>
  <header class="faculty-header">
    <div class="header-left">
      <button
        class="btn btn-link text-white d-lg-none"
        @click="toggleSidebar"
        aria-label="Toggle sidebar"
      >
        <i class="fas fa-bars"></i>
      </button>
      <h1 class="header-title mb-0">{{ currentPageTitle }}</h1>
    </div>

    <div class="header-right">
      <div class="header-actions d-flex align-items-center gap-3">
        <!-- Notifications -->
        <div class="dropdown">
          <button
            class="btn btn-link text-white position-relative"
            id="notificationsDropdown"
            data-bs-toggle="dropdown"
            aria-expanded="false"
          >
            <i class="fas fa-bell"></i>
            <span
              class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger"
            >
              3
            </span>
          </button>
          <ul
            class="dropdown-menu dropdown-menu-end"
            aria-labelledby="notificationsDropdown"
          >
            <li><h6 class="dropdown-header">Notifications</h6></li>
            <li>
              <a class="dropdown-item" href="#">New assignment submission</a>
            </li>
            <li>
              <a class="dropdown-item" href="#">Grade deadline reminder</a>
            </li>
            <li>
              <a class="dropdown-item" href="#">Course enrollment update</a>
            </li>
            <li><hr class="dropdown-divider" /></li>
            <li>
              <a class="dropdown-item" href="#">View all notifications</a>
            </li>
          </ul>
        </div>

        <!-- User menu -->
        <div class="dropdown">
          <button
            class="btn btn-link text-white d-flex align-items-center gap-2"
            id="userDropdown"
            data-bs-toggle="dropdown"
            aria-expanded="false"
          >
            <div class="user-avatar">
              {{ userInitials }}
            </div>
            <span class="d-none d-md-inline">{{ userFullName }}</span>
            <i class="fas fa-chevron-down"></i>
          </button>
          <ul
            class="dropdown-menu dropdown-menu-end"
            aria-labelledby="userDropdown"
          >
            <li>
              <h6 class="dropdown-header">
                {{ userFullName }}<br /><small>{{ userRole }}</small>
              </h6>
            </li>
            <li><hr class="dropdown-divider" /></li>
            <li>
              <a class="dropdown-item" href="#" @click="viewProfile"
                ><i class="fas fa-user me-2"></i>My Profile</a
              >
            </li>
            <li>
              <a class="dropdown-item" href="#" @click="viewSettings"
                ><i class="fas fa-cog me-2"></i>Settings</a
              >
            </li>
            <li>
              <a class="dropdown-item" href="#" @click="viewHelp"
                ><i class="fas fa-question-circle me-2"></i>Help</a
              >
            </li>
            <li><hr class="dropdown-divider" /></li>
            <li>
              <a class="dropdown-item" href="#" @click="logout"
                ><i class="fas fa-sign-out-alt me-2"></i>Sign Out</a
              >
            </li>
          </ul>
        </div>
      </div>
    </div>
  </header>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useAuthStore } from "@/stores/auth";

const route = useRoute();
const router = useRouter();
const authStore = useAuthStore();

// Computed properties
const userFullName = computed(() => authStore.userFullName);
const userRole = computed(() => authStore.userRole);
const userInitials = computed(() => {
  const user = authStore.user;
  if (!user) return "?";
  return `${user.firstName.charAt(0)}${user.lastName.charAt(0)}`.toUpperCase();
});

const currentPageTitle = computed(() => {
  return (route.meta?.title as string) || "Faculty Dashboard";
});

// Methods
const toggleSidebar = () => {
  // Emit event to parent or use a global state
  const sidebar = document.querySelector(".faculty-sidebar");
  if (sidebar) {
    sidebar.classList.toggle("show");
  }
};

const viewProfile = () => {
  router.push("/profile");
};

const viewSettings = () => {
  router.push("/settings");
};

const viewHelp = () => {
  router.push("/help");
};

const logout = async () => {
  try {
    await authStore.logout();
    await router.push("/login");
  } catch (error) {
    console.error("Logout failed:", error);
  }
};
</script>

<style lang="scss" scoped>
.faculty-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 1.5rem;

  .header-left {
    display: flex;
    align-items: center;
    gap: 1rem;

    .header-title {
      font-size: 1.25rem;
      font-weight: 600;
      color: white;
    }
  }

  .header-right {
    .user-avatar {
      width: 36px;
      height: 36px;
      border-radius: 50%;
      background: rgba(255, 255, 255, 0.2);
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 0.9rem;
      font-weight: bold;
    }
  }
}

.dropdown-menu {
  min-width: 200px;

  .dropdown-header {
    font-size: 0.9rem;
    line-height: 1.2;
  }
}
</style>
