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
public class CheckYourAnswersControllerGetTests
{
    [MoqAutoData]
    public void Get_ReturnsViewResult(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] CheckYourAnswersController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.Onboarding.CurrentJobTitle);
        var jobTitle = "Test Job Title";
        OnboardingSessionModel sessionModel = new();
        sessionModel.ProfileData.Add(new ProfileModel { Id = ProfileDataId.JobTitle, Value = jobTitle });
        sessionServiceMock.Setup(s => s.Get<OnboardingSessionModel>()).Returns(sessionModel);

        var result = sut.Get();

        sessionServiceMock.Verify(s => s.Set(It.Is<OnboardingSessionModel>(m => m.HasSeenPreview)));
        result.As<ViewResult>().Model.As<CheckYourAnswersViewModel>().JobTitle.Should().Be(jobTitle);
        result.As<ViewResult>().Model.As<CheckYourAnswersViewModel>().JobTitleChangeLink.Should().Be(TestConstants.DefaultUrl);
    }

    [MoqAutoData]
    public void JobTitleChangeLink_LinksToJobTitleRoute(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] CheckYourAnswersController sut,
        OnboardingSessionModel sessionModel)
    {
        var jobTitle = "Some Title";
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.Onboarding.CurrentJobTitle);
        sessionModel.ProfileData.Add(new ProfileModel { Id = ProfileDataId.JobTitle, Value = jobTitle });
        sessionServiceMock.Setup(s => s.Get<OnboardingSessionModel>()).Returns(sessionModel);

        var result = sut.Get();

        sessionServiceMock.Verify(s => s.Set(It.Is<OnboardingSessionModel>(m => m.HasSeenPreview)));
        result.As<ViewResult>().Model.As<CheckYourAnswersViewModel>().JobTitle.Should().Be(jobTitle);
        result.As<ViewResult>().Model.As<CheckYourAnswersViewModel>().JobTitleChangeLink.Should().Be(TestConstants.DefaultUrl);
    }

    [MoqAutoData]
    public void Get_ReturnsViewResult_ValidReasons(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] CheckYourAnswersController sut,
        OnboardingSessionModel sessionModel,
    string reasonsUrl)
    {
        var reasonForJoiningTheNetwork = "The reason to join the network.";
        var jobTitle = "Some Title";

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.Onboarding.ReasonToJoin, reasonsUrl);
        sessionModel.ApprenticeDetails.ReasonForJoiningTheNetwork = reasonForJoiningTheNetwork;
        sessionModel.ProfileData.Add(new ProfileModel { Id = ProfileDataId.JobTitle, Value = jobTitle });

        sessionServiceMock.Setup(s => s.Get<OnboardingSessionModel>()).Returns(sessionModel);

        var result = sut.Get();

        result.As<ViewResult>().Model.As<CheckYourAnswersViewModel>().ReasonForJoiningTheNetwork.Should().Be(reasonForJoiningTheNetwork);
        result.As<ViewResult>().Model.As<CheckYourAnswersViewModel>().ReasonForJoiningTheNetworkChangeLink.Should().Be(reasonsUrl);
    }
    
    [MoqAutoData]
    public void Get_ReturnsViewResult_ValidRegion(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] CheckYourAnswersController sut,
        string regionsUrl)
    {
        var regionName = "London";
        var jobTitle = "Some Title";

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.Onboarding.Regions, regionsUrl);
        OnboardingSessionModel sessionModel = new OnboardingSessionModel();
        sessionModel.RegionName = regionName;
        sessionModel.ProfileData.Add(new ProfileModel { Id = ProfileDataId.JobTitle, Value = jobTitle });

        sessionServiceMock.Setup(s => s.Get<OnboardingSessionModel>()).Returns(sessionModel);

        var result = sut.Get();

        result.As<ViewResult>().Model.As<CheckYourAnswersViewModel>().Region.Should().Be(regionName);
        result.As<ViewResult>().Model.As<CheckYourAnswersViewModel>().RegionChangeLink.Should().Be(regionsUrl);
    }

    [MoqAutoData]
    public void Get_ReturnsViewResult_ValidAreasOfInterestAndSetsChangeLink(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] CheckYourAnswersController sut,
        string areasOfInterestUrl)
    {
        var jobTitle = "Some Job Title";

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.Onboarding.AreasOfInterest, areasOfInterestUrl);
        OnboardingSessionModel sessionModel = new OnboardingSessionModel();
        sessionModel.ProfileData.Add(new ProfileModel { Id = ProfileDataId.JobTitle, Value = jobTitle });
        sessionModel.ProfileData.Add(new ProfileModel { Id = 1, Category = "Events", Value = "Presenting at online events" });
        sessionModel.ProfileData.Add(new ProfileModel { Id = 2, Category = "Events", Value = "Networking at events in person" });
        sessionModel.ProfileData.Add(new ProfileModel { Id = 3, Category = "Promotions", Value = "Carrying out and writing up case studies" });


        sessionServiceMock.Setup(s => s.Get<OnboardingSessionModel>()).Returns(sessionModel);

        var result = sut.Get();

        result.As<ViewResult>().Model.As<CheckYourAnswersViewModel>().AreasOfInterest.Should().Be(CheckYourAnswersViewModel.GetAreasOfInterest(sessionModel));
        result.As<ViewResult>().Model.As<CheckYourAnswersViewModel>().AreasOfInterestChangeLink.Should().Be(areasOfInterestUrl);
    }
}