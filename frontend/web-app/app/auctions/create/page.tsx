import Heading from "@/app/components/Heading"
import AuctionForm from "../AuctionForm"

const Create = () => {
  return (
    <div className="mx-auto max-w-[75%] shadow-lg bg-white rounded-lg p-5">
      <Heading title="Sell your car!" subtitle="Please enter the details of your car"/>
      <AuctionForm/>
    </div>
  )
}
export default Create