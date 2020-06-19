using System;
using System.Collections.Generic;

namespace s17738_cw3.OrmModels
{
    public partial class Token
    {
        public string Id { get; set; }
        public string UserId { get; set; }

        public virtual Student User { get; set; }
    }
}
