<template>
    <div class="selected-panel">
        <AddForm v-if="selectedItem" :selectedItem="selectedItem" :catalogType="catalogType"
            :schemeCounts="schemeCounts" :brandType="brandType" :currentUserRole="currentUserRole"
            :currentUserName="currentUserName" :winders="winders" :selectedWinderId="selectedWinderId"
            :selectedQuantity="selectedQuantity" :selectedCount="selectedCount" @add="handleAddFromForm"
            @update:selectedQuantity="val => $emit('update:selectedQuantity', val)"
            @update:selectedCount="val => $emit('update:selectedCount', val)"
            @update:selectedWinderId="val => $emit('update:selectedWinderId', val)" />
        <div v-else class="empty-state-small">Выберите элемент для добавления</div>

        <div class="panel-header selected-header">
            <span>Выбрано</span>
            <span class="count">{{ selectedItems.length }}</span>
        </div>

        <SelectedTable :items="selectedItems" @contextmenu="onContextMenu" />

        <div class="submit-row">
            <button @click="$emit('submit')" class="submit-btn">Внести</button>
        </div>

        <ContextMenu v-if="contextMenuVisible" :x="contextMenuX" :y="contextMenuY" @edit="handleEdit"
            @delete="handleDelete" @close="closeContextMenu" />

        <EditModal v-if="editModalVisible" :value="editQuantityValue" @save="handleSaveEdit" @close="closeEditModal" />
    </div>
</template>

<script setup>
import { ref } from 'vue'
import AddForm from './AddForm.vue'
import SelectedTable from './SelectedTable.vue'
import ContextMenu from './ContextMenu.vue'
import EditModal from './EditModal.vue'

const props = defineProps({
    selectedItems: Array,
    selectedItem: Object,
    catalogType: String,
    schemeCounts: Array,
    brandType: String,
    currentUserRole: String,
    currentUserName: String,
    winders: Array,
    selectedWinderId: Number,
    selectedQuantity: Number,
    selectedCount: Number
})

const emit = defineEmits([
    'add',
    'submit',
    'delete',
    'editQuantity',
    'update:selectedQuantity',
    'update:selectedCount',
    'update:selectedWinderId'
])

// Контекстное меню
const contextMenuVisible = ref(false)
const contextMenuX = ref(0)
const contextMenuY = ref(0)
const contextMenuIndex = ref(null)

// Модалка
const editModalVisible = ref(false)
const editQuantityValue = ref(1)
const editModalIndex = ref(null)

// ============================================================
// ОБРАБОТЧИКИ СОБЫТИЙ
// ============================================================

const handleAddFromForm = () => {
    console.log('=== SelectedPanel: handleAddFromForm вызван ===')
    emit('add')
}

const onContextMenu = (event, index) => {
    contextMenuX.value = event.clientX
    contextMenuY.value = event.clientY
    contextMenuIndex.value = index
    contextMenuVisible.value = true
}

const closeContextMenu = () => {
    contextMenuVisible.value = false
    contextMenuIndex.value = null
}

const handleEdit = () => {
    console.log('=== handleEdit ===')
    console.log('contextMenuIndex:', contextMenuIndex.value)

    if (contextMenuIndex.value !== null) {
        editModalIndex.value = contextMenuIndex.value
        editQuantityValue.value = props.selectedItems[contextMenuIndex.value]?.quantity || 1
        editModalVisible.value = true
        console.log('editQuantityValue:', editQuantityValue.value)
        closeContextMenu()
    }
}

const handleDelete = () => {
    if (contextMenuIndex.value !== null) {
        emit('delete', contextMenuIndex.value)
        closeContextMenu()
    }
}

const handleSaveEdit = (newValue) => {
    console.log('=== handleSaveEdit ===')
    console.log('editModalIndex:', editModalIndex.value)
    console.log('newValue:', newValue)

    if (editModalIndex.value !== null && newValue > 0) {
        emit('editQuantity', editModalIndex.value, newValue)
        console.log('✅ Событие editQuantity отправлено')
        closeEditModal()
    } else {
        console.log('❌ Ошибка: индекс или значение некорректны')
    }
}

const closeEditModal = () => {
    editModalVisible.value = false
    editModalIndex.value = null
}

// Закрытие контекстного меню по клику вне
document.addEventListener('click', () => {
    if (contextMenuVisible.value) closeContextMenu()
})
</script>

<style scoped>
.selected-panel {
    background: white;
    border-radius: 12px;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.08);
    overflow: hidden;
    display: flex;
    flex-direction: column;
    min-width: 320px;
}

.empty-state-small {
    padding: 1rem;
    text-align: center;
    color: #999;
    font-size: 0.85rem;
    flex-shrink: 0;
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

.selected-header {
    border-top: 1px solid #e0e0e0;
}

.count {
    background: #667eea;
    color: white;
    padding: 0.1rem 0.6rem;
    border-radius: 12px;
    font-size: 0.8rem;
}

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
</style>