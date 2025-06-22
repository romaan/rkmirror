<template>
  <div class="flex flex-wrap items-center gap-4 p-4 bg-white shadow rounded-md">
    <!-- Name Input -->
    <input
        data-testid="name-input"
        v-model="filters.name"
        type="text"
        :placeholder="nameField?.placeholder"
        class="flex-1 px-4 py-2 border rounded-md focus:outline-none focus:ring focus:border-blue-300"
    />

    <!-- Type Dropdown -->
    <select
        data-testid="type-select"
        v-model="filters.type"
        class="px-4 py-2 border rounded-md focus:outline-none focus:ring focus:border-blue-300"
    >
      <option
          v-for="option in typeField?.options || []"
          :key="option.value"
          :value="option.value"
      >
        {{ option.label }}
      </option>
    </select>

    <!-- Search Button -->
    <button
        data-testid="search-button"
        @click="emitFilter"
        class="px-6 py-2 bg-blue-600 text-white font-semibold rounded-md hover:bg-blue-700 transition cursor-pointer"
    >
      Search
    </button>

    <!-- Conditionally Show Clear Button -->
    <button
        v-if="filters.name || filters.type"
        data-testid="clear-button"
        @click="clearFilters"
        class="text-red-600 hover:text-red-800 font-medium transition cursor-pointer"
    >
      x Clear
    </button>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import { usePsychologistSearchForm } from '@/composables/usePsychologistSearchForm'

const emit = defineEmits(['filter'])

const { fields, initialValues } = usePsychologistSearchForm()

const filters = ref({ ...initialValues })

// Access specific field configs
const nameField = computed(() => fields.value.find(f => f.name === 'name'))
const typeField = computed(() => fields.value.find(f => f.name === 'type'))

function emitFilter() {
  emit('filter', { ...filters.value })
}

function clearFilters() {
  filters.value = { ...initialValues }
  emitFilter()
}

// Debounce name input for real-time filtering
let debounceTimeout: ReturnType<typeof setTimeout> | null = null
watch(() => filters.value.name, () => {
  if (debounceTimeout) clearTimeout(debounceTimeout)
  debounceTimeout = setTimeout(() => {
    emitFilter()
  }, 1000)
})
</script>
