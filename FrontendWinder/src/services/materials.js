import api from './api';

export const materialsService = {
  // Получить мои запросы
  getMyRequests() {
    return api.get('/Materials/requests/my');
  },
  
  // Получить запрос по ID
  getRequest(id) {
    return api.get(`/Materials/requests/${id}`);
  },
  
  // Создать запрос
  createRequest(taskIds, materials, notes) {
    return api.post('/Materials/requests', { taskIds, materials, notes });
  },
  
  // Подтвердить получение
  receiveMaterials(requestId) {
    return api.post('/Materials/requests/receive', { requestId });
  }
};