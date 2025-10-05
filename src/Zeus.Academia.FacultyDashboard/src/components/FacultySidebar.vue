<template>
  <aside class="faculty-sidebar">
    <div class="sidebar-content">
      <!-- Logo and Brand -->
      <div class="sidebar-brand">
        <div class="brand-logo">🎓</div>
        <div class="brand-text">
          <div class="brand-title">Zeus Academia</div>
          <div class="brand-subtitle">Faculty Portal</div>
        </div>
      </div>

      <!-- Navigation -->
      <nav class="faculty-sidebar-nav">
        <div class="nav-section">
          <div class="section-title">Main</div>
          <ul class="nav-list">
            <li class="nav-item">
              <router-link
                to="/dashboard"
                class="nav-link"
                active-class="active"
              >
                <span class="nav-icon">📊</span>
                <span class="nav-text">Dashboard</span>
              </router-link>
            </li>
            <li class="nav-item">
              <router-link to="/courses" class="nav-link" active-class="active">
                <span class="nav-icon">📚</span>
                <span class="nav-text">My Courses</span>
                <span class="nav-badge">{{ courseCount }}</span>
              </router-link>
            </li>
            <li class="nav-item">
              <router-link
                to="/gradebook"
                class="nav-link"
                active-class="active"
              >
                <span class="nav-icon">📝</span>
                <span class="nav-text">Gradebook</span>
              </router-link>
            </li>
          </ul>
        </div>

        <div class="nav-section">
          <div class="section-title">Teaching</div>
          <ul class="nav-list">
            <li class="nav-item">
              <router-link
                to="/assignments"
                class="nav-link"
                active-class="active"
              >
                <span class="nav-icon">📋</span>
                <span class="nav-text">Assignments</span>
              </router-link>
            </li>
            <li class="nav-item">
              <router-link
                to="/students"
                class="nav-link"
                active-class="active"
              >
                <span class="nav-icon">👥</span>
                <span class="nav-text">Students</span>
              </router-link>
            </li>
            <li class="nav-item">
              <router-link
                to="/analytics"
                class="nav-link"
                active-class="active"
              >
                <span class="nav-icon">📈</span>
                <span class="nav-text">Analytics</span>
              </router-link>
            </li>
          </ul>
        </div>

        <div v-if="hasAdminPermissions" class="nav-section">
          <div class="section-title">Administration</div>
          <ul class="nav-list">
            <li class="nav-item">
              <router-link to="/faculty" class="nav-link" active-class="active">
                <span class="nav-icon">👨‍🏫</span>
                <span class="nav-text">Faculty</span>
              </router-link>
            </li>
            <li class="nav-item">
              <router-link to="/reports" class="nav-link" active-class="active">
                <span class="nav-icon">📊</span>
                <span class="nav-text">Reports</span>
              </router-link>
            </li>
          </ul>
        </div>

        <div class="nav-section">
          <div class="section-title">Tools</div>
          <ul class="nav-list">
            <li class="nav-item">
              <router-link
                to="/calendar"
                class="nav-link"
                active-class="active"
              >
                <span class="nav-icon">📅</span>
                <span class="nav-text">Calendar</span>
              </router-link>
            </li>
            <li class="nav-item">
              <router-link
                to="/messages"
                class="nav-link"
                active-class="active"
              >
                <span class="nav-icon">💬</span>
                <span class="nav-text">Messages</span>
                <span v-if="unreadMessages > 0" class="nav-badge">{{
                  unreadMessages
                }}</span>
              </router-link>
            </li>
            <li class="nav-item">
              <router-link
                to="/resources"
                class="nav-link"
                active-class="active"
              >
                <span class="nav-icon">📁</span>
                <span class="nav-text">Resources</span>
              </router-link>
            </li>
          </ul>
        </div>
      </nav>

      <!-- User Profile Section -->
      <div class="user-profile">
        <div class="user-info">
          <div class="user-avatar">{{ userInitials }}</div>
          <div class="user-details">
            <div class="user-name">{{ userFullName }}</div>
            <div class="user-role">{{ userRole }}</div>
          </div>
          <div class="user-actions">
            <div class="dropdown">
              <button
                class="dropdown-toggle"
                data-bs-toggle="dropdown"
                aria-expanded="false"
              >
                ⋮
              </button>
              <ul class="dropdown-menu">
                <li>
                  <a class="dropdown-item" href="#" @click="viewProfile"
                    >Profile</a
                  >
                </li>
                <li>
                  <a class="dropdown-item" href="#" @click="logout">Sign Out</a>
                </li>
              </ul>
            </div>
          </div>
        </div>
      </div>
    </div>
  </aside>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "@/stores/auth";
import { useGradebookStore } from "@/stores/gradebook";

const router = useRouter();
const authStore = useAuthStore();
const gradebookStore = useGradebookStore();

// Computed properties
const userFullName = computed(() => authStore.userFullName);
const userRole = computed(() => authStore.userRole);
const userInitials = computed(() => {
  const user = authStore.user;
  if (!user) return "?";
  return `${user.firstName.charAt(0)}${user.lastName.charAt(0)}`.toUpperCase();
});

const hasAdminPermissions = computed(() => {
  return authStore.hasAnyRole(["chair", "dean", "admin"]);
});

const courseCount = computed(() => gradebookStore.courses.length);

// Mock data for demonstration
const unreadMessages = computed(() => 3);

// Methods
const viewProfile = () => {
  router.push("/profile");
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
.sidebar-brand {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1.5rem 1rem 1rem;
  border-bottom: 1px solid #e0e0e0;

  .brand-logo {
    width: 40px;
    height: 40px;
    border-radius: 8px;
    background: linear-gradient(
      135deg,
      var(--faculty-primary),
      var(--faculty-secondary)
    );
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 1.5rem;
    flex-shrink: 0;
  }

  .brand-text {
    flex: 1;
    min-width: 0;

    .brand-title {
      font-size: 1.1rem;
      font-weight: 700;
      color: var(--faculty-text-primary);
      line-height: 1.2;
      margin: 0;
    }

    .brand-subtitle {
      font-size: 0.8rem;
      color: var(--faculty-text-secondary);
      margin: 0;
    }
  }
}

.sidebar-content {
  height: 100%;
  display: flex;
  flex-direction: column;
  position: relative;
}

.faculty-sidebar-nav {
  flex: 1;
  overflow-y: auto;
  padding-bottom: 80px; // Space for user profile
}

.user-profile {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
}

// Mobile sidebar overlay
@media (max-width: 768px) {
  .faculty-sidebar {
    &::after {
      content: "";
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background: rgba(0, 0, 0, 0.5);
      z-index: -1;
      opacity: 0;
      visibility: hidden;
      transition: all 0.3s ease;
    }

    &.show::after {
      opacity: 1;
      visibility: visible;
    }
  }
}
</style>
