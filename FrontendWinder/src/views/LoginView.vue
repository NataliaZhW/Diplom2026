<template>
  <div class="login-container">
    <div class="login-card">
      <h1>Вход в систему</h1>
      
      <div v-if="error" class="error">{{ error }}</div>
      
      <form @submit.prevent="handleLogin">
        <div class="form-group">
          <label>Email</label>
          <input type="email" v-model="email" required class="form-control" />
        </div>
        
        <div class="form-group">
          <label>Пароль</label>
          <input type="password" v-model="password" required class="form-control" />
        </div>
        
        <button type="submit" class="btn-primary" :disabled="loading">
          {{ loading ? 'Вход...' : 'Войти' }}
        </button>
      </form>
      
      <p class="register-link">
        Нет аккаунта? <router-link to="/register">Зарегистрироваться</router-link>
      </p>
    </div>
  </div>
</template>

<script>
import { useAuthStore } from '@/stores/auth';

export default {
  name: 'LoginView',
  data() {
    return {
      email: '',
      password: '',
      loading: false,
      error: null
    };
  },
  methods: {
    async handleLogin() {
      this.loading = true;
      this.error = null;
      
      try {
        const authStore = useAuthStore();
        await authStore.login(this.email, this.password);
        this.$router.push('/');
      } catch (error) {
        this.error = error.response?.data?.message || 'Ошибка входа';
      } finally {
        this.loading = false;
      }
    }
  }
};
</script>

<style scoped>
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background-color: #f5f5f5;
}

.login-card {
  background: white;
  padding: 30px;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  width: 100%;
  max-width: 400px;
}

.form-group {
  margin-bottom: 15px;
}

.form-control {
  width: 100%;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
}

.btn-primary {
  width: 100%;
  padding: 12px;
  background-color: #1976d2;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 16px;
}

.btn-primary:hover {
  background-color: #1565c0;
}

.btn-primary:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}

.error {
  background-color: #ffebee;
  color: #c62828;
  padding: 10px;
  border-radius: 4px;
  margin-bottom: 15px;
}

.register-link {
  text-align: center;
  margin-top: 15px;
}
</style>