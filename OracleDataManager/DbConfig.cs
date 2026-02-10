using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OracleDataManager
{
    public sealed class DbConfig
    {
        public string UserId { get; set; } = "";
        public string PasswordEnc { get; set; } = ""; 
        public string DataSource { get; set; } = "";  
    }
}
