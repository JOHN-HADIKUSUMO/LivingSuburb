using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using LivingSuburb.Models;
using LivingSuburb.Database;
using LivingSuburb.Services;

namespace Tests
{
    public class JobServiceTest : Base
    {
        private JobService jobService;
        [SetUp]
        public void Setup()
        {
            jobService = new JobService(context);
        }

        [TestCase]
        public void Create_Test()
        {
            Job job = jobService.Read(8);
            string title = job.Title;
            for(int i=0;i<200;i++)
            {
                Job temp = new Job();
                temp.Title = job.Title + " - " + i.ToString();
                temp.Url = job.Url;
                temp.Suburb = job.Suburb;
                temp.SubCategory = job.SubCategory;
                temp.State = job.State;
                temp.ShortDescription = job.ShortDescription;
                temp.PublishedDate = job.PublishedDate;
                temp.FullDescription = job.FullDescription;
                temp.Company = job.Company;
                temp.Code = job.Code;
                temp.ClosingDate = job.ClosingDate;
                temp.Category = job.Category;
                jobService.Create(temp);
            }
            Assert.IsTrue(true);
        }


        [TestCase()]
        public void LINQQuery_Test1()
        {
            List<int> test = context.Jobs.GroupJoin(this.context.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { JobId = z.a.JobId, Title = z.a.Title, Company = z.a.Company, Code = z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                .GroupJoin(context.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, Title = h.a.Title, Company = h.a.Company, Code = h.a.Code, CategoryId = h.a.CategoryId, StateId = h.a.StateId, PublishingDate = h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                .Join(context.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = b.Name, StateId = a.StateId, PublishingDate = a.PublishingDate, TagName = a.TagName })
                .Join(context.States, a => a.StateId, b => b.StateId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = a.Category, State = b.LongName, PublishingDate = a.PublishingDate, TagName = a.TagName })
                .OrderByDescending(d => d.PublishingDate).Select(s => s.Id).ToList();
            Assert.IsNotNull(test);
        }

        [TestCase(new int[] {21,22,23,24,25,26 })]
        public void LINQQuery_Test2(int[] ids)
        {
            var result = context.Jobs.Where(w=> ids.Contains(w.JobId)).Join(context.JobCategories, a => a.Category, b => b.CategoryId, (a, b) => new { Id = a.JobId, Title = a.Title, ShortDescription = a.ShortDescription, URL = a.Url, Category = b.Name, PublishingDate = a.PublishedDate, SubCategoryId = a.SubCategory, StateId = a.State, SuburbId = a.Suburb })
                .Join(context.States, s => s.StateId, t => t.StateId, (s, t) => new { Id = s.Id, Title = s.Title, ShortDescription = s.ShortDescription, URL = s.URL, Category = s.Category, PublishingDate = s.PublishingDate, SubCategoryId = s.SubCategoryId, State = t.LongName, SuburbId = s.SuburbId })
                .GroupJoin(context.JobSubCategories, z => z.SubCategoryId, y => y.JobSubCategoryId, (z, y) => new { Id = z.Id, Title = z.Title, ShortDescription = z.ShortDescription, URL = z.URL, Category = z.Category, PublishingDate = z.PublishingDate, State = z.State, SuburbId = z.SuburbId, y })
                .SelectMany(m => m.y.DefaultIfEmpty(), (k, l) => new { Id = k.Id, Title = k.Title, ShortDescription = k.ShortDescription, URL = k.URL, Category = k.Category, SubCategory = l == null ? string.Empty : l.Name, PublishingDate = k.PublishingDate, State = k.State, SuburbId = k.SuburbId })
                .GroupJoin(context.Suburbs, p => p.SuburbId, q => q.SuburbId, (p, q) => new { Id = p.Id, Title = p.Title, ShortDescription = p.ShortDescription, URL = p.URL, Category = p.Category, SubCategory = p.SubCategory, State = p.State, PublishingDate = p.PublishingDate, q })
                .SelectMany(n => n.q.DefaultIfEmpty(), (f, g) => new JobSimple { Id = f.Id, Title = f.Title, Description = f.ShortDescription, URL = f.URL, Category = f.Category, SubCategory = f.SubCategory, State = f.State, Suburb = g == null ? string.Empty : g.Name, PublishingDate = f.PublishingDate})
                .OrderByDescending(d => d.PublishingDate).ToList();

            Assert.IsNotNull(result);
        }

    }
}