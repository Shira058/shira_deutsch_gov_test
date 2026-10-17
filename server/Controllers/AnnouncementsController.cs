using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using RA_MuniAnnouncements.DTOs;
using RA_MuniAnnouncements.Services.Interfaces;

namespace RA_MuniAnnouncements.Controllers;

[ApiController]
[Route("api/announcements")]
public sealed class AnnouncementsController(IAnnouncementsService announcementsService) : ControllerBase
{
    /// <summary>Returns the active announcements of an authority, normalized by its site settings.</summary>
    /// <param name="authorityCode">Authority code: 1-10 ASCII digits.</param>
    /// <param name="cancellationToken">Cancelled when the client disconnects.</param>
    [HttpGet("{authorityCode}")]
    [ProducesResponseType<AnnouncementsResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public Task<AnnouncementsResponseDto> Get(
        // [0-9] rather than \d, which also matches non-ASCII digits such as Arabic-Indic ones.
        // [ApiController] turns a validation failure into a 400 ProblemDetails before the action runs.
        [RegularExpression("^[0-9]{1,10}$", ErrorMessage = "authorityCode must be 1-10 digits.")] string authorityCode,
        CancellationToken cancellationToken) =>
        announcementsService.GetActiveAnnouncementsAsync(authorityCode, cancellationToken);
}
