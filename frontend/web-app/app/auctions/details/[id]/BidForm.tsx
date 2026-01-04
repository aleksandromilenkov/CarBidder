'use client';

import { placeBidForAuction } from "@/app/actions/auctionActions";
import { useBidStore } from "@/hooks/useBidStore";
import { numberWithCommas } from "@/lib/numberWithCommas";
import { FieldValues, useForm } from "react-hook-form";
import toast from "react-hot-toast";

type Props = {
    auctionId: string;
    highestBid: number;
}
const BidForm = ({auctionId, highestBid}: Props) => {
   const {register, handleSubmit, reset} = useForm();
   const addBid = useBidStore((state) => state.addBid);

   const onSubmit = async (data: FieldValues) => {
        placeBidForAuction(auctionId, parseFloat(data.amount)).then((newBid) => {
            if(newBid.error){
                reset(); 
                throw newBid.error;
             }
            addBid(newBid);
            reset();
        }).catch((error) => {
            toast.error(error.message || "Failed to place bid.");
        });
   };
  return (
    <form onSubmit={handleSubmit(onSubmit)} className="flex items-center border-2 rounded-lg py-2">
        <input type="number" {...register('amount')} className="input-custom" placeholder={`Enter your bid (minimum bid: $${numberWithCommas(highestBid + 1)})`}/>
    </form>
  )
}
export default BidForm