using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ServerControlPanel.Models
{
    public class StandardPageModel
    {
        public StandardPageModel(HttpRequest _request)
        {
            Request = _request;
        }

        public HttpRequest Request;
    }
}
