<template>
    <div class="catalog-container">
        <h1>📋 Каталог</h1>

        <!-- Верхняя панель: переключатели -->
        <div class="toolbar">
            <div class="radio-group">
                <label class="radio-label">
                    <input type="radio" value="kit" v-model="catalogType" />
                    Набор
                </label>
                <label class="radio-label">
                    <input type="radio" value="scheme" v-model="catalogType" />
                    Схема
                </label>
                <label class="radio-label">
                    <input type="radio" value="thread" v-model="catalogType" />
                    Нить
                </label>
            </div>

            <div class="radio-group">
                <label class="radio-label" :class="{ disabled: catalogType === 'kit' }">
                    <input type="radio" value="pnk" v-model="brandType" :disabled="catalogType === 'kit'" />
                    ПНК
                </label>
                <label class="radio-label" :class="{ disabled: catalogType === 'kit' }">
                    <input type="radio" value="dmc" v-model="brandType" :disabled="catalogType === 'kit'" />
                    DMC
                </label>
            </div>
        </div>

        <!-- Поиск -->
        <div class="search-bar">
            <input type="text" v-model="searchQuery" placeholder="🔍 Поиск по номеру или названию..."
                class="search-input" />
        </div>

        <!-- Основной контент -->
        <div class="content">
            <!-- Список -->
            <div class="list-panel">
                <div class="panel-header">
                    <span>{{ catalogType === 'kit' ? 'Наборы' : catalogType === 'scheme' ? 'Схемы' : 'Нити' }}</span>
                    <span class="count">{{ filteredItems.length }}</span>
                </div>

                <div v-if="loading" class="loading">Загрузка...</div>
                <div v-else-if="error" class="error">{{ error }}</div>

                <div v-else class="list-items">
                    <div v-for="item in filteredItems" :key="item.id" class="list-item"
                        :class="{ active: selectedItem?.id === item.id }" @click="selectItem(item)">
                        <span class="item-code">{{ item.internalCode || item.code }}</span>
                        <span class="item-name">{{ item.name || item.name }}</span>
                    </div>
                    <div v-if="filteredItems.length === 0" class="empty-list">
                        Нет элементов
                    </div>
                </div>
            </div>

            <!-- Детали -->
            <div class="detail-panel">
                <div class="panel-header">
                    <span>Состав</span>
                    <span v-if="selectedItem" class="item-title">
                        {{ selectedItem.internalCode || selectedItem.code }}
                        {{ selectedItem.name || selectedItem.name }}
                    </span>
                </div>

                <div v-if="!selectedItem" class="empty-state">
                    Выберите элемент из списка
                </div>

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

                <!-- НАБОР: Значок, Код, Название, Метраж, Бирочки -->
                <div v-else-if="catalogType === 'kit'" class="detail-content">
                    <table class="detail-table">
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

                <!-- СХЕМА: Код, Название, только ненулевые каунты -->
                <div v-else-if="catalogType === 'scheme'" class="detail-content">
                    <table class="detail-table">
                        <thead>
                            <tr>
                                <th>Код</th>
                                <th>Название</th>
                                <th v-for="count in schemeCounts" :key="count">
                                    {{ count }}
                                </th>
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
        </div>
    </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { catalogApi } from '../api'

// Состояние
const catalogType = ref('kit')
const brandType = ref('dmc')
const searchQuery = ref('')
const items = ref([])
const selectedItem = ref(null)
const loading = ref(false)
const error = ref(null)

// Каунты, которые есть в текущей схеме (только непустые)
const schemeCounts = computed(() => {
    if (!selectedItem.value) return []

    const counts = new Set()
    selectedItem.value.compositions?.forEach(comp => {
        [252, 272, 283, 282, 302, 322, 362, 401].forEach(count => {
            const key = 'count' + count
            if (comp[key] > 0) counts.add(count)
        })
    })
    return Array.from(counts).sort((a, b) => a - b)
})

// Вычисляемые свойства
const filteredItems = computed(() => {
    if (!searchQuery.value.trim()) return items.value

    const query = searchQuery.value.toLowerCase().trim()
    return items.value.filter(item => {
        const code = (item.internalCode || item.code || '').toLowerCase()
        const name = (item.name || '').toLowerCase()
        return code.includes(query) || name.includes(query)
    })
})

