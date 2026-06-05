<template>
  <div class="materials">
    <h2>Запросы материалов</h2>
    
    <div v-if="loading">Загрузка...</div>
    <div v-if="error" class="error">{{ error }}</div>
    
    <div v-if="!loading && requests.length === 0" class="empty">
      У вас пока нет запросов материалов
    </div>
    
    <table v-if="requests.length > 0" class="materials-table">
      <thead>
        <tr>
          <th>ID</th>
          <th>Задания</th>
          <th>Статус</th>
          <th>Дата запроса</th>
          <th>Примечание</th>
          <th>Действия</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="req in requests" :key="req.id">
          <td>{{ req.id }}</td>
          <td>{{ getTaskIds(req.taskIds) }}</td>
          <td>{{ getStatusName(req.status) }}</td>
          <td>{{ formatDate(req.requestedAt) }}</td>
          <td>{{ req.notes || '-' }}</td>
          <td>
            <button 
              v-if="req.status === 'Pending'"
              @click="receiveMaterials(req.id)" 
              class="btn-small"
            >
              Подтвердить получение
            </button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
import { materialsService } from '@/services/materials';

export default {
  name: 'MaterialsView',
  data() {
    return {
      requests: [],
      loading: false,
      error: null
    };
  },
  mounted() {
    this.loadRequests();
  },
  methods: {
    async loadRequests() {
      this.loading = true;
      try {
        const response = await materialsService.getMyRequests();
        this.requests = response.data;
      } catch (error) {
        this.error = error.response?.data?.message || 'Ошибка загрузки';
      } finally {
        this.loading = false;
      }
    },
    
    async receiveMaterials(requestId) {
      try {
        await materialsService.receiveMaterials(requestId);
        await this.loadRequests();
        alert('Получение материалов подтверждено');
      } catch (error) {
        alert('Ошибка: ' + (error.response?.data?.message || 'Неизвестная ошибка'));
      }
    },
    
    getTaskIds(taskIds) {
      try {
        const ids = JSON.parse(taskIds);
        return ids.join(', ');
      } catch {
        return taskIds;
      }
    },
    
    getStatusName(status) {
      const names = {
        'Pending': 'Ожидает',
        'Received': 'Получено',
        'Cancelled': 'Отменено'
      };
      return names[status] || status;
    },
    
    formatDate(date) {
      if (!date) return '-';
      return new Date(date).toLocaleDateString('ru-RU');
    }
  }
};
</script>

<style scoped>
.materials {
  padding: 20px;
}

.materials-table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 15px;
}

.materials-table th,
.materials-table td {
  border: 1px solid #ddd;
  padding: 10px;
  text-align: left;
}

.materials-table th {
  background-color: #f2f2f2;
}

.btn-small {
  padding: 5px 10px;
  background-color: #4caf50;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.error {
  color: red;
  text-align: center;
  padding: 20px;
}

.empty {
  text-align: center;
  padding: 40px;
  color: #666;
}
</style>