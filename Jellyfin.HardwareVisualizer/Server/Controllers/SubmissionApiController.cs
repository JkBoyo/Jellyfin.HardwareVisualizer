using System.ComponentModel.DataAnnotations;
using System.Net;
using Jellyfin.HardwareVisualizer.Database;
using Jellyfin.HardwareVisualizer.Server.Services.Mapper;
using Jellyfin.HardwareVisualizer.Server.Services.Submission;
using Jellyfin.HardwareVisualizer.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.HardwareVisualizer.Server.Controllers;

/// <summary>
///		Api Controller for submitting or getting the results of an Hardware Survey.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SubmissionApiController : ControllerBase
{
	private readonly ISubmissionService _submissionService;
	private readonly ILogger<SubmissionApiController> _logger;
	private readonly IMapperService _mapperService;

	public SubmissionApiController(ISubmissionService submissionService,
		ILogger<SubmissionApiController> logger,
		IMapperService mapperService)
	{
		_submissionService = submissionService;
		_logger = logger;
		_mapperService = mapperService;
	}
	
	/// <summary>
	///		Posts a new Hardware Survey Result.
	/// </summary>
	/// <param name="submission">A doc containing all set values for the hardware Survey.</param>
	/// <returns>The ID of the single hardware survey.</returns>
	[HttpPost()]
	[ProducesResponseType<string>(StatusCodes.Status200OK)]
	public async Task<IActionResult> Submit([FromBody, Required] TranscodeSubmission submission)
	{
		return Ok(await _submissionService.SubmitHardwareSurvey(submission));
	}
	
	/// <summary>
	///		[Internal] 
	/// </summary>
	/// <returns></returns>
	[HttpPost("Recalc")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> Recalc()
	{
		await _submissionService.RecalcHardwareStats();
		return Ok();
	}

	/// <summary>
	///		Gets the Hardware Survey result of a single submission.
	/// </summary>
	/// <param name="id">The id as returned by <code>POST /</code></param>
	/// <returns></returns>
	[HttpGet("single/{Id}")]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType<HardwareSurveySubmission>(StatusCodes.Status200OK)]
	public async Task<IActionResult> GetSingleData([FromQuery] Guid id)
	{
		var data = await _submissionService.GetSingleSubmission(id);
		if (data is null)
		{
			return NotFound();
		}
		return Ok(data);
	}

	/// <summary>
	///		Gets a set of aggregated data points of all submissions for the given <see cref="deviceId"/>
	/// </summary>
	/// <param name="deviceId"></param>
	/// <returns></returns>
	[HttpGet("")]
	[ProducesResponseType<IEnumerable<HardwareDisplayModel>>(StatusCodes.Status200OK)]
	public async Task<IActionResult> GetData([FromQuery, Required]string deviceId)
	{
		var data = await _submissionService.GetSubmissions(deviceId);
		return Ok(_mapperService.ViewModelMapper.Map<IEnumerable<HardwareDisplayModel>>(data));
	}
}