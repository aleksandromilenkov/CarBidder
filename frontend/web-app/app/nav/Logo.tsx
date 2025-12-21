'use client'
import { useParamsStore } from "@/hooks/useParamsStore";
import { AiOutlineCar } from "react-icons/ai";

const Logo = () => {
    const {resetParams} = useParamsStore();
  return (
    <div onClick={resetParams}  className="cursor-pointer flex items-center gap-2 text-3xl font-semibold text-red-500">
      <AiOutlineCar size={30} />
      <div>CarBidder Auctions</div>
    </div>
  );
};
export default Logo;
