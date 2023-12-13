using System;

namespace PatchManager.Model
{
    public class ProgramVersion
    {
        public int Major { get; internal set; }

        public int Minor { get; internal set; }

        public int Patch { get; internal set; }

        public ProgramVersion(int major, int minor = 0, int patch = 0)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Patch}";
        }

        public static ProgramVersion ConstructFromString(string versionString)
        {
            var version = new[]
            {
                0, 0, 0
            };
            var parts = versionString.Trim().Split('.');
            if (parts.Length > version.Length)
            {
                throw new FormatException($"Given argument versionString '{versionString}' contains to many version " +
                                          "number parts! Give at most 3! Eg. '1.2.3'");
            }

            for (var i = 0; i < version.Length; i++)
            {
                if (parts.Length > i)
                {
                    version[i] = int.Parse(parts[i]);
                }
            }

            return new ProgramVersion(version[0], version[1], version[2]);
        }
    }
}
