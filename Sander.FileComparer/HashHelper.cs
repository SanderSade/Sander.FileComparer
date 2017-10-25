using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace Sander.FileComparer
{
	public static class HashHelper
	{
		public static byte[] CalculateHash<T>(string fileName) where T : HashAlgorithm
		{
			using (var hash = CreateHashAlgorithm<T>())
			{
				using (var stream = new FileStream(fileName, FileMode.Open))
					return hash.ComputeHash(stream);
			}
		}


		public static byte[] CalculateHash<T>(FileInfo file) where T : HashAlgorithm
		{
			return CalculateHash<T>(file.FullName);
		}


		public static byte[] CalculateHash<T>(Stream fileContent) where T : HashAlgorithm
		{
			using (var hash = CreateHashAlgorithm<T>())
			{
				return hash.ComputeHash(fileContent);
			}
		}


		public static byte[] CalculateHash<T>(byte[] fileContent) where T : HashAlgorithm
		{
			using (var hash = CreateHashAlgorithm<T>())
			{
				return hash.ComputeHash(fileContent);
			}
		}


		public static Guid Md5HashToGuid(byte[] hash)
		{
			if (hash == null || hash.Length != 16)
				throw new ArgumentNullException(nameof(hash), "Invalid MD5 hash! Has to be exactly 16 bytes");

			return new Guid(hash);
		}

		internal static T CreateHashAlgorithm<T>() where T : HashAlgorithm
		{
			var type = typeof(T);
			return (type.BaseType == typeof(KeyedHashAlgorithm) || type.BaseType?.BaseType == typeof(KeyedHashAlgorithm)
					? type.InvokeMember(null, BindingFlags.CreateInstance, null, null, null)
					: type.InvokeMember("Create", BindingFlags.InvokeMethod, null, null, null))
				as T;
		}


	}
}
