<template>
    <div class="filters">
        <div class="filter-group">
            <label>Статус:</label>
            <select :value="filterStatus" @change="$emit('update:filterStatus', $event.target.value)" class="filter-select">
                <option value="">Все</option>
                <option value="!archived">Все, кроме архива</option>
                <option v-for="(label, key) in statusLabels" :key="key" :value="key">
                    {{ label }}
                </option>
            </select>
        </div>

        <div class="filter-group">
            <label>Тип:</label>
            <select :value="filterType" @change="$emit('update:filterType', $event.target.value)" class="filter-select">
                <option value="">Все</option>
                <option value="kit">Набор</option>
                <option value="scheme">Схема</option>
                <option value="thread">Нить</option>
            </select>
        </div>

        <div class="filter-group">
            <label>Мотальщик:</label>
            <select
                v-if="isMaster"
                :value="filterWinder"
                @change="$emit('update:filterWinder', $event.target.value)"
                class="filter-select"
            >
                <option value="">Все</option>
                <option v-for="user in winders" :key="user.id" :value="user.id">
                    {{ user.fullName }}
                </option>
            </select>
            <span v-else class="filter-static">{{ currentUserName }}</span>
        </div>

        <button @click="$emit('refresh')" class="refresh-btn">🔄 Обновить</button>
    </div>
</template>

<script setup>
import { STATUS_LABELS } from '../constants/statuses'

defineProps({
    filterStatus: String,
    filterType: String,
    filterWinder: String,
    isMaster: Boolean,
    winders: Array,
    currentUserName: String
})

defineEmits(['update:filterStatus', 'update:filterType', 'update:filterWinder', 'refresh'])

const statusLabels = STATUS_LABELS
</script>

<style scoped>
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
</style>