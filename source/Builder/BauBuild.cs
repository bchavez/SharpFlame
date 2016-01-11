using System;
using System.IO;
using BauCore;
using BauExec;
using BauMSBuild;
using Builder.Extensions;
using FluentAssertions;
using FluentBuild;
using Ionic.Zip;

namespace Builder
{
    public static class BauBuild
    {
        //Build Tasks
        public const string MsBuild = "msb";
        public const string MonoBuild = "mono";
        public const string Clean = "clean";
        public const string BuildInfo = "buildinfo";
        public const string Pack = "pack";

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
                .DependsOn(Clean, MsBuild)
                .MSBuild(MsBuild).Desc("Invokes MSBuild to build solution")
                .DependsOn(Clean, BuildInfo)
                .Do(msb =>
                    {
                        msb.ToolsVersion = "14.0";
                        msb.Solution = Projects.VisualStudioSln.ToString();
                        msb.Properties = new
                            {
                                Configuration = "Release",
                                OutDir = Projects.Gui.WindowsOutput.ToString()
                            };
                        msb.Targets = new[] {"SharpFlame_Gui_Windows:Rebuild"};
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
                .Task(Pack).Desc("Packs build for distribution")
                .DependsOn(MsBuild, MonoBuild)
                .Do(() =>
                    {
                        using( var z = new ZipFile(Projects.Gui.WindowsZip.ToString()) )
                        {
                            z.AddDirectory(Projects.Gui.WindowsOutput.ToString(), Path.GetFileNameWithoutExtension(Projects.Gui.WindowsZip.ToString()));
                            z.AddDirectory(Folders.Data.ToString(), "Data");
                            z.Save();
                        }
                        using (var z = new ZipFile(Projects.Gui.LinuxZip.ToString()))
                        {
                            z.AddDirectory(Projects.Gui.LinuxOutput.ToString(), Path.GetFileNameWithoutExtension(Projects.Gui.LinuxZip.ToString()));
                            z.AddDirectory(Folders.Data.ToString(), "Data");
                            z.Save();
                        }
                    })
                .Exec(MonoBuild).Desc("Produces runs the mono xbuild.")
                .DependsOn(Clean, BuildInfo)
                .Do(exec =>
                {
                    var monopath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)}\Mono\bin";
                    exec.Run("cmd.exe")
                        .With("/c",
                            $@"""{monopath}\setmonopath.bat"" & ",
                            $@"xbuild.bat {Projects.MonoDevelopSln.ToString()} /p:OutDir={Projects.Gui.LinuxOutput}\"
                        ).In(Folders.Source.ToString());
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