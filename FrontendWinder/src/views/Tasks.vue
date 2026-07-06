<template>
    <div class="tasks-container">
        <h1>📋 Задания</h1>

        <!-- Фильтры -->
        <div class="filters">
            <div class="filter-group">
                <label>Статус:</label>
                <select v-model="filterStatus" class="filter-select">
                    <option value="">Все</option>
                    <option v-for="(label, key) in statusLabels" :key="key" :value="key">
                        {{ label }}
                    </option>
                </select>
            </div>

            <div class="filter-group">
                <label>Тип:</label>
                <select v-model="filterType" class="filter-select">
                    <option value="">Все</option>
                    <option value="kit">Набор</option>
                    <option value="scheme">Схема</option>
                    <option value="thread">Нить</option>
                </select>
            </div>

            <div class="filter-group">
                <label>Мотальщик:</label>
                <select v-model="filterWinder" class="filter-select" v-if="isMaster">
                    <option value="">Все</option>
                    <option v-for="user in winders" :key="user.id" :value="user.id">
                        {{ user.fullName }}
                    </option>
                </select>
                <span v-else class="filter-static">{{ currentUserName }}</span>
            </div>

            <button @click="loadTasks" class="refresh-btn">🔄 Обновить</button>
        </div>

        <!-- Таблица заданий -->
        <div class="table-container">
            <div v-if="loading" class="loading">Загрузка...</div>
            <div v-else-if="error" class="error">{{ error }}</div>
            <div v-else-if="filteredTasks.length === 0" class="empty-state">Нет заданий</div>
            <table v-else class="tasks-table">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Статус</th>
                        <th>Тип</th>
                        <th>Код</th>
                        <th>Название</th>
                        <th>ПНК/DMC</th>
                        <th>Каунт</th>
                        <th>Кол-во</th>
                        <th>Мотальщик</th>
                        <th>Создано</th>
                        <th>Действия</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="task in filteredTasks" :key="task.id">
                        <td>{{ task.id }}</td>
                        <td>
                            <span :class="['status-badge', getStatusClass(task.status)]">
                                {{ task.statusLabel }}
                            </span>
                        </td>
                        <td>{{ task.itemTypeLabel }}</td>
                        <td>{{ task.itemCode }}</td>
                        <td>{{ task.itemName }}</td>
                        <td>{{ task.brandLabel || '-' }}</td>
                        <td>{{ task.countValue || '-' }}</td>
                        <td>{{ task.quantity }}</td>
                        <td>{{ task.winderName }}</td>
                        <td>{{ formatDate(task.createdAt) }}</td>
                        <td>
                            <select v-if="canChangeStatus(task)" v-model="task.status" @change="updateStatus(task)"
                                class="status-select">
                                <option v-for="(label, key) in availableStatuses(task)" :key="key" :value="key">
                                    {{ label }}
                                </option>
                            </select>
                            <span v-else class="no-action">—</span>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '../store/auth'
import api from '../api'

const authStore = useAuthStore()
console.log(authStore.user?.role)
const isMaster = authStore.isMaster
const currentUserId = ref(null)

// ============================================================
// СОСТОЯНИЕ
// ============================================================

const tasks = ref([])
const winders = ref([])
const loading = ref(false)
const error = ref(null)

// Фильтры
const filterStatus = ref('')
const filterType = ref('')
const filterWinder = ref('')

// Статусы
const statusLabels = {
    new: 'Новое',
    planned: 'Запланировано',
    materials_requested: 'Материалы запрошены',
    materials_issued: 'Материалы выданы',
    in_progress: 'В работе',
    completed: 'Сдано',
    accepted: 'Принято',
    archived: 'В архиве'
}

// Следующие статусы (для выпадающего списка)
const statusFlow = {
    new: ['planned'],
    planned: ['materials_requested'],
    materials_requested: ['materials_issued'],
    materials_issued: ['in_progress'],
    in_progress: ['completed'],
    completed: ['accepted'],
    accepted: ['archived'],
    archived: []
}

// ============================================================
// ВЫЧИСЛЯЕМЫЕ СВОЙСТВА
// ============================================================

const currentUserName = computed(() => {
    return authStore.user?.fullName || 'Пользователь'
})

const filteredTasks = computed(() => {
    let result = tasks.value

    if (filterStatus.value) {
        result = result.filter(t => t.status === filterStatus.value)
    }

    if (filterType.value) {
        result = result.filter(t => t.itemType === filterType.value)
    }

    if (filterWinder.value && isMaster) {
        result = result.filter(t => t.winderId === parseInt(filterWinder.value))
    }

    return result
})

// ============================================================
// МЕТОДЫ
// ============================================================

const loadTasks = async () => {
    loading.value = true
    error.value = null

    try {
        const response = await api.get('/Tasks')
        tasks.value = response.data
    } catch (err) {
        console.error('Ошибка загрузки заданий:', err)
        error.value = 'Ошибка при загрузке заданий'
    } finally {
        loading.value = false
    }
}

const loadWinders = async () => {
    try {
        const response = await api.get('/Reference/users')
        winders.value = response.data.filter(u => u.role === 'winder')
    } catch (err) {
        console.error('Ошибка загрузки мотальщиков:', err)
    }
}

const formatDate = (dateString) => {
    if (!dateString) return '-'
    const date = new Date(dateString)
    return date.toLocaleDateString('ru-RU') + ' ' + date.toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' })
}

const getStatusClass = (status) => {
    const classes = {
        new: 'status-new',
        planned: 'status-planned',
        materials_requested: 'status-requested',
        materials_issued: 'status-issued',
        in_progress: 'status-progress',
        completed: 'status-completed',
        accepted: 'status-accepted',
        archived: 'status-archived'
    }
    return classes[status] || ''
}

