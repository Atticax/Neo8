namespace PatchManager.Exception
{
    public class ProgramAlreadyExistsException : System.Exception
    {
        public ProgramAlreadyExistsException(string programName)
            : base("A program with the name '" + programName + "' already exists!")
        {
        }
    }
}
