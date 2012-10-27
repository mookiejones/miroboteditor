using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using miRobotEditor.Core.Services;

namespace miRobotEditor.Core.Services
{
    public static class ClassBrowserIconService
    {
        #region WinForms ImageList
        static ImageList imglist;

        /// <summary>
        /// Gets the ImageList.
        /// Do not directly add images to the list, you must use <see cref="AddImage"/>!
        /// </summary>
        public static ImageList ImageList
        {
            get
            {
                Gui.WorkbenchSingleton.AssertMainThread();
                if (imglist == null)
                {
                    lock (lockObj)
                    {
                        imglist = new ImageList();
                        AddNewImagesToList();
                    }
                }
                return imglist;
            }
        }

        static void AddNewImagesToList()
        {
            lock (lockObj)
            {
                if (imglist.Images.Count > imglistEntries.Count)
                    throw new InvalidOperationException("Too many images in list (list was modified externally?)");
                while (imglist.Images.Count < imglistEntries.Count)
                {
                    imglist.Images.Add(imglistEntries[imglist.Images.Count].Bitmap);
                }
            }
        }

        static readonly object lockObj = new Object();
        static readonly List<ClassBrowserImage> imglistEntries = new List<ClassBrowserImage>();

        public static ClassBrowserImage GetImageByIndex(int index)
        {
            lock (lockObj)
            {
                return imglistEntries[index];
            }
        }

        static ClassBrowserImage AddImage(string resourceName)
        {
            return AddImage(new ResourceServiceImage(resourceName));
        }

        public static ClassBrowserImage AddImage(IImage baseImage)
        {
            if (baseImage == null)
                throw new ArgumentNullException("baseImage");
            ClassBrowserImage image;
            bool imgListPresent;
            lock (lockObj)
            {
                image = new ClassBrowserImage(baseImage, imglistEntries.Count);
                imglistEntries.Add(image);
                imgListPresent = (imglist != null);
            }
            // We need to do the call outside the lock to prevent deadlocks.
            // We cannot use an async call because we need to ensure that the image is added to the list
            // before we return.
            if (imgListPresent)
            {
                Gui.WorkbenchSingleton.SafeThreadCall(AddNewImagesToList);
            }
            return image;
        }
        #endregion

        public static readonly System.Windows.Size ImageSize = new System.Windows.Size(16, 16);

        #region Entity Images
        static readonly ClassBrowserImage[] entityImages = {
			AddImage("Icons.16x16.Class"),
			AddImage("Icons.16x16.InternalClass"),
			AddImage("Icons.16x16.ProtectedClass"),
			AddImage("Icons.16x16.PrivateClass"),
			
			AddImage("Icons.16x16.Struct"),
			AddImage("Icons.16x16.InternalStruct"),
			AddImage("Icons.16x16.ProtectedStruct"),
			AddImage("Icons.16x16.PrivateStruct"),
			
			AddImage("Icons.16x16.Interface"),
			AddImage("Icons.16x16.InternalInterface"),
			AddImage("Icons.16x16.ProtectedInterface"),
			AddImage("Icons.16x16.PrivateInterface"),
			
			AddImage("Icons.16x16.Enum"),
			AddImage("Icons.16x16.InternalEnum"),
			AddImage("Icons.16x16.ProtectedEnum"),
			AddImage("Icons.16x16.PrivateEnum"),
			
			AddImage("Icons.16x16.Method"),
			AddImage("Icons.16x16.InternalMethod"),
			AddImage("Icons.16x16.ProtectedMethod"),
			AddImage("Icons.16x16.PrivateMethod"),
			
			AddImage("Icons.16x16.Property"),
			AddImage("Icons.16x16.InternalProperty"),
			AddImage("Icons.16x16.ProtectedProperty"),
			AddImage("Icons.16x16.PrivateProperty"),
			
			AddImage("Icons.16x16.Field"),
			AddImage("Icons.16x16.InternalField"),
			AddImage("Icons.16x16.ProtectedField"),
			AddImage("Icons.16x16.PrivateField"),
			
			AddImage("Icons.16x16.Delegate"),
			AddImage("Icons.16x16.InternalDelegate"),
			AddImage("Icons.16x16.ProtectedDelegate"),
			AddImage("Icons.16x16.PrivateDelegate"),
			
			AddImage("Icons.16x16.Event"),
			AddImage("Icons.16x16.InternalEvent"),
			AddImage("Icons.16x16.ProtectedEvent"),
			AddImage("Icons.16x16.PrivateEvent"),
			
			AddImage("Icons.16x16.Indexer"),
			AddImage("Icons.16x16.InternalIndexer"),
			AddImage("Icons.16x16.ProtectedIndexer"),
			AddImage("Icons.16x16.PrivateIndexer"),
			
			AddImage("Icons.16x16.ExtensionMethod"),
			AddImage("Icons.16x16.InternalExtensionMethod"),
			AddImage("Icons.16x16.ProtectedExtensionMethod"),
			AddImage("Icons.16x16.PrivateExtensionMethod")
		};

        const int ClassIndex = 0;
        const int StructIndex = ClassIndex + 1 * 4;
        const int InterfaceIndex = ClassIndex + 2 * 4;
        const int EnumIndex = ClassIndex + 3 * 4;
        const int MethodIndex = ClassIndex + 4 * 4;
        const int PropertyIndex = ClassIndex + 5 * 4;
        const int FieldIndex = ClassIndex + 6 * 4;
        const int DelegateIndex = ClassIndex + 7 * 4;
        const int EventIndex = ClassIndex + 8 * 4;
        const int IndexerIndex = ClassIndex + 9 * 4;
        const int ExtensionMethodIndex = ClassIndex + 10 * 4;

