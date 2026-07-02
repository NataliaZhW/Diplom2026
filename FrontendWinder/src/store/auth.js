import { defineStore } from 'pinia'
import { authApi } from '../api'

export const useAuthStore = defineStore('auth', {
    state: () => ({
        user: null,
        token: localStorage.getItem('token') || null,
        isAuthenticated: !!localStorage.getItem('token')
    }),

    getters: {
        isMaster: (state) => state.user?.role === 'master'
    },

    actions: {
        async login(login, password) {
            try {
                console.log('1. Отправка запроса:', { login, password })
                const response = await authApi.login(login, password)
                console.log('2. Ответ:', response)

                const data = response.data
                console.log('3. Данные:', data)

                this.user = {
                    id: data.userId,
                    login: data.login,
                    fullName: data.fullName,
                    role: data.role
                }
                this.token = data.token
                this.isAuthenticated = true

                localStorage.setItem('token', data.token)
                localStorage.setItem('userRole', data.role)
                localStorage.setItem('userName', data.fullName)

                console.log('4. Успешно!')
                return { success: true }
            } catch (error) {
                console.error('ОШИБКА:', error)
                console.error('error.response:', error.response)
                return {
                    success: false,
                    message: error.response?.data?.message || 'Ошибка при входе'
                }
            }
        },

        logout() {
            this.user = null
            this.token = null
            this.isAuthenticated = false

            localStorage.removeItem('token')
            localStorage.removeItem('userRole')
            localStorage.removeItem('userName')
        }
    }
})