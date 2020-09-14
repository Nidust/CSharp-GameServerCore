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

                foreach (XElement nestedClass in document.Descendants("NestedClass"))
                {
                    GenerateNestedClass(nestedClass);
                    WriteLine();
                }

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

        private void GenerateNestedClass(XElement nestedClass)
        {
            WriteLine($"public class {nestedClass.Name}");
            WriteLine("{");
            PushIndent();
            {
                foreach (XElement element in nestedClass.Elements())
                {
                    Type packetType = PacketType.Parse(element.Name.ToString());

                    WriteLine($"public {packetType.Name} {{ get; set; }}");
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
                foreach (XElement element in packet.Elements())
                {
                    Type packetType = PacketType.Parse(element.Name.ToString());

                    WriteLine($"public {packetType.Name} {element.Attribute("name").Value} {{ get; set; }}");
                }
            }
            PopIndent();
            WriteLine("}");
        }

        private void GenerateSerializeMethod()
        {

        }
        #endregion
    }
}
