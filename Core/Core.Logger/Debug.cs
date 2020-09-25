using System;

namespace Core.Logger
{
    public static class Debug
    {
        #region Properties
        public static ConsoleColor Color { get; set; }
        #endregion

        #region Methods
        static Debug()
        {
            Color = ConsoleColor.Green;
        }

        public static void Log(Object contents)
        {
            Logger.WriteLine(Color, $"[Debug] {contents}");
        }
        #endregion
    }
}
