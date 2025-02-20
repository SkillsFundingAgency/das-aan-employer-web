﻿using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Aan.SharedUi.Constants;
using SFA.DAS.ApprenticeAan.Domain.Interfaces;
using SFA.DAS.ApprenticeAan.Web.Infrastructure;
using SFA.DAS.ApprenticeAan.Web.Models;
using SFA.DAS.ApprenticeAan.Web.Models.Onboarding;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;

namespace SFA.DAS.ApprenticeAan.Web.Controllers.Onboarding;

[Route("onboarding/current-job-title", Name = RouteNames.Onboarding.CurrentJobTitle)]
[Authorize]
[HideNavigationBar(true, true)]
public class CurrentJobTitleController : Controller
{
    public const string ViewPath = "~/Views/Onboarding/CurrentJobTitle.cshtml";
    private readonly ISessionService _sessionService;
    private readonly IValidator<CurrentJobTitleSubmitModel> _validator;

    public CurrentJobTitleController(ISessionService sessionService,
    IValidator<CurrentJobTitleSubmitModel> validator)
    {
        _sessionService = sessionService;
        _validator = validator;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var sessionModel = _sessionService.Get<OnboardingSessionModel>();
        var model = GetViewModel(sessionModel);

        model.JobTitle = sessionModel.GetProfileValue(ProfileConstants.ProfileIds.JobTitle);

        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult Post(CurrentJobTitleSubmitModel submitModel)
    {
        var sessionModel = _sessionService.Get<OnboardingSessionModel>();

        ValidationResult result = _validator.Validate(submitModel);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            var model = GetViewModel(sessionModel);
            return View(ViewPath, model);
        }

        sessionModel.SetProfileValue(ProfileConstants.ProfileIds.JobTitle, submitModel.JobTitle!.Trim());
        _sessionService.Set(sessionModel);

        return RedirectToRoute(sessionModel.HasSeenPreview ? RouteNames.Onboarding.CheckYourAnswers : RouteNames.Onboarding.AreasOfInterest);
    }

    private CurrentJobTitleViewModel GetViewModel(OnboardingSessionModel sessionModel)
    {
        return new CurrentJobTitleViewModel()
        {
            BackLink = sessionModel.HasSeenPreview ? Url.RouteUrl(@RouteNames.Onboarding.CheckYourAnswers)! : Url.RouteUrl(@RouteNames.Onboarding.EmployerSearch)!,
        };
    }
}