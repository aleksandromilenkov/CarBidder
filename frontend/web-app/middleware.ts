import { auth } from "@/auth"
import type { NextRequest } from "next/server"
import { NextResponse } from "next/server"

export const middleware = auth(async (req: NextRequest & { auth: any }) => {
    if (!req.auth) {
    const signInUrl = new URL("/api/auth/signin", req.url)
    signInUrl.searchParams.set("callbackUrl", req.nextUrl.pathname)
    return NextResponse.redirect(signInUrl)
  }

  // authenticated → allow request
  return NextResponse.next()
})

export const config = {
  matcher: ["/session"],
  pages: {
    signIn: '/api/auth/signin'
  }
}
