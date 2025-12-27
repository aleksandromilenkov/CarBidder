'use client';
import { Button, Spinner } from "flowbite-react";
import { usePathname, useRouter } from "next/navigation";
import { FieldValues, useForm } from "react-hook-form";
import Input from "../components/Input";
import { useEffect } from "react";
import DateInput from "../components/DateInput";
import { createAuction, updateAuction } from "../actions/auctionActions";
import toast from "react-hot-toast";
import { Auction } from "@/types";

type Props = {
    auction?: Auction
}

const AuctionForm = ({auction}: Props) => {
    const router = useRouter();
    const pathName = usePathname();
    const {control, handleSubmit, reset, setFocus, formState: {isSubmitting, isValid, isDirty}} = useForm({
        mode: 'onTouched'
    });
    const onSubmit = async (data: FieldValues) => {
        console.log(data);
        try{
            let id = '';
            let res;
            if (pathName.includes('create')){
                res = await createAuction(data);
                id = res.id;
            } else {
                if (auction) {
                    res = await updateAuction(auction.id, data);
                    id = auction.id;
                }
            }
            if (res.error) throw res.error;
            router.push(`/auctions/details/${id}`);
        }catch (error: any){
            toast.error(error.status + " " + error.message);
        }
    }
    
    useEffect(() => {
        if (auction){
            const {make, model, color, mileage, year} = auction;
            reset({make, model, color, mileage, year});
        }

        setFocus("make");
    }, [setFocus]);

  return (
   <form className="flex flex-col mt-6" onSubmit={handleSubmit(onSubmit)} >
        <Input name="make" label="Make" control={control} rules={{required: "Make is required"}}/>
        <Input name="model" label="Model" control={control} rules={{required: "Model is required"}}/>
        <Input name="color" label="Color" control={control} rules={{required: "Color is required"}}/>
        <div className="grid grid-col-2 gap-3">
            <Input name="year" label="Year" type="number" control={control} rules={{required: "Year is required"}}/>
            <Input name="mileage" label="Mileage" control={control} rules={{required: "Mileage is required"}}/>
        </div>
        {pathName === "/auctions/create" && <>
        <Input name="imageUrl" label="Image URL" control={control} rules={{required: "Image URL is required"}}/>
        <div className="grid grid-col-2 gap-3">
            <Input name="reservePrice" label="Reserve Price(enter 0 if no reserve)" type="number" control={control} rules={{required: "Reserve Price is required"}}/>
           <DateInput name="auctionEnd"  showTimeSelect dateFormat="dd MMMM yyyy h:mm a" label="Auction End Date" control={control} rules={{required: "Auction End Date is required"}}/>
        </div>
        </> }
        <div className="flex justify-between">
        <Button color='alternative' onClick={() => router.push('/')}>Cancel</Button>
        <Button color='green'
            type="submit"
            disabled={!isValid || !isDirty || isSubmitting}
         >
            {isSubmitting && <Spinner size="sm" light={true} className="mr-2"/>}
            Submit
        </Button>
    </div>
   </form>
  )
}
export default AuctionForm