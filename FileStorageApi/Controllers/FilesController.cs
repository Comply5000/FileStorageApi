using FileStorageApi.DAL.Context;
using FileStorageApi.DAL.Entities;
using FileStorageApi.Models;
using FileStorageApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FileStorageApi.Controllers;

[ApiController]
[Route("files")]
public class FilesController : ControllerBase
{
    private readonly EFContext _context;
    private readonly IFileStorageService _fileStorageService;

    public FilesController(EFContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> UploadFile(IFormFile file, CancellationToken cancellationToken)
    {
        var fileResponse = await _fileStorageService.UploadAsync(file, cancellationToken);

        var systemFile = new SystemFile
        {
            Name = file.FileName,
            ContentType = fileResponse.ComntentType,
            Data = fileResponse.Data,
            TotalBytes = fileResponse.Data.Length
        };

        var result = await _context.Files.AddAsync(systemFile, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(result.Entity.Id);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteFile([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var file = await _context.Files.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (file is null)
            return NotFound();

        _context.Remove(file);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok();
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> DownloadFile([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var file = await _context.Files
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (file is null)
            return NotFound();
        
        return File(file.Data, file.ContentType, file.Name);
    }
    
    [HttpGet]
    public async Task<ActionResult<List<FileDto>>> GetFiles([FromQuery] string? search, CancellationToken cancellationToken)
    {
        var query = _context.Files.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => EF.Functions.ILike(x.Name, $"%{search}%"));

        var files = await query
            .Select(x => new FileDto(x.Id, x.Name))
            .ToListAsync(cancellationToken);

        return files;
    }
    
}