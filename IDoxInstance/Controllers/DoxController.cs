using IDoxInstance.Context;
using IDoxInstance.Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace IDoxInstance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoxController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppDbContext _context;
        public DoxController(IWebHostEnvironment webHostEnvironment, AppDbContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }
        [HttpPost("Post")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Post([FromForm] FileDTO dto)
        {
            var claim = User.Identity as ClaimsIdentity;
            var wwwroot = _webHostEnvironment.WebRootPath;
            foreach (IFormFile doc in dto.formFiles)
            {
                if (doc.Length > 0)
                {
                    string filePath = Path.Combine(wwwroot, doc.FileName);
                    using (Stream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        doc.CopyTo(fileStream);
                    }
                }
            }
            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == claim.Name/* dto.UserEmail*/);
            
            var file = new Entities.Document
            {
                CreatedUserId = currentUser.Id,
                DocumentName = dto.DocumentName,    
                DocumentType = dto.DocumentType,
                CreatedDate = dto.CreatedDate,
                ReceiverUserId = dto.ReceiverUserId,
                Path = wwwroot + "\\" + dto.formFiles[0].FileName,
            };
            await _context.Documents.AddAsync(file);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPut("UpdateFile")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> UpdateFile([FromForm] UpdatefileDTO dto)
        {
            var path = _webHostEnvironment.WebRootPath;
            var claim = User.Identity as ClaimsIdentity;
            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == claim.Name);
            var fileQuery = await _context.Documents.FirstOrDefaultAsync(x => x.Id == dto.fileId);
            if (fileQuery.CreatedUserId != currentUser.Id)
                return BadRequest();
            if(dto.file != null)
            {
                path = path + "\\" + dto.file[0].FileName;
                System.IO.File.Delete(fileQuery.Path);
                fileQuery.Path = path;
                foreach (IFormFile doc in dto.file)
                {
                    if (doc.Length > 0)
                    {
                        //string filePath = Path.Combine(path);
                        using (Stream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                        {
                            doc.CopyTo(fileStream);
                        }
                    }
                }
            }
            if (dto.ReceiverUserId != null)
                fileQuery.ReceiverUserId = (int)dto.ReceiverUserId;
            if(dto.DocumentName != null)
                fileQuery.DocumentName = dto.DocumentName;
            if (dto.DocumentType != null)
                fileQuery.DocumentType = dto.DocumentType;
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete("Delete")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Delete()
        {
            var claim = User.Identity as ClaimsIdentity;
            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == claim.Name);
            var fileQuery = await _context.Documents.FirstOrDefaultAsync(x => x.CreatedUserId == currentUser.Id);
            if (fileQuery.CreatedUserId != currentUser.Id)
                return BadRequest();
            System.IO.File.Delete(fileQuery.Path);
            _context.Documents.Remove(fileQuery);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("List")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> List()
        {
            var claim = User.Identity as ClaimsIdentity;
            var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == claim.Name);
            var files = await _context.Documents.Where(x => x.CreatedUserId == currentUser.Id || x.ReceiverUserId == currentUser.Id).ToListAsync();
            return Ok(files);
        }
    }
}
