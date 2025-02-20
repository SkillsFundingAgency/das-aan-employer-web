﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Aan.SharedUi.Constants;
using SFA.DAS.ApprenticeAan.Domain.Interfaces;
using SFA.DAS.ApprenticeAan.Domain.OuterApi.Requests;
using SFA.DAS.ApprenticeAan.Web.Infrastructure;
using SFA.DAS.ApprenticeAan.Web.Models;
using SFA.DAS.ApprenticeAan.Web.Models.Onboarding;
using SFA.DAS.ApprenticePortal.Authentication;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;
using static SFA.DAS.ApprenticeAan.Domain.OuterApi.Requests.CreateApprenticeMemberRequest;

namespace SFA.DAS.ApprenticeAan.Web.Controllers.Onboarding;

[Authorize]
[Route("onboarding/check-your-answers", Name = RouteNames.Onboarding.CheckYourAnswers)]
[HideNavigationBar(true, true)]
public class CheckYourAnswersController : Controller
{
    public const string ViewPath = "~/Views/Onboarding/CheckYourAnswers.cshtml";
    public const string ApplicationSubmittedViewPath = "~/Views/Onboarding/ApplicationSubmitted.cshtml";
    private readonly ISessionService _sessionService;
    private readonly IOuterApiClient _outerApiClient;

    public CheckYourAnswersController(ISessionService sessionService, IOuterApiClient outerApiClient)
    {
        _sessionService = sessionService;
        _outerApiClient = outerApiClient;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var sessionModel = _sessionService.Get<OnboardingSessionModel>();
        CheckYourAnswersViewModel model = new(Url, sessionModel);
        return View(ViewPath, model);
    }

    [HttpPost]
    public async Task<IActionResult> Post()
    {
        var onboardingSessionModel = _sessionService.Get<OnboardingSessionModel>();
        var result = await _outerApiClient.PostApprenticeMember(GenerateCreateApprenticeMemberRequest(onboardingSessionModel));

        _sessionService.Set(Constants.SessionKeys.Member.MemberId, result.MemberId.ToString().ToLower());
        _sessionService.Set(Constants.SessionKeys.Member.Status, MemberStatus.Live.ToString());

        return View(ApplicationSubmittedViewPath);
    }

    private CreateApprenticeMemberRequest GenerateCreateApprenticeMemberRequest(OnboardingSessionModel source)
    {
        CreateApprenticeMemberRequest request = new()
        {
            ApprenticeId = source.ApprenticeDetails.ApprenticeId,
            JoinedDate = DateTime.UtcNow,
            OrganisationName = source.GetProfileValue(ProfileConstants.ProfileIds.EmployerName)!,
            RegionId = source.RegionId.GetValueOrDefault()
        };
        request.ProfileValues.AddRange(source.ProfileData.Where(p => !string.IsNullOrWhiteSpace(p.Value)).Select(p => new ProfileValue(p.Id, p.Value!)));
        request.Email = source.ApprenticeDetails.Email;
        request.FirstName = User.FindFirstValue(IdentityClaims.GivenName)!;
        request.LastName = User.FindFirstValue(IdentityClaims.FamilyName)!;
        request.ReceiveNotifications = source.ReceiveNotifications!.Value;
        request.MemberNotificationEventFormatValues.AddRange(
            source.EventTypes?.Select(p => new MemberNotificationEventFormatValues(p.EventType, p.Ordering, p.IsSelected)) ?? Enumerable.Empty<MemberNotificationEventFormatValues>()
        );
        request.MemberNotificationLocationValues.AddRange(
            source.NotificationLocations?.Select(p => new MemberNotificationLocationValues(p.LocationName, p.Radius, p.GeoPoint[0], p.GeoPoint[1]))
            ?? Enumerable.Empty<MemberNotificationLocationValues>()
        );
        return request;
    }
}
