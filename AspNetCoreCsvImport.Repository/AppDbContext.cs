using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using AspNetCoreCsvImport.DTO;

namespace AspNetCoreCDRImport.Repository
{
   public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions Options) : base(Options)
        {

        }
        public DbSet<CDR> CDRs { get; set; }
    }
}
