using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Linq;
using LivingSuburb.Models;
using LivingSuburb.Database;

namespace LivingSuburb.Services
{
    public class GalleryService
    {
        private DataContext datacontext;
        private string baseS3URL;
        private IConfiguration configuration;
        public GalleryService(DataContext context, IConfiguration configuration)
        {
            this.datacontext = context;
            this.configuration = configuration;
            baseS3URL = this.configuration.GetSection("AWS:S3:Galleries:BaseURL").Value;
        }

        public string GetImageURL(string url,int width,int height)
        {
            string[] temps = url.Split("/", StringSplitOptions.RemoveEmptyEntries);
            string[] timps = temps[temps.Length - 1].Split(".");
            StringBuilder result = new StringBuilder();
            result.Append(baseS3URL);
            result.Append("/" + timps[0] + "_" + width + "_" + height + "." + timps[1]);
            return result.ToString();
        }

        public GalleryListResult Search(string keywords, int Orderby, int PageNo, int PageSize, int BlockSize)
        {
            GalleryListResult result = new GalleryListResult();
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
                NumberOfRecords = datacontext.Galleries.OrderBy(o => o.GalleryId).Count();
            }
            else
            {
                NumberOfRecords = datacontext.Galleries.Join(datacontext.States, a => a.State, b => b.StateId, (a, b) => new { Id = a.GalleryId, a.Filename, State = b.ShortName })
                .Where(w => w.Filename.StartsWith(keywords) || w.State.StartsWith(keywords)).Count();
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
                List<GallerySimpleItem> temps;

                if (string.IsNullOrEmpty(keywords))
                {
                    switch (Orderby)
                    {
                        case 0: /* by Id */
                            temps = datacontext.Galleries.OrderBy(o => o.GalleryId).Skip(Skip).Take(Take).Select(s => new GallerySimpleItem(s.GalleryId, s.Filename, s.URL, GetImageURL(s.URL, 114,64), GetImageURL(s.URL, 570,320))).ToList();
                            break;
                        case 1: /* by Filename */
                            temps = datacontext.Galleries.OrderBy(o => o.Filename).Skip(Skip).Take(Take).Select(s => new GallerySimpleItem(s.GalleryId, s.Filename, s.URL, GetImageURL(s.URL, 114, 64), GetImageURL(s.URL, 570, 320))).ToList();
                            break;
                        case 2: /* by Suburb */
                            temps = datacontext.Galleries.OrderBy(o => o.Suburb).Skip(Skip).Take(Take).Select(s => new GallerySimpleItem(s.GalleryId, s.Filename, s.URL, GetImageURL(s.URL, 114, 64), GetImageURL(s.URL, 570, 320))).ToList();
                            break;
                        default: /* by State */
                            temps = datacontext.Galleries.Join(datacontext.States, a => a.State, b => b.StateId, (a, b) => new { Id = a.GalleryId, a.Filename, a.URL, State = b.ShortName }).OrderBy(o=> o.State).Select(s => new GallerySimpleItem(s.Id, s.Filename, s.URL, GetImageURL(s.URL, 114, 64), GetImageURL(s.URL, 570, 320))).ToList();
                            break;
                    }
                }
                else
                {
                    switch (Orderby)
                    {
                        case 0: /* by Id */
                            temps = datacontext.Galleries.Join(datacontext.States, a=> a.State, b=> b.StateId,(a,b)=> new { Id = a.GalleryId, a.Filename, a.URL, a.Suburb, State = b.ShortName}).Where(w=> w.Filename.StartsWith(keywords) || w.Suburb.StartsWith(keywords) || w.State.StartsWith(keywords)).OrderBy(o=> o.Id).Skip(Skip).Take(Take).Select(s => new GallerySimpleItem(s.Id, s.Filename, s.URL, GetImageURL(s.URL, 114, 64), GetImageURL(s.URL, 570, 320))).ToList();
                            break;
                        case 1: /* by Filename */
                            temps = datacontext.Galleries.Join(datacontext.States, a => a.State, b => b.StateId, (a, b) => new { Id = a.GalleryId, a.Filename, a.URL, a.Suburb, State = b.ShortName }).Where(w => w.Filename.StartsWith(keywords) || w.Suburb.StartsWith(keywords) || w.State.StartsWith(keywords)).OrderBy(o => o.Filename).Skip(Skip).Take(Take).Select(s => new GallerySimpleItem(s.Id, s.Filename, s.URL, GetImageURL(s.URL, 114, 64), GetImageURL(s.URL, 570, 320))).ToList();
                            break;
                        case 2: /* by Suburb */
                            temps = datacontext.Galleries.Join(datacontext.States, a => a.State, b => b.StateId, (a, b) => new { Id = a.GalleryId, a.Filename, a.URL, a.Suburb, State = b.ShortName }).Where(w => w.Filename.StartsWith(keywords) || w.Suburb.StartsWith(keywords) || w.State.StartsWith(keywords)).OrderBy(o => o.Suburb).Skip(Skip).Take(Take).Select(s => new GallerySimpleItem(s.Id, s.Filename, s.URL, GetImageURL(s.URL, 114, 64), GetImageURL(s.URL, 570, 320))).ToList();
                            break;
                        default: /* by State */
                            temps = datacontext.Galleries.Join(datacontext.States, a => a.State, b => b.StateId, (a, b) => new { Id = a.GalleryId, a.Filename, a.URL, a.Suburb, State = b.ShortName }).Where(w => w.Filename.StartsWith(keywords) || w.Suburb.StartsWith(keywords) || w.State.StartsWith(keywords)).OrderBy(o => o.State).Skip(Skip).Take(Take).Select(s => new GallerySimpleItem(s.Id, s.Filename, s.URL, GetImageURL(s.URL, 114, 64), GetImageURL(s.URL, 570, 320))).ToList();
                            break;
                    }
                }

                List<GallerySimpleItem> collection = new List<GallerySimpleItem>();
                int count = 0;
                for (int i = 0; i < temps.Count(); i++)
                {
                    collection.Add(temps[i]);
                    count++;
                    if (count >= PageSize || i == temps.Count() - 1)
                    {
                        result.Galleries.Add(collection);
                        collection = new List<GallerySimpleItem>();
                        count = 0;
                    }
                }
            }
            result.NumberOfPages = NumberOfPages;
            result.SelectedPageNo = PageNo;
            if(PageNo > result.Pages.Count)
            {
                result.SelectedIndex = (PageNo -((BlockNo - 1) * BlockSize) - 1);
            }
            else
            {
                result.SelectedIndex = PageNo - 1;
            }
            return result;
        }

        public bool Update(Gallery model)
        {
            bool status = false;
            if (datacontext.Galleries.Where(w => w.GalleryId == model.GalleryId).Any())
            {
                datacontext.Galleries.Update(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Create(Gallery model)
        {
            bool status = false;
            if (!datacontext.Galleries.Where(w => w.GalleryId == model.GalleryId).Any())
            {
                datacontext.Galleries.Add(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Delete(Gallery model)
        {
            bool status = false;
            if (datacontext.Galleries.Where(w => w.GalleryId == model.GalleryId).Any())
            {
                datacontext.Galleries.Remove(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Delete(int id)
        {
            bool status = false;
            if (datacontext.Galleries.Where(w => w.GalleryId == id).Any())
            {
                Gallery model = datacontext.Galleries.Where(w => w.GalleryId == id).FirstOrDefault();
                datacontext.Galleries.Remove(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public Gallery Read(int id)
        {
            return datacontext.Galleries.Where(w => w.GalleryId == id).FirstOrDefault();
        }

        public List<Gallery> ReadAll()
        {
            return datacontext.Galleries.ToList();
        }
    }
}
