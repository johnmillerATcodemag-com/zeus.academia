<template>
  <div class="login-page min-vh-100 d-flex align-items-center">
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-md-6 col-lg-4">
          <div class="card shadow">
            <div class="card-body p-5">
              <div class="text-center mb-4">
                <h2 class="zeus-primary">Student Login</h2>
                <p class="text-muted">Access your Zeus Academia account</p>
              </div>

              <form @submit.prevent="handleLogin">
                <div class="mb-3">
                  <label for="email" class="form-label">Email Address</label>
                  <input
                    id="email"
                    v-model="form.email"
                    type="email"
                    class="form-control"
                    :class="{ 'is-invalid': errors.email }"
                    placeholder="Enter your email"
                    autocomplete="username"
                    required
                  />
                  <div v-if="errors.email" class="invalid-feedback">
                    {{ errors.email }}
                  </div>
                </div>

                <div class="mb-3">
                  <label for="password" class="form-label">Password</label>
                  <input
                    id="password"
                    v-model="form.password"
                    type="password"
                    class="form-control"
                    :class="{ 'is-invalid': errors.password }"
                    placeholder="Enter your password"
                    autocomplete="current-password"
                    required
                  />
                  <div v-if="errors.password" class="invalid-feedback">
                    {{ errors.password }}
                  </div>
                </div>

                <div class="mb-3 form-check">
                  <input
                    id="remember"
                    v-model="form.remember"
                    type="checkbox"
                    class="form-check-input"
                  />
                  <label class="form-check-label" for="remember">
                    Remember me
                  </label>
                </div>

                <div class="d-grid">
                  <button
                    type="submit"
                    class="btn btn-primary btn-lg"
                    :disabled="isLoading"
                  >
                    <span
                      v-if="isLoading"
                      class="spinner-border spinner-border-sm me-2"
                      role="status"
                      aria-hidden="true"
                    ></span>
                    {{ isLoading ? "Signing In..." : "Sign In" }}
                  </button>
                </div>
              </form>

              <div v-if="errorMessage" class="alert alert-danger mt-3">
                {{ errorMessage }}
              </div>

              <div class="text-center mt-4">
                <small class="text-muted">
                  <a href="#" class="text-decoration-none"
                    >Forgot your password?</a
                  >
                </small>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from "vue";
import { useRouter } from "vue-router";

const router = useRouter();

const form = reactive({
  email: "",
  password: "",
  remember: false,
});

const errors = reactive({
  email: "",
  password: "",
});

const isLoading = ref(false);
const errorMessage = ref("");

const validateForm = (): boolean => {
  errors.email = "";
  errors.password = "";

  if (!form.email) {
    errors.email = "Email is required";
    return false;
  }

  if (!form.email.includes("@")) {
    errors.email = "Please enter a valid email address";
    return false;
  }

  if (!form.password) {
    errors.password = "Password is required";
    return false;
  }

  if (form.password.length < 6) {
    errors.password = "Password must be at least 6 characters";
    return false;
  }

  return true;
};

const handleLogin = async () => {
  errorMessage.value = "";

  if (!validateForm()) {
    return;
  }

  isLoading.value = true;

  try {
    // Simulate login with test credentials
    // For demo purposes: email: student@zeus.edu, password: password123
    if (form.email === "student@zeus.edu" && form.password === "password123") {
      // Store simple auth state in localStorage for demo
      localStorage.setItem("zeus_auth", "true");
      localStorage.setItem(
        "zeus_user",
        JSON.stringify({
          name: "Test Student",
          email: form.email,
          id: "1",
        })
      );

      router.push("/dashboard");
    } else {
      errorMessage.value =
        "Invalid credentials. Use student@zeus.edu / password123";
    }
  } catch (error) {
    errorMessage.value = "An unexpected error occurred. Please try again.";
  } finally {
    isLoading.value = false;
  }
};
</script>

<style scoped>
.login-page {
  background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
}

.card {
  border: none;
  border-radius: 1rem;
}

.btn-primary {
  background: linear-gradient(
    135deg,
    var(--zeus-primary) 0%,
    var(--zeus-secondary) 100%
  );
  border: none;
}

.btn-primary:hover {
  background: linear-gradient(
    135deg,
    var(--zeus-secondary) 0%,
    var(--zeus-primary) 100%
  );
}
</style>
