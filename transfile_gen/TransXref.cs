namespace transfile_gen
{
    internal class TransXref
    {
        public string baseKey;
        public string baseText;
        public string transText;

        public TransXref(string key, string value1, string value2)
        {
            baseKey = key;
            baseText = value1;
            transText = value2;
        }
    }
}