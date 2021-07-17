import { createApp } from 'vue'
import VueApexCharts from 'vue3-apexcharts'
import App from './App.vue'
import router from './router'

createApp(App)
  .use(router)
  .use(VueApexCharts)
  .mount('#app')
