namespace NeosIT.DB_Migrator.DBMigration
{
    /**
     * Version entity to make migrations comparable
     */

    public class Version
    {
        public static int MinorMaxlength = 4;
        public string Major = "0";
        public string Minor = "0";
        private long? _version;

        public Version()
        {
        }

        public Version(long version)
        {
            _version = version;
        }

        /**
	     * @return int a version number
	     */

        public long GetVersion()
        {
            /*if (null == _version && !_version.HasValue)
            {*/
                _version = long.Parse(Major + Minor.PadLeft(MinorMaxlength, '0'));
            /*}*/

            return _version.Value;
        }

        /**
	     * @param other_version compares this version against other_version
	     * @return true if this is higher than other (means larger, newer)
	     */

        public bool IsHigherThan(Version otherVersion)
        {
            return GetVersion() > otherVersion.GetVersion();
        }

        /**
	     * string representation of version
	     * @return
	     */

        public override string ToString()
        {
            return GetVersion().ToString();
        }

        public override bool Equals(object obj)
        {
            if (null == obj)
            {
                return false;
            }

            if (!(obj is Version))
            {
                return false;
            }

            return ToString().Equals(obj.ToString());
        }

        public override int GetHashCode()
        {
            return (int) GetVersion();
        }
    }
}