namespace Yak2D.Utility
{
    public static class ArrayFunctions
    {
        public static T[] DoubleArraySizeAndKeepContents<T>(T[] array)
        {
            if(array == null)
            {
                return null;
            }

            var oldSize = array.Length;
            var newSize = 2 * oldSize;

            var doubled = new T[newSize];

            for (var n = 0; n < oldSize; n++)
            {
                doubled[n] = array[n];
            }

            return doubled;
        }
    }
}