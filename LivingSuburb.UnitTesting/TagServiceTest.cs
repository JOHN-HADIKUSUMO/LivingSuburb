using System;
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
    public class TagServiceTest:Base
    {
        private TagService tagService;
        public TagServiceTest():base()
        {
            tagService = new TagService(context);
        }

        [TestCase("A")]
        [TestCase("B")]
        [TestCase("C")]
        [TestCase("D")]
        public void Search_Test(string startWith)
        {
            SearchTagListResult result = tagService.Search(1, startWith, null, 1, 10, 10);
            Assert.IsNotNull(result);
        }

        [TestCase((int)TagGroupType.Job_Search, "Student Service Officer")]
        [TestCase((int)TagGroupType.Job_Search, "Pick & Packing")]
        public void Create_Test(int group, string name)
        {
            Tag tag = new Tag(group, name);
            bool id = tagService.Create(tag);
            Assert.Greater(id, 0);
        }

        [TestCase(1)] /* See Table Tags on Database, make sure it does exist */
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void Delete_Test(int id)
        {
            bool status = tagService.Delete(id);
            Assert.IsTrue(status);
        }

        [TestCase()]
        public void Random_Test()
        {
            List<string> arrayOfStrings = new List<string>() {
            "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z",
            "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",
            "0","1","2","3","4","5","6","7","8","9"};
            int arrayCount = arrayOfStrings.Count;
            long tickCount = DateTime.Now.Ticks;
            string Hex = tickCount.ToString("X");
            Assert.IsTrue(true);
        }

        private long Recursive(long input,long tally)
        {
            return 0L;
        }
    };
}
