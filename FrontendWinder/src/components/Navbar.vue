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

            <!-- ✅ Используем authStore.isMaster для проверки -->
            <router-link v-if="authStore.isMaster" to="/users" class="nav-link">
                Пользователи
            </router-link>
        </div>

        <div class="navbar-end">
            <!-- ✅ Используем authStore.user для отображения имени -->
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
    flex-wrap: wrap;
    justify-content: space-between;
    align-items: center;
    padding: 0.8rem 2rem;
    background: #2c3e50;
    color: white;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    gap: 0.5rem 1rem;
}

.navbar-brand a {
    color: white;
    text-decoration: none;
    font-size: 1.5rem;
    font-weight: bold;
    white-space: nowrap;
}

.navbar-menu {
    display: flex;
    flex-wrap: wrap;
    gap: 1.5rem;
    align-items: center;
    flex: 1 1 auto;
}

.nav-link {
    color: rgba(255, 255, 255, 0.7);
    text-decoration: none;
    padding: 0.4rem 0;
    transition: color 0.3s;
    border-bottom: 2px solid transparent;
    white-space: nowrap;
    font-size: 0.95rem;
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
    flex-wrap: wrap;
    margin-left: auto;
}

.user-name {
    color: rgba(255, 255, 255, 0.8);
    font-size: 0.9rem;
    white-space: nowrap;
}

.logout-btn {
    background-color: #e74c3c;
    color: white;
    border: none;
    padding: 0.4rem 1rem;
    border-radius: 4px;
    cursor: pointer;
    transition: background-color 0.3s;
    font-size: 0.9rem;
    white-space: nowrap;
}

.logout-btn:hover {
    background-color: #c0392b;
}

/* ============================================================
   АДАПТИВНОСТЬ
   ============================================================ */

@media (max-width: 768px) {
    .navbar {
        flex-direction: column;
        align-items: stretch;
        padding: 0.8rem 1rem;
    }

    .navbar-brand {
        text-align: center;
        margin-bottom: 0.3rem;
    }

    .navbar-menu {
        justify-content: center;
        gap: 0.8rem 1.2rem;
    }

    .nav-link {
        font-size: 0.85rem;
    }

    .navbar-end {
        justify-content: center;
        margin-left: 0;
        margin-top: 0.3rem;
        gap: 0.8rem;
    }

    .user-name {
        font-size: 0.8rem;
    }

    .logout-btn {
        font-size: 0.8rem;
        padding: 0.3rem 0.8rem;
    }
}

@media (max-width: 480px) {
    .navbar-menu {
        flex-wrap: wrap;
        justify-content: center;
        gap: 0.4rem 0.8rem;
    }

    .nav-link {
        font-size: 0.75rem;
        padding: 0.2rem 0.3rem;
    }

    .navbar-brand a {
        font-size: 1.2rem;
    }
}
</style>