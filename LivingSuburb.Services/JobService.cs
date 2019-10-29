using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using LivingSuburb.Models;
using LivingSuburb.Database;

namespace LivingSuburb.Services
{
    public class JobService
    {
        private DataContext datacontext;
        public JobService(DataContext context)
        {
            datacontext = context;
        }

        public Job Read(int id)
        {
            return datacontext.Jobs.Where(w => w.JobId == id).FirstOrDefault<Job>();
        }

        public void Create(Job model)
        {
            datacontext.Jobs.Add(model);
            datacontext.SaveChanges();
        }


        public bool Update(Job model)
        {
            bool status = false;
            if (datacontext.Jobs.Where(w => w.JobId == model.JobId).Any())
            {
                datacontext.Jobs.Update(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Delete(Job model)
        {
            bool status = false;
            if (datacontext.Jobs.Where(w => w.JobId == model.JobId).Any())
            {
                datacontext.Jobs.Remove(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Delete(int jobId)
        {
            bool status = false;
            Job job = datacontext.Jobs.Where(w => w.JobId == jobId).FirstOrDefault();
            if (job != null)
            {
                datacontext.Jobs.Remove(job);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public SearchJobListResult Search(string Keywords, int Category, int SubCategory, int State, int PageNo, int PageSize, int BlockSize)
        {
            SearchJobListResult result = new SearchJobListResult();
            PageSize = PageSize <= 0 ? 10 : PageSize;
            BlockSize = BlockSize <= 0 ? 10 : BlockSize;

            int Start = 0;
            int Stop = 0;
            int NumberOfPages = 0;
            int NumberOfBlocks = 0;
            int BlockNo = 0;
            int NumberOfRecords = 0;
            List<int> Ids = new List<int>() { };

            if (string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State == 0)
            {
                Ids = this.datacontext.Jobs.OrderByDescending(o => o.PublishedDate).Select(s => s.JobId).ToList();
            }
            else if (string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State != 0)
            {
                Ids = this.datacontext.Jobs.OrderByDescending(o => o.PublishedDate).Where(w => w.State == State).Select(s => s.JobId).ToList();
            }
            else if (string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State == 0)
            {
                Ids = this.datacontext.Jobs.OrderByDescending(o => o.PublishedDate).Where(w => w.Category == Category).Select(s => s.JobId).ToList();
            }
            else if (string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State != 0)
            {
                Ids = this.datacontext.Jobs.OrderByDescending(o => o.PublishedDate).Where(w => w.Category == Category && w.State == State).Select(s => s.JobId).ToList();
            }
            else if (!string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State == 0)
            {
                Ids = this.datacontext.Jobs.GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                    .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { JobId = z.a.JobId, Title = z.a.Title, Description = z.a.ShortDescription, URL = z.a.Url, Company = z.a.Company, Code = z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                    .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                    .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, Title = h.a.Title, Company = h.a.Company, Code = h.a.Code, CategoryId = h.a.CategoryId, StateId = h.a.StateId, PublishingDate = h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                    .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = b.Name, StateId = a.StateId, PublishingDate = a.PublishingDate, TagName = a.TagName })
                    .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = a.Category, State = b.LongName, PublishingDate = a.PublishingDate, TagName = a.TagName })
                    .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                    .OrderByDescending(d => d.PublishingDate).Select(s => s.Id).ToList();
            }
            else if (!string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State != 0)
            {
                Ids = this.datacontext.Jobs.Where(w => w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                    .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { JobId = z.a.JobId, Title = z.a.Title, Company = z.a.Company, Code = z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                    .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                    .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, Title = h.a.Title, Company = h.a.Company, Code = h.a.Code, CategoryId = h.a.CategoryId, StateId = h.a.StateId, PublishingDate = h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                    .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = b.Name, StateId = a.StateId, PublishingDate = a.PublishingDate, TagName = a.TagName })
                    .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = a.Category, State = b.LongName, PublishingDate = a.PublishingDate, TagName = a.TagName })
                    .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                    .OrderByDescending(d => d.PublishingDate).Select(s => s.Id).ToList();
            }
            else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State == 0)
            {
                Ids = this.datacontext.Jobs.Where(w => w.Category == Category).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                    .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { JobId = z.a.JobId, Title = z.a.Title, Company = z.a.Company, Code = z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                    .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                    .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, Title = h.a.Title, Company = h.a.Company, Code = h.a.Code, CategoryId = h.a.CategoryId, StateId = h.a.StateId, PublishingDate = h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                    .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = b.Name, StateId = a.StateId, PublishingDate = a.PublishingDate, TagName = a.TagName })
                    .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = a.Category, State = b.LongName, PublishingDate = a.PublishingDate, TagName = a.TagName })
                    .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                    .OrderByDescending(d => d.PublishingDate).Select(s => s.Id).ToList();
            }
            else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory != 0 && State == 0)
            {
                Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.SubCategory == SubCategory).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                    .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { JobId = z.a.JobId, Title = z.a.Title, Company = z.a.Company, Code = z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                    .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                    .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, Title = h.a.Title, Company = h.a.Company, Code = h.a.Code, CategoryId = h.a.CategoryId, StateId = h.a.StateId, PublishingDate = h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                    .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = b.Name, StateId = a.StateId, PublishingDate = a.PublishingDate, TagName = a.TagName })
                    .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = a.Category, State = b.LongName, PublishingDate = a.PublishingDate, TagName = a.TagName })
                    .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                    .OrderByDescending(d => d.PublishingDate).Select(s => s.Id).ToList();
            }
            else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State != 0)
            {
                Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                    .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { JobId = z.a.JobId, Title = z.a.Title, Company = z.a.Company, Code = z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                    .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                    .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, Title = h.a.Title, Company = h.a.Company, Code = h.a.Code, CategoryId = h.a.CategoryId, StateId = h.a.StateId, PublishingDate = h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                    .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = b.Name, StateId = a.StateId, PublishingDate = a.PublishingDate, TagName = a.TagName })
                    .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = a.Category, State = b.LongName, PublishingDate = a.PublishingDate, TagName = a.TagName })
                    .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                    .OrderByDescending(d => d.PublishingDate).Select(s => s.Id).ToList();
            }
            else //if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory != 0 && State != 0)
            {
                Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.SubCategory == SubCategory && w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                    .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { JobId = z.a.JobId, Title = z.a.Title, Company = z.a.Company, Code = z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                    .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                    .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, Title = h.a.Title, Company = h.a.Company, Code = h.a.Code, CategoryId = h.a.CategoryId, StateId = h.a.StateId, PublishingDate = h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                    .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = b.Name, StateId = a.StateId, PublishingDate = a.PublishingDate, TagName = a.TagName })
                    .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = a.Category, State = b.LongName, PublishingDate = a.PublishingDate, TagName = a.TagName })
                    .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                    .OrderByDescending(d => d.PublishingDate).Select(s => s.Id).ToList();
            }

            NumberOfRecords = Ids.Count;

            if (NumberOfRecords == 0)
            {
                NumberOfPages = 0;
                NumberOfBlocks = 0;
                PageNo = 0;
                BlockNo = 0;
            }
            else
            {
                if (NumberOfRecords <= PageSize)
                {
                    NumberOfPages = 1;
                    NumberOfBlocks = 1;
                    PageNo = 1;
                    BlockNo = 1;
                }
                else
                {
                    NumberOfPages = NumberOfRecords / PageSize;
                    if (NumberOfRecords % PageSize > 0)
                    {
                        NumberOfPages++;
                    }

                    NumberOfBlocks = NumberOfPages / BlockSize;
                    if (NumberOfPages % BlockSize > 0)
                    {
                        NumberOfBlocks++;
                    }

                    if (PageNo > NumberOfPages)
                    {
                        PageNo = NumberOfPages;
                    }

                    if (PageNo <= BlockSize)
                    {
                        BlockNo = 1;
                    }
                    else
                    {
                        BlockNo = (int)Math.Abs(Math.Floor((decimal)PageNo / (decimal)BlockSize));
                        if (PageNo % BlockSize > 0)
                        {
                            BlockNo++;
                        }
                    }
                }

                Start = ((BlockNo - 1) * BlockSize) + 1;
                Stop = BlockNo * BlockSize;

                if (Stop > NumberOfPages) Stop = NumberOfPages;

                for (int i = Start; i <= Stop; i++)
                {
                    result.Pages.Add(i);
                }

                if (NumberOfBlocks == 1)
                {
                    result.First = null;
                    result.Prev = null;
                    result.Next = null;
                    result.Last = null;
                }
                else
                {
                    if (BlockNo == NumberOfBlocks)
                    {
                        result.First = 1;
                        result.Prev = PageNo - 1;
                        result.Next = null;
                        result.Last = null;
                    }
                    else if (BlockNo == 1)
                    {
                        result.First = null;
                        result.Prev = null;
                        result.Next = PageNo + 1;
                        result.Last = NumberOfPages;
                    }
                    else
                    {
                        result.First = 1;
                        result.Prev = PageNo - 1;
                        result.Next = PageNo + 1;
                        result.Last = NumberOfPages;
                    }
                }

                int Skip = (Start - 1) * PageSize;
                int Take = (Stop - (Start - 1)) * PageSize;
                List<int> filteredIds = Ids.Skip(Skip).Take(Take).ToList();
                List<JobSimple> temps = this.datacontext.Jobs.Where(w => filteredIds.Contains(w.JobId)).Join(this.datacontext.JobCategories, a => a.Category, b => b.CategoryId, (a, b) => new { Id = a.JobId, a.Title, a.TitleURL, a.Company, a.ShortDescription, URL = a.Url, Category = b.Name, PublishingDate = a.PublishedDate, SubCategoryId = a.SubCategory, StateId = a.State, SuburbId = a.Suburb })
                .Join(this.datacontext.States, s => s.StateId, t => t.StateId, (s, t) => new {s.Id, s.Title, s.TitleURL, s.Company, s.ShortDescription, s.URL, s.Category, s.PublishingDate, s.SubCategoryId, State = t.ShortName, s.SuburbId })
                .GroupJoin(this.datacontext.JobSubCategories, z => z.SubCategoryId, y => y.JobSubCategoryId, (z, y) => new { z.Id, z.Title, z.TitleURL, z.Company, z.ShortDescription, z.URL, z.Category, z.PublishingDate, z.State, z.SuburbId, y })
                .SelectMany(m => m.y.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.Company, k.ShortDescription, k.URL, k.Category, SubCategory = l == null ? string.Empty : l.Name, k.PublishingDate, k.State, k.SuburbId })
                .GroupJoin(this.datacontext.Suburbs, p => p.SuburbId, q => q.SuburbId, (p, q) => new { p.Id, p.Title, p.TitleURL, p.Company, p.ShortDescription, p.URL, p.Category, p.SubCategory, p.State, p.PublishingDate, q })
                .SelectMany(n => n.q.DefaultIfEmpty(), (f, g) => new JobSimple { Id = f.Id, Title = f.Title, TitleURL = f.TitleURL, Company = f.Company, Description = f.ShortDescription, URL = f.URL, Category = f.Category, SubCategory = f.SubCategory, State = f.State, Suburb = g == null ? string.Empty : g.Name, StrPublishingDate = string.Format("{0:dd/MM/yyyy hh:mm tt}",f.PublishingDate), PublishingDate = f.PublishingDate })
                .OrderByDescending(d => d.PublishingDate).Select(s => s).ToList();

                List<JobSimple> collection = new List<JobSimple>();

                int count = 0;
                for (int i = 0; i < temps.Count(); i++)
                {
                    collection.Add(temps[i]);
                    count++;
                    if (count >= PageSize || i == temps.Count() - 1)
                    {
                        result.Jobs.Add(collection);
                        collection = new List<JobSimple>();
                        count = 0;
                    }
                }
            }
            result.NumberOfPages = NumberOfPages;
            result.SelectedPageNo = PageNo;
            if (PageNo > result.Pages.Count)
            {
                result.SelectedIndex = (PageNo - ((BlockNo - 1) * BlockSize) - 1);
            }
            else
            {
                result.SelectedIndex = PageNo - 1;
            }
            return result;
        }

        public SearchJobListResult Search(string UserId, string Keywords, int Category, int SubCategory, int State, int PageNo, int PageSize, int BlockSize)
        {
            SearchJobListResult result = new SearchJobListResult();
            PageSize = PageSize <= 0 ? 10 : PageSize;
            BlockSize = BlockSize <= 0 ? 10 : BlockSize;

            int Start = 0;
            int Stop = 0;
            int NumberOfPages = 0;
            int NumberOfBlocks = 0;
            int BlockNo = 0;
            int NumberOfRecords = 0;
            List<int> Ids = new List<int>() { };

            if (string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State == 0)
            {
                Ids = this.datacontext.Jobs.Where(w=> w.UserId == UserId).OrderBy(o => o.JobId).Select(s => s.JobId).ToList();
            }
            else if (string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State != 0)
            {
                Ids = this.datacontext.Jobs.Where(w => w.UserId == UserId && w.State == State).Select(s => s.JobId).ToList();
            }
            else if (string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State == 0)
            {
                Ids = this.datacontext.Jobs.Where(w => w.UserId == UserId && w.Category == Category).Select(s => s.JobId).ToList();
            }
            else if (string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State != 0)
            {
                Ids = this.datacontext.Jobs.Where(w => w.UserId == UserId && w.Category == Category && w.State == State).Select(s => s.JobId).ToList();
            }
            else if (!string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State == 0)
            {
                Ids = this.datacontext.Jobs.GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                    .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.UserId, z.a.JobId, z.a.Title, Description = z.a.ShortDescription, URL = z.a.Url, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                    .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                    .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new {h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                    .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new {a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                    .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new {a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                    .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                    .OrderByDescending(d => d.PublishingDate).Select(s => s.Id).ToList();
            }
            else if (!string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State != 0)
            {
                Ids = this.datacontext.Jobs.Where(w => w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                    .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.UserId, z.a.JobId, z.a.Title, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                    .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                    .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                    .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new {a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                    .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new {a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                    .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                    .OrderByDescending(d => d.PublishingDate).Select(s => s.Id).ToList();
            }
            else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State == 0)
            {
                Ids = this.datacontext.Jobs.Where(w => w.Category == Category).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                    .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new {z.a.UserId, z.a.JobId, z.a.Title, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                    .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                    .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new {h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                    .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new {a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                    .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new {a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                    .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                    .OrderByDescending(d => d.PublishingDate).Select(s => s.Id).ToList();
            }
            else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory != 0 && State == 0)
            {
                Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.SubCategory == SubCategory).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                    .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new {z.a.UserId, z.a.JobId, z.a.Title, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                    .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                    .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new {h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                    .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new {a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                    .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new {a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                    .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                    .OrderByDescending(d => d.PublishingDate).Select(s => s.Id).ToList();
            }
            else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State != 0)
            {
                Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                    .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.UserId, z.a.JobId, z.a.Title, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                    .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                    .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code,  h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                    .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new {a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                    .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new {a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                    .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                    .OrderByDescending(d => d.PublishingDate).Select(s => s.Id).ToList();
            }
            else //if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory != 0 && State != 0)
            {
                Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.SubCategory == SubCategory && w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                    .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new {z.a.UserId, z.a.JobId, z.a.Title, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                    .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                    .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new {h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                    .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new {a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                    .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new {a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                    .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                    .OrderByDescending(d => d.PublishingDate).Select(s => s.Id).ToList();
            }

            NumberOfRecords = Ids.Count;

            if (NumberOfRecords == 0)
            {
                NumberOfPages = 0;
                NumberOfBlocks = 0;
                PageNo = 0;
                BlockNo = 0;
            }
            else
            {
                if (NumberOfRecords <= PageSize)
                {
                    NumberOfPages = 1;
                    NumberOfBlocks = 1;
                    PageNo = 1;
                    BlockNo = 1;
                }
                else
                {
                    NumberOfPages = NumberOfRecords / PageSize;
                    if (NumberOfRecords % PageSize > 0)
                    {
                        NumberOfPages++;
                    }

                    NumberOfBlocks = NumberOfPages / BlockSize;
                    if (NumberOfPages % BlockSize > 0)
                    {
                        NumberOfBlocks++;
                    }

                    if (PageNo > NumberOfPages)
                    {
                        PageNo = NumberOfPages;
                    }

                    if (PageNo <= BlockSize)
                    {
                        BlockNo = 1;
                    }
                    else
                    {
                        BlockNo = (int)Math.Abs(Math.Floor((decimal)PageNo / (decimal)BlockSize));
                        if (PageNo % BlockSize > 0)
                        {
                            BlockNo++;
                        }
                    }
                }

                Start = ((BlockNo - 1) * BlockSize) + 1;
                Stop = BlockNo * BlockSize;

                if (Stop > NumberOfPages) Stop = NumberOfPages;

                for (int i = Start; i <= Stop; i++)
                {
                    result.Pages.Add(i);
                }

                if (NumberOfBlocks == 1)
                {
                    result.First = null;
                    result.Prev = null;
                    result.Next = null;
                    result.Last = null;
                }
                else
                {
                    if (BlockNo == NumberOfBlocks)
                    {
                        result.First = 1;
                        result.Prev = PageNo - 1;
                        result.Next = null;
                        result.Last = null;
                    }
                    else if (BlockNo == 1)
                    {
                        result.First = null;
                        result.Prev = null;
                        result.Next = PageNo + 1;
                        result.Last = NumberOfPages;
                    }
                    else
                    {
                        result.First = 1;
                        result.Prev = PageNo - 1;
                        result.Next = PageNo + 1;
                        result.Last = NumberOfPages;
                    }
                }

                int Skip = (Start - 1) * PageSize;
                int Take = (Stop - (Start - 1)) * PageSize;
                List<int> filteredIds = Ids.Skip(Skip).Take(Take).ToList();
                List<JobSimple> temps = this.datacontext.Jobs.Where(w => Ids.Contains(w.JobId)).Join(this.datacontext.JobCategories, a => a.Category, b => b.CategoryId, (a, b) => new { Id = a.JobId, Title = a.Title, TitleURL = a.TitleURL, Company = a.Company, ShortDescription = a.ShortDescription, URL = a.Url, Category = b.Name, PublishingDate = a.PublishedDate, SubCategoryId = a.SubCategory, StateId = a.State, SuburbId = a.Suburb })
                    .Join(this.datacontext.States, s => s.StateId, t => t.StateId, (s, t) => new { Id = s.Id, Title = s.Title, TitleURL = s.TitleURL,Company = s.Company, ShortDescription = s.ShortDescription, URL = s.URL, Category = s.Category, PublishingDate = s.PublishingDate, SubCategoryId = s.SubCategoryId, State = t.ShortName, SuburbId = s.SuburbId })
                    .GroupJoin(this.datacontext.JobSubCategories, z => z.SubCategoryId, y => y.JobSubCategoryId, (z, y) => new { Id = z.Id, Title = z.Title, TitleURL = z.TitleURL, Company = z.Company, ShortDescription = z.ShortDescription, URL = z.URL, Category = z.Category, PublishingDate = z.PublishingDate, State = z.State, SuburbId = z.SuburbId, y })
                    .SelectMany(m => m.y.DefaultIfEmpty(), (k, l) => new { Id = k.Id, Title = k.Title, TitleURL = k.TitleURL, Company = k.Company, ShortDescription = k.ShortDescription, URL = k.URL, Category = k.Category, SubCategory = l == null ? string.Empty : l.Name, PublishingDate = k.PublishingDate, State = k.State, SuburbId = k.SuburbId })
                    .GroupJoin(this.datacontext.Suburbs, p => p.SuburbId, q => q.SuburbId, (p, q) => new { Id = p.Id, Title = p.Title, TitleURL = p.TitleURL, Company = p.Company, ShortDescription = p.ShortDescription, URL = p.URL, Category = p.Category, SubCategory = p.SubCategory, State = p.State, PublishingDate = p.PublishingDate, q })
                    .SelectMany(n => n.q.DefaultIfEmpty(), (f, g) => new JobSimple { Id = f.Id, Title = f.Title, TitleURL = f.TitleURL, Company = f.Company, Description = f.ShortDescription, URL = f.URL, Category = f.Category, SubCategory = f.SubCategory, State = f.State, Suburb = g == null ? string.Empty : g.Name,StrPublishingDate = GetWordDay(f.PublishingDate), PublishingDate = f.PublishingDate })
                    .OrderByDescending(d => d.PublishingDate).ToList();

                List<JobSimple> collection = new List<JobSimple>();
                int count = 0;
                for (int i = 0; i < temps.Count(); i++)
                {
                    collection.Add(temps[i]);
                    count++;
                    if (count >= PageSize || i == temps.Count() - 1)
                    {
                        result.Jobs.Add(collection);
                        collection = new List<JobSimple>();
                        count = 0;
                    }
                }
            }
            result.NumberOfPages = NumberOfPages;
            result.SelectedPageNo = PageNo;
            if (PageNo > result.Pages.Count)
            {
                result.SelectedIndex = (PageNo - ((BlockNo - 1) * BlockSize) - 1);
            }
            else
            {
                result.SelectedIndex = PageNo - 1;
            }
            return result;
        }

        private string GetWordDay(DateTime model)
        {
            DateTime today = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0);
            DateTime comparable = new DateTime(model.Year,model.Month,model.Day,0,0,0);
            string result = String.Empty;
            if(comparable.CompareTo(today) >= 0)
            {
                result = "Today";
            }
            else
            {
                switch((comparable.Date - today.Date).TotalDays)
                {
                    case -1:
                        {
                            result = "Yesterday";
                        }
                        break;
                    case -2:
                        {
                            result = "2 days ago";
                        }
                        break;
                    case -3:
                        {
                            result = "3 days ago";                            
                        }
                        break;
                    default:
                        {
                            result = string.Format("{0:dd/MMM/yyyy}", comparable);
                        }
                        break;
                }
            }
            return result;
        }

        public SearchJobListResult Search2(string Keywords, int Category, int SubCategory, int State, int OrderBy, int PageNo, int PageSize, int BlockSize)
        {
            SearchJobListResult result = new SearchJobListResult();
            PageSize = PageSize <= 0 ? 10 : PageSize;
            BlockSize = BlockSize <= 0 ? 10 : BlockSize;

            int Start = 0;
            int Stop = 0;
            int NumberOfPages = 0;
            int NumberOfBlocks = 0;
            int BlockNo = 0;
            int NumberOfRecords = 0;
            List<int> Ids = new List<int>() { };

            switch (OrderBy)
            {
                case 0:  /* by publishing date */
                    {
                        if (string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.OrderByDescending(o => o.PublishedDate).Select(s => s.JobId).ToList<int>();
                        }
                        else if (string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.OrderByDescending(o => o.PublishedDate).Where(w => w.State == State).Select(s => s.JobId).ToList<int>();
                        }
                        else if (string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.OrderByDescending(o => o.PublishedDate).Where(w => w.Category == Category).Select(s => s.JobId).ToList<int>();
                        }
                        else if (string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.OrderByDescending(o => o.PublishedDate).Where(w => w.Category == Category && w.State == State).Select(s => s.JobId).ToList<int>();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { JobId = z.a.JobId, Title = z.a.Title, Description = z.a.ShortDescription, URL = z.a.Url, Company = z.a.Company, Code = z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, Title = h.a.Title, Company = h.a.Company, Code = h.a.Code, CategoryId = h.a.CategoryId, StateId = h.a.StateId, PublishingDate = h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = b.Name, StateId = a.StateId, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = a.Category, State = b.LongName, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                                .OrderByDescending(d => d.PublishingDate).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { JobId = z.a.JobId, Title = z.a.Title, Company = z.a.Company, Code = z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, Title = h.a.Title, Company = h.a.Company, Code = h.a.Code, CategoryId = h.a.CategoryId, StateId = h.a.StateId, PublishingDate = h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = b.Name, StateId = a.StateId, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = a.Category, State = b.LongName, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                                .OrderByDescending(d => d.PublishingDate).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { JobId = z.a.JobId, Title = z.a.Title, Company = z.a.Company, Code = z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, Title = h.a.Title, Company = h.a.Company, Code = h.a.Code, CategoryId = h.a.CategoryId, StateId = h.a.StateId, PublishingDate = h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = b.Name, StateId = a.StateId, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = a.Category, State = b.LongName, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                                .OrderByDescending(d => d.PublishingDate).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory != 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.SubCategory == SubCategory).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { JobId = z.a.JobId, Title = z.a.Title, Company = z.a.Company, Code = z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, Title = h.a.Title, Company = h.a.Company, Code = h.a.Code, CategoryId = h.a.CategoryId, StateId = h.a.StateId, PublishingDate = h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = b.Name, StateId = a.StateId, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = a.Category, State = b.LongName, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                                .OrderByDescending(d => d.PublishingDate).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { JobId = z.a.JobId, Title = z.a.Title, Company = z.a.Company, Code = z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, Title = h.a.Title, Company = h.a.Company, Code = h.a.Code, CategoryId = h.a.CategoryId, StateId = h.a.StateId, PublishingDate = h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = b.Name, StateId = a.StateId, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = a.Category, State = b.LongName, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                                .OrderByDescending(d => d.PublishingDate).Select(s => s.Id).ToList();
                        }
                        else //if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory != 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.SubCategory == SubCategory && w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { JobId = z.a.JobId, Title = z.a.Title, Company = z.a.Company, Code = z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, Title = h.a.Title, Company = h.a.Company, Code = h.a.Code, CategoryId = h.a.CategoryId, StateId = h.a.StateId, PublishingDate = h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = b.Name, StateId = a.StateId, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = a.Category, State = b.LongName, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                                .OrderByDescending(d => d.PublishingDate).Select(s => s.Id).ToList();
                        }
                    }
                    break;
                case 1: /* by Job Title */
                    {
                        if (string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.OrderBy(o => o.Title).Select(s => s.JobId).ToList<int>();
                        }
                        else if (string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.State == State).OrderBy(o => o.Title).Select(s => s.JobId).ToList<int>();
                        }
                        else if (string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category).OrderBy(o => o.Title).Select(s => s.JobId).ToList<int>(); 
                        }
                        else if (string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.State == State).OrderBy(o => o.Title).Select(s => s.JobId).ToList<int>(); 
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { JobId = z.a.JobId, Title = z.a.Title, Description = z.a.ShortDescription, URL = z.a.Url, Company = z.a.Company, Code = z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, Title = h.a.Title, Company = h.a.Company, Code = h.a.Code, CategoryId = h.a.CategoryId, StateId = h.a.StateId, PublishingDate = h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = b.Name, StateId = a.StateId, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = a.Category, State = b.LongName, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                                .OrderBy(d => d.Title).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { JobId = z.a.JobId, Title = z.a.Title, Company = z.a.Company, Code = z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, Title = h.a.Title, Company = h.a.Company, Code = h.a.Code, CategoryId = h.a.CategoryId, StateId = h.a.StateId, PublishingDate = h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = b.Name, StateId = a.StateId, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = a.Category, State = b.LongName, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                                .OrderBy(d => d.Title).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { JobId = z.a.JobId, Title = z.a.Title, Company = z.a.Company, Code = z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, Title = h.a.Title, Company = h.a.Company, Code = h.a.Code, CategoryId = h.a.CategoryId, StateId = h.a.StateId, PublishingDate = h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = b.Name, StateId = a.StateId, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = a.Category, State = b.LongName, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                                .OrderBy(d => d.Title).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory != 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.SubCategory == SubCategory).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { JobId = z.a.JobId, Title = z.a.Title, Company = z.a.Company, Code = z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, Title = h.a.Title, Company = h.a.Company, Code = h.a.Code, CategoryId = h.a.CategoryId, StateId = h.a.StateId, PublishingDate = h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = b.Name, StateId = a.StateId, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = a.Category, State = b.LongName, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                                .OrderBy(d => d.Title).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { JobId = z.a.JobId, Title = z.a.Title, Company = z.a.Company, Code = z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, Title = h.a.Title, Company = h.a.Company, Code = h.a.Code, CategoryId = h.a.CategoryId, StateId = h.a.StateId, PublishingDate = h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = b.Name, StateId = a.StateId, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = a.Category, State = b.LongName, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                                .OrderBy(d => d.Title).Select(s => s.Id).ToList();
                        }
                        else //if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory != 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.SubCategory == SubCategory && w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { JobId = z.a.JobId, Title = z.a.Title, Company = z.a.Company, Code = z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, Title = h.a.Title, Company = h.a.Company, Code = h.a.Code, CategoryId = h.a.CategoryId, StateId = h.a.StateId, PublishingDate = h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = b.Name, StateId = a.StateId, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = a.Category, State = b.LongName, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                                .OrderBy(d => d.Title).Select(s => s.Id).ToList();
                        }
                    }
                    break;
                case 2: /* By Category */
                    {
                        if (string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Join(this.datacontext.JobCategories, a => a.Category, b => b.CategoryId, (a, b) => new { JobId = a.JobId, CategoryName = b.Name })
                                .OrderBy(o => o.CategoryName).Select(s => s.JobId).ToList<int>();
                        }
                        else if (string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.State == State).Join(this.datacontext.JobCategories, a => a.Category, b => b.CategoryId, (a, b) => new { JobId = a.JobId, CategoryName = b.Name })
                                .OrderBy(o => o.CategoryName).Select(s => s.JobId).ToList<int>();
                        }
                        else if (string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category).Join(this.datacontext.JobCategories, a => a.Category, b => b.CategoryId, (a, b) => new { JobId = a.JobId, CategoryName = b.Name })
                               .OrderBy(o => o.CategoryName).Select(s => s.JobId).ToList<int>();
                        }
                        else if (string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.State == State && w.Category == Category)
                                .Join(this.datacontext.JobCategories, a => a.Category, b => b.CategoryId, (a, b) => new { JobId = a.JobId, CategoryName = b.Name })
                                .OrderBy(o => o.CategoryName).Select(s => s.JobId).ToList<int>();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { JobId = z.a.JobId, Title = z.a.Title, Description = z.a.ShortDescription, URL = z.a.Url, Company = z.a.Company, Code = z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, Title = h.a.Title, Company = h.a.Company, Code = h.a.Code, CategoryId = h.a.CategoryId, StateId = h.a.StateId, PublishingDate = h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = b.Name, StateId = a.StateId, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = a.Category, State = b.LongName, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                                .OrderBy(d => d.Category).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { JobId = z.a.JobId, Title = z.a.Title, Company = z.a.Company, Code = z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, Title = h.a.Title, Company = h.a.Company, Code = h.a.Code, CategoryId = h.a.CategoryId, StateId = h.a.StateId, PublishingDate = h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = b.Name, StateId = a.StateId, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = a.Category, State = b.LongName, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                                .OrderBy(d => d.Category).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { JobId = z.a.JobId, Title = z.a.Title, Company = z.a.Company, Code = z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, Title = h.a.Title, Company = h.a.Company, Code = h.a.Code, CategoryId = h.a.CategoryId, StateId = h.a.StateId, PublishingDate = h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = b.Name, StateId = a.StateId, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = a.Category, State = b.LongName, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                                .OrderBy(d => d.Category).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory != 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.SubCategory == SubCategory).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { JobId = z.a.JobId, Title = z.a.Title, Company = z.a.Company, Code = z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, Title = h.a.Title, Company = h.a.Company, Code = h.a.Code, CategoryId = h.a.CategoryId, StateId = h.a.StateId, PublishingDate = h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = b.Name, StateId = a.StateId, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = a.Category, State = b.LongName, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                                .OrderBy(d => d.Category).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { JobId = z.a.JobId, Title = z.a.Title, Company = z.a.Company, Code = z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, Title = h.a.Title, Company = h.a.Company, Code = h.a.Code, CategoryId = h.a.CategoryId, StateId = h.a.StateId, PublishingDate = h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = b.Name, StateId = a.StateId, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = a.Category, State = b.LongName, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                                .OrderBy(d => d.Category).Select(s => s.Id).ToList();
                        }
                        else //if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory != 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.SubCategory == SubCategory && w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { JobId = z.a.JobId, Title = z.a.Title, Company = z.a.Company, Code = z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, Title = h.a.Title, Company = h.a.Company, Code = h.a.Code, CategoryId = h.a.CategoryId, StateId = h.a.StateId, PublishingDate = h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = b.Name, StateId = a.StateId, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { Id = a.Id, Title = a.Title, Company = a.Company, Code = a.Code, Category = a.Category, State = b.LongName, PublishingDate = a.PublishingDate, TagName = a.TagName })
                                .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                                .OrderBy(d => d.Category).Select(s => s.Id).ToList();
                        }
                        break;
                    }
                default: /* By State */
                    {
                        if (string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Join(this.datacontext.States, a => a.State, b => b.StateId, (a, b) => new { JobId = a.JobId, StateName = b.ShortName })
                                .OrderBy(o => o.StateName).Select(s => s.JobId).ToList<int>();
                        }
                        else if (string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.State == State).Join(this.datacontext.States, a => a.State, b => b.StateId, (a, b) => new { JobId = a.JobId, StateName = b.ShortName })
                                .OrderBy(o => o.StateName).Select(s => s.JobId).ToList<int>();
                        }
                        else if (string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category).Join(this.datacontext.States, a => a.State, b => b.StateId, (a, b) => new { JobId = a.JobId, StateName = b.ShortName })
                               .OrderBy(o => o.StateName).Select(s => s.JobId).ToList<int>();
                        }
                        else if (string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.State == State && w.Category == Category)
                                .Join(this.datacontext.States, a => a.State, b => b.StateId, (a, b) => new { JobId = a.JobId, StateName = b.ShortName })
                                .OrderBy(o => o.StateName).Select(s => s.JobId).ToList<int>();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.JobId, z.a.Title, Description = z.a.ShortDescription, URL = z.a.Url, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                                .OrderBy(d => d.State).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.JobId, z.a.Title, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                                .OrderBy(d => d.State).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.JobId, z.a.Title, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                                .OrderBy(d => d.State).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory != 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.SubCategory == SubCategory).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.JobId, z.a.Title, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                                .OrderBy(d => d.State).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.JobId, z.a.Title, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                                .OrderBy(d => d.State).Select(s => s.Id).ToList();
                        }
                        else //if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory != 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.SubCategory == SubCategory && w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.JobId, z.a.Title, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords))
                                .OrderBy(d => d.State).Select(s => s.Id).ToList();
                        }
                        break;
                    }
            }

            NumberOfRecords = Ids.Count;

            if (NumberOfRecords == 0)
            {
                NumberOfPages = 0;
                NumberOfBlocks = 0;
                PageNo = 0;
                BlockNo = 0;
            }
            else
            {
                if (NumberOfRecords <= PageSize)
                {
                    NumberOfPages = 1;
                    NumberOfBlocks = 1;
                    PageNo = 1;
                    BlockNo = 1;
                }
                else
                {
                    NumberOfPages = NumberOfRecords / PageSize;
                    if (NumberOfRecords % PageSize > 0)
                    {
                        NumberOfPages++;
                    }

                    NumberOfBlocks = NumberOfPages / BlockSize;
                    if (NumberOfPages % BlockSize > 0)
                    {
                        NumberOfBlocks++;
                    }

                    if (PageNo > NumberOfPages)
                    {
                        PageNo = NumberOfPages;
                    }

                    if (PageNo <= BlockSize)
                    {
                        BlockNo = 1;
                    }
                    else
                    {
                        BlockNo = (int)Math.Abs(Math.Floor((decimal)PageNo / (decimal)BlockSize));
                        if (PageNo % BlockSize > 0)
                        {
                            BlockNo++;
                        }
                    }
                }

                Start = ((BlockNo - 1) * BlockSize) + 1;
                Stop = BlockNo * BlockSize;

                if (Stop > NumberOfPages) Stop = NumberOfPages;

                for (int i = Start; i <= Stop; i++)
                {
                    result.Pages.Add(i);
                }

                if (NumberOfBlocks == 1)
                {
                    result.First = null;
                    result.Prev = null;
                    result.Next = null;
                    result.Last = null;
                }
                else
                {
                    if (BlockNo == NumberOfBlocks)
                    {
                        result.First = 1;
                        result.Prev = PageNo - 1;
                        result.Next = null;
                        result.Last = null;
                    }
                    else if (BlockNo == 1)
                    {
                        result.First = null;
                        result.Prev = null;
                        result.Next = PageNo + 1;
                        result.Last = NumberOfPages;
                    }
                    else
                    {
                        result.First = 1;
                        result.Prev = PageNo - 1;
                        result.Next = PageNo + 1;
                        result.Last = NumberOfPages;
                    }
                }

                int Skip = (Start - 1) * PageSize;
                int Take = (Stop - (Start - 1)) * PageSize;
                List<int> filteredIds = Ids.Skip(Skip).Take(Take).ToList();
                List<JobSimple> temps = new List<JobSimple>() { };

                switch (OrderBy)
                {
                    case 0:  /* by publishing date */
                        {
                            temps = this.datacontext.Jobs.Where(w => filteredIds.Contains(w.JobId)).Join(this.datacontext.JobCategories, a => a.Category, b => b.CategoryId, (a, b) => new { Id = a.JobId, a.Title, a.TitleURL, a.Company, a.ShortDescription, URL = a.Url, Category = b.Name, PublishingDate = a.PublishedDate, SubCategoryId = a.SubCategory, StateId = a.State, SuburbId = a.Suburb, a.IsApproved, a.ClosingDate })
                            .Join(this.datacontext.States, s => s.StateId, t => t.StateId, (s, t) => new { s.Id, s.Title, s.TitleURL, s.Company, s.ShortDescription, s.URL, s.Category, s.PublishingDate, s.SubCategoryId, State = t.ShortName, s.SuburbId, s.IsApproved, s.ClosingDate })
                            .GroupJoin(this.datacontext.JobSubCategories, z => z.SubCategoryId, y => y.JobSubCategoryId, (z, y) => new { z.Id, z.Title, z.TitleURL, z.Company, z.ShortDescription, z.URL, z.Category, z.PublishingDate, z.State, z.SuburbId, z.IsApproved, z.ClosingDate, y })
                            .SelectMany(m => m.y.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.Company, k.ShortDescription, k.URL, k.Category, SubCategory = l == null ? string.Empty : l.Name, k.PublishingDate, k.State, k.SuburbId, k.IsApproved, k.ClosingDate })
                            .GroupJoin(this.datacontext.Suburbs, p => p.SuburbId, q => q.SuburbId, (p, q) => new { p.Id, p.Title, p.TitleURL, p.Company, p.ShortDescription, p.URL, p.Category, p.SubCategory, p.State, p.PublishingDate, p.IsApproved, p.ClosingDate, q })
                            .SelectMany(n => n.q.DefaultIfEmpty(), (f, g) => new JobSimple { Id = f.Id, Title = f.Title, TitleURL = f.TitleURL, Company = f.Company, Description = f.ShortDescription, URL = f.URL, Category = f.Category, SubCategory = f.SubCategory, State = f.State, Suburb = g == null ? string.Empty : g.Name, StrPublishingDate = string.Format("{0:dd/MM/yyyy hh:mm tt}", f.PublishingDate), PublishingDate = f.PublishingDate, IsApproved = f.IsApproved??false, IsExpired = (f.ClosingDate.CompareTo(DateTime.Today) < 0) })
                            .OrderByDescending(d => d.PublishingDate).ToList();
                        }
                        break;
                    case 1:  /* by Job Title */
                        {
                            temps = this.datacontext.Jobs.Where(w => filteredIds.Contains(w.JobId)).Join(this.datacontext.JobCategories, a => a.Category, b => b.CategoryId, (a, b) => new { Id = a.JobId, a.Title, a.TitleURL, a.Company, a.ShortDescription, URL = a.Url, Category = b.Name, PublishingDate = a.PublishedDate, SubCategoryId = a.SubCategory, StateId = a.State, SuburbId = a.Suburb, a.IsApproved, a.ClosingDate })
                            .Join(this.datacontext.States, s => s.StateId, t => t.StateId, (s, t) => new { s.Id, s.Title, s.TitleURL, s.Company, s.ShortDescription, s.URL, s.Category, s.PublishingDate, s.SubCategoryId, State = t.ShortName, s.SuburbId, s.IsApproved, s.ClosingDate })
                            .GroupJoin(this.datacontext.JobSubCategories, z => z.SubCategoryId, y => y.JobSubCategoryId, (z, y) => new { z.Id, z.Title, z.TitleURL, z.Company, z.ShortDescription, z.URL, z.Category, z.PublishingDate, z.State, z.SuburbId, z.IsApproved, z.ClosingDate, y })
                            .SelectMany(m => m.y.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.Company, k.ShortDescription, k.URL, k.Category, SubCategory = l == null ? string.Empty : l.Name, k.PublishingDate, k.State, k.SuburbId, k.IsApproved, k.ClosingDate })
                            .GroupJoin(this.datacontext.Suburbs, p => p.SuburbId, q => q.SuburbId, (p, q) => new { p.Id, p.Title, p.TitleURL, p.Company, p.ShortDescription, p.URL, p.Category, p.SubCategory, p.State, p.PublishingDate, p.IsApproved, p.ClosingDate, q })
                            .SelectMany(n => n.q.DefaultIfEmpty(), (f, g) => new JobSimple { Id = f.Id, Title = f.Title, TitleURL = f.TitleURL, Company = f.Company, Description = f.ShortDescription, URL = f.URL, Category = f.Category, SubCategory = f.SubCategory, State = f.State, Suburb = g == null ? string.Empty : g.Name, StrPublishingDate = string.Format("{0:dd/MM/yyyy hh:mm tt}", f.PublishingDate), PublishingDate = f.PublishingDate, IsApproved = f.IsApproved??false, IsExpired = (f.ClosingDate.CompareTo(DateTime.Today) < 0) })
                            .OrderBy(d => d.Title).ToList();
                        }
                        break;
                    case 2:  /* by Category */
                        {
                            temps = this.datacontext.Jobs.Where(w => filteredIds.Contains(w.JobId)).Join(this.datacontext.JobCategories, a => a.Category, b => b.CategoryId, (a, b) => new { Id = a.JobId, a.Title, a.TitleURL, a.Company, a.ShortDescription, URL = a.Url, Category = b.Name, PublishingDate = a.PublishedDate, SubCategoryId = a.SubCategory, StateId = a.State, SuburbId = a.Suburb, a.IsApproved, a.ClosingDate })
                            .Join(this.datacontext.States, s => s.StateId, t => t.StateId, (s, t) => new { s.Id, s.Title, s.TitleURL, s.Company, s.ShortDescription, s.URL, s.Category, s.PublishingDate, s.SubCategoryId, State = t.ShortName, s.SuburbId, s.IsApproved, s.ClosingDate })
                            .GroupJoin(this.datacontext.JobSubCategories, z => z.SubCategoryId, y => y.JobSubCategoryId, (z, y) => new { z.Id, z.Title, z.TitleURL, z.Company, z.ShortDescription, z.URL, z.Category, z.PublishingDate, z.State, z.SuburbId, z.IsApproved, z.ClosingDate, y })
                            .SelectMany(m => m.y.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.Company, k.ShortDescription, k.URL, k.Category, SubCategory = l == null ? string.Empty : l.Name, k.PublishingDate, k.State, k.SuburbId, k.IsApproved, k.ClosingDate })
                            .GroupJoin(this.datacontext.Suburbs, p => p.SuburbId, q => q.SuburbId, (p, q) => new { p.Id, p.Title, p.TitleURL, p.Company, p.ShortDescription, p.URL, p.Category, p.SubCategory, p.State, p.PublishingDate, p.IsApproved, p.ClosingDate, q })
                            .SelectMany(n => n.q.DefaultIfEmpty(), (f, g) => new JobSimple { Id = f.Id, Title = f.Title, TitleURL = f.TitleURL, Company = f.Company, Description = f.ShortDescription, URL = f.URL, Category = f.Category, SubCategory = f.SubCategory, State = f.State, Suburb = g == null ? string.Empty : g.Name, StrPublishingDate = string.Format("{0:dd/MM/yyyy hh:mm tt}", f.PublishingDate), PublishingDate = f.PublishingDate, IsApproved = f.IsApproved??false, IsExpired = (f.ClosingDate.CompareTo(DateTime.Today) < 0) })
                            .OrderBy(d => d.Category).ToList();
                        }
                        break;
                    default: /* by State */
                        {
                            temps = this.datacontext.Jobs.Where(w => filteredIds.Contains(w.JobId)).Join(this.datacontext.JobCategories, a => a.Category, b => b.CategoryId, (a, b) => new { Id = a.JobId, a.Title, a.TitleURL, a.Company, a.ShortDescription, URL = a.Url, Category = b.Name, PublishingDate = a.PublishedDate, SubCategoryId = a.SubCategory, StateId = a.State, SuburbId = a.Suburb, a.IsApproved, a.ClosingDate })
                            .Join(this.datacontext.States, s => s.StateId, t => t.StateId, (s, t) => new { s.Id, s.Title, s.TitleURL, s.Company, s.ShortDescription, s.URL, s.Category, s.PublishingDate, s.SubCategoryId, State = t.ShortName, s.SuburbId, s.IsApproved, s.ClosingDate })
                            .GroupJoin(this.datacontext.JobSubCategories, z => z.SubCategoryId, y => y.JobSubCategoryId, (z, y) => new { z.Id, z.Title, z.TitleURL, z.Company, z.ShortDescription, z.URL, z.Category, z.PublishingDate, z.State, z.SuburbId, z.IsApproved, z.ClosingDate, y })
                            .SelectMany(m => m.y.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.Company, k.ShortDescription, k.URL, k.Category, SubCategory = l == null ? string.Empty : l.Name, k.PublishingDate, k.State, k.SuburbId, k.IsApproved, k.ClosingDate })
                            .GroupJoin(this.datacontext.Suburbs, p => p.SuburbId, q => q.SuburbId, (p, q) => new { p.Id, p.Title, p.TitleURL, p.Company, p.ShortDescription, p.URL, p.Category, p.SubCategory, p.State, p.PublishingDate, p.IsApproved, p.ClosingDate, q })
                            .SelectMany(n => n.q.DefaultIfEmpty(), (f, g) => new JobSimple { Id = f.Id, Title = f.Title, TitleURL = f.TitleURL, Company = f.Company, Description = f.ShortDescription, URL = f.URL, Category = f.Category, SubCategory = f.SubCategory, State = f.State, Suburb = g == null ? string.Empty : g.Name, StrPublishingDate = string.Format("{0:dd/MM/yyyy hh:mm tt}", f.PublishingDate), PublishingDate = f.PublishingDate, IsApproved = f.IsApproved??false, IsExpired = (f.ClosingDate.CompareTo(DateTime.Today) < 0) })
                            .OrderBy(d => d.State).ToList();
                            break;
                        }
                }

                List<JobSimple> collection = new List<JobSimple>();
                int count = 0;
                for (int i = 0; i < temps.Count(); i++)
                {
                    collection.Add(temps[i]);
                    count++;
                    if (count >= PageSize || i == temps.Count() - 1)
                    {
                        result.Jobs.Add(collection);
                        collection = new List<JobSimple>();
                        count = 0;
                    }
                }
            }
            result.NumberOfPages = NumberOfPages;
            result.SelectedPageNo = PageNo;
            if (PageNo > result.Pages.Count)
            {
                result.SelectedIndex = (PageNo - ((BlockNo - 1) * BlockSize) - 1);
            }
            else
            {
                result.SelectedIndex = PageNo - 1;
            }
            return result;
        }

        public SearchJobListResult Search2(string UserId, string Keywords, int Category, int SubCategory, int State, int OrderBy, int PageNo, int PageSize, int BlockSize)
        {
            SearchJobListResult result = new SearchJobListResult();
            PageSize = PageSize <= 0 ? 10 : PageSize;
            BlockSize = BlockSize <= 0 ? 10 : BlockSize;

            int Start = 0;
            int Stop = 0;
            int NumberOfPages = 0;
            int NumberOfBlocks = 0;
            int BlockNo = 0;
            int NumberOfRecords = 0;
            List<int> Ids = new List<int>() { };

            switch (OrderBy)
            {
                case 0:  /* by publishing date */
                    {
                        if (string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.UserId == UserId).OrderByDescending(o => o.PublishedDate).Select(s => s.JobId).ToList<int>();
                        }
                        else if (string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.UserId == UserId).OrderByDescending(o => o.PublishedDate).Where(w => w.State == State).Select(s => s.JobId).ToList<int>();
                        }
                        else if (string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.UserId == UserId).OrderByDescending(o => o.PublishedDate).Where(w => w.Category == Category).Select(s => s.JobId).ToList<int>();
                        }
                        else if (string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.UserId == UserId).OrderByDescending(o => o.PublishedDate).Where(w => w.Category == Category && w.State == State).Select(s => s.JobId).ToList<int>();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new {z.a.UserId, z.a.JobId, z.a.Title, Description = z.a.ShortDescription, URL = z.a.Url, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new {h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new {a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new {a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                                .OrderByDescending(d => d.PublishingDate).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new {z.a.UserId, z.a.JobId, z.a.Title, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new {h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new {a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new {a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                                .OrderByDescending(d => d.PublishingDate).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new {z.a.UserId, z.a.JobId, z.a.Title, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new {h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new {a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new {a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                                .OrderByDescending(d => d.PublishingDate).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory != 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.SubCategory == SubCategory).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.UserId, z.a.JobId, z.a.Title, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new {a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new {a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                                .OrderByDescending(d => d.PublishingDate).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new {z.a.UserId, z.a.JobId, z.a.Title, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new {h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new {a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new {a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                                .OrderByDescending(d => d.PublishingDate).Select(s => s.Id).ToList();
                        }
                        else //if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory != 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.SubCategory == SubCategory && w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.UserId, z.a.JobId, z.a.Title, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                                .OrderByDescending(d => d.PublishingDate).Select(s => s.Id).ToList();
                        }
                    }
                    break;
                case 1: /* by Job Title */
                    {
                        if (string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.UserId == UserId).OrderBy(o => o.Title).Select(s => s.JobId).ToList<int>();
                        }
                        else if (string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.UserId == UserId && w.State == State).OrderBy(o => o.Title).Select(s => s.JobId).ToList<int>();
                        }
                        else if (string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.UserId == UserId && w.Category == Category).OrderBy(o => o.Title).Select(s => s.JobId).ToList<int>();
                        }
                        else if (string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.UserId == UserId && w.Category == Category && w.State == State).OrderBy(o => o.Title).Select(s => s.JobId).ToList<int>();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new {z.a.UserId, z.a.JobId, z.a.Title, Description = z.a.ShortDescription, URL = z.a.Url, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new {h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new {a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new {a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                                .OrderBy(d => d.Title).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.UserId, z.a.JobId, z.a.Title, Description = z.a.ShortDescription, URL = z.a.Url, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                                .OrderBy(d => d.Title).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.UserId, z.a.JobId, z.a.Title, Description = z.a.ShortDescription, URL = z.a.Url, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                                .OrderBy(d => d.Title).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory != 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.SubCategory == SubCategory).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.UserId, z.a.JobId, z.a.Title, Description = z.a.ShortDescription, URL = z.a.Url, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                                .OrderBy(d => d.Title).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.UserId, z.a.JobId, z.a.Title, Description = z.a.ShortDescription, URL = z.a.Url, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                                .OrderBy(d => d.Title).Select(s => s.Id).ToList();
                        }
                        else //if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory != 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.SubCategory == SubCategory && w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.UserId, z.a.JobId, z.a.Title, Description = z.a.ShortDescription, URL = z.a.Url, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                                .OrderBy(d => d.Title).Select(s => s.Id).ToList();
                        }
                    }
                    break;
                case 2: /* By Category */
                    {
                        if (string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w=> w.UserId == UserId).Join(this.datacontext.JobCategories, a => a.Category, b => b.CategoryId, (a, b) => new { JobId = a.JobId, CategoryName = b.Name })
                                .OrderBy(o => o.CategoryName).Select(s => s.JobId).ToList<int>();
                        }
                        else if (string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.UserId == UserId && w.State == State).Join(this.datacontext.JobCategories, a => a.Category, b => b.CategoryId, (a, b) => new { JobId = a.JobId, CategoryName = b.Name })
                                .OrderBy(o => o.CategoryName).Select(s => s.JobId).ToList<int>();
                        }
                        else if (string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.UserId == UserId && w.Category == Category).Join(this.datacontext.JobCategories, a => a.Category, b => b.CategoryId, (a, b) => new { JobId = a.JobId, CategoryName = b.Name })
                               .OrderBy(o => o.CategoryName).Select(s => s.JobId).ToList<int>();
                        }
                        else if (string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.UserId == UserId && w.State == State && w.Category == Category)
                                .Join(this.datacontext.JobCategories, a => a.Category, b => b.CategoryId, (a, b) => new { JobId = a.JobId, CategoryName = b.Name })
                                .OrderBy(o => o.CategoryName).Select(s => s.JobId).ToList<int>();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new {z.a.UserId, z.a.JobId, z.a.Title, Description = z.a.ShortDescription, URL = z.a.Url, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new {h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new {a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new {a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                                .OrderBy(d => d.Category).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.UserId, z.a.JobId, z.a.Title, Description = z.a.ShortDescription, URL = z.a.Url, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                                .OrderBy(d => d.Category).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.UserId, z.a.JobId, z.a.Title, Description = z.a.ShortDescription, URL = z.a.Url, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                                .OrderBy(d => d.Category).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory != 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.SubCategory == SubCategory).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.UserId, z.a.JobId, z.a.Title, Description = z.a.ShortDescription, URL = z.a.Url, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                                .OrderBy(d => d.Category).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.UserId, z.a.JobId, z.a.Title, Description = z.a.ShortDescription, URL = z.a.Url, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                                .OrderBy(d => d.Category).Select(s => s.Id).ToList();
                        }
                        else //if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory != 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.SubCategory == SubCategory && w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.UserId, z.a.JobId, z.a.Title, Description = z.a.ShortDescription, URL = z.a.Url, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                                .OrderBy(d => d.Category).Select(s => s.Id).ToList();
                        }
                        break;
                    }
                default: /* By State */
                    {
                        if (string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w=> w.UserId == UserId).Join(this.datacontext.States, a => a.State, b => b.StateId, (a, b) => new { JobId = a.JobId, StateName = b.ShortName })
                                .OrderBy(o => o.StateName).Select(s => s.JobId).ToList<int>();
                        }
                        else if (string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.UserId == UserId && w.State == State).Join(this.datacontext.States, a => a.State, b => b.StateId, (a, b) => new { JobId = a.JobId, StateName = b.ShortName })
                                .OrderBy(o => o.StateName).Select(s => s.JobId).ToList<int>();
                        }
                        else if (string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.UserId == UserId && w.Category == Category).Join(this.datacontext.States, a => a.State, b => b.StateId, (a, b) => new { JobId = a.JobId, StateName = b.ShortName })
                               .OrderBy(o => o.StateName).Select(s => s.JobId).ToList<int>();
                        }
                        else if (string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.UserId == UserId && w.State == State && w.Category == Category)
                                .Join(this.datacontext.States, a => a.State, b => b.StateId, (a, b) => new { JobId = a.JobId, StateName = b.ShortName })
                                .OrderBy(o => o.StateName).Select(s => s.JobId).ToList<int>();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.UserId, z.a.JobId, z.a.Title, Description = z.a.ShortDescription, URL = z.a.Url, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                                .OrderBy(d => d.State).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category == 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.UserId, z.a.JobId, z.a.Title, Description = z.a.ShortDescription, URL = z.a.Url, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                                .OrderBy(d => d.State).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.UserId, z.a.JobId, z.a.Title, Description = z.a.ShortDescription, URL = z.a.Url, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                                .OrderBy(d => d.State).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory != 0 && State == 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.SubCategory == SubCategory).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.UserId, z.a.JobId, z.a.Title, Description = z.a.ShortDescription, URL = z.a.Url, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                                .OrderBy(d => d.State).Select(s => s.Id).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory == 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.UserId, z.a.JobId, z.a.Title, Description = z.a.ShortDescription, URL = z.a.Url, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                                .OrderBy(d => d.State).Select(s => s.Id).ToList();
                        }
                        else //if (!string.IsNullOrEmpty(Keywords) && Category != 0 && SubCategory != 0 && State != 0)
                        {
                            Ids = this.datacontext.Jobs.Where(w => w.Category == Category && w.SubCategory == SubCategory && w.State == State).GroupJoin(this.datacontext.JobTags, a => a.JobId, b => b.JobId, (a, b) => new { a, b })
                                .SelectMany(m => m.b.DefaultIfEmpty(), (z, y) => new { z.a.UserId, z.a.JobId, z.a.Title, Description = z.a.ShortDescription, URL = z.a.Url, z.a.Company, z.a.Code, CategoryId = z.a.Category, StateId = z.a.State, PublishingDate = z.a.PublishedDate, TagId = (y == null ? 0 : y.TagId) })
                                .GroupJoin(this.datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                                .SelectMany(n => n.b.DefaultIfEmpty(), (h, j) => new { h.a.UserId, Id = h.a.JobId, h.a.Title, h.a.Company, h.a.Code, h.a.CategoryId, h.a.StateId, h.a.PublishingDate, TagName = (j == null ? string.Empty : j.Name) })
                                .Join(this.datacontext.JobCategories, a => a.CategoryId, b => b.CategoryId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, Category = b.Name, a.StateId, a.PublishingDate, a.TagName })
                                .Join(this.datacontext.States, a => a.StateId, b => b.StateId, (a, b) => new { a.UserId, a.Id, a.Title, a.Company, a.Code, a.Category, State = b.LongName, a.PublishingDate, a.TagName })
                                .Where(w => w.UserId == UserId && (w.Title.StartsWith(Keywords) || w.Company.StartsWith(Keywords) || w.Code.StartsWith(Keywords) || w.Category.StartsWith(Keywords) || w.State.StartsWith(Keywords) || w.TagName.StartsWith(Keywords)))
                                .OrderBy(d => d.State).Select(s => s.Id).ToList();
                        }
                        break;
                    }
            }

            NumberOfRecords = Ids.Count;

            if (NumberOfRecords == 0)
            {
                NumberOfPages = 0;
                NumberOfBlocks = 0;
                PageNo = 0;
                BlockNo = 0;
            }
            else
            {
                if (NumberOfRecords <= PageSize)
                {
                    NumberOfPages = 1;
                    NumberOfBlocks = 1;
                    PageNo = 1;
                    BlockNo = 1;
                }
                else
                {
                    NumberOfPages = NumberOfRecords / PageSize;
                    if (NumberOfRecords % PageSize > 0)
                    {
                        NumberOfPages++;
                    }

                    NumberOfBlocks = NumberOfPages / BlockSize;
                    if (NumberOfPages % BlockSize > 0)
                    {
                        NumberOfBlocks++;
                    }

                    if (PageNo > NumberOfPages)
                    {
                        PageNo = NumberOfPages;
                    }

                    if (PageNo <= BlockSize)
                    {
                        BlockNo = 1;
                    }
                    else
                    {
                        BlockNo = (int)Math.Abs(Math.Floor((decimal)PageNo / (decimal)BlockSize));
                        if (PageNo % BlockSize > 0)
                        {
                            BlockNo++;
                        }
                    }
                }

                Start = ((BlockNo - 1) * BlockSize) + 1;
                Stop = BlockNo * BlockSize;

                if (Stop > NumberOfPages) Stop = NumberOfPages;

                for (int i = Start; i <= Stop; i++)
                {
                    result.Pages.Add(i);
                }

                if (NumberOfBlocks == 1)
                {
                    result.First = null;
                    result.Prev = null;
                    result.Next = null;
                    result.Last = null;
                }
                else
                {
                    if (BlockNo == NumberOfBlocks)
                    {
                        result.First = 1;
                        result.Prev = PageNo - 1;
                        result.Next = null;
                        result.Last = null;
                    }
                    else if (BlockNo == 1)
                    {
                        result.First = null;
                        result.Prev = null;
                        result.Next = PageNo + 1;
                        result.Last = NumberOfPages;
                    }
                    else
                    {
                        result.First = 1;
                        result.Prev = PageNo - 1;
                        result.Next = PageNo + 1;
                        result.Last = NumberOfPages;
                    }
                }

                int Skip = (Start - 1) * PageSize;
                int Take = (Stop - (Start - 1)) * PageSize;
                List<int> filteredIds = Ids.Skip(Skip).Take(Take).ToList();
                List<JobSimple> temps = new List<JobSimple>() { };

                switch (OrderBy)
                {
                    case 0:  /* by publishing date */
                        {
                            temps = this.datacontext.Jobs.Where(w => filteredIds.Contains(w.JobId)).Join(this.datacontext.JobCategories, a => a.Category, b => b.CategoryId, (a, b) => new { Id = a.JobId, Title = a.Title, TitleURL = a.TitleURL, Company = a.Company, ShortDescription = a.ShortDescription, URL = a.Url, Category = b.Name, PublishingDate = a.PublishedDate, SubCategoryId = a.SubCategory, StateId = a.State, SuburbId = a.Suburb })
                            .Join(this.datacontext.States, s => s.StateId, t => t.StateId, (s, t) => new { s.Id, s.Title, s.TitleURL, s.Company, s.ShortDescription, s.URL, s.Category, s.PublishingDate, s.SubCategoryId, State = t.ShortName, s.SuburbId })
                            .GroupJoin(this.datacontext.JobSubCategories, z => z.SubCategoryId, y => y.JobSubCategoryId, (z, y) => new { z.Id, z.Title, z.TitleURL, z.Company, z.ShortDescription, z.URL, z.Category, z.PublishingDate, z.State, z.SuburbId, y })
                            .SelectMany(m => m.y.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.Company, k.ShortDescription, k.URL, k.Category, SubCategory = l == null ? string.Empty : l.Name, k.PublishingDate, k.State, k.SuburbId })
                            .GroupJoin(this.datacontext.Suburbs, p => p.SuburbId, q => q.SuburbId, (p, q) => new { p.Id, p.Title, p.TitleURL, p.Company, p.ShortDescription, p.URL, p.Category, p.SubCategory, p.State, p.PublishingDate, q })
                            .SelectMany(n => n.q.DefaultIfEmpty(), (f, g) => new JobSimple { Id = f.Id, Title = f.Title, TitleURL = f.TitleURL, Company = f.Company, Description = f.ShortDescription, URL = f.URL, Category = f.Category, SubCategory = f.SubCategory, State = f.State, Suburb = g == null ? string.Empty : g.Name, StrPublishingDate = string.Format("{0:yyyy/MM/dd hh:mm:tt}", f.PublishingDate), PublishingDate = f.PublishingDate })
                            .OrderByDescending(d => d.PublishingDate).ToList();
                        }
                        break;
                    case 1:  /* by Job Title */
                        {
                            temps = this.datacontext.Jobs.Where(w => filteredIds.Contains(w.JobId)).Join(this.datacontext.JobCategories, a => a.Category, b => b.CategoryId, (a, b) => new { Id = a.JobId, Title = a.Title, TitleURL = a.TitleURL, Company = a.Company, ShortDescription = a.ShortDescription, URL = a.Url, Category = b.Name, PublishingDate = a.PublishedDate, SubCategoryId = a.SubCategory, StateId = a.State, SuburbId = a.Suburb })
                            .Join(this.datacontext.States, s => s.StateId, t => t.StateId, (s, t) => new { s.Id, s.Title, s.TitleURL, s.Company, s.ShortDescription, s.URL, s.Category, s.PublishingDate, s.SubCategoryId, State = t.ShortName, s.SuburbId })
                            .GroupJoin(this.datacontext.JobSubCategories, z => z.SubCategoryId, y => y.JobSubCategoryId, (z, y) => new { z.Id, z.Title, z.TitleURL, z.Company, z.ShortDescription, z.URL, z.Category, z.PublishingDate, z.State, z.SuburbId, y })
                            .SelectMany(m => m.y.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.Company, k.ShortDescription, k.URL, k.Category, SubCategory = l == null ? string.Empty : l.Name, k.PublishingDate, k.State, k.SuburbId })
                            .GroupJoin(this.datacontext.Suburbs, p => p.SuburbId, q => q.SuburbId, (p, q) => new { p.Id, p.Title, p.TitleURL, p.Company, p.ShortDescription, p.URL, p.Category, p.SubCategory, p.State, p.PublishingDate, q })
                            .SelectMany(n => n.q.DefaultIfEmpty(), (f, g) => new JobSimple { Id = f.Id, Title = f.Title, TitleURL = f.TitleURL, Company = f.Company, Description = f.ShortDescription, URL = f.URL, Category = f.Category, SubCategory = f.SubCategory, State = f.State, Suburb = g == null ? string.Empty : g.Name, StrPublishingDate = string.Format("{0:yyyy/MM/dd hh:mm:tt}", f.PublishingDate), PublishingDate = f.PublishingDate })
                            .OrderBy(d => d.Title).ToList();
                        }
                        break;
                    case 2:  /* by Category */
                        {
                            temps = this.datacontext.Jobs.Where(w => filteredIds.Contains(w.JobId)).Join(this.datacontext.JobCategories, a => a.Category, b => b.CategoryId, (a, b) => new { Id = a.JobId, Title = a.Title, TitleURL = a.TitleURL, Company = a.Company, ShortDescription = a.ShortDescription, URL = a.Url, Category = b.Name, PublishingDate = a.PublishedDate, SubCategoryId = a.SubCategory, StateId = a.State, SuburbId = a.Suburb })
                            .Join(this.datacontext.States, s => s.StateId, t => t.StateId, (s, t) => new { s.Id, s.Title, s.TitleURL, s.Company, s.ShortDescription, s.URL, s.Category, s.PublishingDate, s.SubCategoryId, State = t.ShortName, s.SuburbId })
                            .GroupJoin(this.datacontext.JobSubCategories, z => z.SubCategoryId, y => y.JobSubCategoryId, (z, y) => new { z.Id, z.Title, z.TitleURL, z.Company, z.ShortDescription, z.URL, z.Category, z.PublishingDate, z.State, z.SuburbId, y })
                            .SelectMany(m => m.y.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.Company, k.ShortDescription, k.URL, k.Category, SubCategory = l == null ? string.Empty : l.Name, k.PublishingDate, k.State, k.SuburbId })
                            .GroupJoin(this.datacontext.Suburbs, p => p.SuburbId, q => q.SuburbId, (p, q) => new { p.Id, p.Title, p.TitleURL, p.Company, p.ShortDescription, p.URL, p.Category, p.SubCategory, p.State, p.PublishingDate, q })
                            .SelectMany(n => n.q.DefaultIfEmpty(), (f, g) => new JobSimple { Id = f.Id, Title = f.Title, TitleURL = f.TitleURL, Company = f.Company, Description = f.ShortDescription, URL = f.URL, Category = f.Category, SubCategory = f.SubCategory, State = f.State, Suburb = g == null ? string.Empty : g.Name, StrPublishingDate = string.Format("{0:yyyy/MM/dd hh:mm:tt}", f.PublishingDate), PublishingDate = f.PublishingDate })
                            .OrderBy(d => d.Category).ToList();
                        }
                        break;
                    default: /* by State */
                        {
                            temps = this.datacontext.Jobs.Where(w => filteredIds.Contains(w.JobId)).Join(this.datacontext.JobCategories, a => a.Category, b => b.CategoryId, (a, b) => new { Id = a.JobId, Title = a.Title, TitleURL = a.TitleURL, Company = a.Company, ShortDescription = a.ShortDescription, URL = a.Url, Category = b.Name, PublishingDate = a.PublishedDate, SubCategoryId = a.SubCategory, StateId = a.State, SuburbId = a.Suburb })
                            .Join(this.datacontext.States, s => s.StateId, t => t.StateId, (s, t) => new { s.Id, s.Title, s.TitleURL, s.Company, s.ShortDescription, s.URL, s.Category, s.PublishingDate, s.SubCategoryId, State = t.ShortName, s.SuburbId })
                            .GroupJoin(this.datacontext.JobSubCategories, z => z.SubCategoryId, y => y.JobSubCategoryId, (z, y) => new { z.Id, z.Title, z.TitleURL, z.Company, z.ShortDescription, z.URL, z.Category, z.PublishingDate, z.State, z.SuburbId, y })
                            .SelectMany(m => m.y.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.Company, k.ShortDescription, k.URL, k.Category, SubCategory = l == null ? string.Empty : l.Name, k.PublishingDate, k.State, k.SuburbId })
                            .GroupJoin(this.datacontext.Suburbs, p => p.SuburbId, q => q.SuburbId, (p, q) => new { p.Id, p.Title, p.TitleURL, p.Company, p.ShortDescription, p.URL, p.Category, p.SubCategory, p.State, p.PublishingDate, q })
                            .SelectMany(n => n.q.DefaultIfEmpty(), (f, g) => new JobSimple { Id = f.Id, Title = f.Title, TitleURL = f.TitleURL, Company = f.Company, Description = f.ShortDescription, URL = f.URL, Category = f.Category, SubCategory = f.SubCategory, State = f.State, Suburb = g == null ? string.Empty : g.Name, StrPublishingDate = string.Format("{0:yyyy/MM/dd hh:mm:tt}", f.PublishingDate), PublishingDate = f.PublishingDate })
                            .OrderBy(d => d.State).ToList();
                            break;
                        }
                }

                List<JobSimple> collection = new List<JobSimple>();
                int count = 0;
                for (int i = 0; i < temps.Count(); i++)
                {
                    collection.Add(temps[i]);
                    count++;
                    if (count >= PageSize || i == temps.Count() - 1)
                    {
                        result.Jobs.Add(collection);
                        collection = new List<JobSimple>();
                        count = 0;
                    }
                }
            }
            result.NumberOfPages = NumberOfPages;
            result.SelectedPageNo = PageNo;
            if (PageNo > result.Pages.Count)
            {
                result.SelectedIndex = (PageNo - ((BlockNo - 1) * BlockSize) - 1);
            }
            else
            {
                result.SelectedIndex = PageNo - 1;
            }
            return result;
        }
    }
}
