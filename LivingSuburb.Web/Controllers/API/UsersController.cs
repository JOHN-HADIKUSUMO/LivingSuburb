using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using LivingSuburb.Web.Data;
using LivingSuburb.Models;

namespace LivingSuburb.Web.Controllers.API
{
    public class UsersController : ControllerBase
    {
        private UserManager<ApplicationUser> userManager;
        private IHostingEnvironment hostingEnvironment;
        private IConfiguration configuration;
        public UsersController(
            UserManager<ApplicationUser> userManager,
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration
        )
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/USERS/DELETE")]
        public async Task<IActionResult> Delete([FromBody] UserId UserId)
        {
            ApplicationUser user = await this.userManager.FindByIdAsync(UserId.Id);
            if(user != null)
            {
                IdentityResult deleteResult =  await this.userManager.DeleteAsync(user);
                if(deleteResult.Succeeded)
                {
                    return Ok();
                }
            }
            return BadRequest("Invalid User Id.");
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/USERS/SEARCH")]
        public IActionResult Search([FromBody] SearchUserKeywords model)
        {
            UserListResult result = new UserListResult();
            model.OrderBy = model.OrderBy <= 0 ? 0 : (model.OrderBy > 1 ? 1 : model.OrderBy);
            model.PageSize = model.PageSize <= 0 ? 10 : model.PageSize;
            model.BlockSize = model.BlockSize <= 0 ? 10 : model.BlockSize;

            int Start = 0;
            int Stop = 0;
            int NumberOfPages = 0;
            int NumberOfBlocks = 0;
            int BlockNo = 0;
            int NumberOfRecords = 0;

            if (string.IsNullOrEmpty(model.Keywords))
            {
                NumberOfRecords = this.userManager.Users.Count();
            }
            else
            {
                NumberOfRecords = this.userManager.Users.Where(w => w.Fullname.StartsWith(model.Keywords)).Count();
            }

            if (NumberOfRecords == 0)
            {
                NumberOfPages = 0;
                NumberOfBlocks = 0;
                model.PageNo = 0;
                BlockNo = 0;
            }
            else
            {
                if (NumberOfRecords <= model.PageSize)
                {
                    NumberOfPages = 1;
                    NumberOfBlocks = 1;
                    model.PageNo = 1;
                    BlockNo = 1;
                }
                else
                {
                    NumberOfPages = NumberOfRecords / model.PageSize;
                    if (NumberOfRecords % model.PageSize > 0)
                    {
                        NumberOfPages++;
                    }

                    NumberOfBlocks = NumberOfPages / model.BlockSize;
                    if (NumberOfPages % model.BlockSize > 0)
                    {
                        NumberOfBlocks++;
                    }

                    if (model.PageNo > NumberOfPages)
                    {
                        model.PageNo = NumberOfPages;
                    }

                    if (model.PageNo <= model.BlockSize)
                    {
                        BlockNo = 1;
                    }
                    else
                    {
                        BlockNo = (int)Math.Abs(Math.Floor((decimal)model.PageNo / (decimal)model.BlockSize));
                        if (model.PageNo % model.BlockSize > 0)
                        {
                            BlockNo++;
                        }
                    }
                }

                Start = ((BlockNo - 1) * model.BlockSize) + 1;
                Stop = BlockNo * model.BlockSize;

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
                        result.Prev = model.PageNo - 1;
                        result.Next = null;
                        result.Last = null;
                    }
                    else if (BlockNo == 1)
                    {
                        result.First = null;
                        result.Prev = null;
                        result.Next = model.PageNo + 1;
                        result.Last = NumberOfPages;
                    }
                    else
                    {
                        result.First = 1;
                        result.Prev = model.PageNo - 1;
                        result.Next = model.PageNo + 1;
                        result.Last = NumberOfPages;
                    }
                }

                int Skip = (Start - 1) * model.PageSize;
                int Take = (Stop - (Start - 1)) * model.PageSize;
                List<UserSimpleItem> temps;

                if (string.IsNullOrEmpty(model.Keywords))
                {
                    switch (model.OrderBy)
                    {
                        case 0: /* by Name */
                            temps = this.userManager.Users.OrderBy(o => o.Fullname).Skip(Skip).Take(Take).Select(s => new UserSimpleItem(s.Id, s.Fullname, s.Email, s.URLAvatar, string.Empty, s.EmailConfirmed)).ToList();
                            break;
                        default: /* by Email */
                            temps = this.userManager.Users.OrderBy(o => o.Email).Skip(Skip).Take(Take).Select(s => new UserSimpleItem(s.Id, s.Fullname, s.Email, s.URLAvatar, string.Empty, s.EmailConfirmed)).ToList();
                            break;
                    }
                }
                else
                {
                    switch (model.OrderBy)
                    {
                        case 0: /* by Name */
                            temps = this.userManager.Users.Where(w=> w.Fullname.StartsWith(model.Keywords)).OrderBy(o => o.Fullname).Skip(Skip).Take(Take).Select(s => new UserSimpleItem(s.Id, s.Fullname, s.Email, s.URLAvatar, string.Empty, s.EmailConfirmed)).ToList();
                            break;
                        default: /* by Email */
                            temps = this.userManager.Users.Where(w => w.Fullname.StartsWith(model.Keywords)).OrderBy(o => o.Email).Skip(Skip).Take(Take).Select(s => new UserSimpleItem(s.Id, s.Fullname, s.Email, s.URLAvatar, string.Empty, s.EmailConfirmed)).ToList();
                            break;
                    }
                }

                foreach(UserSimpleItem item in temps)
                {
                    Task<ApplicationUser> user = this.userManager.FindByIdAsync(item.Id);
                    if(user != null)
                    {
                        Task<IList<string>> roles = this.userManager.GetRolesAsync(user.Result);
                        item.Role = string.Join(",", roles.Result);
                    }
                }

                List<UserSimpleItem> collection = new List<UserSimpleItem>();
                int count = 0;
                for (int i = 0; i < temps.Count(); i++)
                {
                    collection.Add(temps[i]);
                    count++;
                    if (count >= model.PageSize || i == temps.Count() - 1)
                    {
                        result.Users.Add(collection);
                        collection = new List<UserSimpleItem>();
                        count = 0;
                    }
                }
            }
            result.NumberOfPages = NumberOfPages;
            result.SelectedPageNo = model.PageNo;
            if (model.PageNo > result.Pages.Count)
            {
                result.SelectedIndex = (model.PageNo - ((BlockNo - 1) * model.BlockSize) - 1);
            }
            else
            {
                result.SelectedIndex = model.PageNo - 1;
            }
            return Ok(result);
        }
    }
}
