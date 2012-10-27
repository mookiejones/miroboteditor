using System;
using System.Collections.Generic;

namespace DMC_Robot_Editor.Classes
{
    public interface IClass : IEntity
    {
        /// <summary>
        /// Region of the whole class including the body.
        /// </summary>
        DomRegion Region
        {
            get;
        }

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

        /// <summary>
        /// Gets the using scope of contains this class.
        /// </summary>
        IUsingScope UsingScope
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
    public interface IEntity : ICompletionEntry, IFreezable, IComparable
    {
        string FullyQualifiedName
        {
            get;
        }

        string Namespace
        {
            get;
        }

        /// <summary>
        /// The fully qualified name in the internal .NET notation (with `1 for generic types)
        /// </summary>
        string DotNetName
        {
            get;
        }

        DomRegion BodyRegion
        {
            get;
        }

        /// <summary>
        /// Gets the declaring type.
        /// For members, this is the type that contains the member.
        /// For classes, this is the outer class (for nested classes), or null if there this
        /// is a top-level class.
        /// </summary>
        IClass DeclaringType
        {
            get;
        }

        ModifierEnum Modifiers
        {
            get;
        }

        IList<IAttribute> Attributes
        {
            get;
        }

        string Documentation
        {
            get;
        }

        /// <summary>
        /// Returns true if this entity has the 'abstract' modifier set. 
        /// (Returns false for interface members).
        /// </summary>
        bool IsAbstract
        {
            get;
        }

        bool IsSealed
        {
            get;
        }

        /// <summary>
        /// Gets whether this entity is static.
        /// Returns true if either the 'static' or the 'const' modifier is set.
        /// </summary>
        bool IsStatic
        {
            get;
        }

        /// <summary>
        /// Gets whether this entity is a constant (C#-like const).
        /// </summary>
        bool IsConst
        {
            get;
        }

        /// <summary>
        /// Gets if the member is virtual. Is true only if the "virtual" modifier was used, but non-virtual
        /// members can be overridden, too; if they are already overriding a method.
        /// </summary>
        bool IsVirtual
        {
            get;
        }

        bool IsPublic
        {
            get;
        }

        bool IsProtected
        {
            get;
        }

        bool IsPrivate
        {
            get;
        }

        bool IsInternal
        {
            get;
        }

        bool IsReadonly
        {
            get;
        }

        [Obsolete("This property does not do what one would expect - it merely checks if protected+internal are set, it is not the equivalent of AssemblyAndFamily in Reflection!")]
        bool IsProtectedAndInternal
        {
            get;
        }

        [Obsolete("This property does not do what one would expect - it merely checks if one of protected+internal is set, it is not the equivalent of AssemblyOrFamily in Reflection!")]
        bool IsProtectedOrInternal
        {
            get;
        }

        bool IsOverride
        {
            get;
        }
        /// <summary>
        /// Gets if the member can be overridden. Returns true when the member is "virtual" or "override" but not "sealed".
        /// </summary>
        bool IsOverridable
        {
            get;
        }

        bool IsNew
        {
            get;
        }
        bool IsSynthetic
        {
            get;
        }

        /// <summary>
        /// Gets the compilation unit that contains this entity.
        /// </summary>
        ICompilationUnit CompilationUnit
        {
            get;
        }

        /// <summary>
        /// The project content in which this entity is defined.
        /// </summary>
        IProjectContent ProjectContent
        {
            get;
        }

        /// <summary>
        /// This property can be used to attach any user-defined data to this class/method.
        /// This property is mutable, it can be changed when the class/method is frozen.
        /// </summary>
        object UserData
        {
            get;
            set;
        }

        EntityType EntityType
        {
            get;
        }

        bool IsAccessible(IClass callingClass, bool isAccessThoughReferenceOfCurrentClass);
    }

    public enum EntityType
    {
        Class,
        Field,
        Property,
        Method,
        Event
    }
    public interface ICompletionEntry
    {
        string Name
        {
            get;
        }
    }
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
    public struct DomRegion : IEquatable<DomRegion>
    {
        readonly int beginLine;
        readonly int endLine;
        readonly int beginColumn;
        readonly int endColumn;

        public readonly static DomRegion Empty = new DomRegion(-1, -1);

        public bool IsEmpty
        {
            get
            {
                return BeginLine <= 0;
            }
        }

        public int BeginLine
        {
            get
            {
                return beginLine;
            }
        }

        /// <value>
        /// if the end line is == -1 the end column is -1 too
        /// this stands for an unknwon end
        /// </value>
        public int EndLine
        {
            get
            {
                return endLine;
            }
        }

