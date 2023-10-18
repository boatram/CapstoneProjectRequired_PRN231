using BusinessObjects;
using BusinessObjects.BusinessObjects;
using BusinessObjects.DTOs;
using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class TopicDAO
    {
        private static TopicDAO instance = null;
        public static readonly object instanceLock = new object();
        public static TopicDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new TopicDAO();
                    }
                    return instance;
                }
            }
        }
        public IEnumerable<Topic> GetTopics()
        {
            var topics = new List<Topic>();
            try
            {
                using var context = new CPRContext();
                topics = context.Topics.Include(a=>a.Specialization).Include(a=>a.Semester).Include(c=>c.TopicOfLecturers).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return topics;
        }
        public Topic GetTopicByName(string? name)
        {
            var topic = new Topic();
            try
            {
                using var context = new CPRContext();
                topic = context.Topics.SingleOrDefault(m => m.Name.Equals(name));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return topic;
        }

        public Topic GetTopicById(int? id)
        {
            var topic = new Topic();
            try
            {
                using var context = new CPRContext();
                topic = context.Topics.SingleOrDefault(m => m.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return topic;
        }

        public void CreateTopic(Topic topic)
        {
            try
            {

                using var context = new CPRContext();              
               context.Topics.Add(topic);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public Topic Update(Topic topic, int id)
        {
            try
            {
                Topic _topic = GetTopicById(id);
                if (_topic != null)
                {
                    using var context = new CPRContext();
                    context.Topics.Update(topic);
                    context.SaveChanges();
                    return topic;
                }
                else
                {
                    throw new Exception("The topic does not already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void UpdateStatus(int Id)
        {
            try
            {
                Topic _topic = GetTopicById(Id);
                if (_topic != null)
                {
                    using var context = new CPRContext();
                    _topic.Status = false;
                    List<TopicOfLecturer> topicOfLecturers = _topic.TopicOfLecturers.Where(s => s.TopicId == Id).ToList();
                    foreach (TopicOfLecturer topic in topicOfLecturers)
                    {
                        topic.Status = false;
                        context.TopicOfLecturers.Update(topic);
                    }
                    context.Topics.Update(_topic);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("The topic does not already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
