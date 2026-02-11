using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OracleDataManager
{
    public sealed class DbConnectionProfile
    {
        public string Name { get; set; } = "";        // 표시명 (DEV/QA/운영)
        public string UserId { get; set; } = "";
        public string DataSource { get; set; } = "";  // //host:1521/SERVICE
        public string PasswordEnc { get; set; } = ""; // DPAPI 암호화
        public bool AskPasswordEveryTime { get; set; } = true; // 실행 시 비번 입력 여부
        public bool IsProduction { get; set; } = false;        // 운영 플래그(경고/색상)
    }

}
