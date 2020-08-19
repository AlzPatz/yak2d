using System;

namespace Yak2D.Graphics
{
    public class GaussianBlurWeightsAndOffsetsCache : IGaussianBlurWeightsAndOffsetsCache
    {
        private const int SAMPLE_ARRAY_SIZE = 8;

        public int WeightsAndOffsetsArraySize { get { return SAMPLE_ARRAY_SIZE; } }
        public int MaxNumberOfSamplesToRequestAwayFromCentre { get { return SAMPLE_ARRAY_SIZE - 1; } }

        private GaussianBlurArrayComponent[][] _weightsAndOffsets;
        private int[][] _pascalsTriangle;

        public GaussianBlurWeightsAndOffsetsCache()
        {
            AssignPascalsTriangle();
            GenerateWeightsAndOffsets();
        }

        private void AssignPascalsTriangle()
        {
            _pascalsTriangle = new int[][]
            {
                new int[] {1},
                new int[] {1,1},
                new int[] {1,2,1},
                new int[] {1,3 ,        3 ,        1},
                new int[] {1,4 ,        6 ,        4,         1},
                new int[] {1,5 ,       10 ,       10,         5,         1},
                new int[] {1,6 ,       15 ,       20,        15,         6,         1},
                new int[] {1,7 ,       21 ,       35,        35,        21,         7,         1},
                new int[] {1,8 ,       28 ,       56,        70,        56,        28,         8 ,        1},
                new int[] {1,9 ,       36 ,       84,        126,       126,       84,       36,         9,         1},
                new int[] {1,10,        45,        120,       210,       252,       210,       120,       45,        10,         1},
                new int[] {1,11,        55,        165,       330,       462,       462,       330,       165,       55,        11,         1},
                new int[] {1,12,        66,        220,       495,       792,       924,       792,       495,       220,       66,        12 ,        1},
                new int[] {1,13,        78,        286,       715,      1287,      1716,      1716 ,     1287,       715 ,      286,       78,        13 ,        1},
                new int[] {1,14,        91,        364,      1001,      2002,      3003,      3432 ,     3003,      2002 ,     1001,       364,       91,        14,         1},
                new int[] {1,15,        105,       455,      1365,      3003,      5005,      6435 ,     6435,      5005 ,     3003,      1365,       455,       105,       15,         1},
                new int[] {1,16,        120,       560,      1820,      4368,      8008,      11440,     12870,     11440,     8008,      4368,      1820,       560,      120 ,      16 ,        1},
                new int[] {1,17,        136,       680,      2380,      6188,      12376,     19448,     24310,     24310,     19448,     12376,     6188,      2380,       680,       136,       17,         1},
                new int[] {1,18,        153,       816,      3060,      8568,      18564,     31824,     43758,     48620,     43758,     31824,     18564,     8568,      3060,       816,       153,       18 ,        1},
                new int[] {1,19,        171,       969,      3876,      11628,     27132,     50388,     75582,     92378,     92378,     75582,     50388,     27132,     11628,     3876,       969 ,      171,       19,         1},
                new int[] {1,20,        190,      1140,      4845,      15504,     38760,     77520,    125970,    167960,    184756 ,   167960,    125970,     77520,     38760 ,    15504,     4845 ,     1140,       190,       20,         1},
                new int[] {1,21,        210,      1330,      5985,      20349,     54264,    116280,    203490,    293930,    352716 ,   352716,    293930,    203490,    116280,     54264,     20349,     5985,      1330,       210,       21,         1},
                new int[] {1,22,        231,      1540,      7315,      26334,     74613,    170544,    319770,    497420,    646646 ,   705432,    646646,    497420,    319770,    170544,     74613,     26334,     7315,      1540,       231,       22 ,        1},
                new int[] {1,23,        253,      1771,      8855,      33649,    100947,    245157,    490314,    817190,    1144066,   1352078,   1352078,   1144066,   817190 ,   490314,    245157 ,   100947,     33649,     8855 ,     1771 ,      253,       23 ,        1},
                new int[] {1,24,        276,      2024,      10626,     42504,    134596,    346104,    735471,    1307504,   1961256,   2496144,   2704156,   2496144,   1961256,   1307504,   735471,    346104,    134596,     42504,     10626,     2024,       276,       24,         1},
                new int[] {1,25,        300,      2300,      12650,     53130,    177100,    480700,    1081575,   2042975,   3268760,   4457400,   5200300,   5200300,   4457400,   3268760,   2042975,   1081575,   480700,    177100,     53130,     12650,     2300,       300,       25,         1},
                new int[] {1,26,        325,      2600,      14950,     65780,    230230,    657800,    1562275,   3124550,   5311735,   7726160,   9657700,  10400600,   9657700,   7726160,   5311735,   3124550,   1562275,   657800,    230230,     65780,     14950,     2600,       325,       26 ,        1},
                new int[] {1,27,        351,      2925,      17550,     80730,    296010,    888030,    2220075,   4686825,   8436285,  13037895,  17383860,  20058300,  20058300,  17383860,  13037895 ,  8436285,   4686825,   2220075,   888030 ,   296010,     80730,     17550,     2925,       351,       27,         1},
                new int[] {1,28,        378,      3276,      20475,     98280,    376740,    1184040,   3108105,   6906900,  13123110,  21474180,  30421755,  37442160,  40116600,  37442160,  30421755 , 21474180,  13123110,   6906900 ,  3108105,   1184040,   376740,     98280,     20475,     3276,       378,       28,         1},
                new int[] {1,29,        406,      3654,      23751,    118755,    475020,    1560780,   4292145,  10015005,  20030010,  34597290,  51895935,  67863915,  77558760,  77558760,  67863915 , 51895935,  34597290,  20030010 , 10015005,   4292145,   1560780,   475020,    118755,     23751,     3654,       406,       29,         1},
                new int[] {1,30,        435,      4060,      27405,    142506,    593775,    2035800,   5852925,  14307150,  30045015,  54627300,  86493225,  119759850, 145422675, 155117520, 145422675, 119759850, 86493225,  54627300 , 30045015,  14307150,   5852925,   2035800,   593775,    142506,     27405,     4060,       435,       30,         1},
                new int[] {1,31,        465,      4495,      31465,    169911,    736281,    2629575,   7888725,  20160075,  44352165,  84672315,  141120525, 206253075, 265182525, 300540195, 300540195, 265182525, 206253075, 141120525, 84672315,  44352165,  20160075,   7888725,   2629575,   736281,    169911,     31465,     4495,       465,       31,         1 },
                new int[] {1,32,        496,      4960,      35960,    201376,    906192,    3365856,  10518300,  28048800,  64512240,  129024480, 225792840, 347373600, 471435600, 565722720, 601080390, 565722720, 471435600, 347373600, 225792840, 129024480, 64512240,  28048800,  10518300,   3365856,   906192,    201376,     35960,     4960,       496,       32,         1}
            };
        }

