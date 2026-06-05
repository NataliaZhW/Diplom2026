<template>
  <div class="home">
    <nav class="navbar">
      <h1>Управление заданиями</h1>
      <div class="user-info">
        <span>{{ fullName }}</span>
        <button @click="logout" class="btn-logout">Выйти</button>
      </div>
    </nav>
    
    <div class="tabs">
      <button :class="{ active: activeTab === 'tasks' }" @click="activeTab = 'tasks'">
        Мои задания
      </button>
      <button :class="{ active: activeTab === 'create' }" @click="activeTab = 'create'">
        Создать задание
      </button>
      <button :class="{ active: activeTab === 'materials' }" @click="activeTab = 'materials'">
        Запросы материалов
      </button>
      <button v-if="isAdmin" :class="{ active: activeTab === 'admin' }" @click="activeTab = 'admin'">
        Админ-панель
      </button>
    </div>
    
    <div class="tab-content">
      <TaskList v-if="activeTab === 'tasks'" />
      <CreateTaskView v-if="activeTab === 'create'" @task-created="onTaskCreated" />
      <MaterialsView v-if="activeTab === 'materials'" />
      <AdminView v-if="activeTab === 'admin' && isAdmin" />
    </div>
  </div>
</template>

<script>
import { useAuthStore } from '@/stores/auth';
import { mapState, mapActions } from 'pinia';
import TaskList from '@/components/TaskList.vue';
import CreateTaskView from './CreateTaskView.vue';
import MaterialsView from './MaterialsView.vue';
import AdminView from './AdminView.vue';

export default {
  name: 'HomeView',
  components: {
    TaskList,
    CreateTaskView,
    MaterialsView,
    AdminView
  },
  data() {
    return {
      activeTab: 'tasks'
    };
  },
  computed: {
    ...mapState(useAuthStore, ['user', 'isAdmin']),
    fullName() {
      return this.user?.fullName || this.user?.email || 'Пользователь';
    }
  },
  methods: {
    ...mapActions(useAuthStore, ['logout']),
    onTaskCreated() {
      this.activeTab = 'tasks';
    }
  }
};
</script>

<style scoped>
.home {
  min-height: 100vh;
  background-color: #f5f5f5;
}

.navbar {
  background-color: #1976d2;
  color: white;
  padding: 15px 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 15px;
}

.btn-logout {
  padding: 5px 15px;
  background-color: rgba(255,255,255,0.2);
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.btn-logout:hover {
  background-color: rgba(255,255,255,0.3);
}

.tabs {
  background-color: white;
  padding: 0 20px;
  display: flex;
  gap: 10px;
  border-bottom: 1px solid #ddd;
}

.tabs button {
  padding: 12px 20px;
  background: none;
  border: none;
  cursor: pointer;
  font-size: 14px;
  border-bottom: 2px solid transparent;
}

.tabs button.active {
  border-bottom-color: #1976d2;
  color: #1976d2;
}

.tab-content {
  padding: 20px;
}
</style>