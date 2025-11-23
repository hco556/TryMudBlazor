using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using TryMudBlazor.Server.Dto;
using static TryMudBlazor.Server.Utilities.SnippetsEncoder;

namespace TryMudBlazor.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FilesController : ControllerBase
{
    const string defaultPath = @"C:\Users\hcopp\Documents\Work\OData\Northwind.OData.Templates\MudBlazor.Northwind\";
    public FilesController(IConfiguration config)
    {

    }

    [HttpGet("{fileName}/{path}")]
    public async Task<IActionResult> Get(string fileName, string path = "")
    {
        if(path == "")
            path = defaultPath + @"ViewModels\";
        return Ok(System.IO.File.ReadAllTextAsync(path));

    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] FileDto dto)
    {
        if (dto == null || string.IsNullOrEmpty(dto.Contents))
            return BadRequest("Invalid payload.");

        // Combine the provided path with the hosting environment root
       // var targetPath = Path.Combine(dto.Path,dto.FileName);

        // Ensure the directory exists
       // Directory.CreateDirectory(targetPath);

        var filePath = Path.Combine(dto.Path, dto.FileName);

        // Write the file contents to disk
        await System.IO.File.WriteAllTextAsync(filePath, dto.Contents);

        return Ok(new { message = "File saved successfully", filePath });
    }

}