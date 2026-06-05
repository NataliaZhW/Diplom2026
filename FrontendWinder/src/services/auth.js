import api from './api';

export const authService = {
  // Регистрация
  register(email, password, fullName) {
    return api.post('/Auth/register', { email, password, fullName });
  },
  
  // Вход
  login(email, password) {
    return api.post('/Auth/login', { email, password });
  },
  
  // Получение профиля
  getProfile() {
    return api.get('/Auth/profile');
  },
  
  // Смена пароля
  changePassword(currentPassword, newPassword) {
    return api.post('/Auth/change-password', { currentPassword, newPassword });
  },
  
  // Сохранение токена и данных пользователя
  setAuthData(token, user) {
    localStorage.setItem('token', token);
    localStorage.setItem('user', JSON.stringify(user));
  },
  
  // Очистка данных при выходе
  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  },
  
  // Получение текущего пользователя
  getCurrentUser() {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  },
  
  // Проверка, авторизован ли пользователь
  isAuthenticated() {
    return !!localStorage.getItem('token');
  },
  
  // Проверка, является ли пользователь админом
  isAdmin() {
    const user = this.getCurrentUser();
    return user?.roles?.includes('Admin') || false;
  }
};