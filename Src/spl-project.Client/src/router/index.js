import { createRouter, createWebHistory } from 'vue-router'

import LoginView from '../views/LoginView.vue'
import DashboardView from '../views/DashboardView.vue'

const routes = [
  {
    path: '/',
    redirect: { name: 'LoginView' }
  },
  {
    path: '/login',
    name: 'LoginView',
    component: LoginView
  },
  {
    path: '/dashboard',
    name: 'DashboardView',
    component: DashboardView,
    meta: {
      requiresAuth: true
    }
  }
]


const router = createRouter({
  history: createWebHistory(),
  routes
})

function isTokenExpired(token) {
  try {
    const payload = JSON.parse(atob(token.split('.')[1]))

    return payload.exp * 1000 <= Date.now()
  }
  catch {
    return true
  }
}

router.beforeEach((to) => {
  const token = localStorage.getItem('token')

  /*
   * 1. User is trying to access Dashboard
   */
  if (to.meta.requiresAuth) {

    // No token
    if (!token) {
      return { name: 'LoginView' }
    }

    // Token expired
    if (isTokenExpired(token)) {
      localStorage.removeItem('token')

      return { name: 'LoginView' }
    }

    // Token is valid
    return true
  }

  /*
   * 2. User is trying to access Login
   */
  if (to.name === 'LoginView') {

    // No token → allow login page
    if (!token) {
      return true
    }

    // Token expired → remove it and allow login page
    if (isTokenExpired(token)) {
      localStorage.removeItem('token')

      return true
    }

    // Token is valid → go to Dashboard
    return { name: 'DashboardView' }
  }

  /*
   * 3. Any other public route
   */
  return true
})

export default router
