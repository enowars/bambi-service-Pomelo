<template>
  <h2>Register</h2>
  <div>
    <input v-model="name" placeholder="name">
    <input v-model="department" placeholder="department">
    <button @click="this.register()">Register</button>
  </div>
</template>

<script lang="ts">
import { postRegister } from '@/services/pomeloAPI'
import { defineComponent } from 'vue'

export default defineComponent({
  name: 'Register',
  data() {
    return {
      name: null as string | null,
      department: null as string | null
    }
  },
  methods: {
    async register() {
      if (this.name != null && this.department != null) {
        const employee = await postRegister(this.name, this.department, null)
        this.$router.push({ name: 'Overview' })
      }
    }
  }
})
</script>
