using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using System.Security.Cryptography;
using System;
using System.IO;
using System.IO.Hashing;
using System.Linq;
using System.Text;

namespace FileHasher
{
    public static class ControlExtensions
    {
        private static Dictionary<Control, bool> FocusMap = [];

        /// <summary>
        /// Starts tracking a <see cref="Control"/> for its focus state.
        /// </summary>
        public static void StartTrackFocusState(this Control ctrl)
        {
            FocusMap.Add(ctrl, false);

            ctrl.GotFocus += (s, e) =>
            {
                FocusMap[ctrl] = true;
            };

            ctrl.LostFocus += (s, e) =>
            {
                FocusMap[ctrl] = false;
            };
        }
        /// <summary>
        /// Checks if the control is focused if the control is tracked.
        /// </summary>
        /// <remarks>To start tracking a control, use <c>StartTrackFocusState</c>.</remarks>
        /// <returns><see langword="true"/> if the <see cref="Control"/> is focused, <see langword="false"/> if the <see cref="Control"/> isn't focused, or <see langword="null"/> if the <see cref="Control"/> isn't tracked.</returns>
        public static bool? IsFocused(this Control ctrl)
        {
            if (FocusMap.TryGetValue(ctrl, out bool val))
            {
                return val;
            }
            return null;
        }
    }

    public static class UIHelper
    {
        /// <summary>
        /// Gets all the items in a <see cref="NavigationView"/> recursively.
        /// </summary>
        /// <param name="navView">The <see cref="NavigationView"/>.</param>
        /// <returns>The items in the <see cref="NavigationView"/>, including sub-items.</returns>
        public static List<NavigationViewItem> GetNavigationViewItemsRecursive(NavigationView navView)
        {
            var result = new List<NavigationViewItem>();

            foreach (var item in navView.MenuItems)
            {
                if (item is NavigationViewItem navItem)
                {
                    result.Add(navItem);
                    CollectSubItems(navItem, ref result);
                }
            }

            return result;
        }