        public int BeginColumn
        {
            get
            {
                return beginColumn;
            }
        }

        /// <value>
        /// if the end column is == -1 the end line is -1 too
        /// this stands for an unknown end
        /// </value>
        public int EndColumn
        {
            get
            {
                return endColumn;
            }
        }

        public static DomRegion FromLocation(Location start, Location end)
        {
            return new DomRegion(start.Y, start.X, end.Y, end.X);
        }

        public DomRegion(int beginLine, int beginColumn, int endLine, int endColumn)
        {
            this.beginLine = beginLine;
            this.beginColumn = beginColumn;
            this.endLine = endLine;
            this.endColumn = endColumn;
        }

        public DomRegion(int beginLine, int beginColumn)
        {
            this.beginLine = beginLine;
            this.beginColumn = beginColumn;
            this.endLine = -1;
            this.endColumn = -1;
        }

        /// <remarks>
        /// Returns true, if the given coordinates (row, column) are in the region.
        /// This method assumes that for an unknown end the end line is == -1
        /// </remarks>
        public bool IsInside(int row, int column)
        {
            if (IsEmpty)
                return false;
            return row >= BeginLine &&
                (row <= EndLine || EndLine == -1) &&
                (row != BeginLine || column >= BeginColumn) &&
                (row != EndLine || column <= EndColumn);
        }

        public override string ToString()
        {
            return String.Format("[Region: BeginLine = {0}, EndLine = {1}, BeginColumn = {2}, EndColumn = {3}]",
                                 beginLine,
                                 endLine,
                                 beginColumn,
                                 endColumn);
        }

        public override bool Equals(object obj)
        {
            return obj is DomRegion && Equals((DomRegion)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return BeginColumn + 1100009 * BeginLine + 1200007 * BeginColumn + 1300021 * EndColumn;
            }
        }

        public bool Equals(DomRegion other)
        {
            return BeginLine == other.BeginLine && BeginColumn == other.BeginColumn
                && EndLine == other.EndLine && EndColumn == other.EndColumn;
        }

        public static bool operator ==(DomRegion lhs, DomRegion rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(DomRegion lhs, DomRegion rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
    public interface ICompilationUnit : IFreezable
    {
        string FileName
        {
            get;
            set;
        }

        bool ErrorsDuringCompile
        {
            get;
            set;
        }

        object Tag
        {
            get;
            set;
        }

        IProjectContent ProjectContent
        {
            get;
        }

        /// <summary>
        /// Gets the language this compilation unit was written in.
        /// </summary>
        LanguageProperties Language
        {
            get;
        }

        /// <summary>
        /// Gets the main using scope of the compilation unit.
        /// That scope usually represents the root namespace.
        /// </summary>
        IUsingScope UsingScope
        {
            get;
        }

        IList<IAttribute> Attributes
        {
            get;
        }

        IList<IClass> Classes
        {
            get;
        }

        IList<IComment> MiscComments
        {
            get;
        }

        IList<IComment> DokuComments
        {
            get;
        }

        IList<TagComment> TagComments
        {
            get;
        }

        IList<FoldingRegion> FoldingRegions
        {
            get;
        }

        /// <summary>
        /// Returns the innermost class in which the carret currently is, returns null
        /// if the carret is outside any class boundaries.
        /// </summary>
        IClass GetInnermostClass(int caretLine, int caretColumn);
    }
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
        string FullyQualifiedName
        {
            get;
        }

        /// <summary>
        /// Gets the short name of the class the return type is pointing to.
        /// </summary>
        /// <returns>
        /// "Int32" or "int" (depending how the return type was created) for int[]<br/>
        /// "List" for List&lt;string&gt;
        /// </returns>
        string Name
        {
            get;
        }

        /// <summary>
        /// Gets the namespace of the class the return type is pointing to.
        /// </summary>
        /// <returns>
        /// "System" for int[]<br/>
        /// "System.Collections.Generic" for List&lt;string&gt;
        /// </returns>
        string Namespace
        {
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
        string DotNetName
        {
            get;
        }

        /// <summary>
        /// Gets the number of type parameters the target class should have
        /// / the number of type arguments specified by this type reference.
        /// </summary>
        int TypeArgumentCount
        {
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

        bool IsArrayReturnType { get; }
        ArrayReturnType CastToArrayReturnType();

        bool IsGenericReturnType { get; }
        GenericReturnType CastToGenericReturnType();

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
}
