'use client'
import { useParamsStore } from "@/hooks/useParamsStore";
import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { AiOutlineCar } from "react-icons/ai";

const Logo = () => {
    const {resetParams} = useParamsStore();
    const router = useRouter();
    const pathName = usePathname();

    const handleReset = () => {
        if (pathName !== "/") router.push('/');
        resetParams();
    }
  return (
    <div onClick={handleReset}  className="cursor-pointer flex items-center gap-2 text-3xl font-semibold text-red-500">
      <AiOutlineCar size={30} />
      <div><Link href={"/"}>CarBidder Auctions</Link></div>
    </div>
  );
};
export default Logo;
