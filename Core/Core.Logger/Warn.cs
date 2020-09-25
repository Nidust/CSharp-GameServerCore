using System;

namespace Core.Logger
{
    public static class Warn
    {
        #region Properties
        public static ConsoleColor Color { get; set; }
        #endregion

        #region Methods
        static Warn()
        {
            Color = ConsoleColor.Yellow;
        }

        public static void Log(Object contents)
        {
            Logger.WriteLine(Color, $"[Warn] {contents}");
        }
        #endregion
    }
}