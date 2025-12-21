import { create } from 'zustand';

type State = {
    pageNumber: number;
    pageSize: number;
    pageCount: number;
    searchTerm: string;
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