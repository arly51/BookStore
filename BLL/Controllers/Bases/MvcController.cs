using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Controllers.Bases
{
    public abstract class MvcController : Controller
    {
        protected MvcController()
        {
            CultureInfo cultureInfo = new CultureInfo("tr-TR");
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }
    }
}
