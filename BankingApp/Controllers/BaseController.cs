﻿using BankingApp.Services.Common.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BankingApp.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        private readonly IdentityOptions _identityOptions;

        protected BaseController(IOptions<IdentityOptions> identityOptions) 
        {
            _identityOptions = identityOptions.Value;
        }
        
        protected UnauthorizedObjectResult CustomUnauthorized()
            => new(new Error("Authentication", "Current user is not logged in"));

        protected long? GetCurrentUserId() 
        {
            var nameId = User.Claims
                .Where(x => x.Type == _identityOptions.ClaimsIdentity.UserIdClaimType)
                .FirstOrDefault();

            if (nameId == null) 
            {
                return null;
            }

            return long.Parse(nameId.Value);
        }
    }
}
