<template>
    <div class="icons-container">
        <h1>🎨 Значки</h1>
        <p class="subtitle">Все доступные значки для маркировки</p>

        <div v-if="loading" class="loading">Загрузка...</div>
        <div v-else-if="error" class="error">{{ error }}</div>

        <div v-else class="icons-list">
            <div v-for="icon in icons" :key="icon" class="icon-item">
                <span class="icon-symbol">{{ icon }}</span>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { catalogApi } from '../api'

const icons = ref([])
const loading = ref(true)
const error = ref(null)

onMounted(async () => {
    try {
        const response = await catalogApi.getIcons()
        icons.value = response.data
    } catch (err) {
        console.error('Ошибка загрузки значков:', err)
        error.value = 'Ошибка при загрузке значков'
    } finally {
        loading.value = false
    }
})
</script>

<style scoped>
.icons-container {
    padding: 2rem;
    max-width: 600px;
    margin: 0 auto;
}

h1 {
    color: #2c3e50;
    margin-bottom: 0.25rem;
}

.subtitle {
    color: #7f8c8d;
    margin-bottom: 1.5rem;
}

.icons-list {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
    background: white;
    border-radius: 12px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
    padding: 0.5rem;
}

.icon-item {
    padding: 0.6rem 1rem;
    border-bottom: 1px solid #f0f0f0;
    transition: background 0.2s;
    border-radius: 4px;
}

.icon-item:hover {
    background: #f8f9fa;
}

.icon-item:last-child {
    border-bottom: none;
}

.icon-symbol {
    font-size: 1.3rem;
}

.loading,
.error {
    text-align: center;
    padding: 2rem;
    color: #7f8c8d;
}

.error {
    color: #e74c3c;
}
</style>