import { defineStore } from 'pinia';
import { tasksService } from '@/services/tasks';

export const useTasksStore = defineStore('tasks', {
  state: () => ({
    tasks: [],
    loading: false,
    error: null
  }),
  
  actions: {
    // Загрузить мои задания
    async loadMyTasks() {
      this.loading = true;
      try {
        const response = await tasksService.getMyTasks();
        this.tasks = response.data;
        this.error = null;
      } catch (error) {
        this.error = error.response?.data?.message || 'Ошибка загрузки заданий';
        console.error(error);
      } finally {
        this.loading = false;
      }
    },
    
    // Создать задание
    async createTask(taskData) {
      try {
        const response = await tasksService.createTask(taskData);
        await this.loadMyTasks();
        return response.data;
      } catch (error) {
        this.error = error.response?.data?.message || 'Ошибка создания задания';
        throw error;
      }
    },
    
    // Изменить статус
    async updateStatus(taskId, newStatus) {
      try {
        await tasksService.updateStatus(taskId, newStatus);
        await this.loadMyTasks();
      } catch (error) {
        this.error = error.response?.data?.message || 'Ошибка изменения статуса';
        throw error;
      }
    },
    
    // Запросить материалы
    async requestMaterials(taskId, notes) {
      try {
        await tasksService.requestMaterials(taskId, notes);
        await this.loadMyTasks();
      } catch (error) {
        this.error = error.response?.data?.message || 'Ошибка запроса материалов';
        throw error;
      }
    }
  }
});