import { useParamsStore } from "@/hooks/useParamsStore";
import { Button, ButtonGroup } from "flowbite-react";



const pageSizeButtons = [4, 8, 12]

const Filters = () => {
    const {pageSize, setParams} = useParamsStore();
    const setPageSize = (size: number) => {
        setParams({pageSize: size, pageNumber: 1});
    }
  return (
   <div className="flex justify-between items-center mb-4">
        <div>
            <span className="uppercase text-sm text-gray-500 mr-2">Page size:</span>
            <ButtonGroup outline>
                {pageSizeButtons.map((value, index) => (
                    <Button
                        key={index}
                        color={`${pageSize === value ? 'red' : 'gray'}`}
                        className="focus:ring-0"
                        onClick={() => setPageSize(value)}
                    >
                        {value}
                    </Button>
                ))}
            </ButtonGroup>
        </div>
   </div>
  )
}
export default Filters