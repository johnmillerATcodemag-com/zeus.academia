<template>
  <div style="background: red; color: white; padding: 20px; margin: 20px">
    <h1>DEBUG: App is working!</h1>
    <p>If you can see this, Vue is loading correctly.</p>
    <p>Current route: {{ $route.path }}</p>
    <p>Timestamp: {{ new Date().toISOString() }}</p>
  </div>
</template>

<script setup lang="ts">
console.log("DEBUG: Debug component loaded!");
console.log("Current route:", window.location.pathname);
console.log("Vue app mounted successfully");
</script>
