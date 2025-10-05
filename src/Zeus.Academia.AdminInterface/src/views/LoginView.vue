<template>
  <div
    class="login-page min-vh-100 d-flex align-items-center justify-content-center"
  >
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-md-6 col-lg-4">
          <div class="card shadow-lg">
            <div class="card-body p-4">
              <div class="text-center mb-4">
                <h2 class="fw-bold text-zeus-primary">Zeus Academia</h2>
                <p class="text-muted">Administrative Interface</p>
              </div>

              <!-- Login Form -->
              <form @submit.prevent="handleLogin">
                <div class="mb-3">
                  <label for="email" class="form-label">Email Address</label>
                  <input
                    id="email"
                    v-model="loginForm.email"
                    type="email"
                    class="form-control"
                    :class="{ 'is-invalid': errors.email }"
                    placeholder="Enter your email"
                    required
                    :disabled="loading"
                  />
                  <div v-if="errors.email" class="invalid-feedback">
                    {{ errors.email }}
                  </div>
                </div>

                <div class="mb-3">
                  <label for="password" class="form-label">Password</label>
                  <input
                    id="password"
                    v-model="loginForm.password"
                    type="password"
                    class="form-control"
                    :class="{ 'is-invalid': errors.password }"
                    placeholder="Enter your password"
                    required
                    :disabled="loading"
                  />
                  <div v-if="errors.password" class="invalid-feedback">
                    {{ errors.password }}
                  </div>
                </div>

                <!-- MFA Code (if required) -->
                <div v-if="showMfaInput" class="mb-3">
                  <label for="mfaCode" class="form-label">
                    Multi-Factor Authentication Code
                  </label>
                  <input
                    id="mfaCode"
                    v-model="loginForm.mfaCode"
                    type="text"
                    class="form-control"
                    :class="{ 'is-invalid': errors.mfaCode }"
                    placeholder="Enter your MFA code"
                    maxlength="6"
                    :disabled="loading"
                  />
                  <div v-if="errors.mfaCode" class="invalid-feedback">
                    {{ errors.mfaCode }}
                  </div>
                  <small class="form-text text-muted">
                    Enter the 6-digit code from your authenticator app
                  </small>
                </div>

                <!-- Error Display -->
                <div v-if="authError" class="alert alert-danger" role="alert">
                  {{ authError }}
                </div>

                <!-- Submit Button -->
                <div class="d-grid">
                  <button
                    type="submit"
                    class="btn btn-primary btn-lg"
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

              <!-- Additional Options -->
              <div class="text-center mt-3">
                <small class="text-muted">
                  For security purposes, administrative access requires
                  multi-factor authentication.
                </small>
              </div>
            </div>
          </div>

          <!-- Security Notice -->
          <div class="card mt-3">
            <div class="card-body text-center py-3">
              <small class="text-muted">
                <i class="bi bi-shield-lock me-1"></i>
                This is a secure administrative interface. All actions are
                logged and monitored.
              </small>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from "vue";
import { useRouter, useRoute } from "vue-router";
import { useAuthStore } from "@/stores/auth";

const router = useRouter();
const route = useRoute();
const authStore = useAuthStore();

const loading = ref(false);
const authError = ref("");
const showMfaInput = ref(false);

const loginForm = reactive({
  email: "",
  password: "",
  mfaCode: "",
});

const errors = reactive({
  email: "",
  password: "",
  mfaCode: "",
});

const validateForm = (): boolean => {
  // Reset errors
  errors.email = "";
  errors.password = "";
  errors.mfaCode = "";

  let isValid = true;

  // Email validation
  if (!loginForm.email) {
    errors.email = "Email is required";
    isValid = false;
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(loginForm.email)) {
    errors.email = "Please enter a valid email address";
    isValid = false;
  }

  // Password validation
  if (!loginForm.password) {
    errors.password = "Password is required";
    isValid = false;
  } else if (loginForm.password.length < 8) {
    errors.password = "Password must be at least 8 characters";
    isValid = false;
  }

  // MFA validation (if required)
  if (showMfaInput.value) {
    if (!loginForm.mfaCode) {
      errors.mfaCode = "MFA code is required";
      isValid = false;
    } else if (!/^\d{6}$/.test(loginForm.mfaCode)) {
      errors.mfaCode = "MFA code must be 6 digits";
      isValid = false;
    }
  }

  return isValid;
};

const handleLogin = async () => {
  if (!validateForm()) return;

  loading.value = true;
  authError.value = "";

  try {
    const result = await authStore.login(
      loginForm.email,
      loginForm.password,
      loginForm.mfaCode || undefined
    );

    if (result.success) {
      // Redirect to intended page or dashboard
      const redirectPath = (route.query.redirect as string) || "/dashboard";
      await router.push(redirectPath);
    } else {
      // Check if MFA is required
      if (
        result.error?.includes("MFA") ||
        result.error?.includes("multi-factor")
      ) {
        showMfaInput.value = true;
        authError.value = "Please enter your multi-factor authentication code";
      } else {
        authError.value = result.error || "Login failed. Please try again.";
      }
    }
  } catch (error: any) {
    console.error("Login error:", error);
    authError.value = "An unexpected error occurred. Please try again.";
  } finally {
    loading.value = false;
  }
};

// Check for redirect message
onMounted(() => {
  if (route.query.message) {
    authError.value = route.query.message as string;
  }
});
</script>

<style lang="scss" scoped>
.login-page {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  position: relative;

  &::before {
    content: "";
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100"><defs><pattern id="grain" width="100" height="100" patternUnits="userSpaceOnUse"><circle cx="25" cy="25" r="1" fill="rgba(255,255,255,0.1)"/><circle cx="75" cy="75" r="1" fill="rgba(255,255,255,0.1)"/><circle cx="50" cy="10" r="0.5" fill="rgba(255,255,255,0.05)"/><circle cx="10" cy="50" r="0.5" fill="rgba(255,255,255,0.05)"/><circle cx="90" cy="30" r="0.5" fill="rgba(255,255,255,0.05)"/></pattern></defs><rect width="100" height="100" fill="url(%23grain)"/></svg>');
    opacity: 0.3;
  }

  .container {
    position: relative;
    z-index: 1;
  }
}

.card {
  border: none;
  border-radius: 1rem;
  backdrop-filter: blur(10px);
  background: rgba(255, 255, 255, 0.95);
}

.text-zeus-primary {
  color: var(--zeus-primary);
}

.form-control {
  border-radius: 0.5rem;
  border: 2px solid #e9ecef;
  padding: 0.75rem 1rem;

  &:focus {
    border-color: var(--zeus-primary);
    box-shadow: 0 0 0 0.2rem rgba(44, 62, 80, 0.25);
  }

  &.is-invalid {
    border-color: var(--zeus-danger);
  }
}

.btn-primary {
  border-radius: 0.5rem;
  padding: 0.75rem 1.5rem;
  font-weight: 600;
  letter-spacing: 0.025em;
  transition: all 0.3s ease;

  &:hover:not(:disabled) {
    transform: translateY(-2px);
    box-shadow: 0 5px 15px rgba(44, 62, 80, 0.4);
  }

  &:disabled {
    opacity: 0.7;
  }
}

.alert {
  border-radius: 0.5rem;
  border: none;
}

.spinner-border-sm {
  width: 1rem;
  height: 1rem;
}

// Animation for MFA input
.fade-enter-active,
.fade-leave-active {
  transition: all 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}
</style>
