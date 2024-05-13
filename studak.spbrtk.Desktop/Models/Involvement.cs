using System;
using System.Collections.Generic;
using studak.spbrtk.API.Models;

namespace studak.spbrtk.API.Models
{
    
    public partial class Involvement
    {
        public int Eventid { get; set; }

        public int Userid { get; set; }

        public int Status { get; set; }

        public DateTime Createtime { get; set; }
    }

}
