using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.ComponentModel;
namespace miRobotEditor.ValidationRules
{
    public class IsNumericValidationRule:ValidationRule,IDataErrorInfo
    {

        private string _fieldName;
        public string FieldName
        {
            get { return _fieldName; }
            set { _fieldName = value; }
        }

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string columnName]
        {
            get { throw new NotImplementedException(); }
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            int num = 0;

            if (!int.TryParse(value.ToString(), out num))
                return new ValidationResult(false, String.Format("{0} must contain an integer value.", FieldName));

            return new ValidationResult(true, null);
        }


    }
}
