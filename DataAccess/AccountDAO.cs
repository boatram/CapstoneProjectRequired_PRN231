using BusinessObjects;
using BusinessObjects.BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class AccountDAO
    {
        private static AccountDAO instance = null;
        public static readonly object instanceLock = new object();
        public static AccountDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new AccountDAO();
                    }
                    return instance;
                }
            }
        }

        public IEnumerable<Account> GetAccounts()
        {
            var accounts = new List<Account>();
            try
            {
                using var context = new CPRContext();
                accounts = context.Accounts.Include(c => c.Role).Include(c=>c.Specialization).Include(c=>c.TopicOfLecturers).ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return accounts;
        }
        public Account GetAccountByID(int? cusID)
        {
            Account account = null;
            try
            {
                using var context = new CPRContext();
                account = context.Accounts.Include(c => c.Role).Include(c=>c.Specialization).SingleOrDefault(m => m.Id == cusID);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return account;
        }
        public Account AddNew(Account account)
        {
            try
            {
                Account _account = GetAccountByID(account.Id);
                if (_account == null)
                {
                    using var context = new CPRContext();
                    context.Accounts.Add(account);
                    context.SaveChanges();
                    return _account;
                }
                else
                {
                    throw new Exception("The account is already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public Account Update(Account account, int id)
        {
            try
            {
                Account _account = GetAccountByID(id);
                if (_account != null)
                {
                    using var context = new CPRContext();
                    context.Accounts.Update(account);
                    context.SaveChanges();
                    return account;
                }
                else
                {
                    throw new Exception("The account does not already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
