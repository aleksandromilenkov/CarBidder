export type PagedResult<T> = {
    results: T[];
    pageCount: number;
    totalCount: number;
}

export type Auction = {
    id: string;
    title: string;
    description: string;
    seller: string;
    winner?: string;
    currentHighBid?: number;
    soldAmount?: number;
    reservePrice: number;
    mileage: number;
    auctionEnd: string; // ISO date string
    createdAt: string; // ISO date string
    updatedAt: string; // ISO date string
    imageUrl: string;
    color: string;
    make: string;
    status: string;
    model: string;
    year: number;
};