using System.IO;
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
    public class CarouselServiceTest : Base
    {
        private CarouselService carouselService;
        [SetUp]
        public void Setup()
        {
            carouselService = new CarouselService(context,configuration);
        }
        [TestCase((int)TagGroupType.Job_Search, "Student Service Officer")]
        [TestCase((int)TagGroupType.Job_Search, "Pick & Packing")]
        public void Create_Test(int group,string name)
        {
            Carousel carousel = new Carousel();
            bool check = carouselService.Create(carousel);
            Assert.IsTrue(check);
        }
    }
}