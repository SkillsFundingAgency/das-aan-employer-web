﻿using SFA.DAS.ApprenticeAan.Domain.OuterApi.Responses;
using SFA.DAS.ApprenticeAan.Web.Models.NetworkEvents;

namespace SFA.DAS.ApprenticeAan.Web.Models;

public class NetworkEventsViewModel
{
    public PaginationModel Pagination { get; set; } = new PaginationModel();

    public int TotalCount { get; set; }

    public List<CalendarEventSummary> CalendarEvents { get; set; } = new List<CalendarEventSummary>();

    // filter choices made by the user
    public EventFilterChoices FilterChoices { get; set; } = new EventFilterChoices();

    // model that builds the filter choices into a model that can populate FE
    public List<SelectedFilter> SelectedFilters { get; set; } = new List<SelectedFilter>();

    // Regions is populated from the Region table from aan-hub db to give a lookup for event types
    public List<Region> Regions { get; set; } = new List<Region>();

    public bool ShowFilterOptions => SelectedFilters.Any();


    public static implicit operator NetworkEventsViewModel(GetCalendarEventsQueryResult result) => new()
    {
        Pagination = new PaginationModel
        {
            Page = result.Page,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages
        },
        TotalCount = result.TotalCount,
        CalendarEvents = result.CalendarEvents.ToList()
    };
}