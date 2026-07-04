<template>
    <div class="selected-list">
        <table class="selected-table">
            <thead>
                <tr>
                    <th>Тип</th>
                    <th>Код</th>
                    <th>Название</th>
                    <th>ПНК/DMC</th>
                    <th>Каунт</th>
                    <th>Кол-во</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="(item, index) in items" :key="index"
                    @contextmenu.prevent="$emit('contextmenu', $event, index)">
                    <td>{{ item.typeLabel }}</td>
                    <td>{{ item.code }}</td>
                    <td>{{ item.name }}</td>
                    <td>{{ item.brandLabel || '-' }}</td>
                    <td>{{ item.count || '-' }}</td>
                    <td>{{ item.quantity }}</td>
                </tr>
                <tr v-if="items.length === 0">
                    <td colspan="6" class="empty-row">Список пуст</td>
                </tr>
            </tbody>
        </table>
    </div>
</template>

<script setup>
defineProps({
    items: Array
})

defineEmits(['contextmenu'])
</script>

<style scoped>
.selected-list {
    flex: 1;
    overflow-y: auto;
    max-height: 350px;
}

.selected-table {
    width: 100%;
    border-collapse: collapse;
    font-size: 0.82rem;
}

.selected-table th {
    background: #f8f9fa;
    padding: 0.4rem 0.5rem;
    text-align: left;
    font-weight: 600;
    color: #2c3e50;
    border-bottom: 2px solid #e0e0e0;
    position: sticky;
    top: 0;
    z-index: 1;
}

.selected-table td {
    padding: 0.3rem 0.5rem;
    border-bottom: 1px solid #f0f0f0;
}

.selected-table tr:hover {
    background: #f8f9fa;
    cursor: context-menu;
}

.selected-table .empty-row {
    text-align: center;
    color: #999;
    padding: 1.5rem;
}
</style>