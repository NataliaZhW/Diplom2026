import api from './api';

export const referenceService = {
  // Получить бренды
  getBrands() {
    return api.get('/Reference/brands');
  },
  
  // Получить цвета
  getColors() {
    return api.get('/Reference/colors');
  },
  
  // Получить каунты
  getCountTypes() {
    return api.get('/Reference/count-types');
  },
  
  // Получить комплекты
  getKitsSchemes() {
    return api.get('/Reference/kits-schemes');
  },
  
  // Получить детали комплекта
  getKitSchemeDetails(code) {
    return api.get(`/Reference/kits-schemes/${code}`);
  }
};