using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace InlineFormParser.Model
{
	
	[Serializable]
	public sealed class ServiceNotFoundException : AdeException
	{
		private string service = "";

		private string component = "";

		public override string Message => string.Format(CultureInfo.InvariantCulture, "Cannot find the service {0} in the component {1}. {2}", service, component, base.Message);

		public ServiceNotFoundException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public ServiceNotFoundException(string message)
			: base(message)
		{
		}

		public ServiceNotFoundException()
		{
		}

		public ServiceNotFoundException(string message, string service, string component)
			: base(message)
		{
			this.service = service;
			this.component = component;
		}

		public ServiceNotFoundException(string service, string component)
		{
			this.service = service;
			this.component = component;
		}

		private ServiceNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			service = info.GetString("service");
			component = info.GetString("component");
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}
			info.AddValue("service", service);
			info.AddValue("component", component);
			base.GetObjectData(info, context);
		}
	}
}