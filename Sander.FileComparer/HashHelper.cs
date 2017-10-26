using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace Sander.FileComparer
{
	/// <summary>
	/// Helper methods for hash calculation.
	/// <para>These are made public for convinience and re-use.</para>
	/// </summary>
	public static class HashHelper
	{
		/// <summary>
		/// Calculate hash for specified file using specified <see cref="HashAlgorithm"/>
		/// </summary>
		/// <typeparam name="T">Type inheriting from System.Security.Cryptography.HashAlgorithm (see https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.hashalgorithm )</typeparam>
		public static byte[] CalculateHash<T>(string fileName) where T : HashAlgorithm
		{
			using (var hash = CreateHashAlgorithm<T>())
			{
				using (var stream = new FileStream(fileName, FileMode.Open))
					return hash.ComputeHash(stream);
			}
		}


		/// <summary>
		/// Calculate hash for specified file using specified <see cref="HashAlgorithm"/>
		/// </summary>
		/// <typeparam name="T">Type inheriting from System.Security.Cryptography.HashAlgorithm (see https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.hashalgorithm )</typeparam>
		public static byte[] CalculateHash<T>(FileInfo file) where T : HashAlgorithm
		{
			return CalculateHash<T>(file.FullName);
		}


		/// <summary>
		/// Calculate hash for specified stream using specified <see cref="HashAlgorithm"/>
		/// </summary>
		/// <typeparam name="T">Type inheriting from System.Security.Cryptography.HashAlgorithm (see https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.hashalgorithm )</typeparam>
		public static byte[] CalculateHash<T>(Stream fileContent) where T : HashAlgorithm
		{
			using (var hash = CreateHashAlgorithm<T>())
			{
				return hash.ComputeHash(fileContent);
			}
		}


		/// <summary>
		/// Calculate hash for specified byte array using specified <see cref="HashAlgorithm"/>
		/// </summary>
		/// <typeparam name="T">Type inheriting from System.Security.Cryptography.HashAlgorithm (see https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.hashalgorithm )</typeparam>
		public static byte[] CalculateHash<T>(byte[] fileContent) where T : HashAlgorithm
		{
			using (var hash = CreateHashAlgorithm<T>())
			{
				return hash.ComputeHash(fileContent);
			}
		}

		/// <summary>
		/// Helper method to convert MD5 hash to GUID
		/// </summary>
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
