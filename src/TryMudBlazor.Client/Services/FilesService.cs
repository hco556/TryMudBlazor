namespace TryMudBlazor.Client.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.Extensions.Options;
    using SharpCompress.Archives;
    using Try.Core;
    using TryMudBlazor.Client.Models;

    public class FilesService
    {
        private readonly HttpClient httpClient;
        private readonly string filesService;

        public FilesService(HttpClient httpClient, NavigationManager navigationManager)
        {
            this.httpClient = httpClient;
            this.filesService = $"{navigationManager.BaseUri}FilesService";
        }

        public async Task<string> SaveFileAsync(string fileName, string path, string contents)//IEnumerable<CodeFile> codeFiles)
        {
            var dto = new
            {
                FileName = fileName,
                Path = path,
                Contents = contents
            };

            // Serialize to JSON
            var json = System.Text.Json.JsonSerializer.Serialize(dto);

            // Wrap in StringContent with application/json header
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Post to your API
            var response = await this.httpClient.PostAsync("api/files", content);

   
            return path + fileName;
        }

    }
}
