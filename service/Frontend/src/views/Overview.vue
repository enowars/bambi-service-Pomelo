<template>
  <h2> Overview</h2>
  <div>
    <table>
      <thead>
        <tr>
          <td>Name</td>
          <td>Begin</td>
          <td>End</td>
          <td>Absolute Deviation</td>
          <td>Relative Deviation</td>
        </tr>
      </thead>
      <tbody>
        <tr v-for="project in this.projects" :key="project.id">
          <td><router-link :to="{name: 'Project', params: { projectId: project.id }}">{{ project.name }}</router-link></td>
          <td>{{ project.begin }}</td>
          <td>{{ project.end }}</td>
          <td>0</td>
          <td>0</td>
        </tr>
      </tbody>
    </table>
  </div>
  <input v-model="projectName" placeholder="projectName">
  <button @click="this.createProject()">Create Project</button>
</template>

<script lang="ts">
import { getProjectDepartmentProjects, postProjectCreate, Project } from '@/services/pomeloAPI'
import { defineComponent } from 'vue'

export default defineComponent({
  name: 'Overview',
  created() {
    this.init()
  },
  data() {
    return {
      projectName: null as string | null,
      projects: [] as Project[]
    }
  },
  methods: {
    async createProject() {
      if (this.projectName !== null) {
        await postProjectCreate(this.projectName, new Date(), new Date())
        await this.init()
      }
    },
    async init() {
      this.projects = await getProjectDepartmentProjects()
    }
  }
})
</script>
