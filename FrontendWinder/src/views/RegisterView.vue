<template>
  <div class="register-container">
    <div class="register-card">
      <h1>Регистрация</h1>
      
      <div v-if="error" class="error">{{ error }}</div>
      
      <form @submit.prevent="handleRegister">
        <div class="form-group">
          <label>ФИО</label>
          <input type="text" v-model="fullName" required class="form-control" />
        </div>
        
        <div class="form-group">
          <label>Email</label>
          <input type="email" v-model="email" required class="form-control" />
        </div>
        
        <div class="form-group">
          <label>Пароль</label>
          <input type="password" v-model="password" required class="form-control" />
          <small>Минимум 6 символов, хотя бы одна заглавная буква, одна цифра и один спецсимвол</small>
        </div>
        
        <button type="submit" class="btn-primary" :disabled="loading">
          {{ loading ? 'Регистрация...' : 'Зарегистрироваться' }}
        </button>
      </form>
      
      <p class="login-link">
        Уже есть аккаунт? <router-link to="/login">Войти</router-link>
      </p>
    </div>
  </div>
</template>

<script>
import { useAuthStore } from '@/stores/auth';

export default {
  name: 'RegisterView',
  data() {
    return {
      fullName: '',
      email: '',
      password: '',
      loading: false,
      error: null
    };
  },
  methods: {
    async handleRegister() {
      this.loading = true;
      this.error = null;
      
      try {
        const authStore = useAuthStore();
        await authStore.register(this.email, this.password, this.fullName);
        this.$router.push('/');
      } catch (error) {
        const errors = error.response?.data?.errors;
        if (errors && errors.length) {
          this.error = errors.join('. ');
        } else {
          this.error = error.response?.data?.message || 'Ошибка регистрации';
        }
      } finally {
        this.loading = false;
      }
    }
  }
};
</script>

<style scoped>
.register-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background-color: #f5f5f5;
}

.register-card {
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

.error {
  background-color: #ffebee;
  color: #c62828;
  padding: 10px;
  border-radius: 4px;
  margin-bottom: 15px;
}

.login-link {
  text-align: center;
  margin-top: 15px;
}

small {
  color: #666;
  font-size: 11px;
}
</style>