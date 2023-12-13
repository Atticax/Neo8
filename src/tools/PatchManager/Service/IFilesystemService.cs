namespace PatchManager.Service
{
    public interface IFilesystemService
    {
        void AddProgram(string programName, string pathToProgram);
        string GetProgramCurrentVersionPath(string programName);
    }
}
