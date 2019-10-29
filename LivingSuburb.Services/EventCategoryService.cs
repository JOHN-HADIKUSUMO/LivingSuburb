using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using LivingSuburb.Models;
using LivingSuburb.Database;

namespace LivingSuburb.Services
{
    public class EventCategoryService
    {
        private DataContext datacontext;
        public EventCategoryService(DataContext context)
        {
            datacontext = context;
        }

        public EventCategory Read(string nameURL)
        {
            return datacontext.EventCategories.Where(w => w.NameURL.Trim().ToLower() == nameURL.Trim().ToLower()).FirstOrDefault();
        }

        public EventCategory Read(int id)
        {
            return datacontext.EventCategories.Where(w => w.EventCategoryId == id).FirstOrDefault();
        }

        public bool Create(EventCategory model)
        {
            bool status = false;
            if (!datacontext.EventCategories.Where(w => w.Name.Trim().ToUpper() == model.Name.Trim().ToUpper()).Any())
            {
                datacontext.EventCategories.Add(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Delete(int id)
        {
            bool status = false;
            EventCategory eventCategory = datacontext.EventCategories.Where(w => w.EventCategoryId == id).FirstOrDefault();
            if (eventCategory != null)
            {
                datacontext.EventCategories.Remove(eventCategory);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Update(EventCategory model)
        {
            bool status = false;
            if (datacontext.EventCategories.Where(w => w.EventCategoryId == model.EventCategoryId).Any())
            {
                datacontext.EventCategories.Update(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public EventCategory Get(string nameurl)
        {
            return datacontext.EventCategories.Where(w => w.NameURL == nameurl).FirstOrDefault();
        }

        public List<EventCategory> GetAll()
        {
            return datacontext.EventCategories.OrderBy(o => o.Rank).ToList();
        }

        public JobCategoryListResult Search(string keywords,int Orderby, int PageNo, int PageSize, int BlockSize)
        {
            JobCategoryListResult result = new JobCategoryListResult();
            Orderby = Orderby <= 0 ? 0 : (Orderby > 2 ? 2: Orderby);
            PageSize = PageSize <= 0 ? 10 : PageSize;
            BlockSize = BlockSize <= 0 ? 10 : BlockSize;

            int Start = 0;
            int Stop = 0;
            int NumberOfPages = 0;
            int NumberOfBlocks = 0;
            int BlockNo = 0;
            int NumberOfRecords = 0;

            if (string.IsNullOrEmpty(keywords))
            {
                switch (Orderby)
                {
                    case 0: /* by Rank */
                        NumberOfRecords = datacontext.JobCategories.OrderBy(o => o.Rank).Count();
                        break;
                    case 1: /* by Name */
                        NumberOfRecords = datacontext.JobCategories.OrderBy(o => o.Name).Count();
                        break;
                    default: /* by Id */
                        NumberOfRecords = datacontext.JobCategories.OrderBy(o => o.CategoryId).Count();
                        break;
                }
            }               
            else
            {
                switch (Orderby)
                {
                    case 0: /* by Rank */
                        NumberOfRecords = datacontext.JobCategories.Where(w => w.Name.StartsWith(keywords)).OrderBy(o => o.Rank).Count();
                        break;
                    case 1: /* by Name */
                        NumberOfRecords = datacontext.JobCategories.Where(w => w.Name.StartsWith(keywords)).OrderBy(o => o.Name).Count();
                        break;
                    default: /* by Id */
                        NumberOfRecords = datacontext.JobCategories.Where(w => w.Name.StartsWith(keywords)).OrderBy(o => o.CategoryId).Count();
                        break;
                }
            }            

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
                List<JobCategorySimpleItem> temps;

                if (string.IsNullOrEmpty(keywords))
                {
                    switch (Orderby)
                    {
                        case 0: /* by Rank */
                            temps = datacontext.JobCategories.OrderBy(o => o.Rank).Select(s => new JobCategorySimpleItem(s.CategoryId, s.Name, s.Rank)).ToList();
                            break;
                        case 1: /* by Name */
                            temps = datacontext.JobCategories.OrderBy(o => o.Name).Select(s => new JobCategorySimpleItem(s.CategoryId, s.Name, s.Rank)).ToList();
                            break;
                        default: /* by Id */
                            temps = datacontext.JobCategories.OrderBy(o => o.CategoryId).Select(s => new JobCategorySimpleItem(s.CategoryId, s.Name, s.Rank)).ToList();
                            break;
                    }
                }
                else
                {
                    switch (Orderby)
                    {
                        case 0: /* by Rank */
                            temps = datacontext.JobCategories.Where(w => w.Name.StartsWith(keywords)).OrderBy(o => o.Rank).Select(s => new JobCategorySimpleItem(s.CategoryId, s.Name, s.Rank)).ToList();
                            break;
                        case 1: /* by Name */
                            temps = datacontext.JobCategories.Where(w => w.Name.StartsWith(keywords)).OrderBy(o => o.Name).Select(s => new JobCategorySimpleItem(s.CategoryId, s.Name, s.Rank)).ToList();
                            break;
                        default: /* by Id */
                            temps = datacontext.JobCategories.Where(w => w.Name.StartsWith(keywords)).OrderBy(o => o.CategoryId).Select(s => new JobCategorySimpleItem(s.CategoryId, s.Name, s.Rank)).ToList();
                            break;
                    }
                }

                List<JobCategorySimpleItem> collection = new List<JobCategorySimpleItem>();
                int count = 0;
                for (int i = 0; i < temps.Count(); i++)
                {
                    collection.Add(temps[i]);
                    count++;
                    if (count >= PageSize || i == temps.Count() - 1)
                    {
                        result.JobCategories.Add(collection);
                        collection = new List<JobCategorySimpleItem>();
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
