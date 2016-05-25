using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Common.Core.Interfaces
{
	public interface IPushNotifications
	{
		void SubscribePushNotifications(bool silentMode = false);
		void UnSubscribePushNotifications();
	}

	public class IncomingPushNotificationEventArgs : EventArgs
	{
		public Dictionary<string, object> Payload { get; set; }
	}

	public class NotificationPayload
	{
		[JsonProperty("Action")]
		public string Action { get; set; }

		[JsonIgnore]
		public NotificationInteractionType NotificationInteractionType { get; set; }

		[JsonProperty("Payload")]
		public Dictionary<string, object> Payload { get; set; }

		public NotificationPayload()
		{
			Payload = new Dictionary<string, object>();
			NotificationInteractionType = NotificationInteractionType.NotificationClicked;
		}
	}

	public enum NotificationInteractionType
	{
		NotificationClicked,
		NotificationInApp
	}
}