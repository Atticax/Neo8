using System.IO;

namespace PatchManager.Exception
{
    public class ProgramSourceNotFound : FileNotFoundException
    {
        public ProgramSourceNotFound(string pathToSource)
            : base("The program's source could not be found at '" + Path.GetFullPath(pathToSource) + "'")
        {
        }
    }
}
