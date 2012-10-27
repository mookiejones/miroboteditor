using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
namespace DMC_Robot_Editor.Globals.Logging
{
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), DebuggerNonUserCode, CompilerGenerated]
    internal class Resources
    {
        private static ResourceManager resourceMan;
        private static CultureInfo resourceCulture;
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(Resources.resourceMan, null))
                {
                    ResourceManager resourceManager = new ResourceManager("KukaRoboter.CoreUtil.Logging.Properties.Resources", typeof(Resources).Assembly);
                    Resources.resourceMan = resourceManager;
                }
                return Resources.resourceMan;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get
            {
                return Resources.resourceCulture;
            }
            set
            {
                Resources.resourceCulture = value;
            }
        }
        internal static string AverageTraceExecutionTimeBaseHelpResource
        {
            get
            {
                return Resources.ResourceManager.GetString("AverageTraceExecutionTimeBaseHelpResource", Resources.resourceCulture);
            }
        }
        internal static string AverageTraceExecutionTimeHelpResource
        {
            get
            {
                return Resources.ResourceManager.GetString("AverageTraceExecutionTimeHelpResource", Resources.resourceCulture);
            }
        }
        internal static string BlockName
        {
            get
            {
                return Resources.ResourceManager.GetString("BlockName", Resources.resourceCulture);
            }
        }
        internal static string ComPlusInfo_ActivityId
        {
            get
            {
                return Resources.ResourceManager.GetString("ComPlusInfo_ActivityId", Resources.resourceCulture);
            }
        }
        internal static string ComPlusInfo_ApplicationId
        {
            get
            {
                return Resources.ResourceManager.GetString("ComPlusInfo_ApplicationId", Resources.resourceCulture);
            }
        }
        internal static string ComPlusInfo_DirectCallerAccountName
        {
            get
            {
                return Resources.ResourceManager.GetString("ComPlusInfo_DirectCallerAccountName", Resources.resourceCulture);
            }
        }
        internal static string ComPlusInfo_OriginalCallerAccountName
        {
            get
            {
                return Resources.ResourceManager.GetString("ComPlusInfo_OriginalCallerAccountName", Resources.resourceCulture);
            }
        }
        internal static string ComPlusInfo_TransactionID
        {
            get
            {
                return Resources.ResourceManager.GetString("ComPlusInfo_TransactionID", Resources.resourceCulture);
            }
        }
        internal static string ConfigurationFailureLogging
        {
            get
            {
                return Resources.ResourceManager.GetString("ConfigurationFailureLogging", Resources.resourceCulture);
            }
        }
        internal static string ConfigurationFailureUpdating
        {
            get
            {
                return Resources.ResourceManager.GetString("ConfigurationFailureUpdating", Resources.resourceCulture);
            }
        }
        internal static string ConfigurationSectionNotFound
        {
            get
            {
                return Resources.ResourceManager.GetString("ConfigurationSectionNotFound", Resources.resourceCulture);
            }
        }
        internal static string CouldNotLookupAccountSid
        {
            get
            {
                return Resources.ResourceManager.GetString("CouldNotLookupAccountSid", Resources.resourceCulture);
            }
        }
        internal static string DebugInfo_SchemaHelperAtString
        {
            get
            {
                return Resources.ResourceManager.GetString("DebugInfo_SchemaHelperAtString", Resources.resourceCulture);
            }
        }
        internal static string DebugInfo_SchemaHelperLine
        {
            get
            {
                return Resources.ResourceManager.GetString("DebugInfo_SchemaHelperLine", Resources.resourceCulture);
            }
        }
        internal static string DebugInfo_SchemaHelperUnknownType
        {
            get
            {
                return Resources.ResourceManager.GetString("DebugInfo_SchemaHelperUnknownType", Resources.resourceCulture);
            }
        }
        internal static string DebugInfo_StackTrace
        {
            get
            {
                return Resources.ResourceManager.GetString("DebugInfo_StackTrace", Resources.resourceCulture);
            }
        }
        internal static string DebugInfo_StackTraceException
        {
            get
            {
                return Resources.ResourceManager.GetString("DebugInfo_StackTraceException", Resources.resourceCulture);
            }
        }
        internal static string DebugInfo_StackTraceSecurityException
        {
            get
            {
                return Resources.ResourceManager.GetString("DebugInfo_StackTraceSecurityException", Resources.resourceCulture);
            }
        }
        internal static string DefaultLogDestinationFailed
        {
            get
            {
                return Resources.ResourceManager.GetString("DefaultLogDestinationFailed", Resources.resourceCulture);
            }
        }
        internal static string DefaultLogDestinationMessage
        {
            get
            {
                return Resources.ResourceManager.GetString("DefaultLogDestinationMessage", Resources.resourceCulture);
            }
        }
        internal static string DefaultLogDestinationSinkName
        {
            get
            {
                return Resources.ResourceManager.GetString("DefaultLogDestinationSinkName", Resources.resourceCulture);
            }
        }
        internal static string DefaultTextFormat
        {
            get
            {
                return Resources.ResourceManager.GetString("DefaultTextFormat", Resources.resourceCulture);
            }
        }
        internal static string DistEventLoggerMessagePrefix
        {
            get
            {
                return Resources.ResourceManager.GetString("DistEventLoggerMessagePrefix", Resources.resourceCulture);
            }
        }
        internal static string DistEventLoggerSummary
        {
            get
            {
                return Resources.ResourceManager.GetString("DistEventLoggerSummary", Resources.resourceCulture);
            }
        }
        internal static string DistributorEventLoggerDefaultApplicationName
        {
            get
            {
                return Resources.ResourceManager.GetString("DistributorEventLoggerDefaultApplicationName", Resources.resourceCulture);
            }
        }
        internal static string EmailSinkMissingParameters
        {
            get
            {
                return Resources.ResourceManager.GetString("EmailSinkMissingParameters", Resources.resourceCulture);
            }
        }
        internal static string ErrorWritingData
        {
            get
            {
                return Resources.ResourceManager.GetString("ErrorWritingData", Resources.resourceCulture);
            }
        }
        internal static string EventLogAccessDenied
        {
            get
            {
                return Resources.ResourceManager.GetString("EventLogAccessDenied", Resources.resourceCulture);
            }
        }
        internal static string EventLogEntryExceptionTemplate
        {
            get
            {
                return Resources.ResourceManager.GetString("EventLogEntryExceptionTemplate", Resources.resourceCulture);
            }
        }
        internal static string EventLogEntryHeaderTemplate
        {
            get
            {
                return Resources.ResourceManager.GetString("EventLogEntryHeaderTemplate", Resources.resourceCulture);
            }
        }
        internal static string EventLogSinkMissingEventSource
        {
            get
            {
                return Resources.ResourceManager.GetString("EventLogSinkMissingEventSource", Resources.resourceCulture);
            }
        }
        internal static string ExceptionArgumentShouldDeriveFromIDictionary
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionArgumentShouldDeriveFromIDictionary", Resources.resourceCulture);
            }
        }
        internal static string ExceptionArgumentShouldDeriveFromIList
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionArgumentShouldDeriveFromIList", Resources.resourceCulture);
            }
        }
        internal static string ExceptionAssemblerAttributeNotSet
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionAssemblerAttributeNotSet", Resources.resourceCulture);
            }
        }
        internal static string ExceptionAssemblerTypeNotCompatible
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionAssemblerTypeNotCompatible", Resources.resourceCulture);
            }
        }
        internal static string ExceptionBaseConfigurationSourceElementIsInvalid
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionBaseConfigurationSourceElementIsInvalid", Resources.resourceCulture);
            }
        }
        internal static string ExceptionCanNotConvertType
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionCanNotConvertType", Resources.resourceCulture);
            }
        }
        internal static string ExceptionCannotLoadDefaultCategory
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionCannotLoadDefaultCategory", Resources.resourceCulture);
            }
        }
        internal static string ExceptionCannotLoadDefaultFormatter
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionCannotLoadDefaultFormatter", Resources.resourceCulture);
            }
        }
        internal static string ExceptionCategoryFilterDataName
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionCategoryFilterDataName", Resources.resourceCulture);
            }
        }
        internal static string ExceptionCategoryNotDefined
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionCategoryNotDefined", Resources.resourceCulture);
            }
        }
        internal static string ExceptionConfigurationFileNotFound
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionConfigurationFileNotFound", Resources.resourceCulture);
            }
        }
        internal static string ExceptionConfigurationLoadFileNotFound
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionConfigurationLoadFileNotFound", Resources.resourceCulture);
            }
        }
        internal static string ExceptionConfigurationObjectIsNotCustomProviderData
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionConfigurationObjectIsNotCustomProviderData", Resources.resourceCulture);
            }
        }
        internal static string ExceptionConfigurationObjectIsNotCustomTraceListenerData
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionConfigurationObjectIsNotCustomTraceListenerData", Resources.resourceCulture);
            }
        }
        internal static string ExceptionConfigurationObjectWithTypeDoesNotHaveTypeSet
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionConfigurationObjectWithTypeDoesNotHaveTypeSet", Resources.resourceCulture);
            }
        }
        internal static string ExceptionConfigurationSourceSectionNotFound
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionConfigurationSourceSectionNotFound", Resources.resourceCulture);
            }
        }
        internal static string ExceptionCustomFactoryAttributeNotFound
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionCustomFactoryAttributeNotFound", Resources.resourceCulture);
            }
        }
        internal static string ExceptionCustomListenerTypeDoesNotHaveDefaultConstructor
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionCustomListenerTypeDoesNotHaveDefaultConstructor", Resources.resourceCulture);
            }
        }
        internal static string ExceptionCustomListenerTypeDoesNotHaveRequiredConstructorSignature
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionCustomListenerTypeDoesNotHaveRequiredConstructorSignature", Resources.resourceCulture);
            }
        }
        internal static string ExceptionCustomTraceListenerTypeDoesNotHaveRequiredConstructor
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionCustomTraceListenerTypeDoesNotHaveRequiredConstructor", Resources.resourceCulture);
            }
        }
        internal static string ExceptionDetails
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionDetails", Resources.resourceCulture);
            }
        }
        internal static string ExceptionDuringFormattingOriginalEntryForReporting
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionDuringFormattingOriginalEntryForReporting", Resources.resourceCulture);
            }
        }
        internal static string ExceptionEventRaisingFailed
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionEventRaisingFailed", Resources.resourceCulture);
            }
        }
        internal static string ExceptionFactoryMethodHasInvalidReturnType
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionFactoryMethodHasInvalidReturnType", Resources.resourceCulture);
            }
        }
        internal static string ExceptionFailedToAcquireLockToUpdate
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionFailedToAcquireLockToUpdate", Resources.resourceCulture);
            }
        }
        internal static string ExceptionFailedToAcquireLockToWriteLog
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionFailedToAcquireLockToWriteLog", Resources.resourceCulture);
            }
        }
        internal static string ExceptionFormatterHeader
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionFormatterHeader", Resources.resourceCulture);
            }
        }
        internal static string ExceptionFormatterNotDefined
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionFormatterNotDefined", Resources.resourceCulture);
            }
        }
        internal static string ExceptionIncompatibleConfigurationType
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionIncompatibleConfigurationType", Resources.resourceCulture);
            }
        }
        internal static string ExceptionInvalidType
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionInvalidType", Resources.resourceCulture);
            }
        }
        internal static string ExceptionLoggingSectionNotFound
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionLoggingSectionNotFound", Resources.resourceCulture);
            }
        }
        internal static string ExceptionNamedConfigurationNotFound
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionNamedConfigurationNotFound", Resources.resourceCulture);
            }
        }
        internal static string ExceptionNoConfigurationElementAttribute
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionNoConfigurationElementAttribute", Resources.resourceCulture);
            }
        }
        internal static string ExceptionNoMethodAnnotatedForInjectionFound
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionNoMethodAnnotatedForInjectionFound", Resources.resourceCulture);
            }
        }
        internal static string ExceptionNoSinkDefined
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionNoSinkDefined", Resources.resourceCulture);
            }
        }
        internal static string ExceptionNoSinksDefined
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionNoSinksDefined", Resources.resourceCulture);
            }
        }
        internal static string ExceptionNoSuitableFactoryMethodFound
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionNoSuitableFactoryMethodFound", Resources.resourceCulture);
            }
        }
        internal static string ExceptionNoTypeAttribute
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionNoTypeAttribute", Resources.resourceCulture);
            }
        }
        internal static string ExceptionNullOrEmptyString
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionNullOrEmptyString", Resources.resourceCulture);
            }
        }
        internal static string ExceptionParameterNotAnnotatedForInjection
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionParameterNotAnnotatedForInjection", Resources.resourceCulture);
            }
        }
        internal static string ExceptionPerformanceCounterRedefined
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionPerformanceCounterRedefined", Resources.resourceCulture);
            }
        }
        internal static string ExceptionPropertyNotFound
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionPropertyNotFound", Resources.resourceCulture);
            }
        }
        internal static string ExceptionRetrievalAttributeNotFound
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionRetrievalAttributeNotFound", Resources.resourceCulture);
            }
        }
        internal static string ExceptionSourcePropertyDoesNotExist
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionSourcePropertyDoesNotExist", Resources.resourceCulture);
            }
        }
        internal static string ExceptionStackTraceDetails
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionStackTraceDetails", Resources.resourceCulture);
            }
        }
        internal static string ExceptionStringNullOrEmpty
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionStringNullOrEmpty", Resources.resourceCulture);
            }
        }
        internal static string ExceptionSummary
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionSummary", Resources.resourceCulture);
            }
        }
        internal static string ExceptionSystemSourceNotDefined
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionSystemSourceNotDefined", Resources.resourceCulture);
            }
        }
        internal static string ExceptionTraceListenerConfigurationElementMissingTypeAttribute
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionTraceListenerConfigurationElementMissingTypeAttribute", Resources.resourceCulture);
            }
        }
        internal static string ExceptionTraceListenerConfigurationElementTypeNotFound
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionTraceListenerConfigurationElementTypeNotFound", Resources.resourceCulture);
            }
        }
        internal static string ExceptionTraceListenerConfigurationNotFound
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionTraceListenerConfigurationNotFound", Resources.resourceCulture);
            }
        }
        internal static string ExceptionType
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionType", Resources.resourceCulture);
            }
        }
        internal static string ExceptionTypeCouldNotBeCreated
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionTypeCouldNotBeCreated", Resources.resourceCulture);
            }
        }
        internal static string ExceptionTypeNotCustomFactory
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionTypeNotCustomFactory", Resources.resourceCulture);
            }
        }
        internal static string ExceptionTypeNotNameMapper
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionTypeNotNameMapper", Resources.resourceCulture);
            }
        }
        internal static string ExceptionTypeNotRetriever
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionTypeNotRetriever", Resources.resourceCulture);
            }
        }
        internal static string ExceptionUnexpectedType
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionUnexpectedType", Resources.resourceCulture);
            }
        }
        internal static string ExceptionWriterShouldNotBeNull
        {
            get
            {
                return Resources.ResourceManager.GetString("ExceptionWriterShouldNotBeNull", Resources.resourceCulture);
            }
        }
        internal static string ExtendedPropertyError
        {
            get
            {
                return Resources.ResourceManager.GetString("ExtendedPropertyError", Resources.resourceCulture);
            }
        }
        internal static string FailureWhileCheckingFilters
        {
            get
            {
                return Resources.ResourceManager.GetString("FailureWhileCheckingFilters", Resources.resourceCulture);
            }
        }
        internal static string FailureWhileReportingMissingCategories
        {
            get
            {
                return Resources.ResourceManager.GetString("FailureWhileReportingMissingCategories", Resources.resourceCulture);
            }
        }
        internal static string FailureWhileTracing
        {
            get
            {
                return Resources.ResourceManager.GetString("FailureWhileTracing", Resources.resourceCulture);
            }
        }
        internal static string FileConfigurationSourceName
        {
            get
            {
                return Resources.ResourceManager.GetString("FileConfigurationSourceName", Resources.resourceCulture);
            }
        }
        internal static string FileSinkMissingConfiguration
        {
            get
            {
                return Resources.ResourceManager.GetString("FileSinkMissingConfiguration", Resources.resourceCulture);
            }
        }
        internal static string FilterEvaluationFailed
        {
            get
            {
                return Resources.ResourceManager.GetString("FilterEvaluationFailed", Resources.resourceCulture);
            }
        }
        internal static string FilterEvaluationFailed2
        {
            get
            {
                return Resources.ResourceManager.GetString("FilterEvaluationFailed2", Resources.resourceCulture);
            }
        }
        internal static string FilterEvaluationFailed3
        {
            get
            {
                return Resources.ResourceManager.GetString("FilterEvaluationFailed3", Resources.resourceCulture);
            }
        }
        internal static string FormatterFactoryName
        {
            get
            {
                return Resources.ResourceManager.GetString("FormatterFactoryName", Resources.resourceCulture);
            }
        }
        internal static string InstanceNameNotFoundForSpecifiedType
        {
            get
            {
                return Resources.ResourceManager.GetString("InstanceNameNotFoundForSpecifiedType", Resources.resourceCulture);
            }
        }
        internal static string InstrumentationCounterCategory
        {
            get
            {
                return Resources.ResourceManager.GetString("InstrumentationCounterCategory", Resources.resourceCulture);
            }
        }
        internal static string InstrumentationCounterCategoryHelp
        {
            get
            {
                return Resources.ResourceManager.GetString("InstrumentationCounterCategoryHelp", Resources.resourceCulture);
            }
        }
        internal static string InstrumentationEventSource
        {
            get
            {
                return Resources.ResourceManager.GetString("InstrumentationEventSource", Resources.resourceCulture);
            }
        }
        internal static string IntrinsicPropertyError
        {
            get
            {
                return Resources.ResourceManager.GetString("IntrinsicPropertyError", Resources.resourceCulture);
            }
        }
        internal static string InvalidSink
        {
            get
            {
                return Resources.ResourceManager.GetString("InvalidSink", Resources.resourceCulture);
            }
        }
        internal static string InvalidSinkMessage
        {
            get
            {
                return Resources.ResourceManager.GetString("InvalidSinkMessage", Resources.resourceCulture);
            }
        }
        internal static string LogEntryIntrinsicPropertyNoUnmanagedCodePermissionError
        {
            get
            {
                return Resources.ResourceManager.GetString("LogEntryIntrinsicPropertyNoUnmanagedCodePermissionError", Resources.resourceCulture);
            }
        }
        internal static string LoggingEventRaisedHelpResource
        {
            get
            {
                return Resources.ResourceManager.GetString("LoggingEventRaisedHelpResource", Resources.resourceCulture);
            }
        }
        internal static string ManagedSecurity_AuthenticationType
        {
            get
            {
                return Resources.ResourceManager.GetString("ManagedSecurity_AuthenticationType", Resources.resourceCulture);
            }
        }
        internal static string ManagedSecurity_IdentityName
        {
            get
            {
                return Resources.ResourceManager.GetString("ManagedSecurity_IdentityName", Resources.resourceCulture);
            }
        }
        internal static string ManagedSecurity_IsAuthenticated
        {
            get
            {
                return Resources.ResourceManager.GetString("ManagedSecurity_IsAuthenticated", Resources.resourceCulture);
            }
        }
        internal static string MethodNotImplemented
        {
            get
            {
                return Resources.ResourceManager.GetString("MethodNotImplemented", Resources.resourceCulture);
            }
        }
        internal static string MissingCategories
        {
            get
            {
                return Resources.ResourceManager.GetString("MissingCategories", Resources.resourceCulture);
            }
        }
        internal static string MissingDefaultFormatter
        {
            get
            {
                return Resources.ResourceManager.GetString("MissingDefaultFormatter", Resources.resourceCulture);
            }
        }
        internal static string NumLogsDefaultSinkSec
        {
            get
            {
                return Resources.ResourceManager.GetString("NumLogsDefaultSinkSec", Resources.resourceCulture);
            }
        }
        internal static string NumLogsDefaultSinkSecMsg
        {
            get
            {
                return Resources.ResourceManager.GetString("NumLogsDefaultSinkSecMsg", Resources.resourceCulture);
            }
        }
        internal static string NumLogsDistributedSec
        {
            get
            {
                return Resources.ResourceManager.GetString("NumLogsDistributedSec", Resources.resourceCulture);
            }
        }
        internal static string NumLogsDistributedSecMsg
        {
            get
            {
                return Resources.ResourceManager.GetString("NumLogsDistributedSecMsg", Resources.resourceCulture);
            }
        }
        internal static string NumLogsWrittenSec
        {
            get
            {
                return Resources.ResourceManager.GetString("NumLogsWrittenSec", Resources.resourceCulture);
            }
        }
        internal static string NumLogsWrittenSecMsg
        {
            get
            {
                return Resources.ResourceManager.GetString("NumLogsWrittenSecMsg", Resources.resourceCulture);
            }
        }
        internal static string ProcessMessageFailed
        {
            get
            {
                return Resources.ResourceManager.GetString("ProcessMessageFailed", Resources.resourceCulture);
            }
        }
        internal static string ProcessMessageFailed2
        {
            get
            {
                return Resources.ResourceManager.GetString("ProcessMessageFailed2", Resources.resourceCulture);
            }
        }
        internal static string ProcessMessageFailed3
        {
            get
            {
                return Resources.ResourceManager.GetString("ProcessMessageFailed3", Resources.resourceCulture);
            }
        }
        internal static string PropertyAccessFailed
        {
            get
            {
                return Resources.ResourceManager.GetString("PropertyAccessFailed", Resources.resourceCulture);
            }
        }
        internal static string ReflectedPropertyTokenNotFound
        {
            get
            {
                return Resources.ResourceManager.GetString("ReflectedPropertyTokenNotFound", Resources.resourceCulture);
            }
        }
        internal static string SingleLineTextFormat
        {
            get
            {
                return Resources.ResourceManager.GetString("SingleLineTextFormat", Resources.resourceCulture);
            }
        }
        internal static string SinkFactoryName
        {
            get
            {
                return Resources.ResourceManager.GetString("SinkFactoryName", Resources.resourceCulture);
            }
        }
        internal static string SinkFailure
        {
            get
            {
                return Resources.ResourceManager.GetString("SinkFailure", Resources.resourceCulture);
            }
        }
        internal static string SystemConfigurationSourceName
        {
            get
            {
                return Resources.ResourceManager.GetString("SystemConfigurationSourceName", Resources.resourceCulture);
            }
        }
        internal static string TemplateForFailedLogEntry
        {
            get
            {
                return Resources.ResourceManager.GetString("TemplateForFailedLogEntry", Resources.resourceCulture);
            }
        }
        internal static string TraceListenerEntryWrittenHelpResource
        {
            get
            {
                return Resources.ResourceManager.GetString("TraceListenerEntryWrittenHelpResource", Resources.resourceCulture);
            }
        }
        internal static string TraceOperationStartedHelpResource
        {
            get
            {
                return Resources.ResourceManager.GetString("TraceOperationStartedHelpResource", Resources.resourceCulture);
            }
        }
        internal static string Tracer_EndMessageFormat
        {
            get
            {
                return Resources.ResourceManager.GetString("Tracer_EndMessageFormat", Resources.resourceCulture);
            }
        }
        internal static string Tracer_StartMessageFormat
        {
            get
            {
                return Resources.ResourceManager.GetString("Tracer_StartMessageFormat", Resources.resourceCulture);
            }
        }
        internal static string TraceSourceFailed
        {
            get
            {
                return Resources.ResourceManager.GetString("TraceSourceFailed", Resources.resourceCulture);
            }
        }
        internal static string TraceSourceFailed2
        {
            get
            {
                return Resources.ResourceManager.GetString("TraceSourceFailed2", Resources.resourceCulture);
            }
        }
        internal static string TraceSourceFailed3
        {
            get
            {
                return Resources.ResourceManager.GetString("TraceSourceFailed3", Resources.resourceCulture);
            }
        }
        internal static string UnknownError
        {
            get
            {
                return Resources.ResourceManager.GetString("UnknownError", Resources.resourceCulture);
            }
        }
        internal static string UnknownFailure
        {
            get
            {
                return Resources.ResourceManager.GetString("UnknownFailure", Resources.resourceCulture);
            }
        }
        internal static string UnmanagedSecurity_CurrentUser
        {
            get
            {
                return Resources.ResourceManager.GetString("UnmanagedSecurity_CurrentUser", Resources.resourceCulture);
            }
        }
        internal static string UnmanagedSecurity_ProcessAccountName
        {
            get
            {
                return Resources.ResourceManager.GetString("UnmanagedSecurity_ProcessAccountName", Resources.resourceCulture);
            }
        }
        internal Resources()
        {
        }
    }
}
