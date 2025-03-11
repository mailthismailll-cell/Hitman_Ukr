namespace ZipBuilder
{
    public class HLocClass
    {
        public List<HLocClass> children { get; set; }
        public string name { get; set; }
        public int? numChildren { get; set; }
        public byte? org_tbyte { get; set; }
        public string type { get; set; }
        public string value { get; set; }
        public string FullKey { get; set; }
    }
}
