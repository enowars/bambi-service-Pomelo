<template>
  <h2> Overview</h2>
  <div>
    Welcome to Pomelo, the PrOject MangEment pLanning platfOrm!
    <br>
    <br>
    <div style="color: red;">
      You NEED information from the <a href="https://bambi6.enoflag.de/api/attackinfo">attack info endpoint</a> to exploit this service.
    </div>
    Flags are being put into user notes, project names, and booking files.
    There are 3 intended security issues.
    The OpenAPI interface of this service can be found <a href="swagger">here</a>.
    Good luck!
  </div>
  <div>
    <div>
      <h4>Update your User Note</h4>
      <input v-model="note" placeholder="secret note">
      <button @click="this.updateNote()">Update</button>
    </div>

    <h4>Projects</h4>
    <table>
      <thead>
        <tr>
          <td>Name</td>
          <td>Begin</td>
          <td>End</td>
        </tr>
      </thead>
      <tbody>
        <tr v-for="project in this.projects" :key="project.id">
          <td><router-link :to="{name: 'Project', params: { projectId: project.id }}">{{ project.name }}</router-link></td>
          <td>{{ project.begin }}</td>
          <td>{{ project.end }}</td>
        </tr>
      </tbody>
    </table>
  </div>
  <input v-model="projectName" placeholder="projectName">
  <input v-model="startDate" type="date">
  <input v-model="endDate" type="date">
  <button @click="this.createProject()">Create Project</button>
</template>

<script lang="ts">
import { getProjectDepartmentProjects, postProjectCreate, Project, postNote } from '@/services/pomeloAPI'
import { defineComponent } from 'vue'

export default defineComponent({
  name: 'Overview',
  created() {
    this.init()
  },
  data() {
    return {
      projectName: null as string | null,
      projects: [] as Project[],
      startDate: null as string | null,
      endDate: null as string | null,
      note: null as string | null
    }
  },
  methods: {
    async createProject() {
      if (this.projectName !== null && this.startDate !== null && this.endDate !== null) {
        await postProjectCreate(this.projectName, new Date(Date.parse(this.startDate)), new Date(Date.parse(this.endDate)))
        await this.init()
      }
    },
    async init() {
      this.projects = await getProjectDepartmentProjects()
    },
    async updateNote() {
      if (this.note) {
        await postNote(this.note)
        this.note = null
      }
    }
  }
})
</script>