// Методы
const loadData = async () => {
    loading.value = true
    error.value = null
    selectedItem.value = null

    try {
        if (catalogType.value === 'thread') {
            const response = await catalogApi.getThreads(brandType.value)
            items.value = response.data
        } else {
            const type = catalogType.value === 'kit' ? 'kit' : 'scheme'
            const response = await catalogApi.getKits(type)
            items.value = response.data
        }
    } catch (err) {
        console.error('Ошибка загрузки:', err)
        error.value = 'Ошибка при загрузке данных'
    } finally {
        loading.value = false
    }
}

const selectItem = (item) => {
    selectedItem.value = item
}

// Слежение за изменениями
watch(catalogType, (newVal) => {
    if (newVal === 'kit') {
        brandType.value = 'dmc'
    }
    loadData()
})

watch(brandType, (newVal, oldVal) => {
    if (catalogType.value === 'thread' && newVal !== oldVal) {
        loadData()
    }
})

// Инициализация
onMounted(() => {
    loadData()
})
</script>

<style scoped>
.catalog-container {
    padding: 2rem;
    max-width: 1400px;
    margin: 0 auto;
}

h1 {
    color: #2c3e50;
    margin-bottom: 1.5rem;
}

.toolbar {
    display: flex;
    gap: 2rem;
    margin-bottom: 1rem;
    flex-wrap: wrap;
}

.radio-group {
    display: flex;
    gap: 0.5rem;
    background: #f8f9fa;
    padding: 0.3rem;
    border-radius: 8px;
}

.radio-label {
    display: flex;
    align-items: center;
    gap: 0.3rem;
    padding: 0.3rem 0.8rem;
    border-radius: 6px;
    cursor: pointer;
    font-size: 0.9rem;
}

.radio-label:hover {
    background: #e9ecef;
}

.radio-label input[type="radio"] {
    margin: 0;
    cursor: pointer;
}

.radio-label.disabled {
    opacity: 0.5;
    cursor: not-allowed;
}

.search-bar {
    margin-bottom: 1rem;
}

.search-input {
    width: 100%;
    max-width: 400px;
    padding: 0.6rem 1rem;
    border: 2px solid #e0e0e0;
    border-radius: 8px;
    font-size: 0.95rem;
    transition: border-color 0.3s;
}

.search-input:focus {
    outline: none;
    border-color: #667eea;
}

.content {
    display: grid;
    grid-template-columns: 300px 1fr;
    gap: 1.5rem;
    min-height: 500px;
}

.list-panel,
.detail-panel {
    background: white;
    border-radius: 12px;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.08);
    overflow: hidden;
}

.panel-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0.8rem 1rem;
    background: #f8f9fa;
    border-bottom: 1px solid #e0e0e0;
    font-weight: 600;
    color: #2c3e50;
}

.count {
    background: #667eea;
    color: white;
    padding: 0.1rem 0.6rem;
    border-radius: 12px;
    font-size: 0.8rem;
}

.list-items {
    max-height: 600px;
    overflow-y: auto;
}

.list-item {
    display: flex;
    gap: 0.8rem;
    padding: 0.6rem 1rem;
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
    min-width: 60px;
}

.item-name {
    color: #555;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
}

.detail-panel {
    display: flex;
    flex-direction: column;
}

.item-title {
    font-weight: 600;
    color: #667eea;
}

.empty-state,
.empty-list {
    display: flex;
    justify-content: center;
    align-items: center;
    flex: 1;
    color: #999;
    font-size: 0.95rem;
    padding: 2rem;
}

.detail-content {
    padding: 1rem;
    overflow-x: auto;
    flex: 1;
}

.detail-table {
    width: 100%;
    border-collapse: collapse;
    font-size: 0.9rem;
}

.detail-table th {
    background: #f8f9fa;
    padding: 0.5rem 0.8rem;
    text-align: left;
    font-weight: 600;
    color: #2c3e50;
    border-bottom: 2px solid #e0e0e0;
    position: sticky;
    top: 0;
    z-index: 1;
}

.detail-table td {
    padding: 0.4rem 0.8rem;
    border-bottom: 1px solid #f0f0f0;
}

.detail-table tr:hover {
    background: #f8f9fa;
}

.icon-cell {
    font-size: inherit;
    text-align: left;
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

@media (max-width: 768px) {
    .content {
        grid-template-columns: 1fr;
    }

    .list-panel {
        max-height: 300px;
    }

    .toolbar {
        flex-direction: column;
        gap: 0.5rem;
    }
}
</style>