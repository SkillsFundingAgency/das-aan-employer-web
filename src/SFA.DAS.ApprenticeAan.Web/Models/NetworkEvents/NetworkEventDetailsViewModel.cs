﻿using SFA.DAS.ApprenticeAan.Domain.Constants;
using SFA.DAS.ApprenticeAan.Domain.Interfaces;
using SFA.DAS.ApprenticeAan.Domain.OuterApi.Responses;
using SFA.DAS.ApprenticeAan.Web.UrlHelpers;

namespace SFA.DAS.ApprenticeAan.Web.Models.NetworkEvents;

public class NetworkEventDetailsViewModel
{
    public Guid CalendarEventId { get; init; }
    public string CalendarName { get; init; }
    public EventFormat EventFormat { get; init; }
    public string EventFormatAppTagSuffix => EventFormat.ToString().ToLower();
    public string EventFormatAppTagValue => GetEventFormatAppTagValue();
    public string StartDate { get; init; }
    public string EndDate { get; init; }
    public string StartTime { get; init; }
    public string EndTime { get; init; }
    public string Description { get; init; }
    public string? Summary { get; init; }
    public LocationDetails? LocationDetails { get; init; }
    public string? EventLink { get; init; }
    public string ContactName { get; init; }
    public string ContactEmail { get; init; }
    public string? CancelReason { get; init; }
    public string PartialViewName => GetPartialViewName();
    public IReadOnlyList<Attendee> Attendees { get; }
    public int AttendeeCount => Attendees.Count;
    public IReadOnlyList<EventGuest> EventGuests { get; }
    public bool IsSignedUp { get; init; }
    public string EmailLink => MailtoLinkValue.FromAddressAndSubject(ContactEmail, Description);

    public bool ShowMap { get; init; }
    public string? StaticMapImageUrl { get; }
    public string? FullMapUrl { get; }

    public NetworkEventDetailsViewModel(CalendarEvent source, Guid memberId, IGoogleMapsService googleMapsService)
    {
        CalendarEventId = source.CalendarEventId;
        CalendarName = source.CalendarName;
        EventFormat = source.EventFormat;
        StartDate = source.StartDate.ToString("dddd, d MMMM yyyy");
        EndDate = source.EndDate.ToString("dddd, d MMMM yyyy");
        StartTime = source.StartDate.ToString("h:mm tt");
        EndTime = source.EndDate.ToString("h:mm tt");
        Description = source.Description;
        Summary = source.Summary;
        EventLink = source.EventLink;
        ContactName = source.ContactName;
        ContactEmail = source.ContactEmail;
        CancelReason = source.CancelReason;
        Attendees = source.Attendees;
        EventGuests = source.EventGuests;

        IsSignedUp = Attendees.Any(a => a.MemberId == memberId);

        if (EventFormat != EventFormat.Online)
        {
            LocationDetails = new LocationDetails()
            {
                Location = source.Location,
                Postcode = source.Postcode,
                Distance = source.Distance,
            };
            ShowMap = source.Latitude.HasValue && source.Longitude.HasValue;
            if (ShowMap)
            {
                StaticMapImageUrl = googleMapsService.GetStaticMapUrl(source.Latitude.GetValueOrDefault(), source.Longitude.GetValueOrDefault());
                FullMapUrl = googleMapsService.GetFullMapUrl(source.Latitude.GetValueOrDefault(), source.Longitude.GetValueOrDefault());
            }
        }
    }

    private string GetPartialViewName()
    {
        return EventFormat switch
        {
            EventFormat.Online => "_OnlineEventPartial.cshtml",
            EventFormat.InPerson => "_InPersonEventPartial.cshtml",
            _ => throw new NotImplementedException($"Failed to find a matching partial view for event format \"{EventFormat}\""),
        };
    }

    private string GetEventFormatAppTagValue()
    {
        return EventFormat switch
        {
            EventFormat.Online => EventFormat.Online.ToString(),
            EventFormat.Hybrid => EventFormat.Hybrid.ToString(),
            EventFormat.InPerson => "In person",
            _ => throw new NotImplementedException($"Failed to find a matching app tag value for event format \"{EventFormat}\""),
        };
    }
}
