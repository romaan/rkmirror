// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-05-15',
  devtools: { enabled: true },
  modules: ['@nuxt/ui', '@nuxt/icon', '@nuxt/fonts', '@nuxt/image', '@nuxtjs/tailwindcss'],
  runtimeConfig: {
    public: {
      apiBase: process.env.API_BASE || 'http://localhost:7001/api',
    },
  },
})