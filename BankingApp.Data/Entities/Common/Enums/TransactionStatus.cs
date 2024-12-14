using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NpgsqlTypes;
using System.Runtime.Serialization;

namespace BankingApp.Data.Entities.Common.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransactionStatus
    {
        [PgName(nameof(Credit)), EnumMember(Value = nameof(Credit))]
        Credit,

        [PgName(nameof(Debit)), EnumMember(Value = nameof(Debit))]
        Debit,
    }
}
