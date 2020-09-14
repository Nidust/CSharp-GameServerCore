using System;

namespace PacketGenerator
{
    public static class GeneratorConfig
    {
        #region Properties
        public static String DefinitionPath { get; private set; }
        public static String WorkingDirectory { get; private set; }
        public static String OutputDirecotry { get; private set; }
        public static String Namespace { get; private set; }
        #endregion

        #region Methods
        public static void Init(String[] args)
        {
            if (args.Length < 3)
            {
                throw ThrowArgumentException();
            }

            WorkingDirectory = Environment.CurrentDirectory;

            foreach (String argument in args)
            {
                if (argument.Contains("--workingPath="))
                {
                    WorkingDirectory = argument.Replace("--workingPath=", "");
                }
                else if (argument.Contains("--definitionPath="))
                {
                    DefinitionPath = argument.Replace("--definitionPath=", "");
                }
                else if (argument.Contains("--outputPath="))
                {
                    OutputDirecotry = argument.Replace("--outputPath=", "");
                }
                else if (argument.Contains("--namespace="))
                {
                    Namespace = argument.Replace("--namespace=", "");
                }
                else
                {
                    throw ThrowArgumentException();
                }
            }

            if (String.IsNullOrWhiteSpace(DefinitionPath) || String.IsNullOrWhiteSpace(OutputDirecotry))
            {
                throw ThrowArgumentException();
            }
        }
        #endregion

        #region Private
        private static Exception ThrowArgumentException()
        {
            return new Exception(@"PacketGenerator: Invalid Argument
                --workingPath: Working Directory (optional)
                --definitionPath: Packet Definition Path (.xml)
                --outputPath: Packet Code Output Directory
                --namespace: Output Code Namespace

                Example: PacketGenerator.exe --definitionPath=DenifitionDirectory/PacketDefinition.xml --outputPath=OutputDirectory
            ");
        }
        #endregion
    }
}
