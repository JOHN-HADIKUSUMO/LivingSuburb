using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LivingSuburb.Models;
using LivingSuburb.Services;

namespace LivingSuburb.Web.Controllers
{
    public class JobsController : Controller
    {
        private CategoryService categoryService;
        private SubCategoryService subCategoryService;
        private StateService stateService;
        private SuburbService suburbService;
        private JobService jobService;

        public JobsController(
            CategoryService categoryService,
            SubCategoryService subCategoryService, 
            StateService stateService,
            JobService jobService,
            SuburbService suburbService
            )
        {
            this.categoryService = categoryService;
            this.subCategoryService = subCategoryService;
            this.stateService = stateService;
            this.jobService = jobService;
            this.suburbService = suburbService;
        }

        [Route("Jobs")]
        public IActionResult Index()
        {

            return View();
        }

        [Route("Jobs/Detail/{id}/{title}")]
        public IActionResult Detail(int id,string title)
        {
            Job job = jobService.Read(id);
            if (job == null)
                return Redirect("/Error/400");
            JobCategory category = categoryService.Read(job.Category);
            if (category == null)
                return Redirect("/Error/400");

            JobSubCategory subcategory = subCategoryService.Read(job.SubCategory ?? 0);

            State state = stateService.Read(job.State);

            Suburb suburb = this.suburbService.Read(job.Suburb ?? 0);

            JobDetail detail = new JobDetail();
            detail.Id = job.JobId;
            detail.Title = job.Title;
            detail.FullDescription = job.FullDescription;
            detail.Url = job.Url;
            detail.Code = job.Code;
            detail.Company = job.Company;
            detail.Category = category.Name;
            detail.SubCategory = subcategory == null ? string.Empty : subcategory.Name;
            detail.State = state.ShortName;
            detail.Suburb = suburb == null ? string.Empty : suburb.Name;
            detail.StrClosingDate = string.Format("{0:dd/MMM/yyyy hh:mm tt}", job.ClosingDate);
            detail.StrPublishedDate = string.Format("{0:dd/MMM/yyyy hh:mm tt}", job.PublishedDate);
            return View(detail);
        }
    }
}
