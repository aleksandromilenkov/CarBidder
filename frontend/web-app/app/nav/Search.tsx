"use client";

import { ChangeEvent, useEffect, useState } from "react";
import { useParamsStore } from "@/hooks/useParamsStore";
import { FaSearch } from "react-icons/fa";

const Search = () => {
    const { setParams, searchTerm } = useParamsStore();
  const [searchInput, setSearchInput] = useState<string>(searchTerm);

  const handleSearch = () => {
    setParams({ searchTerm: searchInput });
  }

  const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
    if (e.target.value === "") { 
        setParams({ searchTerm: "" });
    }
    setSearchInput(e.target.value);
  }

  useEffect(()=>{
    if(searchTerm === "") setSearchInput("");
  }, [searchTerm])

  return (
    <div className="flex w-[50%] items-center border-2 border-gray-300 rounded-full py-2 shadow-sm">
      <input
        type="text"
        placeholder="Search for cars by make, model or color"
        className="flex-grow pl-5 bg-transparent focus:outline-none border-transparent focus:border-transparent focus:ring-0 text-sm text-gray-600"
        id="search"
        name="search"
        value={searchInput}
        onChange={handleChange}
        onKeyDown={(e) => { e.key === "Enter" && handleSearch()}}
      />
      <button onClick={handleSearch}>
        <FaSearch size={34} className="bg-red-400 text-white rounded-full p-2 cursor-pointer mx-2"/>
      </button>
    </div>
  );
};
export default Search;
