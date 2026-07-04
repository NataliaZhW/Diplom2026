<template>
    <div class="add-form">
        <div class="form-row">
            <label>Количество</label>
            <input type="number" :value="selectedQuantity"
                @input="$emit('update:selectedQuantity', parseInt($event.target.value) || 1)" min="1"
                class="form-input quantity-input" />
        </div>

        <div class="form-row" v-if="catalogType === 'scheme'">
            <label>Каунт</label>
            <select :value="selectedCount" @change="$emit('update:selectedCount', parseInt($event.target.value))"
                class="form-input count-select">
                <option v-for="count in schemeCounts" :key="count" :value="count">
                    {{ count }}
                </option>
            </select>
        </div>

        <div class="form-row" v-if="currentUserRole === 'master'">
            <label>Мотальщик</label>
            <select :value="selectedWinderId" @change="$emit('update:selectedWinderId', parseInt($event.target.value))"
                class="form-input winder-select">
                <option v-for="user in winders" :key="user.id" :value="user.id">
                    {{ user.fullName }}
                </option>
            </select>
        </div>
        <div class="form-row" v-else>
            <label>Мотальщик</label>
            <span class="winder-name">{{ currentUserName }}</span>
        </div>

        <button @click="handleAdd" class="add-btn">Добавить</button>
    </div>
</template>

<script setup>
const props = defineProps({
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
    'update:selectedQuantity',
    'update:selectedCount',
    'update:selectedWinderId'
])

const handleAdd = () => {
    console.log('=== AddForm: handleAdd вызван ===')
    emit('add')
}
</script>

<style scoped>
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

.quantity-input {
    width: 80px;
}

.count-select {
    width: 70px;
    cursor: pointer;
}

.winder-select {
    width: 140px;
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
</style>