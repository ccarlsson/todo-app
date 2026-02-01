import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/authStore'
import LoginPage from '../pages/LoginPage.vue'
import RegisterPage from '../pages/RegisterPage.vue'
import TodoListPage from '../pages/TodoListPage.vue'
import TodoEditPage from '../pages/TodoEditPage.vue'

const routes = [
  { path: '/', redirect: '/todos' },
  { path: '/login', component: LoginPage },
  { path: '/register', component: RegisterPage },
  { path: '/todos', component: TodoListPage, meta: { requiresAuth: true } },
  { path: '/todos/new', component: TodoEditPage, meta: { requiresAuth: true } },
  { path: '/todos/:id', component: TodoEditPage, meta: { requiresAuth: true } },
]

export const router = createRouter({
  history: createWebHistory(),
  routes,
})

router.beforeEach((to) => {
  const authStore = useAuthStore()
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    return { path: '/login', query: { redirect: to.fullPath } }
  }
  if ((to.path === '/login' || to.path === '/register') && authStore.isAuthenticated) {
    return { path: '/todos' }
  }
  return true
})
