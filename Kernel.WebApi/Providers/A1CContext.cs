using FluentData;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Kernel.WebApi.Providers
{
    internal class A1CContext
    {
        #region DB Context

        public IDbContext DB;
        public A1CContext()
            : base()
        {
            this.DB = new DbContext().ConnectionString(ConfigurationManager.ConnectionStrings["A1C"].ConnectionString, new MySqlProvider());
            this.DB.IgnoreIfAutoMapFails(true);
            this.DB.CommandTimeout(360);
        }

        private static A1CContext mySql;

        public static A1CContext MySql
        {
            get
            {
                //if (_GClaims == null)
                //{
                mySql = new A1CContext();
                //}
                return mySql;
            }
        }

        #endregion



    }
}