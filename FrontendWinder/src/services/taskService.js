import { tasksApi } from '../api'

export const taskService = {
    async getTasks() {
        try {
            const response = await tasksApi.getTasks()
            return { success: true, data: response.data }
        } catch (error) {
            return {
                success: false,
                message: error.response?.data?.message || 'Ошибка загрузки заданий'
            }
        }
    },

    async createBatchTasks(items) {
        try {
            const response = await tasksApi.createBatchTasks(items)
            return { success: true, data: response.data }
        } catch (error) {
            return {
                success: false,
                message: error.response?.data?.message || 'Ошибка создания заданий'
            }
        }
    },

    async updateStatus(id, status) {
        try {
            await tasksApi.updateStatus(id, status)
            return { success: true }
        } catch (error) {
            return {
                success: false,
                message: error.response?.data?.message || 'Ошибка обновления статуса'
            }
        }
    },

    async deleteTask(id) {
        try {
            await tasksApi.deleteTask(id)
            return { success: true }
        } catch (error) {
            return {
                success: false,
                message: error.response?.data?.message || 'Ошибка удаления задания'
            }
        }
    }
}