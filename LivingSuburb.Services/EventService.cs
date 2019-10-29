using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LivingSuburb.Models;
using LivingSuburb.Database;

namespace LivingSuburb.Services
{
    public class EventService
    {
        private DataContext datacontext;
        public EventService(DataContext context)
        {
            datacontext = context;
        }

        public Event Read(int id)
        {
            return datacontext.Events.Where(w => w.EventId == id).FirstOrDefault();
        }

        public EventDetail ReadDetail(int id)
        {
            var tempEvent = datacontext.Events.Where(w => w.EventId == id).Join(datacontext.Countries, a => a.CountryId, b => b.CountryId, (a, b) => new { Id = a.EventId, a.Title, a.Description, a.Location, a.SourceUrl, Country = b.Name, Period = a.From.Equals(a.To) ? string.Format("{0:dddd, dd MMM yyyy}", a.From) : (string.Format("{0:dddd, dd MMM yyyy}", a.From) + " - " + string.Format("{0:dddd, dd MMM yyyy}", a.To)), a.EventCategoryId, a.EventTypeId })
                .Join(datacontext.EventCategories, c => c.EventCategoryId, d => d.EventCategoryId, (c, d) => new { c.Id, c.Title, c.Description, Category = d.Name, c.Country, c.Location, c.SourceUrl, c.Period, c.EventTypeId })
                .GroupJoin(datacontext.EventTypeCategories, f => f.EventTypeId, g => g.EventTypeId, (f, g) => new { f.Id, f.Title, f.Description, f.Category, f.Country, f.Location, f.SourceUrl, f.Period, g })
                .SelectMany(m => m.g.DefaultIfEmpty(), (h, g) => new { h.Id, h.Title, h.Description, h.Category, h.Country, h.Location, h.SourceUrl, h.Period, EventTypeId = g == null ? 0 : g.EventTypeId })
                .GroupJoin(datacontext.EventTypes, i => i.EventTypeId, j => j.EventTypeId, (i, j) => new { i.Id, i.Title, i.Description, i.Category, i.Country, i.Location, i.SourceUrl, i.Period, j })
                .SelectMany(m => m.j.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.Description, k.Category, Event = l.Name, k.Country, k.Location, k.SourceUrl, k.Period })
                .Distinct().Select(s => new { s.Id, s.Title, s.Description, s.Category, s.Event, s.Country, s.Location, s.SourceUrl, s.Period }).FirstOrDefault();

            if(tempEvent == null)
                return null;
            else
            {
                EventDetail result = new EventDetail(tempEvent.Id, tempEvent.Title, tempEvent.Description, tempEvent.Category, tempEvent.Event, tempEvent.Country, tempEvent.Location, tempEvent.SourceUrl, tempEvent.Period);
                result.Tags = datacontext.EventTags.Where(w => w.EventId == id).Join(datacontext.Tags, a => a.TagId, b => b.TagId, (a, b) => new { a, b })
                    .Select(s => s.b.Name).Distinct().OrderBy(o=> o).ToList();
                return result;
            }
        }


