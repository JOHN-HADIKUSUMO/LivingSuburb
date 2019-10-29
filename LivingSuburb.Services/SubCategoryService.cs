using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using LivingSuburb.Models;
using LivingSuburb.Database;

namespace LivingSuburb.Services
{
    public class SubCategoryService
    {
        private DataContext datacontext;
        public SubCategoryService(DataContext context)
        {
            datacontext = context;
        }

        public bool Update(JobSubCategory model)
        {
            bool status = false;
            if (datacontext.JobSubCategories.Where(w => w.JobSubCategoryId == model.JobSubCategoryId).Any())
            {
                datacontext.JobSubCategories.Update(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Delete(int id)
        {
            bool status = false;
            JobSubCategory jobSubCategory = datacontext.JobSubCategories.Where(w => w.JobSubCategoryId == id).FirstOrDefault();
            if (jobSubCategory != null)
            {
                datacontext.JobSubCategories.Remove(jobSubCategory);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Create(JobSubCategory model)
        {
            bool status = false;
            if (!datacontext.JobSubCategories.Where(w => w.Name.Trim().ToUpper() == model.Name.Trim().ToUpper()).Any())
            {
                datacontext.JobSubCategories.Add(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public JobSubCategory Read(int id)
        {
            return datacontext.JobSubCategories.Where(w => w.JobSubCategoryId == id).FirstOrDefault();
        }

        public JobSubCategory Get(int categoryid, string nameurl)
        {
            return datacontext.JobSubCategories.Where(w => w.JobCategoryId == categoryid && w.NameURL == nameurl).FirstOrDefault();
        }

        public List<JobSubCategory> GetAll(int categoryid)
        {
            return datacontext.JobSubCategories.Where(w => w.JobCategoryId == categoryid).OrderBy(o => o.Rank).ToList();
        }

        public JobSubCategoryListResult Search(string keywords, int Category, int Orderby, int PageNo, int PageSize, int BlockSize)
        {
            JobSubCategoryListResult result = new JobSubCategoryListResult();
            Category = Category <= 0 ? 0 : Category;
            Orderby = Orderby <= 0 ? 0 : (Orderby > 2 ? 2 : Orderby);
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
                if(Category == 0) /* if category is not selected */
                {
                    switch (Orderby)
                    {
                        case 0: /* by Rank */
                            NumberOfRecords = datacontext.JobSubCategories.OrderBy(o => o.Rank).Count();
                            break;
                        case 1: /* by Name */
                            NumberOfRecords = datacontext.JobSubCategories.OrderBy(o => o.Name).Count();
                            break;
                        default: /* by Id */
                            NumberOfRecords = datacontext.JobSubCategories.OrderBy(o => o.JobSubCategoryId).Count();
                            break;
                    }
                }
                else/* if category is selected */
                {
                    switch (Orderby)
                    {
                        case 0: /* by Rank */
                            NumberOfRecords = datacontext.JobSubCategories.Where(w=> w.JobCategoryId == Category).OrderBy(o => o.Rank).Count();
                            break;
                        case 1: /* by Name */
                            NumberOfRecords = datacontext.JobSubCategories.Where(w => w.JobCategoryId == Category).OrderBy(o => o.Name).Count();
                            break;
                        default: /* by Id */
                            NumberOfRecords = datacontext.JobSubCategories.Where(w => w.JobCategoryId == Category).OrderBy(o => o.JobSubCategoryId).Count();
                            break;
                    }
                }
            }
            else /* if Keywords is provided */
            {
                if(Category == 0) /* if category is not selected */
                {
                    switch (Orderby)
                    {
                        case 0: /* by Rank */
                            NumberOfRecords = datacontext.JobSubCategories.Where(w => w.Name.StartsWith(keywords)).OrderBy(o => o.Rank).Count();
                            break;
                        case 1: /* by Name */
                            NumberOfRecords = datacontext.JobSubCategories.Where(w => w.Name.StartsWith(keywords)).OrderBy(o => o.Name).Count();
                            break;
                        default: /* by Id */
                            NumberOfRecords = datacontext.JobSubCategories.Where(w => w.Name.StartsWith(keywords)).OrderBy(o => o.JobSubCategoryId).Count();
                            break;
                    }
                }
                else /* if category is selected */
                {
                    switch (Orderby)
                    {
                        case 0: /* by Rank */
                            NumberOfRecords = datacontext.JobSubCategories.Where(w => w.Name.StartsWith(keywords) && w.JobCategoryId == Category).OrderBy(o => o.Rank).Count();
                            break;
                        case 1: /* by Name */
                            NumberOfRecords = datacontext.JobSubCategories.Where(w => w.Name.StartsWith(keywords) && w.JobCategoryId == Category).OrderBy(o => o.Name).Count();
                            break;
                        default: /* by Id */
                            NumberOfRecords = datacontext.JobSubCategories.Where(w => w.Name.StartsWith(keywords) && w.JobCategoryId == Category).OrderBy(o => o.JobSubCategoryId).Count();
                            break;
                    }
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
                List<JobSubCategorySimpleItem> temps;

                if (string.IsNullOrEmpty(keywords))
                {
                    if(Category == 0)
                    {
                        switch (Orderby)
                        {
                            case 0: /* by Rank */
                                temps = datacontext.JobSubCategories.OrderBy(o => o.Rank).Select(s => new JobSubCategorySimpleItem(s.JobSubCategoryId, s.Name, s.Rank)).ToList();
                                break;
                            case 1: /* by Name */
                                temps = datacontext.JobSubCategories.OrderBy(o => o.Name).Select(s => new JobSubCategorySimpleItem(s.JobSubCategoryId, s.Name, s.Rank)).ToList();
                                break;
                            default: /* by Id */
                                temps = datacontext.JobSubCategories.OrderBy(o => o.JobSubCategoryId).Select(s => new JobSubCategorySimpleItem(s.JobSubCategoryId, s.Name, s.Rank)).ToList();
                                break;
                        }
                    }
                    else /* if Category != 0 */
                    {
                        switch (Orderby)
                        {
                            case 0: /* by Rank */
                                temps = datacontext.JobSubCategories.Where(w=>w.JobCategoryId == Category).OrderBy(o => o.Rank).Select(s => new JobSubCategorySimpleItem(s.JobSubCategoryId, s.Name, s.Rank)).ToList();
                                break;
                            case 1: /* by Name */
                                temps = datacontext.JobSubCategories.Where(w => w.JobCategoryId == Category).OrderBy(o => o.Name).Select(s => new JobSubCategorySimpleItem(s.JobSubCategoryId, s.Name, s.Rank)).ToList();
                                break;
                            default: /* by Id */
                                temps = datacontext.JobSubCategories.Where(w => w.JobCategoryId == Category).OrderBy(o => o.JobSubCategoryId).Select(s => new JobSubCategorySimpleItem(s.JobSubCategoryId, s.Name, s.Rank)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    if(Category == 0)
                    {
                        switch (Orderby)
                        {
                            case 0: /* by Rank */
                                temps = datacontext.JobSubCategories.Where(w => w.Name.StartsWith(keywords)).OrderBy(o => o.Rank).Select(s => new JobSubCategorySimpleItem(s.JobSubCategoryId, s.Name, s.Rank)).ToList();
                                break;
                            case 1: /* by Name */
                                temps = datacontext.JobSubCategories.Where(w => w.Name.StartsWith(keywords)).OrderBy(o => o.Name).Select(s => new JobSubCategorySimpleItem(s.JobSubCategoryId, s.Name, s.Rank)).ToList();
                                break;
                            default: /* by Id */
                                temps = datacontext.JobSubCategories.Where(w => w.Name.StartsWith(keywords)).OrderBy(o => o.JobSubCategoryId).Select(s => new JobSubCategorySimpleItem(s.JobSubCategoryId, s.Name, s.Rank)).ToList();
                                break;
                        }
                    }
                    else
                    {
                        switch (Orderby)
                        {
                            case 0: /* by Rank */
                                temps = datacontext.JobSubCategories.Where(w => w.Name.StartsWith(keywords) && w.JobCategoryId == Category).OrderBy(o => o.Rank).Select(s => new JobSubCategorySimpleItem(s.JobSubCategoryId, s.Name, s.Rank)).ToList();
                                break;
                            case 1: /* by Name */
                                temps = datacontext.JobSubCategories.Where(w => w.Name.StartsWith(keywords) && w.JobCategoryId == Category).OrderBy(o => o.Name).Select(s => new JobSubCategorySimpleItem(s.JobSubCategoryId, s.Name, s.Rank)).ToList();
                                break;
                            default: /* by Id */
                                temps = datacontext.JobSubCategories.Where(w => w.Name.StartsWith(keywords) && w.JobCategoryId == Category).OrderBy(o => o.JobSubCategoryId).Select(s => new JobSubCategorySimpleItem(s.JobSubCategoryId, s.Name, s.Rank)).ToList();
                                break;
                        }
                    }
                }

                List<JobSubCategorySimpleItem> collection = new List<JobSubCategorySimpleItem>();
                int count = 0;
                for (int i = 0; i < temps.Count(); i++)
                {
                    collection.Add(temps[i]);
                    count++;
                    if (count >= PageSize || i == temps.Count() - 1)
                    {
                        result.JobSubCategories.Add(collection);
                        collection = new List<JobSubCategorySimpleItem>();
                        count = 0;
                    }
                }
            }
            result.NumberOfPages = NumberOfPages;
            result.SelectedPageNo = PageNo;
            return result;
        }
    }
}
