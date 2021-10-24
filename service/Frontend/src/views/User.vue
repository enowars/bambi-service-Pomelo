<template>
  <div v-if="this.employee">
    <h2>{{ this.employee.name }}</h2>

    <table>
      <thead>
        <tr>
          <td>Begin</td>
          <td>Reserve</td>
          <td v-for="employeeProjectHours in this.employee.employeeProjectHours" :key="employeeProjectHours.employeeId + '_' + employeeProjectHours.projectId">
            {{ this.getProjectName(employeeProjectHours.projectId) }}
          </td>
        </tr>
      </thead>
      <tbody>
        <tr v-for="week in this.weeks" :key="week.begin">
          <td>{{ week.begin }}</td>
          <td>0</td>
          <td v-for="weeklyProjectCapacity in week.capacities" :key="weeklyProjectCapacity.projectId" v-on:change="onInput(weeklyProjectCapacity)">
            <!-- @input="event => onInput(event, weeklyProjectCapacity.projectId, weeklyProjectCapacity.start)" -->
            <!-- <div contenteditable="true">{{ weeklyProjectCapacity.hours }}</div> -->
            <input type="number" v-model="weeklyProjectCapacity.capacity">
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script lang="ts">
import { getEmployee, getProjectDepartmentProjects, postWeeklyProjectCapacity, Project, EmployeeProjectWeeklyCapacityDto, EmployeeDetails } from '@/services/pomeloAPI'
import { addDays, getLastMonday } from '@/util'
import { defineComponent, inject } from 'vue'
import { useRoute } from 'vue-router'

interface Week {
  begin: string,
  reserve: 0,
  capacities: EmployeeProjectWeeklyCapacityDto[]
}

export default defineComponent({
  name: 'User',
  data() {
    return {
      employeeId: Number(useRoute().params.employeeId),
      employee: null as EmployeeDetails | null,
      projects: [] as Project[],
      weeks: [] as Week[]
    }
  },
  created() {
    this.init()
  },
  methods: {
    async onInput(capacity: EmployeeProjectWeeklyCapacityDto) {
      await postWeeklyProjectCapacity(this.employeeId, capacity.projectId, capacity.start, capacity.capacity)
    },
    async init() {
      this.employee = await getEmployee(this.employeeId)
      this.projects = await getProjectDepartmentProjects()
      var begin = new Date()
      begin.setUTCHours(0, 0, 0, 0)
      begin = getLastMonday(begin)

      var test = new Date()
      test.setUTCHours(0, 0, 0, 0)
      test = getLastMonday(begin)
      for (var x = 0; x < 6; x++) {
        console.log(test.toUTCString())
        test = addDays(test, 7)
      }

      for (var i = 0; i < 12; i++) {
        // build a week
        var week = {
          calendarWeek: 0,
          begin: begin.toUTCString(),
          reserve: 0,
          capacities: []
        } as Week
        for (var p in this.employee.employeeProjectHours) {
          const plannedHours = this.employee.employeeProjectHours[p]
          const upstreamCapacity = this.employee.employeeProjectWeeklyCapacities
            .find(wpc => wpc.projectId === plannedHours.projectId && new Date(Date.parse(wpc.start + 'Z')).getTime() === begin.getTime())
          if (upstreamCapacity) {
            week.capacities.push(upstreamCapacity)
          } else {
            week.capacities.push({
              employeeId: this.employeeId,
              projectId: plannedHours.projectId,
              start: begin.toUTCString(),
              capacity: 0
            } as EmployeeProjectWeeklyCapacityDto)
          }
        }
        this.weeks.push(week)
        begin = addDays(begin, 7)
      }
      console.log(this.weeks)
    },
    getProjectName(projectId: number) : string {
      const project = this.projects.find(p => p.id === projectId)
      if (project) {
        return project.name
      } else {
        console.log(project)
      }
      return 'UNKNOWN'
    }
  }
})
</script>
