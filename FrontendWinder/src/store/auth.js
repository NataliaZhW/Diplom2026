import { defineStore } from 'pinia'
import { authApi } from '../api'

export const useAuthStore = defineStore('auth', {
    state: () => ({
        user: JSON.parse(localStorage.getItem('user') || 'null'),
        token: localStorage.getItem('token') || null,
        isAuthenticated: !!localStorage.getItem('token')
    }),

    getters: {
        isMaster: (state) => state.user?.role === 'master'
    },

    actions: {
        async login(login, password) {
            try {
                const response = await authApi.login(login, password)
                const data = response.data

                this.user = {
                    id: data.userId,
                    login: data.login,
                    fullName: data.fullName,
                    role: data.role
                }
                this.token = data.token
                this.isAuthenticated = true

                // ✅ Сохраняем ВСЁ в localStorage
                localStorage.setItem('token', data.token)
                localStorage.setItem('userRole', data.role)
                localStorage.setItem('userName', data.fullName)
                localStorage.setItem('userId', data.userId.toString())
                localStorage.setItem('user', JSON.stringify(this.user))  // ← ДОБАВИТЬ!

                return { success: true }
            } catch (error) {
                console.error('ОШИБКА:', error)
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
            localStorage.removeItem('userId')
            localStorage.removeItem('user')  // ← ДОБАВИТЬ!
        },

        // ✅ ДОБАВИТЬ МЕТОД ДЛЯ ВОССТАНОВЛЕНИЯ ПОЛЬЗОВАТЕЛЯ
        restoreUser() {
            const userData = localStorage.getItem('user')
            if (userData) {
                try {
                    this.user = JSON.parse(userData)
                    this.isAuthenticated = true
                } catch {
                    this.user = null
                    this.isAuthenticated = false
                }
            }
        }
    }
})