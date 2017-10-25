using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sander.FileComparer.Test
{
	[TestClass]
	public class HashTests
	{
		[TestMethod]
		public void GetHash()
		{
			var fileContent = new byte[] { 0x01, 0x02 };
			var hmacHash = HashHelper.CalculateHash<HMACRIPEMD160>(fileContent);
			Assert.AreEqual(20, hmacHash.Length);
			var md5Hash = HashHelper.CalculateHash<MD5>(fileContent);
			Assert.AreEqual(16, md5Hash.Length);
		}

		[TestMethod]
		public void GetString()
		{
			var fileContent = new byte[] { 0x01, 0x02 };
			var md5Hash = HashHelper.CalculateHash<MD5>(fileContent);
			Trace.WriteLine(HashHelper.Md5HashToGuid(md5Hash).ToString("N"));
		}
	}
}
