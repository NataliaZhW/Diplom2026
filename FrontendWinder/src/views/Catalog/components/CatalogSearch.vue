<template>
    <div class="search-bar">
        <input type="text" 
        :value="searchQuery" 
        @input="$emit('update:searchQuery', $event.target.value)"
        placeholder="🔍 Поиск по номеру или названию..." 
        class="search-input" />
    </div>
</template>

<script setup>
defineProps({
    searchQuery: String
})

defineEmits(['update:searchQuery'])
</script>

<style scoped>
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
</style>