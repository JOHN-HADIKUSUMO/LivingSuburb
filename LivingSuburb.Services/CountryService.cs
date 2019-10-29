using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using LivingSuburb.Models;
using LivingSuburb.Database;

namespace LivingSuburb.Services
{
    public class CountryService
    {
        private DataContext datacontext;
        public CountryService(DataContext context)
        {
            datacontext = context;
        }

        public Country Read(int id)
        {
            return datacontext.Countries.Where(w => w.CountryId == id).FirstOrDefault();
        }

        public bool Create(Country model)
        {
            bool status = false;
            if (!datacontext.Countries.Where(w => w.Name.Trim().ToUpper() == model.Name.Trim().ToUpper()).Any())
            {
                datacontext.Countries.Add(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Delete(int id)
        {
            bool status = false;
            Country country = datacontext.Countries.Where(w => w.CountryId == id).FirstOrDefault();
            if (country != null)
            {
                datacontext.Countries.Remove(country);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Update(Country model)
        {
            bool status = false;
            if (datacontext.Countries.Where(w => w.CountryId == model.CountryId).Any())
            {
                datacontext.Countries.Update(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public List<Country> GetAll()
        {
            return datacontext.Countries.OrderBy(o => o.Name).ToList();
        }

        public List<SearchCountryResult> SearchByKeyword(string Keywords, int Take) /* Select by keywords only */
        {
            return datacontext.Countries.Where(w => w.Name.StartsWith(Keywords)).OrderBy(o => o.Name).Take(Take).Select(s => new SearchCountryResult { Id = s.CountryId, Name = s.Name }).ToList();
        }

        public CountryListResult Search(string keywords,int Orderby, int PageNo, int PageSize, int BlockSize)
        {
            CountryListResult result = new CountryListResult();
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
                    case 0: /* by Name */
                        NumberOfRecords = datacontext.Countries.OrderBy(o => o.Name).Count();
                        break;
                    case 1: /* by Code */
                        NumberOfRecords = datacontext.Countries.OrderBy(o => o.Code).Count();
                        break;
                    default: /* by Id */
                        NumberOfRecords = datacontext.Countries.OrderBy(o => o.CountryId).Count();
                        break;
                }
            }               
            else
            {
                switch (Orderby)
                {
                    case 0: /* by Name */
                        NumberOfRecords = datacontext.Countries.Where(w => w.Name.StartsWith(keywords)).OrderBy(o => o.Name).Count();
                        break;
                    case 1: /* by Code */
                        NumberOfRecords = datacontext.Countries.Where(w => w.Name.StartsWith(keywords)).OrderBy(o => o.Code).Count();
                        break;
                    default: /* by Id */
                        NumberOfRecords = datacontext.Countries.Where(w => w.Name.StartsWith(keywords)).OrderBy(o => o.CountryId).Count();
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
                List<CountrySimpleItem> temps;

                if (string.IsNullOrEmpty(keywords))
                {
                    switch (Orderby)
                    {
                        case 0: /* by Name */
                            temps = datacontext.Countries.OrderBy(o => o.Name).Skip(Skip).Take(Take).Select(s=> new CountrySimpleItem(s.CountryId,s.Name,s.Code)).ToList();
                            break;
                        case 1: /* by Code */
                            temps = datacontext.Countries.OrderBy(o => o.Code).Skip(Skip).Take(Take).Select(s => new CountrySimpleItem(s.CountryId, s.Name, s.Code)).ToList();
                            break;
                        default: /* by Id */
                            temps = datacontext.Countries.OrderBy(o => o.CountryId).Skip(Skip).Take(Take).Select(s => new CountrySimpleItem(s.CountryId, s.Name, s.Code)).ToList();
                            break;
                    }
                }
                else
                {
                    switch (Orderby)
                    {
                        case 0: /* by Name */
                            temps = datacontext.Countries.Where(w => w.Name.StartsWith(keywords)).OrderBy(o => o.Name).Skip(Skip).Take(Take).Select(s => new CountrySimpleItem(s.CountryId, s.Name, s.Code)).ToList();
                            break;
                        case 1: /* by Code */
                            temps = datacontext.Countries.Where(w => w.Name.StartsWith(keywords)).OrderBy(o => o.Code).Skip(Skip).Take(Take).Select(s => new CountrySimpleItem(s.CountryId, s.Name, s.Code)).ToList();
                            break;
                        default: /* by Id */
                            temps = datacontext.Countries.Where(w => w.Name.StartsWith(keywords)).OrderBy(o => o.CountryId).Skip(Skip).Take(Take).Select(s => new CountrySimpleItem(s.CountryId, s.Name, s.Code)).ToList();
                            break;
                    }
                }

                List<CountrySimpleItem> collection = new List<CountrySimpleItem>();
                int count = 0;
                for (int i = 0; i < temps.Count(); i++)
                {
                    collection.Add(temps[i]);
                    count++;
                    if (count >= PageSize || i == temps.Count() - 1)
                    {
                        result.Countries.Add(collection);
                        collection = new List<CountrySimpleItem>();
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
