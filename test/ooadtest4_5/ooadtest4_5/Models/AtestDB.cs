using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace ooadtest4_5.Models
{
    public class AtestDB
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Title { get; set; }
        public string Director { get; set; }
        public DateTime Date { get; set; }
    }
    public class AtestDBContext : DbContext
    {
        public AtestDBContext() : base("SQLConnectionString") { }
        public DbSet<AtestDB> Atest { get; set; }
    }
    public class us
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string user { get; set; }

        public string pwd { get; set; }
    }
    public class usContext: DbContext
    {
        public usContext() : base("SQLConnectionString") { }
        public DbSet<us> db { get; set; }
    }
}
