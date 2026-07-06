<template>
    <div class="catalog-container">
        <h1>📋 Каталог</h1>

        <CatalogToolbar :catalog-type="catalogType" :brand-type="brandType" @update:catalogType="catalogType = $event"
            @update:brandType="brandType = $event" />

        <CatalogSearch :search-query="searchQuery" @update:searchQuery="searchQuery = $event" />

        <div class="content">
            <ItemList :title="catalogType === 'kit' ? 'Наборы' : catalogType === 'scheme' ? 'Схемы' : 'Нити'"
                :items="filteredItems" :selected-item="selectedItem" :loading="loading" :error="error"
                @select="handleSelectItem" />

            <ItemDetail :selected-item="selectedItem" :catalog-type="catalogType" :brand-type="brandType"
                :scheme-counts="schemeCounts" />

            <SelectedPanel :selected-items="selectedItems" :selected-item="selectedItem" :catalog-type="catalogType"
                :scheme-counts="schemeCounts" :brand-type="brandType" :current-user-role="currentUserRole"
                :current-user-name="currentUserName" :winders="winders" :selected-winder-id="selectedWinderId"
                :selected-quantity="selectedQuantity" :selected-count="selectedCount" @add="handleAddSelected"
                @submit="handleSubmit" @delete="deleteSelected" @edit-quantity="updateQuantity"
                @update:selectedQuantity="selectedQuantity = $event" @update:selectedCount="selectedCount = $event"
                @update:selectedWinderId="selectedWinderId = $event" />
        </div>
    </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import { useCatalog } from './composables/useCatalog'
import { useSelected } from './composables/useSelected'
import { referenceApi } from '../../api'

import CatalogToolbar from './components/CatalogToolbar.vue'
import CatalogSearch from './components/CatalogSearch.vue'
import ItemList from './components/ItemList.vue'
import ItemDetail from './components/ItemDetail.vue'
import SelectedPanel from './components/SelectedPanel.vue'

// ============================================================
// КОМПОЗАБЛЫ
// ============================================================

const {
    catalogType,
    brandType,
    searchQuery,
    items,
    selectedItem,
    loading,
    error,
    schemeCounts,
    filteredItems,
    loadData,
    selectItem
} = useCatalog()

const {
    selectedItems,
    selectedQuantity,
    selectedCount,
    addSelected,
    deleteSelected,
    updateQuantity,
    submitSelected
} = useSelected()

// ============================================================
// ТЕКУЩИЙ ПОЛЬЗОВАТЕЛЬ
// ============================================================

const currentUserId = ref(parseInt(localStorage.getItem('userId') || '0'))
const currentUserRole = ref(localStorage.getItem('userRole') || 'winder')
const currentUserName = ref(localStorage.getItem('userName') || 'Пользователь')

// Мотальщики
const winders = ref([])
const selectedWinderId = ref(null)

// ============================================================
// СЛЕДИМ ЗА СМЕНОЙ МОТАЛЬЩИКА
// ============================================================

watch(selectedWinderId, (newVal) => {
    // Обновляем winderId у всех выбранных элементов
    selectedItems.value.forEach(item => {
        item.winderId = newVal
    })
})

// ============================================================
// МЕТОДЫ
// ============================================================

const loadWinders = async () => {
    try {
        const response = await referenceApi.getUsers()
        winders.value = response.data.filter(u => u.role === 'winder')
        if (currentUserRole.value === 'master' && winders.value.length > 0) {
            selectedWinderId.value = winders.value[0].id
        }
    } catch (err) {
        console.error('Ошибка загрузки мотальщиков:', err)
    }
}

const handleSubmit = async () => {
    const result = await submitSelected(currentUserRole.value, currentUserId.value)
    if (result) {
        // При успешном внесении можно обновить что-то
    }
}

// ============================================================
// ОБЁРТКА ДЛЯ selectItem
// ============================================================

const handleSelectItem = (item) => {
    selectItem(item, (count) => {
        selectedCount.value = count
    })
}

// ============================================================
// ОБЁРТКА ДЛЯ addSelected С ЛОГОМ
// ============================================================

const handleAddSelected = () => {
    console.log('=== Catalog: handleAddSelected вызван ===')
    console.log('selectedItem:', selectedItem.value)
    console.log('selectedQuantity:', selectedQuantity.value)
    console.log('selectedCount:', selectedCount.value)
    console.log('catalogType:', catalogType.value)
    console.log('brandType:', brandType.value)
    console.log('currentUserRole:', currentUserRole.value)
    console.log('currentUserId:', currentUserId.value)
    console.log('selectedWinderId:', selectedWinderId.value)

    // Вызываем оригинальный addSelected с параметрами
    addSelected(
        selectedItem.value,
        catalogType.value,
        brandType.value,
        schemeCounts.value,
        currentUserRole.value,
        currentUserId.value,
        selectedWinderId.value
    )

    console.log('=== После addSelected ===')
    console.log('selectedItems:', selectedItems.value)
}

// ============================================================
// ИНИЦИАЛИЗАЦИЯ
// ============================================================

onMounted(() => {
    loadData()
    loadWinders()
})
</script>

<style scoped>
.catalog-container {
    padding: 2rem;
    max-width: 1600px;
    margin: 0 auto;
}

h1 {
    color: #2c3e50;
    margin-bottom: 1.5rem;
}

.content {
    display: grid;
    grid-template-columns: 250px 1fr 380px;
    gap: 1.5rem;
    min-height: 500px;
}

@media (max-width: 1200px) {
    .content {
        grid-template-columns: 1fr 1fr;
    }
}

@media (max-width: 768px) {
    .content {
        grid-template-columns: 1fr;
    }
}
</style>