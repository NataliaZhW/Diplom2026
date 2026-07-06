import { authApi } from '../api'

export const authService = {
    async login(login, password) {
        try {
            const response = await authApi.login(login, password)
            const data = response.data

            return {
                success: true,
                data: {
                    userId: data.userId,
                    login: data.login,
                    fullName: data.fullName,
                    role: data.role,
                    token: data.token
                }
            }
        } catch (error) {
            return {
                success: false,
                message: error.response?.data?.message || 'Ошибка при входе'
            }
        }
    },

    saveUserData(userData) {
        localStorage.setItem('token', userData.token)
        localStorage.setItem('userRole', userData.role)
        localStorage.setItem('userName', userData.fullName)
        localStorage.setItem('userId', userData.userId.toString())
        localStorage.setItem('user', JSON.stringify(userData))
    },

    clearUserData() {
        localStorage.removeItem('token')
        localStorage.removeItem('userRole')
        localStorage.removeItem('userName')
        localStorage.removeItem('userId')
        localStorage.removeItem('user')
    },

    restoreUser() {
        try {
            const userData = localStorage.getItem('user')
            if (userData) {
                return JSON.parse(userData)
            }
        } catch {
            return null
        }
        return null
    }
}