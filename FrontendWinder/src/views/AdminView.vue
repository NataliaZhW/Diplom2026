<template>
  <div class="admin">
    <h2>Админ-панель</h2>
    
    <div class="admin-tabs">
      <button :class="{ active: activeTab === 'users' }" @click="activeTab = 'users'">
        Пользователи
      </button>
      <button :class="{ active: activeTab === 'all-tasks' }" @click="activeTab = 'all-tasks'">
        Все задания
      </button>
    </div>
    
    <div v-if="activeTab === 'users'">
      <div v-if="loadingUsers">Загрузка...</div>
      <table v-if="users.length > 0" class="admin-table">
        <thead>
          <tr>
            <th>Email</th>
            <th>ФИО</th>
            <th>Роли</th>
            <th>Действия</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="user in users" :key="user.userId">
            <td>{{ user.email }}</td>
            <td>{{ user.fullName }}</td>
            <td>{{ user.roles.join(', ') }}</td>
            <td>
              <button 
                v-if="!user.roles.includes('Admin')"
                @click="makeAdmin(user.userId)" 
                class="btn-small"
              >
                Сделать админом
              </button>
              <button 
                v-else
                @click="removeAdmin(user.userId)" 
                class="btn-small btn-danger"
              >
                Убрать админа
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    
    <div v-if="activeTab === 'all-tasks'">
      <div v-if="loadingTasks">Загрузка...</div>
      <div class="stats">
        <span>Всего: {{ tasksStats.total }}</span>
        <span>Новых: {{ tasksStats.new }}</span>
        <span>В работе: {{ tasksStats.inProgress }}</span>
        <span>Сдано: {{ tasksStats.submitted }}</span>
        <span>В архиве: {{ tasksStats.archived }}</span>
      </div>
      <table v-if="allTasks.length > 0" class="admin-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Пользователь</th>
            <th>Тип</th>
            <th>Название</th>
            <th>Кол-во</th>
            <th>Статус</th>
            <th>Действия</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="task in allTasks" :key="task.id">
            <td>{{ task.id }}</td>
            <td>{{ task.assignedToUserId.slice(0, 8) }}...</td>
            <td>{{ task.taskType }}</td>
            <td>{{ task.kitSchemeName || task.kitSchemeCode || `${task.brandCode}/${task.colorCode}` }}</td>
            <td>{{ task.quantity }}</td>
            <td>{{ task.status }}</td>
            <td>
              <button @click="deleteTask(task.id)" class="btn-small btn-danger">Удалить</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script>
import { adminService } from '@/services/admin';

export default {
  name: 'AdminView',
  data() {
    return {
      activeTab: 'users',
      users: [],
      allTasks: [],
      loadingUsers: false,
      loadingTasks: false
    };
  },
  computed: {
    tasksStats() {
      return {
        total: this.allTasks.length,
        new: this.allTasks.filter(t => t.status === 'New').length,
        inProgress: this.allTasks.filter(t => ['MaterialsRequested', 'MaterialsReceived', 'InProgress'].includes(t.status)).length,
        submitted: this.allTasks.filter(t => ['Submitted', 'Reported'].includes(t.status)).length,
        archived: this.allTasks.filter(t => t.status === 'Archived').length
      };
    }
  },
  mounted() {
    this.loadUsers();
    this.loadAllTasks();
  },
  methods: {
    async loadUsers() {
      this.loadingUsers = true;
      try {
        const response = await adminService.getUsers();
        this.users = response.data;
      } catch (error) {
        console.error(error);
      } finally {
        this.loadingUsers = false;
      }
    },
    
    async loadAllTasks() {
      this.loadingTasks = true;
      try {
        const response = await adminService.getAllTasks();
        this.allTasks = response.data.tasks || [];
      } catch (error) {
        console.error(error);
      } finally {
        this.loadingTasks = false;
      }
    },
    
    async makeAdmin(userId) {
      try {
        await adminService.updateUserRole(userId, 'Admin', true);
        await this.loadUsers();
        alert('Пользователь стал админом');
      } catch (error) {
        alert('Ошибка: ' + (error.response?.data?.message || 'Неизвестная ошибка'));
      }
    },
    
    async removeAdmin(userId) {
      try {
        await adminService.updateUserRole(userId, 'Admin', false);
        await this.loadUsers();
        alert('Права админа удалены');
      } catch (error) {
        alert('Ошибка: ' + (error.response?.data?.message || 'Неизвестная ошибка'));
      }
    },
    
    async deleteTask(taskId) {
      if (confirm('Удалить задание?')) {
        try {
          await adminService.deleteTask(taskId);
          await this.loadAllTasks();
          alert('Задание удалено');
        } catch (error) {
          alert('Ошибка: ' + (error.response?.data?.message || 'Неизвестная ошибка'));
        }
      }
    }
  }
};
</script>

<style scoped>
.admin {
  padding: 20px;
}

.admin-tabs {
  display: flex;
  gap: 10px;
  margin-bottom: 20px;
}

.admin-tabs button {
  padding: 8px 16px;
  background: #e0e0e0;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.admin-tabs button.active {
  background: #1976d2;
  color: white;
}

.admin-table {
  width: 100%;
  border-collapse: collapse;
}

.admin-table th,
.admin-table td {
  border: 1px solid #ddd;
  padding: 8px;
  text-align: left;
}

.admin-table th {
  background-color: #f2f2f2;
}

.stats {
  display: flex;
  gap: 20px;
  margin-bottom: 15px;
  padding: 10px;
  background: #e3f2fd;
  border-radius: 4px;
}

.btn-small {
  padding: 4px 8px;
  background-color: #4caf50;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  margin: 2px;
}

.btn-danger {
  background-color: #f44336;
}
</style>