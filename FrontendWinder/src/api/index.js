import axios from 'axios'

const API_BASE_URL = 'http://localhost:5217/api'

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Добавляем лог для отладки
api.interceptors.request.use(
  (config) => {
    console.log('=== ЗАПРОС ===')
    console.log('URL:', config.url)
    console.log('Метод:', config.method)
    console.log('Данные:', config.data)
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    console.error('Ошибка запроса:', error)
    return Promise.reject(error)
  }
)

api.interceptors.response.use(
  (response) => {
    console.log('=== ОТВЕТ ===')
    console.log('Статус:', response.status)
    console.log('Данные:', response.data)
    return response
  },
  (error) => {
    console.error('Ошибка ответа:', error)
    if (error.response?.status === 401) {
      localStorage.removeItem('token')
      localStorage.removeItem('userRole')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

export default api

export const authApi = {
  login: (login, password) => api.post('/Auth/login', { login, password })
}

export const referenceApi = {
  getColors: () => api.get('/Reference/colors'),
  getBrands: () => api.get('/Reference/brands'),
  getUsers: () => api.get('/Reference/users')
}