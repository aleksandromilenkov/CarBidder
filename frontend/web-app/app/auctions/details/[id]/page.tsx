const Details = async ({params}: {params: Promise<{ id: string }>}) => {
  const { id } = await params

  return <div>Details for: {id}</div>
}

export default Details
