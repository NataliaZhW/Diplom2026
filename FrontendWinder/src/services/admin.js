import api from './api';

export const adminService = {
  // Получить всех пользователей
  getUsers() {
    return api.get('/Admin/users');
  },
  
  // Обновить роль пользователя
  updateUserRole(userId, role, add) {
    return api.post('/Admin/users/role', { userId, role, add });
  },
  
  // Получить все задания
  getAllTasks() {
    return api.get('/Admin/tasks');
  },
  
  // Получить задания пользователя
  getUserTasks(userId) {
    return api.get(`/Admin/users/${userId}/tasks`);
  },
  
  // Обновить задание
  updateTask(taskId, quantity, notes) {
    return api.put(`/Admin/tasks/${taskId}`, { quantity, notes });
  },
  
  // Удалить задание
  deleteTask(taskId) {
    return api.delete(`/Admin/tasks/${taskId}`);
  }
};