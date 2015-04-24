namespace SS
{
    internal class BackItem
    {
        public string name;
        public string value;

        public BackItem(string cellName, string oldVal)
        {
            name = cellName;
            value = oldVal;
        }
    }
}