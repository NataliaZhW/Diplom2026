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

        <!-- Основной контент: 3 колонки -->
        <div class="content">
            <!-- Колонка 1: Список -->
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

            <!-- Колонка 2: Состав -->
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

                <!-- НАБОР -->
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

                <!-- СХЕМА -->
                <div v-else-if="catalogType === 'scheme'" class="detail-content">
                    <table class="scheme-table">
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

            <!-- Колонка 3: Выбрано (шире) -->
            <div class="selected-panel">
                <!-- Форма добавления -->
                <div v-if="selectedItem" class="add-form">
                    <div class="form-row">
                        <label>Количество</label>
                        <input type="number" v-model.number="selectedQuantity" min="1"
                            class="form-input quantity-input" />
                    </div>

                    <div class="form-row" v-if="catalogType === 'scheme'">
                        <label>Каунт</label>
                        <select v-model="selectedCount" class="form-input count-select">
                            <option v-for="count in schemeCounts" :key="count" :value="count">
                                {{ count }}
                            </option>
                        </select>
                    </div>

                    <!-- ВЫБОР МОТАЛЬЩИКА (только для мастера) -->
                    <div class="form-row" v-if="currentUserRole === 'master'">
                        <label>Мотальщик</label>
                        <select v-model="selectedWinderId" class="form-input winder-select">
                            <option v-for="user in winders" :key="user.id" :value="user.id">
                                {{ user.fullName }}
                            </option>
                        </select>
                    </div>
                    <!-- Для мотальщика показываем его имя -->
                    <div class="form-row" v-else>
                        <label>Мотальщик</label>
                        <span class="winder-name">{{ currentUserName }}</span>
                    </div>

                    <button @click="addSelected" class="add-btn">Добавить</button>
                </div>
                <div v-else class="empty-state-small">
                    Выберите элемент для добавления
                </div>

                <!-- Заголовок "Выбрано" (ПОД ФОРМОЙ) -->
                <div class="panel-header selected-header">
                    <span>Выбрано</span>
                    <span class="count">{{ selectedItems.length }}</span>
                </div>

                <!-- Таблица выбранных -->
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
                            <tr v-for="(item, index) in selectedItems" :key="index"
                                @contextmenu.prevent="showContextMenu($event, index)">
                                <td>{{ item.typeLabel }}</td>
                                <td>{{ item.code }}</td>
                                <td>{{ item.name }}</td>
                                <td>{{ item.brandLabel || '-' }}</td>
                                <td>{{ item.count || '-' }}</td>
                                <td>{{ item.quantity }}</td>
                            </tr>
                            <tr v-if="selectedItems.length === 0">
                                <td colspan="6" class="empty-row">Список пуст</td>
                            </tr>
                        </tbody>
                    </table>
                </div>

                <!-- Кнопка Внести -->
                <div class="submit-row">
                    <button @click="submitSelected" class="submit-btn">Внести</button>
                </div>

                <!-- Контекстное меню -->
                <div v-if="contextMenuVisible" class="context-menu"
                    :style="{ top: contextMenuY + 'px', left: contextMenuX + 'px' }" @click.stop>
                    <div class="context-item" @click="editQuantity">✏️ Изменить количество</div>
                    <div class="context-item danger" @click="deleteSelected">🗑️ Удалить</div>
                    <div class="context-item" @click="closeContextMenu">❌ Отмена</div>
                </div>

                <!-- Модалка изменения количества -->
                <div v-if="editModalVisible" class="modal-overlay" @click.self="closeEditModal">
                    <div class="modal-content">
                        <h3>Изменить количество</h3>
                        <input type="number" v-model.number="editQuantityValue" min="1"
                            class="form-input modal-input" />
                        <div class="modal-actions">
                            <button @click="saveEditQuantity" class="save-btn">Сохранить</button>
                            <button @click="closeEditModal" class="cancel-btn">Отмена</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { catalogApi, tasksApi, referenceApi } from '../api'

// ============================================================
// СОСТОЯНИЕ
// ============================================================

const catalogType = ref('kit')
const brandType = ref('dmc')
const searchQuery = ref('')
const items = ref([])
const selectedItem = ref(null)
const loading = ref(false)
const error = ref(null)

