<template>
  <div v-if="this.project">
    <h2>{{ this.project.name }}</h2>

    <table>
      <thead>
        <tr>
          <td>Name</td>
          <td>Planned Hours</td>
          <td>Performed Hours</td>
          <td>Absolute Deviation</td>
          <td>Relative Deviation</td>
        </tr>
      </thead>
      <tbody>
        <tr v-for="hours in this.project.totalPlannings" :key="hours.id">
          <td>{{ hours.employeeId }}</td>
          <td>{{ hours.totalHours }}</td>
          <td>{{ hours.performedHours }}</td>
          <td>0</td>
          <td>0</td>
        </tr>
      </tbody>
    </table>

    <div v-if="this.uninvolvedEmployees">
      <h4>Plan hours</h4>
      <select v-model="this.newEmployee">
        <option v-for="uninvolvedEmployee in this.uninvolvedEmployees" v-bind:value="{ id: uninvolvedEmployee.id }" :key="uninvolvedEmployee.id">
          {{ uninvolvedEmployee.name }}
        </option>
      </select>
      <input type="number" v-model="newHours" placeholder="planned hours">
      <button @click="this.add()">Add</button>
    </div>
  </div>
</template>

<script lang="ts">
import { Project, getProjectDetails, Employee, getAccountDepartment, postProjectTotalPlanning } from '@/services/pomeloAPI'
import { defineComponent } from 'vue'
import { useRoute } from 'vue-router'

export default defineComponent({
  name: 'Project',
  data() {
    return {
      projectId: Number(useRoute().params.projectId),
      project: null as Project | null,
      uninvolvedEmployees: [] as Employee[],
      plannedHours: null as number | null,
      newEmployee: null as Employee | null,
      newHours: null as number | null
    }
  },
  created() {
    this.init()
  },
  methods: {
    async init() {
      this.project = await getProjectDetails(this.projectId)
      const departmentEmployees = await getAccountDepartment()
      this.uninvolvedEmployees = departmentEmployees
    },
    async add() {
      if (this.newEmployee != null && this.newHours != null) {
        console.log(this.newEmployee)
        await postProjectTotalPlanning(this.newEmployee.id, this.projectId, this.newHours)
        this.project = await getProjectDetails(this.projectId)
      } else {
        console.log('invalid arguments')
        console.log(this.newEmployee)
      }
    }
  }
})
</script>
