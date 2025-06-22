<template>
  <div class="min-h-screen bg-gradient-to-b from-blue-50 to-white text-gray-800 font-sans">
    <!-- Optional: Add a navbar or header here -->
    <header class="bg-white shadow-md py-4 px-6 mb-6">
      <h1 class="text-2xl font-bold">Mirror</h1>
    </header>

    <!-- This renders your route page (e.g., pages/index.vue) -->
    <NuxtPage />
  </div>
</template>

<script setup>
// No logic needed unless you're using global composition
</script>

<style>
/* Optional global styles (rarely needed with Tailwind) */
html,
body {
  font-family: 'Inter', sans-serif;
}
</style>
