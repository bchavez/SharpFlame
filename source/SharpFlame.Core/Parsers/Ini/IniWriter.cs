using System;
using System.IO;
using System.Text;
using NLog;

namespace SharpFlame.Core.Parsers.Ini
{
	public class IniWriter
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private StreamWriter file;
		private bool disposed;

        public string EqualsChar = "=";
        public string LineEndChar = "\n";

        public IniWriter(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("Stream in null.");
			if (!stream.CanWrite)
				throw new ArgumentException("Stream must be writeable.");

            logger.Debug("Writing INI.");
			file = new StreamWriter(stream, Encoding.UTF8);
			disposed = false;
        }

		public IniWriter(StreamWriter output)
		{
			if (output == null)
				throw new ArgumentNullException("StreamWriter in null.");

			logger.Debug("Writing INI.");
			file = output;
		}

        public void AddSection(string Name)
        {
            Name = Name.Replace(LineEndChar, "");
			file.Write("{0}[{1}]{2}", LineEndChar, Name, LineEndChar);
        }

        public void AddProperty(string name, string value)
        {
            name = name.Replace(LineEndChar, "");
            name = name.Replace(EqualsChar, "");
            value = value.Replace(LineEndChar, "");
            value = value.Replace(EqualsChar, "");

            file.Write("{0} {1} {2}{3}", name, EqualsChar, value, LineEndChar);
        }

		public void Flush()
		{
			file.Flush();
		}
	}
}