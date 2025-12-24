"use client";
import { Auction } from "@/types";
import { PagedResult } from "@/types";
import AuctionCard from "./AuctionCard";
import AppPagination from "../components/AppPagination";
import { getAuctions } from "../actions/auctionActions";
import { useEffect, useState } from "react";
import Filters from "./Filters";
import { useParamsStore } from "@/hooks/useParamsStore";
import { useShallow } from "zustand/react/shallow";
import qs from "query-string";
import EmptyFilter from "../components/EmptyFilter";

const Listings = () => {
  const [data, setData] = useState<PagedResult<Auction>>();
  const {
    pageNumber,
    pageSize,
    pageCount,
    orderBy,
    filterBy,
    winner,
    seller,
    searchTerm,
    setParams,
  } = useParamsStore(
    useShallow((state) => ({
      pageNumber: state.pageNumber,
      pageSize: state.pageSize,
      pageCount: state.pageCount,
      searchTerm: state.searchTerm,
      orderBy: state.orderBy,
      winner: state.winner,
      seller: state.seller,
      filterBy: state.filterBy,
      setParams: state.setParams,
      resetParams: state.resetParams,
    }))
  );
  const url = qs.stringifyUrl(
    { url: "", query: { pageNumber, pageSize, searchTerm, orderBy, filterBy, winner, seller } },
    { skipEmptyString: true }
  );

  useEffect(() => {
    getAuctions(url).then((data: PagedResult<Auction>) => {
      setData(data);
      setParams({ pageCount: data.pageCount });
    });
  }, [setParams, url]);

  if (!data) {
    return <p>Loading auctions...</p>;
  }

  return (
    <>
      <Filters />
      {data.totalCount === 0 ? (
        <EmptyFilter showReset />
      ) : (
        <>
          <div className="grid grid-cols-4 gap-6">
            {!data?.results || data.results.length === 0 ? (
              <p>No auctions found.</p>
            ) : (
              data?.results?.map((auction: Auction) => (
                <AuctionCard key={auction.id} auction={auction} />
              ))
            )}
          </div>
          {pageCount > 1 ? (
            <div className="flex justify-center mt-4">
              <AppPagination />
            </div>
          ) : (
            <div className="mt-4 p-4 "></div>
          )}
        </>
      )}
    </>
  );
};
export default Listings;
