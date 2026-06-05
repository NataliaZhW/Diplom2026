<template>
  <router-view />
</template>

<script>
import { useAuthStore } from '@/stores/auth';

export default {
  name: 'App',
  mounted() {
    const authStore = useAuthStore();
    authStore.init();
  }
};
</script>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}
</style>