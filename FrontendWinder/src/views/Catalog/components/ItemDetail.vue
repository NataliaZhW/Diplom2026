<template>
    <div class="detail-panel">
        <div class="panel-header">
            <span>Состав</span>
            <span v-if="selectedItem" class="item-title">
                {{ selectedItem.internalCode || selectedItem.code }}
                {{ selectedItem.name }}
            </span>
        </div>

        <div v-if="!selectedItem" class="empty-state">Выберите элемент из списка</div>

        <!-- Нить -->
        <div v-else-if="catalogType === 'thread'" class="detail-content">
            <table class="detail-table">
                <thead>
                    <tr>
                        <th>Код</th>
                        <th>Название</th>
                        <th>{{ brandType === 'pnk' ? 'ПНК' : 'DMC' }}</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>{{ selectedItem.code }}</td>
                        <td>{{ selectedItem.name }}</td>
                        <td>{{ brandType === 'pnk' ? selectedItem.pnk : selectedItem.dmc }}</td>
                    </tr>
                </tbody>
            </table>
        </div>

        <!-- Набор -->
        <div v-else-if="catalogType === 'kit'" class="detail-content">
            <table class="kit-table">
                <thead>
                    <tr>
                        <th>Значок</th>
                        <th>Код</th>
                        <th>Название</th>
                        <th>Метраж</th>
                        <th>Бирочки</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="comp in selectedItem.compositions" :key="comp.id">
                        <td class="icon-cell">{{ comp.icon }}</td>
                        <td>{{ comp.colorCode }}</td>
                        <td>{{ comp.colorName }}</td>
                        <td>{{ comp.meterage ?? '-' }}</td>
                        <td>{{ comp.labelsCount ?? '-' }}</td>
                    </tr>
                </tbody>
            </table>
        </div>

        <!-- Схема -->
        <div v-else-if="catalogType === 'scheme'" class="detail-content">
            <table class="scheme-table">
                <thead>
                    <tr>
                        <th>Код</th>
                        <th>Название</th>
                        <th v-for="count in schemeCounts" :key="count">{{ count }}</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="comp in selectedItem.compositions" :key="comp.id">
                        <td>{{ comp.colorCode }}</td>
                        <td>{{ comp.colorName }}</td>
                        <td v-for="count in schemeCounts" :key="count">
                            {{ comp['count' + count] > 0 ? comp['count' + count] : '-' }}
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>

<script setup>
defineProps({
    selectedItem: Object,
    catalogType: String,
    brandType: String,
    schemeCounts: Array
})
</script>

<style scoped>
.detail-panel {
    background: white;
    border-radius: 12px;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.08);
    overflow: hidden;
    display: flex;
    flex-direction: column;
    flex: 1;
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

.item-title {
    font-weight: 600;
    color: #667eea;
}

.empty-state {
    display: flex;
    justify-content: center;
    align-items: center;
    flex: 1;
    color: #999;
    font-size: 0.95rem;
    padding: 2rem;
}

.detail-content {
    padding: 0.8rem;
    overflow-x: auto;
    flex: 1;
}

.detail-table,
.kit-table,
.scheme-table {
    border-collapse: collapse;
    font-size: 0.85rem;
    width: 100%;
}

.detail-table th,
.detail-table td,
.kit-table th,
.kit-table td,
.scheme-table th,
.scheme-table td {
    padding: 0.3rem 0.8rem;
    text-align: left;
    white-space: nowrap;
    border-bottom: 1px solid #f0f0f0;
}

.detail-table th,
.kit-table th,
.scheme-table th {
    background: #f8f9fa;
    font-weight: 600;
    color: #2c3e50;
    border-bottom: 2px solid #e0e0e0;
    position: sticky;
    top: 0;
    z-index: 1;
}

.detail-table tr:hover,
.kit-table tr:hover,
.scheme-table tr:hover {
    background: #f8f9fa;
}

.kit-table th:nth-child(4),
.kit-table td:nth-child(4),
.kit-table th:nth-child(5),
.kit-table td:nth-child(5),
.scheme-table th:nth-child(n+3),
.scheme-table td:nth-child(n+3) {
    text-align: center;
}

.icon-cell {
    font-size: inherit;
    text-align: center;
}
</style>