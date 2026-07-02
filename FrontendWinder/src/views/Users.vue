<template>
    <div class="page-container">
        <div class="page-header">
            <h1>Пользователи</h1>
            <span class="count">Всего: {{ users.length }}</span>
        </div>

        <div v-if="loading" class="loading">Загрузка...</div>
        <div v-else-if="error" class="error">{{ error }}</div>

        <div v-else class="table-container">
            <table>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Логин</th>
                        <th>ФИО</th>
                        <th>Роль</th>
                        <th>Статус</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="user in users" :key="user.id">
                        <td>{{ user.id }}</td>
                        <td class="login">{{ user.login }}</td>
                        <td>{{ user.fullName }}</td>
                        <td>
                            <span :class="['role-badge', user.role]">
                                {{ user.role === 'master' ? 'Мастер' : 'Мотальщик' }}
                            </span>
                        </td>
                        <td>
                            <span :class="['status-badge', user.isActive ? 'active' : 'inactive']">
                                {{ user.isActive ? 'Активен' : 'Заблокирован' }}
                            </span>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { referenceApi } from '../api'

const users = ref([])
const loading = ref(true)
const error = ref(null)

onMounted(async () => {
    try {
        const response = await referenceApi.getUsers()
        users.value = response.data
    } catch (err) {
        error.value = 'Ошибка при загрузке пользователей'
        console.error(err)
    } finally {
        loading.value = false
    }
})
</script>

<style scoped>
.page-container {
    padding: 2rem;
    max-width: 1200px;
    margin: 0 auto;
}

.page-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 1.5rem;
}

.page-header h1 {
    color: #2c3e50;
}

.count {
    background: #9b59b6;
    color: white;
    padding: 0.3rem 0.8rem;
    border-radius: 20px;
    font-size: 0.9rem;
}

.table-container {
    background: white;
    border-radius: 12px;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.08);
    overflow: hidden;
}

table {
    width: 100%;
    border-collapse: collapse;
}

thead {
    background: #f8f9fa;
}

th {
    padding: 0.75rem 1rem;
    text-align: left;
    font-weight: 600;
    color: #2c3e50;
    border-bottom: 2px solid #e0e0e0;
}

td {
    padding: 0.7rem 1rem;
    border-bottom: 1px solid #f0f0f0;
}

tr:hover {
    background: #f8f9fa;
}

.login {
    font-weight: 600;
    color: #2c3e50;
}

.role-badge {
    display: inline-block;
    padding: 0.2rem 0.6rem;
    border-radius: 12px;
    font-size: 0.8rem;
    font-weight: 600;
}

.role-badge.master {
    background: #9b59b6;
    color: white;
}

.role-badge.winder {
    background: #3498db;
    color: white;
}

.status-badge {
    display: inline-block;
    padding: 0.2rem 0.6rem;
    border-radius: 12px;
    font-size: 0.8rem;
    font-weight: 600;
}

.status-badge.active {
    background: #2ecc71;
    color: white;
}

.status-badge.inactive {
    background: #e74c3c;
    color: white;
}

.loading,
.error {
    text-align: center;
    padding: 2rem;
    color: #7f8c8d;
}

.error {
    color: #e74c3c;
}
</style>