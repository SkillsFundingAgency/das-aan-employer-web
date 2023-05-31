﻿namespace SFA.DAS.ApprenticeAan.Domain.Interfaces;

public interface IOuterApiClient
{
    [Get("/regions")]
    Task<GetRegionsResult> GetRegions();

    [Get("/profiles/{userType}")]
    Task<GetProfilesResult> GetProfilesByUserType([Path("userType")] string userType);

    [Get("/apprentices/{apprenticeId}/account")]
    [AllowAnyStatusCode]
    Task<Response<ApprenticeAccount?>> GetApprenticeAccount([Path] Guid apprenticeId);

    [Get("/locations")]
    Task<GetAddressesResult> GetAddresses([Query] string query);

    [Get("/locations/{postcode}")]
    [AllowAnyStatusCode]
    Task<Response<GetCoordinatesResult>> GetCoordinates([Path] string postcode);

    [Get("/apprentices/{apprenticeId}")]
    [AllowAnyStatusCode]
    Task<Response<Apprentice?>> GetApprentice([Path] Guid apprenticeId);

    [Get("calendarEvents")]
    Task<GetCalendarEventsQueryResult> GetCalendarEvents([Header(Constants.RequestHeaders.RequestedByMemberIdHeader)] Guid requestedByMemberId, CancellationToken cancellationToken);

    [Get("/myapprenticeship/{apprenticeId}")]
    [AllowAnyStatusCode]
    Task<Response<MyApprenticeship>> GetMyApprenticeship([Path] Guid apprenticeId, CancellationToken cancellationToken);

    [Post("/myapprenticeship")]
    Task CreateMyApprenticeship([Body] CreateMyApprenticeshipRequest body, CancellationToken cancellationToken);

    [Post("/apprentices")]
    Task<CreateApprenticeMemberResponse> PostApprenticeMember([Body] CreateApprenticeMemberRequest request);

    [Get("/calendarevents/{calendarEventId}")]
    Task<CalendarEvent> GetEventDetails([Path] Guid calendarEventId,
        [Header(Constants.ApiHeaders.RequestedByMemberIdHeader)] Guid requestedByMemberId,
        CancellationToken cancellationToken);

    [Get("/stagedapprentices")]
    [AllowAnyStatusCode]
    Task<Response<StagedApprentice?>> GetStagedApprentice([Query] string lastName, [Query] string dateOfBirth, [Query] string email, CancellationToken cancellationToken);
}
