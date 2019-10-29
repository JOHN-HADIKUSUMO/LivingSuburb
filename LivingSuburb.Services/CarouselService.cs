using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Text;
using LivingSuburb.Models;
using LivingSuburb.Database;

namespace LivingSuburb.Services
{
    public class CarouselService
    {
        private DataContext datacontext;
        private string baseS3URL;
        private IConfiguration configuration;
        public CarouselService(DataContext context, IConfiguration configuration)
        {
            this.datacontext = context;
            this.configuration = configuration;
            baseS3URL = this.configuration.GetSection("AWS:S3:Carousels:BaseURL").Value;
        }

        public string GetImageURL(string url, int width, int height)
        {
            string[] temps = url.Split("/", StringSplitOptions.RemoveEmptyEntries);
            string[] timps = temps[temps.Length - 1].Split(".");
            StringBuilder result = new StringBuilder();
            result.Append(baseS3URL);
            result.Append("/" + timps[0] + "_" + width + "_" + height + "." + timps[1]);
            return result.ToString();
        }

        public bool Update(Carousel model)
        {
            bool status = false;
            try
            {
                this.datacontext.Carousels.Update(model);
                this.datacontext.SaveChanges();
                status = true;
            }
            catch (Exception ex)
            {
                string test = ex.GetBaseException().ToString();
            }
            return status;
        }

        public bool Create(Carousel model)
        {
            bool status = false;
            if (!this.datacontext.Carousels.Where(w => w.CarouselId == model.CarouselId).Any())
            {
                this.datacontext.Carousels.Add(model);
                this.datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Delete(Carousel model)
        {
            bool status = false;
            if (this.datacontext.Carousels.Where(w => w.CarouselId == model.CarouselId).Any())
            {
                this.datacontext.Carousels.Remove(model);
                this.datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Delete(int id)
        {
            bool status = false;
            if (this.datacontext.Carousels.Where(w => w.CarouselId == id).Any())
            {
                Carousel model = this.datacontext.Carousels.Where(w => w.CarouselId == id).FirstOrDefault();
                this.datacontext.Carousels.Remove(model);
                this.datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public Carousel Read(int id)
        {
            return this.datacontext.Carousels.Where(w => w.CarouselId == id).FirstOrDefault();
        }

        public List<Carousel> ReadAll()
        {
            return this.datacontext.Carousels.ToList();
        }

        public SearchCarouselListResult Search2(string Keywords, int PageNo, int PageSize, int BlockSize)
        {
            SearchCarouselListResult result = new SearchCarouselListResult();
            PageSize = PageSize <= 0 ? 10 : PageSize;
            BlockSize = BlockSize <= 0 ? 10 : BlockSize;

            int Start = 0;
            int Stop = 0;
            int NumberOfPages = 0;
            int NumberOfBlocks = 0;
            int BlockNo = 0;
            int NumberOfRecords = 0;
            List<int> Ids = new List<int>() { };

            if(string.IsNullOrEmpty(Keywords))
            {
                Ids = this.datacontext.Carousels.OrderBy(o => o.Location).Select(s => s.CarouselId).ToList<int>();
            }
            else
            {
                Ids = this.datacontext.Carousels.Where(w=> w.Location.StartsWith(Keywords)).OrderBy(o => o.Location).Select(s => s.CarouselId).ToList<int>();
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
                int test = this.datacontext.Carousels.Count();
                List<CarouselSimple> temps = this.datacontext.Carousels.Where(w => filteredIds.Contains(w.CarouselId)).Select(s => new CarouselSimple(s.CarouselId, s.Location, s.PublishedDate)).ToList();

                List<CarouselSimple> collection = new List<CarouselSimple>();
                int count = 0;
                for (int i = 0; i < temps.Count(); i++)
                {
                    collection.Add(temps[i]);
                    count++;
                    if (count >= PageSize || i == temps.Count() - 1)
                    {
                        result.Carousels.Add(collection);
                        collection = new List<CarouselSimple>();
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
