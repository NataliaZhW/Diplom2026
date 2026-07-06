<template>
    <div class="tasks-container">
        <h1>📋 Задания</h1>

        <!-- Кнопки для мотальщика (сверху) -->
        <div v-if="!isMaster" class="action-bar">
            <button 
                @click="calculateMaterials" 
                :disabled="selectedIds.length === 0 || !canCalculate" 
                class="action-btn primary"
            >
                📊 Рассчитать материалы
            </button>
            <button 
                @click="submitReport" 
                :disabled="selectedIds.length === 0 || !canReport" 
                class="action-btn success"
            >
                📄 В отчет
            </button>
        </div>

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
                            <!-- ============================================================
                            МАСТЕР / АДМИН
                            ============================================================ -->
                            <template v-if="isMaster">
                                <!-- Сдано → кнопка "Принять" -->
                                <template v-if="task.status === 'submitted'">
                                    <button v-if="!task.acceptedAt" @click="acceptTask(task)" class="accept-btn">
                                        ✅ Принять
                                    </button>
                                    <span v-else class="accepted-label"
                                        @contextmenu.prevent="showContextMenu($event, task.id)">
                                        ✅ Принято {{ formatDate(task.acceptedAt) }}
                                    </span>
                                </template>

                                <!-- Внесено в отчетность → кнопка "Принять" -->
                                <template v-else-if="task.status === 'reported'">
                                    <button v-if="!task.acceptedAt" @click="acceptTask(task)" class="accept-btn">
                                        ✅ Принять
                                    </button>
                                    <span v-else class="accepted-label"
                                        @contextmenu.prevent="showContextMenu($event, task.id)">
                                        ✅ Принято {{ formatDate(task.acceptedAt) }}
                                    </span>
                                </template>

                                <!-- В архиве → кнопка "Принять" -->
                                <template v-else-if="task.status === 'archived'">
                                    <button v-if="!task.acceptedAt" @click="acceptTask(task)" class="accept-btn">
                                        ✅ Принять
                                    </button>
                                    <span v-else class="accepted-label"
                                        @contextmenu.prevent="showContextMenu($event, task.id)">
                                        ✅ Принято {{ formatDate(task.acceptedAt) }}
                                    </span>
                                </template>

                                <!-- Остальные статусы → пусто -->
                                <span v-else class="no-action">—</span>
                            </template>
                            <!-- ============================================================
                            МОТАЛЬЩИК
                            ============================================================ -->
                            <template v-else>
                                <!-- Новое → чекбокс + надпись "расчет материалов" -->
                                <template v-if="task.status === 'new'">
                                    <input
                                        type="checkbox"
                                        :value="task.id"
                                        v-model="selectedIds"
                                        class="task-checkbox"
                                    />
                                    <span class="action-label">расчет материалов</span>
                                </template>

                                <!-- Сдано → чекбокс + надпись "в отчет" -->
                                <template v-else-if="task.status === 'submitted'">
                                    <input
                                        type="checkbox"
                                        :value="task.id"
                                        v-model="selectedIds"
                                        class="task-checkbox"
                                    />
                                    <span class="action-label">в отчет</span>
                                </template>

                                <!-- Пошаговое переключение: кнопка со следующим статусом -->
                                <template v-else-if="canChangeStatus(task)">
                                    <button @click="showStatusConfirm(task)" class="status-btn">
                                        {{ getNextStatusLabel(task) }}
                                    </button>
                                </template>

                                <!-- Остальные статусы → пусто -->
                                <span v-else class="no-action">—</span>
                            </template>
                        </td>
                    </tr>
                </tbody>
            </table>

<!-- Модалка подтверждения -->
<div v-if="confirmModalVisible" class="modal-overlay" @click.self="cancelConfirm">
    <div class="modal-content">
        <h3>Подтверждение</h3>
        <p v-if="confirmAction === 'status'">
            Перевести задание <strong>{{ confirmTask?.itemName }}</strong> 
            в статус <strong>{{ getNextStatusLabel(confirmTask) }}</strong>?
        </p>
        <p v-else-if="confirmAction === 'accept'">
            Принять задание <strong>{{ confirmTask?.itemName }}</strong>?
        </p>
        <div class="modal-actions">
            <button @click="cancelConfirm" class="cancel-btn">Отмена</button>
            <button @click="confirmStatusChange" class="save-btn">ОК</button>
        </div>
    </div>
