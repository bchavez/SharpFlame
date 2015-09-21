using System;
using System.IO;
using BauCore;
using BauMSBuild;
using Builder.Extensions;
using FluentAssertions;
using FluentBuild;

namespace Builder
{
    public static class BauBuild
    {
        //Build Tasks
        public const string Build = "build";
        public const string Clean = "clean";
        public const string Restore = "restore";
        public const string BuildInfo = "buildinfo";
        public const string CodeGen = "codegen";
        public const string Pack = "pack";
        public const string Push = "push";

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
                {
                    Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~");
                    Console.WriteLine("     BUILDER ERROR    ");
                    Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~");
                    Console.WriteLine(e.ExceptionObject);
                    Environment.Exit(1);
                };

            new Bau(Arguments.Parse(args))
                .DependsOn(Clean, Build)
                .MSBuild(Build).Desc("Invokes MSBuild to build solution")
                .DependsOn(Clean, BuildInfo)
                .Do(msb =>
                    {
                        msb.ToolsVersion = "14.0";
                        msb.Solution = Projects.SolutionFile.ToString();
                        msb.Properties = new
                            {
                                Configuration = "Release",
                            };
                        msb.Targets = new[] { "SharpFlame_Gui_Windows:Rebuild" };
                    })
                .Task(BuildInfo).Desc("Creates dynamic AssemblyInfos for projects")
                .Do(() =>
                    {
                        Task.CreateAssemblyInfo.Language.CSharp(aid =>
                            {
                                Projects.SharpFlame.AssemblyInfo(aid);
                                var outputPath = Projects.Core.Folder.File("GlobalAssemblyInfo.cs");
                                Console.WriteLine($"Creating AssemblyInfo file: {outputPath}");
                                aid.OutputPath(outputPath);
                            });
                    })
                .Task(Clean).Desc("Cleans project files")
                .Do(() =>
                    {
                        Console.WriteLine($"Removing {Folders.CompileOutput}");
                        Folders.CompileOutput.Wipe();
                        Directory.CreateDirectory(Folders.CompileOutput.ToString());
                        Console.WriteLine($"Removing {Folders.Package}");
                        Folders.Package.Wipe();
                        Directory.CreateDirectory(Folders.Package.ToString());
                    })

                .Run();
        }

    }
}