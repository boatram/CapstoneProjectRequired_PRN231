using BusinessObjects.DTOs;
using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using Microsoft.AspNetCore.Http;
using Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface ITopicRepository
    {
        Task<IEnumerable<TopicResponse>> GetTopics();
        Task<List<TopicResponse>> Create(IFormFile file);
        void UpdateStatus(int Id);
    }
}
