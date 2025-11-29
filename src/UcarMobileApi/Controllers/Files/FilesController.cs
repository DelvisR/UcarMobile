using Microsoft.AspNetCore.Mvc;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs.Files;
using UcarMobileApi.Authorization;
using UcarMobileApi.Core.Entities.Storage;

namespace UcarMobileApi.Controllers.Files;

/// <summary>
/// Handles all file storage operations
/// </summary>
[ApiController]
[Route("api/files")]
[Produces("application/json")]
public class FilesController(IFileStorageService storageService) : ControllerBase
{
    /// <summary>
    /// Uploads a file to the server (streamed) and persists metadata in the database.
    /// Requires 'ACTION_UPLOAD_FILE' action.
    /// </summary>
    /// <param name="request">File to upload.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Stored file metadata.</returns> 
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [RequireAction("ACTION_UPLOAD_FILE")]
    [ProducesResponseType(typeof(StoredFile), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<StoredFile>> Upload([FromForm] FileUploadRequest request, CancellationToken ct)
    {
        if (request.File.Length == 0)
            return BadRequest("Empty or missing file.");

        await using var stream = request.File.OpenReadStream();

        var stored = await storageService.UploadAsync(
            stream,
            request.File.FileName,
            request.File.ContentType,
            request.Prefix,
            ct
        );

        return CreatedAtAction(nameof(PresignDownloadById), new { id = stored.Id }, stored);
    }


    /// <summary>
    /// Generates a presigned PUT URL for client-side direct upload.
    /// Requires 'ACTION_UPLOAD_FILE' action.
    /// </summary>
    /// <param name="request">Upload presign request (filename, content type, etc).</param>
    /// <returns>Presigned URL string.</returns>
    [HttpPost("presign/upload")]
    [RequireAction("ACTION_UPLOAD_FILE")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> PresignUpload([FromBody] UploadPresignRequest request)
    {
        var resp = await storageService.GenerateUploadPresignedUrl(request);
        return Ok(resp);
    }

    /// <summary>
    /// Notifies the API that a file was uploaded directly using a presigned URL, and persists its metadata.
    /// Requires 'ACTION_UPLOAD_FILE' action.
    /// </summary>
    /// <param name="request">Notification data with file key, name, content type, etc.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Stored file metadata.</returns>
    [HttpPost("notify")]
    [RequireAction("ACTION_UPLOAD_FILE")]
    [ProducesResponseType(typeof(StoredFile), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<StoredFile>> Notify([FromBody] NotifyUploadRequest request, CancellationToken ct)
    {
        var stored = await storageService.NotifyUploadAsync(request, ct);

        // Return CreatedAtAction so client can easily follow to a download presign
        return CreatedAtAction(nameof(PresignDownloadById), new { id = stored.Id }, stored);
    }

    /// <summary>
    /// Generates a presigned GET URL to download a file using its ID.
    /// Requires 'ACTION_VIEW_FILE' action.
    /// </summary>
    /// <param name="id">StoredFile ID.</param>
    /// <param name="expiresMinutes">Optional expiration time in minutes (default is applied if omitted).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Presigned URL string.</returns>
    [HttpGet("presign/download/{id:int}")]
    [RequireAction("ACTION_VIEW_FILE")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> PresignDownloadById([FromRoute] int id, [FromQuery] int? expiresMinutes, CancellationToken ct)
    {
        if (id <= 0)
            return BadRequest("Invalid file ID.");

        var resp = await storageService.GenerateDownloadPresignedUrlByIdAsync(id, expiresMinutes, ct);

        return Ok(resp);
    }

    /// <summary>
    /// Deletes a file from S3 and removes its record from the database.
    /// Requires 'ACTION_DELETE_FILE' action.
    /// </summary>
    /// <param name="id">StoredFile ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content if deleted, not found otherwise.</returns>
    [HttpDelete("{id:int}")]
    [RequireAction("ACTION_DELETE_FILE")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
    {
        if (id <= 0)
            return BadRequest("Invalid file ID.");

        var deleted = await storageService.DeleteAsync(id, ct);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
