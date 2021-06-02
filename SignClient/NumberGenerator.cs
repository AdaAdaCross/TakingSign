using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading;

namespace SignClient
{
    class NumberGenerator
    {
        private BigInteger p, g, e, iq, d;
        private Random rnd;

        public BigInteger P
        {
            get
            {
                return p;
            }
        }

        public BigInteger Q
        {
            get
            {
                return g;
            }
        }

        public BigInteger N
        {
            get
            {
                return p * g;
            }
        }

        public BigInteger E
        {
            get
            {
                return e;
            }
        }

        public BigInteger InverseQ
        {
            get
            {
                return iq;
            }
        }

        public BigInteger D
        {
            get
            {
                return d;
            }
        }

        public BigInteger DP
        {
            get
            {
                return d % (p - 1);
            }
        }

        public BigInteger DQ
        {
            get
            {
                return d % (g - 1);
            }
        }

        public NumberGenerator(int seed)
        {
            rnd = new Random(seed);

            InitNums();
        }

        public void InitNums()
        {
            int keysize = 64;
            p = GenSimple(keysize);
            g = GenSimple(keysize);
            e = new BigInteger(new byte[] { 1, 0, 1 });
            iq = FindInvers(g, p);
            d = FindInvers(e, (p - 1) * (g - 1));
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
    }
}
