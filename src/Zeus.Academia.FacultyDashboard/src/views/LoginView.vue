<template>
  <div
    class="login-container d-flex align-items-center justify-content-center min-vh-100"
  >
    <div class="login-form">
      <div class="login-header">
        <div class="login-logo">🎓</div>
        <h1 class="login-title">Faculty Portal</h1>
        <p class="login-subtitle">Zeus Academia System</p>
      </div>

      <form @submit.prevent="handleLogin" class="faculty-form">
        <div class="form-group">
          <label for="email" class="form-label required">Email Address</label>
          <input
            id="email"
            v-model="loginForm.email"
            type="email"
            class="form-control"
            :class="{ 'is-invalid': errors.email }"
            placeholder="Enter your university email"
            required
            autocomplete="email"
          />
          <div v-if="errors.email" class="invalid-feedback">
            {{ errors.email }}
          </div>
        </div>

        <div class="form-group">
          <label for="password" class="form-label required">Password</label>
          <input
            id="password"
            v-model="loginForm.password"
            type="password"
            class="form-control"
            :class="{ 'is-invalid': errors.password }"
            placeholder="Enter your password"
            required
            autocomplete="current-password"
          />
          <div v-if="errors.password" class="invalid-feedback">
            {{ errors.password }}
          </div>
        </div>

        <div class="form-group">
          <div class="form-check">
            <input
              id="remember"
              v-model="loginForm.remember"
              class="form-check-input"
              type="checkbox"
            />
            <label class="form-check-label" for="remember"> Remember me </label>
          </div>
        </div>

        <div v-if="authError" class="alert alert-danger" role="alert">
          {{ authError }}
        </div>

        <div class="login-actions">
          <button
            type="submit"
            class="btn btn-primary btn-login w-100"
            :disabled="loading"
          >
            <span
              v-if="loading"
              class="spinner-border spinner-border-sm me-2"
              role="status"
              aria-hidden="true"
            ></span>
            {{ loading ? "Signing In..." : "Sign In" }}
          </button>
        </div>
      </form>

      <div class="login-footer">
        <a href="#" class="forgot-password">Forgot your password?</a>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "@/stores/auth";

const router = useRouter();
const authStore = useAuthStore();

const loading = ref(false);
const authError = ref("");

const loginForm = reactive({
  email: "",
  password: "",
  remember: false,
});

const errors = reactive({
  email: "",
  password: "",
});

const validateForm = (): boolean => {
  errors.email = "";
  errors.password = "";

  let isValid = true;

  if (!loginForm.email) {
    errors.email = "Email is required";
    isValid = false;
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(loginForm.email)) {
    errors.email = "Please enter a valid email address";
    isValid = false;
  }

  if (!loginForm.password) {
    errors.password = "Password is required";
    isValid = false;
  } else if (loginForm.password.length < 6) {
    errors.password = "Password must be at least 6 characters";
    isValid = false;
  }

  return isValid;
};

const handleLogin = async () => {
  if (!validateForm()) return;

  loading.value = true;
  authError.value = "";

  try {
    const result = await authStore.login(loginForm.email, loginForm.password);

    if (result.success) {
      // Remember user preference
      if (loginForm.remember) {
        localStorage.setItem("zeus_faculty_remember", "true");
        localStorage.setItem("zeus_faculty_email", loginForm.email);
      }

      await router.push("/dashboard");
    } else {
      authError.value = result.error || "Login failed. Please try again.";
    }
  } catch (error) {
    authError.value = "An unexpected error occurred. Please try again.";
    console.error("Login error:", error);
  } finally {
    loading.value = false;
  }
};

// Check for remembered email on component mount
const rememberedEmail = localStorage.getItem("zeus_faculty_email");
if (
  rememberedEmail &&
  localStorage.getItem("zeus_faculty_remember") === "true"
) {
  loginForm.email = rememberedEmail;
  loginForm.remember = true;
}
</script>

<style lang="scss" scoped>
.login-container {
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  min-height: 100vh;
}
</style>
