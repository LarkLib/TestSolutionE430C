using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace TestVueWebApplication.Controllers
{
    public partial class Status
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> FinalStatus { get; set; }
    }

    public class TestVueController : ApiController
    {
        public IEnumerable<Status> GetStatusList()
        {
            var json = "[{\"Id\":0,\"Name\":\"待确认\",\"FinalStatus\":0},{\"Id\":1,\"Name\":\"待收货\",\"FinalStatus\":0},{\"Id\":2,\"Name\":\"完成收货\",\"FinalStatus\":1},{\"Id\":3,\"Name\":\"生成中\",\"FinalStatus\":0},{\"Id\":4,\"Name\":\"超时关闭\",\"FinalStatus\":1},{\"Id\":5,\"Name\":\"生成失败\",\"FinalStatus\":1},{\"Id\":7,\"Name\":\"部分收货\",\"FinalStatus\":0},{\"Id\":8,\"Name\":\"已取消\",\"FinalStatus\":1},{\"Id\":9,\"Name\":\"取消中\",\"FinalStatus\":0},{\"Id\":10,\"Name\":\"取消失败\",\"FinalStatus\":1},{\"Id\":11,\"Name\":\"待审核\",\"FinalStatus\":0}]";
            var status = JsonConvert.DeserializeObject<IEnumerable<Status>>(json);

            return status;
        }

    }
}
