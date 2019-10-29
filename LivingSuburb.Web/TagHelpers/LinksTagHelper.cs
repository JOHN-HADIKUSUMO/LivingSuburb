using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using LivingSuburb.Services;

namespace LivingSuburb.Web.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("Links")]
    public class LinksTagHelper : TagHelper
    {
        private LinkService linkService;
        public LinksTagHelper(LinkService linkService)
        {
            this.linkService = linkService;
        }

        private StringBuilder content = new StringBuilder(@"
        <div class=""headline""><h2>Links</h2></div>
        <ul class=""list-unstyled link-list"">
            {{Links}}
        </ul>
        ") {};

        private string getContent()
        {
            StringBuilder tempString = new StringBuilder() { };
            var links = linkService.GetAll();
            foreach (var item in links)
            {
                tempString.Append("<li><a href = \"" + item.URL + " target=\"_blank\">" + item.Label + "</a><i class=\"fa fa-angle-right\"></i></li>" + Environment.NewLine);
            }
            return content.Replace(@"{{Links}}", tempString.ToString()).ToString();
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "col-md-3 md-margin-bottom-40");
            output.PostContent.AppendHtml(getContent());
        }
    }
}
