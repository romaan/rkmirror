import { ref, onMounted } from 'vue'
import axios from 'axios';
import {useRuntimeConfig} from '#build/imports';


export interface FormField {
    name: string
    label: string
    type: 'text' | 'select'
    placeholder?: string
    options?: { label: string; value: string }[]
}

export const usePsychologistSearchForm = () => {
    const fields = ref<FormField[]>([])
    const initialValues = {
        name: '',
        type: '',
    }
    const config = useRuntimeConfig()
    const loadFields = async () => {
        try {
            const res = await axios.get(`${config.public.apiBase}/api/psychologist-types`)
            const types: string[] = res.data
            fields.value = [
                {
                    name: 'name',
                    label: 'Name',
                    type: 'text',
                    placeholder: 'Enter psychologist name...',
                },
                {
                    name: 'type',
                    label: 'Type',
                    type: 'select',
                    placeholder: 'Select type',
                    options: [
                        { label: 'Type', value: '' },
                        ...types.map(type => ({
                            label: type,
                            value: type,
                        })),
                    ],
                },
            ]
        } catch (error) {
            console.error('Failed to fetch psychologist types:', error)
        }
    }

    onMounted(loadFields)

    return {
        fields,
        initialValues,
    }
}
