'use client'

import { useParamsStore } from "@/hooks/useParamsStore"
import { Dropdown, DropdownDivider, DropdownItem } from "flowbite-react"
import { User } from "next-auth"
import { signOut } from "next-auth/react"
import Link from "next/link"
import { usePathname, useRouter } from "next/navigation"
import { AiFillCar, AiFillTrophy, AiOutlineLogout } from "react-icons/ai"
import { HiCog, HiUser } from "react-icons/hi"

type Props = {
  user: User
}

const UserActions = ({user} : Props) => {
  const router = useRouter();
  const pathName = usePathname();
  const setParams = useParamsStore((state) => state.setParams);

  const myAuctionsHandler = () => {
    setParams({ seller: user.username, winner: undefined });
    if (pathName !== "/") router.push("/");
  }
  const auctionsWonHandler = () => {
    setParams({ winner: user.username, seller: undefined });
    if (pathName !== "/") router.push("/");
  }
  return (
   <Dropdown inline label={`Welcome ${user.name}`} className="cursor-pointer">
      <DropdownItem icon={HiUser} onClick={myAuctionsHandler}>
        My Auctions 
      </DropdownItem>
      <DropdownItem icon={AiFillTrophy} onClick={auctionsWonHandler}>
        Auctions won 
      </DropdownItem>
      <DropdownItem icon={AiFillCar}>
        <Link href="/auctions/create">
        Sell my car 
        </Link>
      </DropdownItem>
      <DropdownItem icon={HiCog}>
        <Link href="/session">
          Session (dev only!)
        </Link>
      </DropdownItem>
      <DropdownDivider/>
      <DropdownItem icon={AiOutlineLogout} onClick={()=>signOut({redirectTo: "/"})}>
        Sign out
      </DropdownItem>
   </Dropdown>
  )
}
export default UserActions