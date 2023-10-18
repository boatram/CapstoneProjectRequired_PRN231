using BusinessObjects.BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class TopicOfGroupDAO
    {
        private static TopicOfGroupDAO instance = null;
        public static readonly object instanceLock = new object();
        public static TopicOfGroupDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new TopicOfGroupDAO();
                    }
                    return instance;
                }
            }
        }

        public IEnumerable<TopicOfGroup> GetTopicOfGroups()
        {
            var topics = new List<TopicOfGroup>();
            try
            {
                using var context = new CPRContext();
                topics = context.TopicOfGroups.Include(c => c.GroupProject).Include(c => c.Topic).ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return topics;
        }
       
       
        
    }
}
