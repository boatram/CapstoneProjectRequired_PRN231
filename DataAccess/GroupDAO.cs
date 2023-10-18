using BusinessObjects.BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class GroupDAO
    {
        private static GroupDAO instance = null;
        public static readonly object instanceLock = new object();
        public static GroupDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new GroupDAO();
                    }
                    return instance;
                }
            }
        }

        public IEnumerable<GroupProject> GetGroupProjects()
        {
            var groups = new List<GroupProject>();
            try
            {
                using var context = new CPRContext();
                groups = context.GroupProjects.Include(c => c.TopicOfGroups).Include(c=>c.StudentInGroups).ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return groups;
        }
        public GroupProject GetGroupProjectByID(int? cusID)
        {
            GroupProject group = null;
            try
            {
                using var context = new CPRContext();
                group = context.GroupProjects.Include(c => c.TopicOfGroups).Include(c => c.StudentInGroups).SingleOrDefault(m => m.Id == cusID);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return group;
        }
        public GroupProject AddNew(GroupProject group)
        {
            try
            {
                GroupProject _group = GetGroupProjectByID(group.Id);
                if (_group == null)
                {
                    using var context = new CPRContext();
                    context.GroupProjects.Add(group);
                    context.SaveChanges();
                    return _group;
                }
                else
                {
                    throw new Exception("The group is already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public GroupProject AddTopicOfGroup(TopicOfGroup group)
        {
            try
            {
                GroupProject _group = GetGroupProjectByID(group.GroupProjectId);
                if (_group != null)
                {
                    using var context = new CPRContext();
                    context.TopicOfGroups.Add(group);
                    context.SaveChanges();
                    return _group;
                }
                else
                {
                    throw new Exception("The group does not already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public GroupProject Update(GroupProject group, int id)
        {
            try
            {
                GroupProject _group = GetGroupProjectByID(id);
                if (_group != null)
                {
                    using var context = new CPRContext();
                    context.GroupProjects.Update(group);
                    context.SaveChanges();
                    return group;
                }
                else
                {
                    throw new Exception("The group does not already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void RemoveStudentInGroup(StudentInGroup group, int id)
        {
            try
            {
                GroupProject _group = GetGroupProjectByID(id);
                if (_group != null)
                {
                    using var context = new CPRContext();
                    context.StudentInGroups.Remove(group);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("The group does not already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
