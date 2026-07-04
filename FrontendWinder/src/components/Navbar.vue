<template>
    <nav class="navbar">
        <div class="navbar-brand">
            <router-link to="/colors">Diplom_Winder</router-link>
        </div>
        <div class="navbar-menu">
            <router-link to="/colors" class="nav-link">Цвета</router-link>
            <router-link to="/brands" class="nav-link">Бренды</router-link>
            <router-link to="/catalog" class="nav-link">Каталог</router-link>
            <router-link to="/icons" class="nav-link">Значки</router-link>
            <router-link to="/tasks" class="nav-link">Задания</router-link>
            <router-link v-if="authStore.isMaster" to="/users" class="nav-link">
                Пользователи
            </router-link>
        </div>
        <div class="navbar-end">
            <span class="user-name">{{ authStore.user?.fullName || 'Пользователь' }}</span>
            <button @click="logout" class="logout-btn">Выйти</button>
        </div>
    </nav>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { useAuthStore } from '../store/auth'

const router = useRouter()
const authStore = useAuthStore()

const logout = () => {
    authStore.logout()
    router.push('/login')
}
</script>

<style scoped>
.navbar {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 1rem 2rem;
    background-color: #2c3e50;
    color: white;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.navbar-brand a {
    color: white;
    text-decoration: none;
    font-size: 1.5rem;
    font-weight: bold;
}

.navbar-menu {
    display: flex;
    gap: 2rem;
}

.nav-link {
    color: rgba(255, 255, 255, 0.7);
    text-decoration: none;
    padding: 0.5rem 0;
    transition: color 0.3s;
    border-bottom: 2px solid transparent;
}

.nav-link:hover {
    color: white;
}

.nav-link.router-link-active {
    color: white;
    border-bottom-color: #3498db;
}

.navbar-end {
    display: flex;
    align-items: center;
    gap: 1rem;
}

.user-name {
    color: rgba(255, 255, 255, 0.8);
    font-size: 0.9rem;
}

.logout-btn {
    background-color: #e74c3c;
    color: white;
    border: none;
    padding: 0.4rem 1rem;
    border-radius: 4px;
    cursor: pointer;
    transition: background-color 0.3s;
}

.logout-btn:hover {
    background-color: #c0392b;
}
</style>