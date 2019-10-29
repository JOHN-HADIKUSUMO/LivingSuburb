using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using LivingSuburb.Services;
using LivingSuburb.Models;

namespace LivingSuburb.Web.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("OurMission")]
    public class MissionsTagHelper : TagHelper
    {
        private OurMissionService ourMissionService;
        private IMemoryCache cache;
        public MissionsTagHelper(OurMissionService ourMissionService, IMemoryCache cache)
        {
            this.ourMissionService = ourMissionService;
            this.cache = cache;
        }

        private StringBuilder content = new StringBuilder(@"
        <div class=""headline""><h2>Our Mission</h2></div>
        {{Statement}}
        ") {};

        private async Task<string> getContent()
        {
           OurMission ourMission = await ourMissionService.Get();
           return content.Replace("{{Statement}}", ourMission.Statement).ToString();
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            string content = await getContent();
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "col-md-3 md-margin-bottom-40");
            output.PostContent.AppendHtml(content);
        }
    }
}
