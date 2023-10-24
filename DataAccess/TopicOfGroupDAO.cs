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
        public List<int> GetTopicIdByStatusIsTrue()
        {
            List<int> result = new List<int>();
            try
            {
                using var context = new CPRContext();
                IEnumerable<TopicOfGroup> rs = context.TopicOfGroups.Where(c => c.Status.Equals(true)).ToList();
                for (int i = 0; i <= rs.Count(); i++)
                {
                    foreach (var r in rs)
                    {
                        result[i] = r.TopicId;
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        public IEnumerable<TopicOfGroup> GetTopicOfGroupByTopicID(int? topicID)
        {
            IEnumerable<TopicOfGroup> topic = null;
            try
            {
                using var context = new CPRContext();
                topic = context.TopicOfGroups.Include(c => c.Topic).Include(c => c.GroupProject).Where(m => m.TopicId == topicID).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return topic;
        }

        public TopicOfGroup GetTopicOfGroupByID(int? topicID, int? groupID)
        {
            TopicOfGroup topic = null;
            try
            {
                using var context = new CPRContext();
                topic = context.TopicOfGroups.Include(c => c.Topic).Include(c => c.GroupProject).Where(m => m.TopicId == topicID).SingleOrDefault(g => g.GroupProjectId == groupID);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return topic;
        }

        public void Update(TopicOfGroup topic, int topicid)
        {
            try
            {
                using var context = new CPRContext();
                IEnumerable<TopicOfGroup> topicOfGroups = GetTopicOfGroupByTopicID(topicid);
                if (topicOfGroups != null)
                {
                    TopicOfGroup _topic = GetTopicOfGroupByID(topic.TopicId, topic.GroupProjectId);
                    if (_topic != null)
                    {
                        foreach (TopicOfGroup topicOfGroup in topicOfGroups.ToList())
                        {
                            if (topicOfGroup.TopicId == _topic.TopicId && topicOfGroup.GroupProjectId == _topic.GroupProjectId)
                            {
                                topicOfGroup.Status = true;
                                context.TopicOfGroups.Update(topicOfGroup);
                                context.SaveChanges();
                            }
                            else if (topicOfGroup.TopicId == _topic.TopicId && topicOfGroup.GroupProjectId != _topic.GroupProjectId)
                            {
                                topicOfGroup.Status = false;
                                context.TopicOfGroups.Update(topicOfGroup);
                                context.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("The topic of group does not already exist.");
                    }

                }
                else
                {
                    throw new Exception("The topic of group does not already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}
