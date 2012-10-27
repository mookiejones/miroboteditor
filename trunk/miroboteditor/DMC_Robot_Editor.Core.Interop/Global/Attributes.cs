using System;

namespace DMC_Robot_Editor.Global
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Delegate, AllowMultiple = false, Inherited = true)]
    public class ExcludeFromAutomaticTestAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = false)]
    public class CanBeEmptyAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class CanBeNegativeAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class CanBeNoneAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = false)]
    public class CanBeNullAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = false)]
    public class CanBeUnknownAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = false)]
    public class CanBeZeroAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.All)]
    public class ExcludeFromCodeCoverageAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class InvalidStringAttribute : StringAttribute
    {
        public InvalidStringAttribute(string value)
            : base(value)
        {
        }
    }
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = false)]
    public class MustBeGreaterThanAttribute : ParameterAttribute
    {
        public MustBeGreaterThanAttribute(string parameterName)
            : base(parameterName)
        {
        }
    }
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = false)]
    public class MustBeGreaterThanOrEqualToAttribute : ParameterAttribute
    {
        public MustBeGreaterThanOrEqualToAttribute(string parameterName)
            : base(parameterName)
        {
        }
    }
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = false)]
    public class MustNotHaveValueAttribute : ValidStringAttribute
    {
        public MustNotHaveValueAttribute(string value)
            : base(value.ToUpperInvariant())
        {
        }
        public MustNotHaveValueAttribute(bool value)
            : this(value.ToString())
        {
        }
    }
    public abstract class ParameterAttribute : Attribute
    {
        public string ParameterName
        {
            get;
            private set;
        }
        protected ParameterAttribute(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                throw new ArgumentNullException("parameterName");
            }
            ParameterName = parameterName;
        }
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
    public class RangeAttribute : Attribute
    {
        public long MinValue
        {
            get;
            private set;
        }
        public long MaxValue
        {
            get;
            private set;
        }
        public RangeAttribute([CanBeNegative, CanBeZero] long minValue, [CanBeNegative, CanBeZero, MustBeGreaterThanOrEqualTo("minValue")] long maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentException("minValue > maxValue");
            }
            MinValue = minValue;
            MaxValue = maxValue;
        }
    }
    [AttributeUsage(AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = false)]
    public class RegularExpressionAttribute : StringAttribute
    {
        public RegularExpressionAttribute(string value)
            : base(value)
        {
        }
    }
    public abstract class StringAttribute : Attribute
    {
        public string Value
        {
            get;
            private set;
        }
        protected StringAttribute(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }
            Value = value;
        }
    }
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class ThrowsAttribute : Attribute
    {
        public Type Exception
        {
            get;
            private set;
        }
        public ThrowsAttribute(Type exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            if (!typeof(Exception).IsAssignableFrom(exception))
            {
                throw new ArgumentException("exception does not inherit from Exception");
            }
            Exception = exception;
        }
    }
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = true)]
    public class UseAsMockUpAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class UsesServiceAttribute : Attribute
    {
        public Type ServiceType
        {
            get;
            private set;
        }
        public UsesServiceAttribute(Type service)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            if (!service.IsInterface)
            {
                throw new ArgumentException("service is not an interface");
            }
            ServiceType = service;
        }
    }
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class ValidStringAttribute : StringAttribute
    {
        public ValidStringAttribute(string value)
            : base(value)
        {
        }
    }
}
