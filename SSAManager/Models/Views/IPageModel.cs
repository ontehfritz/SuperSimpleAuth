using System;

namespace SSAManager
{
    public interface IPageModel
    {
        Manager Manager { get; set; }
        string Title { get; set; }
    }
}

