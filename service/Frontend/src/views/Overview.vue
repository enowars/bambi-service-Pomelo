<template>
  <h2>Overview</h2>
  <div class="home">
    <img alt="Vue logo" src="../assets/logo.png">
    <HelloWorld msg="Welcome to Your Vue.js + TypeScript App"/>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import HelloWorld from '@/components/HelloWorld.vue' // @ is an alias to /src
import About from './About.vue'

export default defineComponent({
  name: 'Home',
  components: {
    HelloWorld,
    About
  }
})
</script>
