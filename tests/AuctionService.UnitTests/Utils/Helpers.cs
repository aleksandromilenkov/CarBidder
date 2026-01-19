using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace AuctionService.UnitTests.Utils
{
    public class Helpers
    {
        public static ClaimsPrincipal GetClaimsPrincipal()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "test") };
            var identity = new ClaimsIdentity(claims, "testingIdentity");
            return new ClaimsPrincipal(identity);
        }
    }
}
