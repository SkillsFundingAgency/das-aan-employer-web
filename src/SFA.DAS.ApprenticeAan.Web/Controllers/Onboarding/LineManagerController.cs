﻿using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeAan.Domain.Interfaces;
using SFA.DAS.ApprenticeAan.Web.Infrastructure;
using SFA.DAS.ApprenticeAan.Web.Models;
using SFA.DAS.ApprenticeAan.Web.Models.Onboarding;

namespace SFA.DAS.ApprenticeAan.Web.Controllers.Onboarding;

[Route("onboarding/line-manager", Name = RouteNames.Onboarding.LineManager)]
public class LineManagerController : Controller
{
    public const string ViewPath = "~/Views/Onboarding/LineManager.cshtml";
    private readonly ISessionService _sessionService;
    private readonly IValidator<LineManagerSubmitModel> _validator;

    public LineManagerController(ISessionService sessionService,
        IValidator<LineManagerSubmitModel> validator)
    {
        _validator = validator;
        _sessionService = sessionService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var model = new LineManagerViewModel()
        {
            BackLink = Url.RouteUrl(@RouteNames.Onboarding.TermsAndConditions)!
        };
        return View(ViewPath, model);
    }

    [HttpPost]
    public IActionResult Post(LineManagerSubmitModel submitmodel)
    {
        var sessionModel = _sessionService.Get<OnboardingSessionModel>();

        ValidationResult result = _validator.Validate(submitmodel);

        var model = new LineManagerViewModel()
        {
            BackLink = Url.RouteUrl(@RouteNames.Onboarding.TermsAndConditions)!
        };

        if (!result.IsValid)
        {
            result.AddToModelState(this.ModelState);
            return View(ViewPath, model);
        }

        sessionModel.HasEmployersApproval = (bool)submitmodel.HasEmployersApproval!;
        _sessionService.Set(sessionModel);

        return View(ViewPath, model);
    }
}