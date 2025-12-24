const Update = async ({params}: {params: Promise<{ id: string }>}) => {
  const { id } = await params

  return <div>Update for: {id}</div>
}

export default Update
