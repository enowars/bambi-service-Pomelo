<template>
  <div id="nav">
    <router-link to="/">Pomelo</router-link>
  </div>
  <router-view/>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { getAccount } from './services/pomeloAPI'

export default defineComponent({
  created() {
    this.init()
  },
  methods: {
    async init() {
      const accountInfo = await getAccount()
      if (accountInfo === null) {
        this.$router.push({ name: 'Register' })
      } else {
        console.log(accountInfo)
      }
    }
  }
})
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
