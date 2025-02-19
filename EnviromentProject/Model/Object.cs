namespace EnviromentProject.Model
{
    public class Object
    {
        public Guid Id { get; set; }
        public double PrefabId { get; set; }
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public double ScaleX { get; set; }
        public double ScaleY { get; set; }
        public double RotationZ { get; set; }
        public int SortingLayer { get; set; }
        public Guid EnvironmentId { get; set; } 
    }
}
