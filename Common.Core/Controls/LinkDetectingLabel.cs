using Xamarin.Forms;

namespace Common.Core.Controls
{
	public class LinkDetectingLabel : Label
	{
		public static readonly BindableProperty DataLinkTypeProperty = BindableProperty.Create(
			propertyName: "DataLinkType",	 
			returnType: typeof(LinkType),
			declaringType: typeof(LinkDetectingLabel),
			defaultValue: LinkType.None);

		public LinkType DataLinkType
		{
			get { return (LinkType)GetValue(DataLinkTypeProperty); }
			set { SetValue(DataLinkTypeProperty, value); }
		}

		public enum LinkType
		{
			All,
			Email,
			Map,
			None,
			Phone,
			Url
		}
	}
}
