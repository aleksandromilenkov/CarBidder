'use client';
import { getBidsForAuction } from "@/app/actions/auctionActions";
import Heading from "@/app/components/Heading";
import { useBidStore } from "@/hooks/useBidStore";
import { Auction } from "@/types";
import { User } from "next-auth";
import { useEffect, useState } from "react";
import toast from "react-hot-toast";
import BidItem from "./BidItem";

type Props = {
  user: User | null;
  auction: Auction;
};
const BidList = ({ user, auction }: Props) => {
  const [loading, setLoading] = useState(true);
  const bids = useBidStore((state) => state.bids);
  const setBids = useBidStore((state) => state.setBids);

  useEffect(() => {
    getBidsForAuction(auction.id)
      .then((fetchedBids: any) => {
        if (fetchedBids.error) {
          throw fetchedBids.error;
        }
        setBids(fetchedBids);
        setLoading(false);
      })
      .catch((error) => {
        console.error("Error fetching bids:", error);
        toast.error(error.message || "Failed to load bids.");
      })
      .finally(() => {
        setLoading(false);
      });
  }, [setBids, auction.id]);

  if (loading) {
    return <span>Loading bids...</span>;
  }
  return (
    <div className="border-2 rounded-lg p-2 bg-gray-200">
      <Heading title="Bids" />
      {bids?.map((bid) => (
        <BidItem key={bid.id} bid={bid} />
      ))}
    </div>
  );
};
export default BidList;
