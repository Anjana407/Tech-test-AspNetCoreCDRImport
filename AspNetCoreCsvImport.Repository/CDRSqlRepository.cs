using AspNetCoreCsvImport.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace AspNetCoreCDRImport.Repository
{
    // This class is used to save Employees Data to SqlDataBase
    public class CDRSqlRepository : ISQLRepository
    {

        private readonly AppDbContext appDbContext;

        public CDRSqlRepository(AppDbContext dbContext)
        {
            appDbContext = dbContext;
        }

        public void SaveEmployeeData(IEnumerable<CDR> CDR)
        {
            appDbContext.AddRange(CDR);
            appDbContext.SaveChanges();

        }
    }
}
