using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{

  public  class SSO
    {
        public Guid Cert { get; set; }//一次性密钥
        public Guid Token { get; set; }//分站令牌
        public DateTime Time { get; set; }//过期时间
    }
 
}
