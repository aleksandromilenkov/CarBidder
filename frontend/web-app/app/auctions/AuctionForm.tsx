'use client';
import { Button, HelperText, Spinner, TextInput } from "flowbite-react";
import { useRouter } from "next/navigation";
import { FieldValues, useForm } from "react-hook-form";
import Input from "../components/Input";
import { useEffect } from "react";

const AuctionForm = () => {
    const router = useRouter();
    const {control, handleSubmit, setFocus, formState: {isSubmitting, isValid, isDirty}} = useForm({
        mode: 'onTouched'
    });
    const onSubmit = async (data: FieldValues) => {
        console.log(data);
    }
    
    useEffect(() => {
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
        <div className="grid grid-col-2 gap-3">
            <Input name="reservePrice" label="Reserve Price(enter 0 if no reserve)" type="number" control={control} rules={{required: "Reserve Price is required"}}/>
            <Input name="auctionEnd" label="Auction End Date" control={control} rules={{required: "Auction End Date is required"}}/>
        </div>
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