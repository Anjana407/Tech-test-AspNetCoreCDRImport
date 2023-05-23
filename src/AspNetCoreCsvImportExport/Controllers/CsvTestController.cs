using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreCsvImport.Model;
using AspNetCoreCsvImport.DTO;
using AspNetCoreCDRImport.Repository;
using System.Linq;

namespace AspNetCoreCsvImport.Controllers
{
    [Route("api/[controller]")]
    public class CsvImportController : Controller
    {

        private  IEnumerable<ISQLRepository> cdrRepositories;
        

        public CsvImportController(IEnumerable<ISQLRepository> _cdrRepositories)
        {
            cdrRepositories = _cdrRepositories;
           
        }

        // POST api/csvtest/import
        //The Import method uses the Content-Type HTTP Request Header, to decide how to handle the request body.
        //If the ‘text/csv’ is defined, the custom csv input formatter will be used.
        [HttpPost]
        [Route("import")]
        public IActionResult Import([FromBody]List<CDR> cdrData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                List<CDR> data = cdrData;

                // saving employee.csv data to SQL 
                var cdrRepository = cdrRepositories.SingleOrDefault(x => x.GetType() == typeof(CDRSqlRepository));
                cdrRepository.SaveEmployeeData(data);
                return Ok();
            }
        }

    }
}
