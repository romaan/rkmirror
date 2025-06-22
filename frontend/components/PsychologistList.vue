<template>
  <div class="p-4 max-w-6xl mx-auto space-y-10">
    <!-- Loading or No Result States -->
    <div v-if="loading" class="text-center text-gray-500 text-lg">Loading...</div>

    <div v-else-if="psychologists.length === 0" class="text-center text-gray-500 text-lg">
      No psychologists found.
    </div>

    <!-- Psychologist Cards -->
    <div v-else class="grid md:grid-cols-2 gap-6">
      <PsychologistCard
          v-for="psych in psychologists"
          :key="psych.Id"
          :psychologist="psych"
      />
    </div>

    <!-- Pagination Controls -->
    <div
        v-if="totalPages > 1"
        class="flex justify-center items-center mt-10 space-x-4 text-base"
    >
      <button
          data-testid="pagination-prev"
          @click="goToPage(currentPage - 1)"
          :disabled="currentPage === 1"
          class="px-4 py-2 border rounded-lg disabled:opacity-50 disabled:cursor-not-allowed"
      >
        Prev
      </button>

      <span class="font-medium">Page {{ currentPage }} of {{ totalPages }}</span>

      <button
          data-testid="pagination-next"
          @click="goToPage(currentPage + 1)"
          :disabled="currentPage === totalPages"
          class="px-4 py-2 border rounded-lg disabled:opacity-50 disabled:cursor-not-allowed"
      >
        Next
      </button>
    </div>
  </div>
</template>


<script lang="ts" setup>
import { ref, watch, computed } from 'vue'
import axios from 'axios'
import type { Psychologist, PsychologistFilter } from '@/composables/usePsychologists'
import PsychologistCard from './PsychologistCard.vue'
import { useRuntimeConfig } from '#imports'

const props = defineProps<{
  filters: PsychologistFilter
}>()

const psychologists = ref<Psychologist[]>([])
const totalCount = ref(0)
const loading = ref(false)
const currentPage = ref(1)
const pageSize = ref(10)

const fetchPsychologists = async () => {
  loading.value = true
  try {
    const config = useRuntimeConfig()
    const query = new URLSearchParams()
    if (props.filters.name) query.append('name', props.filters.name)
    if (props.filters.type) query.append('type', props.filters.type)
    query.append('page', currentPage.value.toString())
    query.append('pageSize', pageSize.value.toString())

    const response = await axios.get(`${config.public.apiBase}/psychologists?${query.toString()}`)
    const data = response.data

    psychologists.value = data.Items
    totalCount.value = data.TotalCount
    currentPage.value = data.Page
    pageSize.value = data.PageSize
  } catch (error) {
    console.error('Error fetching psychologists:', error)
    psychologists.value = []
    totalCount.value = 0
  } finally {
    loading.value = false
  }
}

watch(() => props.filters, () => {
  currentPage.value = 1
  fetchPsychologists()
}, { immediate: true, deep: true })

const totalPages = computed(() => Math.ceil(totalCount.value / pageSize.value))

const goToPage = (page: number) => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page
    fetchPsychologists()
  }
}
</script>
