<template>
  <div v-if="this.employee">
    <h2>{{ this.employee.name }}</h2>

    <table>
      <thead>
        <tr>
          <td>Cal. Week</td>
          <td>Begin</td>
          <td>Reserve</td>
          <td v-for="plannedHours in this.employee.plannedHours" :key="plannedHours.id">
            {{ this.getProjectName(plannedHours.projectId) }}
          </td>
        </tr>
      </thead>
      <tbody>
        <tr v-for="week in this.weeks" :key="week.begin">
          <td>0</td>
          <td>{{ week.begin }}</td>
          <td>0</td>
          <td v-for="weeklyProjectCapacity in week.capacities" :key="weeklyProjectCapacity.projectId" contenteditable @input="event => onInput(event, weeklyProjectCapacity.projectId, weeklyProjectCapacity.start)">
            <div contenteditable="true">{{ weeklyProjectCapacity.hours }}</div>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script lang="ts">
import { Employee, getAccountData, getAccountInfo, getAccountUserData, getProjectDepartmentProjects, postWeeklyProjectCapacity, Project, WeeklyProjectCapacity } from '@/services/pomeloAPI'
import { defineComponent, inject } from 'vue'
import { useRoute } from 'vue-router'

interface Week {
  begin: string,
  reserve: 0,
  capacities: WeeklyProjectCapacity[]
}

export default defineComponent({
  name: 'User',
  data() {
    return {
      employeeId: Number(useRoute().params.employeeId),
      employee: null as Employee | null,
      projects: [] as Project[],
      weeks: [] as Week[]
    }
  },
  created() {
    this.init()
  },
  methods: {
    async onInput(event: any, projectId: number, start: string) {
      console.log('onInput')
      console.log(event)
      await postWeeklyProjectCapacity(this.employeeId, projectId, start, event.data)
    },
    async init() {
      this.employee = await getAccountUserData(this.employeeId)
      var begin = new Date()
      begin.setUTCHours(0, 0, 0, 0)
      begin.setDate(1)
      for (var i = 0; i < 12; i++) {
        // build a week
        var week = {
          calendarWeek: 0,
          begin: begin.toUTCString(),
          reserve: 0,
          capacities: []
        } as Week
        for (var p in this.employee.plannedHours) {
          const plannedHours = this.employee.plannedHours[p]
          console.log('handling project ' + plannedHours.projectId)
          const upstreamCapacity = this.employee.weeklyProjectCapacities
            .find(wpc => wpc.projectId === plannedHours.projectId && new Date(Date.parse(wpc.start + 'Z')).getTime() === begin.getTime())
          if (upstreamCapacity) {
            week.capacities.push(upstreamCapacity)
          } else {
            week.capacities.push({
              id: 0,
              employeeId: this.employeeId,
              projectId: plannedHours.projectId,
              start: begin.toUTCString(),
              hours: 0
            } as WeeklyProjectCapacity)
          }
        }
        console.log(week)
        this.weeks.push(week)
        begin = this.addDays(begin, 7)
      }
    },
    addDays(date: Date, days: number) : Date {
      var newDate = new Date(date)
      newDate.setDate(newDate.getDate() + days)
      return newDate
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
