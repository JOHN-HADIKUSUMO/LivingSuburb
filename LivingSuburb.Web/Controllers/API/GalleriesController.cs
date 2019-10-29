using System;
using System.Collections.Generic;
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
using LivingSuburb.Models;
using LivingSuburb.Services;
using LivingSuburb.Web.Data;


namespace LivingSuburb.Web.Controllers.API
{
    public class GalleriesController : ControllerBase
    {
        private UserManager<ApplicationUser> userManager;
        private IHostingEnvironment hostingEnvironment;
        private IConfiguration configuration;
        private GalleryService galleryService;
        private string baseURL;
        private string baseS3URL;
        private string bucket;
        private string folder;
        private string region;
        private string accessKey;
        private string secretKey;
        private string path;

        public GalleriesController(
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration,
            CarouselService corouselService,
            UserManager<ApplicationUser> userManager,
            GalleryService galleryService
            )
        {
            this.userManager = userManager;
            this.hostingEnvironment = hostingEnvironment;
            this.configuration = configuration;
            this.galleryService = galleryService;
            baseURL = this.configuration.GetSection("BaseURL").Value;
            baseS3URL = this.configuration.GetSection("AWS:S3:Galleries:BaseURL").Value;
            bucket = this.configuration.GetSection("AWS:S3:Galleries:BucketName").Value;
            folder = this.configuration.GetSection("AWS:S3:Galleries:Folder").Value;
            region = this.configuration.GetSection("AWS:Region").Value;
            accessKey = this.configuration.GetSection("AWS:AccessKey").Value;
            secretKey = this.configuration.GetSection("AWS:SecretKey").Value;
            path = this.hostingEnvironment.WebRootPath;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Route("API/GALLERIES/DELETE/{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            AmazonS3Client client = new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.GetBySystemName(region));
            Gallery gallery = this.galleryService.Read(id);
            if(gallery == null) return BadRequest();
            bool status = this.galleryService.Delete(gallery);
            if (!status)
                return BadRequest();

            string[] temps = gallery.URL.Split("/", StringSplitOptions.RemoveEmptyEntries);
            string[] timps = temps[temps.Length - 1].Split(".", StringSplitOptions.None);
            string extension = timps[timps.Length - 1];

            DeleteObjectRequest deleteRequest = new DeleteObjectRequest();
            deleteRequest.BucketName = bucket;
            deleteRequest.Key = folder + "/" + gallery.GUID + "." + extension;
            DeleteObjectResponse deleteResponse = await client.DeleteObjectAsync(deleteRequest);

            DeleteObjectRequest deleteRequest_570_320 = new DeleteObjectRequest();
            deleteRequest_570_320.BucketName = bucket;
            deleteRequest_570_320.Key = folder + "/" + gallery.GUID + "_570_320" + "." + extension;
            DeleteObjectResponse deleteResponse_570_320 = await client.DeleteObjectAsync(deleteRequest_570_320);

            DeleteObjectRequest deleteRequest_285_160 = new DeleteObjectRequest();
            deleteRequest_285_160.BucketName = bucket;
            deleteRequest_285_160.Key = folder + "/" + gallery.GUID + "_285_160" + "." + extension;
            DeleteObjectResponse deleteResponse_285_160 = await client.DeleteObjectAsync(deleteRequest_285_160);

            DeleteObjectRequest deleteRequest_114_64 = new DeleteObjectRequest();
            deleteRequest_114_64.BucketName = bucket;
            deleteRequest_114_64.Key = folder + "/" + gallery.GUID + "_114_64" + "." + extension;
            DeleteObjectResponse deleteResponse_114_64 = await client.DeleteObjectAsync(deleteRequest_114_64);

            DeleteObjectRequest deleteRequest_57_32 = new DeleteObjectRequest();
            deleteRequest_57_32.BucketName = bucket;
            deleteRequest_57_32.Key = folder + "/" + gallery.GUID + "_57_32" + "." + extension;
            DeleteObjectResponse deleteResponse_57_32 = await client.DeleteObjectAsync(deleteRequest_57_32);

            return Ok();
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Route("API/GALLERIES/READ/{id}")]
        public IActionResult Read([FromRoute] int id)
        {
            Gallery gallery = this.galleryService.Read(id);
            if(gallery == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(new { Id = gallery.GalleryId, URL = this.galleryService.GetImageURL(gallery.URL, 570,320), Suburb = gallery.Suburb, State = gallery.State, Note = gallery.Note });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/GALLERIES/SEARCH")]
        public IActionResult Search([FromBody] GallerySearch model)
        {
            GalleryListResult result = this.galleryService.Search(model.Keywords, model.OrderBy, model.PageNo, model.PageSize, model.BlockSize);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("API/GALLERIES/UPDATE")]
        public async Task<IActionResult> Update([FromBody]UpdateGallery model)
        {
            Gallery gallery = this.galleryService.Read(model.Id);
            if (gallery == null)
            {
                return BadRequest();
            }
            if(!string.IsNullOrEmpty(model.GUID))
            {
                PutObjectRequest putRequest = new PutObjectRequest();
                putRequest.BucketName = bucket;
                putRequest.Key = folder + "/" + model.GUID + model.Extension;
                putRequest.FilePath = path + @"\temporary\" + model.GUID + model.Extension;
                putRequest.CannedACL = S3CannedACL.PublicRead;

                PutObjectRequest putRequest_570_320 = new PutObjectRequest();
                putRequest_570_320.BucketName = bucket;
                putRequest_570_320.Key = folder + "/" + model.GUID + "_570_320" + model.Extension;
                putRequest_570_320.FilePath = path + @"\temporary\" + model.GUID + "_570_320" + model.Extension;
                putRequest_570_320.CannedACL = S3CannedACL.PublicRead;

                PutObjectRequest putRequest_285_160 = new PutObjectRequest();
                putRequest_285_160.BucketName = bucket;
                putRequest_285_160.Key = folder + "/" + model.GUID + "_285_160" + model.Extension;
                putRequest_285_160.FilePath = path + @"\temporary\" + model.GUID + "_285_160" + model.Extension;
                putRequest_285_160.CannedACL = S3CannedACL.PublicRead;

                PutObjectRequest putRequest_114_64 = new PutObjectRequest();
                putRequest_114_64.BucketName = bucket;
                putRequest_114_64.Key = folder + "/" + model.GUID + "_114_64" + model.Extension;
                putRequest_114_64.FilePath = path + @"\temporary\" + model.GUID + "_114_64" + model.Extension;
                putRequest_114_64.CannedACL = S3CannedACL.PublicRead;

                PutObjectRequest putRequest_57_32 = new PutObjectRequest();
                putRequest_57_32.BucketName = bucket;
                putRequest_57_32.Key = folder + "/" + model.GUID + "_57_32" + model.Extension;
                putRequest_57_32.FilePath = path + @"\temporary\" + model.GUID + "_57_32" + model.Extension;
                putRequest_57_32.CannedACL = S3CannedACL.PublicRead;

                AmazonS3Client client = new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.GetBySystemName(region));
                try
                {
                    PutObjectResponse putResponse = await client.PutObjectAsync(putRequest);
                    PutObjectResponse putResponse_570_320 = await client.PutObjectAsync(putRequest_570_320);
                    PutObjectResponse putResponse_285_160 = await client.PutObjectAsync(putRequest_285_160);
                    PutObjectResponse putResponse_114_64 = await client.PutObjectAsync(putRequest_114_64);
                    PutObjectResponse putResponse_57_32 = await client.PutObjectAsync(putRequest_57_32);

                    if (putResponse.HttpStatusCode == HttpStatusCode.OK && putResponse_570_320.HttpStatusCode == HttpStatusCode.OK && putResponse_285_160.HttpStatusCode == HttpStatusCode.OK && putResponse_114_64.HttpStatusCode == HttpStatusCode.OK && putResponse_57_32.HttpStatusCode == HttpStatusCode.OK)
                    {
                        bool status = await Task.Run(() =>
                        {
                            gallery.GUID = model.GUID;
                            gallery.Filename = model.Filename;
                            gallery.URL = baseS3URL + "/" + model.GUID + model.Extension;
                            gallery.Suburb = model.Suburb;
                            gallery.State = model.State;
                            gallery.Note = model.Note;
                            return this.galleryService.Update(gallery);
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

                            DeleteObjectRequest deleteRequest_570_320 = new DeleteObjectRequest()
                            {
                                BucketName = bucket + folder,
                                Key = folder + "/" + model.GUID + "_570_320" + model.Extension
                            };
                            DeleteObjectResponse deleteResponse_570_320 = await client.DeleteObjectAsync(deleteRequest_570_320);

                            DeleteObjectRequest deleteRequest_285_160 = new DeleteObjectRequest()
                            {
                                BucketName = bucket + folder,
                                Key = folder + "/" + model.GUID + "_285_160" + model.Extension
                            };
                            DeleteObjectResponse deleteResponse_285_160 = await client.DeleteObjectAsync(deleteRequest_285_160);

                            DeleteObjectRequest deleteRequest_114_64 = new DeleteObjectRequest()
                            {
                                BucketName = bucket + folder,
                                Key = folder + "/" + model.GUID + "_114_64" + model.Extension
                            };
                            DeleteObjectResponse deleteResponse_114_64 = await client.DeleteObjectAsync(deleteRequest_114_64);

                            DeleteObjectRequest deleteRequest_57_32 = new DeleteObjectRequest()
                            {
                                BucketName = bucket + folder,
                                Key = folder + "/" + model.GUID + "_57_32" + model.Extension
                            };
                            DeleteObjectResponse deleteResponse_57_32 = await client.DeleteObjectAsync(deleteRequest_57_32);

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
                    gallery.Suburb = model.Suburb;
                    gallery.State = model.State;
                    gallery.Note = model.Note;
                    return this.galleryService.Update(gallery);
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
        [Route("API/GALLERIES/CREATE")]
        public async Task<IActionResult> Create([FromBody]CreateGallery model)
        {
            PutObjectRequest putRequest = new PutObjectRequest();
            putRequest.BucketName = bucket;
            putRequest.Key = folder + "/" + model.GUID + model.Extension;
            putRequest.FilePath = path + @"\temporary\" + model.GUID + model.Extension;
            putRequest.CannedACL = S3CannedACL.PublicRead;

            PutObjectRequest putRequest_570_320 = new PutObjectRequest();
            putRequest_570_320.BucketName = bucket;
            putRequest_570_320.Key = folder + "/" + model.GUID + "_570_320" + model.Extension;
            putRequest_570_320.FilePath = path + @"\temporary\" + model.GUID + "_570_320" + model.Extension;
            putRequest_570_320.CannedACL = S3CannedACL.PublicRead;

            PutObjectRequest putRequest_285_160 = new PutObjectRequest();
            putRequest_285_160.BucketName = bucket;
            putRequest_285_160.Key = folder + "/" + model.GUID + "_285_160" + model.Extension;
            putRequest_285_160.FilePath = path + @"\temporary\" + model.GUID + "_285_160" + model.Extension;
            putRequest_285_160.CannedACL = S3CannedACL.PublicRead;

            PutObjectRequest putRequest_114_64 = new PutObjectRequest();
            putRequest_114_64.BucketName = bucket;
            putRequest_114_64.Key = folder + "/" + model.GUID + "_114_64" + model.Extension;
            putRequest_114_64.FilePath = path + @"\temporary\" + model.GUID + "_114_64" + model.Extension;
            putRequest_114_64.CannedACL = S3CannedACL.PublicRead;

            PutObjectRequest putRequest_57_32 = new PutObjectRequest();
            putRequest_57_32.BucketName = bucket;
            putRequest_57_32.Key = folder + "/" + model.GUID + "_57_32" + model.Extension;
            putRequest_57_32.FilePath = path + @"\temporary\" + model.GUID + "_57_32" + model.Extension;
            putRequest_57_32.CannedACL = S3CannedACL.PublicRead;

            AmazonS3Client client = new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.GetBySystemName(region));
            try
            {
                PutObjectResponse putResponse = await client.PutObjectAsync(putRequest);
                PutObjectResponse putResponse_570_320 = await client.PutObjectAsync(putRequest_570_320);
                PutObjectResponse putResponse_285_160 = await client.PutObjectAsync(putRequest_285_160);
                PutObjectResponse putResponse_114_64 = await client.PutObjectAsync(putRequest_114_64);
                PutObjectResponse putResponse_57_32 = await client.PutObjectAsync(putRequest_57_32);

                if (putResponse.HttpStatusCode == HttpStatusCode.OK && putResponse_570_320.HttpStatusCode == HttpStatusCode.OK && putResponse_285_160.HttpStatusCode == HttpStatusCode.OK && putResponse_114_64.HttpStatusCode == HttpStatusCode.OK && putResponse_57_32.HttpStatusCode == HttpStatusCode.OK)
                {
                    bool status = await Task.Run(() =>
                    {
                        Gallery gallery = new Gallery();
                        gallery.GUID = model.GUID;
                        gallery.Filename = model.Filename;
                        gallery.URL = baseS3URL + "/" + model.GUID + model.Extension;
                        gallery.Suburb = model.Suburb;
                        gallery.State = model.State;
                        gallery.Note = model.Note;
                        return this.galleryService.Create(gallery);
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

                        DeleteObjectRequest deleteRequest_570_320 = new DeleteObjectRequest()
                        {
                            BucketName = bucket + folder,
                            Key = folder + "/" + model.GUID + "_570_320" + model.Extension
                        };
                        DeleteObjectResponse deleteResponse_570_320 = await client.DeleteObjectAsync(deleteRequest_570_320);

                        DeleteObjectRequest deleteRequest_285_160 = new DeleteObjectRequest()
                        {
                            BucketName = bucket + folder,
                            Key = folder + "/" + model.GUID + "_285_160" + model.Extension
                        };
                        DeleteObjectResponse deleteResponse_285_160 = await client.DeleteObjectAsync(deleteRequest_285_160);

                        DeleteObjectRequest deleteRequest_114_64 = new DeleteObjectRequest()
                        {
                            BucketName = bucket + folder,
                            Key = folder + "/" + model.GUID + "_114_64" + model.Extension
                        };
                        DeleteObjectResponse deleteResponse_114_64 = await client.DeleteObjectAsync(deleteRequest_114_64);

                        DeleteObjectRequest deleteRequest_57_32 = new DeleteObjectRequest()
                        {
                            BucketName = bucket + folder,
                            Key = folder + "/" + model.GUID + "_57_32" + model.Extension
                        };
                        DeleteObjectResponse deleteResponse_57_32 = await client.DeleteObjectAsync(deleteRequest_57_32);

                        return new StatusCodeResult(500);
                    }
                }
            }
            catch(Exception ex)
            {
                string errors = ex.GetBaseException().ToString();
            }
            client.Dispose();
            return new StatusCodeResult(500);
        }


        [HttpPost, DisableRequestSizeLimit]
        [Authorize(Roles = "Administrator")]
        [Route("API/GALLERIES/UPLOAD")]
        public async Task<IActionResult> Upload()
        {
            ApplicationUser user = await this.userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null) return new BadRequestResult();
            if (Request.Form.Files.Count == 1)
            {
                string baseURL = this.configuration.GetSection("BaseURL").Value;
                string path = this.hostingEnvironment.WebRootPath;
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
                string _570_320 = guid + "_570_320" + extension;
                string _285_160 = guid + "_285_160" + extension;
                string _114_64 = guid + "_114_64" + extension;
                string _57_32 = guid + "_57_32" + extension;
                FileStream fileStream = new FileStream(path + "/temporary/" + filename, FileMode.Create, FileAccess.Write);
                Request.Form.Files[0].CopyTo(fileStream);
                fileStream.Close();

                Image<Rgba32> image = Image.Load(path + "/temporary/" + filename);
    
                if (image.Width > 1425 || image.Height > 800)
                    return BadRequest("Image must be 1425 x 800 pixels.");
                else
                {
                    image.Mutate(x => x.Resize(570, 320));
                    image.Save(path + "/temporary/" + _570_320);

                    image.Mutate(x => x.Resize(285, 120));
                    image.Save(path + "/temporary/" + _285_160);

                    image.Mutate(x => x.Resize(114, 64));
                    image.Save(path + "/temporary/" + _114_64);

                    image.Mutate(x => x.Resize(57, 32));
                    image.Save(path + "/temporary/" + _57_32);

                    return Ok(new {
                        URL = baseURL + "/temporary/" + _570_320,
                        GUID = guid,
                        Filename = originalFilename,
                        Extension = extension
                    });
                }
            }
            return BadRequest("Only one image that can be uploaded.");
        }
    }
}
