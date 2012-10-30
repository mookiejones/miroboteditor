﻿using System;
using System.ComponentModel;
using miRobotEditor.Core.Convertors;

namespace miRobotEditor.Core
{
    /// <summary>
    /// LocalizedPropertyDescriptor enhances the base class bay obtaining the display name for a property
    /// from the resource.
    /// </summary>
    public class LocalizedPropertyDescriptor : PropertyDescriptor
    {
        PropertyDescriptor basePropertyDescriptor;

        string localizedName = String.Empty;
        string localizedDescription = String.Empty;
        string localizedCategory = String.Empty;

        TypeConverter customTypeConverter = null;

        public override bool IsReadOnly
        {
            get
            {
                return this.basePropertyDescriptor.IsReadOnly;
            }
        }

        public override string Name
        {
            get
            {
                return this.basePropertyDescriptor.Name;
            }
        }

        public override Type PropertyType
        {
            get
            {
                return this.basePropertyDescriptor.PropertyType;
            }
        }

        public override Type ComponentType
        {
            get
            {
                return basePropertyDescriptor.ComponentType;
            }
        }

       

        public override TypeConverter Converter
        {
            get
            {
                if (customTypeConverter != null)
                {
                    return customTypeConverter;
                }
                return base.Converter;
            }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="basePropertyDescriptor"></param>
        public LocalizedPropertyDescriptor(PropertyDescriptor basePropertyDescriptor)
            : base(basePropertyDescriptor)
        {
            LocalizedPropertyAttribute localizedPropertyAttribute = null;

            foreach (Attribute attr in basePropertyDescriptor.Attributes)
            {
                localizedPropertyAttribute = attr as LocalizedPropertyAttribute;
                if (localizedPropertyAttribute != null)
                {
                    break;
                }
            }

            if (localizedPropertyAttribute != null)
            {
                localizedName = localizedPropertyAttribute.Name;
                localizedDescription = localizedPropertyAttribute.Description;
                localizedCategory = localizedPropertyAttribute.Category;
            }
            else
            {
                localizedName = basePropertyDescriptor.Name;
                localizedDescription = basePropertyDescriptor.Description;
                localizedCategory = basePropertyDescriptor.Category;
            }

            this.basePropertyDescriptor = basePropertyDescriptor;

            // "Booleans" get a localized type converter
            if (basePropertyDescriptor.PropertyType == typeof(System.Boolean))
            {
                customTypeConverter = new BooleanTypeConverter();
            }
        }

        public override bool CanResetValue(object component)
        {
            return basePropertyDescriptor.CanResetValue(component);
        }

        public override object GetValue(object component)
        {
            return this.basePropertyDescriptor.GetValue(component);
        }

        public override void ResetValue(object component)
        {
            this.basePropertyDescriptor.ResetValue(component);
            if (component is LocalizedObject)
            {
                ((LocalizedObject)component).InformSetValue(this, component, null);
            }
        }

        public override bool ShouldSerializeValue(object component)
        {
            return this.basePropertyDescriptor.ShouldSerializeValue(component);
        }

        public override void SetValue(object component, object value)
        {
            if (this.customTypeConverter != null && value.GetType() != PropertyType)
            {
                this.basePropertyDescriptor.SetValue(component, this.customTypeConverter.ConvertFrom(value));
            }
            else
            {
                this.basePropertyDescriptor.SetValue(component, value);
            }
            if (component is LocalizedObject)
            {
                ((LocalizedObject)component).InformSetValue(this, component, value);
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
	public sealed class LocalizedPropertyAttribute : Attribute
	{
		
		
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Category
        /// </summary>
        public string Category { get; set; }
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		public LocalizedPropertyAttribute(string name)
		{
			this.Name = name;
		}
	}
}
