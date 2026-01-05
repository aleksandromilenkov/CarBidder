'use client';
import { useAuctionStore } from "@/hooks/useAuctionStore";
import { useBidStore } from "@/hooks/useBidStore";
import { Auction, Bid } from "@/types";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { User } from "next-auth";
import { useParams } from "next/navigation";
import { useCallback, useEffect, useRef } from "react";
import AuctionCreatedToast from "../components/AuctionCreatedToast";
import toast from "react-hot-toast";

type Props = {
  children: React.ReactNode;
  user: User | null;
};
const SignalRProvider = ({ children, user }: Props) => {
  const connection = useRef<HubConnection | null>(null);
  const setCurrentPrice = useAuctionStore((state) => state.setCurrentPrice);
  const addBid = useBidStore((state) => state.addBid);
  const params = useParams<{ id: string }>();

  const handleBidPlaced = useCallback((bid: Bid) => {
     if(bid.bidStatus.includes("Accepted")){
        setCurrentPrice(bid.auctionId, bid.amount);
     }
     if(params.id === bid.auctionId){
        addBid(bid);
     }
  }, [addBid, setCurrentPrice, params.id]);

  const handleAuctionCreated = useCallback((auction: Auction) => {
    if (user?.username !== auction.seller) {
        return toast(<AuctionCreatedToast auction={auction} />, { duration: 10000 } );
    }
  }, [user?.username]);

  useEffect(() => {
    if (!connection.current) {
      connection.current = new HubConnectionBuilder()
        .withUrl("http://localhost:6001/notifications")
        .withAutomaticReconnect()
        .build();
      connection.current.start()
        .then(() => console.log("SignalR Connected"))
        .catch((error) => console.error("SignalR Connection Error: ", error));
    }
    connection.current.on("BidPlaced", handleBidPlaced);
    connection.current.on("AuctionCreated", handleAuctionCreated);

    return () => {
        connection.current?.off("BidPlaced", handleBidPlaced);
        connection.current?.off("AuctionCreated", handleAuctionCreated);
    }
  }, [handleBidPlaced, handleAuctionCreated]);
  return <div>

    {children}
    </div>;
};
export default SignalRProvider;
