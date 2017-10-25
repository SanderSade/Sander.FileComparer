using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Sander.FileComparer
{
	public static class Comparer
	{
		public static async Task<bool> Compare(FileInfo firstFile, FileInfo secondFile)
		{
			throw new NotImplementedException();
		}


		public static bool Compare<T>(FileInfo firstFile, FileInfo secondFile) where T : HashAlgorithm
		{
			if (firstFile.Length != secondFile.Length)
				return false;

			using (var hash = HashHelper.CreateHashAlgorithm<T>())
			{
				using (var stream = new FileStream(firstFile.FullName, FileMode.Open))
					using (var stream2 = new FileStream(secondFile.FullName, FileMode.Open))
						return hash.ComputeHash(stream).SequenceEqual(hash.ComputeHash(stream2));
			}
		}


		public static bool Compare<T>(string firstFile, string secondFile) where T : HashAlgorithm
		{
			return Compare<T>(new FileInfo(firstFile), new FileInfo(secondFile));
		}
	}
}
