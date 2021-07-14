<template>
  <div v-if="!store.storeMethods.getLoggedIn()">
    not logged in!
    <Register/>
  </div>
  <div v-else>
    <div id="nav">
      <router-link to="/">Overview</router-link> |
      <router-link to="/about">Project</router-link> |
      <router-link to="/user">User</router-link>
    </div>
    <router-view/>
  </div>
</template>

<script lang="ts">
import { provide } from 'vue'
import store from '@/store'
import Register from '@/views/Register.vue'
store.storeMethods.init()

export default {
  setup() {
    provide('store', store)
    return {
      store
    }
  },
  components: {
    Register
  }
}
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}

#nav {
  padding: 30px;
}

#nav a {
  font-weight: bold;
  color: #2c3e50;
}

#nav a.router-link-exact-active {
  color: #42b983;
}
</style>