// Выбранные элементы
const selectedItems = ref([])
const selectedQuantity = ref(1)
const selectedCount = ref(null)

// Контекстное меню
const contextMenuVisible = ref(false)
const contextMenuX = ref(0)
const contextMenuY = ref(0)
const contextMenuIndex = ref(null)

// Модалка изменения количества
const editModalVisible = ref(false)
const editQuantityValue = ref(1)
const editModalIndex = ref(null)

// ============================================================
// ТЕКУЩИЙ ПОЛЬЗОВАТЕЛЬ
// ============================================================

const currentUserId = ref(parseInt(localStorage.getItem('userId') || '0'))
const currentUserRole = ref(localStorage.getItem('userRole') || 'winder')
const currentUserName = ref(localStorage.getItem('userName') || 'Пользователь')

console.log('=== ТЕКУЩИЙ ПОЛЬЗОВАТЕЛЬ ===')
console.log('Role:', currentUserRole.value)
console.log('Id:', currentUserId.value)
console.log('Name:', currentUserName.value)

// Мотальщики (для мастера)
const winders = ref([])
const selectedWinderId = ref(null)

// ============================================================
// ВЫЧИСЛЯЕМЫЕ СВОЙСТВА
// ============================================================

const schemeCounts = computed(() => {
    if (!selectedItem.value || catalogType.value !== 'scheme') return []

    const counts = new Set()
    selectedItem.value.compositions?.forEach(comp => {
        [252, 272, 283, 282, 302, 322, 362, 401].forEach(count => {
            const key = 'count' + count
            if (comp[key] > 0) counts.add(count)
        })
    })
    return Array.from(counts).sort((a, b) => a - b)
})

const filteredItems = computed(() => {
    if (!searchQuery.value.trim()) return items.value

    const query = searchQuery.value.toLowerCase().trim()
    return items.value.filter(item => {
        const code = (item.internalCode || item.code || '').toLowerCase()
        const name = (item.name || '').toLowerCase()
        return code.includes(query) || name.includes(query)
    })
})

// ============================================================
// МЕТОДЫ
// ============================================================

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
    selectedQuantity.value = 1
    if (schemeCounts.value.length > 0) {
        selectedCount.value = schemeCounts.value[0]
    }
}

// ============================================================
// ЗАГРУЗКА МОТАЛЬЩИКОВ
// ============================================================

const loadWinders = async () => {
    try {
        const response = await referenceApi.getUsers()
        winders.value = response.data.filter(u => u.role === 'winder')
        
        console.log('Загружено мотальщиков:', winders.value.length)
        
        // Для мастера: выбираем первого мотальщика
        if (currentUserRole.value === 'master' && winders.value.length > 0) {
            selectedWinderId.value = winders.value[0].id
            console.log('Выбран мотальщик:', selectedWinderId.value)
        }
    } catch (err) {
        console.error('Ошибка загрузки мотальщиков:', err)
    }
}

// ============================================================
// РАБОТА С ВЫБРАННЫМИ
// ============================================================

const addSelected = () => {
    console.log('=== addSelected ===')
    console.log('currentUserRole:', currentUserRole.value)
    console.log('currentUserId:', currentUserId.value)

    if (!selectedItem.value) return

    const typeLabels = {
        kit: 'Набор',
        scheme: 'Схема',
        thread: 'Нить'
    }

    let brandLabel = '-'

    if (catalogType.value === 'thread') {
        const value = brandType.value === 'pnk' ? selectedItem.value.pnk : selectedItem.value.dmc

        if (value === 'С' || value === 'Б') {
            brandLabel = brandType.value === 'pnk' ? 'ПНК' : 'DMC'
        } else if (value === 'П') {
            brandLabel = 'ПЕРЛЕ'
        } else if (value === 'М') {
            brandLabel = 'Металлик'
        } else {
            brandLabel = value || '-'
        }
    } else if (catalogType.value === 'scheme') {
        brandLabel = brandType.value === 'pnk' ? 'ПНК' : 'DMC'
    }

    // ============================================================
    // ОПРЕДЕЛЯЕМ КОМУ НАЗНАЧИТЬ
    // ============================================================
    let winderId
    if (currentUserRole.value === 'master') {
        winderId = selectedWinderId.value
    } else {
        winderId = currentUserId.value
    }

    const newItem = {
        type: catalogType.value,
        typeLabel: typeLabels[catalogType.value],
        id: selectedItem.value.id,
        code: selectedItem.value.internalCode || selectedItem.value.code,
        name: selectedItem.value.name,
        quantity: selectedQuantity.value || 1,
        count: catalogType.value === 'scheme' ? selectedCount.value : null,
        brand: catalogType.value === 'thread' ? brandType.value : null,
        brandLabel: brandLabel,
        winderId: winderId
    }

    // Проверка на дубликат
    const existingIndex = selectedItems.value.findIndex(item => {
        if (item.type !== newItem.type) return false
        if (item.id !== newItem.id) return false

        if (newItem.type === 'scheme') {
            return item.count === newItem.count && item.brandLabel === newItem.brandLabel
        }

        if (newItem.type === 'thread') {
            return item.brand === newItem.brand
        }

        return true
    })

    if (existingIndex !== -1) {
        selectedItems.value[existingIndex].quantity += newItem.quantity
    } else {
        selectedItems.value.push(newItem)
    }

    selectedQuantity.value = 1
}

