﻿using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeAan.Web.Extensions;
using SFA.DAS.ApprenticeAan.Web.Infrastructure;
using SFA.DAS.ApprenticeAan.Web.Models.NetworkEvents;
using System.Globalization;

namespace SFA.DAS.ApprenticeAan.Web.Services;

public static class FilterBuilder
{
    public static List<SelectedFilter> Build(GetNetworkEventsRequest request, IUrlHelper urlHelper, IEnumerable<ChecklistLookup> eventFormatLookups, IEnumerable<ChecklistLookup> eventTypeLookups, IEnumerable<ChecklistLookup> regionLookups)
    {
        var filters = new List<SelectedFilter>();
        var fullQueryParameters = BuildQueryParameters(request);

        if (request.FromDate.HasValue)
        {
            filters.AddFilterItems(urlHelper, fullQueryParameters, new[] { request.FromDate.Value.ToScreenString() }, "From date", "fromDate", Enumerable.Empty<ChecklistLookup>());
        }

        if (request.ToDate.HasValue)
        {
            filters.AddFilterItems(urlHelper, fullQueryParameters, new[] { request.ToDate.Value.ToScreenString() }, "To date", "toDate", Enumerable.Empty<ChecklistLookup>());
        }

        filters.AddFilterItems(urlHelper, fullQueryParameters, request.EventFormat.Select(e => e.ToString()), "Event format", "eventFormat", eventFormatLookups);

        filters.AddFilterItems(urlHelper, fullQueryParameters, request.CalendarId.Select(e => e.ToString()), "Event type", "calendarId", eventTypeLookups);

        filters.AddFilterItems(urlHelper, fullQueryParameters, request.RegionId.Select(e => e.ToString()), "Region", "regionId", regionLookups);

        return filters;
    }

    public static string BuildQueryString(GetNetworkEventsRequest request, IUrlHelper url)
    {
        var fullQueryParameters = BuildQueryParameters(request);
        var querystring = BuildQueryString(url, fullQueryParameters, "none");

        return !string.IsNullOrEmpty(querystring) ? querystring : url.RouteUrl(RouteNames.NetworkEvents)!;
    }

    private static void AddFilterItems(this ICollection<SelectedFilter> filters, IUrlHelper url, List<string> fullQueryParameters, IEnumerable<string> selectedValues, string fieldName, string parameterName, IEnumerable<ChecklistLookup> lookups)
    {
        if (!selectedValues.Any()) return;

        var filter = new SelectedFilter
        {
            FieldName = fieldName,
            FieldOrder = filters.Count + 1
        };

        int i = 0;

        foreach (var value in selectedValues)
        {
            var v = lookups.Any() ? lookups.First(l => l.Value == value).Name : value;
            filter.Filters.Add(BuildFilterItem(url, fullQueryParameters, BuildQueryParameter(parameterName, value), v, ++i));
        }

        filters.Add(filter);
    }

    private static List<string> BuildQueryParameters(GetNetworkEventsRequest request)
    {
        var queryParameters = new List<string>();

        if (request.FromDate != null)
        {
            queryParameters.Add(BuildDateQueryParameter("fromDate", request.FromDate.GetValueOrDefault()));
        }

        if (request.ToDate != null)
        {
            queryParameters.Add(BuildDateQueryParameter("toDate", request.ToDate.GetValueOrDefault()));
        }

        queryParameters.AddRange(request.EventFormat.Select(eventFormat => "eventFormat=" + eventFormat));
        queryParameters.AddRange(request.CalendarId.Select(eventType => "calendarId=" + eventType));
        queryParameters.AddRange(request.RegionId.Select(region => "regionId=" + region));

        return queryParameters;
    }

    private static EventFilterItem BuildFilterItem(IUrlHelper url, List<string> queryParameters, string filterToRemove, string filterValue, int filterFieldOrder)
        =>
        new()
        {
            ClearFilterLink = BuildQueryString(url, queryParameters, filterToRemove),
            Order = filterFieldOrder,
            Value = filterValue
        };

    private static string? BuildQueryString(IUrlHelper url, List<string> queryParameters, string filterToRemove)
    {
        var queryParametersToBuild = queryParameters.Where(s => s != filterToRemove);

        return queryParametersToBuild.Any() ? $"{url.RouteUrl(RouteNames.NetworkEvents)}?{string.Join('&', queryParametersToBuild)}" : url.RouteUrl(RouteNames.NetworkEvents);
    }

    private static string BuildDateQueryParameter(string name, DateTime value) => $"{name}={value.ToApiString()}";

    private static string BuildQueryParameter(string name, string value)
    {
        return DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
            out DateTime parsedDate) ?
            $"{name}={parsedDate.ToApiString()}" :
            $"{name}={value}";
    }
}
