import { defineStore } from 'pinia';
import { authService } from '@/services/auth';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null,
    token: null,
    isAuthenticated: false
  }),
  
  getters: {
    isAdmin: (state) => state.user?.roles?.includes('Admin') || false,
    fullName: (state) => state.user?.fullName || '',
    email: (state) => state.user?.email || ''
  },
  
  actions: {
    // Инициализация из localStorage
    init() {
      const token = localStorage.getItem('token');
      const user = localStorage.getItem('user');
      
      if (token && user) {
        this.token = token;
        this.user = JSON.parse(user);
        this.isAuthenticated = true;
      }
    },
    
    // Регистрация
    async register(email, password, fullName) {
      const response = await authService.register(email, password, fullName);
      const { token, userId, email: userEmail, fullName: userName, roles } = response.data;
      
      this.token = token;
      this.user = { userId, email: userEmail, fullName: userName, roles };
      this.isAuthenticated = true;
      
      authService.setAuthData(token, this.user);
      return response.data;
    },
    
    // Вход
    async login(email, password) {
      const response = await authService.login(email, password);
      const { token, userId, email: userEmail, fullName: userName, roles } = response.data;
      
      this.token = token;
      this.user = { userId, email: userEmail, fullName: userName, roles };
      this.isAuthenticated = true;
      
      authService.setAuthData(token, this.user);
      return response.data;
    },
    
    // Выход
    logout() {
      this.token = null;
      this.user = null;
      this.isAuthenticated = false;
      authService.logout();
    },
    
    // Обновление профиля
    async updateProfile(profileData) {
      // TODO: обновление профиля
    }
  }
});