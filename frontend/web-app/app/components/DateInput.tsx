import DatePicker, { DatePickerProps } from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import { useController, UseControllerProps } from "react-hook-form";

type Props = {
  label: string;
  type?: string;
} & UseControllerProps &
  DatePickerProps;
const DateInput = (props: Props) => {
  const { field, fieldState } = useController({ ...props });
  return (
    <div className="mb-3 ">
      <DatePicker
        {...props}
        {...field}
        selected={field.value}
        placeholderText={props.label}
        className={`rounded-lg w-full border border-gray-600 p-2 flex flex-col ${
          fieldState?.error
            ? "bg-red-50 border-red-500 text-red-900"
            : fieldState.isDirty && !fieldState.invalid
            ? "border-green-500 bg-green-50 text-green-900"
            : ""
        }`}
      />
      {fieldState.error && (
        <div className="mt-2 text-sm text-red-600">{fieldState.error.message}</div>
      )}
    </div>
  );
};
export default DateInput;
