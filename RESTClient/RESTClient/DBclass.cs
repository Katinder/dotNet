using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTClient
{
    class DBclass
    {
        public int log_id { get; set; }
        public int c1_pk { get; set; }
        public string c2_name { get; set; }
        public float c3_amount { get; set; }
        public DateTime c4_updated_at { get; set; }
        public string Operation { get; set; }
    }

}
