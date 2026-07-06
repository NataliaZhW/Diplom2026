import { ref, computed, watch } from 'vue'
import { catalogApi } from '../../../api'
import { BUSINESS_RULES } from '../../../config'

export function useCatalog() {
    const catalogType = ref('kit')
    const brandType = ref('dmc')
    const searchQuery = ref('')
    const items = ref([])
    const selectedItem = ref(null)
    const loading = ref(false)
    const error = ref(null)

    const schemeCounts = computed(() => {
        if (!selectedItem.value || catalogType.value !== 'scheme') return []
        const counts = new Set()
        const allCounts = BUSINESS_RULES.AVAILABLE_COUNTS
        selectedItem.value.compositions?.forEach(comp => {
            allCounts.forEach(count => {
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

    watch(catalogType, (newVal) => {
        if (newVal === 'kit') brandType.value = 'dmc'
        loadData()
    })

    watch(brandType, (newVal, oldVal) => {
        if (catalogType.value === 'thread' && newVal !== oldVal) {
            loadData()
        }
    })

    return {
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
    }
}