import Image from "next/image"

type Props = {
    auction:any;
}
const AuctionCard = ({auction}: Props) => {
  return (
    <a href="#">
        <div className="w-full bg-gray-200 aspect-video rounded-lg overflow-hidden relative">
            <Image
                src={auction.imageUrl || '/placeholder.png'}
                alt='Image of auctioned car'
                fill
                className="object-cover hover:scale-105 transition-transform duration-300"
                priority
                sizes="(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 25vw"
            />
        </div>
        <div className="flex justify-between items-center mt-4">
            <h3 className="text-gray-700">{auction.make} {auction.model}</h3>
            <p className="font-semibold text-sm">{auction.year}</p>
        </div>
    </a>
  )
}
export default AuctionCard