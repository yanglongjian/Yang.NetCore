using Furion.DistributedIDGenerator;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Yang.Core
{
    /// <summary>
    /// �ֲ�ʽΨһId������������ѩ���㷨(SnowFlake)
    /// </summary>
    public abstract class Id
    {
        #region 
        internal static readonly long _randomLong = CalculateRandomValue();

        #region Random Function
        // private static methods
        private static long CalculateRandomValue()
        {
            var seed = (int)DateTime.UtcNow.Ticks ^ GetMachineHash() ^ GetPid();
            var random = new Random(seed);
            var high = random.Next();
            var low = random.Next();
            var combined = (long)((ulong)(uint)high << 32 | (ulong)(uint)low);
            return combined & 0xffffffffff; // low order 5 bytes
        }

        /// <summary>
        /// Gets the current process id.  This method exists because of how CAS operates on the call stack, checking
        /// for permissions before executing the method.  Hence, if we inlined this call, the calling method would not execute
        /// before throwing an exception requiring the try/catch at an even higher level that we don't necessarily control.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static int GetCurrentProcessId()
        {
            return Environment.ProcessId;
        }

        private static int GetMachineHash()
        {
            // use instead of Dns.HostName so it will work offline
            var machineName = GetMachineName();
            return 0x00ffffff & machineName.GetHashCode(); // use first 3 bytes of hash
        }

        private static string GetMachineName()
        {
            return Environment.MachineName;
        }

        private static short GetPid()
        {
            try
            {
                return (short)GetCurrentProcessId(); // use low order two bytes only
            }
            catch (SecurityException)
            {
                return 0;
            }
        }
        #endregion

        protected int _a;
        protected int _b;
        protected int _c;
        protected short _d;

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            int hash = 17;
            hash = 37 * hash + _a.GetHashCode();
            hash = 37 * hash + _b.GetHashCode();
            hash = 37 * hash + _c.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Converts the UnqiueId to a byte array.
        /// </summary>
        /// <returns>A byte array.</returns>
        public byte[] ToByteArray()
        {
            var bytes = new byte[_d];
            ToByteArray(bytes, 0);
            return bytes;
        }

        /// <summary>
        /// Converts the UnqiueId to a byte array.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="offset">The offset.</param>
        public void ToByteArray(byte[] destination, int offset)
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }
            if (offset + _d > destination.Length)
            {
                throw new ArgumentException("Not enough room in destination buffer.", "offset");
            }

            switch (_d)
            {
                case 9:
                    destination[offset + 0] = (byte)(_a >> 24);
                    destination[offset + 1] = (byte)(_a >> 16);
                    destination[offset + 2] = (byte)(_a >> 8);
                    destination[offset + 3] = (byte)(_a);
                    destination[offset + 4] = (byte)(_b >> 8);
                    destination[offset + 5] = (byte)(_b);
                    destination[offset + 6] = (byte)(_c >> 24);
                    destination[offset + 7] = (byte)(_c >> 8);
                    destination[offset + 8] = (byte)(_c);
                    break;
                case 10:
                    destination[offset + 0] = (byte)(_a >> 24);
                    destination[offset + 1] = (byte)(_a >> 16);
                    destination[offset + 2] = (byte)(_a >> 8);
                    destination[offset + 3] = (byte)(_a);
                    destination[offset + 4] = (byte)(_b >> 16);
                    destination[offset + 5] = (byte)(_b >> 8);
                    destination[offset + 6] = (byte)(_b);
                    destination[offset + 7] = (byte)(_c >> 24);
                    destination[offset + 8] = (byte)(_c >> 8);
                    destination[offset + 9] = (byte)(_c);
                    break;
                case 11:
                    destination[offset + 0] = (byte)(_a >> 24);
                    destination[offset + 1] = (byte)(_a >> 16);
                    destination[offset + 2] = (byte)(_a >> 8);
                    destination[offset + 3] = (byte)(_a);
                    destination[offset + 4] = (byte)(_b >> 24);
                    destination[offset + 5] = (byte)(_b >> 16);
                    destination[offset + 6] = (byte)(_b >> 8);
                    destination[offset + 7] = (byte)(_b);
                    destination[offset + 8] = (byte)(_c >> 24);
                    destination[offset + 9] = (byte)(_c >> 8);
                    destination[offset + 10] = (byte)(_c);
                    break;
                case 12:
                    destination[offset + 0] = (byte)(_a >> 24);
                    destination[offset + 1] = (byte)(_a >> 16);
                    destination[offset + 2] = (byte)(_a >> 8);
                    destination[offset + 3] = (byte)(_a);
                    destination[offset + 4] = (byte)(_b >> 24);
                    destination[offset + 5] = (byte)(_b >> 16);
                    destination[offset + 6] = (byte)(_b >> 8);
                    destination[offset + 7] = (byte)(_b);
                    destination[offset + 8] = (byte)(_c >> 24);
                    destination[offset + 9] = (byte)(_c >> 16);
                    destination[offset + 10] = (byte)(_c >> 8);
                    destination[offset + 11] = (byte)(_c);
                    break;
                default:
                    throw new NotSupportedException(_d.ToString());
            }
        }

        /// <summary>
        /// Returns a string representation of the value.
        /// </summary>
        /// <returns>A string representation of the value.</returns>
        public override string ToString()
        {
            switch (_d)
            {
                case 9:
                    {
                        var c = new char[18];
                        c[0] = BsonUtils.ToHexChar((_a >> 28) & 0x0f);
                        c[1] = BsonUtils.ToHexChar((_a >> 24) & 0x0f);
                        c[2] = BsonUtils.ToHexChar((_a >> 20) & 0x0f);
                        c[3] = BsonUtils.ToHexChar((_a >> 16) & 0x0f);
                        c[4] = BsonUtils.ToHexChar((_a >> 12) & 0x0f);
                        c[5] = BsonUtils.ToHexChar((_a >> 8) & 0x0f);
                        c[6] = BsonUtils.ToHexChar((_a >> 4) & 0x0f);
                        c[7] = BsonUtils.ToHexChar(_a & 0x0f);
                        c[8] = BsonUtils.ToHexChar((_b >> 12) & 0x0f);
                        c[9] = BsonUtils.ToHexChar((_b >> 8) & 0x0f);
                        c[10] = BsonUtils.ToHexChar((_b >> 4) & 0x0f);
                        c[11] = BsonUtils.ToHexChar(_b & 0x0f);
                        c[12] = BsonUtils.ToHexChar((_c >> 28) & 0x0f);
                        c[13] = BsonUtils.ToHexChar((_c >> 24) & 0x0f);
                        c[14] = BsonUtils.ToHexChar((_c >> 12) & 0x0f);
                        c[15] = BsonUtils.ToHexChar((_c >> 8) & 0x0f);
                        c[16] = BsonUtils.ToHexChar((_c >> 4) & 0x0f);
                        c[17] = BsonUtils.ToHexChar(_c & 0x0f);
                        return new string(c);
                    }
                case 10:
                    {
                        var c = new char[20];
                        c[0] = BsonUtils.ToHexChar((_a >> 28) & 0x0f);
                        c[1] = BsonUtils.ToHexChar((_a >> 24) & 0x0f);
                        c[2] = BsonUtils.ToHexChar((_a >> 20) & 0x0f);
                        c[3] = BsonUtils.ToHexChar((_a >> 16) & 0x0f);
                        c[4] = BsonUtils.ToHexChar((_a >> 12) & 0x0f);
                        c[5] = BsonUtils.ToHexChar((_a >> 8) & 0x0f);
                        c[6] = BsonUtils.ToHexChar((_a >> 4) & 0x0f);
                        c[7] = BsonUtils.ToHexChar(_a & 0x0f);
                        c[8] = BsonUtils.ToHexChar((_b >> 20) & 0x0f);
                        c[9] = BsonUtils.ToHexChar((_b >> 16) & 0x0f);
                        c[10] = BsonUtils.ToHexChar((_b >> 12) & 0x0f);
                        c[11] = BsonUtils.ToHexChar((_b >> 8) & 0x0f);
                        c[12] = BsonUtils.ToHexChar((_b >> 4) & 0x0f);
                        c[13] = BsonUtils.ToHexChar(_b & 0x0f);
                        c[14] = BsonUtils.ToHexChar((_c >> 28) & 0x0f);
                        c[15] = BsonUtils.ToHexChar((_c >> 24) & 0x0f);
                        c[16] = BsonUtils.ToHexChar((_c >> 12) & 0x0f);
                        c[17] = BsonUtils.ToHexChar((_c >> 8) & 0x0f);
                        c[18] = BsonUtils.ToHexChar((_c >> 4) & 0x0f);
                        c[19] = BsonUtils.ToHexChar(_c & 0x0f);
                        return new string(c);
                    }
                case 11:
                    {
                        var c = new char[22];
                        c[0] = BsonUtils.ToHexChar((_a >> 28) & 0x0f);
                        c[1] = BsonUtils.ToHexChar((_a >> 24) & 0x0f);
                        c[2] = BsonUtils.ToHexChar((_a >> 20) & 0x0f);
                        c[3] = BsonUtils.ToHexChar((_a >> 16) & 0x0f);
                        c[4] = BsonUtils.ToHexChar((_a >> 12) & 0x0f);
                        c[5] = BsonUtils.ToHexChar((_a >> 8) & 0x0f);
                        c[6] = BsonUtils.ToHexChar((_a >> 4) & 0x0f);
                        c[7] = BsonUtils.ToHexChar(_a & 0x0f);
                        c[8] = BsonUtils.ToHexChar((_b >> 28) & 0x0f);
                        c[9] = BsonUtils.ToHexChar((_b >> 24) & 0x0f);
                        c[10] = BsonUtils.ToHexChar((_b >> 20) & 0x0f);
                        c[11] = BsonUtils.ToHexChar((_b >> 16) & 0x0f);
                        c[12] = BsonUtils.ToHexChar((_b >> 12) & 0x0f);
                        c[13] = BsonUtils.ToHexChar((_b >> 8) & 0x0f);
                        c[14] = BsonUtils.ToHexChar((_b >> 4) & 0x0f);
                        c[15] = BsonUtils.ToHexChar(_b & 0x0f);
                        c[16] = BsonUtils.ToHexChar((_c >> 28) & 0x0f);
                        c[17] = BsonUtils.ToHexChar((_c >> 24) & 0x0f);
                        c[18] = BsonUtils.ToHexChar((_c >> 12) & 0x0f);
                        c[19] = BsonUtils.ToHexChar((_c >> 8) & 0x0f);
                        c[20] = BsonUtils.ToHexChar((_c >> 4) & 0x0f);
                        c[21] = BsonUtils.ToHexChar(_c & 0x0f);
                        return new string(c);
                    }
                case 12:
                    {
                        var c = new char[24];
                        c[0] = BsonUtils.ToHexChar((_a >> 28) & 0x0f);
                        c[1] = BsonUtils.ToHexChar((_a >> 24) & 0x0f);
                        c[2] = BsonUtils.ToHexChar((_a >> 20) & 0x0f);
                        c[3] = BsonUtils.ToHexChar((_a >> 16) & 0x0f);
                        c[4] = BsonUtils.ToHexChar((_a >> 12) & 0x0f);
                        c[5] = BsonUtils.ToHexChar((_a >> 8) & 0x0f);
                        c[6] = BsonUtils.ToHexChar((_a >> 4) & 0x0f);
                        c[7] = BsonUtils.ToHexChar(_a & 0x0f);
                        c[8] = BsonUtils.ToHexChar((_b >> 28) & 0x0f);
                        c[9] = BsonUtils.ToHexChar((_b >> 24) & 0x0f);
                        c[10] = BsonUtils.ToHexChar((_b >> 20) & 0x0f);
                        c[11] = BsonUtils.ToHexChar((_b >> 16) & 0x0f);
                        c[12] = BsonUtils.ToHexChar((_b >> 12) & 0x0f);
                        c[13] = BsonUtils.ToHexChar((_b >> 8) & 0x0f);
                        c[14] = BsonUtils.ToHexChar((_b >> 4) & 0x0f);
                        c[15] = BsonUtils.ToHexChar(_b & 0x0f);
                        c[16] = BsonUtils.ToHexChar((_c >> 28) & 0x0f);
                        c[17] = BsonUtils.ToHexChar((_c >> 24) & 0x0f);
                        c[18] = BsonUtils.ToHexChar((_c >> 20) & 0x0f);
                        c[19] = BsonUtils.ToHexChar((_c >> 16) & 0x0f);
                        c[20] = BsonUtils.ToHexChar((_c >> 12) & 0x0f);
                        c[21] = BsonUtils.ToHexChar((_c >> 8) & 0x0f);
                        c[22] = BsonUtils.ToHexChar((_c >> 4) & 0x0f);
                        c[23] = BsonUtils.ToHexChar(_c & 0x0f);
                        return new string(c);
                    }
                default:
                    throw new NotSupportedException(_d.ToString());
            }
        }

        public static byte[] MinValue12 => new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        #endregion


        /// <summary>
        /// �����ֲ�ʽId
        /// </summary>
        /// <returns></returns>
        public static string GenerateString() => new Id<Model>().ToString();

        /// <summary>
        /// ����GUID
        /// </summary>
        /// <returns></returns>
        public static string NextId()
        {
            return IDGen.NextID().ToString("N");
        }

        /// <summary>
        /// ��ID
        /// </summary>
        /// <returns></returns>
        public static string ShortId(int length)
        {
            return ShortIDGen.NextID(new GenerationOptions
            {
                UseNumbers = false, // ����������
                UseSpecialCharacters = false, // �����������
                Length = length    // ���ó���
            });
        }
    }

    /// <summary>
    /// Represents an UnqiueId (see also BsonUnqiueId).
    /// </summary>
    [Serializable]
    public class Id<E> : Id, IComparable<Id<E>>, IEquatable<Id<E>>, IConvertible
    {
        // private static fields
        //private static readonly Id<E> __emptyInstance = default(Id<E>);
        private static int __staticIncrement = (new Random()).Next();

        // constructors
        /// <summary>
        /// Initializes a new instance of the UnqiueId class.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        public Id(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            if(bytes.Length != 12)
            {
                throw new ArgumentException($@"Byte array must be 12 length", "bytes");
            }

            _d = (short)bytes.Length;
            FromByteArray(bytes, 0, _d, out _a, out _b, out _c);
        }

        /// <summary>
        /// Initializes a new instance of the UnqiueId class.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="index">The index into the byte array where the UnqiueId starts.</param>
        internal Id(byte[] bytes, int index)
        {
            _d = 12;
            FromByteArray(bytes, index, _d, out _a, out _b, out _c);
        }


        /// <summary>
        /// Initializes a new instance of the UnqiueId class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Id(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var bytes = BsonUtils.ParseHexString(value);
            _d = (short)(value.Length / 2);

            if (_d != 12) throw new NotSupportedException(_d.ToString());

            FromByteArray(bytes, 0, _d, out _a, out _b, out _c);
        }

        private Id(int a, int b, int c)
        {
            _a = a;
            _b = b;
            _c = c;
            _d = 12;
        }

        public Id()
        {
            int timestamp = GetTimestampFromDateTime(DateTime.UtcNow);
            int increment = Interlocked.Increment(ref __staticIncrement) & 0x00ffffff; // only use low order 3 bytes
            var random = Id._randomLong;

            if (random < 0 || random > 0xffffffffff)
            {
                throw new ArgumentOutOfRangeException(nameof(random), "The random value must be between 0 and 1099511627775 (it must fit in 5 bytes).");
            }
            if (increment < 0 || increment > 0xffffff)
            {
                throw new ArgumentOutOfRangeException(nameof(increment), "The increment value must be between 0 and 16777215 (it must fit in 3 bytes).");
            }

            _a = timestamp;
            _b = (int)(random >> 8); // first 4 bytes of random
            _c = (int)(random << 24) | increment; // 5th byte of random and 3 byte increment
            _d = 12;
        }

        //// public static properties
        ///// <summary>
        ///// Gets an instance of UnqiueId where the value is empty.
        ///// </summary>
        //public static Id<E> Empty
        //{
        //    get { return __emptyInstance; }
        //}

        // public properties
        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        public int Timestamp
        {
            get { return _a; }
        }

        /// <summary>
        /// Gets the creation time (derived from the timestamp).
        /// </summary>
        public DateTime CreationTime
        {
            get { return BsonConstants.UnixEpoch.AddSeconds((uint)Timestamp); }
        }

        // public operators
        /// <summary>
        /// Compares two UnqiueIds.
        /// </summary>
        /// <param name="lhs">The first UnqiueId.</param>
        /// <param name="rhs">The other UnqiueId</param>
        /// <returns>True if the first UnqiueId is less than the second UnqiueId.</returns>
        public static bool operator <(Id<E> lhs, Id<E> rhs)
        {
            return lhs.CompareTo(rhs) < 0;
        }

        /// <summary>
        /// Compares two UnqiueIds.
        /// </summary>
        /// <param name="lhs">The first UnqiueId.</param>
        /// <param name="rhs">The other UnqiueId</param>
        /// <returns>True if the first UnqiueId is less than or equal to the second UnqiueId.</returns>
        public static bool operator <=(Id<E> lhs, Id<E> rhs)
        {
            return lhs.CompareTo(rhs) <= 0;
        }

        /// <summary>
        /// Compares two UnqiueIds.
        /// </summary>
        /// <param name="lhs">The first UnqiueId.</param>
        /// <param name="rhs">The other UnqiueId.</param>
        /// <returns>True if the two UnqiueIds are equal.</returns>
        public static bool operator ==(Id<E> lhs, Id<E> rhs)
        {
            if (lhs is null)
            {
                if (rhs is null) return true;
                else return false;
            }
            else
            {
                if (rhs is null) return false;
                else return lhs.Equals(rhs);
            }
        }

        /// <summary>
        /// Compares two UnqiueIds.
        /// </summary>
        /// <param name="lhs">The first UnqiueId.</param>
        /// <param name="rhs">The other UnqiueId.</param>
        /// <returns>True if the two UnqiueIds are not equal.</returns>
        public static bool operator !=(Id<E> lhs, Id<E> rhs)
        {
            if (lhs is null)
            {
                if (rhs is null) return false;
                else return true;
            }
            else
            {
                if (rhs is null) return true;
                return !lhs.Equals(rhs);
            }
        }

        /// <summary>
        /// Compares two UnqiueIds.
        /// </summary>
        /// <param name="lhs">The first UnqiueId.</param>
        /// <param name="rhs">The other UnqiueId</param>
        /// <returns>True if the first UnqiueId is greather than or equal to the second UnqiueId.</returns>
        public static bool operator >=(Id<E> lhs, Id<E> rhs)
        {
            return lhs.CompareTo(rhs) >= 0;
        }

        /// <summary>
        /// Compares two UnqiueIds.
        /// </summary>
        /// <param name="lhs">The first UnqiueId.</param>
        /// <param name="rhs">The other UnqiueId</param>
        /// <returns>True if the first UnqiueId is greather than the second UnqiueId.</returns>
        public static bool operator >(Id<E> lhs, Id<E> rhs)
        {
            return lhs.CompareTo(rhs) > 0;
        }

        // public static methods
        /// <summary>
        /// Generates a new UnqiueId with a unique value.
        /// </summary>
        /// <returns>An UnqiueId.</returns>
        internal static Id<E> GenerateNewId()
        {
            return GenerateNewId(GetTimestampFromDateTime(DateTime.UtcNow));
        }

        /// <summary>
        /// Generates a new UnqiueId with a unique value (with the timestamp component based on a given DateTime).
        /// </summary>
        /// <param name="timestamp">The timestamp component (expressed as a DateTime).</param>
        /// <returns>An UnqiueId.</returns>
        internal static Id<E> GenerateNewId(DateTime timestamp)
        {
            return GenerateNewId(GetTimestampFromDateTime(timestamp));
        }

        /// <summary>
        /// Generates a new UnqiueId with a unique value (with the given timestamp).
        /// </summary>
        /// <param name="timestamp">The timestamp component.</param>
        /// <returns>An UnqiueId.</returns>
        internal static Id<E> GenerateNewId(int timestamp)
        {
            int increment = Interlocked.Increment(ref __staticIncrement) & 0x00ffffff; // only use low order 3 bytes
            return Create(timestamp, Id._randomLong, increment);
        }

        /// <summary>
        /// Parses a string and creates a new UnqiueId.
        /// </summary>
        /// <param name="s">The string value.</param>
        /// <returns>A UnqiueId.</returns>
        public static Id<E> Parse(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }

            Id<E> UnqiueId;
            if (TryParse(s, out UnqiueId))
            {
                return UnqiueId;
            }
            else
            {
                var message = string.Format("'{0}' is not a valid 24 digit hex string.", s);
                throw new FormatException(message);
            }
        }

        /// <summary>
        /// Tries to parse a string and create a new UnqiueId.
        /// </summary>
        /// <param name="s">The string value.</param>
        /// <param name="UnqiueId">The new UnqiueId.</param>
        /// <returns>True if the string was parsed successfully.</returns>
        public static bool TryParse(string s, out Id<E> UnqiueId)
        {
            // don't throw ArgumentNullException if s is null
            if (s != null && s.Length == 24)
            {
                byte[] bytes;
                if (BsonUtils.TryParseHexString(s, out bytes))
                {
                    UnqiueId = new Id<E>(bytes);
                    return true;
                }
            }

            UnqiueId = default;
            return false;
        }


        private static Id<E> Create(int timestamp, long random, int increment)
        {
            if (random < 0 || random > 0xffffffffff)
            {
                throw new ArgumentOutOfRangeException(nameof(random), "The random value must be between 0 and 1099511627775 (it must fit in 5 bytes).");
            }
            if (increment < 0 || increment > 0xffffff)
            {
                throw new ArgumentOutOfRangeException(nameof(increment), "The increment value must be between 0 and 16777215 (it must fit in 3 bytes).");
            }

            var a = timestamp;
            var b = (int)(random >> 8); // first 4 bytes of random
            var c = (int)(random << 24) | increment; // 5th byte of random and 3 byte increment
            return new Id<E>(a, b, c);
        }

        

        private static int GetTimestampFromDateTime(DateTime timestamp)
        {
            var secondsSinceEpoch = (long)Math.Floor((BsonUtils.ToUniversalTime(timestamp) - BsonConstants.UnixEpoch).TotalSeconds);
            if (secondsSinceEpoch < uint.MinValue || secondsSinceEpoch > uint.MaxValue)
            {
                throw new ArgumentOutOfRangeException("timestamp");
            }
            return (int)(uint)secondsSinceEpoch;
        }

        private static void FromByteArray(byte[] bytes, int offset, short length, out int a, out int b, out int c)
        {
            switch (length)
            {
                case 9:
                    a = (bytes[offset] << 24) | (bytes[offset + 1] << 16) | (bytes[offset + 2] << 8) | bytes[offset + 3];
                    b = (bytes[offset + 4] << 8) | bytes[offset + 5];
                    c = (bytes[offset + 6] << 24) | (bytes[offset + 7] << 8) | bytes[offset + 8];
                    break;
                case 10:
                    a = (bytes[offset] << 24) | (bytes[offset + 1] << 16) | (bytes[offset + 2] << 8) | bytes[offset + 3];
                    b = (bytes[offset + 4] << 16) | (bytes[offset + 5] << 8) | bytes[offset + 6];
                    c = (bytes[offset + 7] << 24) | (bytes[offset + 8] << 8) | bytes[offset + 9];
                    break;
                case 11:
                    a = (bytes[offset] << 24) | (bytes[offset + 1] << 16) | (bytes[offset + 2] << 8) | bytes[offset + 3];
                    b = (bytes[offset + 4] << 24) | (bytes[offset + 5] << 16) | (bytes[offset + 6] << 8) | bytes[offset + 7];
                    c = (bytes[offset + 8] << 24) | (bytes[offset + 9] << 8) | bytes[offset + 10];
                    break;
                case 12:
                    a = (bytes[offset] << 24) | (bytes[offset + 1] << 16) | (bytes[offset + 2] << 8) | bytes[offset + 3];
                    b = (bytes[offset + 4] << 24) | (bytes[offset + 5] << 16) | (bytes[offset + 6] << 8) | bytes[offset + 7];
                    c = (bytes[offset + 8] << 24) | (bytes[offset + 9] << 16) | (bytes[offset + 10] << 8) | bytes[offset + 11];
                    break;
                default:
                    throw new NotSupportedException(length.ToString());
            }
        }

        // public methods
        /// <summary>
        /// Compares this UnqiueId to another UnqiueId.
        /// </summary>
        /// <param name="other">The other UnqiueId.</param>
        /// <returns>A 32-bit signed integer that indicates whether this UnqiueId is less than, equal to, or greather than the other.</returns>
        public int CompareTo(Id<E> other)
        {
            int result = ((uint)_a).CompareTo((uint)other._a);
            if (result != 0) { return result; }
            result = ((uint)_b).CompareTo((uint)other._b);
            if (result != 0) { return result; }
            return ((uint)_c).CompareTo((uint)other._c);
        }

        /// <summary>
        /// Compares this UnqiueId to another UnqiueId.
        /// </summary>
        /// <param name="rhs">The other UnqiueId.</param>
        /// <returns>True if the two UnqiueIds are equal.</returns>
        public bool Equals(Id<E> rhs)
        {
            return
                _a == rhs._a &&
                _b == rhs._b &&
                _c == rhs._c;
        }

        /// <summary>
        /// Compares this UnqiueId to another object.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>True if the other object is an UnqiueId and equal to this one.</returns>
        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                if (obj is Id<E>)
                {
                    return Equals((Id<E>)obj);
                }
                else
                {
                    return false;
                }
            }
            else return false;
        }

        // explicit IConvertible implementation
        TypeCode IConvertible.GetTypeCode()
        {
            return TypeCode.Object;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return ToString();
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            switch (Type.GetTypeCode(conversionType))
            {
                case TypeCode.String:
                    return ((IConvertible)this).ToString(provider);
                case TypeCode.Object:
                    if (conversionType == typeof(object) || conversionType == typeof(Id<E>))
                    {
                        return this;
                    }
                    //if (conversionType == typeof(BsonUnqiueId))
                    //{
                    //    return new BsonUnqiueId(this);
                    //}
                    //if (conversionType == typeof(BsonString))
                    //{
                    //    return new BsonString(((IConvertible)this).ToString(provider));
                    //}
                    break;
            }

            throw new InvalidCastException();
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

namespace Yang.Core
{

    /// <summary>
    /// A static class containing BSON utility methods.
    /// </summary>
    internal static class BsonUtils
    {
        // public static methods
        /// <summary>
        /// Gets a friendly class name suitable for use in error messages.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A friendly class name.</returns>
        public static string GetFriendlyTypeName(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (!typeInfo.IsGenericType)
            {
                return type.Name;
            }

            var sb = new StringBuilder();
            sb.AppendFormat("{0}<", Regex.Replace(type.Name, @"\`\d+$", ""));
            foreach (var typeParameter in type.GetTypeInfo().GetGenericArguments())
            {
                sb.AppendFormat("{0}, ", GetFriendlyTypeName(typeParameter));
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append('>');
            return sb.ToString();
        }

        /// <summary>
        /// Parses a hex string into its equivalent byte array.
        /// </summary>
        /// <param name="s">The hex string to parse.</param>
        /// <returns>The byte equivalent of the hex string.</returns>
        public static byte[] ParseHexString(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            byte[] bytes;
            if (!TryParseHexString(s, out bytes))
            {
                throw new FormatException("String should contain only hexadecimal digits.");
            }

            return bytes;
        }

        /// <summary>
        /// Converts from number of milliseconds since Unix epoch to DateTime.
        /// </summary>
        /// <param name="millisecondsSinceEpoch">The number of milliseconds since Unix epoch.</param>
        /// <returns>A DateTime.</returns>
        public static DateTime ToDateTimeFromMillisecondsSinceEpoch(long millisecondsSinceEpoch)
        {
            if (millisecondsSinceEpoch < BsonConstants.DateTimeMinValueMillisecondsSinceEpoch ||
                millisecondsSinceEpoch > BsonConstants.DateTimeMaxValueMillisecondsSinceEpoch)
            {
                var message = string.Format(
                    "The value {0} for the BsonDateTime MillisecondsSinceEpoch is outside the" +
                    "range that can be converted to a .NET DateTime.",
                    millisecondsSinceEpoch);
                throw new ArgumentOutOfRangeException("millisecondsSinceEpoch", message);
            }

            // MaxValue has to be handled specially to avoid rounding errors
            if (millisecondsSinceEpoch == BsonConstants.DateTimeMaxValueMillisecondsSinceEpoch)
            {
                return DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc);
            }
            else
            {
                return BsonConstants.UnixEpoch.AddTicks(millisecondsSinceEpoch * 10000);
            }
        }

        /// <summary>
        /// Converts a value to a hex character.
        /// </summary>
        /// <param name="value">The value (assumed to be between 0 and 15).</param>
        /// <returns>The hex character.</returns>
        public static char ToHexChar(int value)
        {
            return (char)(value + (value < 10 ? '0' : 'a' - 10));
        }

        /// <summary>
        /// Converts a byte array to a hex string.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        /// <returns>A hex string.</returns>
        public static string ToHexString(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            var length = bytes.Length;
            var c = new char[length * 2];

            for (int i = 0, j = 0; i < length; i++)
            {
                var b = bytes[i];
                c[j++] = ToHexChar(b >> 4);
                c[j++] = ToHexChar(b & 0x0f);
            }

            return new string(c);
        }

        /// <summary>
        /// Converts a DateTime to local time (with special handling for MinValue and MaxValue).
        /// </summary>
        /// <param name="dateTime">A DateTime.</param>
        /// <returns>The DateTime in local time.</returns>
        public static DateTime ToLocalTime(DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
            {
                return DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Local);
            }
            else if (dateTime == DateTime.MaxValue)
            {
                return DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Local);
            }
            else
            {
                return dateTime.ToLocalTime();
            }
        }

        /// <summary>
        /// Converts a DateTime to number of milliseconds since Unix epoch.
        /// </summary>
        /// <param name="dateTime">A DateTime.</param>
        /// <returns>Number of seconds since Unix epoch.</returns>
        public static long ToMillisecondsSinceEpoch(DateTime dateTime)
        {
            var utcDateTime = ToUniversalTime(dateTime);
            return (utcDateTime - BsonConstants.UnixEpoch).Ticks / 10000;
        }

        /// <summary>
        /// Converts a DateTime to UTC (with special handling for MinValue and MaxValue).
        /// </summary>
        /// <param name="dateTime">A DateTime.</param>
        /// <returns>The DateTime in UTC.</returns>
        public static DateTime ToUniversalTime(DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
            {
                return DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
            }
            else if (dateTime == DateTime.MaxValue)
            {
                return DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc);
            }
            else
            {
                return dateTime.ToUniversalTime();
            }
        }

        /// <summary>
        /// Tries to parse a hex string to a byte array.
        /// </summary>
        /// <param name="s">The hex string.</param>
        /// <param name="bytes">A byte array.</param>
        /// <returns>True if the hex string was successfully parsed.</returns>
        public static bool TryParseHexString(string s, out byte[] bytes)
        {
            bytes = null;

            if (s == null)
            {
                return false;
            }

            var buffer = new byte[(s.Length + 1) / 2];

            var i = 0;
            var j = 0;

            if ((s.Length % 2) == 1)
            {
                // if s has an odd length assume an implied leading "0"
                int y;
                if (!TryParseHexChar(s[i++], out y))
                {
                    return false;
                }
                buffer[j++] = (byte)y;
            }

            while (i < s.Length)
            {
                int x, y;
                if (!TryParseHexChar(s[i++], out x))
                {
                    return false;
                }
                if (!TryParseHexChar(s[i++], out y))
                {
                    return false;
                }
                buffer[j++] = (byte)((x << 4) | y);
            }

            bytes = buffer;
            return true;
        }

        // private static methods
        private static bool TryParseHexChar(char c, out int value)
        {
            if (c >= '0' && c <= '9')
            {
                value = c - '0';
                return true;
            }

            if (c >= 'a' && c <= 'f')
            {
                value = 10 + (c - 'a');
                return true;
            }

            if (c >= 'A' && c <= 'F')
            {
                value = 10 + (c - 'A');
                return true;
            }

            value = 0;
            return false;
        }
    }
}