        public bool Create(Event model)
        {
            datacontext.Events.Add(model);
            datacontext.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            bool status = false;
            Event model = datacontext.Events.Where(w => w.EventId == id).FirstOrDefault();
            if (model != null)
            {
                datacontext.Events.Remove(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Update(Event model)
        {
            bool status = false;
            if (datacontext.Events.Where(w => w.EventId == model.EventId).Any())
            {
                datacontext.Events.Update(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public List<Event> GetAll()
        {
            return datacontext.Events.OrderBy(o => o.From).ToList();
        }

        public EventListResult Search(string keywords,int Orderby, int PageNo, int PageSize, int BlockSize)
        {
            EventListResult result = new EventListResult();
            Orderby = Orderby <= 0 ? 0 : (Orderby > 2 ? 2: Orderby);
            PageSize = PageSize <= 0 ? 10 : PageSize;
            BlockSize = BlockSize <= 0 ? 10 : BlockSize;

            int Start = 0;
            int Stop = 0;
            int NumberOfPages = 0;
            int NumberOfBlocks = 0;
            int BlockNo = 0;
            int NumberOfRecords = 0;
            List<int> Ids = new List<int>() { };

            if (string.IsNullOrEmpty(keywords))
            {
                NumberOfRecords = datacontext.Events.Count();
            }
            else
            {
                Ids = datacontext.Events.Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { a, b })
                    .Select(s => new { Id = s.a.EventId, Title = s.a.Title, CategoryName = s.b.Name,CountryId = s.a.CountryId })
                    .Join(datacontext.Countries,c=>c.CountryId,d=> d.CountryId,(c,d)=> new {c.Id, c.Title,c.CategoryName,CountryName = d.Name })
                    .GroupJoin(datacontext.EventTags, c => c.Id, d => d.EventId, (c, d) => new { c, d })
                    .SelectMany(e => e.d.DefaultIfEmpty(), (e, f) => new { Id = e.c.Id, e.c.Title, e.c.CategoryName,e.c.CountryName, TagId = f == null ? 0 : f.EventId })
                    .GroupJoin(datacontext.Tags, g => g.TagId, h => h.TagId, (g, h) => new { g, h })
                    .SelectMany(i => i.h.DefaultIfEmpty(), (i, j) => new { Id = i.g.Id, i.g.Title, i.g.CategoryName,i.g.CountryName, TagName = j == null ? string.Empty : j.Name })
                    .Where(w => w.Title.StartsWith(keywords) || w.CategoryName.StartsWith(keywords) || w.TagName.StartsWith(keywords) || w.CountryName.StartsWith(keywords))
                    .Select(s => s.Id).Distinct<int>().ToList();

                NumberOfRecords = Ids.Count();
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
                List<EventSimpleItem> temps;

                if (string.IsNullOrEmpty(keywords))
                {
                    switch (Orderby)
                    {
                        case 0: /* Period */
                            {
                                temps = datacontext.Events.Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { a, b })
                                    .OrderByDescending(o => o.a.From).Skip(Skip).Take(Take)
                                    .Select(s => new EventSimpleItem(s.a.EventId, s.a.Title, s.b.Name, string.Format("{0:dd/MM/yyyy}", s.a.From) + " - " + string.Format("{0:dd/MM/yyyy}", s.a.To))).ToList();
                            }
                            break;
                        case 1: /* Name */
                            {
                                temps = datacontext.Events.Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { a, b })
                                    .OrderBy(o => o.a.Title).Skip(Skip).Take(Take)
                                    .Select(s => new EventSimpleItem(s.a.EventId, s.a.Title, s.b.Name, string.Format("{0:dd/MM/yyyy}", s.a.From) + " - " + string.Format("{0:dd/MM/yyyy}", s.a.To))).ToList();
                            }
                            break;
                        case 2: /* Category */
                            {
                                temps = datacontext.Events.Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { a, b })
                                    .OrderBy(o => o.b.Name).Skip(Skip).Take(Take)
                                    .Select(s => new EventSimpleItem(s.a.EventId, s.a.Title, s.b.Name, string.Format("{0:dd/MM/yyyy}", s.a.From) + " - " + string.Format("{0:dd/MM/yyyy}", s.a.To))).ToList();
                            }
                            break;
                        default: /* Id */
                            {
                                temps = datacontext.Events.Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { a, b })
                                    .OrderBy(o => o.a.EventId).Skip(Skip).Take(Take)
                                    .Select(s => new EventSimpleItem(s.a.EventId, s.a.Title, s.b.Name, string.Format("{0:dd/MM/yyyy}", s.a.From) + " - " + string.Format("{0:dd/MM/yyyy}", s.a.To))).ToList();
                            }
                            break;
                    }
                }
                else /* if keywords is not null */
                {
                    switch (Orderby)
                    {
                        case 0: /* Period */
                            {
                                temps = datacontext.Events
                                    .Where(w=> Ids.Contains(w.EventId))
                                    .Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { a, b })
                                    .OrderByDescending(o => o.a.From).Skip(Skip).Take(Take)
                                    .Select(s => new EventSimpleItem(s.a.EventId, s.a.Title, s.b.Name, string.Format("{0:dd/MM/yyyy}", s.a.From) + " - " + string.Format("{0:dd/MM/yyyy}", s.a.To))).ToList();
                            }
                            break;
                        case 1: /* Name */
                            {
                                temps = datacontext.Events
                                    .Where(w => Ids.Contains(w.EventId))
                                    .Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { a, b })
                                    .OrderBy(o => o.a.Title).Skip(Skip).Take(Take)
                                    .Select(s => new EventSimpleItem(s.a.EventId, s.a.Title, s.b.Name, string.Format("{0:dd/MM/yyyy}", s.a.From) + " - " + string.Format("{0:dd/MM/yyyy}", s.a.To))).ToList();
                            }
                            break;
                        case 2: /* Category */
                            {
                                temps = datacontext.Events
                                    .Where(w => Ids.Contains(w.EventId))
                                    .Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { a, b })
                                    .OrderBy(o => o.b.Name).Skip(Skip).Take(Take)
                                    .Select(s => new EventSimpleItem(s.a.EventId, s.a.Title, s.b.Name, string.Format("{0:dd/MM/yyyy}", s.a.From) + " - " + string.Format("{0:dd/MM/yyyy}", s.a.To))).ToList();
                            }
                            break;
                        default: /* Id */
                            {
                                temps = datacontext.Events
                                    .Where(w => Ids.Contains(w.EventId))
                                    .Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { a, b })
                                    .OrderBy(o => o.a.EventId).Skip(Skip).Take(Take)
                                    .Select(s => new EventSimpleItem(s.a.EventId, s.a.Title, s.b.Name, string.Format("{0:dd/MM/yyyy}", s.a.From) + " - " + string.Format("{0:dd/MM/yyyy}", s.a.To))).ToList();
                            }
                            break;
                    }
                }

                List<EventSimpleItem> collection = new List<EventSimpleItem>();
                int count = 0;
                for (int i = 0; i < temps.Count(); i++)
                {
                    collection.Add(temps[i]);
                    count++;
                    if (count >= PageSize || i == temps.Count() - 1)
                    {
                        result.Events.Add(collection);
                        collection = new List<EventSimpleItem>();
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

        public EventListResult2 Search2(string keywords,int CategoryId,int EventTypeId, int Orderby, int PageNo, int PageSize, int BlockSize)
        {
            EventListResult2 result = new EventListResult2();
            Orderby = Orderby <= 0 ? 0 : (Orderby > 2 ? 2 : Orderby);
            PageSize = PageSize <= 0 ? 10 : PageSize;
            BlockSize = BlockSize <= 0 ? 10 : BlockSize;

            int Start = 0;
            int Stop = 0;
            int NumberOfPages = 0;
            int NumberOfBlocks = 0;
            int BlockNo = 0;
            int NumberOfRecords = 0;
            List<int> Ids = new List<int>() { };

            if (string.IsNullOrEmpty(keywords))
            {
                if(CategoryId == 0)
                {
                    NumberOfRecords = datacontext.Events.Count();
                }
                else /* if CategoryId > 0 */
                {
                    if(EventTypeId == 0)
                    {
                        NumberOfRecords = datacontext.Events.Where(w=> w.EventCategoryId == CategoryId).Count();
                    }
                    else /* EventTypeId > 0 */
                    {
                        NumberOfRecords = datacontext.Events.Where(w => w.EventCategoryId == CategoryId && w.EventTypeId == EventTypeId).Count();
                    }
                }                
            }
            else /* if keywords is not empty */
            {
                if (CategoryId == 0)
                {
                    Ids = datacontext.Events.Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { a, b })
                    .Select(s => new { Id = s.a.EventId, Title = s.a.Title, CategoryName = s.b.Name, CountryId = s.a.CountryId })
                    .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.CategoryName, CountryName = d.Name })
                    .GroupJoin(datacontext.EventTags, c => c.Id, d => d.EventId, (c, d) => new { c, d })
                    .SelectMany(e => e.d.DefaultIfEmpty(), (e, f) => new { Id = e.c.Id, e.c.Title, e.c.CategoryName, e.c.CountryName, TagId = f == null ? 0 : f.EventId })
                    .GroupJoin(datacontext.Tags, g => g.TagId, h => h.TagId, (g, h) => new { g, h })
                    .SelectMany(i => i.h.DefaultIfEmpty(), (i, j) => new { Id = i.g.Id, i.g.Title, i.g.CategoryName, i.g.CountryName, TagName = j == null ? string.Empty : j.Name })
                    .Where(w => w.Title.StartsWith(keywords) || w.CategoryName.StartsWith(keywords) || w.TagName.StartsWith(keywords) || w.CountryName.StartsWith(keywords))
                    .Select(s => s.Id).Distinct<int>().ToList();
                }
                else /* if CategoryId > 0 */
                {
                    if (EventTypeId == 0)
                    {
                        Ids = datacontext.Events.Where(w=>w.EventCategoryId == CategoryId).Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { a, b })
                        .Select(s => new { Id = s.a.EventId, Title = s.a.Title, CategoryName = s.b.Name, CountryId = s.a.CountryId })
                        .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.CategoryName, CountryName = d.Name })
                        .GroupJoin(datacontext.EventTags, c => c.Id, d => d.EventId, (c, d) => new { c, d })
                        .SelectMany(e => e.d.DefaultIfEmpty(), (e, f) => new { Id = e.c.Id, e.c.Title, e.c.CategoryName, e.c.CountryName, TagId = f == null ? 0 : f.EventId })
                        .GroupJoin(datacontext.Tags, g => g.TagId, h => h.TagId, (g, h) => new { g, h })
                        .SelectMany(i => i.h.DefaultIfEmpty(), (i, j) => new { Id = i.g.Id, i.g.Title, i.g.CategoryName, i.g.CountryName, TagName = j == null ? string.Empty : j.Name })
                        .Where(w => w.Title.StartsWith(keywords) || w.CategoryName.StartsWith(keywords) || w.TagName.StartsWith(keywords) || w.CountryName.StartsWith(keywords))
                        .Select(s => s.Id).Distinct<int>().ToList();
                    }
                    else /* EventTypeId > 0 */
                    {
                        Ids = datacontext.Events.Where(w => w.EventCategoryId == CategoryId && w.EventTypeId == EventTypeId).Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { a, b })
                        .Select(s => new { Id = s.a.EventId, Title = s.a.Title, CategoryName = s.b.Name, CountryId = s.a.CountryId })
                        .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.CategoryName, CountryName = d.Name })
                        .GroupJoin(datacontext.EventTags, c => c.Id, d => d.EventId, (c, d) => new { c, d })
                        .SelectMany(e => e.d.DefaultIfEmpty(), (e, f) => new { Id = e.c.Id, e.c.Title, e.c.CategoryName, e.c.CountryName, TagId = f == null ? 0 : f.EventId })
                        .GroupJoin(datacontext.Tags, g => g.TagId, h => h.TagId, (g, h) => new { g, h })
                        .SelectMany(i => i.h.DefaultIfEmpty(), (i, j) => new { Id = i.g.Id, i.g.Title, i.g.CategoryName, i.g.CountryName, TagName = j == null ? string.Empty : j.Name })
                        .Where(w => w.Title.StartsWith(keywords) || w.CategoryName.StartsWith(keywords) || w.TagName.StartsWith(keywords) || w.CountryName.StartsWith(keywords))
                        .Select(s => s.Id).Distinct<int>().ToList();
                    }
                }

                NumberOfRecords = Ids.Count();
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
                List<EventSimpleItem2> temps;

                if (string.IsNullOrEmpty(keywords))
                {
                    if (CategoryId == 0)
                    {
                        switch(Orderby)
                        {
                            case 0:/* Period */
                                {
                                    temps = datacontext.Events.Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { Id = a.EventId, a.Title, a.TitleURL, a.ShortDescription, Period = a.From.Equals(a.To) ? string.Format("{0:dd/MM/yyyy}", a.From) : string.Format("{0:dd/MM/yyyy}", a.From) + " - " + string.Format("{0:dd/MM/yyyy}", a.To), Category = b.Name, CategoryURL = b.NameURL, a.CountryId, a.EventTypeId, b })
                                    .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.TitleURL, c.ShortDescription, c.Category, c.CategoryURL, c.Period, Country = d.Name, c.EventTypeId })
                                    .GroupJoin(datacontext.EventTypeCategories, e => e.EventTypeId, f => f.EventTypeId, (e, f) => new { e.Id, e.Title, e.TitleURL, e.ShortDescription, e.Category, e.CategoryURL, e.Period, e.Country, f })
                                    .SelectMany(m => m.f.DefaultIfEmpty(), (g, h) => new { g.Id, g.Title, g.TitleURL, g.ShortDescription, g.Category, g.CategoryURL, g.Period, g.Country, h.EventTypeId })
                                    .GroupJoin(datacontext.EventTypes, i => i.EventTypeId, j => j.EventTypeId, (i, j) => new { i.Id, i.Title, i.TitleURL, i.ShortDescription, i.Category, i.CategoryURL, i.Period, i.Country, j })
                                    .SelectMany(m => m.j.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.ShortDescription, k.Category, k.CategoryURL, k.Period, k.Country, EventType = (l == null ? string.Empty : l.Name), EventTypeURL = (l == null ? string.Empty : l.NameURL) })
                                    .Distinct()
                                    .OrderByDescending(o => o.Period)
                                    .Skip(Skip)
                                    .Take(Take)
                                    .Select(s => new EventSimpleItem2() { Id = s.Id, Title = s.Title, TitleURL = s.TitleURL, ShortDescription = s.ShortDescription, Category = s.Category, CategoryURL = s.CategoryURL, EventType = s.EventType, EventTypeURL = s.EventTypeURL, Period = s.Period, Country = s.Country })
                                    .ToList();
                                }
                                break;
                            case 1: /* Title */
                                {
                                    temps = datacontext.Events.Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { Id = a.EventId, a.Title, a.TitleURL, a.ShortDescription, Period = a.From.Equals(a.To) ? string.Format("{0:dd/MM/yyyy}", a.From) : string.Format("{0:dd/MM/yyyy}", a.From) + " - " + string.Format("{0:dd/MM/yyyy}", a.To), Category = b.Name, CategoryURL = b.NameURL, a.CountryId, a.EventTypeId, b })
                                    .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.TitleURL, c.ShortDescription, c.Category, c.CategoryURL, c.Period, Country = d.Name, c.EventTypeId })
                                    .GroupJoin(datacontext.EventTypeCategories, e => e.EventTypeId, f => f.EventTypeId, (e, f) => new { e.Id, e.Title, e.TitleURL, e.ShortDescription, e.Category, e.CategoryURL, e.Period, e.Country, f })
                                    .SelectMany(m => m.f.DefaultIfEmpty(), (g, h) => new { g.Id, g.Title, g.TitleURL, g.ShortDescription, g.Category, g.CategoryURL, g.Period, g.Country, h.EventTypeId })
                                    .GroupJoin(datacontext.EventTypes, i => i.EventTypeId, j => j.EventTypeId, (i, j) => new { i.Id, i.Title, i.TitleURL, i.ShortDescription, i.Category, i.CategoryURL, i.Period, i.Country, j })
                                    .SelectMany(m => m.j.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.ShortDescription, k.Category, k.CategoryURL, k.Period, k.Country, EventType = (l == null ? string.Empty : l.Name), EventTypeURL = (l == null ? string.Empty : l.NameURL) })
                                    .Distinct()
                                    .OrderBy(o => o.Title)
                                    .Skip(Skip)
                                    .Take(Take)
                                    .Select(s => new EventSimpleItem2() { Id = s.Id, Title = s.Title, TitleURL = s.TitleURL, ShortDescription = s.ShortDescription, Category = s.Category, CategoryURL = s.CategoryURL, EventType = s.EventType, EventTypeURL = s.EventTypeURL, Period = s.Period, Country = s.Country })
                                    .ToList();
                                }
                                break;
                            case 2: /* Category */
                                {
                                    temps = datacontext.Events.Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { Id = a.EventId, a.Title, a.TitleURL, a.ShortDescription, Period = a.From.Equals(a.To) ? string.Format("{0:dd/MM/yyyy}", a.From) : string.Format("{0:dd/MM/yyyy}", a.From) + " - " + string.Format("{0:dd/MM/yyyy}", a.To), Category = b.Name, CategoryURL = b.NameURL, a.CountryId, a.EventTypeId, b })
                                    .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.TitleURL, c.ShortDescription, c.Category, c.CategoryURL, c.Period, Country = d.Name, c.EventTypeId })
                                    .GroupJoin(datacontext.EventTypeCategories, e => e.EventTypeId, f => f.EventTypeId, (e, f) => new { e.Id, e.Title, e.TitleURL, e.ShortDescription, e.Category, e.CategoryURL, e.Period, e.Country, f })
                                    .SelectMany(m => m.f.DefaultIfEmpty(), (g, h) => new { g.Id, g.Title, g.TitleURL, g.ShortDescription, g.Category, g.CategoryURL, g.Period, g.Country, h.EventTypeId })
                                    .GroupJoin(datacontext.EventTypes, i => i.EventTypeId, j => j.EventTypeId, (i, j) => new { i.Id, i.Title, i.TitleURL, i.ShortDescription, i.Category, i.CategoryURL, i.Period, i.Country, j })
                                    .SelectMany(m => m.j.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.ShortDescription, k.Category, k.CategoryURL, k.Period, k.Country, EventType = (l == null ? string.Empty : l.Name), EventTypeURL = (l == null ? string.Empty : l.NameURL) })
                                    .Distinct()
                                    .OrderBy(o => o.Category)
                                    .Skip(Skip)
                                    .Take(Take)
                                    .Select(s => new EventSimpleItem2() { Id = s.Id, Title = s.Title, TitleURL = s.TitleURL, ShortDescription = s.ShortDescription, Category = s.Category, CategoryURL = s.CategoryURL, EventType = s.EventType, EventTypeURL = s.EventTypeURL, Period = s.Period, Country = s.Country })
                                    .ToList();
                                }
                                break;
                            default: /* Id */
                                {
                                    temps = datacontext.Events.Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { Id = a.EventId, a.Title, a.TitleURL, a.ShortDescription, Period = a.From.Equals(a.To) ? string.Format("{0:dd/MM/yyyy}", a.From) : string.Format("{0:dd/MM/yyyy}", a.From) + " - " + string.Format("{0:dd/MM/yyyy}", a.To), Category = b.Name, CategoryURL = b.NameURL, a.CountryId, a.EventTypeId, b })
                                    .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.TitleURL, c.ShortDescription, c.Category, c.CategoryURL, c.Period, Country = d.Name, c.EventTypeId })
                                    .GroupJoin(datacontext.EventTypeCategories, e => e.EventTypeId, f => f.EventTypeId, (e, f) => new { e.Id, e.Title, e.TitleURL, e.ShortDescription, e.Category, e.CategoryURL, e.Period, e.Country, f })
                                    .SelectMany(m => m.f.DefaultIfEmpty(), (g, h) => new { g.Id, g.Title, g.TitleURL, g.ShortDescription, g.Category, g.CategoryURL, g.Period, g.Country, h.EventTypeId })
                                    .GroupJoin(datacontext.EventTypes, i => i.EventTypeId, j => j.EventTypeId, (i, j) => new { i.Id, i.Title, i.TitleURL, i.ShortDescription, i.Category, i.CategoryURL, i.Period, i.Country, j })
                                    .SelectMany(m => m.j.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.ShortDescription, k.Category, k.CategoryURL, k.Period, k.Country, EventType = (l == null ? string.Empty : l.Name), EventTypeURL = (l == null ? string.Empty : l.NameURL) })
                                    .Distinct()
                                    .OrderBy(o => o.Id)
                                    .Skip(Skip)
                                    .Take(Take)
                                    .Select(s => new EventSimpleItem2() { Id = s.Id, Title = s.Title, TitleURL = s.TitleURL, ShortDescription = s.ShortDescription, Category = s.Category, CategoryURL = s.CategoryURL, EventType = s.EventType, EventTypeURL = s.EventTypeURL, Period = s.Period, Country = s.Country })
                                    .ToList();
                                }
                                break;
                        }

                    }
                    else /* if CategoryId > 0 */
                    {
                        if (EventTypeId == 0)
                        {
                            switch(Orderby)
                            {
                                case 0:/* Period */
                                    {
                                        temps = datacontext.Events.Where(w => w.EventCategoryId == CategoryId).Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { Id = a.EventId, a.Title, a.TitleURL, a.ShortDescription, Period = a.From.Equals(a.To) ? string.Format("{0:dd/MM/yyyy}", a.From) : string.Format("{0:dd/MM/yyyy}", a.From) + " - " + string.Format("{0:dd/MM/yyyy}", a.To), Category = b.Name, CategoryURL = b.NameURL, a.CountryId, a.EventTypeId, b })
                                        .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.TitleURL, c.ShortDescription, c.Category, c.CategoryURL, c.Period, Country = d.Name, c.EventTypeId })
                                        .GroupJoin(datacontext.EventTypeCategories, e => e.EventTypeId, f => f.EventTypeId, (e, f) => new { e.Id, e.Title, e.TitleURL, e.ShortDescription, e.Category, e.CategoryURL, e.Period, e.Country, f })
                                        .SelectMany(m => m.f.DefaultIfEmpty(), (g, h) => new { g.Id, g.Title, g.TitleURL, g.ShortDescription, g.Category, g.CategoryURL, g.Period, g.Country, h.EventTypeId })
                                        .GroupJoin(datacontext.EventTypes, i => i.EventTypeId, j => j.EventTypeId, (i, j) => new { i.Id, i.Title, i.TitleURL, i.ShortDescription, i.Category, i.CategoryURL, i.Period, i.Country, j })
                                        .SelectMany(m => m.j.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.ShortDescription, k.Category, k.CategoryURL, k.Period, k.Country, EventType = l.Name, EventTypeURL = l.NameURL })
                                        .Distinct()
                                        .OrderByDescending(o => o.Period)
                                        .Skip(Skip)
                                        .Take(Take)
                                        .Select(s => new EventSimpleItem2() { Id = s.Id, Title = s.Title, TitleURL = s.TitleURL, ShortDescription = s.ShortDescription, Category = s.Category, CategoryURL = s.CategoryURL, EventType = s.EventType, EventTypeURL = s.EventTypeURL, Period = s.Period, Country = s.Country })
                                        .ToList();
                                    }
                                    break;
                                case 1: /* Title */
                                    {
                                        temps = datacontext.Events.Where(w => w.EventCategoryId == CategoryId).Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { Id = a.EventId, a.Title, a.TitleURL, a.ShortDescription, Period = a.From.Equals(a.To) ? string.Format("{0:dd/MM/yyyy}", a.From) : string.Format("{0:dd/MM/yyyy}", a.From) + " - " + string.Format("{0:dd/MM/yyyy}", a.To), Category = b.Name, CategoryURL = b.NameURL, a.CountryId, a.EventTypeId, b })
                                        .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.TitleURL, c.ShortDescription, c.Category, c.CategoryURL, c.Period, Country = d.Name, c.EventTypeId })
                                        .GroupJoin(datacontext.EventTypeCategories, e => e.EventTypeId, f => f.EventTypeId, (e, f) => new { e.Id, e.Title, e.TitleURL, e.ShortDescription, e.Category, e.CategoryURL, e.Period, e.Country, f })
                                        .SelectMany(m => m.f.DefaultIfEmpty(), (g, h) => new { g.Id, g.Title, g.TitleURL, g.ShortDescription, g.Category, g.CategoryURL, g.Period, g.Country, h.EventTypeId })
                                        .GroupJoin(datacontext.EventTypes, i => i.EventTypeId, j => j.EventTypeId, (i, j) => new { i.Id, i.Title, i.TitleURL, i.ShortDescription, i.Category, i.CategoryURL, i.Period, i.Country, j })
                                        .SelectMany(m => m.j.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.ShortDescription, k.Category, k.CategoryURL, k.Period, k.Country, EventType = l.Name, EventTypeURL = l.NameURL })
                                        .Distinct()
                                        .OrderBy(o => o.Title)
                                        .Skip(Skip)
                                        .Take(Take)
                                        .Select(s => new EventSimpleItem2() { Id = s.Id, Title = s.Title, TitleURL = s.TitleURL, ShortDescription = s.ShortDescription, Category = s.Category, CategoryURL = s.CategoryURL, EventType = s.EventType, EventTypeURL = s.EventTypeURL, Period = s.Period, Country = s.Country })
                                        .ToList();
                                    }
                                    break;
                                case 2: /* Category */
                                    {
                                        temps = datacontext.Events.Where(w => w.EventCategoryId == CategoryId).Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { Id = a.EventId, a.Title, a.TitleURL, a.ShortDescription, Period = a.From.Equals(a.To) ? string.Format("{0:dd/MM/yyyy}", a.From) : string.Format("{0:dd/MM/yyyy}", a.From) + " - " + string.Format("{0:dd/MM/yyyy}", a.To), Category = b.Name, CategoryURL = b.NameURL, a.CountryId, a.EventTypeId, b })
                                        .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.TitleURL, c.ShortDescription, c.Category, c.CategoryURL, c.Period, Country = d.Name, c.EventTypeId })
                                        .GroupJoin(datacontext.EventTypeCategories, e => e.EventTypeId, f => f.EventTypeId, (e, f) => new { e.Id, e.Title, e.TitleURL, e.ShortDescription, e.Category, e.CategoryURL, e.Period, e.Country, f })
                                        .SelectMany(m => m.f.DefaultIfEmpty(), (g, h) => new { g.Id, g.Title, g.TitleURL, g.ShortDescription, g.Category, g.CategoryURL, g.Period, g.Country, h.EventTypeId })
                                        .GroupJoin(datacontext.EventTypes, i => i.EventTypeId, j => j.EventTypeId, (i, j) => new { i.Id, i.Title, i.TitleURL, i.ShortDescription, i.Category, i.CategoryURL, i.Period, i.Country, j })
                                        .SelectMany(m => m.j.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.ShortDescription, k.Category, k.CategoryURL, k.Period, k.Country, EventType = l.Name, EventTypeURL = l.NameURL })
                                        .Distinct()
                                        .OrderBy(o => o.Category)
                                        .Skip(Skip)
                                        .Take(Take)
                                        .Select(s => new EventSimpleItem2() { Id = s.Id, Title = s.Title, TitleURL = s.TitleURL, ShortDescription = s.ShortDescription, Category = s.Category, CategoryURL = s.CategoryURL, EventType = s.EventType, EventTypeURL = s.EventTypeURL, Period = s.Period, Country = s.Country })
                                        .ToList();
                                    }
                                    break;
                                default: /* Id */
                                    {
                                        temps = datacontext.Events.Where(w => w.EventCategoryId == CategoryId).Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { Id = a.EventId, a.Title, a.TitleURL, a.ShortDescription, Period = a.From.Equals(a.To) ? string.Format("{0:dd/MM/yyyy}", a.From) : string.Format("{0:dd/MM/yyyy}", a.From) + " - " + string.Format("{0:dd/MM/yyyy}", a.To), Category = b.Name, CategoryURL = b.NameURL, a.CountryId, a.EventTypeId, b })
                                        .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.TitleURL, c.ShortDescription, c.Category, c.CategoryURL, c.Period, Country = d.Name, c.EventTypeId })
                                        .GroupJoin(datacontext.EventTypeCategories, e => e.EventTypeId, f => f.EventTypeId, (e, f) => new { e.Id, e.Title, e.TitleURL, e.ShortDescription, e.Category, e.CategoryURL, e.Period, e.Country, f })
                                        .SelectMany(m => m.f.DefaultIfEmpty(), (g, h) => new { g.Id, g.Title, g.TitleURL, g.ShortDescription, g.Category, g.CategoryURL, g.Period, g.Country, h.EventTypeId })
                                        .GroupJoin(datacontext.EventTypes, i => i.EventTypeId, j => j.EventTypeId, (i, j) => new { i.Id, i.Title, i.TitleURL, i.ShortDescription, i.Category, i.CategoryURL, i.Period, i.Country, j })
                                        .SelectMany(m => m.j.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.ShortDescription, k.Category, k.CategoryURL, k.Period, k.Country, EventType = l.Name, EventTypeURL = l.NameURL })
                                        .Distinct()
                                        .OrderBy(o => o.Id)
                                        .Skip(Skip)
                                        .Take(Take)
                                        .Select(s => new EventSimpleItem2() { Id = s.Id, Title = s.Title, TitleURL = s.TitleURL, ShortDescription = s.ShortDescription, Category = s.Category, CategoryURL = s.CategoryURL, EventType = s.EventType, EventTypeURL = s.EventTypeURL, Period = s.Period, Country = s.Country })
                                        .ToList();
                                    }
                                    break;
                            }

                        }
                        else /* EventTypeId > 0 */
                        {
                            switch (Orderby)
                            {
                                case 0:/* Period */
                                    {
                                        temps = datacontext.Events.Where(w => w.EventCategoryId == CategoryId && w.EventTypeId == EventTypeId).Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { Id = a.EventId, a.Title, a.TitleURL, a.ShortDescription, Period = a.From.Equals(a.To) ? string.Format("{0:dd/MM/yyyy}", a.From) : string.Format("{0:dd/MM/yyyy}", a.From) + " - " + string.Format("{0:dd/MM/yyyy}", a.To), Category = b.Name, CategoryURL = b.NameURL, a.CountryId, a.EventTypeId, b })
                                        .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.TitleURL, c.ShortDescription, c.Category, c.CategoryURL, c.Period, Country = d.Name, c.EventTypeId })
                                        .GroupJoin(datacontext.EventTypeCategories, e => e.EventTypeId, f => f.EventTypeId, (e, f) => new { e.Id, e.Title, e.TitleURL, e.ShortDescription, e.Category, e.CategoryURL, e.Period, e.Country, f })
                                        .SelectMany(m => m.f.DefaultIfEmpty(), (g, h) => new { g.Id, g.Title, g.TitleURL, g.ShortDescription, g.Category, g.CategoryURL, g.Period, g.Country, h.EventTypeId })
                                        .GroupJoin(datacontext.EventTypes, i => i.EventTypeId, j => j.EventTypeId, (i, j) => new { i.Id, i.Title, i.TitleURL, i.ShortDescription, i.Category, i.CategoryURL, i.Period, i.Country, j })
                                        .SelectMany(m => m.j.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.ShortDescription, k.Category, k.CategoryURL, k.Period, k.Country, EventType = l.Name, EventTypeURL = l.NameURL })
                                        .Distinct()
                                        .OrderByDescending(o => o.Period)
                                        .Skip(Skip)
                                        .Take(Take)
                                        .Select(s => new EventSimpleItem2() { Id = s.Id, Title = s.Title, TitleURL = s.TitleURL, ShortDescription = s.ShortDescription, Category = s.Category, CategoryURL = s.CategoryURL, EventType = s.EventType, EventTypeURL = s.EventTypeURL, Period = s.Period, Country = s.Country })
                                        .ToList();
                                    }
                                    break;
                                case 1: /* Title */
                                    {
                                        temps = datacontext.Events.Where(w => w.EventCategoryId == CategoryId && w.EventTypeId == EventTypeId).Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { Id = a.EventId, a.Title, a.TitleURL, a.ShortDescription, Period = a.From.Equals(a.To) ? string.Format("{0:dd/MM/yyyy}", a.From) : string.Format("{0:dd/MM/yyyy}", a.From) + " - " + string.Format("{0:dd/MM/yyyy}", a.To), Category = b.Name, CategoryURL = b.NameURL, a.CountryId, a.EventTypeId, b })
                                        .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.TitleURL, c.ShortDescription, c.Category, c.CategoryURL, c.Period, Country = d.Name, c.EventTypeId })
                                        .GroupJoin(datacontext.EventTypeCategories, e => e.EventTypeId, f => f.EventTypeId, (e, f) => new { e.Id, e.Title, e.TitleURL, e.ShortDescription, e.Category, e.CategoryURL, e.Period, e.Country, f })
                                        .SelectMany(m => m.f.DefaultIfEmpty(), (g, h) => new { g.Id, g.Title, g.TitleURL, g.ShortDescription, g.Category, g.CategoryURL, g.Period, g.Country, h.EventTypeId })
                                        .GroupJoin(datacontext.EventTypes, i => i.EventTypeId, j => j.EventTypeId, (i, j) => new { i.Id, i.Title, i.TitleURL, i.ShortDescription, i.Category, i.CategoryURL, i.Period, i.Country, j })
                                        .SelectMany(m => m.j.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.ShortDescription, k.Category, k.CategoryURL, k.Period, k.Country, EventType = l.Name, EventTypeURL = l.NameURL })
                                        .Distinct()
                                        .OrderBy(o => o.Title)
                                        .Skip(Skip)
                                        .Take(Take)
                                        .Select(s => new EventSimpleItem2() { Id = s.Id, Title = s.Title, TitleURL = s.TitleURL, ShortDescription = s.ShortDescription, Category = s.Category, CategoryURL = s.CategoryURL, EventType = s.EventType, EventTypeURL = s.EventTypeURL, Period = s.Period, Country = s.Country })
                                        .ToList();
                                    }
                                    break;
                                case 2: /* Category */
                                    {
                                        temps = datacontext.Events.Where(w => w.EventCategoryId == CategoryId && w.EventTypeId == EventTypeId).Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { Id = a.EventId, a.Title, a.TitleURL, a.ShortDescription, Period = a.From.Equals(a.To) ? string.Format("{0:dd/MM/yyyy}", a.From) : string.Format("{0:dd/MM/yyyy}", a.From) + " - " + string.Format("{0:dd/MM/yyyy}", a.To), Category = b.Name, CategoryURL = b.NameURL, a.CountryId, a.EventTypeId, b })
                                        .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.TitleURL, c.ShortDescription, c.Category, c.CategoryURL, c.Period, Country = d.Name, c.EventTypeId })
                                        .GroupJoin(datacontext.EventTypeCategories, e => e.EventTypeId, f => f.EventTypeId, (e, f) => new { e.Id, e.Title, e.TitleURL, e.ShortDescription, e.Category, e.CategoryURL, e.Period, e.Country, f })
                                        .SelectMany(m => m.f.DefaultIfEmpty(), (g, h) => new { g.Id, g.Title, g.TitleURL, g.ShortDescription, g.Category, g.CategoryURL, g.Period, g.Country, h.EventTypeId })
                                        .GroupJoin(datacontext.EventTypes, i => i.EventTypeId, j => j.EventTypeId, (i, j) => new { i.Id, i.Title, i.TitleURL, i.ShortDescription, i.Category, i.CategoryURL, i.Period, i.Country, j })
                                        .SelectMany(m => m.j.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.ShortDescription, k.Category, k.CategoryURL, k.Period, k.Country, EventType = l.Name, EventTypeURL = l.NameURL })
                                        .Distinct()
                                        .OrderBy(o => o.Category)
                                        .Skip(Skip)
                                        .Take(Take)
                                        .Select(s => new EventSimpleItem2() { Id = s.Id, Title = s.Title, TitleURL = s.TitleURL, ShortDescription = s.ShortDescription, Category = s.Category, CategoryURL = s.CategoryURL, EventType = s.EventType, EventTypeURL = s.EventTypeURL, Period = s.Period, Country = s.Country })
                                        .ToList();
                                    }
                                    break;
                                default: /* Id */
                                    {
                                        temps = datacontext.Events.Where(w => w.EventCategoryId == CategoryId && w.EventTypeId == EventTypeId).Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { Id = a.EventId, a.Title, a.TitleURL, a.ShortDescription, Period = a.From.Equals(a.To) ? string.Format("{0:dd/MM/yyyy}", a.From) : string.Format("{0:dd/MM/yyyy}", a.From) + " - " + string.Format("{0:dd/MM/yyyy}", a.To), Category = b.Name, CategoryURL = b.NameURL, a.CountryId, a.EventTypeId, b })
                                        .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.TitleURL, c.ShortDescription, c.Category, c.CategoryURL, c.Period, Country = d.Name, c.EventTypeId })
                                        .GroupJoin(datacontext.EventTypeCategories, e => e.EventTypeId, f => f.EventTypeId, (e, f) => new { e.Id, e.Title, e.TitleURL, e.ShortDescription, e.Category, e.CategoryURL, e.Period, e.Country, f })
                                        .SelectMany(m => m.f.DefaultIfEmpty(), (g, h) => new { g.Id, g.Title, g.TitleURL, g.ShortDescription, g.Category, g.CategoryURL, g.Period, g.Country, h.EventTypeId })
                                        .GroupJoin(datacontext.EventTypes, i => i.EventTypeId, j => j.EventTypeId, (i, j) => new { i.Id, i.Title, i.TitleURL, i.ShortDescription, i.Category, i.CategoryURL, i.Period, i.Country, j })
                                        .SelectMany(m => m.j.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.ShortDescription, k.Category, k.CategoryURL, k.Period, k.Country, EventType = l.Name, EventTypeURL = l.NameURL })
                                        .Distinct()
                                        .OrderBy(o => o.Id)
                                        .Skip(Skip)
                                        .Take(Take)
                                        .Select(s => new EventSimpleItem2() { Id = s.Id, Title = s.Title, TitleURL = s.TitleURL, ShortDescription = s.ShortDescription, Category = s.Category, CategoryURL = s.CategoryURL, EventType = s.EventType, EventTypeURL = s.EventTypeURL, Period = s.Period, Country = s.Country })
                                        .ToList();
                                    }
                                    break;
                            }
                        }
                    }
                }
                else /* if keywords is not null */
                {
                    if (CategoryId == 0)
                    {
                        switch (Orderby)
                        {
                            case 0:/* Period */
                                {
                                    temps = datacontext.Events.Where(w => Ids.Contains(w.EventId)).Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { Id = a.EventId, a.Title, a.TitleURL, a.ShortDescription, Period = a.From.Equals(a.To) ? string.Format("{0:dd/MM/yyyy}", a.From) : string.Format("{0:dd/MM/yyyy}", a.From) + " - " + string.Format("{0:dd/MM/yyyy}", a.To), Category = b.Name, CategoryURL = b.NameURL, a.CountryId, a.EventTypeId, b })
                                    .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.TitleURL, c.ShortDescription, c.Category, c.CategoryURL, c.Period, Country = d.Name, c.EventTypeId })
                                    .GroupJoin(datacontext.EventTypeCategories, e => e.EventTypeId, f => f.EventTypeId, (e, f) => new { e.Id, e.Title, e.TitleURL, e.ShortDescription, e.Category, e.CategoryURL, e.Period, e.Country, f })
                                    .SelectMany(m => m.f.DefaultIfEmpty(), (g, h) => new { g.Id, g.Title, g.TitleURL, g.ShortDescription, g.Category, g.CategoryURL, g.Period, g.Country, h.EventTypeId })
                                    .GroupJoin(datacontext.EventTypes, i => i.EventTypeId, j => j.EventTypeId, (i, j) => new { i.Id, i.Title, i.TitleURL, i.ShortDescription, i.Category, i.CategoryURL, i.Period, i.Country, j })
                                    .SelectMany(m => m.j.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.ShortDescription, k.Category, k.CategoryURL, k.Period, k.Country, EventType = l.Name, EventTypeURL = l.NameURL })
                                    .Distinct()
                                    .OrderByDescending(o => o.Period)
                                    .Skip(Skip)
                                    .Take(Take)
                                    .Select(s => new EventSimpleItem2() { Id = s.Id, Title = s.Title, TitleURL = s.TitleURL, ShortDescription = s.ShortDescription, Category = s.Category, CategoryURL = s.CategoryURL, EventType = s.EventType, EventTypeURL = s.EventTypeURL, Period = s.Period, Country = s.Country })
                                    .ToList();
                                }
                                break;
                            case 1: /* Title */
                                {
                                    temps = datacontext.Events.Where(w => Ids.Contains(w.EventId)).Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { Id = a.EventId, a.Title, a.TitleURL, a.ShortDescription, Period = a.From.Equals(a.To) ? string.Format("{0:dd/MM/yyyy}", a.From) : string.Format("{0:dd/MM/yyyy}", a.From) + " - " + string.Format("{0:dd/MM/yyyy}", a.To), Category = b.Name, CategoryURL = b.NameURL, a.CountryId, a.EventTypeId, b })
                                    .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.TitleURL, c.ShortDescription, c.Category, c.CategoryURL, c.Period, Country = d.Name, c.EventTypeId })
                                    .GroupJoin(datacontext.EventTypeCategories, e => e.EventTypeId, f => f.EventTypeId, (e, f) => new { e.Id, e.Title, e.TitleURL, e.ShortDescription, e.Category, e.CategoryURL, e.Period, e.Country, f })
                                    .SelectMany(m => m.f.DefaultIfEmpty(), (g, h) => new { g.Id, g.Title, g.TitleURL, g.ShortDescription, g.Category, g.CategoryURL, g.Period, g.Country, h.EventTypeId })
                                    .GroupJoin(datacontext.EventTypes, i => i.EventTypeId, j => j.EventTypeId, (i, j) => new { i.Id, i.Title, i.TitleURL, i.ShortDescription, i.Category, i.CategoryURL, i.Period, i.Country, j })
                                    .SelectMany(m => m.j.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.ShortDescription, k.Category, k.CategoryURL, k.Period, k.Country, EventType = l.Name, EventTypeURL = l.NameURL })
                                    .Distinct()
                                    .OrderBy(o => o.Title)
                                    .Skip(Skip)
                                    .Take(Take)
                                    .Select(s => new EventSimpleItem2() { Id = s.Id, Title = s.Title, TitleURL = s.TitleURL, ShortDescription = s.ShortDescription, Category = s.Category, CategoryURL = s.CategoryURL, EventType = s.EventType, EventTypeURL = s.EventTypeURL, Period = s.Period, Country = s.Country })
                                    .ToList();
                                }
                                break;
                            case 2: /* Category */
                                {
                                    temps = datacontext.Events.Where(w => Ids.Contains(w.EventId)).Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { Id = a.EventId, a.Title, a.TitleURL, a.ShortDescription, Period = a.From.Equals(a.To) ? string.Format("{0:dd/MM/yyyy}", a.From) : string.Format("{0:dd/MM/yyyy}", a.From) + " - " + string.Format("{0:dd/MM/yyyy}", a.To), Category = b.Name, CategoryURL = b.NameURL, a.CountryId, a.EventTypeId, b })
                                    .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.TitleURL, c.ShortDescription, c.Category, c.CategoryURL, c.Period, Country = d.Name, c.EventTypeId })
                                    .GroupJoin(datacontext.EventTypeCategories, e => e.EventTypeId, f => f.EventTypeId, (e, f) => new { e.Id, e.Title, e.TitleURL, e.ShortDescription, e.Category, e.CategoryURL, e.Period, e.Country, f })
                                    .SelectMany(m => m.f.DefaultIfEmpty(), (g, h) => new { g.Id, g.Title, g.TitleURL, g.ShortDescription, g.Category, g.CategoryURL, g.Period, g.Country, h.EventTypeId })
                                    .GroupJoin(datacontext.EventTypes, i => i.EventTypeId, j => j.EventTypeId, (i, j) => new { i.Id, i.Title, i.TitleURL, i.ShortDescription, i.Category, i.CategoryURL, i.Period, i.Country, j })
                                    .SelectMany(m => m.j.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.ShortDescription, k.Category, k.CategoryURL, k.Period, k.Country, EventType = l.Name, EventTypeURL = l.NameURL })
                                    .Distinct()
                                    .OrderBy(o => o.Category)
                                    .Skip(Skip)
                                    .Take(Take)
                                    .Select(s => new EventSimpleItem2() { Id = s.Id, Title = s.Title, TitleURL = s.TitleURL, ShortDescription = s.ShortDescription, Category = s.Category, CategoryURL = s.CategoryURL, EventType = s.EventType, EventTypeURL = s.EventTypeURL, Period = s.Period, Country = s.Country })
                                    .ToList();
                                }
                                break;
                            default: /* Id */
                                {
                                    temps = datacontext.Events.Where(w => Ids.Contains(w.EventId)).Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { Id = a.EventId, a.Title, a.TitleURL, a.ShortDescription, Period = a.From.Equals(a.To) ? string.Format("{0:dd/MM/yyyy}", a.From) : string.Format("{0:dd/MM/yyyy}", a.From) + " - " + string.Format("{0:dd/MM/yyyy}", a.To), Category = b.Name, CategoryURL = b.NameURL, a.CountryId, a.EventTypeId, b })
                                    .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.TitleURL, c.ShortDescription, c.Category, c.CategoryURL, c.Period, Country = d.Name, c.EventTypeId })
                                    .GroupJoin(datacontext.EventTypeCategories, e => e.EventTypeId, f => f.EventTypeId, (e, f) => new { e.Id, e.Title, e.TitleURL, e.ShortDescription, e.Category, e.CategoryURL, e.Period, e.Country, f })
                                    .SelectMany(m => m.f.DefaultIfEmpty(), (g, h) => new { g.Id, g.Title, g.TitleURL, g.ShortDescription, g.Category, g.CategoryURL, g.Period, g.Country, h.EventTypeId })
                                    .GroupJoin(datacontext.EventTypes, i => i.EventTypeId, j => j.EventTypeId, (i, j) => new { i.Id, i.Title, i.TitleURL, i.ShortDescription, i.Category, i.CategoryURL, i.Period, i.Country, j })
                                    .SelectMany(m => m.j.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.ShortDescription, k.Category, k.CategoryURL, k.Period, k.Country, EventType = l.Name, EventTypeURL = l.NameURL })
                                    .Distinct()
                                    .OrderBy(o => o.Id)
                                    .Skip(Skip)
                                    .Take(Take)
                                    .Select(s => new EventSimpleItem2() { Id = s.Id, Title = s.Title, TitleURL = s.TitleURL, ShortDescription = s.ShortDescription, Category = s.Category, CategoryURL = s.CategoryURL, EventType = s.EventType, EventTypeURL = s.EventTypeURL, Period = s.Period, Country = s.Country })
                                    .ToList();
                                }
                                break;
                        }
                    }
                    else /* if CategoryId > 0 */
                    {
                        if (EventTypeId == 0)
                        {
                            switch (Orderby)
                            {
                                case 0:/* Period */
                                    {
                                        temps = datacontext.Events.Where(w => Ids.Contains(w.EventId) && w.EventCategoryId == CategoryId).Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { Id = a.EventId, a.Title, a.TitleURL, a.ShortDescription, Period = a.From.Equals(a.To) ? string.Format("{0:dd/MM/yyyy}", a.From) : string.Format("{0:dd/MM/yyyy}", a.From) + " - " + string.Format("{0:dd/MM/yyyy}", a.To), Category = b.Name, CategoryURL = b.NameURL, a.CountryId, a.EventTypeId, b })
                                        .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.TitleURL, c.ShortDescription, c.Category, c.CategoryURL, c.Period, Country = d.Name, c.EventTypeId })
                                        .GroupJoin(datacontext.EventTypeCategories, e => e.EventTypeId, f => f.EventTypeId, (e, f) => new { e.Id, e.Title, e.TitleURL, e.ShortDescription, e.Category, e.CategoryURL, e.Period, e.Country, f })
                                        .SelectMany(m => m.f.DefaultIfEmpty(), (g, h) => new { g.Id, g.Title, g.TitleURL, g.ShortDescription, g.Category, g.CategoryURL, g.Period, g.Country, h.EventTypeId })
                                        .GroupJoin(datacontext.EventTypes, i => i.EventTypeId, j => j.EventTypeId, (i, j) => new { i.Id, i.Title, i.TitleURL, i.ShortDescription, i.Category, i.CategoryURL, i.Period, i.Country, j })
                                        .SelectMany(m => m.j.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.ShortDescription, k.Category, k.CategoryURL, k.Period, k.Country, EventType = l.Name, EventTypeURL = l.NameURL })
                                        .Distinct()
                                        .OrderByDescending(o => o.Period)
                                        .Skip(Skip)
                                        .Take(Take)
                                        .Select(s => new EventSimpleItem2() { Id = s.Id, Title = s.Title, TitleURL = s.TitleURL, ShortDescription = s.ShortDescription, Category = s.Category, CategoryURL = s.CategoryURL, EventType = s.EventType, EventTypeURL = s.EventTypeURL, Period = s.Period, Country = s.Country })
                                        .ToList();
                                    }
                                    break;
                                case 1: /* Title */
                                    {
                                        temps = datacontext.Events.Where(w => Ids.Contains(w.EventId) && w.EventCategoryId == CategoryId).Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { Id = a.EventId, a.Title, a.TitleURL, a.ShortDescription, Period = a.From.Equals(a.To) ? string.Format("{0:dd/MM/yyyy}", a.From) : string.Format("{0:dd/MM/yyyy}", a.From) + " - " + string.Format("{0:dd/MM/yyyy}", a.To), Category = b.Name, CategoryURL = b.NameURL, a.CountryId, a.EventTypeId, b })
                                        .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.TitleURL, c.ShortDescription, c.Category, c.CategoryURL, c.Period, Country = d.Name, c.EventTypeId })
                                        .GroupJoin(datacontext.EventTypeCategories, e => e.EventTypeId, f => f.EventTypeId, (e, f) => new { e.Id, e.Title, e.TitleURL, e.ShortDescription, e.Category, e.CategoryURL, e.Period, e.Country, f })
                                        .SelectMany(m => m.f.DefaultIfEmpty(), (g, h) => new { g.Id, g.Title, g.TitleURL, g.ShortDescription, g.Category, g.CategoryURL, g.Period, g.Country, h.EventTypeId })
                                        .GroupJoin(datacontext.EventTypes, i => i.EventTypeId, j => j.EventTypeId, (i, j) => new { i.Id, i.Title, i.TitleURL, i.ShortDescription, i.Category, i.CategoryURL, i.Period, i.Country, j })
                                        .SelectMany(m => m.j.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.ShortDescription, k.Category, k.CategoryURL, k.Period, k.Country, EventType = l.Name, EventTypeURL = l.NameURL })
                                        .Distinct()
                                        .OrderBy(o => o.Title)
                                        .Skip(Skip)
                                        .Take(Take)
                                        .Select(s => new EventSimpleItem2() { Id = s.Id, Title = s.Title, TitleURL = s.TitleURL, ShortDescription = s.ShortDescription, Category = s.Category, CategoryURL = s.CategoryURL, EventType = s.EventType, EventTypeURL = s.EventTypeURL, Period = s.Period, Country = s.Country })
                                        .ToList();
                                    }
                                    break;
                                case 2: /* Category */
                                    {
                                        temps = datacontext.Events.Where(w => Ids.Contains(w.EventId) && w.EventCategoryId == CategoryId).Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { Id = a.EventId, a.Title, a.TitleURL, a.ShortDescription, Period = a.From.Equals(a.To) ? string.Format("{0:dd/MM/yyyy}", a.From) : string.Format("{0:dd/MM/yyyy}", a.From) + " - " + string.Format("{0:dd/MM/yyyy}", a.To), Category = b.Name, CategoryURL = b.NameURL, a.CountryId, a.EventTypeId, b })
                                        .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.TitleURL, c.ShortDescription, c.Category, c.CategoryURL, c.Period, Country = d.Name, c.EventTypeId })
                                        .GroupJoin(datacontext.EventTypeCategories, e => e.EventTypeId, f => f.EventTypeId, (e, f) => new { e.Id, e.Title, e.TitleURL, e.ShortDescription, e.Category, e.CategoryURL, e.Period, e.Country, f })
                                        .SelectMany(m => m.f.DefaultIfEmpty(), (g, h) => new { g.Id, g.Title, g.TitleURL, g.ShortDescription, g.Category, g.CategoryURL, g.Period, g.Country, h.EventTypeId })
                                        .GroupJoin(datacontext.EventTypes, i => i.EventTypeId, j => j.EventTypeId, (i, j) => new { i.Id, i.Title, i.TitleURL, i.ShortDescription, i.Category, i.CategoryURL, i.Period, i.Country, j })
                                        .SelectMany(m => m.j.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.ShortDescription, k.Category, k.CategoryURL, k.Period, k.Country, EventType = l.Name, EventTypeURL = l.NameURL })
                                        .Distinct()
                                        .OrderBy(o => o.Category)
                                        .Skip(Skip)
                                        .Take(Take)
                                        .Select(s => new EventSimpleItem2() { Id = s.Id, Title = s.Title, TitleURL = s.TitleURL, ShortDescription = s.ShortDescription, Category = s.Category, CategoryURL = s.CategoryURL, EventType = s.EventType, EventTypeURL = s.EventTypeURL, Period = s.Period, Country = s.Country })
                                        .ToList();
                                    }
                                    break;
                                default: /* Id */
                                    {
                                        temps = datacontext.Events.Where(w => Ids.Contains(w.EventId) && w.EventCategoryId == CategoryId).Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { Id = a.EventId, a.Title, a.TitleURL, a.ShortDescription, Period = a.From.Equals(a.To) ? string.Format("{0:dd/MM/yyyy}", a.From) : string.Format("{0:dd/MM/yyyy}", a.From) + " - " + string.Format("{0:dd/MM/yyyy}", a.To), Category = b.Name, CategoryURL = b.NameURL, a.CountryId, a.EventTypeId, b })
                                        .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.TitleURL, c.ShortDescription, c.Category, c.CategoryURL, c.Period, Country = d.Name, c.EventTypeId })
                                        .GroupJoin(datacontext.EventTypeCategories, e => e.EventTypeId, f => f.EventTypeId, (e, f) => new { e.Id, e.Title, e.TitleURL, e.ShortDescription, e.Category, e.CategoryURL, e.Period, e.Country, f })
                                        .SelectMany(m => m.f.DefaultIfEmpty(), (g, h) => new { g.Id, g.Title, g.TitleURL, g.ShortDescription, g.Category, g.CategoryURL, g.Period, g.Country, h.EventTypeId })
                                        .GroupJoin(datacontext.EventTypes, i => i.EventTypeId, j => j.EventTypeId, (i, j) => new { i.Id, i.Title, i.TitleURL, i.ShortDescription, i.Category, i.CategoryURL, i.Period, i.Country, j })
                                        .SelectMany(m => m.j.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.ShortDescription, k.Category, k.CategoryURL, k.Period, k.Country, EventType = l.Name, EventTypeURL = l.NameURL })
                                        .Distinct()
                                        .OrderBy(o => o.Id)
                                        .Skip(Skip)
                                        .Take(Take)
                                        .Select(s => new EventSimpleItem2() { Id = s.Id, Title = s.Title, TitleURL = s.TitleURL, ShortDescription = s.ShortDescription, Category = s.Category, CategoryURL = s.CategoryURL, EventType = s.EventType, EventTypeURL = s.EventTypeURL, Period = s.Period, Country = s.Country })
                                        .ToList();
                                    }
                                    break;
                            }
                        }
                        else /* EventTypeId > 0 */
                        {
                            switch (Orderby)
                            {
                                case 0:/* Period */
                                    {
                                        temps = datacontext.Events.Where(w => Ids.Contains(w.EventId) && w.EventCategoryId == CategoryId && w.EventTypeId == EventTypeId).Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { Id = a.EventId, a.Title, a.TitleURL, a.ShortDescription, Period = a.From.Equals(a.To) ? string.Format("{0:dd/MM/yyyy}", a.From) : string.Format("{0:dd/MM/yyyy}", a.From) + " - " + string.Format("{0:dd/MM/yyyy}", a.To), Category = b.Name, CategoryURL = b.NameURL, a.CountryId, a.EventTypeId, b })
                                        .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.TitleURL, c.ShortDescription, c.Category, c.CategoryURL, c.Period, Country = d.Name, c.EventTypeId })
                                        .GroupJoin(datacontext.EventTypeCategories, e => e.EventTypeId, f => f.EventTypeId, (e, f) => new { e.Id, e.Title, e.TitleURL, e.ShortDescription, e.Category, e.CategoryURL, e.Period, e.Country, f })
                                        .SelectMany(m => m.f.DefaultIfEmpty(), (g, h) => new { g.Id, g.Title, g.TitleURL, g.ShortDescription, g.Category, g.CategoryURL, g.Period, g.Country, h.EventTypeId })
                                        .GroupJoin(datacontext.EventTypes, i => i.EventTypeId, j => j.EventTypeId, (i, j) => new { i.Id, i.Title, i.TitleURL, i.ShortDescription, i.Category, i.CategoryURL, i.Period, i.Country, j })
                                        .SelectMany(m => m.j.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.ShortDescription, k.Category, k.CategoryURL, k.Period, k.Country, EventType = l.Name, EventTypeURL = l.NameURL })
                                        .Distinct()
                                        .OrderByDescending(o => o.Period)
                                        .Skip(Skip)
                                        .Take(Take)
                                        .Select(s => new EventSimpleItem2() { Id = s.Id, Title = s.Title, TitleURL = s.TitleURL, ShortDescription = s.ShortDescription, Category = s.Category, CategoryURL = s.CategoryURL, EventType = s.EventType, EventTypeURL = s.EventTypeURL, Period = s.Period, Country = s.Country })
                                        .ToList();
                                    }
                                    break;
                                case 1: /* Title */
                                    {
                                        temps = datacontext.Events.Where(w => Ids.Contains(w.EventId) && w.EventCategoryId == CategoryId && w.EventTypeId == EventTypeId).Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { Id = a.EventId, a.Title, a.TitleURL, a.ShortDescription, Period = a.From.Equals(a.To) ? string.Format("{0:dd/MM/yyyy}", a.From) : string.Format("{0:dd/MM/yyyy}", a.From) + " - " + string.Format("{0:dd/MM/yyyy}", a.To), Category = b.Name, CategoryURL = b.NameURL, a.CountryId, a.EventTypeId, b })
                                        .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.TitleURL, c.ShortDescription, c.Category, c.CategoryURL, c.Period, Country = d.Name, c.EventTypeId })
                                        .GroupJoin(datacontext.EventTypeCategories, e => e.EventTypeId, f => f.EventTypeId, (e, f) => new { e.Id, e.Title, e.TitleURL, e.ShortDescription, e.Category, e.CategoryURL, e.Period, e.Country, f })
                                        .SelectMany(m => m.f.DefaultIfEmpty(), (g, h) => new { g.Id, g.Title, g.TitleURL, g.ShortDescription, g.Category, g.CategoryURL, g.Period, g.Country, h.EventTypeId })
                                        .GroupJoin(datacontext.EventTypes, i => i.EventTypeId, j => j.EventTypeId, (i, j) => new { i.Id, i.Title, i.TitleURL, i.ShortDescription, i.Category, i.CategoryURL, i.Period, i.Country, j })
                                        .SelectMany(m => m.j.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.ShortDescription, k.Category, k.CategoryURL, k.Period, k.Country, EventType = l.Name, EventTypeURL = l.NameURL })
                                        .Distinct()
                                        .OrderBy(o => o.Title)
                                        .Skip(Skip)
                                        .Take(Take)
                                        .Select(s => new EventSimpleItem2() { Id = s.Id, Title = s.Title, TitleURL = s.TitleURL, ShortDescription = s.ShortDescription, Category = s.Category, CategoryURL = s.CategoryURL, EventType = s.EventType, EventTypeURL = s.EventTypeURL, Period = s.Period, Country = s.Country })
                                        .ToList();
                                    }
                                    break;
                                case 2: /* Category */
                                    {
                                        temps = datacontext.Events.Where(w => Ids.Contains(w.EventId) && w.EventCategoryId == CategoryId && w.EventTypeId == EventTypeId).Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { Id = a.EventId, a.Title, a.TitleURL, a.ShortDescription, Period = a.From.Equals(a.To) ? string.Format("{0:dd/MM/yyyy}", a.From) : string.Format("{0:dd/MM/yyyy}", a.From) + " - " + string.Format("{0:dd/MM/yyyy}", a.To), Category = b.Name, CategoryURL = b.NameURL, a.CountryId, a.EventTypeId, b })
                                        .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.TitleURL, c.ShortDescription, c.Category, c.CategoryURL, c.Period, Country = d.Name, c.EventTypeId })
                                        .GroupJoin(datacontext.EventTypeCategories, e => e.EventTypeId, f => f.EventTypeId, (e, f) => new { e.Id, e.Title, e.TitleURL, e.ShortDescription, e.Category, e.CategoryURL, e.Period, e.Country, f })
                                        .SelectMany(m => m.f.DefaultIfEmpty(), (g, h) => new { g.Id, g.Title, g.TitleURL, g.ShortDescription, g.Category, g.CategoryURL, g.Period, g.Country, h.EventTypeId })
                                        .GroupJoin(datacontext.EventTypes, i => i.EventTypeId, j => j.EventTypeId, (i, j) => new { i.Id, i.Title, i.TitleURL, i.ShortDescription, i.Category, i.CategoryURL, i.Period, i.Country, j })
                                        .SelectMany(m => m.j.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.ShortDescription, k.Category, k.CategoryURL, k.Period, k.Country, EventType = l.Name, EventTypeURL = l.NameURL })
                                        .Distinct()
                                        .OrderBy(o => o.Category)
                                        .Skip(Skip)
                                        .Take(Take)
                                        .Select(s => new EventSimpleItem2() { Id = s.Id, Title = s.Title, TitleURL = s.TitleURL, ShortDescription = s.ShortDescription, Category = s.Category, CategoryURL = s.CategoryURL, EventType = s.EventType, EventTypeURL = s.EventTypeURL, Period = s.Period, Country = s.Country })
                                        .ToList();
                                    }
                                    break;
                                default: /* Id */
                                    {
                                        temps = datacontext.Events.Where(w => Ids.Contains(w.EventId) && w.EventCategoryId == CategoryId && w.EventTypeId == EventTypeId).Join(datacontext.EventCategories, a => a.EventCategoryId, b => b.EventCategoryId, (a, b) => new { Id = a.EventId, a.Title, a.TitleURL, a.ShortDescription, Period = a.From.Equals(a.To) ? string.Format("{0:dd/MM/yyyy}", a.From) : string.Format("{0:dd/MM/yyyy}", a.From) + " - " + string.Format("{0:dd/MM/yyyy}", a.To), Category = b.Name, CategoryURL = b.NameURL, a.CountryId, a.EventTypeId, b })
                                        .Join(datacontext.Countries, c => c.CountryId, d => d.CountryId, (c, d) => new { c.Id, c.Title, c.TitleURL, c.ShortDescription, c.Category, c.CategoryURL, c.Period, Country = d.Name, c.EventTypeId })
                                        .GroupJoin(datacontext.EventTypeCategories, e => e.EventTypeId, f => f.EventTypeId, (e, f) => new { e.Id, e.Title, e.TitleURL, e.ShortDescription, e.Category, e.CategoryURL, e.Period, e.Country, f })
                                        .SelectMany(m => m.f.DefaultIfEmpty(), (g, h) => new { g.Id, g.Title, g.TitleURL, g.ShortDescription, g.Category, g.CategoryURL, g.Period, g.Country, h.EventTypeId })
                                        .GroupJoin(datacontext.EventTypes, i => i.EventTypeId, j => j.EventTypeId, (i, j) => new { i.Id, i.Title, i.TitleURL, i.ShortDescription, i.Category, i.CategoryURL, i.Period, i.Country, j })
                                        .SelectMany(m => m.j.DefaultIfEmpty(), (k, l) => new { k.Id, k.Title, k.TitleURL, k.ShortDescription, k.Category, k.CategoryURL, k.Period, k.Country, EventType = l.Name, EventTypeURL = l.NameURL })
                                        .Distinct()
                                        .OrderBy(o => o.Id)
                                        .Skip(Skip)
                                        .Take(Take)
                                        .Select(s => new EventSimpleItem2() { Id = s.Id, Title = s.Title, TitleURL = s.TitleURL, ShortDescription = s.ShortDescription, Category = s.Category, CategoryURL = s.CategoryURL, EventType = s.EventType, EventTypeURL = s.EventTypeURL, Period = s.Period, Country = s.Country })
                                        .ToList();
                                    }
                                    break;
                            }
                        }
                    }
                }

                List<EventSimpleItem2> collection = new List<EventSimpleItem2>();
                int count = 0;
                for (int i = 0; i < temps.Count(); i++)
                {
                    collection.Add(temps[i]);
                    count++;
                    if (count >= PageSize || i == temps.Count() - 1)
                    {
                        result.Events.Add(collection);
                        collection = new List<EventSimpleItem2>();
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
