import { create } from 'zustand';

type State = {
    pageNumber: number;
    pageSize: number;
    pageCount: number;
    searchTerm: string;
    orderBy: string;
    filterBy: string;
    seller?: string;
    winner?: string;
}

type Actions = {
    setParams : (params: Partial<State>) => void;
    resetParams : () => void;
}

const initialState: State = {
    pageNumber: 1,
    pageSize: 12,
    pageCount: 1,
    searchTerm: '',
    orderBy: 'make',
    filterBy: 'live',
    seller: undefined,
    winner: undefined,
}

export const useParamsStore = create<State & Actions>((set) => ({

    ...initialState,

    setParams: (newParams) =>
        set((state) => ({
            ...state,
            ...newParams,
  })),

    resetParams: () => set(initialState),
}));