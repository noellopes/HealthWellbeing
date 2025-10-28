﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
	public class PlanController : Controller
	{
		private readonly HealthWellbeingDbContext _context;

		public PlanController(HealthWellbeingDbContext context)
		{
			_context = context;
		}

		// GET: Plan
		public async Task<IActionResult> Index()
		{
			return View(await _context.Plan.ToListAsync());
		}

		// GET: Plan/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var plan = await _context.Plan
				.FirstOrDefaultAsync(m => m.PlanId == id);

			if (plan == null)
			{
				return NotFound();
			}

			return View(plan);
		}

		// GET: Plan/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Plan/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Name,Description,Price,DurationDays")] Plan plan)
		{
			if (ModelState.IsValid)
			{
				_context.Add(plan);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(plan);
		}

		// GET: Plan/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var plan = await _context.Plan.FindAsync(id);
			if (plan == null)
			{
				return NotFound();
			}
			return View(plan);
		}

		// POST: Plan/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("PlanId,Name,Description,Price,DurationDays")] Plan plan)
		{
			if (id != plan.PlanId)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(plan);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!PlanExists(plan.PlanId))
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
			return View(plan);
		}

		// GET: Plan/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var plan = await _context.Plan
				.FirstOrDefaultAsync(m => m.PlanId == id);
			if (plan == null)
			{
				return NotFound();
			}

			return View(plan);
		}

		// POST: Plan/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var plan = await _context.Plan.FindAsync(id);
			if (plan != null)
			{
				_context.Plan.Remove(plan);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool PlanExists(int id)
		{
			return _context.Plan.Any(e => e.PlanId == id);
		}
	}
}
