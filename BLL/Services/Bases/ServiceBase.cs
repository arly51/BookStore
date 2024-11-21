using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DAL;

namespace BLL.Services.Bases
{
    public abstract class ServiceBase
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; } = string.Empty;

        private readonly Db _db;

        protected ServiceBase(Db db) 
        {
            _db = db;
        }

        public ServiceBase Success(string message = "")
        {
            IsSuccessful = true;
            Message = message;
            return this;
        }

        public ServiceBase Error(string message= "")
        {
            IsSuccessful = false;
            Message = message;
            return this;
        }

    }
}
