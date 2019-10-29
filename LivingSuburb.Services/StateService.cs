using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using LivingSuburb.Models;
using LivingSuburb.Database;

namespace LivingSuburb.Services
{
    public class StateService
    {
        private DataContext datacontext;
        public StateService(DataContext context)
        {
            datacontext = context;
        }

        public State Read(int id)
        {
            return datacontext.States.Where(w => w.StateId == id).FirstOrDefault();
        }

        public string GetMyState()
        {
            return "NSW";
        }
        public List<State> Search(string keyword)
        {
            return datacontext.States.Where(w => w.LongName.StartsWith(keyword)).ToList();
        }

        public List<State> GetAll()
        {
            return datacontext.States.OrderBy(o=> o.ShortName).ToList();
        }

        public int Add(State model)
        {
            datacontext.States.Add(model);
            datacontext.SaveChanges();
            return model.StateId;
        }

        public bool Update(State model)
        {
            bool status = true;
            try
            {
                datacontext.States.Update(model);
                datacontext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                status = false;
            }

            return status;
        }

        public bool Delete(State model)
        {
            bool status = true;
            try
            {
                datacontext.States.Remove(model);
                datacontext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                status = false;
            }

            return status;
        }
    }
}
