using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresseroConnector.Models
{
    public class SubscriberAsset
    {
		public string SubscriberId { get; set; }
		public string AssetId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Path { get; set; }
		public bool? UserDownloadable { get; set; }
		public bool? IsLocal { get; set; }
		public string AssetType { get; set; }
		public string AssetUsage { get; set; }
		public string GroupId { get; set; }
		public string ProductId { get; set; }
		public string AssetCategoryId { get; set; }
		public string ThumbnailPath { get; set; }
		public bool? GenerateThumbnailForExternalURL { get; set; }
	}

	public class SubscriberAssetData
	{
		public SubscriberAsset[] Items { get; set; }
	}
}
