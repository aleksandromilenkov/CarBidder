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

export type Bid = {
    id:string;
    auctionId: string;
    bidder: string;
    bidTime: string;
    amount: number;
    bidStatus: string;
}

export type AuctionFinished = {
    itemSold: boolean;
    auctionId: string;
    winner: string;
    seller: string;
    amount: number | null;
}