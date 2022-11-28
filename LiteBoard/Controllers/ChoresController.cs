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
            var applicationDbContext = _context.Chores
                .Include(c => c.Project)
                .Where(c => c.Project.MemberId == _userManager.GetUserId(User));
                
            return View(await applicationDbContext.ToListAsync());
        }

		// GET: Chores/Details/5
		[Authorize]
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Chores == null)
            {
                return NotFound();
            }

            var chore = await _context.Chores
                .Include(c => c.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chore == null)
            {
                return NotFound();
            }

            if (chore.Project.MemberId != _userManager.GetUserId(User))
            {
                RedirectToAction("unauthorize", "project");
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
            if (ModelState.IsValid)
            {
                _context.Add(chore);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Id", chore.ProjectId);
            return View(chore);
        }

		// GET: Chores/Edit/5
		[Authorize]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Chores == null)
            {
                return NotFound();
            }
			var chore = await _context.Chores.FindAsync(id);
			if (chore.Project.MemberId != _userManager.GetUserId(User))
			{
				RedirectToAction("unauthorize", "project");
			}
            if (chore == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Id", chore.ProjectId);
            return View(chore);
        }

		// POST: Chores/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[Authorize]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Body,ProjectId")] Chore chore)
        {
            if (id != chore.Id)
            {
                return NotFound();
            }
			if (chore.Project.MemberId != _userManager.GetUserId(User))
			{
				RedirectToAction("unauthorize", "project");
			}

			if (ModelState.IsValid)
            {
                try
                {
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Id", chore.ProjectId);
            return View(chore);
        }

		// GET: Chores/Delete/5
		[Authorize]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Chores == null)
            {
                return NotFound();
            }

            var chore = await _context.Chores
                .Include(c => c.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chore == null)
            {
                return NotFound();
            }
			if (chore.Project.MemberId != _userManager.GetUserId(User))
			{
				RedirectToAction("unauthorize", "project");
			}

			return View(chore);
        }

		// POST: Chores/Delete/5
		[Authorize]
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Chores == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Chores'  is null.");
            }
            var chore = await _context.Chores.FindAsync(id);
            if (chore != null)
            {
                _context.Chores.Remove(chore);
            }
			if (chore.Project.MemberId != _userManager.GetUserId(User))
			{
				RedirectToAction("unauthorize", "project");
			}

			await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChoreExists(int id)
        {
          return (_context.Chores?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
