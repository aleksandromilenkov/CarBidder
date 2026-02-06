'use client'
import { Button } from "flowbite-react"
import { signIn } from "next-auth/react"

const LoginButton = () => {
  return (
    <Button outline onClick={()=> signIn('id-server', {redirectTo: "/"}, {prompt: 'login'})}>Login</Button>
  )
}
export default LoginButton