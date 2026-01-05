'use client';
import { useAuctionStore } from "@/hooks/useAuctionStore";
import { useBidStore } from "@/hooks/useBidStore";
import { Bid } from "@/types";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { useParams } from "next/navigation";
import { useCallback, useEffect, useRef } from "react";

type Props = {
  children: React.ReactNode;
};
const SignalRProvider = ({ children }: Props) => {
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

    return () => {
        connection.current?.off("BidPlaced", handleBidPlaced);
    }
  }, [handleBidPlaced]);
  return <div>{children}</div>;
};
export default SignalRProvider;
