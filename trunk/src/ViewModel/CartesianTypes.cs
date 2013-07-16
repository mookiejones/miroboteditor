using System.ComponentModel;

namespace miRobotEditor.ViewModel
{
    [Localizable(false)]
    public class CartesianTypes
    {
        // Properties
        /// <summary>
        /// CartesianValueEnum
        /// </summary>
        /// <remarks>Cartesian Value Enumerated for Combo box. Used in AngleConverter</remarks>
        public CartesianEnum ValueCartesianEnum { get; set; }

        public string ValueCartesianString { get; set; }
    }
}