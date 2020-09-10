using System;

namespace PacketGenerator
{
    class Program
    {
        static void Main(String[] args)
        {
            try
            {
                GeneratorConfig.Init(args);

                Console.WriteLine($"Packet Denifition: {GeneratorConfig.DefinitionPath}");
                Console.WriteLine($"Output Directory: {GeneratorConfig.OutputDirecotry}");
                Console.WriteLine($"Working Directory: {GeneratorConfig.WorkingDirectory}");
                Console.WriteLine($"Starting Packet Code Generator...");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }
    }
}
