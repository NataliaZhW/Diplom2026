import { createRouter, createWebHistory } from 'vue-router'
import Login from '../views/Login.vue'
import Colors from '../views/Colors.vue'
import Brands from '../views/Brands.vue'
import Users from '../views/Users.vue'

const routes = [
    {    
        path: '/login',
        name: 'Login',
        component: Login,
        meta: { requiresAuth: false }
    },
    {
        path: '/',
        redirect: '/colors'
    },
    {
        path: '/colors',
        name: 'Colors',
        component: Colors,
        meta: { requiresAuth: true }
    },
    {
        path: '/brands',
        name: 'Brands',
        component: Brands,
        meta: { requiresAuth: true }
    },
    {
        path: '/users',
        name: 'Users',
        component: Users,
        meta: { requiresAuth: true, requiresMaster: true }
    }
]

const router = createRouter({
    history: createWebHistory(),
    routes
})

// Guard: проверка авторизации
router.beforeEach((to, from, next) => {
    const token = localStorage.getItem('token')
    const userRole = localStorage.getItem('userRole')

    if (to.meta.requiresAuth && !token) {
        next('/login')
    } else if (to.meta.requiresMaster && userRole !== 'master') {
        next('/colors')
    } else {
        next()
    }
})

export default router