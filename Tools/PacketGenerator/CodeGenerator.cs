using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PacketGenerator
{
    public abstract class CodeGenerator
    {
        #region Properties
        private Int32 mCurrentIndent;
        private List<String> mLines;

        private String mTemporaryString;
        #endregion

        #region Abstract Methods
        public abstract void Generate();
        #endregion

        #region Methods
        protected CodeGenerator()
        {
            mCurrentIndent = 0;
            mLines = new List<String>();
        }

        protected void Flush(String outputPath)
        {
            if (mTemporaryString != null)
                PushTemporaryString();

            String directory = Path.GetDirectoryName(outputPath);
            if (Directory.Exists(directory) == false)
                Directory.CreateDirectory(directory);

            String fileHeader =
                $"/// This file is auto-generated file with PacketGenerator proejct, {Environment.NewLine}" +
                $"/// with {this.GetType().FullName} class. {Environment.NewLine}" +
                $"/// Please do not modify this file directly, or any modification at this file will lost at pre-build. {Environment.NewLine}" +
                $"{Environment.NewLine}";

            File.WriteAllText(outputPath, fileHeader + String.Join(Environment.NewLine, mLines.Select(x => x.TrimEnd())));

            mLines.Clear();
        }

        protected void Write(String code)
        {
            if (mTemporaryString == null)
                mTemporaryString = code;
            else
                mTemporaryString += code;
        }

        protected void WriteLine()
        {
            WriteLine(String.Empty);
        }

        protected void WriteLine(String code)
        {
            Write(code);
            PushTemporaryString();
        }

        protected void PushIndent()
        {
            if (mTemporaryString == null)
                throw new ArgumentException("PushIndent Exception: TemporaryString is null");

            ++mCurrentIndent;
        }

        protected void PopIndent()
        {
            --mCurrentIndent;

            if (mCurrentIndent < 0)
                throw new InvalidOperationException("PopIndent Exception: Please match the indent");
        }
        #endregion

        #region Private
        private void PushTemporaryString()
        {
            if (mTemporaryString == null)
                throw new ArgumentException();

            String indentString = new string(' ', 4 * mCurrentIndent);
            mLines.Add(indentString + mTemporaryString);
            mTemporaryString = null;
        }
        #endregion
    }
}
