<!-- pages/index.vue -->
<template>
  <div class="space-y-6">
    <PsychologistFilter @filter="onFilterChanged" />
    <PsychologistList :filters="filters" />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import PsychologistFilter from '~/components/PsychologistFilter.vue';
import PsychologistList from '~/components/PsychologistList.vue';

const filters = ref({});

const onFilterChanged = (newFilters: any) => {
  filters.value = newFilters;
};
</script>
