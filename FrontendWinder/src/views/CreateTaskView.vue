<template>
  <div class="create-task">
    <h2>Создать задание</h2>
    
    <form @submit.prevent="handleSubmit">
      <div class="form-group">
        <label>Тип задания *</label>
        <select v-model="form.taskType" required>
          <option value="">Выберите тип</option>
          <option value="Scheme">Схема</option>
          <option value="Kit">Набор</option>
          <option value="Thread">Нить</option>
        </select>
      </div>
      
      <div v-if="form.taskType === 'Scheme'">
        <div class="form-group">
          <label>Код комплекта *</label>
          <input v-model="form.kitSchemeCode" placeholder="0048" required />
        </div>
        <div class="form-group">
          <label>Код каунта</label>
          <input v-model="form.countCode" placeholder="283" />
        </div>
      </div>
      
      <div v-if="form.taskType === 'Kit'">
        <div class="form-group">
          <label>Код комплекта *</label>
          <input v-model="form.kitSchemeCode" placeholder="0048" required />
        </div>
      </div>
      
      <div v-if="form.taskType === 'Thread'">
        <div class="form-group">
          <label>Код бренда *</label>
          <input v-model="form.brandCode" placeholder="2 (ПНК) или 3 (DMC)" required />
        </div>
        <div class="form-group">
          <label>Код цвета *</label>
          <input v-model="form.colorCode" placeholder="100" required />
        </div>
      </div>
      
      <div class="form-group">
        <label>Количество *</label>
        <input type="number" v-model="form.quantity" required min="1" />
      </div>
      
      <div class="form-group">
        <label>Примечание</label>
        <textarea v-model="form.notes" placeholder="магазин, оптовик, выставка..."></textarea>
      </div>
      
      <button type="submit" :disabled="loading">Создать</button>
      <div v-if="error" class="error">{{ error }}</div>
      <div v-if="success" class="success">Задание успешно создано!</div>
    </form>
  </div>
</template>

<script>
import { useTasksStore } from '@/stores/tasks';

export default {
  name: 'CreateTaskView',
  data() {
    return {
      form: {
        taskType: '',
        kitSchemeCode: '',
        brandCode: '',
        colorCode: '',
        countCode: null,
        quantity: 1,
        notes: ''
      },
      loading: false,
      error: null,
      success: false
    };
  },
  methods: {
    async handleSubmit() {
      this.loading = true;
      this.error = null;
      this.success = false;
      
      try {
        const tasksStore = useTasksStore();
        await tasksStore.createTask(this.form);
        this.success = true;
        this.form = {
          taskType: '',
          kitSchemeCode: '',
          brandCode: '',
          colorCode: '',
          countCode: null,
          quantity: 1,
          notes: ''
        };
        setTimeout(() => {
          this.$emit('task-created');
        }, 1500);
      } catch (error) {
        this.error = error.response?.data?.message || 'Ошибка создания задания';
      } finally {
        this.loading = false;
      }
    }
  }
};
</script>

<style scoped>
.create-task {
  max-width: 500px;
  margin: 0 auto;
  padding: 20px;
  background: white;
  border-radius: 8px;
}

.form-group {
  margin-bottom: 15px;
}

label {
  display: block;
  margin-bottom: 5px;
  font-weight: bold;
}

input, select, textarea {
  width: 100%;
  padding: 8px;
  border: 1px solid #ddd;
  border-radius: 4px;
}

button {
  padding: 10px 20px;
  background-color: #1976d2;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

button:disabled {
  background-color: #ccc;
}

.error {
  color: red;
  margin-top: 10px;
}

.success {
  color: green;
  margin-top: 10px;
}
</style>