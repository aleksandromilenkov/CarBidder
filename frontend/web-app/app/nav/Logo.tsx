'use client'
import { useParamsStore } from "@/hooks/useParamsStore";
import Link from "next/link";
import { AiOutlineCar } from "react-icons/ai";

const Logo = () => {
    const {resetParams} = useParamsStore();
  return (
    <div onClick={resetParams}  className="cursor-pointer flex items-center gap-2 text-3xl font-semibold text-red-500">
      <AiOutlineCar size={30} />
      <div><Link href={"/"}>CarBidder Auctions</Link></div>
    </div>
  );
};
export default Logo;
