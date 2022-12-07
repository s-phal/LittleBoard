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
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;


// TODO Refactor chores
// TODO Design Logo

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
                .Include(p => p.ProjectMembers)
                .OrderByDescending(p => p.UpdatedDate);	

			return View(await applicationDbContext.ToListAsync());
        }

        // GET: Projects/Details/5
        [Authorize]
		[ActionName("Details")]
		public async Task<IActionResult> ProjectDetails(int? id)
        {
	
			// check if signed in user is part of ProjectMembers table
			// redirect if user is not found
			var projectMember = await _context.ProjectMember
				.Where(p => p.ProjectId == id && p.MemberId == _userManager.GetUserId(User))
				.ToListAsync();

			if (projectMember.Count() == 0)
			{
				return RedirectToAction("index", "projects");
			}


			if (id == null || _context.Project == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .Include(p => p.Member)
                .Include(p => p.Chores)
                .Include(p => p.Activities)
                .FirstOrDefaultAsync(m => m.Id == id);


            if (project == null)
            {
                return RedirectToAction("index","projects");
            }


			ViewData["MembersList"] = new SelectList(_context.Set<Member>(), "Id", "FirstName");

            return View(project);
        }

        // POST: Projects/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProject([Bind("Id,Title,Description, MemberId, ProjectId")] Project project)
        {
            if (ModelState.IsValid)
            {
                // create a new project
                // create a new activity
                // add project creator to ProjectMembers list 

				_context.Add(project); 
				await _context.SaveChangesAsync(); // project must exist in database or ActivityModel will fail

				var newActivity = new ActivityModel() 
                {
                    ProjectId = project.Id,
                    MemberId = project.MemberId,
                    Description = "created_project",
                    Subject = project.Title
                };
         
                var newProjectMember = new ProjectMember()
                {
                    ProjectId = project.Id,
                    MemberId = project.MemberId,
                };

                _context.Add(newActivity);
                _context.Add(newProjectMember);
				await _context.SaveChangesAsync();

				return RedirectToAction("details","projects",new {id = project.Id});
            }

            return RedirectToAction("index", "projects");
        }

		// GET: Projects/Edit/5
		[Authorize]
		public async Task<IActionResult> Edit(int? id)
        {
            // check if signed in user is part of ProjectMembers table
            // redirect if user is not found
			var projectMember = await _context.ProjectMember
	            .Where(p => p.ProjectId == id && p.MemberId == _userManager.GetUserId(User))
	            .ToListAsync();

			if (projectMember.Count() == 0)
			{
				return RedirectToAction("index", "projects");
			}

			if (id == null || _context.Project == null)
            {
				return RedirectToAction("index", "project");
			}

			var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
				return RedirectToAction("index", "project");
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
		public async Task<IActionResult> EditProjectDetails(int id, [Bind("Id,Title,Description,MemberId")] Project project)
		{
			// check if signed in user is part of ProjectMembers table
			// redirect if user is not found
			var projectMember = await _context.ProjectMember
				.Where(p => p.ProjectId == project.Id && p.MemberId == _userManager.GetUserId(User))
				.ToListAsync();

			if (projectMember.Count() == 0)
			{
				TempData["DisplayMessage"] = "You are not authorize to perform this task.";
				return RedirectToAction("index", "project", new { id = project.Id });

			}

			if (id != project.Id)
			{
				return RedirectToAction("index", "project");
			}

			if (ModelState.IsValid)
			{
				try
				{
					// since we can have multiple users working on the same project,
					// we need to ensure that on post update, the project owners data
					// won't be overridden with the ProjectMembers data.

					// find the project and get its current values.
					var getCurrentValueFromDB = await _context.Project
						.AsNoTracking()
						.FirstOrDefaultAsync(p => p.Id == id);
					
					// keep the project owner and the created date values
					// the same when updating the table.
					// without this statement, the ProjectMembers Id
					// will be used and the CreatedDate will be set to NOW
					project.MemberId = getCurrentValueFromDB.MemberId;
					project.CreatedDate = getCurrentValueFromDB.CreatedDate;

					// create an activity when the Project Title will be changed.
					if (project.Title != getCurrentValueFromDB.Title)
					{
						var newActivity = new ActivityModel()
						{
							ProjectId = project.Id,
							MemberId = _userManager.GetUserId(User),
							Description = "updated_title",
							CreatedDate = getCurrentValueFromDB.CreatedDate,
							Subject = $"{getCurrentValueFromDB.Title} <span class='text-black'>to</span> {project.Title}"
						};
					_context.Add(newActivity);

					}

					// create an activity when the Project Description will be changed.
					if (project.Description != getCurrentValueFromDB.Description)
					{
						var newActivity = new ActivityModel()
						{
							ProjectId = project.Id,
							MemberId = _userManager.GetUserId(User),
							Description = "updated_description",
							CreatedDate = getCurrentValueFromDB.CreatedDate,
							Subject = project.Title
						};

					_context.Add(newActivity);
					}

					// record the changes then save to the database.
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
				return RedirectToAction("details", "projects", new { id = id });
			}
			ViewData["MemberId"] = new SelectList(_context.Users, "Id", "Id", project.MemberId);
			return View(project);
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateNotes([Bind("Id,Title,Description,MemberId, Notes")] Project project)
        {

			var projectMember = await _context.ProjectMember
				.Where(ProjectMember => ProjectMember.ProjectId == project.Id && ProjectMember.MemberId == _userManager.GetUserId(User))
				.ToListAsync();

			if (projectMember.Count == 0)
			{
				TempData["DisplayMessage"] = "You are not authorize to perform this task.";
				return RedirectToAction("details", "projects", new { id = project.Id });
			}

			var getCurrentValueFromDB = await _context.Project 
	            .AsNoTracking()
	            .FirstOrDefaultAsync(p => p.Id == project.Id);

			if (project.Notes != getCurrentValueFromDB.Notes)
			{
                project.MemberId = getCurrentValueFromDB.MemberId;
                var activity = new ActivityModel();
                activity.MemberId = _userManager.GetUserId(User);
				activity.Description = "updated_notes";
				activity.CreatedDate = getCurrentValueFromDB.CreatedDate;
				activity.Subject = project.Title;
                activity.ProjectId = project.Id;

				_context.Update(project);
                _context.Update(activity);
				await _context.SaveChangesAsync();
			};

			return RedirectToAction("details", "projects", new { id = project.Id });
        }

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddMember([Bind("Id,Title,MemberId")] Project project)
        {
			var memberExist = await _context.ProjectMember
	            .Where(p => p.ProjectId == project.Id && p.MemberId == project.MemberId)
	            .ToListAsync();

            if(memberExist.Count() == 0)
            {
                var projectMember = new ProjectMember()
                {
                    MemberId = project.MemberId,
                    ProjectId = project.Id
                };
                
			    _context.Add(projectMember);

                var memberActivity = new ActivityModel()
                {

                    ProjectId = project.Id,
                    MemberId = project.MemberId,
                    Description = "added_member",
                    Subject = project.Title
                };

                _context.Activity.Add(memberActivity);


			    await _context.SaveChangesAsync();
				return RedirectToAction("details", "projects", new { id = project.Id });

			}
			TempData["DisplayMessage"] = "Member is already part of the project members list.";
			return RedirectToAction("details", "projects", new { id = project.Id });
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RemoveMember([Bind("Id,Title,MemberId")] Project project)
		{
			// if action caller is project owner
			// redirect with error message
            if(project.MemberId == _userManager.GetUserId(User))
            {
                TempData["DisplayMessage"] = "Can not remove self from Project.";
				return RedirectToAction("details", "projects", new { id = project.Id });

			}

			// project.MemberId is passed in.
			// goto ProjectMember table
			// find that user which matches both MemberId to the Project.Id
			var projectMember = _context.ProjectMember
	            .Where(p => p.ProjectId == project.Id && p.MemberId == project.MemberId)
				.FirstOrDefaultAsync();


			// if exit, remove the extracted user
			if (projectMember != null)
			{
				_context.ProjectMember.Remove(await projectMember);
				await _context.SaveChangesAsync();
				return RedirectToAction("details", "projects", new { id = project.Id });

			}



			return RedirectToAction("details","projects", new { id = project.Id});
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

        private bool ProjectExists(int id)
        {
          return (_context.Project?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
