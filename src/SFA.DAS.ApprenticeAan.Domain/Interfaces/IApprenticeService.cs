﻿using SFA.DAS.ApprenticeAan.Domain.OuterApi.Responses;

namespace SFA.DAS.ApprenticeAan.Domain.Interfaces;

public interface IApprenticeService
{
    Task<Apprentice?> GetApprentice(Guid apprenticeId);
    Task<StagedApprentice?> GetStagedApprentice(string lastName, DateTime dateOfBirth, string email, CancellationToken cancellationToken = new());
}
