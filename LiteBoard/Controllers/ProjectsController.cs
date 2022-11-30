using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LiteBoard.Data;
using LiteBoard.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Collections.Immutable;

// TODO implement slugs
// TODO Sort by Chores->UpdatedDate for each project list
// TODO Hide Edit View

namespace LiteBoard.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Member> _userManager;

        public ProjectsController(ApplicationDbContext context, UserManager<Member> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Projects
        [Authorize]
        public async Task<IActionResult> Index()
        {

            var applicationDbContext = _context.Project
                .Include(p => p.Member)
                .Where(p => p.MemberId == _userManager.GetUserId(User)) // show only projects that belongs to projects owner 
                .OrderByDescending(p => p.UpdatedDate);




            return View(await applicationDbContext.ToListAsync());

        }

        // GET: Projects/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {       

            if (id == null || _context.Project == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .Include(p => p.Member)
                .Include(p => p.Chores)                
                .FirstOrDefaultAsync(m => m.Id == id);

			if (project.MemberId != _userManager.GetUserId(User))
            {
                return RedirectToAction("unauthorize","project");
            }


            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["MemberId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description, MemberId")] Project project)
        {
            if (ModelState.IsValid)
            {                
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MemberId"] = new SelectList(_context.Users, "Id", "Id", project.MemberId);
            return View(project);
        }

		// GET: Projects/Edit/5
		[Authorize]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Project == null)
            {
                return NotFound();
            }

            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            if(project.MemberId != _userManager.GetUserId(User))
            {
                return RedirectToAction("unauthorize", "project");
            }

            ViewData["MemberId"] = new SelectList(_context.Users, "Id", "Id", project.MemberId);
            return View(project);
        }

		// POST: Projects/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[Authorize]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,MemberId, Notes")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }
            if (project.MemberId != _userManager.GetUserId(User))
            {
                return RedirectToAction("unauthorize", "project");
            }

            if (ModelState.IsValid)
            {
                try
                {
					var getCreatedDateValuesFromDB = await _context.Project // Keeps the same CreatedDate value when updating
	                .AsNoTracking()
	                .FirstOrDefaultAsync(p => p.Id == id);

					project.CreatedDate = getCreatedDateValuesFromDB.CreatedDate;

					_context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("details","projects", new { id = id});
            }
            ViewData["MemberId"] = new SelectList(_context.Users, "Id", "Id", project.MemberId);
            return View(project);
        }

		// GET: Projects/Delete/5
		[Authorize]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Project == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .Include(p => p.Member)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            if (project.MemberId != _userManager.GetUserId(User))
            {
                return RedirectToAction("unauthorize", "project");
            }

            return View(project);
        }

		// POST: Projects/Delete/5
		[Authorize]
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Project == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Project'  is null.");
            }
            var project = await _context.Project.FindAsync(id);

            if (project.MemberId != _userManager.GetUserId(User))
            {
                return RedirectToAction("unauthorize", "project");
            }

            if (project != null)
            {
                _context.Project.Remove(project);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Route("/project/unauthorize")]
        public IActionResult Unauthorize()
        {
            return View();
        }

        private bool ProjectExists(int id)
        {
          return (_context.Project?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
