using System;

namespace SSAManager
{
    public class Administrator
    {
        public Guid ManagerId       { get; set; }
        public Guid DomainId        { get; set; }
        public DateTime CreatedAt   { get; set; }
    }
}

