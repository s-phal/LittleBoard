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


// TODO Project Description for GITHUB - TEAMBASED Project Management App
// TODO implement slugs
// TODO Sort by Chores->UpdatedDate for each project list
// TODO Hide Edit View
// TODO Redirect default route of NotFound id to homepage
// TODO Restrict non project owners from adding members to unauthorized projects

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
        public async Task<IActionResult> Details(int? id)
        {
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
                return NotFound();
            }

                                    //if (project.MemberId != _userManager.GetUserId(User))
                                    //{
                                    //    return RedirectToAction("unauthorize", "project");
                                    //}
			ViewData["MembersList"] = new SelectList(_context.Set<Member>(), "Id", "FirstName");

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
        public async Task<IActionResult> Create([Bind("Id,Title,Description, MemberId, ProjectId")] Project project)
        {
            if (ModelState.IsValid)
            {

				project.Activities.Add(new ActivityModel() // On Create, add activty to Activity table
                {  
                    MemberId = project.MemberId,
                    Description = "created_project", 
                    Subject = project.Title                   
                    
                }); 

                project.ProjectMembers.Add(new ProjectMember() // On Create, add user to ProjectMember table
                {
                    ProjectId = project.Id,
                    MemberId = project.MemberId
                }); 

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
			var projectMember = await _context.ProjectMember
	            .Where(p => p.ProjectId == id && p.MemberId == _userManager.GetUserId(User))
	            .ToListAsync();

			if (projectMember.Count() == 0)
			{
				return RedirectToAction("unauthorize", "project");
			}



			if (id == null || _context.Project == null)
            {
                return NotFound();
            }

            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
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

            if (ModelState.IsValid)
            {
                try
                {


					var getCurrentValueFromDB = await _context.Project // get current state of value from database
	                    .AsNoTracking()
	                    .FirstOrDefaultAsync(p => p.Id == id);

					project.CreatedDate = getCurrentValueFromDB.CreatedDate;

					if (project.Title != getCurrentValueFromDB.Title)
					{
						project.Activities.Add(new ActivityModel()
						{
                            MemberId = project.MemberId,
							Description = "updated_title",
							CreatedDate = getCurrentValueFromDB.CreatedDate,
							Subject = $"{getCurrentValueFromDB.Title} <span class='text-black'>to</span> {project.Title}"                            
						}); 
					}

                    // START Redirect if user is not project owner
					var projectMember = await _context.ProjectMember
                        .Where(p => p.ProjectId == project.Id && p.MemberId == _userManager.GetUserId(User))
                        .ToListAsync();

					if (projectMember.Count() == 0)
                    {
						return RedirectToAction("unauthorize", "project");

					}

                    // END ------------------------------------

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

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateNotes(Project project)
        {
			var getCurrentValueFromDB = await _context.Project 
	            .AsNoTracking()
	            .FirstOrDefaultAsync(p => p.Id == project.Id);

			if (project.Notes != getCurrentValueFromDB.Notes)
			{
                var activity = new ActivityModel();
                activity.MemberId = project.MemberId;
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
		public async Task<IActionResult> AddMember( Project project)
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
			TempData["DisplayMessage"] = $"Member is already part of the project members list.";
			return RedirectToAction("details", "projects", new { id = project.Id });
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RemoveMember(Project project)
		{
            if(project.MemberId == _userManager.GetUserId(User))
            {
                TempData["DisplayMessage"] = "Can not remove self from Project.";
				return RedirectToAction("details", "projects", new { id = project.Id });

			}


			var projectMember = _context.ProjectMember
	            .Where(p => p.ProjectId == project.Id && p.MemberId == project.MemberId)
				.FirstOrDefaultAsync();


			if (projectMember != null)
			{
				_context.ProjectMember.Remove(await projectMember);
				await _context.SaveChangesAsync();
				return RedirectToAction("details", "projects", new { id = project.Id });

			}



			return RedirectToAction("details","projects", new { id = project.Id});
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
