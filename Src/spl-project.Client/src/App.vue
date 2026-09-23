<script setup>
  import ToastNotifier from './components/ToastNotifier.vue'
</script>

<template>
  <header>
    <div>
      <router-view />
      <ToastNotifier />
    </div>
  </header>
</template>

<style scoped>
header {
  line-height: 1.5;
}

  header .wrapper {
    display: flex;
    place-items: flex-start;
    flex-wrap: wrap;
  }

</style>
