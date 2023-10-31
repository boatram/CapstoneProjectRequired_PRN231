using BusinessObjects;
using BusinessObjects.BusinessObjects;
using BusinessObjects.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class TopicViewDAO
    {
        private static TopicViewDAO instance = null;
        public static readonly object instanceLock = new object();
        public static TopicViewDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new TopicViewDAO();
                    }
                    return instance;
                }
            }
        }

        public IEnumerable<TopicView> GetTopicViews()
        {
            var topics = new List<TopicView>();
            try
            {
                using var context = new CPRContext();
                topics = context.TopicOfLecturers.Select(x => new TopicView
                {
                    Id=x.TopicId,
                    Topic = context.Topics.SingleOrDefault(t => t.Id == x.TopicId),
                    Name = x.Topic.Name,
                    Description = x.Topic.Description,
                    Status = x.Topic.Status,
                    SemesterCode = x.Topic.Semester.Code,
                    SpecializationName = x.Topic.Specialization.Name,
                    SuperLecturerEmail = context.Accounts.Where(c => c.Id == x.LecturerId && x.IsSuperLecturer==true).SingleOrDefault().Email,
                }).ToList();
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

        public void CreateTopicView(TopicView topic)
        {
            try
            {
                Topic topic2 = new Topic();
                topic2 = instance.GetTopicByName(topic.Name);
                using var context = new CPRContext();              
                if (topic2 == null)
                {
                    var topic1 = new Topic();
                    topic1.Name = topic.Name;
                    topic1.Description = topic.Description;
                    topic1.Status = true;
                    topic1.Semester = context.Semesters.SingleOrDefault(s => s.Code.Equals(topic.SemesterCode));
                    topic1.SemesterId = context.Semesters.SingleOrDefault(s => s.Code.Equals(topic.SemesterCode)).Id;
                    topic1.Specialization = context.Specializations.SingleOrDefault(s => s.Name.Equals(topic.SpecializationName));
                    topic1.SpecializationId = context.Specializations.SingleOrDefault(s => s.Name.Equals(topic.SpecializationName)).Id;

                    var topicOfLecturer = new TopicOfLecturer();
                    topicOfLecturer.Lecturer = context.Accounts.SingleOrDefault(s => s.Email.Equals(topic.SuperLecturerEmail));
                    topicOfLecturer.LecturerId = context.Accounts.SingleOrDefault(s => s.Email.Equals(topic.SuperLecturerEmail)).Id;
                    topicOfLecturer.IsSuperLecturer = true;
                    topicOfLecturer.Status = true;
                    topicOfLecturer.Topic = topic1;
                    topicOfLecturer.TopicId = topic1.Id;

                    topic1.TopicOfLecturers.Add(topicOfLecturer);
                    context.Topics.Add(topic1);
                }
                else
                {
                    /*
                    TopicOfLecturer t = new TopicOfLecturer();
                    t.TopicId = topic2.Id;
                    t.IsSuperLecturer= false;
                    t.LecturerId = context.Accounts.SingleOrDefault(s => s.Email.Equals(topic.SuperLecturerEmail)).Id;
                    topic2.TopicOfLecturers.Add(t);
                    */
                    throw new Exception("The Topic is already exist.");
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
