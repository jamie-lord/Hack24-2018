using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
//using PorgBot.Data;

namespace PorgBot.Contexts
{
    public class PorgContext : DbContext
    {
        //public DbSet<UserDetail> UserDetails { get; set; }

        public PorgContext() : base(nameof(PorgContext))
        {

        }
    }
}