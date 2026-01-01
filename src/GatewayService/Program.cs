using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Resolve authority from either Docker config key or local appsettings
var authority = builder.Configuration["IdentityServiceUrl"] ?? builder.Configuration["IdentityServer:Url"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authority;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.NameClaimType = "username";
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("customPolicy", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .WithOrigins(builder.Configuration["ClientApp"]);
    });
});

// Do not register a policy named "default" because YARP reserves that name for routes.
// Use a distinct policy name and match it in the ReverseProxy route configuration.
const string PolicyNameRequireAuctionScope = "RequireAuctionAppScope";

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyNameRequireAuctionScope, policy =>
    {
        policy.RequireAuthenticatedUser();
        // require the access token to include the auctionApp scope
        policy.RequireClaim("scope", "auctionApp");
    });
});

var app = builder.Build();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.Run();