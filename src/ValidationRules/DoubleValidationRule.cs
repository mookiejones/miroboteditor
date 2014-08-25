using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;

namespace miRobotEditor.ValidationRules
{
    [Localizable(false)]
    public sealed class DoubleValidationRule : ValidationRule
    {
        #region Constructor

        #endregion

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
            }
            catch (Exception e)
            {
                return new ValidationResult(false, "Illegal characters or " + e.Message);
            }

            return new ValidationResult(true, null);
        }
    }
}