</div>
            <!-- Контекстное меню для отмены принятия -->
            <div v-if="contextMenuVisible" 
                    class="context-menu" 
                    :style="{ top: contextMenuY + 'px', left: contextMenuX + 'px' }"
                    @click.stop>
                <div class="context-item" @click="cancelAccept">
                    ↩️ Отменить принятие
                </div>
                <div class="context-item" @click="closeContextMenu">
                    ❌ Отмена
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '../store/auth'
import api from '../api'

const authStore = useAuthStore()
const isMaster = authStore.isMaster
const currentUserId = ref(null)

// ============================================================
// СОСТОЯНИЕ
// ============================================================

const tasks = ref([])
const winders = ref([])
const loading = ref(false)
const error = ref(null)
const selectedIds = ref([])

const contextMenuVisible = ref(false)
const contextMenuX = ref(0)
const contextMenuY = ref(0)
const contextMenuTaskId = ref(null)

const filterStatus = ref('')
const filterType = ref('')
const filterWinder = ref('')

// ============================================================
// МОДАЛКА ПОДТВЕРЖДЕНИЯ
// ============================================================

const confirmModalVisible = ref(false)
const confirmTask = ref(null)
const confirmAction = ref(null) // 'status' или 'accept'

// ============================================================
// СТАТУСЫ
// ============================================================

const statusLabels = {
    new: 'Новое',
    materials_requested: 'Материалы запрошены',
    materials_received: 'Материалы получены',
    submitted: 'Сдано',
    reported: 'Внесено в отчетность',
    archived: 'В архиве'
}

