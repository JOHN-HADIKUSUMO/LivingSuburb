using System.Diagnostics;
using System.Linq;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Extensions.NETCore.Setup;
using System;
using System.Collections.Generic;
using LivingSuburb.Models;
using LivingSuburb.Services;
using LivingSuburb.Web.Data;

namespace LivingSuburb.Web.Controllers.API
{
    public class CarouselController : ControllerBase
    {
        private UserManager<ApplicationUser> userManager;
        private IHostingEnvironment hostingEnvironment;
        private IConfiguration configuration;
        private CarouselService carouselService;
        private string baseURL;
        private string baseS3URL;
        private string bucket;
        private string folder;
        private string region;
        private string accessKey;
        private string secretKey;
        private string path;
        public CarouselController(
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration,
            CarouselService corouselService,
            UserManager<ApplicationUser> userManager
            )
        {
            this.userManager = userManager;
            this.hostingEnvironment = hostingEnvironment;
            this.configuration = configuration;
            this.carouselService = corouselService;
            baseURL = this.configuration.GetSection("BaseURL").Value;
            baseS3URL = this.configuration.GetSection("AWS:S3:Carousels:BaseURL").Value;
            bucket = this.configuration.GetSection("AWS:S3:Carousels:BucketName").Value;
            folder = this.configuration.GetSection("AWS:S3:Carousels:Folder").Value;
            region = this.configuration.GetSection("AWS:Region").Value;
            accessKey = this.configuration.GetSection("AWS:AccessKey").Value;
            secretKey = this.configuration.GetSection("AWS:SecretKey").Value;
            path = this.hostingEnvironment.WebRootPath;
        }

