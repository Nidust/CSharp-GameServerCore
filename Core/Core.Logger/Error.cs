using System;

namespace Core.Logger
{
    public static class Error
    {
        #region Properties
        public static ConsoleColor Color { get; set; }
        #endregion

        #region Methods
        static Error()
        {
            Color = ConsoleColor.Red;
        }

        public static void Log(Object contents)
        {
            Logger.WriteLine(Color, $"[Error] {contents}");
        }
        #endregion
    }
}
