using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using System.Collections.Generic;
using SigimeraModel.CrisisModel;

namespace SigimeraModel
{
    public class DataCommunication
    {

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public static IQueryable<RootObject> LoadCrisis(int numberOfEvents)
        {
            using (SigimeraDataContext context = new SigimeraDataContext(Shared.CONNECTION_STRING))
            {
                IQueryable<RootObject> query = from s in context.CrisisItems select s;
                return query;
            }
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public static List<RootObject> GetCrisis(int elemntsCountToRetrieve)
        {
            List<RootObject> lstCategories = new List<RootObject>();
            using (SigimeraDataContext context = new SigimeraDataContext(Shared.CONNECTION_STRING))
            {
                IQueryable<RootObject> query = from s in context.CrisisItems select s;
                lstCategories = query.Take(elemntsCountToRetrieve).ToList();
            }
            return lstCategories;
        }

        public static RootObject GetCrisis(string id)
        {
            RootObject earthquakeItemViewModel = null;
            using (SigimeraDataContext context = new SigimeraDataContext(Shared.CONNECTION_STRING))
            {
                IQueryable<RootObject> query = from s in context.CrisisItems where s._id == id select s;
                earthquakeItemViewModel = query.FirstOrDefault();
            }
            return earthquakeItemViewModel;
        }

        public static bool CrisisExists(string id)
        {
            RootObject earthquakeItemViewModel = null;
            using (SigimeraDataContext context = new SigimeraDataContext(Shared.CONNECTION_STRING))
            {
                IQueryable<RootObject> query = from s in context.CrisisItems where s._id == id select s;
                earthquakeItemViewModel = query.FirstOrDefault();

                if (earthquakeItemViewModel != null)
                {
                    return true;
                }
            }
            return false;
        }

        public static void AddCrisis(RootObject earthquakeEvent)
        {
            using (SigimeraDataContext context = new SigimeraDataContext(Shared.CONNECTION_STRING))
            {
                context.CrisisItems.InsertOnSubmit(earthquakeEvent);

                // save changes to the database
                context.SubmitChanges();
            }
        }

        public static void DeleteMessage(string id)
        {
            using (SigimeraDataContext context = new SigimeraDataContext(Shared.CONNECTION_STRING))
            {
                IQueryable<RootObject> messageQuery = from c in context.CrisisItems where c._id == id select c;
                RootObject earthquakeItemToDelete = messageQuery.FirstOrDefault();

                context.CrisisItems.DeleteOnSubmit(earthquakeItemToDelete);

                // save changes to the database
                context.SubmitChanges();
            }
        }

        public static bool ClearRecords()
        {
            bool returnVal = true;
            try
            {
                RootObject[] tmp = null;
                using (SigimeraDataContext context = new SigimeraDataContext(Shared.CONNECTION_STRING))
                {
                    do
                    {
                        //Delete in chunks of 10 records each
                        tmp = context.CrisisItems.Take(10).ToArray();
                        if (tmp.Length > 0)
                        {
                            context.CrisisItems.DeleteAllOnSubmit(tmp);
                            context.SubmitChanges();
                        }
                    } while (tmp.Length > 0);
                }
            }
            catch (Exception ex)
            {
                returnVal = false;
            }
            return returnVal;
        }
    }
}