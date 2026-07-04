<template>
    <div class="list-panel">
        <div class="panel-header">
            <span>{{ title }}</span>
            <span class="count">{{ items.length }}</span>
        </div>

        <div v-if="loading" class="loading">Загрузка...</div>
        <div v-else-if="error" class="error">{{ error }}</div>
        <div v-else class="list-items">
            <div v-for="item in items" :key="item.id" class="list-item"
                :class="{ active: selectedItem?.id === item.id }" @click="$emit('select', item)">
                <span class="item-code">{{ item.internalCode || item.code }}</span>
                <span class="item-name">{{ item.name }}</span>
            </div>
            <div v-if="items.length === 0" class="empty-list">Нет элементов</div>
        </div>
    </div>
</template>

<script setup>
defineProps({
    title: String,
    items: Array,
    selectedItem: Object,
    loading: Boolean,
    error: String
})

defineEmits(['select'])
</script>

<style scoped>
.list-panel {
    background: white;
    border-radius: 12px;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.08);
    overflow: hidden;
    display: flex;
    flex-direction: column;
}

.panel-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0.6rem 1rem;
    background: #f8f9fa;
    border-bottom: 1px solid #e0e0e0;
    font-weight: 600;
    color: #2c3e50;
    flex-shrink: 0;
}

.count {
    background: #667eea;
    color: white;
    padding: 0.1rem 0.6rem;
    border-radius: 12px;
    font-size: 0.8rem;
}

.list-items {
    flex: 1;
    overflow-y: auto;
    max-height: 600px;
}

.list-item {
    display: flex;
    gap: 0.8rem;
    padding: 0.5rem 1rem;
    cursor: pointer;
    border-bottom: 1px solid #f0f0f0;
    transition: background 0.2s;
}

.list-item:hover {
    background: #f8f9fa;
}

.list-item.active {
    background: #e8edfe;
    border-left: 3px solid #667eea;
}

.item-code {
    font-weight: 600;
    color: #2c3e50;
    min-width: 55px;
}

.item-name {
    color: #555;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
}

.empty-list {
    padding: 1.5rem;
    text-align: center;
    color: #999;
}

.loading,
.error {
    padding: 2rem;
    text-align: center;
    color: #7f8c8d;
}

.error {
    color: #e74c3c;
}
</style>