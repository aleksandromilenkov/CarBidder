import { Auction } from "@/types";
import CarImage from "./CarImage";
import CountdownTimer from "./CountdownTimer";
import Link from "next/link";
import CurrentBid from "./CurrentBid";

type Props = {
    auction:Auction;
}
const AuctionCard = ({auction}: Props) => {
  return (
    <Link href={`/auctions/details/${auction.id}`} className="block p-4 bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow duration-300">
        <div className="w-full bg-gray-200 aspect-16/10 rounded-lg overflow-hidden relative">
            <CarImage imageUrl={auction.imageUrl} />
            <div className="absolute bottom-2 left-2">
                <CountdownTimer auctionEnd={auction.auctionEnd} />
            </div>
            <div className="absolute top-2 right-2">
                <CurrentBid amount={auction.currentHighBid} reservePrice={auction.reservePrice} />
            </div>
        </div>
        <div className="flex justify-between items-center mt-4">
            <h3 className="text-gray-700">{auction.make} {auction.model}</h3>
            <p className="font-semibold text-sm">{auction.year}</p>
        </div>
    </Link>
  )
}
export default AuctionCard