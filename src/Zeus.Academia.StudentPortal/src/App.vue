<template>
  <div id="app">
    <NavBar />
    <main>
      <RouterView />
    </main>
    <Footer />
    
    <!-- Global notification toast -->
    <Toast
      v-if="notification"
      :show="notification.show"
      :type="notification.type"
      :title="notification.title"
      :message="notification.message"
      @dismiss="clearNotification"
    />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useStore } from 'vuex'
import NavBar from "./components/NavBar.vue";
import Footer from "./components/Footer.vue";
import Toast from "./components/Toast.vue";

const store = useStore()

const notification = computed(() => store.getters.notification)

const clearNotification = () => {
  store.dispatch('clearNotification')
}

console.log("App.vue loaded successfully");
</script>

<style scoped>
#app {
  font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", "Roboto", "Oxygen",
    "Ubuntu", "Cantarell", "Fira Sans", "Droid Sans", "Helvetica Neue",
    sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}
</style>
