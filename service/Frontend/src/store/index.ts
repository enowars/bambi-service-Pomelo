import { initCustomFormatter, reactive } from 'vue'

import { register, getAccountInfo } from '@/services/pomeloAPI'

const state = reactive({
  loggedIn: false,
  counter: 0
})

const storeMethods = {
  getLoggedIn() : boolean {
    return state.loggedIn
  },
  tryRegister(employeeName: string, department: string) : void {
    register(employeeName, department)
  },
  async init() {
    try {
      await getAccountInfo()
      state.loggedIn = true
    } catch (e) {}
  }
}

export default {
  storeMethods
}
