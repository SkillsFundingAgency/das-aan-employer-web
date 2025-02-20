﻿using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Aan.SharedUi.Constants;
using SFA.DAS.ApprenticeAan.Domain.Interfaces;
using SFA.DAS.ApprenticeAan.Web.Controllers.Onboarding;
using SFA.DAS.ApprenticeAan.Web.Infrastructure;
using SFA.DAS.ApprenticeAan.Web.Models;
using SFA.DAS.ApprenticeAan.Web.Models.Onboarding;
using SFA.DAS.ApprenticeAan.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ApprenticeAan.Web.UnitTests.Controllers.Onboarding.PreviousEngagementControllerTests;

[TestFixture]
public class PreviousEngagementControllerGetTests
{
    [MoqAutoData]
    public void Get_ViewResult_SelectedYes_HasCorrectViewPath(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] PreviousEngagementController sut)
    {
        sut.AddUrlHelperMock();
        OnboardingSessionModel sessionModel = new();
        sessionServiceMock.Setup(s => s.Get<OnboardingSessionModel>()).Returns(sessionModel);
        sessionModel.ProfileData.Add(new ProfileModel { Id = ProfileConstants.ProfileIds.EngagedWithAPreviousAmbassadorInTheNetworkApprentice, Value = "True" });

        var result = sut.Get();

        result.As<ViewResult>().ViewName.Should().Be(PreviousEngagementController.ViewPath);
    }

    [MoqAutoData]
    public void Get_ViewResult_SelectedNo_HasCorrectViewPath(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] PreviousEngagementController sut)
    {
        sut.AddUrlHelperMock();
        OnboardingSessionModel sessionModel = new();
        sessionModel.ProfileData.Add(new ProfileModel { Id = ProfileConstants.ProfileIds.EngagedWithAPreviousAmbassadorInTheNetworkApprentice, Value = "False" });
        sessionServiceMock.Setup(s => s.Get<OnboardingSessionModel>()).Returns(sessionModel);

        var result = sut.Get();

        result.As<ViewResult>().ViewName.Should().Be(PreviousEngagementController.ViewPath);
    }

    [MoqAutoData]
    public void Get_ViewModel_HasBackLink(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] PreviousEngagementController sut)
    {
        OnboardingSessionModel sessionModel = new();
        sessionModel.ProfileData.Add(new ProfileModel { Id = ProfileConstants.ProfileIds.EngagedWithAPreviousAmbassadorInTheNetworkApprentice, Value = "True" });
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.Onboarding.ReceiveNotifications);
        sessionServiceMock.Setup(s => s.Get<OnboardingSessionModel>()).Returns(sessionModel);

        var result = sut.Get();

        result.As<ViewResult>().Model.As<PreviousEngagementViewModel>().BackLink.Should().Be(TestConstants.DefaultUrl);
    }

    [MoqAutoData]
    public void Get_ViewModel_DefaultNullValueSetsHasPreviousEngagementToNull(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] PreviousEngagementController sut)
    {
        OnboardingSessionModel sessionModel = new();
        sessionModel.ProfileData.Add(new ProfileModel { Id = ProfileConstants.ProfileIds.EngagedWithAPreviousAmbassadorInTheNetworkApprentice, Value = null });
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.Onboarding.AreasOfInterest);
        sessionServiceMock.Setup(s => s.Get<OnboardingSessionModel>()).Returns(sessionModel);

        var result = sut.Get();

        result.As<ViewResult>().Model.As<PreviousEngagementViewModel>().HasPreviousEngagement.Should().BeNull();
    }

    [MoqAutoData]
    public void Get_ViewModel_SelectedYes_RestoreFromSession(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] PreviousEngagementController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.Onboarding.AreasOfInterest);
        OnboardingSessionModel sessionModel = new();
        sessionServiceMock.Setup(s => s.Get<OnboardingSessionModel>()).Returns(sessionModel);
        sessionModel.ProfileData.Add(new ProfileModel { Id = ProfileConstants.ProfileIds.EngagedWithAPreviousAmbassadorInTheNetworkApprentice, Value = "True" });

        var result = sut.Get();

        result.As<ViewResult>().Model.As<PreviousEngagementViewModel>().HasPreviousEngagement.Should().BeTrue();
    }

    [MoqAutoData]
    public void Get_ViewModel_SelectedNo_RestoreFromSession(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] PreviousEngagementController sut)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.Onboarding.AreasOfInterest);
        OnboardingSessionModel sessionModel = new();
        sessionModel.ProfileData.Add(new ProfileModel { Id = ProfileConstants.ProfileIds.EngagedWithAPreviousAmbassadorInTheNetworkApprentice, Value = "False" });
        sessionServiceMock.Setup(s => s.Get<OnboardingSessionModel>()).Returns(sessionModel);

        var result = sut.Get();

        result.As<ViewResult>().Model.As<PreviousEngagementViewModel>().HasPreviousEngagement.Should().BeFalse();
    }

    [MoqAutoData]
    public void Get_ViewModelHasSeenPreviewIsTrue_BackLinkSetsToCheckYourAnswers(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] PreviousEngagementController sut,
        string checkYourAnswersUrl)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.Onboarding.CheckYourAnswers, checkYourAnswersUrl);

        OnboardingSessionModel sessionModel = new();
        sessionModel.ProfileData.Add(new ProfileModel { Id = ProfileConstants.ProfileIds.EngagedWithAPreviousAmbassadorInTheNetworkApprentice, Value = "False" });
        sessionModel.HasSeenPreview = true;

        sessionServiceMock.Setup(s => s.Get<OnboardingSessionModel>()).Returns(sessionModel);

        var result = sut.Get();

        result.As<ViewResult>().Model.As<PreviousEngagementViewModel>().BackLink.Should().Be(checkYourAnswersUrl);
    }

    [MoqAutoData]
    public void Get_ViewModelHasSeenPreviewIsFalse_BackLinkSetsToAreasOfInterest(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] PreviousEngagementController sut,
        string receiveNotificationsUrl)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.Onboarding.ReceiveNotifications, receiveNotificationsUrl);

        OnboardingSessionModel sessionModel = new();
        sessionModel.ProfileData.Add(new ProfileModel { Id = ProfileConstants.ProfileIds.EngagedWithAPreviousAmbassadorInTheNetworkApprentice, Value = "False" });
        sessionModel.HasSeenPreview = false;

        sessionServiceMock.Setup(s => s.Get<OnboardingSessionModel>()).Returns(sessionModel);

        var result = sut.Get();

        result.As<ViewResult>().Model.As<PreviousEngagementViewModel>().BackLink.Should().Be(receiveNotificationsUrl);
    }
}