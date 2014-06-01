using System;
using System.Diagnostics;

namespace FileVersion
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var versInfo= FileVersionInfo.GetVersionInfo(args[0]);
			String fileVersion = versInfo.FileVersion; 
			String productVersion = versInfo.ProductVersion; 

			//example for own display version string built of the four version parts:
			Console.WriteLine("{0}.{1}.{2}.{3}", versInfo.FileMajorPart, versInfo.FileMinorPart, 
				               versInfo.FileBuildPart, versInfo.FilePrivatePart);
		}
	}
}
