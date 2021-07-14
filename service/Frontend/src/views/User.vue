<template>
  <div>personalpage</div>
  <div>
    loggedIn = {{ store.storeMethods.getLoggedIn() }}
  </div>
</template>

<script lang="ts">
import { defineComponent, inject } from 'vue'
// v-if="store.getLoggedIn()"
export default defineComponent({
  name: 'User',
  setup(props) {
    const store = inject('store')

    return {
      store
    }
  },
  components: {
  }
})
</script>
