using System;

namespace PacketGenerator
{
    public static class PacketType
    {
        public static Type Parse(String typeName)
        {
            Type type = Type.GetType(typeName);

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                case TypeCode.String:
                    return type;
                default:
                    if (typeName.Contains("List"))
                    {
                        String name = typeName.Replace("List", "");

                        return Type.GetType($"List<{name}>");
                    }

                    throw new ArgumentException($"Not found Type: {typeName}");
            }
        }
    }
}
