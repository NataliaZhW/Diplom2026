import { ref, computed } from 'vue'
import api from '../../../api'
import { STATUS_FLOW, STATUS_LABELS } from '../constants/statuses'

export function useTaskActions(tasks, currentUserId, isMaster, loadTasks) {
    const selectedIds = ref([])
    const contextMenuVisible = ref(false)
    const contextMenuX = ref(0)
    const contextMenuY = ref(0)
    const contextMenuTaskId = ref(null)

    // Модалка подтверждения
    const confirmModalVisible = ref(false)
    const confirmTask = ref(null)
    const confirmAction = ref(null)

    // Модалка деталей задания
    const detailModalVisible = ref(false)
    const detailTask = ref(null)

    const canChangeStatus = (task) => {
        if (isMaster) return false
        if (task.winderId !== currentUserId.value) return false
        const nextStatuses = STATUS_FLOW[task.status] || []
        return nextStatuses.length > 0
    }

    const getNextStatusLabel = (task) => {
        const nextStatuses = STATUS_FLOW[task.status] || []
        if (nextStatuses.length === 0) return ''
        const nextKey = nextStatuses[0]
        return STATUS_LABELS[nextKey] || nextKey
    }

    const canCalculate = computed(() => {
        if (selectedIds.value.length === 0) return false
        const selectedTasks = tasks.value.filter(t => selectedIds.value.includes(t.id))
        return selectedTasks.every(t => t.status === 'new')
    })

    const canReport = computed(() => {
        if (selectedIds.value.length === 0) return false
        const selectedTasks = tasks.value.filter(t => selectedIds.value.includes(t.id))
        return selectedTasks.every(t => t.status === 'submitted')
    })

    const showStatusConfirm = (task) => {
        confirmTask.value = task
        confirmAction.value = 'status'
        confirmModalVisible.value = true
    }

    const confirmStatusChange = async () => {
        if (!confirmTask.value) return

        const task = confirmTask.value
        const nextStatuses = STATUS_FLOW[task.status] || []
        if (nextStatuses.length === 0) return

        const newStatus = nextStatuses[0]

        try {
            await api.put(`/Tasks/${task.id}/status`, { status: newStatus })

            task.status = newStatus
            const now = new Date().toISOString()
            switch (newStatus) {
                case 'materials_received':
                    task.materialsIssuedAt = now
                    break
                case 'submitted':
                    task.SubmittedAt = now
                    break
                case 'reported':
                    task.ReportedAt = now
                    break
                case 'archived':
                    task.ArchivedAt = now
                    break
            }

            confirmModalVisible.value = false
            confirmTask.value = null

            await loadTasks()
        } catch (err) {
            console.error('Ошибка обновления статуса:', err)
            alert('❌ Ошибка при обновлении статуса: ' + (err.response?.data?.message || err.message))
        }
    }

    const cancelConfirm = () => {
        confirmModalVisible.value = false
        confirmTask.value = null
        confirmAction.value = null
    }

    const acceptTask = async (task) => {
        try {
            const response = await api.post(`/Tasks/${task.id}/accept`)
            task.acceptedAt = response.data.acceptedAt
            alert('✅ Задание принято')
        } catch (err) {
            alert('❌ ' + (err.response?.data?.message || 'Ошибка'))
        }
    }

    const cancelAccept = async () => {
        if (!contextMenuTaskId.value) return

        try {
            await api.post(`/Tasks/${contextMenuTaskId.value}/cancel-accept`)
            const task = tasks.value.find(t => t.id === contextMenuTaskId.value)
            if (task) {
                task.acceptedAt = null
            }
            alert('✅ Принятие отменено')
        } catch (err) {
            alert('❌ ' + (err.response?.data?.message || 'Ошибка'))
        } finally {
            closeContextMenu()
        }
    }

    const calculateMaterials = async () => {
        if (selectedIds.value.length === 0) {
            alert('Выберите задания в статусе "Новое"')
            return
        }

        try {
            const response = await api.post('/Tasks/batch/calculate-materials', selectedIds.value)
            alert(`✅ ${response.data.message}`)
            selectedIds.value = []
            await loadTasks()
        } catch (err) {
            alert('❌ ' + (err.response?.data?.message || 'Ошибка'))
        }
    }

    const submitReport = async () => {
        if (selectedIds.value.length === 0) {
            alert('Выберите задания в статусе "Сдано"')
            return
        }

        try {
            const response = await api.post('/Tasks/batch/submit-report', selectedIds.value)
            alert(`✅ ${response.data.message}`)
            selectedIds.value = []
            await loadTasks()
        } catch (err) {
            alert('❌ ' + (err.response?.data?.message || 'Ошибка'))
        }
    }

    const showContextMenu = (event, taskId) => {
        contextMenuX.value = event.clientX
        contextMenuY.value = event.clientY
        contextMenuTaskId.value = taskId
        contextMenuVisible.value = true
    }

    const closeContextMenu = () => {
        contextMenuVisible.value = false
        contextMenuTaskId.value = null
    }

    // ============================================================
    // МОДАЛКА ДЕТАЛЕЙ ЗАДАНИЯ
    // ============================================================

    const openDetailModal = (task) => {
        console.log('🔍 openDetailModal вызван!', task)
        detailTask.value = task
        detailModalVisible.value = true
    }

    const closeDetailModal = () => {
        detailModalVisible.value = false
        detailTask.value = null
    }

    // ============================================================

    return {
        // Выбранные ID
        selectedIds,

        // Контекстное меню
        contextMenuVisible,
        contextMenuX,
        contextMenuY,
        contextMenuTaskId,

        // Модалка подтверждения
        confirmModalVisible,
        confirmTask,
        confirmAction,

        // Модалка деталей
        detailModalVisible,
        detailTask,

        // Функции
        canChangeStatus,
        getNextStatusLabel,
        canCalculate,
        canReport,
        showStatusConfirm,
        confirmStatusChange,
        cancelConfirm,
        acceptTask,
        cancelAccept,
        calculateMaterials,
        submitReport,
        showContextMenu,
        closeContextMenu,
        openDetailModal,
        closeDetailModal
    }
}