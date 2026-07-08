import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '../../../store/auth'
import api from '../../../api'
import { STATUS_LABELS, STATUS_CLASSES } from '../constants/statuses'

export function useTasks() {
    const authStore = useAuthStore()
    const isMaster = authStore.isMaster
    const currentUserId = ref(null)

    const tasks = ref([])
    const winders = ref([])
    const loading = ref(false)
    const error = ref(null)

    const filterStatus = ref('')
    const filterType = ref('')
    const filterWinder = ref('')

    const currentUserName = computed(() => {
        return authStore.user?.fullName || 'Пользователь'
    })

    const filteredTasks = computed(() => {
        let result = tasks.value

        if (filterStatus.value) {
        if (filterStatus.value === '!archived') {
            // Все кроме архива
            result = result.filter(t => t.status !== 'archived')
        } else {
            result = result.filter(t => t.status === filterStatus.value)
        }
    }

        if (filterType.value) {
            result = result.filter(t => t.itemType === filterType.value)
        }

        if (filterWinder.value && isMaster) {
            result = result.filter(t => t.winderId === parseInt(filterWinder.value))
        }

        return result
    })

    const loadTasks = async () => {
        loading.value = true
        error.value = null

        try {
            const response = await api.get('/Tasks')
            tasks.value = response.data
        } catch (err) {
            console.error('Ошибка загрузки заданий:', err)
            error.value = 'Ошибка при загрузке заданий'
        } finally {
            loading.value = false
        }
    }

    const loadWinders = async () => {
        // ✅ Только мастер загружает список мотальщиков
        if (!isMaster) return

        try {
            const response = await api.get('/Reference/users')
            winders.value = response.data.filter(u => u.role === 'winder')
        } catch (err) {
            console.error('Ошибка загрузки мотальщиков:', err)
        }
    }

    const getStatusClass = (status) => {
        return STATUS_CLASSES[status] || ''
    }

    const formatDate = (dateString) => {
        if (!dateString) return '-'
        const date = new Date(dateString)
        return date.toLocaleDateString('ru-RU') + ' ' + date.toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' })
    }

    const init = async () => {
        try {
            const userInfo = await api.get('/Auth/me')
            currentUserId.value = userInfo.data.id
        } catch {
            currentUserId.value = authStore.user?.id || null
        }

        await loadWinders()
        await loadTasks()
    }

    return {
        // Состояние
        tasks,
        winders,
        loading,
        error,
        filterStatus,
        filterType,
        filterWinder,
        selectedIds: ref([]),
        currentUserId,
        isMaster,
        currentUserName,

        // Вычисляемые
        filteredTasks,

        // Методы
        loadTasks,
        loadWinders,
        getStatusClass,
        formatDate,
        init
    }
}