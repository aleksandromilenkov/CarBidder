'use client'
import { deleteAuction } from "@/app/actions/auctionActions"
import { Button, Spinner } from "flowbite-react"
import { useRouter } from "next/navigation"
import { useState } from "react"
import toast from "react-hot-toast"

type Props = {
    id: string
}
const DeleteButton = ({id}: Props) => {
    const [loading, setLoading] = useState(false);
    const router = useRouter();
    const handleDelete = async () => {
        try{
            setLoading(true);
            const response = await deleteAuction(id);
            if (!response.error){
                router.push('/');
            } else {
                toast.error(response.error.status + " " + response.error.message);
            }
        } catch (error: any){
                toast.error(error.status + " " + error.message);
        } finally {
            setLoading(false);
        }
    }
  return (
    <Button outline color="red" onClick={handleDelete}>
        {loading && <Spinner size="sm" className="mr-3" light />}
        Delete Auction
    </Button>
  )
}
export default DeleteButton