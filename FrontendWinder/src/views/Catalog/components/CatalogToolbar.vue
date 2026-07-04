<template>
    <div class="toolbar">
        <div class="radio-group">
            <label class="radio-label">
                <input type="radio" value="kit" :checked="catalogType === 'kit'"
                    @change="$emit('update:catalogType', 'kit')" />
                Набор
            </label>
            <label class="radio-label">
                <input type="radio" value="scheme" :checked="catalogType === 'scheme'"
                    @change="$emit('update:catalogType', 'scheme')" />
                Схема
            </label>
            <label class="radio-label">
                <input type="radio" value="thread" :checked="catalogType === 'thread'"
                    @change="$emit('update:catalogType', 'thread')" />
                Нить
            </label>
        </div>

        <div class="radio-group">
            <label class="radio-label" :class="{ disabled: catalogType === 'kit' }">
                <input type="radio" value="pnk" :checked="brandType === 'pnk'" :disabled="catalogType === 'kit'"
                    @change="$emit('update:brandType', 'pnk')" />
                ПНК
            </label>
            <label class="radio-label" :class="{ disabled: catalogType === 'kit' }">
                <input type="radio" value="dmc" :checked="brandType === 'dmc'" :disabled="catalogType === 'kit'"
                    @change="$emit('update:brandType', 'dmc')" />
                DMC
            </label>
        </div>
    </div>
</template>

<script setup>
defineProps({
    catalogType: String,
    brandType: String
})

defineEmits(['update:catalogType', 'update:brandType'])
</script>

<style scoped>
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
</style>