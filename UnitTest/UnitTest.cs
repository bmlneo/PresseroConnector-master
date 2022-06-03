using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Caching.Redis;
using System.Threading.Tasks;
using PresseroConnector.Data;
using PresseroConnector.Managers;

namespace UnitTest
{
	[TestClass]
	public class UnitTest
	{
		ILogger logger = new LoggerFactory().CreateLogger<UnitTest>();

		IDistributedCache cache;

		public UnitTest()
		{
			cache = new RedisCache(new RedisCacheOptions()
			{
				Configuration = "192.168.1.207",
				InstanceName = "epcache"
			});
		}

		[TestMethod]
		public async Task VerifyToken()
		{
			var order = await PresseroManager.GetOrder("613");

			Assert.IsTrue(order != null);
		}
		
		[TestMethod]
		public async Task GetOrders()
		{
			var dal = new OrderDAL(logger, cache);
			var orders = await dal.GetAll(true);
			Assert.IsTrue(!string.IsNullOrEmpty(orders));
		}
	}
}
