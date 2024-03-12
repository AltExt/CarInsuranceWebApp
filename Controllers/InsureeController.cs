using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarInsuranceWebApp.Models;
using CarInsuranceWebApp.ViewModels;

namespace CarInsuranceWebApp.Controllers
{
	public class InsureeController : Controller
	{
		private InsuranceEntities db = new InsuranceEntities();

		// GET: Insuree
		public ActionResult Index()
		{
			return View(db.Insurees.ToList());
		}

		// GET: Insuree/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Insuree insuree = db.Insurees.Find(id);
			if (insuree == null)
			{
				return HttpNotFound();
			}
			return View(insuree);
		}

		// GET: Insuree/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: Insuree/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insuree insuree)
		{
			if (ModelState.IsValid)
			{
				insuree.Quote = CalculateQuote(insuree);
				db.Insurees.Add(insuree);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(insuree);
		}

		// GET: Insuree/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Insuree insuree = db.Insurees.Find(id);
			if (insuree == null)
			{
				return HttpNotFound();
			}
			return View(insuree);
		}

		// POST: Insuree/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insuree insuree)
		{
			if (ModelState.IsValid)
			{
				insuree.Quote = CalculateQuote(insuree);
				db.Entry(insuree).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(insuree);
		}

		// GET: Insuree/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Insuree insuree = db.Insurees.Find(id);
			if (insuree == null)
			{
				return HttpNotFound();
			}
			return View(insuree);
		}

		// POST: Insuree/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			Insuree insuree = db.Insurees.Find(id);
			db.Insurees.Remove(insuree);
			db.SaveChanges();
			return RedirectToAction("Index");
		}

		public ActionResult Admin()
		{
			List<Insuree> insureesFromDB = db.Insurees.ToList();
			List<InsureeVM> insurees = new List<InsureeVM>();

			foreach(Insuree i in insureesFromDB)
			{
				InsureeVM vm = new InsureeVM()
				{
					Id = i.Id,
					FirstName = i.FirstName,
					LastName = i.LastName,
					EmailAddress = i.EmailAddress,
					Quote = i.Quote,
				};
				insurees.Add(vm);
			}

			return View(insurees);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}

		private decimal CalculateQuote(Insuree insuree)
		{
			decimal quoteValue = 50;
			int insureeAge = DateTime.Now.Year - insuree.DateOfBirth.Year - 1;
			if (DateTime.Now.Month > insuree.DateOfBirth.Month || (DateTime.Now.Month == insuree.DateOfBirth.Month && DateTime.Now.Day > insuree.DateOfBirth.Day))
			{
				insureeAge++;
			}
			if (insureeAge <= 18)
			{
				quoteValue += 100;
			}
			else if (insureeAge >= 26)
			{
				quoteValue += 25;
			}
			else
			{
				quoteValue += 50;
			}

			if (insuree.CarYear > 2015 || insuree.CarYear < 2000)
			{
				quoteValue += 25;
			}

			if (insuree.CarMake.ToLower() == "porsche")
			{
				quoteValue += 25;
				if (insuree.CarModel.ToLower() == "911 carrera")
				{
					quoteValue += 25;
				}
			}

			quoteValue += 10 * insuree.SpeedingTickets;

			if (insuree.DUI) quoteValue *= 1.25m;

			if (insuree.CoverageType) quoteValue *= 1.5m;

			return quoteValue;
		}
	}
}
