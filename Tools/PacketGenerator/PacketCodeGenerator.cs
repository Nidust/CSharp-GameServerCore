using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace PacketGenerator
{
    public sealed class PacketCodeGenerator : CodeGenerator
    {
        #region Methods
        public override void Generate()
        {
            XDocument document = XDocument.Load(GeneratorConfig.DefinitionPath);

            CreateUsing();
            CreateNamespaceAndClasses(document);

            Flush(GeneratorConfig.OutputDirecotry);
        }
        #endregion

        #region Private
        private void CreateUsing()
        {
            WriteLine("using System;");
            WriteLine("using System.Collections.Generic;");
            WriteLine();
        }

        private void CreateNamespaceAndClasses(XDocument document)
        {
            WriteLine($"namespace {GeneratorConfig.Namespace}");
            WriteLine("{");
            PushIndent();
            {
                XElement[] packets = document
                    .Descendants("Packet")
                    .ToArray();

                GeneratePacketType(packets);

                WriteLine();

                foreach (XElement packet in packets)
                {
                    GeneratePacketClass(packet);
                    WriteLine();
                }
            }
            PopIndent();
            WriteLine("}");
        }

        private void GeneratePacketType(IEnumerable<XElement> packets)
        {
            WriteLine($"public partial class PacketType");
            WriteLine("{");
            PushIndent();
            {
                int currentIndex = 0;

                foreach (XElement packet in packets)
                {
                    WriteLine($"public static PacketType {packet.Name} = new PacketType({currentIndex}, {packet.Name});");
                    ++currentIndex;
                }
            }
            PopIndent();
            WriteLine("}");
        }

        private void GeneratePacketClass(XElement packet)
        {
            WriteLine($"public class {packet.Name}");
            WriteLine("{");
            PushIndent();
            {
                foreach (XElement nestedClass in packet.Descendants("Class"))
                {
                    GenerateNestedClass(nestedClass);
                    WriteLine();
                }

                GenerateProperties(packet);

                WriteLine();

                GenerateSerializer(packet);
            }
            PopIndent();
            WriteLine("}");
        }

        private void GenerateNestedClass(XElement nestedClass)
        {
            WriteLine($"public class {nestedClass.Name}");
            WriteLine("{");
            PushIndent();
            {
                GenerateProperties(nestedClass);

                WriteLine();

                GenerateSerializer(nestedClass);
            }
            PopIndent();
            WriteLine("}");
        }

        private void GenerateProperties(XElement root)
        {
            foreach (XElement element in root.Elements())
            {
                Type packetType = PacketType.Parse(element.Name.ToString());
                String name = element.Attribute("name").Value;

                WriteLine($"public {packetType.Name} {name} {{ get; set; }}");
            }
        }

        private void GenerateSerializer(XElement root)
        {
            WriteLine("public static void Serialize(BinaryWriter writer)");
            WriteLine("{");
            PushIndent();
            {
                foreach (XElement element in root.Elements())
                {
                    String propertyName = element.Attribute("name").Value;

                    WriteSerializeProperty(element.Name.ToString(), propertyName);
                    WriteLine();
                }
            }
            PopIndent();
            WriteLine("}");

            WriteLine();

            WriteLine("public static void Deserialize(BinaryReader reader)");
            WriteLine("{");
            PushIndent();
            {
                foreach (XElement element in root.Elements())
                {
                    String propertyName = element.Attribute("name").Value;

                    WriteDeserializeProperty(element.Name.ToString(), propertyName);
                    WriteLine();
                }
            }
            PopIndent();
            WriteLine("}");
        }

        private void WriteSerializeProperty(String propertyType, String propertyName)
        {
            Type type = PacketType.Parse(propertyType);

            if (type.IsClass)
            {
                WriteLine($"{propertyName}.Serialize(writer);");
                return;
            }
            else if (type.IsArray) // IsGenericList
            {
                String listValueType = propertyType.Trim('[', ']');
                
                WriteLine($"writer.Write((UInt16){propertyName}.Length)");

                WriteLine($"foreach (var {listValueType}Item in {propertyName}");
                WriteLine("{");
                PushIndent();
                {
                    WriteSerializeProperty(listValueType, $"{listValueType}Item");
                }
                PopIndent();
                WriteLine("}");
                return;
            }
            else if (type == typeof(String))
            {
                WriteLine($"writer.Write((UInt16){propertyName}.Length);");
                WriteLine($"writer.Write(BitConverter.GetBytes({propertyName}));");
                return;
            }
            else if (type == typeof(DateTime))
            {
                WriteLine($"writer.Write((UInt64){propertyName}.Ticks);");
                return;
            }

            WriteLine($"writer.Write({propertyName});");
        }

        private void WriteDeserializeProperty(String propertyType, String propertyName)
        {
            Type type = PacketType.Parse(propertyType);

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    WriteLine($"{propertyName} = reader.ReadBoolean();");
                    break;
                case TypeCode.Char:
                    WriteLine($"{propertyName} = reader.ReadChar();");
                    break;
                case TypeCode.SByte:
                    WriteLine($"{propertyName} = reader.ReadSByte();");
                    break;
                case TypeCode.Byte:
                    WriteLine($"{propertyName} = reader.ReadByte();");
                    break;
                case TypeCode.Int16:
                    WriteLine($"{propertyName} = reader.ReadInt16();");
                    break;
                case TypeCode.UInt16:
                    WriteLine($"{propertyName} = reader.ReadUInt16();");
                    break;
                case TypeCode.Int32:
                    WriteLine($"{propertyName} = reader.ReadInt32();");
                    break;
                case TypeCode.UInt32:
                    WriteLine($"{propertyName} = reader.ReadUInt32();");
                    break;
                case TypeCode.Int64:
                    WriteLine($"{propertyName} = reader.ReadInt64();");
                    break;
                case TypeCode.UInt64:
                    WriteLine($"{propertyName} = reader.ReadUInt64();");
                    break;
                case TypeCode.Single:
                    WriteLine($"{propertyName} = reader.ReadSingle();");
                    break;
                case TypeCode.Double:
                    WriteLine($"{propertyName} = reader.ReadDouble();");
                    break;
                case TypeCode.Decimal:
                    WriteLine($"{propertyName} = reader.ReadDecimal();");
                    break;
                case TypeCode.DateTime:
                    WriteLine($"{propertyName} = Convert.ToDateTime(reader.ReadUInt64());");
                    break;
                case TypeCode.String:
                    WriteLine($"UInt16 {propertyName}Len = reader.ReadUInt16();");
                    WriteLine($"{propertyName} = BitConverter.ToString(reader.ReadBytes({propertyName}Len));");
                    break;
                default:
                    if (type.IsClass)
                    {
                        WriteLine($"{propertyName} = new {propertyType}();");
                        WriteLine($"{propertyName}.Deserialize(reader);");
                    }
                    else if (type.IsArray)
                    {
                        String listValueType = propertyType.Replace("[]", "");

                        WriteLine($"UInt16 {propertyName}Len = reader.ReadUInt16();");
                        WriteLine($"{propertyName} = new {propertyType}({propertyName}Len);");

                        WriteLine($"foreach (var {propertyName}Item in {propertyName})");
                        WriteLine("{");
                        PushIndent();
                        {
                            WriteDeserializeProperty(listValueType, $"{propertyName}Item");
                        }
                        PopIndent();
                        WriteLine("}");
                    }
                    break;
            }
        }
        #endregion
    }
}
