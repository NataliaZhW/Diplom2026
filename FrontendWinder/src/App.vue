<template>
  <div id="app">
    <h1>Управление справочниками</h1>
    <nav>
      <button @click="activeTab = 'brands'">Бренды</button>
      <button @click="activeTab = 'colors'">Цвета нитей</button>
    </nav>
    
    <div v-if="activeTab === 'brands'">
      <BrandManufList />
    </div>
    
    <div v-if="activeTab === 'colors'">
      <ColorThreadList />
    </div>
  </div>
</template>

<script>
import BrandManufList from './components/BrandManufList.vue';
import ColorThreadList from './components/ColorThreadList.vue';

export default {
  name: 'App',
  components: {
    BrandManufList,
    ColorThreadList,
  },
  data() {
    return {
      activeTab: 'brands',
    };
  },
};
</script>

<style>
#app {
  font-family: Arial, sans-serif;
  padding: 20px;
}

nav {
  margin-bottom: 20px;
}

nav button {
  margin-right: 10px;
  padding: 10px;
  cursor: pointer;
}

table {
  margin-top: 20px;
  border-collapse: collapse;
}

th, td {
  padding: 8px;
  text-align: left;
}

button {
  margin: 0 5px;
  padding: 5px 10px;
  cursor: pointer;
}
</style>