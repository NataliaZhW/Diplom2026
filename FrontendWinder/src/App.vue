<template>
  <div id="app">
    <Navbar v-if="authStore.isAuthenticated" />
    <router-view />
  </div>
</template>

<script setup>
import { onMounted } from 'vue'
import { useAuthStore } from './store/auth'
import Navbar from './components/Navbar.vue'

const authStore = useAuthStore()
console.log(authStore.user?.role)

onMounted(() => {
  // ✅ Восстанавливаем пользователя при загрузке приложения
  authStore.restoreUser()
})
</script>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, sans-serif;
  background-color: #f5f5f5;
}

#app {
  min-height: 100vh;
}
</style>