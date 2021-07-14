<template>
  <h2>Register</h2>
  <div>
    <input v-model="message" placeholder="employeeName">
    <button @click="store.storeMethods.tryRegister('a','a')">Register</button>
  </div>
</template>

<script lang="ts">
import { defineComponent, inject } from 'vue'

export default defineComponent({
  name: 'Register',
  setup(_props) {
    const store = inject('store')

    return {
      store
    }
  },
  components: {
  }
})
</script>
