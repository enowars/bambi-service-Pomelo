<template>
  <div v-if="this.employee">
    <h2>{{ this.employee.name }}</h2>

    <table>
      <thead>
        <tr>
          <td>Year</td>
          <td>Cal. Week</td>
          <td>Begin</td>
          <td>Reserve</td>
          <td v-for="totalPlanning in this.employee.totalPlannings" :key="totalPlanning.id">
            {{ this.getProjectName(totalPlanning.projectId) }}
          </td>
        </tr>
      </thead>
      <tbody>
        <!--
        <tr v-for="hours in this.project.totalPlannings" :key="hours.id">
          <td>{{getName(hours.employeeId) }}</td>
          <td>{{ hours.totalHours }}</td>
          <td>{{ hours.performedHours }}</td>
          <td>0</td>
          <td>0</td>
        </tr>
        -->
      </tbody>
    </table>
  </div>
</template>

<script lang="ts">
import { Employee, getAccountData, getAccountInfo, getProjectDepartmentProjects, Project } from '@/services/pomeloAPI'
import { defineComponent, inject } from 'vue'
import { useRoute } from 'vue-router'

export default defineComponent({
  name: 'User',
  data() {
    return {
      employeeId: Number(useRoute().params.projectId),
      employee: null as Employee | null,
      projects: [] as Project[]
    }
  },
  created() {
    this.init()
  },
  methods: {
    async init() {
      this.employee = await getAccountData()
      this.projects = await getProjectDepartmentProjects()
    },
    getProjectName(projectId: number) : string {
      const project = this.projects.find(p => p.id === projectId)
      if (project) {
        return project.name
      }
      return 'UNKNOWN'
    }
  }
})
</script>
