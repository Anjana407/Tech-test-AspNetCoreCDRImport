using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCoreCsvImport.DTO;

namespace AspNetCoreCDRImport.Repository
{
    public interface ISQLRepository
    {
       public void SaveEmployeeData(IEnumerable<CDR> employee);
    }
}
