```vue
<template>
  <div class="toast-container">
    <div v-for="toast in toasts"
         :key="toast.id"
         class="toast"
         :class="'toast-' + toast.type">
      <div class="toast-content">
        <strong v-if="toast.title">
          {{ toast.title }}
        </strong>

        <span>
          {{ toast.message }}
        </span>
      </div>

      <button class="toast-close"
              @click="remove(toast.id)">
        ×
      </button>
    </div>
  </div>
</template>

<script>
  import axios from 'axios'

  export default {
    name: 'ToastNotifier',

    data() {
      return {
        toasts: [],
        nextId: 1
      }
    },

    mounted() {
      console.log('Toast notifier mounted')
      window.addEventListener('show-toast', this.handleToast)
    },
    beforeUnmount() {
      window.removeEventListener('show-toast', this.handleToast)
    },
    methods: {
      handleToast(event) {
        const {
          message,
          type,
          title,
          duration
        } = event.detail

        this.show(
          message,
          type,
          title,
          duration
        )
      },
      show(message, type = 'info', title = '', duration = 4000) {
        const id = this.nextId++

        this.toasts.push({
          id,
          message,
          type,
          title
        })

        if (duration > 0) {
          setTimeout(() => {
            this.remove(id)
          }, duration)
        }
      },
      success(message, title = 'Success', duration = 4000) {
        this.show(message, 'success', title, duration)
      },

      error(message, title = 'Error', duration = 4000) {
        this.show(message, 'error', title, duration)
      },

      warning(message, title = 'Warning', duration = 4000) {
        this.show(message, 'warning', title, duration)
      },

      info(message, title = 'Information', duration = 4000) {
        this.show(message, 'info', title, duration)
      },

      remove(id) {
        this.toasts = this.toasts.filter(
          toast => toast.id !== id
        )
      }
    }
  }
</script>

<style scoped>
  .toast-container {
    position: fixed;
    top: 20px;
    right: 20px;
    z-index: 9999;
    display: flex;
    flex-direction: column;
    gap: 10px;
    width: 350px;
  }

  .toast {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 15px;
    background: white;
    border-radius: 8px;
    box-shadow: 0 4px 15px rgba(0, 0, 0, 0.15);
  }

  .toast-content {
    display: flex;
    flex-direction: column;
    gap: 4px;
  }

  .toast-close {
    border: none;
    background: transparent;
    font-size: 20px;
    cursor: pointer;
  }

  .toast-success {
    border-left: 4px solid #22c55e;
  }

  .toast-error {
    border-left: 4px solid #ef4444;
  }

  .toast-warning {
    border-left: 4px solid #f59e0b;
  }

  .toast-info {
    border-left: 4px solid #3b82f6;
  }
</style>
```
