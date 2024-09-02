using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class WindowsInfo
    {
        public IntPtr Handle { get; set; }
        public string Title { get; set; }
    }

    public class Element
    {
        public IntPtr Handle { get; set; }
        public string ClassName { get; set; }

        public string Text { get; set; }
    }
}