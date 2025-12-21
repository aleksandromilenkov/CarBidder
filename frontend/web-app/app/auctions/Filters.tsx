import { useParamsStore } from "@/hooks/useParamsStore";
import { Button, ButtonGroup } from "flowbite-react";
import { AiOutlineClockCircle, AiOutlineSortAscending } from "react-icons/ai";
import { BsStopCircleFill, BsStopwatchFill } from "react-icons/bs";
import { GiFinishLine, GiFlame } from "react-icons/gi";

const pageSizeButtons = [4, 8, 12];

const orderByOptions = [{ label: "Alphabetical", icon: AiOutlineSortAscending, value: "make"},
    {label: "End Date", icon: AiOutlineClockCircle, value: "ending soon"},
    {label: "Recently Added", icon: BsStopCircleFill, value: "new"},
];

const filterByOptions = [{ label: "Live Auctions", icon: GiFlame, value: "live"},
    {label: "Ending in < 6 hours", icon: GiFinishLine, value: "endingSoon"},
    {label: "Completed", icon: BsStopwatchFill, value: "finished"},
];

const Filters = () => {
  const { pageSize, setParams, orderBy, filterBy } = useParamsStore();
  const setPageSize = (size: number) => {
    setParams({ pageSize: size, pageNumber: 1 });
  };
  return (
    <div className="flex justify-between items-center mb-4">
        <div>
        <span className="uppercase text-sm text-gray-500 mr-2">Filter by:</span>
         <ButtonGroup outline>
          {filterByOptions.map(({label, icon: Icon, value}, idx) => (
            <Button
              key={value}
              color={`${filterBy === value ? "red" : "gray"}`}
              className="focus:ring-0"
              onClick={() => setParams({filterBy: value})}
            >
              <Icon className="mr-3 h-4 w-4"/>
              {label}
            </Button>
          ))}
        </ButtonGroup>
      </div>
      <div>
        <span className="uppercase text-sm text-gray-500 mr-2">Order by:</span>
         <ButtonGroup outline>
          {orderByOptions.map(({label, icon: Icon, value}, idx) => (
            <Button
              key={value}
              color={`${orderBy === value ? "red" : "gray"}`}
              className="focus:ring-0"
              onClick={() => setParams({orderBy: value})}
            >
              <Icon className="mr-3 h-4 w-4"/>
              {label}
            </Button>
          ))}
        </ButtonGroup>
      </div>
      <div>
        <span className="uppercase text-sm text-gray-500 mr-2">Page size:</span>
        <ButtonGroup outline>
          {pageSizeButtons.map((value, index) => (
            <Button
              key={index}
              color={`${pageSize === value ? "red" : "gray"}`}
              className="focus:ring-0"
              onClick={() => setPageSize(value)}
            >
              {value}
            </Button>
          ))}
        </ButtonGroup>
      </div>
    </div>
  );
};
export default Filters;