        private static void CollectSubItems(NavigationViewItem parent, ref List<NavigationViewItem> result)
        {
            foreach (var child in parent.MenuItems)
            {
                if (child is NavigationViewItem childItem)
                {
                    result.Add(childItem);
                    if (childItem.MenuItems?.Count > 0)
                    {
                        CollectSubItems(childItem, ref result);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the parent <see cref="NavigationViewItem"/> of the specified <paramref name="target"/> <see cref="NavigationViewItem"/>.
        /// </summary>
        /// <param name="navView">The <see cref="NavigationView"/> to scan through to find the parent.</param>
        /// <param name="target">The target <see cref="NavigationViewItem"/> to find the parent of.</param>
        /// <returns>The parent <see cref="NavigationViewItem"/>, or <see langword="null"/> if the item is at the root or the parent can't be found.</returns>
        public static NavigationViewItem? GetParentNavigationViewItem(NavigationView navView, NavigationViewItem target)
        {
            foreach (var item in navView.MenuItems)
            {
                if (item is NavigationViewItem parent)
                {
                    var found = FindParentRecursive(parent, target);
                    if (found != null)
                        return found;
                }
            }
            return null;
        }

        private static NavigationViewItem? FindParentRecursive(NavigationViewItem parent, NavigationViewItem target)
        {
            foreach (var child in parent.MenuItems)
            {
                if (child is NavigationViewItem childItem)
                {
                    if (childItem == target)
                        return parent;

                    var result = FindParentRecursive(childItem, target);
                    if (result != null)
                        return result;
                }
            }
            return null;
        }
    }

    /// <summary>
    /// A helper class to handle hashes.
    /// </summary>
    public static class HashHelper
    {
        /// <summary>
        /// Specifies types of hash algorithms supported.
        /// </summary>
        public enum HashAlgorithmType
        {
            Adler32,
            CRC32,
            CRC64,
            ED2K,
            GOST,
            Keccak224,
            Keccak256,
            Keccak384,
            Keccak512,
            MD2,
            MD4,
            MD5,
            RIPEMD128,
            RIPEMD160,
            RIPEMD256,
            RIPEMD320,
            SHA1,
            SHA256,
            SHA256_Base64,
            SHA384,
            SHA512,
            SHA3_224,
            SHA3_256,
            SHA3_384,
            SHA3_512,
            Tiger,
            Whirlpool
        }
        /// <summary>
        /// Computes a hash from the specified <paramref name="text"/> using the specified <paramref name="algorithm"/> and returns it as a string.
        /// </summary>
        /// <param name="algorithm">The algorithm to use when computing the hash.</param>
        /// <param name="text">The text to compute the hash for.</param>
        /// <returns>The computed hash, converted to a string.</returns>
        /// <exception cref="NotSupportedException"/>
        public static string ComputeHash(HashAlgorithmType algorithm, string text)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(text);

            IDigest? digest = algorithm switch
            {
                HashAlgorithmType.SHA1 => new Sha1Digest(),
                HashAlgorithmType.SHA256 => new Sha256Digest(),
                HashAlgorithmType.SHA384 => new Sha384Digest(),
                HashAlgorithmType.SHA512 => new Sha512Digest(),
                HashAlgorithmType.SHA3_224 => new Sha3Digest(224),
                HashAlgorithmType.SHA3_256 => new Sha3Digest(256),
                HashAlgorithmType.SHA3_384 => new Sha3Digest(384),
                HashAlgorithmType.SHA3_512 => new Sha3Digest(512),
                HashAlgorithmType.Keccak224 => new KeccakDigest(224),
                HashAlgorithmType.Keccak256 => new KeccakDigest(256),
                HashAlgorithmType.Keccak384 => new KeccakDigest(384),
                HashAlgorithmType.Keccak512 => new KeccakDigest(512),
                HashAlgorithmType.MD2 => new MD2Digest(),
                HashAlgorithmType.MD4 => new MD4Digest(),
                HashAlgorithmType.MD5 => new MD5Digest(),
                HashAlgorithmType.RIPEMD128 => new RipeMD128Digest(),
                HashAlgorithmType.RIPEMD160 => new RipeMD160Digest(),
                HashAlgorithmType.RIPEMD256 => new RipeMD256Digest(),
                HashAlgorithmType.RIPEMD320 => new RipeMD320Digest(),
                HashAlgorithmType.Whirlpool => new WhirlpoolDigest(),
                HashAlgorithmType.Tiger => new TigerDigest(),
                HashAlgorithmType.GOST => new Gost3411Digest(),
                _ => null
            };

            // Special case: SHA256 Base64 (system crypto)
            if (algorithm == HashAlgorithmType.SHA256_Base64)
            {
                using var sha256 = SHA256.Create();
                var hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }

            if (algorithm == HashAlgorithmType.CRC32)
            {
                return BitConverter.ToString(Crc32.Hash(bytes)).Replace("-", "").ToLowerInvariant();
            }

            if (algorithm == HashAlgorithmType.CRC64)
            {
                return BitConverter.ToString(Crc64.Hash(bytes)).Replace("-", "").ToLowerInvariant();
            }

            if (algorithm == HashAlgorithmType.Adler32)
            {
                using (var stream = new MemoryStream(bytes))
                return ComputeAdler32(stream).ToString("x8");
            }

            if (algorithm == HashAlgorithmType.ED2K)
            {
                using (var stream = new MemoryStream(bytes))
                return ComputeED2K(stream);
            }

            if (digest == null)
                throw new NotSupportedException($"{algorithm} is not supported yet.");

            // Apply digest to byte array
            digest.BlockUpdate(bytes, 0, bytes.Length);
            byte[] result = new byte[digest.GetDigestSize()];
            digest.DoFinal(result, 0);

            return BitConverter.ToString(result).Replace("-", "").ToLowerInvariant();
        }

        private static uint ComputeAdler32(Stream stream)
        {
            const uint MOD_ADLER = 65521;
            uint a = 1, b = 0;

            int read;
            byte[] buffer = new byte[8192];
            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                for (int i = 0; i < read; i++)
                {
                    a = (a + buffer[i]) % MOD_ADLER;
                    b = (b + a) % MOD_ADLER;
                }
            }

            return (b << 16) | a;
        }

        private static ulong ComputeCRC64(Stream stream)
        {
            const ulong POLY = 0xC96C5795D7870F42;
            ulong[] table = new ulong[256];
            for (ulong i = 0; i < 256; i++)
            {
                ulong crc = i;
                for (int j = 0; j < 8; j++)
                    crc = (crc >> 1) ^ ((crc & 1) != 0 ? POLY : 0);
                table[i] = crc;
            }

            ulong hash = ulong.MaxValue;
            int b;
            while ((b = stream.ReadByte()) != -1)
            {
                hash = table[(hash ^ (byte)b) & 0xFF] ^ (hash >> 8);
            }

            return ~hash;
        }

        private static byte[] ComputeDigest(IDigest digest, byte[] buffer, int length)
        {
            digest.BlockUpdate(buffer, 0, length);
            byte[] result = new byte[digest.GetDigestSize()];
            digest.DoFinal(result, 0);
            return result;
        }

        private static string ComputeED2K(Stream stream)
        {
            const int chunkSize = 9728000; // 9500 KiB
            List<byte[]> chunkHashes = new();
            var md4 = new MD4Digest();

            byte[] buffer = new byte[chunkSize];
            int read;

            while ((read = stream.Read(buffer, 0, chunkSize)) > 0)
            {
                var chunkDigest = new MD4Digest(); // New digest per chunk
                var hash = ComputeDigest(chunkDigest, buffer, read);
                chunkHashes.Add(hash);
            }

            if (chunkHashes.Count == 1)
            {
                return BitConverter.ToString(chunkHashes[0]).Replace("-", "").ToLowerInvariant();
            }
            else
            {
                byte[] concatenated = chunkHashes.SelectMany(x => x).ToArray();
                var finalDigest = new MD4Digest();
                var finalHash = ComputeDigest(finalDigest, concatenated, concatenated.Length);
                return BitConverter.ToString(finalHash).Replace("-", "").ToLowerInvariant();
            }
        }

        private static readonly Dictionary<string, HashAlgorithmType> _stringToEnum = new()
        {
            ["Adler32"] = HashAlgorithmType.Adler32,
            ["CRC32"] = HashAlgorithmType.CRC32,
            ["CRC64"] = HashAlgorithmType.CRC64,
            ["ED2K"] = HashAlgorithmType.ED2K,
            ["GOST"] = HashAlgorithmType.GOST,
            ["Keccak-224"] = HashAlgorithmType.Keccak224,
            ["Keccak-256"] = HashAlgorithmType.Keccak256,
            ["Keccak-384"] = HashAlgorithmType.Keccak384,
            ["Keccak-512"] = HashAlgorithmType.Keccak512,
            ["MD2"] = HashAlgorithmType.MD2,
            ["MD4"] = HashAlgorithmType.MD4,
            ["MD5"] = HashAlgorithmType.MD5,
            ["RIPEMD-128"] = HashAlgorithmType.RIPEMD128,
            ["RIPEMD-160"] = HashAlgorithmType.RIPEMD160,
            ["RIPEMD-256"] = HashAlgorithmType.RIPEMD256,
            ["RIPEMD-320"] = HashAlgorithmType.RIPEMD320,
            ["SHA-1"] = HashAlgorithmType.SHA1,
            ["SHA-256"] = HashAlgorithmType.SHA256,
            ["SHA-256 Base64"] = HashAlgorithmType.SHA256_Base64,
            ["SHA-384"] = HashAlgorithmType.SHA384,
            ["SHA-512"] = HashAlgorithmType.SHA512,
            ["SHA3-224"] = HashAlgorithmType.SHA3_224,
            ["SHA3-256"] = HashAlgorithmType.SHA3_256,
            ["SHA3-384"] = HashAlgorithmType.SHA3_384,
            ["SHA3-512"] = HashAlgorithmType.SHA3_512,
            ["Tiger"] = HashAlgorithmType.Tiger,
            ["Whirlpool"] = HashAlgorithmType.Whirlpool,
        };

        private static readonly Dictionary<HashAlgorithmType, string> _enumToString =
    _stringToEnum.ToDictionary(kv => kv.Value, kv => kv.Key);


        //public static HashAlgorithmType? GetHashAlgorithmForString(string name)
        //{
        //    return name switch
        //    {
        //        "Adler32" => HashAlgorithmType.Adler32,
        //        "CRC32" => HashAlgorithmType.CRC32,
        //        "CRC64" => HashAlgorithmType.CRC64,
        //        "ED2K" => HashAlgorithmType.ED2K,
        //        "GOST" => HashAlgorithmType.GOST,
        //        "Keccak-224" => HashAlgorithmType.Keccak224,
        //        "Keccak-256" => HashAlgorithmType.Keccak256,
        //        "Keccak-384" => HashAlgorithmType.Keccak384,
        //        "Keccak-512" => HashAlgorithmType.Keccak512,
        //        "MD2" => HashAlgorithmType.MD2,
        //        "MD4" => HashAlgorithmType.MD4,
        //        "MD5" => HashAlgorithmType.MD5,
        //        "RIPEMD-128" => HashAlgorithmType.RIPEMD128,
        //        "RIPEMD-160" => HashAlgorithmType.RIPEMD160,
        //        "RIPEMD-256" => HashAlgorithmType.RIPEMD160,
        //        "RIPEMD-320" => HashAlgorithmType.RIPEMD320,
        //        "SHA-1" => HashAlgorithmType.SHA1,
        //        "SHA-256" => HashAlgorithmType.SHA256,
        //        "SHA-256 Base64" => HashAlgorithmType.SHA256_Base64,
        //        "SHA-384" => HashAlgorithmType.SHA384,
        //        "SHA-512" => HashAlgorithmType.SHA512,
        //        "SHA3-224" => HashAlgorithmType.SHA3_224,
        //        "SHA3-256" => HashAlgorithmType.SHA3_256,
        //        "SHA3-384" => HashAlgorithmType.SHA3_384,
        //        "SHA3-512" => HashAlgorithmType.SHA3_512,
        //        "Tiger" => HashAlgorithmType.Tiger,
        //        "Whirlpool" => HashAlgorithmType.Whirlpool,
        //        _ => null
        //    };
        //}

        public static HashAlgorithmType? GetHashAlgorithmForString(string name)
    => _stringToEnum.TryGetValue(name, out var val) ? val : null;

        public static string? GetStringForHashAlgorithm(HashAlgorithmType algo)
    => _enumToString.TryGetValue(algo, out var str) ? str : null;

    }
}
