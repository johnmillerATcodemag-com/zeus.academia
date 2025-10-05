<template>
  <div class="faculty-layout">
    <FacultySidebar />
    <FacultyHeader />

    <main class="faculty-main">
      <div class="gradebook-container">
        <h1>Gradebook</h1>
        <p>Gradebook implementation will go here.</p>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import FacultySidebar from "@/components/FacultySidebar.vue";
import FacultyHeader from "@/components/FacultyHeader.vue";
</script>
