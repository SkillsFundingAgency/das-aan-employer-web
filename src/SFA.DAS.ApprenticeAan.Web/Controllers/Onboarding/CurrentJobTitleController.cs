﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeAan.Web.Filters;
using SFA.DAS.ApprenticeAan.Web.Infrastructure;
using SFA.DAS.ApprenticeAan.Web.Models;

namespace SFA.DAS.ApprenticeAan.Web.Controllers.Onboarding
{
    [ExcludeFromCodeCoverage]
    [Route("onboarding/current-job-title", Name = RouteNames.Onboarding.CurrentJobTitle)]
    [RequiredSessionModel(typeof(OnboardingSessionModel))]
    public class CurrentJobTitleController : Controller
    {
        public const string ViewPath = "~/Views/Onboarding/CurrentJobTitle.cshtml";

        [HttpGet]
        public IActionResult Get()
        {
            return View(ViewPath);
        }
    }
}