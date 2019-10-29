using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using LivingSuburb.Models;
using LivingSuburb.Database;

namespace LivingSuburb.Services
{
    public class SuburbService
    {
        private DataContext datacontext;
        public SuburbService(DataContext context)
        {
            datacontext = context;
        }

        public Suburb Read(int id)
        {
            return datacontext.Suburbs.Where(w => w.SuburbId == id).FirstOrDefault();
        }

        public bool Delete(int id)
        {
            bool status = false;
            Suburb suburb = datacontext.Suburbs.Where(w => w.SuburbId == id).FirstOrDefault();
            if(suburb != null)
            {
                datacontext.Suburbs.Remove(suburb);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public SearchSuburbsDetail GetDetail(string state, string suburb)
        {
            SearchSuburbsDetail result;
            var suburbs = datacontext.Suburbs.Where(w => w.NameURL == suburb);
            var states = datacontext.States.Where(w => w.ShortName == state);
            result = suburbs.Join(states, a => a.State.StateId, b => b.StateId, (a, b) => new SearchSuburbsDetail { Id = a.SuburbId,Name = a.Name, State = b.ShortName }).FirstOrDefault();
            return result;
        }

        public SearchSuburbListResult Search(int State,string StartWith,int PageNo,int PageSize,int BlockSize)
        {
            SearchSuburbListResult result = new SearchSuburbListResult();
            State = State <= 0 ? 1 : State;
            StartWith = String.IsNullOrEmpty(StartWith) ? "A" : StartWith;
            PageSize = PageSize <= 0 ? 10 : PageSize;
            BlockSize = BlockSize <= 0 ? 10 : BlockSize;

            int Start = 0;
            int Stop = 0;
            int NumberOfPages = 0;
            int NumberOfBlocks = 0;
            int BlockNo = 0;
            int NumberOfRecords = datacontext.Suburbs.OrderBy(o=> o.Name).Where(w => w.Name.StartsWith(StartWith) && w.StateId == State).Count();

            if(NumberOfRecords == 0)
            {
                NumberOfPages = 0;
                NumberOfBlocks = 0;
                PageNo = 0;
                BlockNo = 0;
            }
            else
            {
                if(NumberOfRecords <= PageSize)
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

                for(int i = Start; i <= Stop; i++)
                {
                    result.Pages.Add(i);
                }

                if(NumberOfBlocks == 1)
                {
                    result.First = null;
                    result.Prev = null;
                    result.Next = null;
                    result.Last = null;
                }
                else
                {
                    if(BlockNo == NumberOfBlocks)
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
                List<Suburb> temps = datacontext.Suburbs.OrderBy(o => o.Name).Where(w => w.Name.StartsWith(StartWith) && w.StateId == State).Skip(Skip).Take(Take).ToList();
                List<Suburb> collection = new List<Suburb>();
                int count = 0;
                for (int i=0;i<temps.Count();i++)
                {
                    collection.Add(temps[i]);
                    count++;
                    if (count >= PageSize || i==temps.Count()-1)
                    {
                        result.Suburbs.Add(collection);
                        collection = new List<Suburb>();
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

        public List<SearchSuburbsResult> Search(string keyword,int limit)
        {
            var suburbs = datacontext.Suburbs.Where(w => w.Name.StartsWith(keyword));
            var compose = datacontext.States.Join(suburbs, a => a.StateId, b => b.State.StateId, (a, b) => new SearchSuburbsResult { Id = b.SuburbId, Name = b.Name, NameURL = b.NameURL,State = a.ShortName});
            var result = compose.OrderBy(o => o.Name).ThenBy(t => t.State).Take(limit).ToList();
           return result;
        }

        public List<SearchSuburbsResult> Search(string keyword, int stateId,int limit)
        {
            var suburbs = datacontext.Suburbs.Where(w => w.Name.StartsWith(keyword));
            var compose = datacontext.States.Where(w=> w.StateId == stateId).Join(suburbs, a => a.StateId, b => b.State.StateId, (a, b) => new SearchSuburbsResult { Id = b.SuburbId, Name = b.Name, NameURL = b.NameURL, State = a.ShortName });
            var result = compose.OrderBy(o => o.Name).ThenBy(t => t.State).Take(limit).ToList();
            return result;
        }

        public bool Update(Suburb model)
        {
            bool status = false;
            if (datacontext.Suburbs.Where(w => w.SuburbId == model.SuburbId).Any())
            {
                datacontext.Suburbs.Update(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Add(AddSuburb model)
        {
            bool status = false;
            Suburb suburb = datacontext.Suburbs.Where(w => w.Name == model.name && w.State.StateId == model.stateid).FirstOrDefault();
            if(suburb==null)
            {
                suburb = new Suburb();
                suburb.StateId = model.stateid;
                suburb.Name = model.name;
                suburb.NameURL = model.nameurl;
                DateTime dt;
                if (DateTime.TryParse(model.established, out dt))
                {
                    suburb.Established = (DateTime)dt;
                }
                else
                {
                    suburb.Established = DateTime.MaxValue;
                }
                datacontext.Suburbs.Add(suburb);
                datacontext.SaveChanges();
                status = true;
            }

            return status;
        }

        public bool AddBulk(AddBulkSuburb model) /* please see https://www.yellowpages.com.au/nsw/localities-a1 */
        {
            bool status = false;
            string[] temps = model.names.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if(temps.Length > 0)
            {
                for(int i=0;i<temps.Length;i++)
                {
                    string[] timps = temps[i].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if(timps.Length > 1)
                    {
                        Suburb suburb = datacontext.Suburbs.Where(w => w.Name == timps[0] && w.State.StateId == model.stateid).FirstOrDefault();
                        if (suburb == null)
                        {
                            suburb = new Suburb()
                            {
                                StateId = model.stateid,
                                Name = timps[0],
                                NameURL = timps[0].Replace(" ", "-")
                            };
                            DateTime dt;
                            if (DateTime.TryParse(model.established, out dt))
                            {
                                suburb.Established = (DateTime)dt;
                            }
                            else
                            {
                                suburb.Established = DateTime.MaxValue;
                            }
                            datacontext.Suburbs.Add(suburb);
                            datacontext.SaveChanges();

                            for(int j=1;j<timps.Length;j++)
                            {
                                PostCode postcode = new PostCode();
                                postcode.SuburbId = suburb.SuburbId;
                                postcode.Code = int.Parse(timps[j]);
                                datacontext.PostCodes.Add(postcode);
                            }
                            datacontext.SaveChanges();
                            status = true;
                        }
                    }
                }
            }
            return status;
        }
    }
}
