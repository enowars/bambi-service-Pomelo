<template>
  <div v-if="this.project" style="width=80%;">
    <h2>{{ this.project.name }}</h2>

    <table>
      <thead>
        <tr>
          <td>Name</td>
          <td>Total Hours</td>
          <td>Performed Hours</td>
        </tr>
      </thead>
      <tbody>
        <tr v-for="hours in this.project.employeeProjectHours" :key="hours.employeeId + '_' + hours.projectId">
          <!--<td><router-link :to="{name: 'User', params: { employeeId: hours.employeeId }}">{{ getName(hours.employeeId) }}</router-link></td>-->
          <td><router-link :to="{name: 'EmployeePage', params: { employeeId: hours.employeeId }}">{{ this.getName(hours.employeeId) }}</router-link></td>
          <td>{{ hours.totalHours }}</td>
          <td>{{ hours.deliveredHours }}</td>
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
      <button @click="this.add()">Set</button>
    </div>

    <div>
      <h4>Upload Performed Hours</h4>
      <input type="file" @change="onBookingFileChange($event)" />
      <button v-on:click="submitBooking()">Submit</button>
    </div>

    <div name="visualization" style="height: 400px;">
      <apexchart height="100%" v-if="this.burnDownChartData" type="line" :options="this.burnDownChartData.options" :series="this.burnDownChartData.series"></apexchart>
    </div>
  </div>
</template>

<script lang="ts">
import { Project, getProjectDetails, EmployeeDto, getAccountDepartment, postProjectTotalPlanning, postBooking } from '@/services/pomeloAPI'
import { getLastMonday, addDays } from '@/util'
import { defineComponent } from 'vue'
import { useRoute } from 'vue-router'

export default defineComponent({
  name: 'Project',
  data() {
    return {
      projectId: Number(useRoute().params.projectId),
      project: null as Project | null,
      departmentEmployees: [] as EmployeeDto[],
      uninvolvedEmployees: [] as EmployeeDto[],
      eph: null as number | null,
      newEmployee: null as EmployeeDto | null,
      newHours: null as number | null,
      burnDownChartData: null as any,
      bookingFile: null as File | null
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
      this.updateBurnDownChart()
    },
    async add() {
      if (this.newEmployee != null && this.newHours != null) {
        console.log(this.newEmployee)
        this.project = await postProjectTotalPlanning(this.newEmployee.id, this.projectId, this.newHours)
        this.updateBurnDownChart()
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
        t = addDays(t, 1)
      }
      return days
    },
    updateBurnDownChart() {
      // The burndown chart starts at the last booking.
      // Bookings may happen at any time, so the begun week needs special handling.
      const project = this.project!
      const today = new Date()
      today.setUTCHours(0, 0, 0, 0)
      const totalProjectHours = project.employeeProjectHours.reduce((sum, eph) => sum + eph.totalHours, 0)
      const deliveredProjectHours = project.employeeProjectHours.reduce((sum, eph) => sum + eph.deliveredHours, 0)
      const projectBegin = new Date(Date.parse(project.begin + 'Z'))
      const projectEnd = new Date(Date.parse(project.end + 'Z'))
      const totalWorkingDays = this.getWorkingDays(projectBegin, projectEnd)
      const remainingWorkingDays = this.getWorkingDays(today, projectEnd)
      const remainingQuota = remainingWorkingDays / totalWorkingDays
      const plannedHours = []

      const begin = new Date()
      begin.setUTCHours(0, 0, 0, 0)
      var weekBegin = getLastMonday(begin)
      weekBegin.setUTCHours(0, 0, 0, 0)

      var predictedRemainingHours = totalProjectHours - deliveredProjectHours
      for (var i = 0; i < 12; i++) {
        plannedHours.push([
          weekBegin.toString(),
          predictedRemainingHours
        ])
        predictedRemainingHours -= project.employeeProjectWeeklyCapacities
          .filter(wpc => new Date(Date.parse(wpc.start + 'Z')).getTime() === weekBegin.getTime())
          .reduce((sum, e) => sum + e.capacity, 0)

        weekBegin = addDays(weekBegin, 7)
      }
      // console.log(plannedHours)
      // console.log(`today: ${today} passedQuota: ${remainingQuota}`)
      this.burnDownChartData = {
        series: [
          {
            name: 'Theoretical Hours',
            data: [
              [today.toString(), Math.round(totalProjectHours * remainingQuota)],
              [project!.end, 0]
            ]
          },
          {
            name: 'Predicted Hours',
            data: plannedHours
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
          },
          markers: {
            size: 5
          }
        }
      }
    },
    onBookingFileChange(event: Event) {
      const target = event.target as HTMLInputElement
      this.bookingFile = target.files![0]
    },
    async submitBooking() {
      if (this.bookingFile) {
        var response = await postBooking(this.projectId, this.bookingFile)
        if (!response[0]) {
          alert(response[1])
        } else {
          this.project = await getProjectDetails(this.projectId)
          this.updateBurnDownChart()
        }
      } else {
        console.log('no file selected')
      }
    }
  }
})
</script>
