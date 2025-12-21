'use client';
import Image from "next/image"
import { useState } from "react";
type Props = {
    imageUrl:any;
}
const CarImage = ({imageUrl}: Props) => {
    const [loading, setLoading] = useState<boolean>(true);
  return (
    <div>
        <Image
                src={imageUrl || '/placeholder.png'}
                alt='Image of auctioned car'
                fill
                className={`object-cover duration-700 ease-in-out hover:scale-105 transition-transform
                     ${loading ? 'opacity-0 scale-110' : 'opacity-100 scale-100'}`}
                priority
                sizes="(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 25vw"
                onLoad={() => setLoading(false)}
            /></div>
  )
}
export default CarImage