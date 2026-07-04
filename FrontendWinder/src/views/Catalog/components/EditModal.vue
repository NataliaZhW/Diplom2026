<template>
    <div class="modal-overlay" @click.self="$emit('close')">
        <div class="modal-content">
            <h3>Изменить количество</h3>
            <input type="number" :value="localValue" @input="localValue = parseInt($event.target.value) || 1" min="1"
                class="form-input modal-input" />
            <div class="modal-actions">
                <button @click="$emit('save', localValue)" class="save-btn">Сохранить</button>
                <button @click="$emit('close')" class="cancel-btn">Отмена</button>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, watch } from 'vue'

const props = defineProps({
    value: Number
})

const emit = defineEmits(['save', 'close'])

// Локальное состояние для редактирования
const localValue = ref(props.value || 1)

// Синхронизация при изменении пропа
watch(() => props.value, (newVal) => {
    localValue.value = newVal || 1
})
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
    min-width: 250px;
    box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
}

.modal-content h3 {
    margin-bottom: 1rem;
    color: #2c3e50;
}

.modal-input {
    width: 100%;
    padding: 0.5rem;
    font-size: 1rem;
    margin-bottom: 1rem;
    border: 1px solid #ddd;
    border-radius: 4px;
}

.modal-actions {
    display: flex;
    gap: 0.5rem;
    justify-content: flex-end;
}

.save-btn {
    background: #28a745;
    color: white;
    border: none;
    padding: 0.4rem 1.2rem;
    border-radius: 4px;
    cursor: pointer;
}

.save-btn:hover {
    background: #218838;
}

.cancel-btn {
    background: #e0e0e0;
    color: #333;
    border: none;
    padding: 0.4rem 1.2rem;
    border-radius: 4px;
    cursor: pointer;
}

.cancel-btn:hover {
    background: #d0d0d0;
}
</style>