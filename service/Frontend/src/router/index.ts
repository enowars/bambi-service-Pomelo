import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router'
import Overview from '../views/Overview.vue'
import User from '../views/User.vue'
import Project from '../views/Project.vue'
import Register from '../views/Register.vue'

const routes: Array<RouteRecordRaw> = [
  {
    path: '/',
    name: 'Overview',
    component: Overview
  },
  {
    path: '/project/:projectId',
    name: 'Project',
    component: Project
  },
  {
    path: '/employee/:employeeId',
    name: 'EmployeePage',
    component: User
  },
  {
    path: '/register',
    name: 'Register',
    component: Register
  }
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

export default router