const showContextMenu = (event, index) => {
    contextMenuX.value = event.clientX
    contextMenuY.value = event.clientY
    contextMenuIndex.value = index
    contextMenuVisible.value = true
}

const closeContextMenu = () => {
    contextMenuVisible.value = false
    contextMenuIndex.value = null
}

const deleteSelected = () => {
    if (contextMenuIndex.value !== null) {
        selectedItems.value.splice(contextMenuIndex.value, 1)
        closeContextMenu()
    }
}

const editQuantity = () => {
    if (contextMenuIndex.value !== null) {
        editModalIndex.value = contextMenuIndex.value
        editQuantityValue.value = selectedItems.value[contextMenuIndex.value].quantity
        editModalVisible.value = true
        closeContextMenu()
    }
}

const saveEditQuantity = () => {
    if (editModalIndex.value !== null && editQuantityValue.value > 0) {
        selectedItems.value[editModalIndex.value].quantity = editQuantityValue.value
        closeEditModal()
    }
}

const closeEditModal = () => {
    editModalVisible.value = false
    editModalIndex.value = null
}

// ============================================================
// ВНЕСТИ ВЫБРАННЫЕ ЭЛЕМЕНТЫ
// ============================================================

const submitSelected = async () => {
    if (selectedItems.value.length === 0) {
        alert('Список пуст. Добавьте элементы.')
        return
    }

    console.log('=== submitSelected ===')
    console.log('Роль пользователя:', currentUserRole.value)
    console.log('Выбранные элементы:', selectedItems.value)

    // ============================================================
    // ПРОВЕРКА ПРАВ
    // ============================================================
    if (currentUserRole.value !== 'master') {
        const hasOtherWinder = selectedItems.value.some(item => item.winderId !== currentUserId.value)
        if (hasOtherWinder) {
            alert('Вы можете создавать задания только для себя!')
            return
        }
    }

    const tasksData = selectedItems.value.map(item => ({
        itemType: item.type,
        itemId: item.id,
        itemCode: item.code,
        itemName: item.name,
        brandLabel: item.brandLabel || null,
        countValue: item.count || null,
        quantity: item.quantity,
        winderId: item.winderId,
        note: null
    }))

    try {
        const response = await tasksApi.createBatchTasks(tasksData)
        console.log('Задания созданы:', response.data)

        alert(`✅ ${response.data.message}`)
        selectedItems.value = []
    } catch (err) {
        console.error('Ошибка при создании заданий:', err)
        alert('❌ Ошибка при создании заданий: ' + (err.response?.data?.message || err.message))
    }
}

// Закрытие контекстного меню по клику вне
document.addEventListener('click', () => {
    if (contextMenuVisible.value) {
        closeContextMenu()
    }
})

// ============================================================
// СЛЕЖЕНИЕ ЗА ИЗМЕНЕНИЯМИ
// ============================================================

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

// ============================================================
// ИНИЦИАЛИЗАЦИЯ
// ============================================================

onMounted(() => {
    loadData()
    loadWinders()
})
</script>

<style scoped>
/* ============================================================
    ОБЩИЕ СТИЛИ
   ============================================================ */