        private void GenerateWeightsAndOffsets()
        {
            _weightsAndOffsets = new GaussianBlurArrayComponent[SAMPLE_ARRAY_SIZE][];

            for (var n = 0; n < SAMPLE_ARRAY_SIZE; n++)
            {
                _weightsAndOffsets[n] = GenerateForASampleSize(n);
            }
        }

        private GaussianBlurArrayComponent[] GenerateForASampleSize(int num)
        {
            //In the filter we take advantage of 'free' gpu linear filtering optimiser, to create same effect of a larger sample set.
            //We need to calculate the weights and offsets of the larger set first
            var numSamplesOnOneSideIncludingCentre = num + 1;

            var expandSampleSize = ((numSamplesOnOneSideIncludingCentre - 1) * 2) + 1;
            var expandedWeightsAndOffsets = CalculateWeightsAndOffsetsOfLargerEquivalentSet(expandSampleSize);

            var weightsAndOffsets = new GaussianBlurArrayComponent[SAMPLE_ARRAY_SIZE];

            weightsAndOffsets[0] = expandedWeightsAndOffsets[0];

            for (var n = 1; n < numSamplesOnOneSideIncludingCentre; n++)
            {
                var expandedIndex0 = 1 + ((n - 1) * 2);
                var expandedIndex1 = expandedIndex0 + 1;

                weightsAndOffsets[n].Weight = expandedWeightsAndOffsets[expandedIndex0].Weight + expandedWeightsAndOffsets[expandedIndex1].Weight;
                weightsAndOffsets[n].Offset = ((expandedWeightsAndOffsets[expandedIndex0].Weight * expandedWeightsAndOffsets[expandedIndex0].Offset) +
                                            (expandedWeightsAndOffsets[expandedIndex1].Weight * expandedWeightsAndOffsets[expandedIndex1].Offset)) / weightsAndOffsets[n].Weight;
            }

            return weightsAndOffsets;
        }

        private GaussianBlurArrayComponent[] CalculateWeightsAndOffsetsOfLargerEquivalentSet(int numSamples)
        {
            var tapNumber = (2 * (numSamples - 1)) + 1;

            var PascalTriangleRowIndexToUse = tapNumber + 3;

            //We use the row minus the two edge numbers on each side
            var PascalsTriangleRowSum = (float)Math.Pow(2.0f, PascalTriangleRowIndexToUse);

            var firstValue = 1;
            var secondValue = CalculateValueInPascalTriangle(PascalTriangleRowIndexToUse, 1);

            var adjustedRowSum = PascalsTriangleRowSum - (2 * (firstValue + secondValue));

            var halfRow = new float[numSamples];

            for (var n = 0; n < numSamples; n++)
            {
                var index = n + 2; // we skip the first two values in the pascals triangle
                halfRow[n] = CalculateValueInPascalTriangle(PascalTriangleRowIndexToUse, index);
            }

            var weightsAndOffsets = new GaussianBlurArrayComponent[numSamples];

            for (var n = 0; n < numSamples; n++)
            {
                weightsAndOffsets[n].Offset = n;
                var reverse = numSamples - n - 1;
                weightsAndOffsets[reverse].Weight = (1.0f * halfRow[n]) / (1.0f * adjustedRowSum);
            }

            return weightsAndOffsets;
        }

        private int CalculateValueInPascalTriangle(int row, int index)
        {
            return _pascalsTriangle[row][index];
        }

        public GaussianBlurArrayComponent[] ReturnWeightsAndOffsets(int numSamplesTakenFromCentre)
        {
            if (numSamplesTakenFromCentre < 0)
            {
                numSamplesTakenFromCentre = 0;
            }

            if (numSamplesTakenFromCentre > MaxNumberOfSamplesToRequestAwayFromCentre)
            {
                numSamplesTakenFromCentre = MaxNumberOfSamplesToRequestAwayFromCentre;
            }

            return _weightsAndOffsets[numSamplesTakenFromCentre];
        }
    }
}