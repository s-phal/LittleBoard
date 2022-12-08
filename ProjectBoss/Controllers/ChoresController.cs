using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectBoss.Data;
using ProjectBoss.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;



namespace ProjectBoss.Controllers
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
	

		// POST: Chores/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[Authorize]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTask([Bind("Id,Body,ProjectId")] Chore chore)
        {
			// check if signed in user is part of ProjectMembers table
			// redirect if user is not found
			var projectMember = await _context.ProjectMember
                .Where(ProjectMember => ProjectMember.ProjectId == chore.ProjectId && ProjectMember.MemberId == _userManager.GetUserId(User))
                .ToListAsync();

            if (projectMember.Count == 0)
            {
                TempData["DisplayMessage"] = "You are not authorize to perform this task.";
				return RedirectToAction("details", "projects", new { id = chore.ProjectId });
            }

            // create a new activity when a chore is created            
			var project = await _context.Project.FindAsync(chore.ProjectId);

			if (ModelState.IsValid)
            {
                var newActivity = new ActivityModel()
                {
					MemberId = _userManager.GetUserId(User),
					ProjectId = project.Id,
					Description = "created_task",
					Subject = chore.Body
			    };

                _context.Add(newActivity);
                _context.Add(chore);
                await _context.SaveChangesAsync();
                return RedirectToAction("details", "projects", new { id = chore.ProjectId});
            }

			return RedirectToAction("details", "projects", new { id = chore.ProjectId });

		}


		// POST: Chores/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[Authorize]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTask(int id, [Bind("Id,Body,ProjectId, Completed")] Chore chore)
        {

			// check if signed in user is part of ProjectMembers table
			// redirect if user is not found
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
				return RedirectToAction("details", "projects", new { id = chore.ProjectId });

			}

			if (ModelState.IsValid)
            {           

			var project = await _context.Project.FindAsync(chore.ProjectId);
                try
                {
					// since we can have multiple users working on the same project,
					// we need to ensure that on post update, the project owners data
					// won't be overridden with the ProjectMembers data.

					// find the project and get its current values.
					var getCreatedDateValuesFromDB = await _context.Chore 
	                    .AsNoTracking()
	                    .FirstOrDefaultAsync(chore => chore.Id == id);

					// keep the created date values the same when updating the row
					// without this statement, the CreatedDate will be set to NOW
					chore.CreatedDate = getCreatedDateValuesFromDB.CreatedDate;

					// create a new activity for each corresponding action
                    if(chore.Completed == true)
                    {
						var newActivity = new ActivityModel()
						{
                            MemberId = _userManager.GetUserId(User),
                            ProjectId = project.Id, 
                            Description = "completed_task", 
                            Subject = chore.Body 
						};
						_context.Update(newActivity);
                    }

					if (chore.Completed == false)
					{
						var newActivity = new ActivityModel()
						{
							MemberId = _userManager.GetUserId(User),
							ProjectId = project.Id,
							Description = "updated_task",
							Subject = chore.Body
						};
						_context.Update(newActivity);
					}

					if (chore.Completed == false && chore.Body == getCreatedDateValuesFromDB.Body)
					{
						var newActivity = new ActivityModel()
						{
							MemberId = _userManager.GetUserId(User),
							ProjectId = project.Id,
							Description = "renewed_task",
							Subject = chore.Body
						};
						_context.Update(newActivity);
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

			return RedirectToAction("details", "projects", new { Id = chore.ProjectId });

		}


		// POST: Chores/Delete/5
		[Authorize]
		[HttpPost, ActionName("DeleteTask")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTask(int id)
        {
			var choreContext = await _context.Chore.FindAsync(id);

			var projectMember = await _context.ProjectMember
				.Where(ProjectMember => ProjectMember.ProjectId == choreContext.ProjectId && ProjectMember.MemberId == _userManager.GetUserId(User))
				.ToListAsync();

			if (projectMember.Count == 0)
			{
				TempData["DisplayMessage"] = "You are not authorize to perform this task.";
				return RedirectToAction("details", "projects", new { id = choreContext.ProjectId });
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
            return RedirectToAction("details","projects", new { id = choreContext.ProjectId });
        }

        private bool ChoreExists(int id)
        {
          return (_context.Chore?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
