<template>
  <footer class="bg-dark text-light py-4 mt-auto">
    <div class="container">
      <div class="row">
        <div class="col-md-6">
          <h5>Zeus Academia</h5>
          <p class="mb-0">
            Empowering students through excellence in education.
          </p>
        </div>
        <div class="col-md-3">
          <h6>Quick Links</h6>
          <ul class="list-unstyled">
            <li>
              <router-link to="/" class="text-light text-decoration-none"
                >Home</router-link
              >
            </li>
            <li>
              <router-link to="/courses" class="text-light text-decoration-none"
                >Courses</router-link
              >
            </li>
            <li>
              <router-link to="/profile" class="text-light text-decoration-none"
                >Profile</router-link
              >
            </li>
          </ul>
        </div>
        <div class="col-md-3">
          <h6>Support</h6>
          <ul class="list-unstyled">
            <li>
              <a href="#" class="text-light text-decoration-none"
                >Help Center</a
              >
            </li>
            <li>
              <a href="#" class="text-light text-decoration-none">Contact Us</a>
            </li>
            <li>
              <a href="#" class="text-light text-decoration-none"
                >Privacy Policy</a
              >
            </li>
          </ul>
        </div>
      </div>
      <hr class="my-3" />
      <div class="row align-items-center">
        <div class="col-md-6">
          <p class="mb-0">
            &copy; {{ currentYear }} Zeus Academia. All rights reserved.
          </p>
        </div>
        <div class="col-md-6 text-md-end">
          <small class="text-muted"
            >Built with Vue.js, TypeScript & Bootstrap</small
          >
        </div>
      </div>
    </div>
  </footer>
</template>

<script setup lang="ts">
const currentYear = new Date().getFullYear();
</script>

<style scoped>
footer a:hover {
  text-decoration: underline !important;
}
</style>
