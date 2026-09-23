import { createApp } from 'vue'
import App from './App.vue'
import router from './router'

const app = createApp(App)

app.config.globalProperties.$toast = {
  success(message, title = 'Success', duration = 4000) {
    window.dispatchEvent(
      new CustomEvent('show-toast', {
        detail: {
          message,
          type: 'success',
          title,
          duration
        }
      })
    )
  },

  error(message, title = 'Error', duration = 4000) {
    window.dispatchEvent(
      new CustomEvent('show-toast', {
        detail: {
          message,
          type: 'error',
          title,
          duration
        }
      })
    )
  },

  warning(message, title = 'Warning', duration = 4000) {
    window.dispatchEvent(
      new CustomEvent('show-toast', {
        detail: {
          message,
          type: 'warning',
          title,
          duration
        }
      })
    )
  },

  info(message, title = 'Information', duration = 4000) {
    window.dispatchEvent(
      new CustomEvent('show-toast', {
        detail: {
          message,
          type: 'info',
          title,
          duration
        }
      })
    )
  }
}

app.use(router)

app.mount('#app')
