using StructForge.Collections;

namespace StructForge.Tests.Collections
{
    public enum SfSimpleEnum { A, B, C } // 0, 1, 2
    public enum SfSparseEnum { First = 0, Middle = 50, Last = 100 }
    public enum SfNegativeEnum { Minus = -5, Zero = 0, Plus = 5 }
    public enum SfByteEnum : byte { Small = 1, Big = 255 }
    public enum SfLargeUShortEnum : ushort
    {
        Small = 10,
        Border = 32767,
        Large = 40000, // Critical Value: Negative as signed
        Max = 65535
    }

    public class SfEnumSetTests
    {
        [Fact]
        public void Add_And_Contains_ShouldWork()
        {
            var set = new SfEnumSet<SfSimpleEnum>();
            
            Assert.DoesNotContain(SfSimpleEnum.A, set);
            
            set.Add(SfSimpleEnum.A);
            set.Add(SfSimpleEnum.C);
            
            Assert.Contains(SfSimpleEnum.A, set);
            Assert.DoesNotContain(SfSimpleEnum.B, set);
            Assert.Contains(SfSimpleEnum.C, set);
            Assert.Equal(2, set.Count);
        }

        [Fact]
        public void NegativeEnum_ShouldWork_WithOffset()
        {
            var set = new SfEnumSet<SfNegativeEnum>();
            
            set.Add(SfNegativeEnum.Minus);
            set.Add(SfNegativeEnum.Plus);

            Assert.Contains(SfNegativeEnum.Minus, set);
            Assert.DoesNotContain(SfNegativeEnum.Zero, set);
            Assert.Contains(SfNegativeEnum.Plus, set);
        }

        [Fact]
        public void SparseEnum_ShouldHandleGaps()
        {
            var set = new SfEnumSet<SfSparseEnum>();
            
            set.Add(SfSparseEnum.First);
            set.Add(SfSparseEnum.Last);

            Assert.Equal(2, set.Count);
            Assert.Contains(SfSparseEnum.Last, set);
        }

        [Fact]
        public void Enumerator_ShouldReturnCorrectValues()
        {
            var set = new SfEnumSet<SfSimpleEnum>();
            set.Add(SfSimpleEnum.C); // 2
            set.Add(SfSimpleEnum.A); // 0
            
            var list = new List<SfSimpleEnum>();
            foreach (var item in set)
            {
                list.Add(item);
            }

            Assert.Equal(2, list.Count);
            Assert.Equal(SfSimpleEnum.A, list[0]);
            Assert.Equal(SfSimpleEnum.C, list[1]);
        }

        [Fact]
        public void UnionWith_ShouldMergeSets()
        {
            var set1 = new SfEnumSet<SfSimpleEnum> { SfSimpleEnum.A };
            var set2 = new SfEnumSet<SfSimpleEnum> { SfSimpleEnum.B, SfSimpleEnum.C };

            set1.UnionWith(set2); // A + (B, C)

            Assert.Equal(3, set1.Count);
            Assert.Contains(SfSimpleEnum.A, set1);
            Assert.Contains(SfSimpleEnum.B, set1);
            Assert.Contains(SfSimpleEnum.C, set1);
        }

        [Fact]
        public void IntersectWith_ShouldKeepCommon()
        {
            var set1 = new SfEnumSet<SfSimpleEnum> { SfSimpleEnum.A, SfSimpleEnum.B };
            var set2 = new SfEnumSet<SfSimpleEnum> { SfSimpleEnum.B, SfSimpleEnum.C };

            set1.IntersectWith(set2);

            Assert.Single(set1);
            Assert.DoesNotContain(SfSimpleEnum.A, set1);
            Assert.Contains(SfSimpleEnum.B, set1);
            Assert.DoesNotContain(SfSimpleEnum.C, set1);
        }

        [Fact]
        public void ExceptWith_ShouldRemoveItems()
        {
            var set1 = new SfEnumSet<SfSimpleEnum> { SfSimpleEnum.A, SfSimpleEnum.B };
            var set2 = new SfEnumSet<SfSimpleEnum> { SfSimpleEnum.B };

            set1.ExceptWith(set2); // A, B - B = A

            Assert.Single(set1);
            Assert.Contains(SfSimpleEnum.A, set1);
            Assert.DoesNotContain(SfSimpleEnum.B, set1);
        }

