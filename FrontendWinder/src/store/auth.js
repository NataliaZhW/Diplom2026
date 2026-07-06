import { defineStore } from 'pinia'
import { authService } from '../services/authService'
import { ROLES } from '../config/roles'

export const useAuthStore = defineStore('auth', {
    state: () => ({
        user: authService.restoreUser(),
        token: localStorage.getItem('token') || null,
        isAuthenticated: !!localStorage.getItem('token')
    }),

    getters: {
        isMaster: (state) => state.user?.role === ROLES.MASTER
    },

    actions: {
        async login(login, password) {
            const result = await authService.login(login, password)

            if (result.success) {
                this.user = result.data
                this.token = result.data.token
                this.isAuthenticated = true
                authService.saveUserData(result.data)

                console.log('Роль пользователя:', this.user?.role)
                
                return { success: true }
            }

            return {
                success: false,
                message: result.message
            }
        },

        logout() {
            this.user = null
            this.token = null
            this.isAuthenticated = false
            authService.clearUserData()
        },

        restoreUser() {
            const userData = authService.restoreUser()
            if (userData) {
                this.user = userData
                this.isAuthenticated = true
            }
        }
    }
})