﻿using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SFA.DAS.ApprenticeAan.Domain.Interfaces;
using SFA.DAS.ApprenticeAan.Web.Extensions;
using SFA.DAS.ApprenticePortal.Authentication;

namespace SFA.DAS.ApprenticeAan.Web.AppStart;

[ExcludeFromCodeCoverage]
public class AuthenticationEventsLocal : OpenIdConnectEvents
{
    private readonly IApprenticeAccountService _apprenticeAccountService;
    private readonly IApprenticeService _apprenticesService;

    public AuthenticationEventsLocal(IApprenticeAccountService apprenticeAccountService, IApprenticeService apprenticesService)
    {
        _apprenticeAccountService = apprenticeAccountService;
        _apprenticesService = apprenticesService;
    }

    public override async Task TokenValidated(TokenValidatedContext context)
    {
        await base.TokenValidated(context);
        await AddClaims(context.Principal!);
    }

    public async Task AddClaims(ClaimsPrincipal principal)
    {
        AuthenticatedUser user = new(principal);

        await AddApprenticeAccountClaims(principal, user.ApprenticeId);
        await AddAanMemberClaims(principal, user.ApprenticeId);
    }

    private async Task AddApprenticeAccountClaims(ClaimsPrincipal principal, Guid apprenticeId)
    {
        var apprentice = await _apprenticeAccountService.GetApprenticeAccountDetails(apprenticeId);

        if (apprentice == null) return;

        principal.AddAccountCreatedClaim();

        AddNameClaims(principal, apprentice);

        if (apprentice.TermsOfUseAccepted)
            principal.AddTermsOfUseAcceptedClaim();
    }

    private async Task AddAanMemberClaims(ClaimsPrincipal principal, Guid apprenticeId)
    {
        var apprentice = await _apprenticesService.GetApprentice(apprenticeId);

        if (apprentice == null) return;

        principal.AddAanMemberIdClaim(apprentice.MemberId);
    }

    private static void AddNameClaims(ClaimsPrincipal principal, IApprenticeAccount apprentice)
    {
        principal.AddIdentity(new ClaimsIdentity(new[]
        {
            new Claim(IdentityClaims.GivenName, apprentice.FirstName),
            new Claim(IdentityClaims.FamilyName, apprentice.LastName),
        }));
    }
}
