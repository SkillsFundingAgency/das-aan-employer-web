﻿using FluentValidation;
using SFA.DAS.ApprenticeAan.Web.Models.Onboarding;

namespace SFA.DAS.ApprenticeAan.Web.Validators.Onboarding;

public class AreasOfInterestSubmitModelValidator : AbstractValidator<AreasOfInterestSubmitModel>
{
    public const string NoSelectionErrorMessage = "Select at least one way to be involved";

    public AreasOfInterestSubmitModelValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(x => x.AreasOfInterest).Must(c => c.Any(p => p.IsSelected)).WithMessage(NoSelectionErrorMessage);
    }
}