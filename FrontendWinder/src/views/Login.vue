<template>
    <div class="login-container">
        <div class="login-card">
            <h1>Diplom_Winder</h1>
            <h2>Вход в систему</h2>

            <form @submit.prevent="handleLogin">
                <div class="form-group">
                    <label for="login">Логин</label>
                    <input id="login" v-model="login" type="text" placeholder="Введите логин" required />
                </div>

                <div class="form-group">
                    <label for="password">Пароль</label>
                    <input id="password" v-model="password" type="password" placeholder="Введите пароль" required />
                </div>

                <div v-if="errorMessage" class="error-message">
                    {{ errorMessage }}
                </div>

                <button type="submit" :disabled="loading">
                    {{ loading ? 'Вход...' : 'Войти' }}
                </button>
            </form>

            <div class="test-accounts">
                <p>Тестовые аккаунты:</p>
                <div class="account-grid">
                    <span>ivanov / password123</span>
                    <span>petrov / password123</span>
                    <span>master1 / password123</span>
                    <span>admin / password123</span>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../store/auth'

const router = useRouter()
const authStore = useAuthStore()
console.log(authStore.user?.role)

const login = ref('')
const password = ref('')
const loading = ref(false)
const errorMessage = ref('')

const handleLogin = async () => {
    console.log('=== handleLogin вызван ===')
    console.log('Логин:', login.value)
    console.log('Пароль:', password.value)

    if (!login.value || !password.value) {
        errorMessage.value = 'Введите логин и пароль'
        return
    }

    loading.value = true
    errorMessage.value = ''

    try {
        const result = await authStore.login(login.value, password.value)
        console.log('Результат входа:', result)

        if (result.success) {
            router.push('/colors')
        } else {
            errorMessage.value = result.message || 'Ошибка при входе'
        }
    } catch (err) {
        console.error('Исключение:', err)
        errorMessage.value = 'Ошибка соединения с сервером'
    }

    loading.value = false
}
</script>

<style scoped>
.login-container {
    display: flex;
    justify-content: center;
    align-items: center;
    min-height: 100vh;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.login-card {
    background: white;
    border-radius: 16px;
    padding: 2.5rem;
    width: 100%;
    max-width: 400px;
    box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
}

.login-card h1 {
    text-align: center;
    color: #2c3e50;
    margin-bottom: 0.25rem;
}

.login-card h2 {
    text-align: center;
    color: #7f8c8d;
    font-size: 1rem;
    font-weight: normal;
    margin-bottom: 2rem;
}

.form-group {
    margin-bottom: 1.25rem;
}

.form-group label {
    display: block;
    margin-bottom: 0.4rem;
    color: #2c3e50;
    font-weight: 500;
}

.form-group input {
    width: 100%;
    padding: 0.75rem;
    border: 2px solid #e0e0e0;
    border-radius: 8px;
    font-size: 1rem;
    transition: border-color 0.3s;
}

.form-group input:focus {
    outline: none;
    border-color: #667eea;
}

.error-message {
    color: #e74c3c;
    background: #fde8e8;
    padding: 0.75rem;
    border-radius: 8px;
    margin-bottom: 1rem;
    text-align: center;
}

button[type="submit"] {
    width: 100%;
    padding: 0.75rem;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
    border: none;
    border-radius: 8px;
    font-size: 1rem;
    font-weight: 600;
    cursor: pointer;
    transition: opacity 0.3s;
}

button[type="submit"]:hover:not(:disabled) {
    opacity: 0.9;
}

button[type="submit"]:disabled {
    opacity: 0.6;
    cursor: not-allowed;
}

.test-accounts {
    margin-top: 1.5rem;
    padding-top: 1.5rem;
    border-top: 1px solid #e0e0e0;
}

.test-accounts p {
    color: #7f8c8d;
    font-size: 0.85rem;
    margin-bottom: 0.5rem;
}

.account-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 0.25rem;
    font-size: 0.8rem;
    color: #555;
    background: #f8f9fa;
    padding: 0.5rem;
    border-radius: 8px;
}
</style>