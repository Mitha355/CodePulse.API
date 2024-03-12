using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Implementation;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;
        public ImagesController(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        //POST: {apibaseurl}/api/images
        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file, [FromForm] string fileName, [FromForm] string title)
        {
            ValidateFileUpload(file);
            if (ModelState.IsValid)
            {
                var blogImage = new BlogImage
                {
                    FileExtension = Path.GetExtension(file.FileName).ToLower(),
                    FileName = fileName,
                    Title = title,
                    DateCreated = DateTime.Now
                };
                blogImage = await _imageRepository.Upload(file, blogImage);

                //Convert Domain Model to DTO

                var response = new BlogImageDto
                {
                    Id = blogImage.Id,
                    FileExtension = blogImage.FileExtension,
                    Title = blogImage.Title,
                    DateCreated = DateTime.Now,
                    FileName = blogImage.FileName,
                    Url = blogImage.Url
                };
                return Ok(response);
            }
            return BadRequest(ModelState);
        }

        private void ValidateFileUpload(IFormFile file)
        {
            var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };
            if (!allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
            {
                ModelState.AddModelError("file", "Unsupported File Format");
            }

            if (file.Length > 10485760)
            {
                ModelState.AddModelError("file", "File size cannot be more than 10 Mb");
            }
        }

        //GET: {apibaseurl}/api/images
        [HttpGet]
        public async Task<IActionResult> GetAllImages()
        {
            var images = await _imageRepository.GetAll();

            // Convert Domain Model to DTO
            var response = new List<BlogImageDto>();
            foreach (var image in images)
            {
                response.Add(new BlogImageDto
                {
                    Id = image.Id,
                    FileExtension = image.FileExtension,
                    Title = image.Title,
                    DateCreated = DateTime.Now,
                    FileName = image.FileName,
                    Url = image.Url
                });
            }
            return Ok(response);
        }
    }
}
