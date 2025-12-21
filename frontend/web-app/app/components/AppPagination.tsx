"use client"
import { useParamsStore } from "@/hooks/useParamsStore";
import { Pagination } from "flowbite-react"

const AppPagination = () => {
  const {pageNumber: currentPage, pageCount, setParams} = useParamsStore();
  
  return (
    <Pagination
        currentPage={currentPage}
        totalPages={pageCount}
        onPageChange={(page) => {
          setParams({pageNumber: page});
        }}
        layout="pagination"
        showIcons={true}
        className="text-blue-500 mb-5"
    />
  )
}
export default AppPagination