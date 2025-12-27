import { getDetailedViewData } from "@/app/actions/auctionActions";
import Heading from "@/app/components/Heading";
import AuctionForm from "../../AuctionForm";

const Update = async ({params}: {params: Promise<{ id: string }>}) => {
  const { id } = await params
  const auction = await getDetailedViewData(id);
  return <div className="max-auto max-w-[75%] shadow-lg p-10 bg-white rounded-lg">
    <Heading title="Update your auction" subtitle="Please update the details of your car(only these auction properties can be updated)"/>
    <AuctionForm auction={auction}/>
  </div>
}

export default Update
