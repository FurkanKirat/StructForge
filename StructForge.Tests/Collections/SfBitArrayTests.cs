using StructForge.Collections;

namespace StructForge.Tests.Collections
{
    public class SfBitArrayTests
    {
        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            int size = 130;
            var bits = new SfBitArray(size);

            Assert.Equal(size, bits.Count);
            Assert.False(bits.IsEmpty);
            for (int i = 0; i < size; i++)
                Assert.False(bits[i]);
        }

        [Fact]
        public void Indexer_GetSet_WorksCorrectly()
        {
            var bits = new SfBitArray(10);
            bits[3] = true;
            bits[7] = true;

            Assert.True(bits[3]);
            Assert.True(bits[7]);
            Assert.False(bits[0]);
            Assert.False(bits[9]);

            bits[3] = false;
            Assert.False(bits[3]);
        }

        [Fact]
        public void Indexer_OutOfRange_Throws()
        {
            var bits = new SfBitArray(5);

            Assert.Throws<ArgumentOutOfRangeException>(() => bits[-1] = true);
            Assert.Throws<ArgumentOutOfRangeException>(() => bits[5] = true);
            Assert.Throws<ArgumentOutOfRangeException>(() => { var x = bits[5]; });
        }

        [Fact]
        public void Toggle_WorksCorrectly()
        {
            var bits = new SfBitArray(5);
            bits.Toggle(2);
            Assert.True(bits[2]);

            bits.Toggle(2);
            Assert.False(bits[2]);
        }

        [Fact]
        public void Clear_WorksCorrectly()
        {
            var bits = new SfBitArray(5);
            bits[0] = true;
            bits[4] = true;

            bits.Clear();

            for (int i = 0; i < bits.Count; i++)
                Assert.False(bits[i]);
        }

        [Fact]
        public void SetAll_WorksCorrectly()
        {
            var bits = new SfBitArray(70);
            bits.SetAll(true);

            for (int i = 0; i < bits.Count; i++)
                Assert.True(bits[i]);

            bits.SetAll(false);

            for (int i = 0; i < bits.Count; i++)
                Assert.False(bits[i]);
        }

        [Fact]
        public void Not_WorksCorrectly()
        {
            var bits = new SfBitArray(10);
            bits[1] = true;
            bits[3] = true;

            bits.Not();

            for (int i = 0; i < bits.Count; i++)
            {
                if (i == 1 || i == 3) Assert.False(bits[i]);
                else Assert.True(bits[i]);
            }
        }

        [Fact]
        public void And_Or_Xor_WorkCorrectly()
        {
            var a = new SfBitArray(5);
            var b = new SfBitArray(5);

            a[0] = true; a[1] = true;
            b[1] = true; b[2] = true;

            // And
            var c = new SfBitArray(a);
            c.And(b);
            Assert.False(c[0]);
            Assert.True(c[1]);
            Assert.False(c[2]);

            // Or
            var d = new SfBitArray(a);
            d.Or(b);
            Assert.True(d[0]);
            Assert.True(d[1]);
            Assert.True(d[2]);

            // Xor
            var e = new SfBitArray(a);
            e.Xor(b);
            Assert.True(e[0]);
            Assert.False(e[1]);
            Assert.True(e[2]);
        }

        [Fact]
        public void CountTrueFalse_WorksCorrectly()
        {
            var bits = new SfBitArray(5);
            bits[0] = true;
            bits[2] = true;

            Assert.Equal(2, bits.CountTrue());
            Assert.Equal(3, bits.CountFalse());
        }

        [Fact]
        public void CopyTo_WorksCorrectly()
        {
            var bits = new SfBitArray(5);
            bits[1] = true;
            bits[3] = true;

            var array = new bool[5];
            bits.CopyTo(array, 0);

            Assert.False(array[0]);
            Assert.True(array[1]);
            Assert.False(array[2]);
            Assert.True(array[3]);
            Assert.False(array[4]);
        }

        [Fact]
        public void CopyTo_ThrowsOnInvalidArguments()
        {
            var bits = new SfBitArray(5);
            bool[] array = null;

            Assert.Throws<ArgumentNullException>(() => bits.CopyTo(array, 0));
            array = new bool[3];
            Assert.Throws<ArgumentException>(() => bits.CopyTo(array, 0));
            array = new bool[5];
            Assert.Throws<ArgumentOutOfRangeException>(() => bits.CopyTo(array, -1));
        }

        [Fact]
        public void Contains_WorksCorrectly()
        {
            var bits = new SfBitArray(5);
            bits[2] = true;

            Assert.True(bits.Contains(true));
            Assert.True(bits.Contains(true, null)); // default comparer
            Assert.True(bits.Contains(false));
        }

        [Fact]
        public void Enumerator_WorksCorrectly()
        {
            bool[] array = new bool[5];
            array[0] = true;
            var bits = new SfBitArray(5);
            bits[0] = true;

            int index = 0;
            foreach (bool a in bits)
            {
                Assert.Equal(a, array[index++]);
            }
            Assert.Equal(5, index);
        }
    }
}
