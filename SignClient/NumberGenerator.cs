using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading;

namespace SignClient
{
    public class CertParams
    {
        public bool hasParams;
        public string savePath;
        public string subject;
        public DateTimeOffset endDate;
        public bool exportSecret;

        public CertParams()
        {
            hasParams = false;
        }

        public CertParams(
            string _savePath,
            string _subject,
            DateTime _endDate,
            bool _exportSecret)
        {
            hasParams = true;
            savePath = _savePath;
            subject = _subject;
            endDate = _endDate;
            exportSecret = _exportSecret;
        }
    }


    class NumberGenerator
    {
        private BigInteger p, g, e, iq, d;
        private Random rnd;
        int keysize = 128;

        public byte[] P
        {
            get
            {
                return CopyAndReverse(p.ToByteArray(), keysize);
            }
        }

        public byte[] Q
        {
            get
            {
                return CopyAndReverse(g.ToByteArray(), keysize);
            }
        }

        public byte[] N
        {
            get
            {
                BigInteger n = p * g;
                return CopyAndReverse((n).ToByteArray(), keysize * 2);
            }
        }

        public byte[] E
        {
            get
            {
                return e.ToByteArray();
            }
        }

        public byte[] InverseQ
        {
            get
            {
                return CopyAndReverse(iq.ToByteArray(), keysize);
            }
        }

        public byte[] D
        {
            get
            {
                return CopyAndReverse(d.ToByteArray(), keysize * 2);
            }
        }

        public byte[] DP
        {
            get
            {
                return CopyAndReverse((d % (p - 1)).ToByteArray(), keysize);
            }
        }

        public byte[] DQ
        {
            get
            {
                return CopyAndReverse((d % (g - 1)).ToByteArray(), keysize);
            }
        }

        public NumberGenerator(int seed)
        {
            rnd = new Random(seed);

            InitNums();
        }

        public void InitNums()
        {
            p = GenSimple(keysize);
            g = GenSimple(keysize);
            e = new BigInteger(new byte[] { 1, 0, 1 });
            iq = FindInvers(g, p);
            d = FindInvers(e, (p - 1) * (g - 1));

            BigInteger check = (d * e) % ((p - 1) * (g - 1));
        }

        private bool IsSimple(BigInteger x)
        {
            BigInteger x1 = 0, y1 = 0;
            if (x == 2)
                return true;
            for (int i=0; i<100; i++)
            {
                BigInteger a = GenNumber(x - 2) + 2;
                if (NOD(a, x, ref x1, ref y1) != 1)
                    return false;
                if (BigInteger.ModPow(a, x - 1, x) != 1)
                    return false;
            }
            return true;
        }

        private BigInteger NOD(BigInteger a, BigInteger b, ref BigInteger x, ref BigInteger y)
        {
            if (a == 0)
            {
                x = 0; y = 1;
                return b;
            }
            BigInteger x1 = 0, y1 = 0;
            BigInteger d = NOD(b % a, a, ref x1, ref y1);
            x = y1 - (b / a) * x1;
            y = x1;
            return d;
        }

        private BigInteger FindInvers(BigInteger a, BigInteger m)
        {
            BigInteger x = 0, y = 0;
            BigInteger g = NOD(a, m, ref x, ref y);
            if (g != 1)
                return 0;
            else
            {
                BigInteger result = (x % m + m) % m;
                BigInteger check = (a * result) % m;
                return result;
            }
        }

        private BigInteger GenNumber(BigInteger max)
        {
            int size = max.ToByteArray().Length;
            byte[] RowNum = new byte[size];
            rnd.NextBytes(RowNum);
            BigInteger Num = new BigInteger(RowNum);
            if (Num < 0) Num = -Num;
            Num %= max;
            return Num;
        }

        private BigInteger GenSimple(int size)
        {
            byte[] RowNum = new byte[32];
            rnd.NextBytes(RowNum);
            BigInteger Num = new BigInteger(RowNum);
            if (Num < 0) Num = -Num;
            if (Num % 2 == 0)
                Num++;

            while (!IsSimple(Num))
            {
                Num += 2;
            }

            int t = 32;
            while (t < size)
            {
                RowNum = new byte[t];
                rnd.NextBytes(RowNum);
                BigInteger N = new BigInteger(RowNum);
                if (N < 0) N = -N;
                if (N % 2 != 0)
                    N++;
                do
                {
                    N += 2;
                } while (!IsSimple(N * Num + 1));
                Num = N * Num + 1;
                t *= 2;
            }


            size = Num.ToByteArray().Length;
            return Num;
        }

        private byte[] CopyAndReverse(byte[] data, int len)
        {
            byte[] reversed = new byte[len];
            Array.Fill<byte>(reversed, 0);
            Array.Copy(data, 0, reversed, 0, data.Length);
            Array.Reverse(reversed);
            return reversed;
        }
    }
}
