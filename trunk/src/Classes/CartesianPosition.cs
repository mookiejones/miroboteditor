namespace miRobotEditor.Classes
{
    /// <summary>
    ///     Cartesian Position to be used with the shift
    /// </summary>
    public class CartesianPosition 
    {

        public CartesianPosition()
        {
            Header = string.Empty;

        }
        public string Header { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

 }
}