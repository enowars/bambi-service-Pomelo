<template>
  <div v-if="this.project" style="width=80%;">
    <h2>{{ this.project.name }}</h2>

    <table>
      <thead>
        <tr>
          <td>Name</td>
          <td>Total Hours</td>
          <td>Performed Hours</td>
          <td>Absolute Deviation</td>
          <td>Relative Deviation</td>
        </tr>
      </thead>
      <tbody>
        <tr v-for="hours in this.project.employeeProjectHours" :key="hours.id">
          <!--<td><router-link :to="{name: 'User', params: { employeeId: hours.employeeId }}">{{ getName(hours.employeeId) }}</router-link></td>-->
          <td><router-link :to="{name: 'EmployeePage', params: { employeeId: hours.employeeId }}">{{ this.getName(hours.employeeId) }}</router-link></td>
          <td>{{ hours.totalHours }}</td>
          <td>{{ hours.deliveredHours }}</td>
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
      <input type="number" v-model="newHours" placeholder="total hours">
      <button @click="this.add()">Add</button>
    </div>

    <div name="visualization" style="height: 400px;">
      <apexchart height="100%" v-if="this.burnDownChartData" type="line" :options="this.burnDownChartData.options" :series="this.burnDownChartData.series"></apexchart>
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
      departmentEmployees: [] as Employee[],
      uninvolvedEmployees: [] as Employee[],
      eph: null as number | null,
      newEmployee: null as Employee | null,
      newHours: null as number | null,
      burnDownChartData: null as any
    }
  },
  created() {
    this.init()
  },
  methods: {
    async init() {
      this.project = await getProjectDetails(this.projectId)
      this.departmentEmployees = await getAccountDepartment()
      this.uninvolvedEmployees = this.departmentEmployees
      this.updateBurnDownChart(this.project)
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
    },
    getName(employeeId: number) {
      const employee = this.departmentEmployees.find(e => e.id === employeeId)
      if (employee) {
        return employee.name
      }
      return 'UNKNOWN'
    },
    addDays(date: Date, days: number) : Date {
      var newDate = new Date(date)
      newDate.setDate(newDate.getDate() + days)
      return newDate
    },
    getWorkingDays(begin: Date, end: Date) : number { // TODO: Take vacation into account
      let days = 0
      begin.setUTCHours(0, 0, 0, 0) // TODO throw if these are no utc days
      end.setUTCHours(0, 0, 0, 0)
      let t = begin

      while (t.getTime() <= end.getTime()) {
        const day = t.getUTCDay()
        if (day > 0 && day < 6) {
          days += 1
        }
        t = this.addDays(t, 1)
      }
      return days
    },
    getMonday(d: Date) : Date { // https://stackoverflow.com/questions/4156434/javascript-get-the-first-day-of-the-week-from-current-date
      var day = d.getDay()
      var diff = d.getDate() - day + (day === 0 ? -6 : 1) // adjust when day is sunday
      return new Date(d.setDate(diff))
    },
    updateBurnDownChart(project: Project) {
      // The burndown chart starts at the last booking.
      // Bookings may happen at any time, so the begun week needs special handling.
      const today = new Date()
      today.setUTCHours(0, 0, 0, 0)
      const totalProjectHours = project.employeeProjectHours.reduce((sum, eph) => sum + eph.totalHours, 0)
      const deliveredProjectHours = project.employeeProjectHours.reduce((sum, eph) => sum + eph.deliveredHours, 0)
      const projectBegin = new Date(Date.parse(project.begin + 'Z'))
      const projectEnd = new Date(Date.parse(project.end + 'Z'))
      const totalWorkingDays = this.getWorkingDays(projectBegin, projectEnd)
      const remainingWorkingDays = this.getWorkingDays(today, projectEnd)
      const remainingQuota = remainingWorkingDays / totalWorkingDays
      const allocatedHours = []

      var weekBegin = new Date()
      weekBegin = this.getMonday(weekBegin)
      weekBegin.setUTCHours(0, 0, 0, 0)

      var projectAllocatedHours = deliveredProjectHours
      for (var i = 0; i < 12; i++) {
        console.log(`handling week ${weekBegin}`)
        allocatedHours.push([
          weekBegin.toString(),
          projectAllocatedHours
        ])
        var pwc = project.employeeProjectWeeklyCapacities
          .filter(wpc => new Date(Date.parse(wpc.start + 'Z')).getTime() === weekBegin.getTime())[0]

        if (!pwc) {
          throw Error('möp')
        }
        weekBegin = this.addDays(weekBegin, 7)
        projectAllocatedHours -= (pwc.capacity / 100) * 40
      }
      // console.log(allocatedHours)
      // console.log(`today: ${today} passedQuota: ${remainingQuota}`)
      console.log(`project end: ${project!.end}`)
      this.burnDownChartData = {
        series: [
          {
            name: 'Total Hours',
            data: [
              [today.toString(), Math.round(totalProjectHours * remainingQuota)],
              [project!.end, 0]
            ]
          },
          {
            name: 'Allocated Hours',
            data: allocatedHours
          }
        ],
        options: {
          chart: {
            id: 'vuechart-example',
            redrawOnParentResize: true,
            toolbar: {
              show: false
            },
            zoom: {
              enabled: false
            },
            animations: {
              enabled: false
            }
          },
          xaxis: {
            type: 'datetime',
            min: today.toString(),
            tickAmount: 12
          },
          tooltip: {
            enabled: true
          }
        }
      }
      console.log(this.burnDownChartData)
    }
  }
})
</script>
