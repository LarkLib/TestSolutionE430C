using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QnyWeb.Models
{
    public class AccountManagerModel
    {
        public ApplicationUser User { get; set; }
        public IList<IdentityRole> Roles { get; set; }
        public IList<PoiList> Pois { get; set; }
    }
}