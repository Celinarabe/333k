using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rabe_Celina_HW5.DAL;
using Rabe_Celina_HW5.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Rabe_Celina_HW5.Controllers
{
    public enum SortOrder { GreaterThan, LessThan }
    public class HomeController : Controller
    {
        // GET: Home
        public IActionResult Index(string SearchString)
        {
            //print out list 
            List<Repository> SelectedRepositories = new List<Repository>();

            //if user didn't search anything
            if (SearchString == null || SearchString == "")
            {
                ViewBag.TotalRepositories = _db.Repositories.Count();
                ViewBag.SelectedRepositories = ViewBag.TotalRepositories;
                SelectedRepositories = _db.Repositories.Include("Language").ToList();
                return View(SelectedRepositories);
            }
            else
            {
                //if user did search, begin the query
                var query = from r in _db.Repositories
                            select r;
                //sets requirements for r
                query = query.Where(r => r.RepositoryName.Contains(SearchString) || r.UserName.Contains(SearchString));
                SelectedRepositories = query.Include(r => r.Language).ToList();

                ViewBag.TotalRepositories = _db.Repositories.Count();
                ViewBag.SelectedRepositories = SelectedRepositories.Count();
                return View(SelectedRepositories.OrderByDescending(r => r.StarCount));


            }
        }
        private AppDbContext _db;

        public HomeController(AppDbContext context)
        {
            _db = context;
        }



        public IActionResult Details(int? id)
        {
            if (id == null) //Repo id not specified
            {
                return View("Error", new String[] { "Repository ID not specified - which repo do you want to view?" });
            }

            Repository repo = _db.Repositories.Include(r => r.Language).FirstOrDefault(r => r.RepositoryID == id);

            if (repo == null) //Repo does not exist in database
            {
                return View("Error", new String[] { "Repository not found in database" });
            }

            //if code gets this far, all is well
            return View(repo);

        }

        public ActionResult DetailedSearch()
        {
            ViewBag.AllLanguages = GetAllLanguages();
            return View();
        }

        public SelectList GetAllLanguages()
        {
            List<Language> Languages = _db.Languages.ToList();

            Language SelectNone = new Models.Language() { LanguageID = 0, Name = "All Languages" };
            Languages.Add(SelectNone);

            SelectList AllLanguages = new SelectList(Languages.OrderBy(l => l.LanguageID), "LanguageID", "Name");

            return AllLanguages;
        }


        public ActionResult DisplaySearchResults(String SearchName, String SearchDescription, int LanguageSelected, String SearchStars, 
                                                 SortOrder SelectedOrder, DateTime? SelectedDate)

        {
            List<Repository> SelectedRepositories = new List<Repository>();
            var query = from r in _db.Repositories
                        select r;
            SelectedRepositories = query.Include(r => r.Language).ToList();


            if (SearchName == null || SearchName == "")
            {
                ViewBag.SearchName = "null";
            }
            else
            {
                query = query.Where(r => r.RepositoryName.Contains(SearchName) || r.UserName.Contains(SearchName));
            }

            if (SearchDescription == null || SearchDescription == "")
            {
                ViewBag.SearchDescription = "Repository name search string was null. Please enter a description.";
            }
            else
            {
                query = query.Where(r => r.Description.Contains(SearchDescription));
            }



            if (LanguageSelected == 0)
            {
                ViewBag.LanguageSelected = "null";
            }
            else
            {
                Language LanguagesToDisplay = _db.Languages.Find(LanguageSelected);
                query = query.Where(r => r.Language.LanguageID == (LanguageSelected));
            }


            if (SearchStars != null && SearchStars != "")
            {
                Decimal decNumberOfStars;
                try
                {
                    decNumberOfStars = Convert.ToDecimal(SearchStars);
                    if (SelectedOrder == SortOrder.GreaterThan)
                    {
                        query = query.Where(r => r.StarCount >= decNumberOfStars);
                    }
                    else
                    {
                        query = query.Where(r => r.StarCount <= decNumberOfStars);
                    }
                }
                catch
                {
                    ViewBag.SeachStars = "null";
                }
            }

            if (SelectedDate != null)
            {
                DateTime datSelected = SelectedDate ?? new DateTime(1990, 1, 1);
                query = query.Where(r => r.LastUpdate >= datSelected);
            }
            else
            {
                ViewBag.SelectedDate = "null";
            }
  

    
            SelectedRepositories = query.ToList();

            //displaying number out of 250 repos
            ViewBag.SelectedRepositories = SelectedRepositories.Count;
            ViewBag.TotalRepositories = _db.Repositories.Count();
            return View("Index", SelectedRepositories);
        }

    }
   }
