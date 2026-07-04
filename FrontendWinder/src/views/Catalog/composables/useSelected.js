import { ref } from 'vue'
import { tasksApi } from '../../../api'

export function useSelected() {
    const selectedItems = ref([])
    const selectedQuantity = ref(1)
    const selectedCount = ref(null)

    const addSelected = (item, catalogType, brandType, schemeCounts, currentUserRole, currentUserId, selectedWinderId) => {
        console.log('=== useSelected: addSelected START ===')
        console.log('item:', item)
        console.log('catalogType:', catalogType)
        console.log('brandType:', brandType)
        console.log('currentUserRole:', currentUserRole)
        console.log('currentUserId:', currentUserId)
        console.log('selectedWinderId:', selectedWinderId)

        if (!item) {
            console.log('❌ item is null')
            return
        }

        const typeLabels = {
            kit: 'Набор',
            scheme: 'Схема',
            thread: 'Нить'
        }

        // Определяем brandLabel
        let brandLabel = '-'
        if (catalogType === 'thread') {
            const value = brandType === 'pnk' ? item.pnk : item.dmc
            if (value === 'С' || value === 'Б') {
                brandLabel = brandType === 'pnk' ? 'ПНК' : 'DMC'
            } else if (value === 'П') {
                brandLabel = 'ПЕРЛЕ'
            } else if (value === 'М') {
                brandLabel = 'Металлик'
            } else {
                brandLabel = value || '-'
            }
        } else if (catalogType === 'scheme') {
            brandLabel = brandType === 'pnk' ? 'ПНК' : 'DMC'
        }

        // Определяем winderId
        let winderId
        if (currentUserRole === 'master') {
            winderId = selectedWinderId
        } else {
            winderId = currentUserId
        }

        console.log('brandLabel:', brandLabel)
        console.log('winderId:', winderId)

        const newItem = {
            type: catalogType,
            typeLabel: typeLabels[catalogType] || catalogType,
            id: item.id,
            code: item.internalCode || item.code,
            name: item.name,
            quantity: selectedQuantity.value || 1,
            count: catalogType === 'scheme' ? selectedCount.value : null,
            brand: catalogType === 'thread' ? brandType : null,
            brandLabel: brandLabel,
            winderId: winderId
        }

        console.log('newItem:', newItem)

        // Проверка на дубликат
        const existingIndex = selectedItems.value.findIndex(existing => {
            if (existing.type !== newItem.type) return false
            if (existing.id !== newItem.id) return false

            if (newItem.type === 'scheme') {
                return existing.count === newItem.count && existing.brandLabel === newItem.brandLabel
            }

            if (newItem.type === 'thread') {
                return existing.brand === newItem.brand
            }

            return true
        })

        console.log('existingIndex:', existingIndex)

        if (existingIndex !== -1) {
            selectedItems.value[existingIndex].quantity += newItem.quantity
            console.log('✅ Обновлён существующий элемент')
        } else {
            selectedItems.value.push(newItem)
            console.log('✅ Добавлен новый элемент')
        }

        console.log('selectedItems.value:', selectedItems.value)
        console.log('=== useSelected: addSelected END ===')

        selectedQuantity.value = 1
    }

    const deleteSelected = (index) => {
        selectedItems.value.splice(index, 1)
    }

    const updateQuantity = (index, newQuantity) => {
        console.log('=== updateQuantity ===')
        console.log('index:', index)
        console.log('newQuantity:', newQuantity)

        if (newQuantity > 0 && selectedItems.value[index]) {
            selectedItems.value[index].quantity = newQuantity
            console.log('✅ Количество обновлено:', selectedItems.value[index].quantity)
        } else {
            console.log('❌ Ошибка: индекс или количество некорректны')
        }
    }

    const clearSelected = () => {
        selectedItems.value = []
    }

    const submitSelected = async (currentUserRole, currentUserId) => {
        if (selectedItems.value.length === 0) {
            alert('Список пуст. Добавьте элементы.')
            return false
        }

        if (currentUserRole !== 'master') {
            const hasOtherWinder = selectedItems.value.some(item => item.winderId !== currentUserId)
            if (hasOtherWinder) {
                alert('Вы можете создавать задания только для себя!')
                return false
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
            alert(`✅ ${response.data.message}`)
            selectedItems.value = []
            return true
        } catch (err) {
            console.error('Ошибка при создании заданий:', err)
            alert('❌ Ошибка при создании заданий: ' + (err.response?.data?.message || err.message))
            return false
        }
    }

    return {
        selectedItems,
        selectedQuantity,
        selectedCount,
        addSelected,
        deleteSelected,
        updateQuantity,
        clearSelected,
        submitSelected
    }
}