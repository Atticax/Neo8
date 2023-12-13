using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleAppFramework;
using Microsoft.Extensions.DependencyInjection;
using PatchManager.Exception;
using PatchManager.Model;
using PatchManager.Model.Entity;
using PatchManager.Service;

namespace PatchManager.Controller
{
    class ConsoleCommandController : ConsoleAppBase
    {
        private readonly IFilesystemService _filesystem;
        private readonly IServiceProvider _serviceProvider;

        public ConsoleCommandController(IFilesystemService filesystem, IServiceProvider serviceProvider)
        {
            _filesystem = filesystem;
            _serviceProvider = serviceProvider;
        }

        [Command(new[]
        {
            "init", "initialize"
        }, "Initialize management for a new program")]
        public int Initialize(
            [Option(0, "Name of the program")] string name,
            [Option(1, "Path to initial version folder")] string path,
            [Option("v", "Version of initial program")] string version = "0.0.1"

        )
        {
            var programVersion = ProgramVersion.ConstructFromString(version);
            var managedProgram = new ManagedProgramEntity()
            {
                Name = name,
                ProgramVersions = new[]
                {
                    new ProgramVersionEntity()
                    {
                        Major = programVersion.Major, Minor = programVersion.Minor, Patch = programVersion.Patch
                    }
                }
            };
            using (var db = _serviceProvider.GetRequiredService<PatchDbContext>())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {

                        db.ManagedPrograms.Add(managedProgram);
                        db.SaveChanges();
                        _filesystem.AddProgram(name, path);
                        transaction.Commit();
                    }
                    catch (System.Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }

                }
            }

            Console.WriteLine($"Success! Name: '{name}', Version: '{version}', Path: '{path}'");
            return 0;
        }


        [Command(new[]
        {
            "gen", "generate", "GeneratePatch", "generate-patch"
        }, "Initialize management for a new program")]
        public int GeneratePatch(
            [Option(0, "Name of the program")] string name,
            [Option(1, "Path to next version folder")]
            string path,
            [Option("v", "Version of initial program")]
            string version = null
        )
        {

            var programCurrentVersionPath = _filesystem.GetProgramCurrentVersionPath(name);
            using (var db = _serviceProvider.GetRequiredService<PatchDbContext>())
            {
                var managedProgram = db.ManagedPrograms.FirstOrDefault(x => x.Name == name);
                if (managedProgram == null)
                {
                    throw new ProgramDoesNotExist(name);
                }

                var currentVersion = db.ProgramVersions
                    .Where(x => x.ManagedProgram == managedProgram)
                    .OrderBy(x => x.Major)
                    .ThenBy(x => x.Minor)
                    .ThenBy(x => x.Patch)
                    .FirstOrDefault();
                if (currentVersion == null)
                {
                    throw new System.Exception("No version found in database!");
                }

                ProgramVersion nextVersion;
                if (version != null)
                {
                    nextVersion = ProgramVersion.ConstructFromString(version);

                    if (
                        db.ProgramVersions.Any(x =>
                            x.ManagedProgram == managedProgram
                            && x.Major == nextVersion.Major
                            && x.Minor == nextVersion.Minor
                            && x.Patch == currentVersion.Patch
                        )
                    )
                    {
                        throw new System.Exception("Version already exists in database!");
                    }
                }
                else
                {
                    nextVersion = new ProgramVersion(currentVersion.Major, currentVersion.Minor, currentVersion.Patch + 1);
                }

            }

            return 0;
        }
    }
}
