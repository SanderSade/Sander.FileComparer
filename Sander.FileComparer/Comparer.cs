using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

#pragma warning disable 1573

namespace Sander.FileComparer
{
	/// <summary>
	/// File comparer.
	/// <para>Note that this class does not implement *any* exception handling whatsoever. Caller is responsible for validation that files exist and are readable.</para>
	/// </summary>
	public static class Comparer
	{
		/// <summary>
		/// Binary comparison of two files or streams
		/// <para>This compares two files chunk-by-chunk. It is more memory-efficient than hash comparison, and faster in certain cases (bigger files)</para>
		/// <para>Note that this is very efficient in SSD drive, but should be avoided for regular HDD's (may cause hard drive trashing).</para>
		/// </summary>
		/// <param name="bufferSize">Chunk size for reading and comparing. Default (4096) is common hard drive sector size.</param>
		/// <returns>True, if files are equal, false otherwise</returns>
		public static async Task<bool> BinaryCompare(string firstFile, string secondFile, int bufferSize = 4096)
		{
			return await BinaryCompare(new FileInfo(firstFile), new FileInfo(secondFile), bufferSize);
		}


		/// <summary>
		/// Binary comparison of two files or streams
		/// <para>This compares two files chunk-by-chunk. It is more memory-efficient than hash comparison, and faster in certain cases (bigger files)</para>
		/// <para>Note that this is very efficient in SSD drive, but should be avoided for regular HDD's (may cause hard drive trashing).</para>
		/// </summary>
		/// <param name="bufferSize">Chunk size for reading and comparing. Default (4096) is common hard drive sector size.</param>
		/// <returns>True, if files are equal, false otherwise</returns>
		public static async Task<bool> BinaryCompare(FileInfo firstFile, FileInfo secondFile, int bufferSize = 4096)
		{
			if (firstFile.Length != secondFile.Length)
				return false;

			using (var firstStream = new FileStream(firstFile.FullName, FileMode.Open))
			using (var secondStream = new FileStream(secondFile.FullName, FileMode.Open))
				return await BinaryCompare(firstStream, secondStream, bufferSize);
		}


		/// <summary>
		/// Binary comparison of two files or streams
		/// <para>This compares two files chunk-by-chunk. It is more memory-efficient than hash comparison, and faster in certain cases (bigger files)</para>
		/// <para>Use the stream version if you have the files in memory, but note that the Position of the stream will be changed.</para>
		/// </summary>
		/// <param name="bufferSize">Chunk size for reading and comparing. Default (4096) is common hard drive sector size.</param>
		/// <returns>True, if files are equal, false otherwise</returns>
		public static async Task<bool> BinaryCompare(Stream firstStream, Stream secondStream, int bufferSize = 4096)
		{
			if (firstStream.Length != secondStream.Length)
				return false;
			var position = 0L;

			firstStream.Position = 0;
			secondStream.Position = 0;

			var buffer1 = new byte[bufferSize];
			var buffer2 = new byte[bufferSize];

			while (position < firstStream.Length)
			{
				var readCount = await firstStream.ReadAsync(buffer1, 0, bufferSize);
				if (readCount == 0)
					break;
				position += readCount;

				await secondStream.ReadAsync(buffer2, 0, bufferSize);

				if (!buffer1.SequenceEqual(buffer2))
					return false;
			}

			return true;
		}


		/// <summary>
		/// Compare two files using specified <see cref="HashAlgorithm"/>
		/// </summary>
		/// <typeparam name="T">Type inheriting from System.Security.Cryptography.HashAlgorithm (see https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.hashalgorithm )</typeparam>
		/// <returns>True, if the files are equal, false otherwise.</returns>
		public static bool Compare<T>(FileInfo firstFile, FileInfo secondFile) where T : HashAlgorithm
		{
			if (firstFile.Length != secondFile.Length)
				return false;

			using (var stream = new FileStream(firstFile.FullName, FileMode.Open))
			using (var stream2 = new FileStream(secondFile.FullName, FileMode.Open))
				return Compare<T>(stream, stream2);
		}


		/// <summary>
		/// Compare two files using specified <see cref="HashAlgorithm"/>
		/// </summary>
		/// <typeparam name="T">Type inheriting from System.Security.Cryptography.HashAlgorithm (see https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.hashalgorithm )</typeparam>
		/// <returns>True, if the files are equal, false otherwise.</returns>
		public static bool Compare<T>(string firstFile, string secondFile) where T : HashAlgorithm
		{
			return Compare<T>(new FileInfo(firstFile), new FileInfo(secondFile));
		}


		/// <summary>
		/// Compare two streams using specified <see cref="HashAlgorithm"/>
		/// </summary>
		/// <typeparam name="T">Type inheriting from System.Security.Cryptography.HashAlgorithm (see https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.hashalgorithm )</typeparam>
		/// <returns>True, if the files are equal, false otherwise.</returns>
		public static bool Compare<T>(Stream firstStream, Stream secondStream) where T : HashAlgorithm
		{
			firstStream.Position = 0;
			secondStream.Position = 0;
			using (var hash = HashHelper.CreateHashAlgorithm<T>())
			{
				return hash.ComputeHash(firstStream).SequenceEqual(hash.ComputeHash(secondStream));
			}
		}
	}
}
