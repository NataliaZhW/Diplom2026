<template>
    <div class="context-menu" :style="{ top: y + 'px', left: x + 'px' }" @click.stop>
        <div class="context-item" @click="$emit('edit')">✏️ Изменить количество</div>
        <div class="context-item danger" @click="$emit('delete')">🗑️ Удалить</div>
        <div class="context-item" @click="$emit('close')">❌ Отмена</div>
    </div>
</template>

<script setup>
defineProps({
    x: Number,
    y: Number
})

defineEmits(['edit', 'delete', 'close'])
</script>

<style scoped>
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

.context-item.danger {
    color: #e74c3c;
}

.context-item.danger:hover {
    background: #fde8e8;
}
</style>