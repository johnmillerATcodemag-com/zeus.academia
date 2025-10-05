<template>
  <div class="faculty-layout">
    <FacultySidebar />
    <FacultyHeader />

    <main class="faculty-main">
      <div class="courses-container">
        <h1>My Courses</h1>
        <p>Courses management implementation will go here.</p>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import FacultySidebar from "@/components/FacultySidebar.vue";
import FacultyHeader from "@/components/FacultyHeader.vue";
</script>
