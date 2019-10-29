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
    public class EventServiceTest : Base
    {
        private EventService eventService;
        [SetUp]
        public void Setup()
        {
            eventService = new EventService(context);
        }
        [TestCase(10)]
        public void ReadDetail_Test(int id)
        {
            EventDetail eventDetail = eventService.ReadDetail(id);
            Assert.IsNull(eventDetail);
        }
    }
}