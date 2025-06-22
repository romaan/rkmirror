<template>
  <div
      class="bg-white rounded-2xl shadow-md p-4 flex flex-col items-center text-center hover:shadow-lg transition"
      data-testid="psychologist-card"
  >
    <!-- Profile Image -->
    <img
        :src="imageSrc"
        alt="Psychologist photo"
        class="w-24 h-24 rounded-full object-cover mb-4 border-2 border-blue-200"
        @error="onImageError"
    />

    <!-- Name -->
    <h2 class="text-lg font-semibold">{{ psychologist.Name }}</h2>

    <!-- Type -->
    <span class="text-sm text-blue-600 font-medium mt-1">
      {{ psychologist.PsychologistType }}
    </span>

    <!-- Description -->
    <p class="text-sm text-gray-600 mt-2 px-2">
      {{ psychologist.ShortDescription }}
    </p>

    <!-- Next Available -->
    <!-- Next Available -->
    <div v-if="psychologist.NextAvailable?.length" class="mt-3 text-sm text-green-600 w-full text-left">
      <p class="font-medium mb-2">Next Available:</p>

      <div class="flex flex-wrap gap-2 justify-start w-full text-left">
    <span
        v-for="(date, index) in displayDates"
        :key="date"
        class="bg-green-100 text-green-800 px-3 py-1 rounded-full text-xs font-medium shadow-sm"
    >
      {{ formatDateTime(date) }}
    </span>
      </div>

      <button
          v-if="psychologist.NextAvailable.length > 4"
          @click="toggleShowAll"
          class="mt-2 text-xs text-blue-600 hover:underline focus:outline-none"
      >
        {{ showAllMap[psychologist.Id] ? 'Show less' : 'See more' }}
      </button>

    </div>


    <div v-else class="mt-3 text-sm text-gray-400 italic">
      No upcoming availability
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

const props = defineProps({
  psychologist: {
    type: Object,
    required: true
  }
})

const imageSrc = ref(props.psychologist.PictureUrl || '/images/01.jpg')

function onImageError() {
  imageSrc.value = '/images/01.jpg'
}

const showAllMap = ref<Record<string, boolean>>({})

const toggleShowAll = () => {
  const id = props.psychologist.Id
  showAllMap.value[id] = !showAllMap.value[id]
}

const displayDates = computed(() => {
  const id = props.psychologist.Id
  const showAll = showAllMap.value[id] || false
  return showAll
      ? props.psychologist.NextAvailable
      : props.psychologist.NextAvailable.slice(0, 4)
})

function formatDateTime(iso: string) {
  return new Date(iso).toLocaleString(undefined, {
    weekday: 'short',
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}
</script>
