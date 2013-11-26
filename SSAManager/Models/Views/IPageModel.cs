using System;
using System.Collections.Generic;

namespace SSAManager
{
    public interface IPageModel
    {
        Manager Manager { get; set; }
        string Title { get; set; }
        List<Error> Errors { get; set; }
    }
}

