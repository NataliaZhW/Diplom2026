<template>
    <div class="table-container">
        <div v-if="loading" class="loading">Загрузка...</div>
        <div v-else-if="error" class="error">{{ error }}</div>
        <div v-else-if="tasks.length === 0" class="empty-state">Нет заданий</div>

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
                <tr v-for="task in tasks" :key="task.id" @dblclick="$emit('openDetail', task)">
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
                        <!-- МАСТЕР / АДМИН -->
                        <template v-if="isMaster">
                            <template v-if="task.status === 'submitted' || task.status === 'reported' || task.status === 'archived'">
                                <button
                                    v-if="!task.acceptedAt"
                                    @click="$emit('accept', task)"
                                    class="accept-btn"
                                >
                                    ✅ Принять
                                </button>
                                <span
                                    v-else
                                    class="accepted-label"
                                    @contextmenu.prevent="$emit('contextmenu', $event, task.id)"
                                >
                                    ✅ Принято {{ formatDate(task.acceptedAt) }}
                                </span>
                            </template>
                            <span v-else class="no-action">—</span>
                        </template>

                        <!-- МОТАЛЬЩИК -->
                        <template v-else>
                            <!-- Новое → чекбокс -->
                            <template v-if="task.status === 'new'">
                                <input
                                    type="checkbox"
                                    :value="task.id"
                                    :checked="selectedIds.includes(task.id)"
                                    @change="$emit('update:selectedIds', $event.target.checked ? [...selectedIds, task.id] : selectedIds.filter(id => id !== task.id))"
                                    class="task-checkbox"
                                />
                                <span class="action-label">расчет материалов</span>
                            </template>

                            <!-- Сдано → чекбокс -->
                            <template v-else-if="task.status === 'submitted'">
                                <input
                                    type="checkbox"
                                    :value="task.id"
                                    :checked="selectedIds.includes(task.id)"
                                    @change="$emit('update:selectedIds', $event.target.checked ? [...selectedIds, task.id] : selectedIds.filter(id => id !== task.id))"
                                    class="task-checkbox"
                                />
                                <span class="action-label">в отчет</span>
                            </template>

                            <!-- Пошаговое переключение -->
                            <button
                                v-else-if="canChangeStatus(task)"
                                @click="$emit('showStatusConfirm', task)"
                                class="status-btn"
                            >
                                {{ getNextStatusLabel(task) }}
                            </button>

                            <span v-else class="no-action">—</span>
                        </template>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</template>

<script setup>
import { STATUS_CLASSES } from '../constants/statuses'

defineProps({
    tasks: Array,
    loading: Boolean,
    error: String,
    isMaster: Boolean,
    selectedIds: Array,
    canChangeStatus: Function,
    getNextStatusLabel: Function,
    getStatusClass: Function,
    formatDate: Function
})

defineEmits(['accept', 'contextmenu', 'showStatusConfirm', 'openDetail', 'update:selectedIds'])
</script>

<style scoped>
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

/* Статусы */
.status-badge {
    display: inline-block;
    padding: 0.15rem 0.6rem;
    border-radius: 12px;
    font-size: 0.75rem;
    font-weight: 600;
    white-space: nowrap;
}

.status-new { background: #e3f2fd; color: #1976d2; }
.status-requested { background: #fce4ec; color: #c62828; }
.status-issued { background: #f3e5f5; color: #6a1b9a; }
.status-completed { background: #e0f7fa; color: #00838f; }
.status-reported { background: #f39c12; color: white; }
.status-archived { background: #eceff1; color: #546e7a; }

/* Действия */
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

.task-checkbox {
    margin-right: 0.4rem;
    cursor: pointer;
}

.action-label {
    font-size: 0.75rem;
    color: #555;
    cursor: default;
}

.no-action {
    color: #999;
    font-size: 0.8rem;
}

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