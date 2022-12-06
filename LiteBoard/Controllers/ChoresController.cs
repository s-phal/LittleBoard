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

// TODO rename routes from CHORES to TASKS


namespace LiteBoard.Controllers
{
    public class ChoresController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Member> _userManager;

        public ChoresController(ApplicationDbContext context, UserManager<Member> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

		// GET: Chores
		[Authorize]
		public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Chore
                .Include(c => c.Project)
                .Where(c => c.Project.MemberId == _userManager.GetUserId(User)) // Show only Tasks that belongs to the Project Owner
                .OrderByDescending(Project => Project.UpdatedDate);

			return View(await applicationDbContext.ToListAsync());
        }

		// GET: Chores/Details/5
		[Authorize]
		public async Task<IActionResult> Details(int? id)
        {
			var choreContext = await _context.Chore.FindAsync(id);
            var project = await _context.Project.FindAsync(choreContext.ProjectId);
			if (project.MemberId != _userManager.GetUserId(User))
			{
				return RedirectToAction("unauthorize", "project");

			}
			if (id == null || _context.Chore == null)
            {
                return NotFound();
            }

            var chore = await _context.Chore
                .Include(c => c.Project)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (chore == null)
            {
                return NotFound();
            }



            return View(chore);
        }

		// GET: Chores/Create
		[Authorize]
		public IActionResult Create()
        {
            ViewData["ProjectId"] = new SelectList(_context.Project
				.Include(p => p.Member)
				.Where(p => p.MemberId == _userManager.GetUserId(User)), "Id", "Id");
            return View();
        }

		// POST: Chores/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[Authorize]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Body,ProjectId")] Chore chore)
        {
			var project = await _context.Project.FindAsync(chore.ProjectId);

            var projectMember = await _context.ProjectMember
                .Where(ProjectMember => ProjectMember.ProjectId == chore.ProjectId && ProjectMember.MemberId == _userManager.GetUserId(User))
                .ToListAsync();

            if (projectMember.Count == 0)
            {
                TempData["DisplayMessage"] = "You are not authorize to perform this task.";
				return RedirectToAction("details", "projects", new { id = chore.ProjectId });
            }



			if (ModelState.IsValid)
            {
                var Activities = _context.Activity;
                Activities.Add(new ActivityModel() { 
                    MemberId = _userManager.GetUserId(User),
                    ProjectId = project.Id, 
                    Description = "created_task", 
                    Subject = chore.Body }); 

                _context.Add(chore);
                await _context.SaveChangesAsync();
                return RedirectToAction("details", "projects", new { id = chore.ProjectId});
            }
            ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Id", chore.ProjectId);
            return View(chore);
        }

		// GET: Chores/Edit/5
		[Authorize]
		public async Task<IActionResult> Edit(int? id)
        {


			var choreContext = await _context.Chore.FindAsync(id);


			var projectMember = await _context.ProjectMember
				.Where(ProjectMember => ProjectMember.ProjectId == choreContext.ProjectId && ProjectMember.MemberId == _userManager.GetUserId(User))
				.ToListAsync();

			if (projectMember.Count == 0)
			{
				ViewData["DisplayMessage"] = "You are not authorize to perform this task.";
				return RedirectToAction("details", "projects", new { id = choreContext.ProjectId });
			}

			if (id == null || _context.Chore == null)
            {
                return NotFound();
            }

			var chore = await _context.Chore.FindAsync(id);
			if (chore == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Id", choreContext.ProjectId);
            return View(chore);
        }

		// POST: Chores/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[Authorize]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Body,ProjectId, Completed")] Chore chore)
        {
			var project = await _context.Project.FindAsync(chore.ProjectId);
			var projectMember = await _context.ProjectMember
				.Where(ProjectMember => ProjectMember.ProjectId == chore.ProjectId && ProjectMember.MemberId == _userManager.GetUserId(User))
				.ToListAsync();

			if (projectMember.Count == 0)
			{
				TempData["DisplayMessage"] = "You are not authorize to perform this task.";
				return RedirectToAction("details", "projects", new { id = chore.ProjectId });
			}

			if (id != chore.Id)
            {
                return NotFound();
            }

			if (ModelState.IsValid)
            {           

                try
                {
					var getCreatedDateValuesFromDB = await _context.Chore // Keeps the same CreatedDate value when updating
	                    .AsNoTracking()
	                    .FirstOrDefaultAsync(chore => chore.Id == id);

					chore.CreatedDate = getCreatedDateValuesFromDB.CreatedDate;

                    if(chore.Completed == true)
                    {
						var Activities = _context.Activity;
						Activities.Add(new ActivityModel() { 
                            MemberId = _userManager.GetUserId(User),
                            ProjectId = project.Id, 
                            Description = "completed_task", 
                            Subject = chore.Body }); // Insert new row of Activity
                    }

					if (chore.Completed == false)
					{
                        var Activities = _context.Activity;
						Activities.Add(new ActivityModel() {
							MemberId = _userManager.GetUserId(User),
							ProjectId = project.Id,
							Description = "updated_task",
							Subject = chore.Body
						}); // Insert new row of Activity
					}



					_context.Update(chore);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChoreExists(chore.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("details","projects", new { Id = chore.ProjectId });
            }
            ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Id", chore.ProjectId);
            return View(chore);
        }

		// GET: Chores/Delete/5
		[Authorize]
		public async Task<IActionResult> Delete(int? id)
        {
			var choreContext = await _context.Chore.FindAsync(id);
			var project = await _context.Project.FindAsync(choreContext.ProjectId);
			if (project.MemberId != _userManager.GetUserId(User))
			{
				return RedirectToAction("unauthorize", "project");

			}
			if (id == null || _context.Chore == null)
            {
                return NotFound();
            }

            var chore = await _context.Chore
                .Include(c => c.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chore == null)
            {
                return NotFound();
            }


			return View(chore);
        }

		// POST: Chores/Delete/5
		[Authorize]
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
			var choreContext = await _context.Chore.FindAsync(id);
			var project = await _context.Project.FindAsync(choreContext.ProjectId);
			if (project.MemberId != _userManager.GetUserId(User))
			{
				return RedirectToAction("unauthorize", "project");

			}
			if (_context.Chore == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Chores'  is null.");
            }
            var chore = await _context.Chore.FindAsync(id);
            if (chore != null)
            {
                _context.Chore.Remove(chore);
            }


			await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChoreExists(int id)
        {
          return (_context.Chore?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
