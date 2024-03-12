using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogpostsController : ControllerBase
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly ICategoryRepository _categoryRepository;

        public BlogpostsController(IBlogPostRepository blogPostRepository, ICategoryRepository categoryRepository)
        {
            _blogPostRepository = blogPostRepository;
            _categoryRepository = categoryRepository;
        }

        //POST:{apibaseurl}/api/blogposts
        [HttpPost]
        public async Task<IActionResult> CreateBlogPosts([FromBody] CreateBlogpostRequestDto request)
        {
            var blogPost = new BlogPost
            {
                Title = request.Title,
                Content = request.Content,
                Author = request.Author,
                ShortDescription = request.ShortDescription,
                UrlHandle = request.UrlHandle,
                FeaturedImageUrl = request.FeaturedImageUrl,
                PublishedDate = request.PublishedDate,
                isVisible = request.isVisible,
                Categories = new List<Category>()
            };
            foreach (var categoryGuid in request.Categories)
            {
                var existingCategory = await _categoryRepository.GetByIdAsync(categoryGuid);
                if (existingCategory is not null)
                {
                    blogPost.Categories.Add(existingCategory);
                }
            }
            await _blogPostRepository.CreateAsync(blogPost);

            //Convert Domain Model to DTO

            var response = new BlogpostDto
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                Content = blogPost.Content,
                Author = blogPost.Author,
                ShortDescription = blogPost.ShortDescription,
                UrlHandle = blogPost.UrlHandle,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                PublishedDate = blogPost.PublishedDate,
                isVisible = blogPost.isVisible,
                Categories = blogPost.Categories.Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle
                }).ToList()
            };
            return Ok(response);
        }

        //GET:{apibaseurl}/api/blogposts
        [HttpGet]
        public async Task<IActionResult> GetAllBlogPosts()
        {
            var blogPosts = await _blogPostRepository.GetAllAsync();

            //Convert Domain Model to Dto
            var response = new List<BlogpostDto>();
            foreach (var blogpost in blogPosts)
            {
                response.Add(new BlogpostDto
                {
                    Id = blogpost.Id,
                    Title = blogpost.Title,
                    Content = blogpost.Content,
                    Author = blogpost.Author,
                    ShortDescription = blogpost.ShortDescription,
                    UrlHandle = blogpost.UrlHandle,
                    FeaturedImageUrl = blogpost.FeaturedImageUrl,
                    PublishedDate = blogpost.PublishedDate,
                    isVisible = blogpost.isVisible,
                    Categories = blogpost.Categories.Select(x => new CategoryDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        UrlHandle = x.UrlHandle
                    }).ToList()
                });
            }
            return Ok(response);
        }

        //GET:{apibaseurl}/api/blogposts/{id}
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetBlogPostById([FromRoute] Guid id)
        {
            var blogPost = await _blogPostRepository.GetByIdAsync(id);
            if (blogPost is null)
            {
                return NotFound();
            }

            //Convert Domain Model to Dto

            var response = new BlogpostDto
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                Content = blogPost.Content,
                Author = blogPost.Author,
                ShortDescription = blogPost.ShortDescription,
                UrlHandle = blogPost.UrlHandle,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                PublishedDate = blogPost.PublishedDate,
                isVisible = blogPost.isVisible,
                Categories = blogPost.Categories.Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle
                }).ToList()
            };
            return Ok(response);
        }

        //PUT: {apibaseurl}/api/blogposts/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateBlogPostById([FromRoute] Guid id, [FromBody] UpdateBlogPostRequestDto request)
        {
            //Convert Dto to Domain Model

            var blogpost = new BlogPost
            {
                Id = id,
                Title = request.Title,
                Content = request.Content,
                Author = request.Author,
                ShortDescription = request.ShortDescription,
                UrlHandle = request.UrlHandle,
                FeaturedImageUrl = request.FeaturedImageUrl,
                PublishedDate = request.PublishedDate,
                isVisible = request.isVisible,
                Categories = new List<Category>()
            };
            foreach (var categoryGuid in request.Categories)
            {
                var existingCategory = await _categoryRepository.GetByIdAsync(categoryGuid);
                if(existingCategory != null)
                {
                    blogpost.Categories.Add(existingCategory);
                }
            }

            var updatedBlogpost=await _blogPostRepository.UpdateAsync(blogpost);
            if(updatedBlogpost==null)
            {
                return NotFound();
            }

            var response = new BlogpostDto
            {
                Id = blogpost.Id,
                Title = blogpost.Title,
                Content = blogpost.Content,
                Author = blogpost.Author,
                ShortDescription = blogpost.ShortDescription,
                UrlHandle = blogpost.UrlHandle,
                FeaturedImageUrl = blogpost.FeaturedImageUrl,
                PublishedDate = blogpost.PublishedDate,
                isVisible = blogpost.isVisible,
                Categories = blogpost.Categories.Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle
                }).ToList()
            };
            return Ok(response);
        }

        //DELETE: {apibaseurl}/api/blogposts/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteBlogPost(Guid id)
        {
            var deletedBlogpost= await _blogPostRepository.DeleteAsync(id);
            if (deletedBlogpost == null)
            {
                return NotFound();
            }
            var response = new BlogpostDto
            {
                Id = deletedBlogpost.Id,
                Title = deletedBlogpost.Title,
                Content = deletedBlogpost.Content,
                Author = deletedBlogpost.Author,
                ShortDescription = deletedBlogpost.ShortDescription,
                UrlHandle = deletedBlogpost.UrlHandle,
                FeaturedImageUrl = deletedBlogpost.FeaturedImageUrl,
                PublishedDate = deletedBlogpost.PublishedDate,
                isVisible = deletedBlogpost.isVisible,
            };
            return Ok(response);
        }

        //GET: {apibaseurl}/api/blogposts/{urlHandle}
        [HttpGet]
        [Route("{urlHandle}")]
        public async Task<IActionResult> GetBlogPostByUrl([FromRoute] string urlHandle)
        {
          var blogPost=  await _blogPostRepository.GetByUrlHandleAsync(urlHandle);

            if (blogPost is null)
            {
                return NotFound();
            }

            //Convert Domain Model to Dto

            var response = new BlogpostDto
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                Content = blogPost.Content,
                Author = blogPost.Author,
                ShortDescription = blogPost.ShortDescription,
                UrlHandle = blogPost.UrlHandle,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                PublishedDate = blogPost.PublishedDate,
                isVisible = blogPost.isVisible,
                Categories = blogPost.Categories.Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle
                }).ToList()
            };
            return Ok(response);
        }

    }
}