        [HttpPost, DisableRequestSizeLimit]
        [Authorize(Roles = "Administrator")]
        [Route("API/CAROUSELS/UPLOAD")]
        public async Task<IActionResult> Upload()
        {
            ApplicationUser user = await this.userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null) return new BadRequestResult();
            if (Request.Form.Files.Count == 1)
            {
                string contentType = Request.Form.Files[0].ContentType;
                string originalFilename = Request.Form.Files[0].FileName;
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
                string _570_240 = guid + "_570_240" + extension;
                string _285_120 = guid + "_285_120" + extension;
                string _114_48 = guid + "_114_48" + extension;
                string _57_24 = guid + "_57_24" + extension;
                FileStream fileStream = new FileStream(path + "/temporary/" + filename, FileMode.Create, FileAccess.Write);
                Request.Form.Files[0].CopyTo(fileStream);
                fileStream.Close();

                Image<Rgba32> image = Image.Load(path + "/temporary/" + filename);

                if (image.Width > 1425 || image.Height > 600)
                    return BadRequest("Image must be 1425 x 600 pixels.");
                else
                {
                    image.Mutate(x => x.Resize(570, 240));
                    image.Save(path + "/temporary/" + _570_240);

                    image.Mutate(x => x.Resize(285, 120));
                    image.Save(path + "/temporary/" + _285_120);

                    image.Mutate(x => x.Resize(114, 64));
                    image.Save(path + "/temporary/" + _114_48);

                    image.Mutate(x => x.Resize(57, 24));
                    image.Save(path + "/temporary/" + _57_24);

                    return Ok(new
                    {
                        URL = baseURL + "/temporary/" + _570_240,
                        GUID = guid,
                        Filename = originalFilename,
                        Extension = extension
                    });
                }
            }
            return BadRequest("Only one image that can be uploaded.");
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Route("API/CAROUSELS/READ/{id}")]
        public IActionResult Read(int id)
        {
            Carousel carousel = this.carouselService.Read(id);
            if (carousel != null)
                return Ok(new { Id = carousel.CarouselId, URL = this.carouselService.GetImageURL(carousel.ImageURL, 570, 240), carousel.Location, carousel.Proverb, carousel.Source, PublishedDate = string.Format("{0:yyyy-MM-dd}",carousel.PublishedDate)});
            else
                return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/CAROUSELS/DELETE")]
        public IActionResult Delete([FromBody] DeleteCarousel model)
        {
            bool status = this.carouselService.Delete(model.Id);
            return Ok(status);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/CAROUSELS/CREATE")]
        public async Task<IActionResult> Create([FromBody] CreateCarousel model)
        {
            PutObjectRequest putRequest = new PutObjectRequest();
            putRequest.BucketName = bucket;
            putRequest.Key = folder + "/" + model.GUID + model.Extension;
            putRequest.FilePath = path + @"\temporary\" + model.GUID + model.Extension;
            putRequest.CannedACL = S3CannedACL.PublicRead;

            PutObjectRequest putRequest_570_240 = new PutObjectRequest();
            putRequest_570_240.BucketName = bucket;
            putRequest_570_240.Key = folder + "/" + model.GUID + "_570_240" + model.Extension;
            putRequest_570_240.FilePath = path + @"\temporary\" + model.GUID + "_570_240" + model.Extension;
            putRequest_570_240.CannedACL = S3CannedACL.PublicRead;

            PutObjectRequest putRequest_285_120 = new PutObjectRequest();
            putRequest_285_120.BucketName = bucket;
            putRequest_285_120.Key = folder + "/" + model.GUID + "_285_120" + model.Extension;
            putRequest_285_120.FilePath = path + @"\temporary\" + model.GUID + "_285_120" + model.Extension;
            putRequest_285_120.CannedACL = S3CannedACL.PublicRead;

            PutObjectRequest putRequest_114_48 = new PutObjectRequest();
            putRequest_114_48.BucketName = bucket;
            putRequest_114_48.Key = folder + "/" + model.GUID + "_114_48" + model.Extension;
            putRequest_114_48.FilePath = path + @"\temporary\" + model.GUID + "_114_48" + model.Extension;
            putRequest_114_48.CannedACL = S3CannedACL.PublicRead;

            PutObjectRequest putRequest_57_24 = new PutObjectRequest();
            putRequest_57_24.BucketName = bucket;
            putRequest_57_24.Key = folder + "/" + model.GUID + "_57_24" + model.Extension;
            putRequest_57_24.FilePath = path + @"\temporary\" + model.GUID + "_57_24" + model.Extension;
            putRequest_57_24.CannedACL = S3CannedACL.PublicRead;

            AmazonS3Client client = new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.GetBySystemName(region));
            try
            {
                PutObjectResponse putResponse = await client.PutObjectAsync(putRequest);
                PutObjectResponse putResponse_570_240 = await client.PutObjectAsync(putRequest_570_240);
                PutObjectResponse putResponse_285_120 = await client.PutObjectAsync(putRequest_285_120);
                PutObjectResponse putResponse_114_48 = await client.PutObjectAsync(putRequest_114_48);
                PutObjectResponse putResponse_57_24 = await client.PutObjectAsync(putRequest_57_24);

                if (putResponse.HttpStatusCode == HttpStatusCode.OK && putResponse_570_240.HttpStatusCode == HttpStatusCode.OK && putResponse_285_120.HttpStatusCode == HttpStatusCode.OK && putResponse_114_48.HttpStatusCode == HttpStatusCode.OK && putResponse_57_24.HttpStatusCode == HttpStatusCode.OK)
                {
                    bool status = await Task.Run(() =>
                    {
                        Carousel carousel = new Carousel();
                        carousel.ImageURL = baseS3URL + "/" + model.GUID + model.Extension;
                        carousel.Location = model.Location;
                        carousel.Proverb = model.Proverb;
                        carousel.Source = model.Source;
                        carousel.PublishedDate = model.PublishedDate;
                        return this.carouselService.Create(carousel); 
                    });
                    if (status)
                    {
                        return Ok(status);
                    }
                    else
                    {
                        DeleteObjectRequest deleteRequest = new DeleteObjectRequest()
                        {
                            BucketName = bucket + folder,
                            Key = folder + "/" + model.GUID + model.Extension
                        };
                        DeleteObjectResponse deleteResponse = await client.DeleteObjectAsync(deleteRequest);

                        DeleteObjectRequest deleteRequest_570_240 = new DeleteObjectRequest()
                        {
                            BucketName = bucket + folder,
                            Key = folder + "/" + model.GUID + "_570_240" + model.Extension
                        };
                        DeleteObjectResponse deleteResponse_570_240 = await client.DeleteObjectAsync(deleteRequest_570_240);

                        DeleteObjectRequest deleteRequest_285_120 = new DeleteObjectRequest()
                        {
                            BucketName = bucket + folder,
                            Key = folder + "/" + model.GUID + "_285_120" + model.Extension
                        };
                        DeleteObjectResponse deleteResponse_285_120 = await client.DeleteObjectAsync(deleteRequest_285_120);

                        DeleteObjectRequest deleteRequest_114_48 = new DeleteObjectRequest()
                        {
                            BucketName = bucket + folder,
                            Key = folder + "/" + model.GUID + "_114_48" + model.Extension
                        };
                        DeleteObjectResponse deleteResponse_114_48 = await client.DeleteObjectAsync(deleteRequest_114_48);

                        DeleteObjectRequest deleteRequest_57_24 = new DeleteObjectRequest()
                        {
                            BucketName = bucket + folder,
                            Key = folder + "/" + model.GUID + "_57_24" + model.Extension
                        };
                        DeleteObjectResponse deleteResponse_57_24 = await client.DeleteObjectAsync(deleteRequest_57_24);

                        return new StatusCodeResult(500);
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

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/CAROUSELS/UPDATE")]
        public async Task<IActionResult> Update([FromBody] UpdateCarousel model)
        {
            Carousel carousel = this.carouselService.Read(model.Id);
            if (carousel == null)
            {
                return BadRequest();
            }
            if (!string.IsNullOrEmpty(model.GUID))
            {

                PutObjectRequest putRequest = new PutObjectRequest();
                putRequest.BucketName = bucket;
                putRequest.Key = folder + "/" + model.GUID + model.Extension;
                putRequest.FilePath = path + @"\temporary\" + model.GUID + model.Extension;
                putRequest.CannedACL = S3CannedACL.PublicRead;

                PutObjectRequest putRequest_570_240 = new PutObjectRequest();
                putRequest_570_240.BucketName = bucket;
                putRequest_570_240.Key = folder + "/" + model.GUID + "_570_240" + model.Extension;
                putRequest_570_240.FilePath = path + @"\temporary\" + model.GUID + "_570_240" + model.Extension;
                putRequest_570_240.CannedACL = S3CannedACL.PublicRead;

                PutObjectRequest putRequest_285_120 = new PutObjectRequest();
                putRequest_285_120.BucketName = bucket;
                putRequest_285_120.Key = folder + "/" + model.GUID + "_285_120" + model.Extension;
                putRequest_285_120.FilePath = path + @"\temporary\" + model.GUID + "_285_120" + model.Extension;
                putRequest_285_120.CannedACL = S3CannedACL.PublicRead;

                PutObjectRequest putRequest_114_48 = new PutObjectRequest();
                putRequest_114_48.BucketName = bucket;
                putRequest_114_48.Key = folder + "/" + model.GUID + "_114_48" + model.Extension;
                putRequest_114_48.FilePath = path + @"\temporary\" + model.GUID + "_114_48" + model.Extension;
                putRequest_114_48.CannedACL = S3CannedACL.PublicRead;

                PutObjectRequest putRequest_57_24 = new PutObjectRequest();
                putRequest_57_24.BucketName = bucket;
                putRequest_57_24.Key = folder + "/" + model.GUID + "_57_24" + model.Extension;
                putRequest_57_24.FilePath = path + @"\temporary\" + model.GUID + "_57_24" + model.Extension;
                putRequest_57_24.CannedACL = S3CannedACL.PublicRead;

                AmazonS3Client client = new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.GetBySystemName(region));
                try
                {
                    PutObjectResponse putResponse = await client.PutObjectAsync(putRequest);
                    PutObjectResponse putResponse_570_240 = await client.PutObjectAsync(putRequest_570_240);
                    PutObjectResponse putResponse_285_120 = await client.PutObjectAsync(putRequest_285_120);
                    PutObjectResponse putResponse_114_48 = await client.PutObjectAsync(putRequest_114_48);
                    PutObjectResponse putResponse_57_24 = await client.PutObjectAsync(putRequest_57_24);

                    if (putResponse.HttpStatusCode == HttpStatusCode.OK && putResponse_570_240.HttpStatusCode == HttpStatusCode.OK && putResponse_285_120.HttpStatusCode == HttpStatusCode.OK && putResponse_114_48.HttpStatusCode == HttpStatusCode.OK && putResponse_57_24.HttpStatusCode == HttpStatusCode.OK)
                    {
                        bool status = await Task.Run(() =>
                        {
                            carousel.ImageURL = baseS3URL + "/" + model.GUID + model.Extension;
                            carousel.Location = model.Location;
                            carousel.Proverb = model.Proverb;
                            carousel.Source = model.Source;
                            carousel.PublishedDate = model.PublishedDate;

                            return this.carouselService.Update(carousel);
                        });
                        if (status)
                        {
                            return Ok(status);
                        }
                        else
                        {
                            DeleteObjectRequest deleteRequest = new DeleteObjectRequest()
                            {
                                BucketName = bucket + folder,
                                Key = folder + "/" + model.GUID + model.Extension
                            };
                            DeleteObjectResponse deleteResponse = await client.DeleteObjectAsync(deleteRequest);

                            DeleteObjectRequest deleteRequest_570_240 = new DeleteObjectRequest()
                            {
                                BucketName = bucket + folder,
                                Key = folder + "/" + model.GUID + "_570_240" + model.Extension
                            };
                            DeleteObjectResponse deleteResponse_570_240 = await client.DeleteObjectAsync(deleteRequest_570_240);

                            DeleteObjectRequest deleteRequest_285_120 = new DeleteObjectRequest()
                            {
                                BucketName = bucket + folder,
                                Key = folder + "/" + model.GUID + "_285_120" + model.Extension
                            };
                            DeleteObjectResponse deleteResponse_285_120 = await client.DeleteObjectAsync(deleteRequest_285_120);

                            DeleteObjectRequest deleteRequest_114_48 = new DeleteObjectRequest()
                            {
                                BucketName = bucket + folder,
                                Key = folder + "/" + model.GUID + "_114_48" + model.Extension
                            };
                            DeleteObjectResponse deleteResponse_114_48 = await client.DeleteObjectAsync(deleteRequest_114_48);

                            DeleteObjectRequest deleteRequest_57_24 = new DeleteObjectRequest()
                            {
                                BucketName = bucket + folder,
                                Key = folder + "/" + model.GUID + "_57_24" + model.Extension
                            };
                            DeleteObjectResponse deleteResponse_57_24 = await client.DeleteObjectAsync(deleteRequest_57_24);
                            return new StatusCodeResult(500);
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
            else
            {
                bool status = await Task.Run(() =>
                {
                    carousel.ImageURL = baseS3URL + "/" + model.GUID + model.Extension;
                    carousel.Location = model.Location;
                    carousel.Proverb = model.Proverb;
                    carousel.Source = model.Source;
                    carousel.PublishedDate = model.PublishedDate;

                    return this.carouselService.Update(carousel);
                });
                if (status)
                {
                    return Ok(status);
                }
                else
                {
                    return BadRequest();
                }
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/CAROUSELS/SEARCH2")]
        public IActionResult Search2([FromBody] JobSearch2 model)
        {
            SearchCarouselListResult result = this.carouselService.Search2(model.Keywords, model.PageNo, model.PageSize, model.BlockSize);
            return Ok(result);
        }
    }
}
