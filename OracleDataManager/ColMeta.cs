using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OracleDataManager
{
    sealed class ColMeta
    {
        public string Name = "";
        public string DataType = "";
        public bool IsNullable;
        public string? DefaultSql;
        public int? CharLength;
        public int? Precision;
        public int? Scale;

        public bool IsLob;
        public bool IsDateLike;
        public bool IsNumber;
    }
}
