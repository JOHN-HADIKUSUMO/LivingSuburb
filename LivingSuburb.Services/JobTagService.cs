using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using LivingSuburb.Models;
using LivingSuburb.Database;

namespace LivingSuburb.Services
{
    public class JobTagService
    {
        private DataContext datacontext;
        public JobTagService(DataContext context)
        {
            datacontext = context;
        }

        public List<JobTag> ReadAllByJobId(int id)
        {
            return datacontext.JobTags.Where(w=> w.JobId == id).ToList();
        }

        public bool Create(JobTag model)
        {
            bool status = false;
            if (!datacontext.JobTags.Where(w=> w.JobId == model.JobId && w.TagId == model.TagId).Any())
            {
                datacontext.JobTags.Add(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Update(JobTag model)
        {
            bool status = false;
            if (datacontext.JobTags.Where(w => w.JobId == model.JobId && w.TagId == model.TagId).Any())
            {
                datacontext.JobTags.Update(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Delete(JobTag model)
        {
            bool status = false;
            if (datacontext.JobTags.Where(w => w.JobId == model.JobId && w.TagId == model.TagId).Any())
            {
                datacontext.JobTags.Remove(model);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Delete(int jobId)
        {
            bool status = false;
            JobTag jobTag = datacontext.JobTags.Where(w => w.JobId == jobId).FirstOrDefault();
            if(jobTag != null)
            {
                datacontext.JobTags.Remove(jobTag);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }

        public bool Delete(int jobId,int tagId)
        {
            bool status = false;
            JobTag jobTag = datacontext.JobTags.Where(w => w.JobId == jobId && w.TagId == tagId).FirstOrDefault();
            if (jobTag != null)
            {
                datacontext.JobTags.Remove(jobTag);
                datacontext.SaveChanges();
                status = true;
            }
            return status;
        }
    }
}
