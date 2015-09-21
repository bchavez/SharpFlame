using System;
using Builder.Extensions;
using FluentBuild;
using FluentBuild.AssemblyInfoBuilding;
using FluentFs.Core;
using Z.ExtensionMethods;

namespace Builder
{
    public class Folders
    {
        public static readonly Directory WorkingFolder = new Directory( Properties.CurrentDirectory );
        public static readonly Directory CompileOutput = WorkingFolder.SubFolder( "__compile" );
        public static readonly Directory Package = WorkingFolder.SubFolder( "__package" );
        public static readonly Directory Source = WorkingFolder.SubFolder( "Source" );
        public static readonly Directory Builder = Source.SubFolder("Builder");
        public static readonly Directory Data = Source.SubFolder("Data");

        public static readonly Directory Lib = Source.SubFolder( "packages" );
    }

    public class BuildContext
    {
        public static readonly string FullVersion = VersionGetter.GetVersion();
        public static readonly string Version = FullVersion.WithoutPreReleaseName();
    }

    public class Projects
    {
        private static void GlobalAssemblyInfo(IAssemblyInfoDetails aid)
        {
            aid.Company("Brian Chavez")
                .Copyright("Brian Chavez © " + DateTime.UtcNow.Year)
                .Version(BuildContext.Version)
                .FileVersion(BuildContext.Version)
                .InformationalVersion($"{BuildContext.FullVersion} built on {DateTime.UtcNow} UTC")
                .Trademark("MIT License")
                .Description("http://www.github.com/bchavez/SharpFlame");
        }

        public static readonly File SolutionFile = Folders.Source.File("SharpFlame.VS2015.sln");

        public class SharpFlame
        {
            public const string Name = "SharpFlame";
            public static readonly Directory Folder = Folders.Source.SubFolder( Name );
            public static readonly File ProjectFile = Folder.File( $"{Name}.csproj" );
            //public static readonly Directory OutputDirectory = Folders.CompileOutput.SubFolder(Name);
            //public static readonly File OutputDll = OutputDirectory.File( $"{Name}.dll" );
            public static readonly Directory PackageDir = Folders.Package.SubFolder( Name );
            
            public static readonly File NugetSpec = Folders.Builder.SubFolder("NuGet").File( $"{Name}.nuspec" );
            public static readonly File NugetNupkg = Folders.Package.File($"{Name}.{BuildContext.FullVersion}.nupkg");

            public static readonly Action<IAssemblyInfoDetails> AssemblyInfo =
                i =>
                    {
                          i.Product(Name);

                        GlobalAssemblyInfo(i);
                    };
        }

        public class Gui
        {
            public static readonly Directory Windows = Folders.Source.SubFolder($"{SharpFlame.Name}.Gui.Windows");
            public static readonly Directory WindowsOutput = Windows.SubFolder("bin/Release");

            public static readonly Directory Linux = Folders.Source.SubFolder($"{SharpFlame.Name}.Gui.Linux");

            public static readonly Directory Mac = Folders.Source.SubFolder($"{SharpFlame.Name}.Gui.Mac");
        }

        public class Core
        {
            public static readonly Directory Folder = Folders.Source.SubFolder($"{SharpFlame.Name}.Core");
        }

        public class TemplatesProject
        {
            public const string Name = "Templates";
            public static readonly Directory Folder = Folders.Source.SubFolder(Name);
            public static readonly File ProjectFile = Folder.File($"{Name}.csproj");
            public static readonly Directory Metadata = Folder.SubFolder("Metadata");
        }

        public class Tests
        {
            public static readonly Directory Folder = Folders.Source.SubFolder( "SharpFlame.Tests" );
        }
    }
}
