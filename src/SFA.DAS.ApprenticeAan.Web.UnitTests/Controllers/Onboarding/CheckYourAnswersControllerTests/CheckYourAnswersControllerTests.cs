﻿using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.ApprenticeAan.Domain.Constants;
using SFA.DAS.ApprenticeAan.Domain.Interfaces;
using SFA.DAS.ApprenticeAan.Web.Controllers.Onboarding;
using SFA.DAS.ApprenticeAan.Web.Infrastructure;
using SFA.DAS.ApprenticeAan.Web.Models;
using SFA.DAS.ApprenticeAan.Web.Models.Onboarding;
using SFA.DAS.ApprenticeAan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ApprenticeAan.Web.UnitTests.Controllers.Onboarding.CheckYourAnswersControllerTests;

[TestFixture]
public class CheckYourAnswersControllerTests
{
    [MoqAutoData]
    public void Index_ChangeJobTitle_ReturnsJobTitleViewResult(
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Greedy] CheckYourAnswersController sut,
    OnboardingSessionModel sessionModel)
    {
        var jobTitle = "Some Title";
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.Onboarding.CurrentJobTitle);
        sessionModel.ProfileData.Add(new ProfileModel { Id = ProfileDataId.JobTitle, Value = jobTitle });
        sessionServiceMock.Setup(s => s.Get<OnboardingSessionModel>()).Returns(sessionModel);

        var result = sut.Index();

        sessionServiceMock.Verify(s => s.Set(It.Is<OnboardingSessionModel>(m => m.HasSeenPreview)));
        result.As<ViewResult>().Model.As<CheckYourAnswersViewModel>().JobTitle.Should().Be(jobTitle);
        result.As<ViewResult>().Model.As<CheckYourAnswersViewModel>().JobTitleChangeLink.Should().Be(TestConstants.DefaultUrl);
    }

    [MoqAutoData]
    public void Get_ViewModel_BackLink_RedirectsRouteToNameOfEmployer(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] CurrentJobTitleController sut,
        OnboardingSessionModel sessionModel)
    {
        sessionModel.HasSeenPreview = false;
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.Onboarding.NameOfEmployer);
        sessionModel.ProfileData.Add(new ProfileModel { Id = ProfileDataId.JobTitle, Value = "Some Title" });
        sessionServiceMock.Setup(s => s.Get<OnboardingSessionModel>()).Returns(sessionModel);

        var result = sut.Get();

        result.As<ViewResult>().Model.As<CurrentJobTitleViewModel>().BackLink.Should().Be(TestConstants.DefaultUrl);
    }
}