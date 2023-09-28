﻿using BusinessObjects;
using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class SpecializationDAO
    {
        private static SpecializationDAO instance = null;
        public static readonly object instanceLock = new object();
        public static SpecializationDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new SpecializationDAO();
                    }
                    return instance;
                }
            }
        }

        public IEnumerable<SpecializationResponse> GetSpecializations()
        {
            var specializations = new List<SpecializationResponse>();
            try
            {
                using var context = new CPRContext();
                specializations = context.Specializations.Select(s => new SpecializationResponse
                {
                    Id = s.Id,
                    Code = s.Code,
                    Name = s.Name,
                    Description = s.Description,
                    Status = s.Status,
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return specializations;
        }

        public Specialization GetSpecializationByID(int? ID)
        {
            Specialization s = null;
            try
            {
                using var context = new CPRContext();
                s = context.Specializations.SingleOrDefault(m => m.Id == ID);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return s;
        }

        public Specialization GetSpecializationByCode(string? code)
        {
            Specialization s = null;
            try
            {
                using var context = new CPRContext();
                s = context.Specializations.SingleOrDefault(m => m.Code.Equals(code));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return s;
        }

        public void Create(SpecializationRequest s)
        {
            try
            {
                Specialization _s = GetSpecializationByCode(s.Code);
                using var context = new CPRContext();
                if (_s== null)
                {
                    var sp = new Specialization();
                    sp.Code = s.Code;
                    sp.Name = s.Name;
                    sp.Description = s.Description;
                    sp.Status = true;
                    context.Specializations.Add(sp);
                }
                else
                {
                    throw new Exception("The Specialization is already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateStatus(int id)
        {
            try
            {
                Specialization _s = GetSpecializationByID(id);
                if (_s != null)
                {
                    using var context = new CPRContext();
                    _s.Status = false;
                    context.Specializations.Update(_s);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("The Specialization does not already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
