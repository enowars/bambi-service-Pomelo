<template>
  <div>personalpage</div>
  <div>
    loggedIn = ?}
  </div>
</template>

<script lang="ts">
import { Employee, getAccountInfo } from '@/services/pomeloAPI'
import { defineComponent, inject } from 'vue'
export default defineComponent({
  name: 'User',
  data() {
    return {
      employee: null as Employee | null
    }
  },
  created() {
    this.init()
  },
  methods: {
    async init() {
      // this.employee = await getAccountInfo()
    }
  }
})
</script>
