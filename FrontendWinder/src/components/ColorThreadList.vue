<template>
  <div>
    <h2>Бренды и производители</h2>
    <button @click="showAddForm = true">Добавить</button>
    
    <table border="1">
      <thead>
        <tr>
          <th>ID</th>
          <th>Код</th>
          <th>Название</th>
          <th>Действия</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="item in items" :key="item.id">
          <td>{{ item.id }}</td>
          <td>{{ item.code }}</td>
          <td>{{ item.name }}</td>
          <td>
            <button @click="editItem(item)">Редактировать</button>
            <button @click="deleteItem(item.id)">Удалить</button>
          </td>
        </tr>
      </tbody>
    </table>

    <!-- Форма добавления/редактирования -->
    <div v-if="showAddForm || editingItem">
      <h3>{{ editingItem ? 'Редактировать' : 'Добавить' }}</h3>
      <form @submit.prevent="saveItem">
        <div>
          <label>Код:</label>
          <input v-model="form.code" required />
        </div>
        <div>
          <label>Название:</label>
          <input v-model="form.name" required />
        </div>
        <button type="submit">Сохранить</button>
        <button type="button" @click="cancelForm">Отмена</button>
      </form>
    </div>
  </div>
</template>

<script>
import { colorThreadsApi } from '../api/colorThreads';

export default {
  name: 'ColorThreadList',
  data() {
    return {
      items: [],
      showAddForm: false,
      editingItem: null,
      form: {
        code: '',
        name: '',
      },
    };
  },
  mounted() {
    this.loadItems();
  },
  methods: {
    async loadItems() {
      try {
        const response = await colorThreadsApi.getAll();
        this.items = response.data;
      } catch (error) {
        console.error('Ошибка загрузки:', error);
      }
    },
    editItem(item) {
      this.editingItem = item;
      this.form = { code: item.code, name: item.name };
      this.showAddForm = true;
    },
    async saveItem() {
      try {
        if (this.editingItem) {
          await colorThreadsApi.update(this.editingItem.id, {
            id: this.editingItem.id,
            ...this.form,
          });
        } else {
          await colorThreadsApi.create(this.form);
        }
        this.cancelForm();
        await this.loadItems();
      } catch (error) {
        console.error('Ошибка сохранения:', error);
      }
    },
    async deleteItem(id) {
      if (confirm('Удалить?')) {
        try {
          await colorThreadsApi.delete(id);
          await this.loadItems();
        } catch (error) {
          console.error('Ошибка удаления:', error);
        }
      }
    },
    cancelForm() {
      this.showAddForm = false;
      this.editingItem = null;
      this.form = { code: '', name: '' };
    },
  },
};
</script>