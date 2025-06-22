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

<script setup lang="ts">
import { computed, watch } from 'vue'
import type { PsychologistFilter } from '@/composables/usePsychologists'
import { usePsychologists } from '@/composables/usePsychologists'
import PsychologistCard from './PsychologistCard.vue'

const props = defineProps<{
  filters: PsychologistFilter
}>()

const {
  psychologists,
  totalCount,
  currentPage,
  pageSize,
  loading,
  fetchPsychologists
} = usePsychologists()

// Fetch when filters change
watch(
    () => props.filters,
    () => {
      fetchPsychologists({ ...props.filters, page: 1 })
    },
    { immediate: true, deep: true }
)

const totalPages = computed(() => Math.ceil(totalCount.value / pageSize.value))

function goToPage(page: number) {
  if (page >= 1 && page <= totalPages.value) {
    fetchPsychologists({ ...props.filters, page })
  }
}
</script>
