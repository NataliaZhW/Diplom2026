<template>
  <div class="task-list">
    <h2>Мои задания</h2>
    
    <div v-if="loading" class="loading">Загрузка...</div>
    
    <div v-if="error" class="error">{{ error }}</div>
    
    <div v-if="!loading && tasks.length === 0" class="empty">
      У вас пока нет заданий
    </div>
    
    <table v-if="tasks.length > 0" class="tasks-table">
      <thead>
        <tr>
          <th>ID</th>
          <th>Тип</th>
          <th>Комплект/Нить</th>
          <th>Количество</th>
          <th>Примечание</th>
          <th>Статус</th>
          <th>Дата выдачи</th>
          <th>Действия</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="task in tasks" :key="task.id">
          <td>{{ task.id }}</td>
          <td>{{ getTaskTypeName(task.taskType) }}</td>
          <td>{{ getTaskName(task) }}</td>
          <td>{{ task.quantity }}</td>
          <td>{{ task.notes || '-' }}</td>
          <td>
            <span :class="'status status-' + task.status">
              {{ getStatusName(task.status) }}
            </span>
          </td>
          <td>{{ formatDate(task.assignedAt) }}</td>
          <td>
            <select v-model="task.newStatus" @change="changeStatus(task)" class="status-select">
              <option v-for="status in availableStatuses" :key="status" :value="status">
                {{ getStatusName(status) }}
              </option>
            </select>
            <button 
              v-if="task.status === 'New' || task.status === 'MaterialsRequested'"
              @click="requestMaterials(task)" 
              class="btn-small"
            >
              Запросить материалы
            </button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
import { useTasksStore } from '@/stores/tasks';
import { mapState, mapActions } from 'pinia';

export default {
  name: 'TaskList',
  data() {
    return {
      availableStatuses: ['New', 'MaterialsRequested', 'MaterialsReceived', 'InProgress', 'Submitted', 'Reported', 'Archived']
    };
  },
  computed: {
    ...mapState(useTasksStore, ['tasks', 'loading', 'error'])
  },
  mounted() {
    this.loadMyTasks();
  },
  methods: {
    ...mapActions(useTasksStore, ['loadMyTasks', 'updateStatus', 'requestMaterials']),
    
    getTaskTypeName(type) {
      const names = {
        'Scheme': 'Схема',
        'Kit': 'Набор',
        'Thread': 'Нить'
      };
      return names[type] || type;
    },
    
    getTaskName(task) {
      if (task.taskType === 'Scheme') {
        return `${task.kitSchemeName || task.kitSchemeCode} (${task.countCode})`;
      } else if (task.taskType === 'Kit') {
        return task.kitSchemeName || task.kitSchemeCode;
      } else {
        return `${task.brandCode} / ${task.colorCode}`;
      }
    },
    
    getStatusName(status) {
      const names = {
        'New': 'Новое',
        'MaterialsRequested': 'Материалы запрошены',
        'MaterialsReceived': 'Материалы получены',
        'InProgress': 'В работе',
        'Submitted': 'Сдано',
        'Reported': 'В отчетности',
        'Archived': 'В архиве'
      };
      return names[status] || status;
    },
    
    formatDate(date) {
      if (!date) return '-';
      return new Date(date).toLocaleDateString('ru-RU');
    },
    
    async changeStatus(task) {
      if (task.newStatus && task.newStatus !== task.status) {
        await this.updateStatus(task.id, task.newStatus);
        task.status = task.newStatus;
        task.newStatus = '';
      }
    },
    
    async requestMaterials(task) {
      const notes = prompt('Примечание к запросу материалов (необязательно):');
      await this.requestMaterials(task.id, notes);
      alert('Запрос материалов отправлен');
    }
  }
};
</script>

<style scoped>
.task-list {
  padding: 20px;
}

.tasks-table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 15px;
}

.tasks-table th,
.tasks-table td {
  border: 1px solid #ddd;
  padding: 10px;
  text-align: left;
}

.tasks-table th {
  background-color: #f2f2f2;
}

.status {
  padding: 3px 8px;
  border-radius: 4px;
  font-size: 12px;
}

.status-New { background-color: #e3f2fd; color: #1976d2; }
.status-MaterialsRequested { background-color: #fff3e0; color: #f57c00; }
.status-MaterialsReceived { background-color: #e8f5e9; color: #388e3c; }
.status-InProgress { background-color: #e8eaf6; color: #3949ab; }
.status-Submitted { background-color: #fce4ec; color: #d81b60; }
.status-Reported { background-color: #e0f2f1; color: #00695c; }
.status-Archived { background-color: #eeeeee; color: #757575; }

.status-select {
  padding: 5px;
  margin-right: 5px;
}

.btn-small {
  padding: 5px 10px;
  background-color: #4caf50;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.btn-small:hover {
  background-color: #45a049;
}

.loading, .error, .empty {
  text-align: center;
  padding: 40px;
}

.error {
  color: red;
}
</style>