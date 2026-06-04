import api from './axios';

export const colorThreadsApi = {
  getAll() {
    return api.get('/ColorThreads');
  },
  
  getById(id) {
    return api.get(`/ColorThreads/${id}`);
  },
  
  create(data) {
    return api.post('/ColorThreads', data);
  },
  
  update(id, data) {
    return api.put(`/ColorThreads/${id}`, data);
  },
  
  delete(id) {
    return api.delete(`/ColorThreads/${id}`);
  },
};