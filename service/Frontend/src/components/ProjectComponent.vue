<template>
  <h1>{{ project.name }}</h1>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { Project } from '@/services/pomeloAPI'

export default defineComponent({
  name: 'project-component',
  props: {
    project: Object as () => Project
  }
})
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
h3 {
  margin: 40px 0 0;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}
</style>
