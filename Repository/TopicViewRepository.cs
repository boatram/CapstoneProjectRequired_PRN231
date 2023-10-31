using BusinessObjects.DTOs;
using BusinessObjects.DTOs.Response;
using DataAccess;
using Repository.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class TopicViewRepository : ITopicViewRepository
    {
        public void CreateTopicView(TopicView topic) => TopicViewDAO.Instance.CreateTopicView(topic);

        public IEnumerable<TopicView> GetTopicViews() => TopicViewDAO.Instance.GetTopicViews();

        public void UpdateStatus(int Id) => TopicViewDAO.Instance.UpdateStatus(Id);
       
    }
}
