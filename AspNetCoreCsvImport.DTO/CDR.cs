using System;

namespace AspNetCoreCsvImport.DTO
{
    public class CDR
    {
        public int caller_id { get; set; }
        public string recipient { get; set; }
        public DateTime call_date { get; set; }
        public DateTime end_time { get; set; }
        public int duration { get; set; }
        public double cost { get; set; }
        public string reference { get; set; }
        public double currency { get; set; }

    }
}
