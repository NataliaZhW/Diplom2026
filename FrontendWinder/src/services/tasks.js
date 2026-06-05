import api from './api';

export const tasksService = {
  // Получить мои задания
  getMyTasks() {
    return api.get('/Tasks/my');
  },
  
  // Получить задание по ID
  getTask(id) {
    return api.get(`/Tasks/${id}`);
  },
  
  // Создать задание
  createTask(taskData) {
    return api.post('/Tasks', taskData);
  },
  
  // Изменить статус
  updateStatus(taskId, newStatus) {
    return api.put(`/Tasks/${taskId}/status`, { taskId, newStatus });
  },
  
  // Запросить материалы
  requestMaterials(taskId, notes) {
    return api.post(`/Tasks/${taskId}/request-materials`, notes);
  }
};