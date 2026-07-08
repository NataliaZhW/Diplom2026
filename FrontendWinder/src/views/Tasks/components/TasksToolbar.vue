<template>
    <div v-if="!isMaster" class="action-bar">
        <button
            @click="handleCalculate"
            :disabled="selectedIds.length === 0 || !canCalculate"
            class="action-btn primary"
        >
            📊 Рассчитать материалы
        </button>
        <button
            @click="handleSubmitReport"
            :disabled="selectedIds.length === 0 || !canReport"
            class="action-btn success"
        >
            📄 В отчет
        </button>
    </div>
</template>

<script setup>
defineProps({
    isMaster: Boolean,
    selectedIds: Array,
    canCalculate: Boolean,
    canReport: Boolean
})

const emit = defineEmits(['calculate', 'submitReport'])

const handleCalculate = () => {
    emit('calculate')
}

const handleSubmitReport = () => {
    emit('submitReport')
}
</script>

<style scoped>
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
</style>