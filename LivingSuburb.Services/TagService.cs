using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using LivingSuburb.Models;
using LivingSuburb.Database;

namespace LivingSuburb.Services
{
    public class TagService
    {
        private DataContext datacontext;
        public TagService(DataContext context)
        {
            datacontext = context;
        }


        public Tag Read(int id)
        {
            Tag result = null;
            if (datacontext.Tags.Where(w => w.TagId == id).Any())
            {
                result = datacontext.Tags.Where(w => w.TagId == id).FirstOrDefault();
            }
            return result;
        }

        public Tag Read(string tagName,int tagGroupId) {
            return datacontext.Tags.Where(w => w.Name == tagName && w.TagGroupId == tagGroupId).FirstOrDefault();
        }

        public bool Create(Tag model)
        {
            bool status = false;
            if (!datacontext.Tags.Where(w=> w.TagGroupId == model.TagGroupId && w.Name.ToUpper() == model.Name.ToUpper()).Any())
            {
                datacontext.Tags.Add(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Update(Tag model)
        {
            bool status = false;
            if (datacontext.Tags.Where(w =>w.TagId > 0 && w.TagGroupId == model.TagGroupId && w.Name.ToUpper() == model.Name.ToUpper()).Any())
            {
                datacontext.Tags.Update(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Delete(Tag model)
        {
            bool status = false;
            if (datacontext.Tags.Where(w => w.TagId > 0 && w.TagGroupId == model.TagGroupId && w.Name.ToUpper() == model.Name.ToUpper()).Any())
            {
                datacontext.Tags.Remove(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Delete(int id)
        {
            bool status = false;
            Tag tag = datacontext.Tags.Where(w => w.TagId == id).FirstOrDefault();
            if(tag != null)
            {
                datacontext.Tags.Remove(tag);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public List<string> SearchByKO(int TagGroupId,string Keywords,int Take) /* Select by keywords only */
        {
            return datacontext.Tags.Where(w => w.TagGroupId == TagGroupId && w.Name.StartsWith(Keywords)).OrderBy(o=>o.Name).Select(s => s.Name).Take(Take).ToList();
        }

        public List<SearchTagResult> SearchByKO2(int TagGroupId, string Keywords, int Take) /* Select by keywords only */
        {
            List<SearchTagResult> result = new List<SearchTagResult>();
            if (!string.IsNullOrEmpty(Keywords))
            {
                result = datacontext.Tags.Where(w => w.TagGroupId == TagGroupId && w.Name.StartsWith(Keywords)).OrderBy(o => o.Name).Take(Take).Select(s => new SearchTagResult { Id = s.TagId, Name = s.Name }).ToList();
            }
            return result;
        }

        public SearchTagListResult Search(int TagGroupId, string StartWith, string Keywords,int PageNo, int PageSize, int BlockSize)
        {
            int Start = 0;
            int Stop = 0;
            int NumberOfPages = 0;
            int NumberOfBlocks = 0;
            int BlockNo = 0;
            int NumberOfRecords = 0;

            PageSize = PageSize <= 0 ? 10 : PageSize;
            BlockSize = BlockSize <= 0 ? 10 : BlockSize;
            List<string> keywordList = new List<string>();
            SearchTagListResult result = new SearchTagListResult();
            TagGroupId = TagGroupId <= 0 ? 1 : TagGroupId;

            if(string.IsNullOrEmpty(Keywords)) /* if keywords is empty, startwith won't be empty */
            {
                NumberOfRecords = datacontext.Tags.Where(w => w.TagGroupId == TagGroupId && w.Name.StartsWith(StartWith)).Count();
            }
            else /* keyword is not empty,startwith must be empty */
            {
                keywordList = Keywords.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                for(int i=0;i<keywordList.Count;i++)
                {
                    NumberOfRecords += datacontext.Tags.Where(w => w.TagGroupId == TagGroupId && w.Name.StartsWith(keywordList[i])).Count();
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

                List<Tag> selectedTags = new List<Tag>();

                if (string.IsNullOrEmpty(Keywords)) /* if keywords is empty, startwith won't be empty */
                {
                    selectedTags = datacontext.Tags.Where(w => w.TagGroupId == TagGroupId && w.Name.StartsWith(StartWith)).OrderBy(o => o.Name).Skip(Skip).Take(Take).ToList();
                }
                else /* keyword is not empty,startwith must be empty */
                {
                    List<Tag> temp = new List<Tag>();
                    for(int i=0;i<keywordList.Count;i++)
                    {
                        temp.AddRange(datacontext.Tags.Where(w => w.TagGroupId == TagGroupId && w.Name.StartsWith(keywordList[i])).ToArray());
                    }
                    selectedTags = temp.OrderBy(o => o.Name).Skip(Skip).Take(Take).ToList();
                }

                List<Tag> collection = new List<Tag>();
                int count = 0;
                for (int i=0;i<selectedTags.Count();i++)
                {
                    collection.Add(selectedTags[i]);
                    count++;
                    if (count >= PageSize || i == selectedTags.Count() - 1)
                    {
                        result.Tags.Add(collection);
                        collection = new List<Tag>();
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
