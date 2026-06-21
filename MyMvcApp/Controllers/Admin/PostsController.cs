using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Data;
using MyMvcApp.Models;

namespace MyMvcApp.Controllers.Admin
{
     [Authorize]
    [Route("Admin/Posts")]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /Admin/Posts
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts
                .OrderByDescending(p => p.PublishedDate)
                .ToListAsync();

            return View("~/Views/Admin/Posts/Index.cshtml", posts);
        }

        // GET /Admin/Posts/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/Posts/Create.cshtml");
        }

        // POST /Admin/Posts/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Admin/Posts/Create.cshtml", post);

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            TempData["AdminSuccess"] = "Post created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET /Admin/Posts/Edit/5
        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            return View("~/Views/Admin/Posts/Edit.cshtml", post);
        }

        // POST /Admin/Posts/Edit/5
        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Post post)
        {
            if (id != post.Id) return BadRequest();

            if (!ModelState.IsValid)
                return View("~/Views/Admin/Posts/Edit.cshtml", post);

            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            TempData["AdminSuccess"] = "Post updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST /Admin/Posts/Delete/5
        [HttpPost("Delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            TempData["AdminSuccess"] = "Post deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}