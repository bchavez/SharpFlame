using System;
using System.ComponentModel;
using Ninject.Extensions.Logging;

namespace SharpFlame.Core.Extensions
{
    public static class ExtensionsForILogger
    {
        public static void ErrorException(this ILogger logger, [Localizable(false)] string message, Exception ex)
        {
            ((NLog.Logger)logger).ErrorException(message, ex);
        }
    }
}