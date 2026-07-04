import axios from 'axios'

const API_BASE_URL = 'http://localhost:5217/api'

const api = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json'
    }
})

// Перехватчик для добавления токена
api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token')
        if (token) {
            config.headers.Authorization = `Bearer ${token}`
        }
        return config
    },
    (error) => Promise.reject(error)
)

api.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response?.status === 401) {
            localStorage.removeItem('token')
            localStorage.removeItem('userRole')
            window.location.href = '/login'
        }
        return Promise.reject(error)
    }
)

export default api

// ============================================================
// API методы
// ============================================================

export const authApi = {
    login: (login, password) => api.post('/Auth/login', { login, password })
}

export const referenceApi = {
    getColors: () => api.get('/Reference/colors'),
    getBrands: () => api.get('/Reference/brands'),
    getUsers: () => api.get('/Reference/users')
}

// ============================================================
// API методы для каталога
// ============================================================

export const catalogApi = {
    // Получить все значки
    getIcons: () => api.get('/Catalog/icons'),

    // Получить все наборы/схемы (с фильтрацией)
    getKits: (type = null, search = null) => {
        let url = '/Catalog/kits'
        const params = new URLSearchParams()
        if (type) params.append('type', type)
        if (search) params.append('search', search)
        if (params.toString()) url += '?' + params.toString()
        return api.get(url)
    },

    // Получить Kit по ID
    getKitById: (id) => api.get(`/Catalog/kits/${id}`),

    // Получить все нити
    getThreads: (brand = null, search = null) => {
        let url = '/Catalog/threads'
        const params = new URLSearchParams()
        if (brand) params.append('brand', brand)
        if (search) params.append('search', search)
        if (params.toString()) url += '?' + params.toString()
        return api.get(url)
    }
}

// ============================================================
// API методы для заданий (ДОБАВИТЬ!)
// ============================================================

export const tasksApi = {
    // Получить все задания
    getTasks: () => api.get('/Tasks'),
    
    // Создать одно задание
    createTask: (data) => api.post('/Tasks', data),
    
    // Массовое создание заданий из списка "Выбрано"
    createBatchTasks: (items) => api.post('/Tasks/batch', items),
    
    // Обновить статус задания
    updateStatus: (id, status) => api.put(`/Tasks/${id}/status`, { status }),
    
    // Удалить задание
    deleteTask: (id) => api.delete(`/Tasks/${id}`)
}