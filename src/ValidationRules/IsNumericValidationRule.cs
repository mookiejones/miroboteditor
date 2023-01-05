using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;

namespace miRobotEditor.ValidationRules
{
    [Localizable(false)]
// ReSharper disable once UnusedMember.Global
    public class IsNumericValidationRule : ValidationRule, IDataErrorInfo
    {
        public IsNumericValidationRule(string fieldName)
        {
            FieldName = fieldName;
        }

        public string FieldName { get; set; }

        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get { throw new NotImplementedException(); }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int num;

            if (value != null && !int.TryParse(value.ToString(), out num))
                return new ValidationResult(false, String.Format("{0} must contain an integer value.", FieldName));

            return new ValidationResult(true, null);
        }
    }
}