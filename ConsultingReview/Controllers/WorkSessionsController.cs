using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ConsultingReview.Models;
using Microsoft.AspNetCore.Http;

namespace ConsultingReview.Controllers
{
    public class WorkSessionsController : Controller
    {
        private readonly ConsultingContext _context;

        public WorkSessionsController(ConsultingContext context)
        {
            _context = context;
        }

        // GET: WorkSessions
        public async Task<IActionResult> Index(string id, string name)
        {
            if (String.IsNullOrWhiteSpace(id)) 
            {
                if (String.IsNullOrEmpty(HttpContext.Session.GetString("code")))
                {
                    TempData["error"] = "Contract is not available";

                    return RedirectToAction("Index", "Contracts");
                }
                else
                {
                    id = HttpContext.Session.GetString("code");
                }
            }
            else
            {
                HttpContext.Session.SetString("code", id);
            }

            if (String.IsNullOrWhiteSpace(name))
            {
                if (String.IsNullOrEmpty(HttpContext.Session.GetString("name")))
                {
                    //TempData["error"] = "Contract is not available";

                    //return RedirectToAction("Index", "Contracts");
                }
                else
                {
                    id = HttpContext.Session.GetString("name");
                }
            }
            else
            {
                HttpContext.Session.SetString("name", name);
            }

            ViewData["id"] = id;
            ViewData["Name"] = name;

            var consultingContext = _context.WorkSession
                .Include(w => w.Consultant)
                .Include(w => w.Contract)
                .OrderByDescending(w => w.DateWorked)
                .Where(w => w.ContractId.ToString()==id);
            return View(await consultingContext.ToListAsync());
        }

        // GET: WorkSessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workSession = await _context.WorkSession
                .Include(w => w.Consultant)
                .Include(w => w.Contract)
                .FirstOrDefaultAsync(m => m.WorkSessionId == id);
            if (workSession == null)
            {
                return NotFound();
            }

            return View(workSession);
        }

        // GET: WorkSessions/Create
        public IActionResult Create(string id, string name)
        {
            ViewData["id"] = id;
            ViewData["Name"] = name;
            ViewData["ConsultantId"] = new SelectList(_context.Consultant.OrderBy(m=>m.LastName), "ConsultantId", "LastName");
            WorkSession worksessiocn = new WorkSession { HourlyRate = 0, TotalChargeBeforeTax = 0 };  // , DateWorked = DateTime.Now
            
            return View(worksessiocn);
        }

        // POST: WorkSessions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WorkSessionId,ContractId,DateWorked,ConsultantId,HoursWorked,WorkDescription,HourlyRate,ProvincialTax,TotalChargeBeforeTax")] WorkSession workSession)
        {
            if (ModelState.IsValid)
            {
                /// Retreive Hourly rate //////
                var consultant = _context.Consultant.Where(e => e.ConsultantId == workSession.ConsultantId).FirstOrDefault();
                if (consultant != null)
                {
                    workSession.HourlyRate = consultant.HourlyRate;
                    workSession.TotalChargeBeforeTax = consultant.HourlyRate * workSession.HoursWorked;
                }
                ///////////////////////////////////////
                ///
                _context.Add(workSession);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index","WorkSessions");
            }
            ViewData["ConsultantId"] = new SelectList(_context.Consultant, "ConsultantId", "FirstName", workSession.ConsultantId);
            ViewData["ContractId"] = new SelectList(_context.Contract, "ContractId", "Name", workSession.ContractId);
            return View(workSession);
        }

        // GET: WorkSessions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workSession = await _context.WorkSession.FindAsync(id);
            if (workSession == null)
            {
                return NotFound();
            }

            ViewData["ConsultantId"] = new SelectList(_context.Consultant.OrderBy(m => m.LastName), "ConsultantId", "LastName");
            return View(workSession);
        }

        // POST: WorkSessions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkSessionId,ContractId,DateWorked,ConsultantId,HoursWorked,WorkDescription,HourlyRate,ProvincialTax,TotalChargeBeforeTax")] WorkSession workSession)
        {
            if (id != workSession.WorkSessionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workSession);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkSessionExists(workSession.WorkSessionId))
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
            ViewData["ConsultantId"] = new SelectList(_context.Consultant, "ConsultantId", "FirstName", workSession.ConsultantId);
            ViewData["ContractId"] = new SelectList(_context.Contract, "ContractId", "Name", workSession.ContractId);
            return View(workSession);
        }

        // GET: WorkSessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workSession = await _context.WorkSession
                .Include(w => w.Consultant)
                .Include(w => w.Contract)
                .FirstOrDefaultAsync(m => m.WorkSessionId == id);
            if (workSession == null)
            {
                return NotFound();
            }

            return View(workSession);
        }

        // POST: WorkSessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workSession = await _context.WorkSession.FindAsync(id);
            _context.WorkSession.Remove(workSession);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkSessionExists(int id)
        {
            return _context.WorkSession.Any(e => e.WorkSessionId == id);
        }
    }
}
