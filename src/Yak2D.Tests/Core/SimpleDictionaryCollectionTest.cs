using NSubstitute;
using Xunit;
using Yak2D.Core;
using Yak2D.Graphics;
using Yak2D.Internal;

namespace Yak2D.Tests
{
    public class SimpleDictionaryCollectionTest
    {
        [Fact]
        public void SimpleDictionaryCollection_Add_ValidateCount()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISimpleCollection<int> collection = new SimpleDictionaryCollection<int>(messenger, 16);

            collection.Add(25UL, 6);
            collection.Add(35UL, 7);
            collection.Add(45UL, 8);
            collection.Add(55UL, 9);

            Assert.Equal(4, collection.Count);
        }

        [Fact]
        public void SimpleDictionaryCollection_Remove_ValidateCount()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISimpleCollection<int> collection = new SimpleDictionaryCollection<int>(messenger, 16);

            collection.Add(25UL, 6);
            collection.Add(35UL, 7);
            collection.Add(45UL, 8);
            collection.Add(55UL, 9);

            collection.Remove(45UL);

            Assert.Equal(3, collection.Count);
        }

        [Fact]
        public void SimpleDictionaryCollection_Add_EnsureWillNotAddAnExisting()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISimpleCollection<int> collection = new SimpleDictionaryCollection<int>(messenger, 16);

            collection.Add(25UL, 6);
            Assert.False(collection.Add(25UL, 7));
        }

        [Fact]
        public void SimpleDictionaryCollection_RemoveAll_ValidateCount()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISimpleCollection<int> collection = new SimpleDictionaryCollection<int>(messenger, 16);

            collection.Add(25UL, 6);
            collection.Add(35UL, 7);
            collection.Add(45UL, 8);
            collection.Add(55UL, 9);

            collection.RemoveAll();

            Assert.Equal(0, collection.Count);
        }

        [Fact]
        public void SimpleDictionaryCollection_Remove_EnsureWillNotRemoveItemThatDoesntExist()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISimpleCollection<int> collection = new SimpleDictionaryCollection<int>(messenger, 16);

            collection.Add(25UL, 6);
            collection.Add(35UL, 7);
            collection.Add(45UL, 8);
            collection.Add(55UL, 9);

            collection.RemoveAll();

            Assert.False(collection.Remove(25UL));
        }


        [Fact]
        public void SimpleDictionaryCollection_Contains_EnsureCorrectResponse()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISimpleCollection<int> collection = new SimpleDictionaryCollection<int>(messenger, 16);

            collection.Add(25UL, 6);
            collection.Add(35UL, 7);
            collection.Add(45UL, 8);
            collection.Add(55UL, 9);

            Assert.True(collection.Contains(25UL));
            Assert.False(collection.Contains(65UL));
        }

        [Fact]
        public void SimpleDictionaryCollection_Retrieve_EnsureCorrectReturned()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISimpleCollection<int> collection = new SimpleDictionaryCollection<int>(messenger, 16);

            collection.Add(25UL, 6);
            collection.Add(35UL, 7);
            collection.Add(45UL, 8);
            collection.Add(55UL, 9);

            var item = collection.Retrieve(45UL);

            Assert.Equal(8, item);
        }

        [Fact]
        public void SimpleDictionaryCollection_Add_EnsureWhenCollectionEnlargedItemNotLost()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISimpleCollection<int> collection = new SimpleDictionaryCollection<int>(messenger, 1);

            collection.Add(25UL, 6);
            collection.Add(35UL, 7);

            Assert.True(collection.Contains(25UL));
            Assert.True(collection.Contains(35UL));
        }

        [Fact]
        public void SimpleDictionaryCollection_Iterate_EnsureAllItemsIncludedInIteration()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISimpleCollection<int> collection = new SimpleDictionaryCollection<int>(messenger, 1);

            collection.Add(25UL, 6);
            collection.Add(35UL, 7);
            collection.Add(45UL, 8);
            collection.Add(55UL, 9);

            var count = 0;

            foreach (var item in collection.Iterate())
            {
                count += item;
            }

            Assert.Equal(30, count);
        }

        [Fact]
        public void SimpleDictionaryCollection_Iterate_EnsureItemsNotIncludedPostAClear()
        {
            var messenger = Substitute.For<IFrameworkMessenger>();

            ISimpleCollection<int> collection = new SimpleDictionaryCollection<int>(messenger, 1);

            collection.Add(25UL, 6);
            collection.Add(35UL, 7);
            collection.Add(45UL, 8);
            collection.Add(55UL, 9);

            collection.RemoveAll();

            var count = 0;

            foreach (var item in collection.Iterate())
            {
                count += item;
            }

            Assert.Equal(0, count);
        }
    }
}