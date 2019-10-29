using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using LivingSuburb.Services;
using LivingSuburb.Models;

namespace LivingSuburb.Web.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("News")]
    public class NewsTagHelper : TagHelper
    {
        private NewsService newsService;
        public NewsTagHelper(NewsService newsService)
        {
            this.newsService = newsService;
        }

        private StringBuilder content = new StringBuilder(@"
            <div class=""headline""><h2>Latest Post</h2></div>
            <ul class=""list-unstyled link-list"">
                {{News}}
            </ul>
        ") { };

        private string getContent()
        {
            StringBuilder tempString = new StringBuilder() { };
            List<News> news = this.newsService.GetLatest(3);
            foreach (var item in news)
            {
                tempString.Append("<li><a href = \"" + item.NewsURL + " target=\"_blank\">" + item.NewsTitle + "</a></li>" + Environment.NewLine);
            }
            return content.Replace(@"{{News}}", tempString.ToString()).ToString();
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "col-md-3 md-margin-bottom-40");
            output.PostContent.AppendHtml(getContent());
        }
    }
}
