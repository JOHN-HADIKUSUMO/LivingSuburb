using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Diagnostics;
using System.Security.Claims;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using Amazon.S3;
using Amazon.S3.Model;
using LivingSuburb.Web.Data;
using LivingSuburb.Models;
using LivingSuburb.Services;

namespace LivingSuburb.Web.Controllers.API
{
    public class AvatarController : ControllerBase
    {
        private UserManager<ApplicationUser> userManager;
        private IHostingEnvironment hostingEnvironment;
        private IConfiguration configuration;
        private string baseURL;
        private string baseS3URL;
        private string bucket;
        private string folder;
        private string region;
        private string accessKey;
        private string secretKey;
        private string path;
        public AvatarController(
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration, 
            UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.hostingEnvironment = hostingEnvironment;
            baseURL = this.configuration.GetSection("BaseURL").Value;
            baseS3URL = this.configuration.GetSection("AWS:S3:Avatars:BaseURL").Value;
            bucket = this.configuration.GetSection("AWS:S3:Avatars:BucketName").Value;
            folder = this.configuration.GetSection("AWS:S3:Avatars:Folder").Value;
            region = this.configuration.GetSection("AWS:Region").Value;
            accessKey = this.configuration.GetSection("AWS:AccessKey").Value;
            secretKey = this.configuration.GetSection("AWS:SecretKey").Value;
            path = this.hostingEnvironment.WebRootPath;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,User")]
        [Route("API/AVATAR/READ")]
        public async Task<IActionResult> Read()
        {
            string baseURL = this.configuration.GetSection("BaseURL").Value;
            ApplicationUser user = await this.userManager.FindByEmailAsync(User.Identity.Name);
            return Ok(string.IsNullOrEmpty(user.URLAvatar) ? (baseURL + "/Avatars/default.jpg") : user.URLAvatar);
        }

        [HttpPost, DisableRequestSizeLimit]
        [Authorize(Roles = "Administrator,User")]
        [Route("API/AVATAR/UPDATE")]
        public async Task<IActionResult> Update([FromBody]URLModel model)
        {
            ApplicationUser user = await this.userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null) return new BadRequestResult();

            PutObjectRequest putRequest = new PutObjectRequest();
            putRequest.BucketName = bucket;
            putRequest.Key = folder + "/" + model.GUID + model.Extension;
            putRequest.FilePath = path + @"\temporary\" + model.GUID + model.Extension;
            putRequest.CannedACL = S3CannedACL.PublicRead;

            AmazonS3Client client = new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.GetBySystemName(region));
            try
            {
                PutObjectResponse putResponse = await client.PutObjectAsync(putRequest);
                if(putResponse.HttpStatusCode == HttpStatusCode.OK)
                {
                    user.URLAvatar = baseS3URL + "/" + model.GUID + model.Extension; 
                    IdentityResult updateResult = await this.userManager.UpdateAsync(user);
                    if (updateResult.Succeeded)
                    {
                        if (User.Claims.Where(w => w.Type == "Avatar").Any())
                        {
                            List<Claim> oldClaims = User.Claims.Where(w => w.Type == "Avatar").ToList();
                            for (int i = 0; i < oldClaims.Count; i++)
                            {
                                IdentityResult removalResult = await this.userManager.RemoveClaimAsync(user, oldClaims[i]);
                            }
                        }
                        Claim newClaim = new Claim("Avatar", user.URLAvatar);
                        IdentityResult addnewResult = await this.userManager.AddClaimAsync(user, newClaim);
                        if (addnewResult.Succeeded) return Ok(true);
                    }
                }
            }
            catch (Exception ex)
            {
                string errors = ex.GetBaseException().ToString();
            }
            client.Dispose();

            return new StatusCodeResult(500);
        }

        [HttpPost, DisableRequestSizeLimit]
        [Authorize(Roles = "Administrator,User")]
        [Route("API/AVATAR/UPLOAD")]
        public async Task<IActionResult> Upload()
        {
            ApplicationUser user = await this.userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null) return new BadRequestResult();
            if (Request.Form.Files.Count == 1)
            {
                string baseURL = this.configuration.GetSection("BaseURL").Value;
                string path = this.hostingEnvironment.WebRootPath;
                string originalFilename = Request.Form.Files[0].FileName;
                string contentType = Request.Form.Files[0].ContentType;
                string extension = string.Empty;
                switch (contentType)
                {
                    case "image/jpg":
                        extension = ".jpg";
                        break;
                    case "image/jpeg":
                        extension = ".jpeg";
                        break;
                    case "image/gif":
                        extension = ".gif";
                        break;
                    case "image/png":
                        extension = ".png";
                        break;
                }
                if (string.IsNullOrEmpty(extension)) return BadRequest("Only jpg,jpeg,png and gif format is allowed.");
                string guid = Guid.NewGuid().ToString();
                string filename = guid + extension;
                FileStream fileStream = new FileStream(path + "/temporary/" + filename, FileMode.Create, FileAccess.Write);
                Request.Form.Files[0].CopyTo(fileStream);
                fileStream.Close();

                Image<Rgba32> image = Image.Load(path + "/temporary/" + filename);
                if (image.Width > 400 || image.Height > 400)
                    return BadRequest("Image must be 400 x 400 pixels.");
                else
                    return Ok(new
                    {
                        URL = baseURL + "/temporary/" + filename,
                        GUID = guid,
                        Filename = originalFilename,
                        Extension = extension
                    });
            }
            return BadRequest("Only one image that can be uploaded.");
        }
    }
}
