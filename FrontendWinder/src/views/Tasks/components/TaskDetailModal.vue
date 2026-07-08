<template>
    <div v-if="visible" class="modal-overlay" @click.self="close">
        <div class="modal-content">
            <div class="modal-header">
                <h2>📋 Детали задания</h2>
                <button @click="close" class="close-btn">✕</button>
            </div>

            <div class="modal-body" v-if="task">
                <!-- ============================================================
                ОСНОВНЫЕ ПОЛЯ
                ============================================================ -->
                <div class="detail-row">
                    <span class="label">ID</span>
                    <span class="value">{{ task.id }}</span>
                </div>
                <div class="detail-row">
                    <span class="label">Статус</span>
                    <span class="value">
                        <span :class="['status-badge', getStatusClass(task.status)]">
                            {{ task.statusLabel }}
                        </span>
                    </span>
                </div>
                <div class="detail-row">
                    <span class="label">Тип</span>
                    <span class="value">{{ task.itemTypeLabel }}</span>
                </div>
                <div class="detail-row">
                    <span class="label">ID элемента</span>
                    <span class="value">{{ task.itemId }}</span>
                </div>
                <div class="detail-row">
                    <span class="label">Код</span>
                    <span class="value">{{ task.itemCode }}</span>
                </div>
                <div class="detail-row">
                    <span class="label">Название</span>
                    <span class="value">{{ task.itemName }}</span>
                </div>
                <div class="detail-row">
                    <span class="label">ПНК/DMC</span>
                    <span class="value">{{ task.brandLabel || '-' }}</span>
                </div>
                <div class="detail-row">
                    <span class="label">Каунт</span>
                    <span class="value">{{ task.countValue || '-' }}</span>
                </div>
                <div class="detail-row">
                    <span class="label">Количество</span>
                    <span class="value">{{ task.quantity }}</span>
                </div>

                <!-- ============================================================
                КТО И КОГДА
                ============================================================ -->
                <div class="detail-row">
                    <span class="label">Мотальщик</span>
                    <span class="value">{{ task.winderName }}</span>
                </div>
                <div class="detail-row">
                    <span class="label">ID мотальщика</span>
                    <span class="value">{{ task.winderId }}</span>
                </div>
                <div class="detail-row">
                    <span class="label">Кем назначено</span>
                    <span class="value">{{ task.assignedByName || '-' }}</span>
                </div>

                <!-- ============================================================
                ВСЕ ДАТЫ (всегда показываются)
                ============================================================ -->
                <div class="detail-row">
                    <span class="label">Дата создания</span>
                    <span class="value">{{ formatDate(task.createdAt) }}</span>
                </div>
                <div class="detail-row">
                    <span class="label">Дата назначения</span>
                    <span class="value">{{ task.assignedAt ? formatDate(task.assignedAt) : '—' }}</span>
                </div>
                <div class="detail-row">
                    <span class="label">Материалы запрошены</span>
                    <span class="value">{{ task.materialsRequestedAt ? formatDate(task.materialsRequestedAt) : '—' }}</span>
                </div>
                <div class="detail-row">
                    <span class="label">Материалы выданы</span>
                    <span class="value">{{ task.materialsIssuedAt ? formatDate(task.materialsIssuedAt) : '—' }}</span>
                </div>
                <div class="detail-row">
                    <span class="label">Сдано</span>
                    <span class="value">{{ task.submittedAt ? formatDate(task.submittedAt) : '—' }}</span>
                </div>
                <div class="detail-row">
                    <span class="label">Принято мастером</span>
                    <span class="value">{{ task.acceptedAt ? formatDate(task.acceptedAt) : '—' }}</span>
                </div>
                <div class="detail-row">
                    <span class="label">Внесено в отчетность</span>
                    <span class="value">{{ task.reportedAt ? formatDate(task.reportedAt) : '—' }}</span>
                </div>
                <div class="detail-row">
                    <span class="label">В архиве</span>
                    <span class="value">{{ task.archivedAt ? formatDate(task.archivedAt) : '—' }}</span>
                </div>

                <!-- ============================================================
                ПРИМЕЧАНИЕ
                ============================================================ -->
                <div class="detail-row">
                    <span class="label">Примечание</span>
                    <span class="value">{{ task.note || '—' }}</span>
                </div>
            </div>

            <div class="modal-footer">
                <button @click="close" class="close-modal-btn">Закрыть</button>
            </div>
        </div>
    </div>
</template>

<script setup>
import { STATUS_CLASSES } from '../constants/statuses'

defineProps({
    visible: Boolean,
    task: Object,
    getStatusClass: Function,
    formatDate: Function
})

const emit = defineEmits(['close'])

const close = () => {
    emit('close')
}
</script>

<style scoped>
.modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.5);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 2000;
    animation: fadeIn 0.2s ease;
}

.modal-content {
    background: white;
    border-radius: 16px;
    width: 100%;
    max-width: 600px;
    max-height: 90vh;
    overflow-y: auto;
    box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
    animation: slideIn 0.3s ease;
}

@keyframes fadeIn {
    from { opacity: 0; }
    to { opacity: 1; }
}

@keyframes slideIn {
    from {
        opacity: 0;
        transform: translateY(-20px) scale(0.95);
    }
    to {
        opacity: 1;
        transform: translateY(0) scale(1);
    }
}

.modal-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 1.2rem 1.5rem;
    border-bottom: 1px solid #e0e0e0;
    background: #f8f9fa;
    border-radius: 16px 16px 0 0;
    position: sticky;
    top: 0;
    z-index: 1;
}

.modal-header h2 {
    margin: 0;
    color: #2c3e50;
    font-size: 1.3rem;
}

.close-btn {
    background: none;
    border: none;
    font-size: 1.5rem;
    cursor: pointer;
    color: #999;
    transition: color 0.2s;
    padding: 0 0.5rem;
}

.close-btn:hover {
    color: #333;
}

.modal-body {
    padding: 1.5rem;
}

.detail-row {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0.6rem 0;
    border-bottom: 1px solid #f0f0f0;
}

.detail-row:last-child {
    border-bottom: none;
}

.detail-row .label {
    font-weight: 600;
    color: #555;
    font-size: 0.9rem;
    min-width: 170px;
    flex-shrink: 0;
}

.detail-row .value {
    color: #2c3e50;
    font-size: 0.95rem;
    text-align: right;
    word-break: break-word;
}

/* Статусы в модалке */
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

.modal-footer {
    padding: 1rem 1.5rem;
    border-top: 1px solid #e0e0e0;
    background: #f8f9fa;
    border-radius: 0 0 16px 16px;
    text-align: right;
    position: sticky;
    bottom: 0;
    z-index: 1;
}

.close-modal-btn {
    padding: 0.5rem 1.5rem;
    background: #667eea;
    color: white;
    border: none;
    border-radius: 8px;
    cursor: pointer;
    font-size: 0.9rem;
    transition: background 0.2s;
}

.close-modal-btn:hover {
    background: #5a67d8;
}

@media (max-width: 768px) {
    .modal-content {
        max-width: 95%;
        margin: 0 1rem;
    }

    .detail-row {
        flex-direction: column;
        align-items: flex-start;
        gap: 0.2rem;
    }

    .detail-row .value {
        text-align: left;
        width: 100%;
    }

    .detail-row .label {
        min-width: auto;
    }
}
</style>