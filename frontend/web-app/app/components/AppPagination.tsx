"use client"
import { Pagination } from "flowbite-react"
type Props = {
    currentPage: number;
    pageCount: number;
    setPageNumber: (page: number) => void;
}
const AppPagination = ({ currentPage, pageCount, setPageNumber }: Props) => {

  return (
    <Pagination
        currentPage={currentPage}
        totalPages={pageCount}
        onPageChange={(page) => {
          setPageNumber(page);
        }}
        layout="pagination"
        showIcons={true}
        className="text-blue-500 mb-5"
    />
  )
}
export default AppPagination