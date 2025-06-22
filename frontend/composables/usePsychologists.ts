import axios from 'axios';

export interface Psychologist {
    Id: string
    Name: string
    ShortDescription: string | null
    PsychologistType: string
    PictureUrl: string | null
    NextAvailable: string[]
}

export interface PsychologistFilter {
    name?: string
    type?: string
    page?: number
    pageSize?: number
}

export interface PaginatedResult<T> {
    Items: T[]
    TotalCount: number
    Page: number
    PageSize: number
}

export function usePsychologists() {
    const psychologists = ref<Psychologist[]>([])
    const totalCount = ref(0)
    const currentPage = ref(1)
    const pageSize = ref(10)
    const loading = ref(false)
    const error = ref<Error | null>(null)

    const fetchPsychologists = async (filters: PsychologistFilter = {}) => {
        try {
            loading.value = true
            error.value = null

            const query = new URLSearchParams()
            if (filters.name) query.append('name', filters.name)
            if (filters.type) query.append('type', filters.type)
            query.append('page', (filters.page ?? currentPage.value).toString())
            query.append('pageSize', (filters.pageSize ?? pageSize.value).toString())

            const response = await axios.get<PaginatedResult<Psychologist>>(`/api/psychologists?${query.toString()}`)

            psychologists.value = response.data.Items
            totalCount.value = response.data.TotalCount
            currentPage.value = response.data.Page
            pageSize.value = response.data.PageSize
        } catch (err: any) {
            error.value = err
        } finally {
            loading.value = false
        }
    }

    return {
        psychologists,
        totalCount,
        currentPage,
        pageSize,
        loading,
        error,
        fetchPsychologists
    }
}