const canChangeStatus = (task) => {
    // Мастер может менять статус у всех, мотальщик — только у своих
    if (isMaster) return true
    return task.winderId === currentUserId.value
}

const availableStatuses = (task) => {
    const nextStatuses = statusFlow[task.status] || []
    const result = {}
    nextStatuses.forEach(key => {
        result[key] = statusLabels[key]
    })
    return result
}

const updateStatus = async (task) => {
    try {
        await api.put(`/Tasks/${task.id}/status`, { status: task.status })
        // Обновляем дату в соответствии со статусом
        const now = new Date().toISOString()
        switch (task.status) {
            case 'planned':
                task.assignedAt = now
                break
            case 'materials_issued':
                task.materialsIssuedAt = now
                break
            case 'completed':
                task.completedAt = now
                break
            case 'accepted':
                task.acceptedAt = now
                break
            case 'archived':
                task.archivedAt = now
                break
        }
    } catch (err) {
        console.error('Ошибка обновления статуса:', err)
        alert('Ошибка при обновлении статуса')
        // Перезагружаем задания
        await loadTasks()
    }
}

// ============================================================
// ИНИЦИАЛИЗАЦИЯ
// ============================================================

onMounted(async () => {
    // Получаем ID текущего пользователя из токена
    try {
        const userInfo = await api.get('/Auth/me')
        currentUserId.value = userInfo.data.id
    } catch {
        // Если нет эндпоинта /me, используем данные из store
        currentUserId.value = authStore.user?.id || null
    }

    await loadWinders()
    await loadTasks()
})
</script>

<style scoped>
.tasks-container {
    padding: 2rem;
    max-width: 1400px;
    margin: 0 auto;
}

h1 {
    color: #2c3e50;
    margin-bottom: 1.5rem;
}

/* ============================================================
   Фильтры
   ============================================================ */

.filters {
    display: flex;
    flex-wrap: wrap;
    gap: 1rem;
    margin-bottom: 1.5rem;
    align-items: center;
    background: white;
    padding: 1rem;
    border-radius: 12px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
}

.filter-group {
    display: flex;
    align-items: center;
    gap: 0.5rem;
}

.filter-group label {
    font-size: 0.85rem;
    color: #555;
    font-weight: 500;
}

.filter-select {
    padding: 0.3rem 0.8rem;
    border: 1px solid #ddd;
    border-radius: 6px;
    font-size: 0.85rem;
    background: white;
    cursor: pointer;
}

.filter-static {
    font-size: 0.85rem;
    color: #333;
    font-weight: 500;
}

.refresh-btn {
    padding: 0.3rem 1rem;
    background: #667eea;
    color: white;
    border: none;
    border-radius: 6px;
    cursor: pointer;
    font-size: 0.85rem;
    transition: background 0.2s;
    margin-left: auto;
}

.refresh-btn:hover {
    background: #5a67d8;
}

/* ============================================================
   Таблица
   ============================================================ */

.table-container {
    background: white;
    border-radius: 12px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
    overflow: hidden;
}

.tasks-table {
    width: 100%;
    border-collapse: collapse;
    font-size: 0.85rem;
}

.tasks-table th {
    background: #f8f9fa;
    padding: 0.6rem 0.8rem;
    text-align: left;
    font-weight: 600;
    color: #2c3e50;
    border-bottom: 2px solid #e0e0e0;
    position: sticky;
    top: 0;
    z-index: 1;
    white-space: nowrap;
}

.tasks-table td {
    padding: 0.5rem 0.8rem;
    border-bottom: 1px solid #f0f0f0;
    vertical-align: middle;
}

.tasks-table tr:hover {
    background: #f8f9fa;
}

/* ============================================================
   Статусы
   ============================================================ */

.status-badge {
    display: inline-block;
    padding: 0.15rem 0.6rem;
    border-radius: 12px;
    font-size: 0.75rem;
    font-weight: 600;
    white-space: nowrap;
}

.status-new {
    background: #e3f2fd;
    color: #1976d2;
}

.status-planned {
    background: #fff3e0;
    color: #e65100;
}

.status-requested {
    background: #fce4ec;
    color: #c62828;
}

.status-issued {
    background: #f3e5f5;
    color: #6a1b9a;
}

.status-progress {
    background: #e8f5e9;
    color: #2e7d32;
}

.status-completed {
    background: #e0f7fa;
    color: #00838f;
}

.status-accepted {
    background: #e8eaf6;
    color: #283593;
}

.status-archived {
    background: #eceff1;
    color: #546e7a;
}

/* ============================================================
   Выпадающий список статусов
   ============================================================ */

.status-select {
    padding: 0.15rem 0.4rem;
    border: 1px solid #ddd;
    border-radius: 4px;
    font-size: 0.8rem;
    background: white;
    cursor: pointer;
}

.no-action {
    color: #999;
    font-size: 0.8rem;
}

/* ============================================================
   Состояния
   ============================================================ */

.loading,
.error,
.empty-state {
    padding: 2rem;
    text-align: center;
    color: #7f8c8d;
}

.error {
    color: #e74c3c;
}

.empty-state {
    color: #999;
}

/* ============================================================
   Адаптивность
   ============================================================ */

@media (max-width: 1200px) {
    .tasks-table {
        font-size: 0.8rem;
    }

    .tasks-table th,
    .tasks-table td {
        padding: 0.4rem 0.5rem;
    }
}

@media (max-width: 768px) {
    .filters {
        flex-direction: column;
        align-items: stretch;
    }

    .filter-group {
        flex-wrap: wrap;
    }

    .refresh-btn {
        margin-left: 0;
    }
}
</style>