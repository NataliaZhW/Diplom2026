<template>
    <div v-if="visible" class="modal-overlay" @click.self="handleCancel">
        <div class="modal-content">
            <h3>Подтверждение</h3>
            <p v-if="action === 'status'">
                Перевести задание <strong>{{ task?.itemName }}</strong>
                в статус <strong>{{ nextStatusLabel }}</strong>?
            </p>
            <p v-else-if="action === 'accept'">
                Принять задание <strong>{{ task?.itemName }}</strong>?
            </p>
            <div class="modal-actions">
                <button @click="handleCancel" class="cancel-btn">Отмена</button>
                <button @click="handleConfirm" class="save-btn">ОК</button>
            </div>
        </div>
    </div>
</template>

<script setup>
defineProps({
    visible: Boolean,
    task: Object,
    action: String,
    nextStatusLabel: String
})

const emit = defineEmits(['confirm', 'cancel'])

const handleConfirm = () => {
    emit('confirm')
}

const handleCancel = () => {
    emit('cancel')
}
</script>

<style scoped>
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
</style>