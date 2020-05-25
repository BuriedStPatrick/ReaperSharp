using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ReaperAPI.Services
{
    public interface IFileService
    {
        Task<List<Guid>> SaveFiles(params IFormFile[] files);
        Task<List<FileInfo>> GetFiles(params Guid[] ids);
    }

    public interface IFileOptions
    {
        string ProcessingFolder { get; }
    }

    public class FileService : IFileService
    {
        private readonly IFileOptions _fileOptions;

        public FileService(IFileOptions fileOptions)
        {
            _fileOptions = fileOptions;
        }

        public async Task<List<Guid>> SaveFiles(params IFormFile[] files) =>
            (await Task.WhenAll(files.Select(SaveFile))).ToList();

        public async Task<List<FileInfo>> GetFiles(params Guid[] ids) =>
            (await Task.WhenAll(ids.Select(GetFile))).ToList();

        private async Task<FileInfo> GetFile(Guid id)
        {
            var target = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _fileOptions.ProcessingFolder);
            var filePath = Path.Combine(target, id.ToString());

            var data = await File.ReadAllBytesAsync(filePath);

            return new FileInfo(id, id + ".RPP", data, "text");
        }

        private async Task<Guid> SaveFile(IFormFile file)
        {
            var target = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _fileOptions.ProcessingFolder);

            if (!Directory.Exists(target))
            {
                Directory.CreateDirectory(target);
            }

            var trackingGuid = Guid.NewGuid();
            var filePath = Path.Combine(target, trackingGuid.ToString());
            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return trackingGuid;
        }
    }

    public class FileInfo
    {
        public Guid Id { get; }
        public string Filename { get; set; }
        public byte[] Data { get; set; }
        public string FileType { get; set; }

        public FileInfo(Guid id, string filename, byte[] data, string fileType)
        {
            Id = id;
            FileType = fileType;
            Data = data;
            Filename = filename;
        }
    }
}
