namespace EnviromentProject.Model
{
    public class Environment
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int MaxHeight { get; set; }
        public int MaxLength { get; set; }
        public string UserId { get; set; }
    }
}

