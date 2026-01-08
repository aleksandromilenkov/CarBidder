"use client";
import { useAuctionStore } from "@/hooks/useAuctionStore";
import { useBidStore } from "@/hooks/useBidStore";
import { Auction, AuctionFinished, Bid } from "@/types";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { useParams } from "next/navigation";
import { useCallback, useEffect, useRef } from "react";
import AuctionCreatedToast from "../components/AuctionCreatedToast";
import toast from "react-hot-toast";
import AuctionFinishedToast from "../components/AuctionFinishedToast";
import { getDetailedViewData } from "../actions/auctionActions";
import { useSession } from "next-auth/react";

type Props = {
  children: React.ReactNode;
};
const SignalRProvider = ({ children }: Props) => {
  const session = useSession();
  const user = session.data?.user;
  const connection = useRef<HubConnection | null>(null);
  const setCurrentPrice = useAuctionStore((state) => state.setCurrentPrice);
  const addBid = useBidStore((state) => state.addBid);
  const params = useParams<{ id: string }>();

  const handleBidPlaced = useCallback(
    (bid: Bid) => {
      if (bid.bidStatus.includes("Accepted")) {
        setCurrentPrice(bid.auctionId, bid.amount);
      }
      if (params.id === bid.auctionId) {
        addBid(bid);
      }
    },
    [addBid, setCurrentPrice, params.id]
  );

  const handleAuctionCreated = useCallback(
    (auction: Auction) => {
      if (user?.username !== auction.seller) {
        return toast(<AuctionCreatedToast auction={auction} />, {
          duration: 10000,
        });
      }
    },
    [user?.username]
  );

  const handleAuctionFinished = useCallback(
    (auctionFinished: AuctionFinished) => {
      const auction = getDetailedViewData(auctionFinished.auctionId);
      if (user?.username !== auctionFinished.seller) {
        return toast.promise(
          auction,
          {
            loading: "Loading",
            success: (auction) => (
              <AuctionFinishedToast
                auction={auction}
                finishedAuction={auctionFinished}
              />
            ),
            error: "Auction finished",
          },
          { duration: 10000, icon: null }
        );
      }
    },
    [user?.username]
  );

  useEffect(() => {
    if (!connection.current) {
      connection.current = new HubConnectionBuilder()
        .withUrl(process.env.NEXT_PUBLIC_NOTIFY_URL!)
        .withAutomaticReconnect()
        .build();
      connection.current
        .start()
        .then(() => console.log("SignalR Connected"))
        .catch((error) => console.error("SignalR Connection Error: ", error));
    }
    connection.current.on("BidPlaced", handleBidPlaced);
    connection.current.on("AuctionCreated", handleAuctionCreated);
    connection.current.on("AuctionFinished", handleAuctionFinished);

    return () => {
      connection.current?.off("BidPlaced", handleBidPlaced);
      connection.current?.off("AuctionCreated", handleAuctionCreated);
      connection.current?.off("AuctionFinished", handleAuctionFinished);
    };
  }, [handleBidPlaced, handleAuctionCreated, handleAuctionFinished]);
  return <div>{children}</div>;
};
export default SignalRProvider;
