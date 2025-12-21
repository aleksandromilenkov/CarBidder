'use client';
import { Auction } from "@/types";
import { PageResult } from "@/types";
import AuctionCard from "./AuctionCard";
import AppPagination from "../components/AppPagination";
import { getAuctions } from "../actions/auctionActions";
import { useEffect, useState } from "react";
import Filters from "./Filters";



const Listings = () => {
  const [auctions, setAuctions] = useState<Auction[]>([]);
  const [pageCount, setPageCount] = useState<number>(0);
  const [pageNumber, setPageNumber] = useState<number>(1);
  const [pageSize, setPageSize] = useState<number>(4);
  
  useEffect(() => {
    getAuctions(pageNumber, pageSize).then((data: PageResult<Auction>) => {
      setAuctions(data.results);
      setPageCount(data.pageCount);
    });
  }, [pageNumber, pageSize]);
  if (auctions.length === 0) {
    return <p>Loading auctions...</p>;
  }

  return (
    <>
    <Filters pageSize={pageSize} setPageSize={setPageSize} />
    <div className="grid grid-cols-4 gap-6">
        {(!auctions || auctions.length === 0) ? (
            <p>No auctions found.</p>
        ) : (
            auctions?.map((auction: Auction) => (
                <AuctionCard key={auction.id} auction={auction}/>
            ))
        )}
    </div>
    <div className="flex justify-center mt-4">
        <AppPagination currentPage={pageNumber} pageCount={pageCount} setPageNumber={setPageNumber}/>
    </div>
    </>
  )
}
export default Listings