        const int internalModifierOffset = 1;
        const int protectedModifierOffset = 2;
        const int privateModifierOffset = 3;

        public static readonly ClassBrowserImage Class = entityImages[ClassIndex];
        public static readonly ClassBrowserImage Struct = entityImages[StructIndex];
        public static readonly ClassBrowserImage Interface = entityImages[InterfaceIndex];
        public static readonly ClassBrowserImage Enum = entityImages[EnumIndex];
        public static readonly ClassBrowserImage Method = entityImages[MethodIndex];
        public static readonly ClassBrowserImage Property = entityImages[PropertyIndex];
        public static readonly ClassBrowserImage Field = entityImages[FieldIndex];
        public static readonly ClassBrowserImage Delegate = entityImages[DelegateIndex];
        public static readonly ClassBrowserImage Event = entityImages[EventIndex];
        public static readonly ClassBrowserImage Indexer = entityImages[IndexerIndex];
        #endregion

        #region Get Methods for Entity Images

       
        public static ClassBrowserImage GetIcon(IEntity entity)
        {
            if (entity is IMethod)
                return GetIcon(entity as IMethod);
            else if (entity is IProperty)
                return GetIcon(entity as IProperty);
            else if (entity is IField)
                return GetIcon(entity as IField);
            else if (entity is IEvent)
                return GetIcon(entity as IEvent);
            else if (entity is IClass)
                return GetIcon(entity as IClass);
            else
                throw new ArgumentException("unknown entity type");
        }

        public static ClassBrowserImage GetIcon(IMethod method)
        {
            if (method.IsOperator)
                return Operator;
            else if (method.IsExtensionMethod)
                return entityImages[ExtensionMethodIndex + GetModifierOffset(method.Modifiers)];
            else
                return entityImages[MethodIndex + GetModifierOffset(method.Modifiers)];
        }

        public static ClassBrowserImage GetIcon(IProperty property)
        {
            if (property.IsIndexer)
                return entityImages[IndexerIndex + GetModifierOffset(property.Modifiers)];
            else
                return entityImages[PropertyIndex + GetModifierOffset(property.Modifiers)];
        }

