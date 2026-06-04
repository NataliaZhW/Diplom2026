import api from './axios';

export const brandManufsApi = {
  getAll() {
    return api.get('/BrandManufs');
  },
  
  getById(id) {
    return api.get(`/BrandManufs/${id}`);
  },
  
  create(data) {
    return api.post('/BrandManufs', data);
  },
  
  update(id, data) {
    return api.put(`/BrandManufs/${id}`, data);
  },
  
  delete(id) {
    return api.delete(`/BrandManufs/${id}`);
  },
};