namespace Yang.Core
{
    internal class BsonConstants
    {

        // private static fields
        private static readonly long __dateTimeMaxValueMillisecondsSinceEpoch;
        private static readonly long __dateTimeMinValueMillisecondsSinceEpoch;
        private static readonly DateTime __unixEpoch;

        // static constructor
        static BsonConstants()
        {
            // unixEpoch has to be initialized first
            __unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            __dateTimeMaxValueMillisecondsSinceEpoch = (DateTime.MaxValue - __unixEpoch).Ticks / 10000;
            __dateTimeMinValueMillisecondsSinceEpoch = (DateTime.MinValue - __unixEpoch).Ticks / 10000;
        }

        // public static properties
        /// <summary>
        /// Gets the number of milliseconds since the Unix epoch for DateTime.MaxValue.
        /// </summary>
        public static long DateTimeMaxValueMillisecondsSinceEpoch
        {
            get { return __dateTimeMaxValueMillisecondsSinceEpoch; }
        }

        /// <summary>
        /// Gets the number of milliseconds since the Unix epoch for DateTime.MinValue.
        /// </summary>
        public static long DateTimeMinValueMillisecondsSinceEpoch
        {
            get { return __dateTimeMinValueMillisecondsSinceEpoch; }
        }

        /// <summary>
        /// Gets the Unix Epoch for BSON DateTimes (1970-01-01).
        /// </summary>
        public static DateTime UnixEpoch { get { return __unixEpoch; } }

    }

    public abstract class Model
    {

    }
}