        [Fact]
        public void CopyTo_ShouldCopyOnlyPresentItems()
        {
            var set = new SfEnumSet<SfNegativeEnum>();
            set.Add(SfNegativeEnum.Zero);
            set.Add(SfNegativeEnum.Plus);

            var arr = new SfNegativeEnum[2];
            set.CopyTo(arr, 0);

            Assert.Equal(SfNegativeEnum.Zero, arr[0]);
            Assert.Equal(SfNegativeEnum.Plus, arr[1]);
        }

        [Fact]
        public void ByteEnum_ShouldWork()
        {
            var set = new SfEnumSet<SfByteEnum>();
            set.Add(SfByteEnum.Big); // 255
            
            Assert.Contains(SfByteEnum.Big, set);
            Assert.DoesNotContain(SfByteEnum.Small, set);
        }
        
        [Fact]
        public void UShortEnum_ShouldHandleLargeValuesCorrectly()
        {
            var set = new SfEnumSet<SfLargeUShortEnum>();
    
            set.Add(SfLargeUShortEnum.Large); 
            set.Add(SfLargeUShortEnum.Max);

            Assert.Contains(SfLargeUShortEnum.Large, set);
            Assert.Contains(SfLargeUShortEnum.Max, set);
            Assert.DoesNotContain(SfLargeUShortEnum.Small, set);
    
            Assert.Equal(2, set.Count);
        }
    }
    
    public class SfEnumSetConstructorTests
    {
        // Test için örnek Enum
        private enum TestFlags
        {
            None = 0,
            Alpha = 1,
            Beta = 2,
            Gamma = 50, // Uzak bir index
            Delta = 63, // 1. ulong'un sonu
            Omega = 100 // 2. ulong'a taşan değer
        }

        [Fact]
        public void Constructor_FromUlongArray_RestoresStateCorrectly()
        {
            // 1. Arrange: Orijinal bir set oluştur ve doldur
            var originalSet = new SfEnumSet<TestFlags>();
            originalSet.Add(TestFlags.Alpha);
            originalSet.Add(TestFlags.Omega); // 100. bit
            
            // Veriyi dışarı al (Simülasyon: Serialization)
            // AsSpan().ToArray() ile raw ulong dizisini alıyoruz
            ulong[] rawBits = originalSet.AsSpan().ToArray();

            // 2. Act: Raw array'den yeni set oluştur
            var restoredSet = new SfEnumSet<TestFlags>(rawBits);

            // 3. Assert: Her şey aynı mı?
            Assert.Equal(originalSet.Count, restoredSet.Count); // Count doğru hesaplandı mı?
            Assert.True(restoredSet.Contains(TestFlags.Alpha));
            Assert.True(restoredSet.Contains(TestFlags.Omega));
            Assert.False(restoredSet.Contains(TestFlags.Beta)); // Olmayan gelmemeli
        }

        [Fact]
        public void Constructor_FromUlongArray_CalculatesCountTrue()
        {
            // Manuel bir ulong dizisi hazırlayalım
            // 1. ulong: 5 (binary: ...101) -> 0. ve 2. bitler set
            // 2. ulong: 1 (binary: ...001) -> 64. bit set
            ulong[] manualBits = new ulong[] { 5, 1 }; 

            var set = new SfEnumSet<TestFlags>(manualBits);

            // Toplam 3 bit set edilmiş olmalı
            Assert.Equal(3, set.Count);
        }

        [Fact]
        public void Constructor_FromNullArray_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => new SfEnumSet<TestFlags>((ulong[])null));
        }

        [Fact]
        public void Constructor_FromEmptyArray_CreatesEmptySet()
        {
            var set = new SfEnumSet<TestFlags>(Array.Empty<ulong>());
            
            Assert.Equal(0, set.Count);
            Assert.True(set.IsEmpty);
        }
    }
}