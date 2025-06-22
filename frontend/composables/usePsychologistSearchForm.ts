import { ref, onMounted } from 'vue'

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

    const loadFields = async () => {
        try {
            const res = await fetch('http://localhost:7001/api/psychologist-types')
            const types: string[] = await res.json()

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
