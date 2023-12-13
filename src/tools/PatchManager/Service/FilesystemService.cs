using System.IO;
using System.Text;
using PatchManager.Config;
using PatchManager.Exception;

namespace PatchManager.Service
{
    public class FilesystemService : IFilesystemService
    {
        private const string InitialVersionPath = "initial";
        private const string PatchesPath = "patches";
        private const string CurrentVersionPath = "current";

        private readonly string _storagePath;

        public FilesystemService(AppOptions appOptions)
        {
            _storagePath = appOptions.StorageDir;
            if (!Path.IsPathRooted(_storagePath))
            {
                _storagePath = Path.GetFullPath(_storagePath);
            }
        }

        public void AddProgram(string programName, string pathToProgram)
        {
            var programStoragePath = GetProgramStoragePath(programName);
            if (Directory.Exists(programStoragePath))
            {
                throw new ProgramAlreadyExistsException(programName);
            }
            if (!Directory.Exists(pathToProgram))
            {
                throw new ProgramSourceNotFound(pathToProgram);
            }

            var programInitialPath = Path.Join(programStoragePath, InitialVersionPath);
            var programCurrentPath = Path.Join(programStoragePath, CurrentVersionPath);
            Directory.CreateDirectory(programStoragePath);
            Directory.CreateDirectory(programInitialPath);
            Directory.CreateDirectory(Path.Join(programStoragePath, PatchesPath));
            Directory.CreateDirectory(programCurrentPath);
            CopyDirectoryRecursive(pathToProgram, programInitialPath, true);
            CopyDirectoryRecursive(programInitialPath, programCurrentPath, true);
        }

        public string GetProgramCurrentVersionPath(string programName)
        {
            return NormalizeDirectoryPath(Path.Join(RequireProgramStoragePath(programName), CurrentVersionPath));
        }

        private string GetProgramStoragePath(string programName)
        {
            RequireStorage();
            return Path.Join(_storagePath, programName);
        }

        private string RequireProgramStoragePath(string programName)
        {
            var programStoragePath = GetProgramStoragePath(programName);
            if (Directory.Exists(programStoragePath))
            {
                throw new ProgramDoesNotExist(programName);
            }

            return programStoragePath;
        }

        private void RequireStorage()
        {
            Directory.CreateDirectory(_storagePath);
            RequireWritable(_storagePath);
        }

        private static void RequireWritable(string path)
        {
            var testFileName = Path.Join(path, "test_writable" + Path.GetRandomFileName());
            if (File.Exists(testFileName))
            {
                File.Delete(testFileName);
            }
            using (var tmpFile = new StreamWriter(testFileName))
            {
                tmpFile.Write(0x00);
            }
            File.Delete(testFileName);
        }

        private static string NormalizeDirectoryPath(string path)
        {
            path = Path.GetFullPath(path.Trim());
            if (!path.EndsWith(Path.DirectorySeparatorChar))
            {
                path += Path.DirectorySeparatorChar;
            }

            return path;
        }

        public static void CopyDirectoryRecursive(string sourceFolder, string destinationFolder, bool overrideExisting = false)
        {
            sourceFolder = NormalizeDirectoryPath(sourceFolder);
            destinationFolder = NormalizeDirectoryPath(destinationFolder);

            foreach (var sourceFile in Directory.GetFiles(sourceFolder, "*", SearchOption.AllDirectories))
            {
                var srcFile = new FileInfo(sourceFile);

                // Create a destination that matches the source structure
                var destFile = new FileInfo(Path.Join(destinationFolder, srcFile.FullName.Substring(sourceFolder.Length)));

                Directory.CreateDirectory(destFile.DirectoryName);

                if (srcFile.LastWriteTime > destFile.LastWriteTime || !destFile.Exists)
                {
                    File.Copy(srcFile.FullName, destFile.FullName, overrideExisting);
                }
            }
        }
    }
}
