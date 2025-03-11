namespace ZipBuilder
{
    public class PremissionHLocClass
    {
        public string FullKey { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public int? Num1 { get; set; }
        public int? Num2 { get; set; }
        public List<PremissionHLocClass> Children { get; set; } = new List<PremissionHLocClass>();
        public void UpdateFullKeys(string parentKey = "")
        {
            FullKey = string.IsNullOrEmpty(parentKey) ? Key : $"{parentKey}.{Key}";
            foreach (var child in Children)
            {
                child.UpdateFullKeys(FullKey);
            }
        }
    }
}