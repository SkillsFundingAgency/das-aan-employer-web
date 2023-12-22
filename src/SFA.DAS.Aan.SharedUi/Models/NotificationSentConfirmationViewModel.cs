﻿namespace SFA.DAS.Aan.SharedUi.Models;

public class NotificationSentConfirmationViewModel : INetworkHubLink
{
    public NotificationSentConfirmationViewModel(string networkDirectoryUrl)
    {
        NetworkDirectoryUrl = networkDirectoryUrl;
    }
    public string NetworkDirectoryUrl { get; set; } = null!;
    public string? NetworkHubLink { get; set; }
};
