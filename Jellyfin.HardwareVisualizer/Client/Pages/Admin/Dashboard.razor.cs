﻿using Microsoft.AspNetCore.Components;

namespace Jellyfin.HardwareVisualizer.Client.Pages.Admin;

public partial class Dashboard
{
	[Inject]
	public HttpClient HttpClient { get; set; }


	protected override async Task OnInitializedAsync()
	{
	}
}