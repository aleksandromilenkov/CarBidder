'use client'
import { Button } from "flowbite-react"
import { signIn } from "next-auth/react"

type Props = {}
const LoginButton = (props: Props) => {
  return (
    <Button outline onClick={()=> signIn('id-server', {redirectTo: "/"}, {prompt: 'login'})}>Login</Button>
  )
}
export default LoginButton