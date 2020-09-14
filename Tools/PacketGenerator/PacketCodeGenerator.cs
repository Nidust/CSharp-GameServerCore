using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace PacketGenerator
{
    public sealed class PacketCodeGenerator : CodeGenerator
    {
        public override void Generate()
        {
            XDocument document = XDocument.Load(GeneratorConfig.DefinitionPath);

            CreateUsing();
            CreateNamespaceAndClasses(document);

            Flush(GeneratorConfig.OutputDirecotry);
        }

        private void CreateUsing()
        {
            WriteLine("using System;");
            WriteLine();
        }

        private void CreateNamespaceAndClasses(XDocument document)
        {
            WriteLine($"namespace {GeneratorConfig.Namespace}");
            WriteLine("{");
            PushIndent();
            {
                GeneratePacketType(document.Descendants("Group"));
                WriteLine();

                GeneratePacketClass(document.Descendants("Packet"));
            }
            PopIndent();
            WriteLine("}");
        }

        private void GeneratePacketType(IEnumerable<XElement> groups)
        {
            WriteLine($"public partial class PacketType");
            WriteLine("{");
            PushIndent();
            {
                int currentIndex = 0;

                foreach (XElement group in groups)
                {
                    if (group.Attribute("startEnumIndex") == null)
                        throw new ArgumentNullException($"{group.Name} group startEnumIndex attribute is null");

                    currentIndex = Int32.Parse(group.Attribute("startEnumIndex").Value);

                    foreach (XElement packet in group.Descendants("Packet"))
                    {
                        String packetName = packet.Attribute("name")?.Value ?? String.Empty;
                        if (String.IsNullOrEmpty(packetName))
                            throw new ArgumentNullException($"{group.Name} group Packet name attribtue is null");

                        WriteLine($"public static PacketType {packetName} = new PacketType({currentIndex}, {packetName});");
                        ++currentIndex;
                    }
                }
            }
            PopIndent();
            WriteLine("}");
        }

        private void GeneratePacketClass(IEnumerable<XElement> packets)
        {

        }
    }
}
