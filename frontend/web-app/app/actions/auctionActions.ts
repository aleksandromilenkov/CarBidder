'use server';
import { auth } from "@/auth";
import { Auction, PagedResult } from "@/types";

export async function getAuctions(query:string): Promise<PagedResult<Auction>>{
    const res = await fetch(`http://localhost:6001/search${query}`);
    if(!res.ok){
        throw new Error('Failed to fetch data');
    }
    const data = await res.json();
    return data;
}

export async function updateAuctionTest():Promise<{status:number, message:string}> {
    const session = await auth();
    const res = await fetch(`http://localhost:6001/auctions/afbee524-5972-4075-8800-7d1f9d7b0a0c`,{
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            "Authorization": `Bearer ${session?.accessToken}`
        },
        body: JSON.stringify({
            mileage: Math.floor(Math.random() * 100000) + 1
        })
    });
    if(!res.ok){
        return {status: res.status, message: res.statusText}
    }
   return {status: res.status, message: res.statusText}
}