        public static ClassBrowserImage GetIcon(IField field)
        {
            if (field.IsConst)
            {
                return Const;
            }
            else if (field.IsParameter)
            {
                return Parameter;
            }
            else if (field.IsLocalVariable)
            {
                return LocalVariable;
            }
            else
            {
                return entityImages[FieldIndex + GetModifierOffset(field.Modifiers)];
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        public static ClassBrowserImage GetIcon(IEvent evt)
        {
            return entityImages[EventIndex + GetModifierOffset(evt.Modifiers)];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static ClassBrowserImage GetIcon(IClass c)
        {
            int imageIndex = ClassIndex;
            switch (c.ClassType)
            {
                case ClassType.Delegate:
                    imageIndex = DelegateIndex;
                    break;
                case ClassType.Enum:
                    imageIndex = EnumIndex;
                    break;
                case ClassType.Struct:
                    imageIndex = StructIndex;
                    break;
                case ClassType.Interface:
                    imageIndex = InterfaceIndex;
                    break;
            }
            return entityImages[imageIndex + GetModifierOffset(c.Modifiers)];
        }

        static int GetVisibilityOffset(MethodBase methodinfo)
        {
            if (methodinfo.IsAssembly)
            {
                return internalModifierOffset;
            }
            if (methodinfo.IsPrivate)
            {
                return privateModifierOffset;
            }
            if (!(methodinfo.IsPrivate || methodinfo.IsPublic))
            {
                return protectedModifierOffset;
            }
            return 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodinfo"></param>
        /// <returns></returns>
        public static ClassBrowserImage GetIcon(MethodBase methodinfo)
        {
            return entityImages[MethodIndex + GetVisibilityOffset(methodinfo)];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyinfo"></param>
        /// <returns></returns>
        public static ClassBrowserImage GetIcon(PropertyInfo propertyinfo)
        {
            if (propertyinfo.CanRead && propertyinfo.GetGetMethod(true) != null)
            {
                return entityImages[PropertyIndex + GetVisibilityOffset(propertyinfo.GetGetMethod(true))];
            }
            if (propertyinfo.CanWrite && propertyinfo.GetSetMethod(true) != null)
            {
                return entityImages[PropertyIndex + GetVisibilityOffset(propertyinfo.GetSetMethod(true))];
            }
            return entityImages[PropertyIndex];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldinfo"></param>
        /// <returns></returns>
        public static ClassBrowserImage GetIcon(FieldInfo fieldinfo)
        {
            if (fieldinfo.IsLiteral)
            {
                return Const;
            }

            if (fieldinfo.IsAssembly)
            {
                return entityImages[FieldIndex + internalModifierOffset];
            }

            if (fieldinfo.IsPrivate)
            {
                return entityImages[FieldIndex + privateModifierOffset];
            }

            if (!(fieldinfo.IsPrivate || fieldinfo.IsPublic))
            {
                return entityImages[FieldIndex + protectedModifierOffset];
            }

            return entityImages[FieldIndex];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventinfo"></param>
        /// <returns></returns>
        public static ClassBrowserImage GetIcon(EventInfo eventinfo)
        {
            if (eventinfo.GetAddMethod(true) != null)
            {
                return entityImages[EventIndex + GetVisibilityOffset(eventinfo.GetAddMethod(true))];
            }
            return entityImages[EventIndex];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ClassBrowserImage GetIcon(System.Type type)
        {
            int BASE = ClassIndex;

            if (type.IsValueType)
            {
                BASE = StructIndex;
            }
            if (type.IsEnum)
            {
                BASE = EnumIndex;
            }
            if (type.IsInterface)
            {
                BASE = InterfaceIndex;
            }
            if (type.IsSubclassOf(typeof(System.Delegate)))
            {
                BASE = DelegateIndex;
            }

            if (type.IsNestedPrivate)
            {
                return entityImages[BASE + privateModifierOffset];
            }

            if (type.IsNotPublic || type.IsNestedAssembly)
            {
                return entityImages[BASE + internalModifierOffset];
            }

            if (type.IsNestedFamily)
            {
                return entityImages[BASE + protectedModifierOffset];
            }
            return entityImages[BASE];
        }
        #endregion

#pragma warning disable 1591
        public static readonly ClassBrowserImage Namespace = AddImage("Icons.16x16.NameSpace");
        public static readonly ClassBrowserImage Solution = AddImage("Icons.16x16.CombineIcon");
        public static readonly ClassBrowserImage Const = AddImage("Icons.16x16.Literal");
        public static readonly ClassBrowserImage GotoArrow = AddImage("Icons.16x16.SelectionArrow");

        public static readonly ClassBrowserImage LocalVariable = AddImage("Icons.16x16.Local");
        public static readonly ClassBrowserImage Parameter = AddImage("Icons.16x16.Parameter");
        public static readonly ClassBrowserImage Keyword = AddImage("Icons.16x16.Keyword");
        public static readonly ClassBrowserImage Operator = AddImage("Icons.16x16.Operator");
        public static readonly ClassBrowserImage CodeTemplate = AddImage("Icons.16x16.TextFileIcon");
#pragma warning restore 1591

    }
    /// <summary>
    /// 
    /// </summary>
    public class ClassBrowserImage : IImage
    {
        readonly IImage baseImage;

        public System.Windows.Media.ImageSource ImageSource
        {
            get { return baseImage.ImageSource; }
        }

        public System.Drawing.Bitmap Bitmap
        {
            get { return baseImage.Bitmap; }
        }

        public System.Drawing.Icon Icon
        {
            get { return baseImage.Icon; }
        }

        /// <summary>
        /// The image's index in the ClassBrowserIconService.ImageList.
        /// </summary>
        public int ImageIndex { get; private set; }

        internal ClassBrowserImage(IImage baseImage, int index)
        {
            this.baseImage = baseImage;
            this.ImageIndex = index;
        }
    }
    public interface ICompletionEntry
	{
		string Name {
			get;
		}
	}
    /// <summary>
    /// 
    /// </summary>
    public interface IMethodOrProperty : IMember
    {///
        IList<IParameter> Parameters
        {
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        bool IsExtensionMethod
        {
            get;
        }
    }

    public interface IEntity : ICompletionEntry, IFreezable, IComparable
	{
		string FullyQualifiedName {
			get;
		}
		
		string Namespace {
			get;
		}
		
		/// <summary>
		/// The fully qualified name in the internal .NET notation (with `1 for generic types)
		/// </summary>
		string DotNetName {
			get;
		}
		
		
		
		
		IList<IAttribute> Attributes {
			get;
		}
		
		string Documentation {
			get;
		}

		/// <summary>
		/// Returns true if this entity has the 'abstract' modifier set. 
		/// (Returns false for interface members).
		/// </summary>
		bool IsAbstract {
			get;
		}

		bool IsSealed {
			get;
		}

		/// <summary>
		/// Gets whether this entity is static.
		/// Returns true if either the 'static' or the 'const' modifier is set.
		/// </summary>
		bool IsStatic {
			get;
		}
		
		/// <summary>
		/// Gets whether this entity is a constant (C#-like const).
		/// </summary>
		bool IsConst {
			get;
		}

		/// <summary>
		/// Gets if the member is virtual. Is true only if the "virtual" modifier was used, but non-virtual
		/// members can be overridden, too; if they are already overriding a method.
		/// </summary>
		bool IsVirtual {
			get;
		}

		bool IsPublic {
			get;
		}

		bool IsProtected {
			get;
		}

		bool IsPrivate {
			get;
		}

		bool IsInternal {
			get;
		}

		bool IsReadonly {
			get;
		}

		
		bool IsOverride {
			get;
		}
		/// <summary>
		/// Gets if the member can be overridden. Returns true when the member is "virtual" or "override" but not "sealed".
		/// </summary>
		bool IsOverridable {
			get;
		}
		
		bool IsNew {
			get;
		}
		bool IsSynthetic {
			get;
		}
		
		
		
		/// <summary>
		/// This property can be used to attach any user-defined data to this class/method.
		/// This property is mutable, it can be changed when the class/method is frozen.
		/// </summary>
		object UserData {
			get;
			set;
		}
		
		EntityType EntityType {
			get;
		}
		
		bool IsAccessible(IClass callingClass, bool isAccessThoughReferenceOfCurrentClass);
	}
	
	public enum EntityType {
		Class,
		Field,
		Property,
		Method,
		Event
	}
    public interface IMember : IEntity, ICloneable
	{


        /// <summary>
        /// Gets/Sets the declaring type reference (declaring type incl. type arguments).
        /// Never returns null.
        /// If the property is set to null (e.g. when this is not a specialized member),
        /// it should return the default type reference to the <see cref="DeclaringType"/>.
        /// </summary>
        IReturnType DeclaringTypeReference { get; set; }

        /// <summary>
		/// Gets the generic member this member is based on.
		/// Returns null if this is not a specialized member.
		/// Specialized members are the result of overload resolution with type substitution.
		/// </summary>
		IMember GenericMember {
			get;
		}
		
		/// <summary>
		/// Creates a copy of this member with its GenericMember property set to this member.
		/// Use this method to create copies of a member that should be regarded as the "same member"
		/// for refactoring purposes.
		/// </summary>
		IMember CreateSpecializedMember();
		
		IReturnType ReturnType {
			get;
			set;
		}
		
	}

    /// <summary>
    /// 
    /// </summary>
    public interface IParameter : IFreezable, IComparable
    {
        /// <summary>
        /// Name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Return Type
        /// </summary>
        IReturnType ReturnType { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IAttribute : IFreezable
        {
           

            
            /// <summary>
            /// 
            /// </summary>
            AttributeTarget AttributeTarget
            {
                get;
            }
            /// <summary>
            /// 
            /// </summary>
            IReturnType AttributeType
            {
                get;
            }
            /// <summary>
            /// 
            /// </summary>
            IList<object> PositionalArguments
            {
                get;
            }
            /// <summary>
            /// 
            /// </summary>
            IDictionary<string, object> NamedArguments
            {
                get;
            }
        

  


    
        IList<IAttribute> Attributes
        {
            get;
        }

        ParameterModifiers Modifiers
        {
            get;
        }


        string Documentation
        {
            get;
        }

        bool IsOut
        {
            get;
        }

        bool IsRef
        {
            get;
        }

        bool IsParams
        {
            get;
        }

        bool IsOptional
        {
            get;
        }
    }
    /// <summary>
	/// Interface for reference to types (classes).
	/// Such a reference can be direct (DefaultReturnType), lazy (SearchClassReturnType) or
	/// returns types that stand for special references (e.g. ArrayReturnType)
	/// </summary>
	public interface IReturnType : IEquatable<IReturnType>
	{
		/// <summary>
		/// Gets the fully qualified name of the class the return type is pointing to.
		/// </summary>
		/// <returns>
		/// "System.Int32" for int[]<br/>
		/// "System.Collections.Generic.List" for List&lt;string&gt;
		/// </returns>
		string FullyQualifiedName {
			get;
		}
		
		/// <summary>
		/// Gets the short name of the class the return type is pointing to.
		/// </summary>
		/// <returns>
		/// "Int32" or "int" (depending how the return type was created) for int[]<br/>
		/// "List" for List&lt;string&gt;
		/// </returns>
		string Name {
			get;
		}
		
		/// <summary>
		/// Gets the namespace of the class the return type is pointing to.
		/// </summary>
		/// <returns>
		/// "System" for int[]<br/>
		/// "System.Collections.Generic" for List&lt;string&gt;
		/// </returns>
		string Namespace {
			get;
		}
		
		/// <summary>
		/// Gets the full dotnet name of the return type. The DotnetName is used for the
		/// documentation tags.
		/// </summary>
		/// <returns>
		/// "System.Int[]" for int[]<br/>
		/// "System.Collections.Generic.List{System.String}" for List&lt;string&gt;
		/// </returns>
		string DotNetName {
			get;
		}
		
		/// <summary>
		/// Gets the number of type parameters the target class should have
		/// / the number of type arguments specified by this type reference.
		/// </summary>
		int TypeArgumentCount {
			get;
		}
		
		/// <summary>
		/// Gets the underlying class of this return type. This method will return <c>null</c> for
		/// generic return types and types that cannot be resolved.
		/// </summary>
		IClass GetUnderlyingClass();
		
		/// <summary>
		/// Gets all methods that can be called on this return type.
		/// </summary>
		List<IMethod> GetMethods();
		
		/// <summary>
		/// Gets all properties that can be called on this return type.
		/// </summary>
		List<IProperty> GetProperties();
		
		/// <summary>
		/// Gets all fields that can be called on this return type.
		/// </summary>
		List<IField> GetFields();
		
		/// <summary>
		/// Gets all events that can be called on this return type.
		/// </summary>
		List<IEvent> GetEvents();
		
		
		/// <summary>
		/// Gets if the return type is a default type, i.e. no array, generic etc.
		/// </summary>
		/// <returns>
		/// True for SearchClassReturnType, GetClassReturnType and DefaultReturnType.<br/>
		/// False for ArrayReturnType, SpecificReturnType etc.
		/// </returns>
		bool IsDefaultReturnType { get; }
		
		/// <summary>
		/// Gets if the cast to the specified decorating return type would be valid.
		/// </summary>
		bool IsDecoratingReturnType<T>() where T : DecoratingReturnType;
		
		/// <summary>
		/// Casts this return type to the decorating return type specified as type parameter.
		/// This methods casts correctly even when the return type is wrapped by a ProxyReturnType.
		/// When the cast is invalid, <c>null</c> is returned.
		/// </summary>
		T CastToDecoratingReturnType<T>() where T : DecoratingReturnType;
		
		bool IsConstructedReturnType { get; }
		ConstructedReturnType CastToConstructedReturnType();
		
		/// <summary>
		/// Gets whether the type is a reference type or value type.
		/// </summary>
		/// <returns>
		/// true, if the type is a reference type.
		/// false, if the type is a value type.
		/// null, if the type is not known (e.g. generic type argument or type not found)
		/// </returns>
		bool? IsReferenceType { get; }
		
		/// <summary>
		/// Gets an identical return type that binds directly to the underlying class, so
		/// that repeatedly calling methods does not cause repeated class lookups.
		/// The direct return type will always point to the old version of the class, so don't
		/// store direct return types!
		/// </summary>
		/// <returns>This method never returns null.</returns>
		IReturnType GetDirectReturnType();
	}


    /// <summary>
    /// 
    /// </summary>
    public interface IMethod : IMethodOrProperty
    {
        /// <summary>
        /// 
        /// </summary>
        IList<ITypeParameter> TypeParameters
        {
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        bool IsConstructor
        {
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        IList<string> HandlesClauses
        {
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        bool IsOperator
        {
            get;
        }
    }
    /// <summary>
    /// Type parameter of a generic class/method.
    /// </summary>
    public interface ITypeParameter : IFreezable
    {
        /// <summary>
        /// The name of the type parameter (for example "T")
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the index of the type parameter in the type parameter list of the owning method/class.
        /// </summary>
        int Index { get; }
        /// <summary>
        /// 
        /// </summary>
        IList<IAttribute> Attributes { get; }

        /// <summary>
        /// The method this type parameter is defined for.
        /// This property is null when the type parameter is for a class.
        /// </summary>
        IMethod Method { get; }

        /// <summary>
        /// The class this type parameter is defined for.
        /// When the type parameter is defined for a method, this is the class containing
        /// that method.
        /// </summary>
        IClass Class { get; }

        /// <summary>
        /// Gets the contraints of this type parameter.
        /// </summary>
        IList<IReturnType> Constraints { get; }

        /// <summary>
        /// Gets if the type parameter has the 'new()' constraint.
        /// </summary>
        bool HasConstructableConstraint { get; }

        /// <summary>
        /// Gets if the type parameter has the 'class' constraint.
        /// </summary>
        bool HasReferenceTypeConstraint { get; }

        /// <summary>
        /// Gets if the type parameter has the 'struct' constraint.
        /// </summary>
        bool HasValueTypeConstraint { get; }

        /// <summary>
        /// Gets the type that was used to bind this type parameter.
        /// This property returns null for generic methods/classes, it
        /// is non-null only for constructed versions of generic methods.
        /// </summary>
        IReturnType BoundTo { get; }

        /// <summary>
        /// If this type parameter was bound, returns the unbound version of it.
        /// </summary>
        ITypeParameter UnboundTypeParameter { get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IFreezable
    {
        /// <summary>
        /// Gets if this instance is frozen. Frozen instances are immutable and thus thread-safe.
        /// </summary>
        bool IsFrozen { get; }

        /// <summary>
        /// Freezes this instance.
        /// </summary>
        void Freeze();
    }
    [Serializable]
    [Flags]
    public enum ParameterModifiers : byte
    {
        // Values must be the same as in NRefactory's ParamModifiers
        None = 0,
        In = 1,
        Out = 2,
        Ref = 4,
        Params = 8,
        Optional = 16,
        This = 32
    }
    public enum AttributeTarget
    {
        None,
        Assembly,
        Field,
        Event,
        Method,
        Module,
        Param,
        Property,
        Return,
        Type
    }
    public interface IClass : IEntity
    {
      

        /// <summary>
        /// The default return type to use for this class.
        /// This property is mutable even when the IClass is frozen, see
        /// documentation for <see cref="HasCompoundClass"/>.
        /// This property is thread-safe.
        /// </summary>
        IReturnType DefaultReturnType { get; }

        ClassType ClassType
        {
            get;
        }

        IList<IReturnType> BaseTypes
        {
            get;
        }

        IList<IClass> InnerClasses
        {
            get;
        }

        IList<IField> Fields
        {
            get;
        }

        IList<IProperty> Properties
        {
            get;
        }

        IList<IMethod> Methods
        {
            get;
        }

        IList<IEvent> Events
        {
            get;
        }

        IEnumerable<IMember> AllMembers
        {
            get;
        }

        IList<ITypeParameter> TypeParameters
        {
            get;
        }

        /// <summary>
        /// Returns the list of all classes that this class inherits from (directly or indirectly).
        /// If this property is used on part of a partial class, it will also return classes inherited in other parts.
        /// </summary>
        IEnumerable<IClass> ClassInheritanceTree
        {
            get;
        }

        IEnumerable<IClass> ClassInheritanceTreeClassesOnly
        {
            get;
        }

        IClass BaseClass
        {
            get;
        }

        IReturnType BaseType
        {
            get;
        }

        /// <summary>
        /// If this is a partial class, gets the compound class containing information from all parts.
        /// If this is not a partial class, a reference to this class is returned.
        /// </summary>
        IClass GetCompoundClass();

        IClass GetInnermostClass(int caretLine, int caretColumn);

        List<IClass> GetAccessibleTypes(IClass callingClass);

        /// <summary>
        /// Searches the member with the specified name. Returns the first member/overload found.
        /// </summary>
        IMember SearchMember(string memberName, LanguageProperties language);

        /// <summary>Return true if the specified class is a base class of this class; otherwise return false.</summary>
        /// <remarks>Returns false when possibleBaseClass is null.</remarks>
        bool IsTypeInInheritanceTree(IClass possibleBaseClass);

        bool HasPublicOrInternalStaticMembers
        {
            get;
        }
        bool HasExtensionMethods
        {
            get;
        }

        bool IsPartial
        {
            get;
        }

        /// <summary>
        /// Gets/sets if this class has an associated compound class.
        /// This property is mutable even if the IClass instance is frozen.
        /// This property is thread-safe.
        /// 
        /// This property may only be set by the IProjectContent implementation to which this class is added.
        /// 
        /// Rational: some languages support partial classes where only one of the parts needs
        /// the "partial" modifier. If the part without the modifier is added to the project
        /// content first, it is added without compound class and may get a DefaultReturnType that refers
        /// to itself.
        /// However, when the other part with the modifier is added, a compound class is created, and the
        /// DefaultReturnType of this class must change even though it is frozen.
        /// </summary>
        bool HasCompoundClass
        {
            get;
            set;
        }

        /// <summary>
        /// Gets whether a default constructor should be added to this class if it is required.
        /// Such automatic default constructors will not appear in IClass.Methods, but will be present
        /// in IClass.DefaultReturnType.GetMethods().
        /// </summary>
        /// <remarks>This way of creating the default constructor is necessary because
        /// we cannot create it directly in the IClass - we need to consider partial classes.</remarks>
        bool AddDefaultConstructorIfRequired
        {
            get;
        }
    }
    public interface IProperty : IMethodOrProperty
    {
       

        bool CanGet
        {
            get;
        }

        bool CanSet
        {
            get;
        }

        bool IsIndexer
        {
            get;
        }

      
    }
    public class LanguageProperties
    {
        /// <summary>
        /// A case-sensitive dummy language that returns false for all Supports.. properties,
        /// uses a dummy code generator and refactoring provider and returns null for CodeDomProvider.
        /// </summary>
        public readonly static LanguageProperties None = new LanguageProperties(StringComparer.Ordinal);

        /// <summary>
        /// C# 3.0 language properties.
        /// </summary>
        public readonly static LanguageProperties CSharp = new CSharpProperties();

        /// <summary>
        /// VB.Net 8 language properties.
        /// </summary>
        public readonly static LanguageProperties VBNet = new VBNetProperties();

        public LanguageProperties(StringComparer nameComparer)
        {
            this.nameComparer = nameComparer;
        }

        #region Language-specific service providers
        readonly StringComparer nameComparer;

        public StringComparer NameComparer
        {
            get
            {
                return nameComparer;
            }
        }

     
        /// <summary>
        /// Gets the CodeDomProvider for this language. Can return null!
        /// </summary>
        public virtual System.CodeDom.Compiler.CodeDomProvider CodeDomProvider
        {
            get
            {
                return null;
            }
        }

        sealed class DummyCodeDomProvider : System.CodeDom.Compiler.CodeDomProvider
        {
            public static readonly DummyCodeDomProvider Instance = new DummyCodeDomProvider();

            [Obsolete("Callers should not use the ICodeGenerator interface and should instead use the methods directly on the CodeDomProvider class.")]
            public override System.CodeDom.Compiler.ICodeGenerator CreateGenerator()
            {
                return null;
            }

            [Obsolete("Callers should not use the ICodeCompiler interface and should instead use the methods directly on the CodeDomProvider class.")]
            public override System.CodeDom.Compiler.ICodeCompiler CreateCompiler()
            {
                return null;
            }
        }
        #endregion

        #region Supports...
        /// <summary>
        /// Gets if the language supports calling C# 3-style extension methods
        /// (first parameter = instance parameter)
        /// </summary>
        public virtual bool SupportsExtensionMethods
        {
            get { return false; }
        }

        /// <summary>
        /// Gets if the language supports calling extension properties
        /// (first parameter = instance parameter)
        /// </summary>
        public virtual bool SupportsExtensionProperties
        {
            get { return false; }
        }

        /// <summary>
        /// Gets if extension methods/properties are searched in imported classes (returns true) or if
        /// only the extensions from the current class, imported classes and imported modules are used
        /// (returns false). This property has no effect if the language doesn't support extension methods or properties.
        /// </summary>
        public virtual bool SearchExtensionsInClasses
        {
            get { return false; }
        }

        /// <summary>
        /// Gets if namespaces are imported (i.e. Imports System, Dim a As Collections.ArrayList)
        /// </summary>
        public virtual bool ImportNamespaces
        {
            get { return false; }
        }

        /// <summary>
        /// Gets if modules are imported with their namespace (i.e. Microsoft.VisualBasic.Randomize()).
        /// </summary>
        public virtual bool ImportModules
        {
            get { return false; }
        }

        /// <summary>
        /// Gets if classes can be imported (i.e. Imports System.Math)
        /// </summary>
        public virtual bool CanImportClasses
        {
            get { return false; }
        }

        /// <summary>
        /// Gets if the language allows partial classes where the partial modifier is not
        /// used on any part.
        /// </summary>
        public virtual bool ImplicitPartialClasses
        {
            get { return false; }
        }

        /// <summary>
        /// Allow invoking an object constructor outside of ExpressionContext.ObjectCreation.
        /// Used for Boo, which creates instances like this: 'self.Size = Size(10, 20)'
        /// </summary>
        public virtual bool AllowObjectConstructionOutsideContext
        {
            get { return false; }
        }

        /// <summary>
        /// Gets if the language supports implicit interface implementations.
        /// </summary>
        public virtual bool SupportsImplicitInterfaceImplementation
        {
            get { return false; }
        }

        /// <summary>
        /// Gets if the language enforces that explicit interface implementations are uncallable except through
        /// the interface itself.
        /// If this property is false, code generators may assume that multiple explicit interface implementations
        /// with conflicting return types are invalid unless they are renamed.
        /// </summary>
        public virtual bool ExplicitInterfaceImplementationIsPrivateScope
        {
            get { return false; }
        }

        /// <summary>
        /// Gets if events explicitly implementing an interface require add {} remove {} regions.
        /// </summary>
        public virtual bool RequiresAddRemoveRegionInExplicitInterfaceImplementation
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the start token of an indexer expression in the language. Usually '[' or '('.
        /// </summary>
        public virtual string IndexerExpressionStartToken
        {
            get { return "["; }
        }

        public virtual TextFinder GetFindClassReferencesTextFinder(IClass c)
        {
            // when finding attribute references, also look for the short form of the name
            if (c.Name.Length > 9 && nameComparer.Equals(c.Name.Substring(c.Name.Length - 9), "Attribute"))
            {
                return new CombinedTextFinder(
                    new WholeWordTextFinder(c.Name.Substring(0, c.Name.Length - 9), nameComparer),
                    new WholeWordTextFinder(c.Name, nameComparer)
                );
            }
            return new WholeWordTextFinder(c.Name, nameComparer);
        }

        public virtual TextFinder GetFindMemberReferencesTextFinder(IMember member)
        {
            IProperty property = member as IProperty;
            if (property != null && property.IsIndexer)
            {
                return new IndexBeforeTextFinder(IndexerExpressionStartToken);
            }
            else
            {
                return new WholeWordTextFinder(member.Name, nameComparer);
            }
        }

        public virtual bool IsClassWithImplicitlyStaticMembers(IClass c)
        {
            return false;
        }
        #endregion

        #region Code-completion filters
        public virtual bool ShowInNamespaceCompletion(IClass c)
        {
            return true;
        }

        public virtual bool ShowMember(IMember member, bool showStatic)
        {
            IProperty property = member as IProperty;
            if (property != null && property.IsIndexer)
            {
                return false;
            }
            IMethod method = member as IMethod;
            if (method != null && (method.IsConstructor || method.IsOperator))
            {
                return false;
            }
            return member.IsStatic == showStatic;
        }

        public virtual bool ShowMemberInOverrideCompletion(IMember member)
        {
            return true;
        }
        #endregion

       

        public override string ToString()
        {
            return "[" + base.ToString() + "]";
        }

        public static LanguageProperties GetLanguage(string language)
        {
            switch (language)
            {
                case "VBNet":
                case "VB":
                    return LanguageProperties.VBNet;
                default:
                    return LanguageProperties.CSharp;
            }
        }

    
        #region Text Finder
        protected sealed class WholeWordTextFinder : TextFinder
        {
            readonly string searchedText;
            readonly bool caseInsensitive;

            public WholeWordTextFinder(string word, StringComparer nameComparer)
            {
                if (word == null) word = string.Empty;

                caseInsensitive = nameComparer.Equals("a", "A");
                if (caseInsensitive)
                    this.searchedText = word.ToLowerInvariant();
                else
                    this.searchedText = word;
            }

            public override string PrepareInputText(string inputText)
            {
                if (caseInsensitive)
                    return inputText.ToLowerInvariant();
                else
                    return inputText;
            }

            public override TextFinderMatch Find(string inputText, int startPosition)
            {
                if (searchedText.Length == 0)
                    return TextFinderMatch.Empty;
                int pos = startPosition - 1;
                while ((pos = inputText.IndexOf(searchedText, pos + 1)) >= 0)
                {
                    if (pos > 0 && char.IsLetterOrDigit(inputText, pos - 1))
                    {
                        continue; // memberName is not a whole word (a.SomeName cannot reference Name)
                    }
                    if (pos < inputText.Length - searchedText.Length - 1
                        && char.IsLetterOrDigit(inputText, pos + searchedText.Length))
                    {
                        continue; // memberName is not a whole word (a.Name2 cannot reference Name)
                    }
                    return new TextFinderMatch(pos, searchedText.Length);
                }
                return TextFinderMatch.Empty;
            }
        }

        protected sealed class CombinedTextFinder : TextFinder
        {
            readonly TextFinder[] finders;

            public CombinedTextFinder(params TextFinder[] finders)
            {
                if (finders == null)
                    throw new ArgumentNullException("finders");
                if (finders.Length == 0)
                    throw new ArgumentException("finders.Length must be > 0");
                this.finders = finders;
            }

            public override string PrepareInputText(string inputText)
            {
                return finders[0].PrepareInputText(inputText);
            }

            public override TextFinderMatch Find(string inputText, int startPosition)
            {
                TextFinderMatch best = TextFinderMatch.Empty;
                foreach (TextFinder f in finders)
                {
                    TextFinderMatch r = f.Find(inputText, startPosition);
                    if (r.Position >= 0 && (best.Position < 0 || r.Position < best.Position))
                    {
                        best = r;
                    }
                }
                return best;
            }
        }

        protected sealed class IndexBeforeTextFinder : TextFinder
        {
            readonly string searchText;

            public IndexBeforeTextFinder(string searchText)
            {
                this.searchText = searchText;
            }

            public override TextFinderMatch Find(string inputText, int startPosition)
            {
                int pos = inputText.IndexOf(searchText, startPosition);
                if (pos > 0)
                {
                    return new TextFinderMatch(pos, searchText.Length, pos - 1);
                }
                return TextFinderMatch.Empty;
            }
        }
        #endregion
    }
    public abstract class TextFinder
    {
        public virtual string PrepareInputText(string inputText)
        {
            return inputText;
        }

        public abstract TextFinderMatch Find(string inputText, int startPosition);
    }
    public struct TextFinderMatch
    {
        public readonly int Position;
        public readonly int Length;

        public readonly int ResolvePosition;

        public static readonly TextFinderMatch Empty = new TextFinderMatch(-1, 0);

        public TextFinderMatch(int position, int length)
        {
            this.Position = position;
            this.Length = length;
            this.ResolvePosition = position;
        }

        public TextFinderMatch(int position, int length, int resolvePosition)
        {
            this.Position = position;
            this.Length = length;
            this.ResolvePosition = resolvePosition;
        }
    }
 
    /// <summary>
    /// A return type that modifies the base return type and is not regarded equal to its base type.
    /// </summary>
    public abstract class DecoratingReturnType : ProxyReturnType
    {
        
    }
    /// <summary>
    /// Base class for return types that wrap around other return types.
    /// </summary>
    public abstract class ProxyReturnType 
    {
        public abstract IReturnType BaseType
        {
            get;
        }

       

       

        protected int GetObjectHashCode()
        {
            return base.GetHashCode();
        }

       

      

    }
    /// <summary>
    /// ConstructedReturnType is a reference to generic class that specifies the type parameters.
    /// When getting the Members, this return type modifies the lists in such a way that the
    /// collection.
    /// Example: List&lt;string&gt;
    /// </summary>
    public sealed class ConstructedReturnType : DecoratingReturnType
    {
        // Return types that should be substituted for the generic types
        // If a substitution is unknown (type could not be resolved), the list
        // contains a null entry.
        IList<IReturnType> typeArguments;
        IReturnType baseType;

        public IList<IReturnType> TypeArguments
        {
            get
            {
                return typeArguments;
            }
        }

        public ConstructedReturnType(IReturnType baseType, IList<IReturnType> typeArguments)
        {
            if (baseType == null)
                throw new ArgumentNullException("baseType");
            if (typeArguments == null)
                throw new ArgumentNullException("typeArguments");
            this.typeArguments = typeArguments;
            this.baseType = baseType;
        }

     

        public override int GetHashCode()
        {
            return this.DotNetName.GetHashCode();
        }

       

        public override IReturnType BaseType
        {
            get
            {
                return baseType;
            }
        }

        public IReturnType UnboundType
        {
            get
            {
                return baseType;
            }
        }

      
      

        IReturnType TranslateType(IReturnType input)
        {
            return TranslateType(input, typeArguments, false);
        }

        public override List<IMethod> GetMethods()
        {
            List<IMethod> l = baseType.GetMethods();
            for (int i = 0; i < l.Count; ++i)
            {
                if (CheckReturnType(l[i].ReturnType) || CheckParameters(l[i].Parameters))
                {
                    l[i] = (IMethod)l[i].CreateSpecializedMember();
                    if (l[i].DeclaringType == baseType.GetUnderlyingClass())
                    {
                        l[i].DeclaringTypeReference = this;
                    }
                    l[i].ReturnType = TranslateType(l[i].ReturnType);
                    for (int j = 0; j < l[i].Parameters.Count; ++j)
                    {
                        l[i].Parameters[j].ReturnType = TranslateType(l[i].Parameters[j].ReturnType);
                    }
                }
            }
            return l;
        }

        public override List<IProperty> GetProperties()
        {
            List<IProperty> l = baseType.GetProperties();
            for (int i = 0; i < l.Count; ++i)
            {
                if (CheckReturnType(l[i].ReturnType) || CheckParameters(l[i].Parameters))
                {
                    l[i] = (IProperty)l[i].CreateSpecializedMember();
                    if (l[i].DeclaringType == baseType.GetUnderlyingClass())
                    {
                        l[i].DeclaringTypeReference = this;
                    }
                    l[i].ReturnType = TranslateType(l[i].ReturnType);
                    for (int j = 0; j < l[i].Parameters.Count; ++j)
                    {
                        l[i].Parameters[j].ReturnType = TranslateType(l[i].Parameters[j].ReturnType);
                    }
                }
            }
            return l;
        }

        public override List<IField> GetFields()
        {
            List<IField> l = baseType.GetFields();
            for (int i = 0; i < l.Count; ++i)
            {
                if (CheckReturnType(l[i].ReturnType))
                {
                    l[i] = (IField)l[i].CreateSpecializedMember();
                    if (l[i].DeclaringType == baseType.GetUnderlyingClass())
                    {
                        l[i].DeclaringTypeReference = this;
                    }
                    l[i].ReturnType = TranslateType(l[i].ReturnType);
                }
            }
            return l;
        }

        public override List<IEvent> GetEvents()
        {
            List<IEvent> l = baseType.GetEvents();
            for (int i = 0; i < l.Count; ++i)
            {
                if (CheckReturnType(l[i].ReturnType))
                {
                    l[i] = (IEvent)l[i].CreateSpecializedMember();
                    if (l[i].DeclaringType == baseType.GetUnderlyingClass())
                    {
                        l[i].DeclaringTypeReference = this;
                    }
                    l[i].ReturnType = TranslateType(l[i].ReturnType);
                }
            }
            return l;
        }

        public override string ToString()
        {
            string r = "[ConstructedReturnType: ";
            r += baseType;
            r += "<";
            for (int i = 0; i < typeArguments.Count; i++)
            {
                if (i > 0) r += ",";
                if (typeArguments[i] != null)
                {
                    r += typeArguments[i];
                }
            }
            return r + ">]";
        }
    }
    public interface IEvent : IMember
    {
        IMethod AddMethod
        {
            get;
        }

        IMethod RemoveMethod
        {
            get;
        }

        IMethod RaiseMethod
        {
            get;
        }
    }
    public interface IField : IMember
    {
        /// <summary>Gets if this field is a local variable that has been converted into a field.</summary>
        bool IsLocalVariable { get; }

        /// <summary>Gets if this field is a parameter that has been converted into a field.</summary>
        bool IsParameter { get; }
    }

}