.winder-select {
    width: 120px;
    cursor: pointer;
}

.winder-name {
    font-size: 0.85rem;
    color: #333;
    font-weight: 500;
    padding: 0.2rem 0.5rem;
    background: #f0f0f0;
    border-radius: 4px;
}
    
.catalog-container {
    padding: 2rem;
    max-width: 1600px;
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
/* ============================================================
    3 КОЛОНКИ — колонка "Выбрано" шире
============================================================ */
    
.content {
    display: grid;
    grid-template-columns: 250px 1fr 380px;
    gap: 1.5rem;
    min-height: 500px;
}

.list-panel,
.detail-panel,
.selected-panel {
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

/* ============================================================
    КОЛОНКА 1: СПИСОК
   ============================================================ */

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

/* ============================================================
    КОЛОНКА 2: СОСТАВ
   ============================================================ */

.detail-panel {
    flex: 1;
}

.detail-content {
    padding: 0.8rem;
    overflow-x: auto;
    flex: 1;
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

.item-title {
    font-weight: 600;
    color: #667eea;
}

/* ============================================================
    ТАБЛИЦЫ СОСТАВА
   ============================================================ */

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

/* ============================================================
    КОЛОНКА 3: ВЫБРАНО
   ============================================================ */

.selected-panel {
    min-width: 320px;
}

/* Форма добавления (СВЕРХУ) */
.add-form {
    padding: 0.8rem 1rem;
    background: #f8f9fa;
    border-bottom: 1px solid #e0e0e0;
    display: flex;
    flex-wrap: wrap;
    align-items: center;
    gap: 0.8rem;
    flex-shrink: 0;
}

.form-row {
    display: flex;
    align-items: center;
    gap: 0.4rem;
}

.form-row label {
    font-size: 0.85rem;
    color: #555;
    font-weight: 500;
}

.form-input {
    padding: 0.3rem 0.5rem;
    border: 1px solid #ddd;
    border-radius: 4px;
    font-size: 0.9rem;
}

/* Количество — шире */
.quantity-input {
    width: 80px;
}

.count-select {
    width: 70px;
    cursor: pointer;
}

.add-btn {
    padding: 0.3rem 1rem;
    background: #28a745;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-size: 0.85rem;
    font-weight: 500;
    transition: background 0.2s;
    margin-left: auto;
}

.add-btn:hover {
    background: #218838;
}

.empty-state-small {
    padding: 1rem;
    text-align: center;
    color: #999;
    font-size: 0.85rem;
    flex-shrink: 0;
}

/* Заголовок "Выбрано" (ПОД ФОРМОЙ) */
.selected-header {
    border-top: 1px solid #e0e0e0;
    flex-shrink: 0;
}

/* Таблица выбранных */
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

/* Кнопка Внести */
.submit-row {
    padding: 0.6rem 1rem;
    border-top: 1px solid #e0e0e0;
    background: #f8f9fa;
    flex-shrink: 0;
}

.submit-btn {
    width: 100%;
    padding: 0.5rem;
    background: #667eea;
    color: white;
    border: none;
    border-radius: 6px;
    font-size: 0.95rem;
    font-weight: 600;
    cursor: pointer;
    transition: background 0.2s;
}

.submit-btn:hover {
    background: #5a67d8;
}

/* ============================================================
    КОНТЕКСТНОЕ МЕНЮ
   ============================================================ */

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

/* ============================================================
    МОДАЛКА
============================================================ */

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

/* ============================================================
    ЗАГРУЗКА / ОШИБКА
   ============================================================ */

.loading,
.error {
    padding: 2rem;
    text-align: center;
    color: #7f8c8d;
}

.error {
    color: #e74c3c;
}

/* ============================================================
    АДАПТИВНОСТЬ
============================================================ */

@media (max-width: 1200px) {
    .content {
        grid-template-columns: 1fr 1fr;
    }

    .selected-panel {
        grid-column: span 2;
    }
}

@media (max-width: 768px) {
    .content {
        grid-template-columns: 1fr;
    }

    .selected-panel {
        grid-column: span 1;
    }

    .add-form {
        flex-direction: column;
        align-items: stretch;
    }

    .form-row {
        justify-content: space-between;
    }

    .add-btn {
        margin-left: 0;
    }
}
</style>