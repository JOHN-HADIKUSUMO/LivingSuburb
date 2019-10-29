using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LivingSuburb.Models;
using LivingSuburb.Services;

namespace LivingSuburb.Web.Controllers.API
{
    public class JobsController : ControllerBase
    {
        private TagService tagService;
        private JobService jobService;
        private JobTagService jobTagService;
        private CategoryService categoryService;
        private SubCategoryService subCategoryService;
        private StateService stateService;
        private SuburbService suburbService;

        public JobsController(TagService tagService,JobService jobService, JobTagService jobTagService, CategoryService categoryService, SubCategoryService subCategoryService, StateService stateService, SuburbService suburbService)
        {
            this.tagService = tagService;
            this.jobService = jobService;
            this.jobTagService = jobTagService;
            this.categoryService = categoryService;
            this.subCategoryService = subCategoryService;
            this.stateService = stateService;
            this.suburbService = suburbService;
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/JOBS/DELETE")]
        public IActionResult Delete([FromBody] DeleteJob model)
        {
            if(jobService.Delete(model.Id))
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/JOBS/UPDATE")]
        public IActionResult Update([FromBody] JobAdd model)
        {
            Job job = jobService.Read(model.JobId);
            if(job != null)
            {
                job.Title = model.Title;
                job.TitleURL = Regex.Replace(model.Title, "[^0-9a-zA-Z]+", "-");
                job.Code = model.Code;
                job.Company = model.Company;
                job.Category = (int)model.Category.Id;
                job.SubCategory = model.SubCategory == null ? null : model.SubCategory.Id;
                job.State = (int)model.State.Id;
                job.Suburb = model.Suburb == null ? null : model.Suburb.Id;
                job.ShortDescription = model.ShortDescription;
                job.FullDescription = model.FullDescription;
                job.Url = model.Url;
                job.PublishedDate = model.PublishingDate;
                job.ClosingDate = model.ClosingDate;

                if(jobService.Update(job))
                {
                    if (model.NewTags != null)
                    {
                        if (model.NewTags.Count() > 0)
                        {
                            foreach (SimpleItem item in model.NewTags)
                            {
                                JobTag jobTag = new JobTag();
                                jobTag.JobId = job.JobId;
                                jobTag.TagId = (int)item.Id;
                                jobTagService.Create(jobTag);
                            }
                        }
                    }

                    if(model.DeletedTags != null)
                    {
                        if (model.DeletedTags.Count() > 0)
                        {
                            foreach (SimpleItem item in model.DeletedTags)
                            {
                                JobTag jobTag = new JobTag();
                                jobTag.JobId = job.JobId;
                                jobTag.TagId = (int)item.Id;
                                jobTagService.Delete(jobTag);
                            }
                        }
                    }
                }
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        [Route("API/JOBS/ADDBYUSER")]
        public IActionResult AddByUser([FromBody] JobAdd model)
        {
            try
            {
                string userid = User.Claims.Where(w => w.Type.Contains("nameidentifier")).FirstOrDefault().Value;
                Job job = new Job();
                job.Title = model.Title;
                job.UserId = userid;
                job.TitleURL = Regex.Replace(model.Title, "[^0-9a-zA-Z]+", "-");
                job.Code = model.Code;
                job.Company = string.IsNullOrEmpty(model.Company)?"Private Advertiser": model.Company;
                job.Category = (int)model.Category.Id;
                job.SubCategory = model.SubCategory == null ? null : model.SubCategory.Id;
                job.State = (int)model.State.Id;
                job.Suburb = model.Suburb == null ? null : model.Suburb.Id;
                job.ShortDescription = model.ShortDescription;
                job.FullDescription = model.FullDescription;
                job.Url = null;
                job.PublishedDate = DateTime.Now;
                job.ClosingDate = new DateTime(model.ClosingDate.Year, model.ClosingDate.Month, model.ClosingDate.Day, 23, 59, 59);

                jobService.Create(job);
                if (model.NewTags != null)
                {
                    if (model.NewTags.Count() > 0)
                    {
                        foreach (SimpleItem item in model.NewTags)
                        {
                            if (item.Id == null)
                            {
                                Tag tempTag = tagService.Read(item.Name, (int)TagGroupType.Job_Search);
                                if (tempTag == null)
                                {
                                    Tag tag = new Tag();
                                    tag.Name = item.Name;
                                    tag.TagGroupId = (int)TagGroupType.Job_Search;
                                    tagService.Create(tag);
                                    item.Id = tag.TagId;
                                }
                                else
                                {
                                    item.Id = tempTag.TagId;
                                }
                            }

                            JobTag jobTag = new JobTag();
                            jobTag.JobId = job.JobId;
                            jobTag.TagId = (int)item.Id;
                            jobTagService.Create(jobTag);
                        }
                    }

                    if (model.DeletedTags.Count() > 0)
                    {
                        foreach (SimpleItem item in model.DeletedTags)
                        {
                            JobTag jobTag = new JobTag();
                            jobTag.JobId = job.JobId;
                            jobTag.TagId = (int)item.Id;
                            jobTagService.Delete(jobTag);
                        }
                    }
                }
                return new OkObjectResult(true);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult("Fail to save the new job.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/JOBS/ADD")]
        public IActionResult Add([FromBody] JobAdd model)
        {
            try
            {
                string userid = User.Claims.Where(w => w.Type.Contains("nameidentifier")).FirstOrDefault().Value;
                Job job = new Job();
                job.Title = model.Title;
                job.UserId = userid;
                job.TitleURL = Regex.Replace(model.Title, "[^0-9a-zA-Z]+", "-");
                job.Code = model.Code;
                job.Company = model.Company;
                job.Type = (int)model.Type.Id;
                job.Term = (int)model.Term.Id;
                job.Category = (int)model.Category.Id;
                job.SubCategory = model.SubCategory == null ? null : model.SubCategory.Id;
                job.State = (int)model.State.Id;
                job.Suburb = model.Suburb == null ? null : model.Suburb.Id;
                job.ShortDescription = model.ShortDescription;
                job.FullDescription = model.FullDescription;
                job.Url = model.Url;
                job.PublishedDate = new DateTime(model.PublishingDate.Year, model.PublishingDate.Month, model.PublishingDate.Day,DateTime.Now.Hour,DateTime.Now.Minute,DateTime.Now.Second);
                job.ClosingDate = new DateTime(model.ClosingDate.Year, model.ClosingDate.Month, model.ClosingDate.Day, 23,59,59); 
                jobService.Create(job);

                if (model.NewTags != null)
                {
                    if (model.NewTags.Count() > 0)
                    {
                        foreach (SimpleItem item in model.NewTags)
                        {
                            if (item.Id == null)
                            {
                                Tag tempTag = tagService.Read(item.Name, (int)TagGroupType.Job_Search);
                                if (tempTag == null)
                                {
                                    Tag tag = new Tag();
                                    tag.Name = item.Name;
                                    tag.TagGroupId = (int)TagGroupType.Job_Search;
                                    tagService.Create(tag);
                                    item.Id = tag.TagId;
                                }
                                else
                                {
                                    item.Id = tempTag.TagId;
                                }
                            }

                            JobTag jobTag = new JobTag();
                            jobTag.JobId = job.JobId;
                            jobTag.TagId = (int)item.Id;
                            jobTagService.Create(jobTag);
                        }
                    }

                    if (model.DeletedTags.Count() > 0)
                    {
                        foreach (SimpleItem item in model.DeletedTags)
                        {
                            JobTag jobTag = new JobTag();
                            jobTag.JobId = job.JobId;
                            jobTag.TagId = (int)item.Id;
                            jobTagService.Delete(jobTag);
                        }
                    }
                }
                return new OkObjectResult(true);
            }
            catch(Exception ex)
            {
                return new BadRequestObjectResult(ex.GetBaseException().ToString());
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Route("API/JOBS/EDIT/{id}")]
        public IActionResult Edit(int id)
        {
            JobEdit result = new JobEdit();

            Job job = jobService.Read(id);
            if(job != null)
            {
                result.JobId = job.JobId;
                result.Title = job.Title;
                result.TitleURL = Regex.Replace(result.Title, "[^0-9a-zA-Z]+", "-");
                result.Code = job.Code;
                result.Company = job.Company;

                JobCategory jobCategory = categoryService.Read(job.Category);
                if(jobCategory != null)
                {
                    result.Category = new SimpleItem(jobCategory.CategoryId, jobCategory.Name);
                }

                if(job.SubCategory != null)
                {
                    JobSubCategory jobSubCategory = subCategoryService.Read((int)job.SubCategory);
                    if (jobSubCategory != null)
                    {
                        result.SubCategory = new SimpleItem(jobSubCategory.JobCategoryId, jobSubCategory.Name);
                    }
                }

                State state = stateService.Read(job.State);
                if (state != null)
                {
                    result.State = new SimpleItem(state.StateId, state.ShortName);
                }

                if(job.Suburb != null)
                {
                    Suburb suburb = suburbService.Read((int)job.Suburb);
                    if (suburb != null)
                    {
                        result.Suburb = new SimpleItem(suburb.SuburbId, suburb.Name);
                    }
                }
                result.ShortDescription = job.ShortDescription;
                result.FullDescription = job.FullDescription;
                result.Url = job.Url;
                result.PublishingDate = string.Format("{0:yyyy-MM-dd hh:mm}", job.PublishedDate);
                result.ClosingDate = string.Format("{0:yyyy-MM-dd hh:mm}", job.ClosingDate); 

                List<JobTag> jobTags = jobTagService.ReadAllByJobId(id);
                if(jobTags != null)
                {
                    if(jobTags.Count() > 0)
                    {
                        for(int i=0;i<jobTags.Count;i++)
                        {
                            Tag tag = tagService.Read(jobTags[i].TagId);
                            if(tag!=null)
                            {
                                result.EditTags.Add(new SimpleItem(tag.TagId,tag.Name));
                            }
                        }
                    }
                }
                return Ok(result);
            }
            return NotFound();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("API/JOBS/SEARCH")]
        public IActionResult Search([FromBody] JobSearch model)
        {
            SearchJobListResult result = jobService.Search(model.Keywords, model.Category, model.SubCategory, model.State, model.PageNo, model.PageSize, model.BlockSize);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/JOBS/SEARCH2")]
        public IActionResult Search2([FromBody] JobSearch2 model)
        {
            SearchJobListResult result = jobService.Search2(model.Keywords, model.Category, model.SubCategory, model.State, model.OrderBy, model.PageNo, model.PageSize, model.BlockSize);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        [Route("API/JOBS/SEARCH3")]
        public IActionResult Search3([FromBody] JobSearch2 model)
        {
            string userid = User.Claims.Where(w => w.Type.Contains("nameidentifier")).FirstOrDefault().Value;
            SearchJobListResult result = jobService.Search2(userid, model.Keywords, model.Category, model.SubCategory, model.State, model.OrderBy, model.PageNo, model.PageSize, model.BlockSize);
            return Ok(result);
        }
    }
}
