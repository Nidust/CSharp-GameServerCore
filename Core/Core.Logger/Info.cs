using System;

namespace Core.Logger
{
    public static class Info
    {
        #region Properties
        public static ConsoleColor Color { get; set; }
        #endregion

        #region Methods
        static Info()
        {
            Color = ConsoleColor.White;
        }

        public static void Log(Object contents)
        {
            Logger.WriteLine(Color, $"[Info] {contents}");
        }
        #endregion
    }
}
