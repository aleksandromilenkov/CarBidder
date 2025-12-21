import { useParamsStore } from "@/hooks/useParamsStore";
import Heading from "./Heading";
import { Button } from "flowbite-react";

type Props = {
  title?: string;
  subtitle?: string;
  showReset?: boolean;
};
const EmptyFilter = ({
  title = "No matches for this filter",
  subtitle = "Try changing the filter or search term",
  showReset,
}: Props) => {
  const { resetParams } = useParamsStore();
  return (
    <div className="flex flex-col gap-2 items-center justify-center h-[40vh] shadow-lg">
      <Heading title={title} subtitle={subtitle} center />
      <div className="mt-4">
        {showReset && (
          <Button outline onClick={resetParams}>
            Remove filters
          </Button>
        )}
      </div>
    </div>
  );
};
export default EmptyFilter;
