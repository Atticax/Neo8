namespace PatchManager.Exception
{
    public class ProgramDoesNotExist : System.Exception
    {
        public ProgramDoesNotExist(string programName)
            : base("A program with the name '" + programName + "' does not exists!")
        {
        }
    }
}