// ✅ Пошаговые переходы (для мотальщика)
const statusFlow = {
    materials_requested: ['materials_received'],
    materials_received: ['submitted'],
    reported: ['archived'],
    new: [],
    submitted: [],
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

const canCalculate = computed(() => {
    if (selectedIds.value.length === 0) return false
    const selectedTasks = tasks.value.filter(t => selectedIds.value.includes(t.id))
    return selectedTasks.every(t => t.status === 'new')
})

const canReport = computed(() => {
    if (selectedIds.value.length === 0) return false
    const selectedTasks = tasks.value.filter(t => selectedIds.value.includes(t.id))
    return selectedTasks.every(t => t.status === 'submitted')
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
        materials_requested: 'status-requested',
        materials_received: 'status-issued',
        submitted: 'status-completed',
        reported: 'status-reported',
        archived: 'status-archived'
    }
    return classes[status] || ''
}

const canChangeStatus = (task) => {
    // Только для мотальщика и только для статусов, у которых есть переходы
    if (isMaster) return false
    if (task.winderId !== currentUserId.value) return false
    const nextStatuses = statusFlow[task.status] || []
    return nextStatuses.length > 0
}

const availableStatuses = (task) => {
    const nextStatuses = statusFlow[task.status] || []
    const result = {}
    nextStatuses.forEach(key => {
        result[key] = statusLabels[key]
    })
    return result
}

// const updateStatus = async (task) => {

//     console.log('=== updateStatus ===')
//     console.log('task.id:', task.id)
//     console.log('task.status (новый):', task.status)
//     console.log('Отправляем:', { status: task.status })

//     try {
//         await api.put(`/Tasks/${task.id}/status`, { status: task.status })
//         const now = new Date().toISOString()
//         switch (task.status) {
//             case 'materials_received':
//                 task.materialsIssuedAt = now
//                 break
//             case 'submitted':
//                 task.SubmittedAt = now
//                 break
//             case 'reported':
//                 task.ReportedAt = now
//                 break
//             case 'archived':
//                 task.ArchivedAt = now
//                 break
//         }
//         alert('✅ Статус обновлён')
//     } catch (err) {
//         console.error('Ошибка обновления статуса:', err)
//         alert('❌ Ошибка при обновлении статуса')
//         await loadTasks()
//     }
// }

const acceptTask = async (task) => {
    try {
        const response = await api.post(`/Tasks/${task.id}/accept`)
        const acceptedDate = response.data.acceptedAt
        
        // ✅ МАСТЕР ВСЕГДА ЗАПИСЫВАЕТ ТОЛЬКО В acceptedAt
        task.acceptedAt = acceptedDate
        
        alert('✅ Задание принято')
    } catch (err) {
        alert('❌ ' + (err.response?.data?.message || 'Ошибка'))
    }
}

const cancelAccept = async () => {
    if (!contextMenuTaskId.value) return
    
    try {
        await api.post(`/Tasks/${contextMenuTaskId.value}/cancel-accept`)
        const task = tasks.value.find(t => t.id === contextMenuTaskId.value)
        
        if (task) {
            // ✅ МАСТЕР ВСЕГДА ОЧИЩАЕТ ТОЛЬКО acceptedAt
            task.acceptedAt = null
        }
        
        alert('✅ Принятие отменено')
    } catch (err) {
        alert('❌ ' + (err.response?.data?.message || 'Ошибка'))
    } finally {
        closeContextMenu()
    }
}

const calculateMaterials = async () => {
    if (selectedIds.value.length === 0) {
        alert('Выберите задания в статусе "Новое"')
        return
    }

    try {
        const response = await api.post('/Tasks/batch/calculate-materials', selectedIds.value)
        alert(`✅ ${response.data.message}`)
        selectedIds.value = []
        await loadTasks()
    } catch (err) {
        alert('❌ ' + (err.response?.data?.message || 'Ошибка'))
    }
}

const submitReport = async () => {
    if (selectedIds.value.length === 0) {
        alert('Выберите задания в статусе "Сдано"')
        return
    }

    try {
        const response = await api.post('/Tasks/batch/submit-report', selectedIds.value)
        alert(`✅ ${response.data.message}`)
        selectedIds.value = []
        await loadTasks()
    } catch (err) {
        alert('❌ ' + (err.response?.data?.message || 'Ошибка'))
    }
}

const showContextMenu = (event, taskId) => {
    contextMenuX.value = event.clientX
    contextMenuY.value = event.clientY
    contextMenuTaskId.value = taskId
    contextMenuVisible.value = true
}

const closeContextMenu = () => {
    contextMenuVisible.value = false
    contextMenuTaskId.value = null
}

document.addEventListener('click', () => {
    if (contextMenuVisible.value) closeContextMenu()
})

// ============================================================
// МЕТОДЫ ДЛЯ КНОПОК СТАТУСОВ
// ============================================================

const getNextStatusLabel = (task) => {
    const nextStatuses = statusFlow[task.status] || []
    if (nextStatuses.length === 0) return ''
    const nextKey = nextStatuses[0]
    return statusLabels[nextKey] || nextKey
}

const showStatusConfirm = (task) => {
    confirmTask.value = task
    confirmAction.value = 'status'
    confirmModalVisible.value = true
}

const confirmStatusChange = async () => {
    if (!confirmTask.value) return
    
    const task = confirmTask.value
    const nextStatuses = statusFlow[task.status] || []
    if (nextStatuses.length === 0) return
    
    const newStatus = nextStatuses[0]
    
    try {
        await api.put(`/Tasks/${task.id}/status`, { status: newStatus })
        
        // Обновляем локальное состояние
        task.status = newStatus
        const now = new Date().toISOString()
        switch (newStatus) {
            case 'materials_received':
                task.materialsIssuedAt = now
                break
            case 'submitted':
                task.SubmittedAt = now
                break
            case 'reported':
                task.ReportedAt = now
                break
            case 'archived':
                task.ArchivedAt = now
                break
        }
        
        confirmModalVisible.value = false
        confirmTask.value = null
        
        // Спрашиваем, обновить ли страницу
        // if (confirm('✅ Статус обновлён.')) {
            await loadTasks()
        // }
    } catch (err) {
        console.error('Ошибка обновления статуса:', err)
        alert('❌ Ошибка при обновлении статуса: ' + (err.response?.data?.message || err.message))
    }
}

const cancelConfirm = () => {
    confirmModalVisible.value = false
    confirmTask.value = null
    confirmAction.value = null
}

// ============================================================
// ИНИЦИАЛИЗАЦИЯ
// ============================================================

onMounted(async () => {
    try {
        const userInfo = await api.get('/Auth/me')
        currentUserId.value = userInfo.data.id
    } catch {
        currentUserId.value = authStore.user?.id || null
    }

    if (isMaster) { await loadWinders() }
    await loadTasks()
})
</script>

<style scoped>
/* ============================================================
   ОСНОВНЫЕ СТИЛИ
   ============================================================ */

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
   КНОПКИ ДЕЙСТВИЙ (для мотальщика)
   ============================================================ */

.action-bar {
    display: flex;
    gap: 1rem;
    margin-bottom: 1rem;
    flex-wrap: wrap;
}

.action-btn {
    padding: 0.5rem 1.5rem;
    border: none;
    border-radius: 8px;
    font-size: 0.9rem;
    font-weight: 600;
    cursor: pointer;
    transition: background 0.2s;
}

.action-btn:disabled {
    opacity: 0.5;
    cursor: not-allowed;
}

.action-btn.primary {
    background: #3498db;
    color: white;
}

.action-btn.primary:hover:not(:disabled) {
    background: #2980b9;
}

.action-btn.success {
    background: #2ecc71;
    color: white;
}

.action-btn.success:hover:not(:disabled) {
    background: #27ae60;
}

/* ============================================================
   ФИЛЬТРЫ
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
   ТАБЛИЦА
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
   СТАТУСЫ
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

.status-requested {
    background: #fce4ec;
    color: #c62828;
}

.status-issued {
    background: #f3e5f5;
    color: #6a1b9a;
}

.status-completed {
    background: #e0f7fa;
    color: #00838f;
}

.status-reported {
    background: #f39c12;
    color: white;
}

.status-archived {
    background: #eceff1;
    color: #546e7a;
}

/* ============================================================
   ДЕЙСТВИЯ
   ============================================================ */

.accept-btn {
    background: #2ecc71;
    color: white;
    border: none;
    padding: 0.2rem 0.8rem;
    border-radius: 4px;
    cursor: pointer;
    font-size: 0.8rem;
    transition: background 0.2s;
}

.accept-btn:hover {
    background: #27ae60;
}

.accepted-label {
    color: #2ecc71;
    font-weight: 600;
    font-size: 0.85rem;
    cursor: context-menu;
}

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

/* Чекбокс + надпись */
.task-checkbox {
    margin-right: 0.4rem;
    cursor: pointer;
}

.action-label {
    font-size: 0.75rem;
    color: #555;
    cursor: default;
}

/* ============================================================
   КОНТЕКСТНОЕ МЕНЮ
   ============================================================ */

.context-menu {
    position: fixed;
    background: white;
    border-radius: 8px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
    padding: 0.3rem 0;
    min-width: 180px;
    z-index: 1000;
    border: 1px solid #e0e0e0;
}

.context-item {
    padding: 0.5rem 1rem;
    cursor: pointer;
    font-size: 0.85rem;
    transition: background 0.1s;
}

.context-item:hover {
    background: #f0f0f0;
}

/* ============================================================
   СОСТОЯНИЯ
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

.status-btn {
    background: #3498db;
    color: white;
    border: none;
    padding: 0.2rem 0.8rem;
    border-radius: 4px;
    cursor: pointer;
    font-size: 0.75rem;
    transition: background 0.2s;
}

.status-btn:hover {
    background: #2980b9;
}


.modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.4);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1001;
}

.modal-content {
    background: white;
    border-radius: 12px;
    padding: 1.5rem 2rem;
    min-width: 300px;
    max-width: 400px;
    box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
}

.modal-content h3 {
    margin-bottom: 0.8rem;
    color: #2c3e50;
}

.modal-content p {
    margin-bottom: 1.2rem;
    color: #555;
    font-size: 0.95rem;
    line-height: 1.5;
}

.modal-content p strong {
    color: #2c3e50;
}

.modal-actions {
    display: flex;
    gap: 0.8rem;
    justify-content: flex-end;
}

.modal-actions button {
    padding: 0.4rem 1.2rem;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-size: 0.9rem;
}

.save-btn {
    background: #28a745;
    color: white;
}

.save-btn:hover {
    background: #218838;
}

.cancel-btn {
    background: #e0e0e0;
    color: #333;
}

.cancel-btn:hover {
    background: #d0d0d0;
}

/* ============================================================
   АДАПТИВНОСТЬ
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

    .action-bar {
        flex-direction: column;
    }
}
</style>