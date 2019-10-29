using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace LivingSuburb.Web.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("ContactUs")]
    public class ContactUsTagHelper : TagHelper
    {
        private StringBuilder content = new StringBuilder(@"
        <div class=""headline""><h2>Contact Us</h2></div>
        <address class=""md-margin-bottom-40"">
            {{Suburb}}<br />
            {{State}}, {{PostCode}} <br />
            {{Country}}<br />
            Email: <a href = ""mailto:{{Email}}"">{{Email}}</a>
        </address>
        ") {};
        public string Suburb { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }

        private string getContent()
        {
            return content.Replace(@"{{Suburb}}", Suburb).Replace("{{State}}",State).Replace("{{PostCode}}",PostCode).Replace("{{Country}}",Country).Replace("{{Email}}",Email).ToString();
        }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "col-md-3 md-margin-bottom-40");
            output.PostContent.AppendHtml(getContent());
        }
    }
}
