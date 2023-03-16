namespace Otterkit;

public static class EnumExtensions
{
    public static string Display(this UsageType usage)
    {
        return usage switch
        {
            //> Used only for identifier validation:
            UsageType.Alphanumeric => "Alphanumeric",
            UsageType.Alphabetic => "Alphabetic",
            UsageType.Boolean => "Boolean",
            UsageType.Decimal => "Decimal",
            UsageType.Integer => "Integer",
            //<
            UsageType.Binary => "BINARY",
            UsageType.BinaryChar => "BINARY-CHAR",
            UsageType.BinaryShort => "BINARY-SHORT",
            UsageType.BinaryLong => "BINARY-LONG",
            UsageType.BinaryDouble => "BINARY-DOUBLE",
            UsageType.Bit => "BIT",
            UsageType.Computational => "COMPUTATIONAL",
            UsageType.Display => "DISPLAY",
            UsageType.FloatBinary32 => "FLOAT-BINARY-32",
            UsageType.FloatBinary64 => "FLOAT-BINARY-64",
            UsageType.FloatBinary128 => "FLOAT-BINARY-128",
            UsageType.FloatDecimal16 => "FLOAT-DECIMAL-16",
            UsageType.FloatDecimal32 => "FLOAT-DECIMAL-32",
            UsageType.FloatExtended => "FLOAT-EXTENDED",
            UsageType.FloatLong => "FLOAT-LONG",
            UsageType.FloatShort => "FLOAT-SHORT",
            UsageType.Index => "INDEX",
            UsageType.MessageTag => "MESSAGE-TAG",
            UsageType.National => "NATIONAL",
            UsageType.ObjectReference => "OBJECT REFERENCE",
            UsageType.PackedDecimal => "PACKED-DECIMAL",
            UsageType.DataPointer => "POINTER",
            UsageType.FunctionPointer => "FUNCTION-POINTER",
            UsageType.ProgramPointer => "PROGRAM-POINTER",
            _ => "NONE"
        };
    }
}
