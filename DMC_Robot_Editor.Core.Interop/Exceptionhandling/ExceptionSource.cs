using System.Runtime.Serialization;
namespace DMC_Robot_Editor.Globals.Exceptionhandling
{
    [DataContract(Name = "ExceptionSource", Namespace = "DMC.Contracts")]
    public enum ExceptionSource
    {
        Unknown,
        Configuration,
        Environment,
        Functionally,
        FunctionallyPermission,
        Permission,
        DataValidation,
        ParameterValidation,
        Development
    }
}
