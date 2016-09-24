using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ocbina_israel_Exam.Models
{
    public class CounterDBContext : DbContext
    {
        public DbSet<Counter> Counters { get; set; }
    }
}