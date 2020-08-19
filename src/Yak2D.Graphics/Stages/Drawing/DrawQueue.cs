using HPCsharp;
using Yak2D.Utility;

namespace Yak2D.Graphics
{
    public class DrawQueue : IDrawQueue
    {
        public QueueData Data { get; private set; }

        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly IComparerCollection _comparers;
        private readonly bool _skipDepthsAndLayersSort;

        public DrawQueue(IFrameworkMessenger frameworkMessenger,
                         IComparerCollection comparers,
                         int initialRequestQueueSize,
                         int scalarForPerElementArraySizes,
                         bool skipDrawQueueSortingByDepthsAndLayers)
        {
            if(initialRequestQueueSize < 1)
            {
                initialRequestQueueSize = 512;
            }

            if(scalarForPerElementArraySizes < 1)
            {
                scalarForPerElementArraySizes = 8;
            }

            _frameworkMessenger = frameworkMessenger;
            _comparers = comparers;
            _skipDepthsAndLayersSort = skipDrawQueueSortingByDepthsAndLayers;

            var perElementArraySize = initialRequestQueueSize * scalarForPerElementArraySizes;

            InitialiseDataObject(initialRequestQueueSize, perElementArraySize);

            Clear(); //Ensure counters set to 0 (will be by default but let's not rely on that)
        }

        private void InitialiseDataObject(int sizePerRequest, int sizePerElement)
        {
            Data = new QueueData
            {
                QueueSizeSingleProperty = sizePerRequest,
                QueueSizeVertex = sizePerElement,
                QueueSizeIndex = sizePerElement,
                Ordering = new int[sizePerRequest],
                Targets = new CoordinateSpace[sizePerRequest],
                Types = new FillType[sizePerRequest],
                BaseColours = new Colour[sizePerRequest],
                NumVertices = new int[sizePerRequest],
                FirstVertexPosition = new int[sizePerRequest],
                NumIndices = new int[sizePerRequest],
                FirstIndexPosition = new int[sizePerRequest],
                Texture0 = new ulong[sizePerRequest],
                Texture1 = new ulong[sizePerRequest],
                TextureMode0 = new TextureCoordinateMode[sizePerRequest],
                TextureMode1 = new TextureCoordinateMode[sizePerRequest],
                Depths = new float[sizePerRequest],
                Layers = new int[sizePerRequest],
                Vertices = new Vertex2D[sizePerElement],
                Indices = new int[sizePerElement],
            };
        }

        public void Clear()
        {
            Data.NumRequests = 0;
            Data.NumVerticesUsed = 0;
            Data.NumIndicesUsed = 0;
        }

        public void Sort()
        {
            //Array.Sort is unstable, so using HPCsharp library. This is Apache License, not MIT. Reasonably compatible
            //Timsort would be a nice option but it's GPL. I'm not a fan of the GPL

            _comparers.TextureCoordMode.SetItems(Data.TextureMode1);
            Data.Ordering.SortMergeInPlace(0, Data.NumRequests, _comparers.TextureCoordMode);

            _comparers.TextureCoordMode.SetItems(Data.TextureMode0);
            Data.Ordering.SortMergeInPlace(0, Data.NumRequests, _comparers.TextureCoordMode);

            _comparers.DrawType.SetItems(Data.Types);
            Data.Ordering.SortMergeInPlace(0, Data.NumRequests, _comparers.DrawType);

            _comparers.ULong.SetItems(Data.Texture1);
            Data.Ordering.SortMergeInPlace(0, Data.NumRequests, _comparers.ULong);

            _comparers.ULong.SetItems(Data.Texture0);
            Data.Ordering.SortMergeInPlace(0, Data.NumRequests, _comparers.ULong);

            if (_skipDepthsAndLayersSort)
            {
                return;
            }

            _comparers.ReverseFloat.SetItems(Data.Depths);
            Data.Ordering.SortMergeInPlace(0, Data.NumRequests, _comparers.ReverseFloat);

             _comparers.Integer.SetItems(Data.Layers);
            Data.Ordering.SortMergeInPlace(0, Data.NumRequests, _comparers.Integer);
        }

        public bool AddIfValid(ref CoordinateSpace target,
                                ref FillType type,
                                ref Colour colour,
                                ref Vertex2D[] vertices,
                                ref int[] indices,
                                ref ulong texture0,
                                ref ulong texture1,
                                ref TextureCoordinateMode texmode0,
                                ref TextureCoordinateMode texmode1,
                                ref float depth,
                                ref int layer)
        {
            if (vertices.Length < 3)
            {
                _frameworkMessenger.Report("Add draw request failed: Less than 3 vertices provided");
                return false;
            }

            if (indices.Length == 0)
            {
                _frameworkMessenger.Report("Add draw request failed: zero indices provided");
                return false;
            }

            if (indices.Length % 3 != 0)
            {
                _frameworkMessenger.Report(string.Concat("Add draw request failed: Number of indices ", indices.Length, " not divisible by 3"));
                return false;
            }

            if (type == FillType.Coloured)
            {
                texture0 = 0UL;
                texture1 = 0UL;
                texmode0 = TextureCoordinateMode.None;
                texmode1 = TextureCoordinateMode.None;
            }
            else
            {
                if (texture0 == 0UL)
                {
                    _frameworkMessenger.Report("Add draw request failed: No Texture0 provided for Textured Drawing");
                    return false;
                }
            }

            if (type == FillType.DualTextured)
            {
                if (texture1 == 0UL)
                {
                    _frameworkMessenger.Report("Add draw request failed: No Texture1 provided for Dual Textured Drawing");
                    return false;
                }
                if (texture0 == texture1)
                {
                    _frameworkMessenger.Report("Add draw request failed: Same texture provided for both textures in Dual Textured drawing. Not supported");
                    return false;
                }

                if (texmode0 == TextureCoordinateMode.None)
                {
                    texmode0 = TextureCoordinateMode.Wrap;
                }

                if (texmode1 == TextureCoordinateMode.None)
                {
                    texmode1 = TextureCoordinateMode.Wrap;
                }
            }

            if (type == FillType.Textured)
            {
                if (texmode0 == TextureCoordinateMode.None)
                {
                    texmode0 = TextureCoordinateMode.Wrap;
                }

                texture1 = 0UL;
                texmode1 = TextureCoordinateMode.None;
            }

            if (depth < 0.0f || depth > 1.0f)
            {
                _frameworkMessenger.Report("Add draw request failed: Depth out of range 0 - 1");
                return false;
            }

            if (layer < 0)
            {
                _frameworkMessenger.Report("Add draw request failed: a negative layer number is not valid");
                return false;
            };

            Add(ref target, ref type, ref colour, ref vertices, ref indices, ref texture0, ref texture1, ref texmode0, ref texmode1, ref depth, ref layer);

            return true;
        }

        public void Add(ref CoordinateSpace target,
                        ref FillType type,
                        ref Colour colour,
                        ref Vertex2D[] vertices,
                        ref int[] indices,
                        ref ulong texture0,
                        ref ulong texture1,
                        ref TextureCoordinateMode texmode0,
                        ref TextureCoordinateMode texmode1,
                        ref float depth,
                        ref int layer)
        {
            var numVertices = vertices.Length;
            var numIndices = indices.Length;

            CheckAndUpsizeSinglePropertyArrays();
            CheckAndUpsizeVertexArray(numVertices);
            CheckAndUpsizeIndexArray(numIndices);

            Data.Ordering[Data.NumRequests] = Data.NumRequests;
            Data.Targets[Data.NumRequests] = target;
            Data.Types[Data.NumRequests] = type;
            Data.BaseColours[Data.NumRequests] = colour;
            Data.NumVertices[Data.NumRequests] = numVertices;
            Data.FirstVertexPosition[Data.NumRequests] = Data.NumVerticesUsed;
            Data.NumIndices[Data.NumRequests] = numIndices;
            Data.FirstIndexPosition[Data.NumRequests] = Data.NumIndicesUsed;
            Data.Texture0[Data.NumRequests] = texture0;
            Data.Texture1[Data.NumRequests] = texture1;
            Data.TextureMode0[Data.NumRequests] = texmode0;
            Data.TextureMode1[Data.NumRequests] = texmode1;
            Data.Depths[Data.NumRequests] = depth;
            Data.Layers[Data.NumRequests] = layer;

            for (var v = 0; v < numVertices; v++)
            {
                Data.Vertices[Data.NumVerticesUsed + v] = vertices[v];
            }

            for (var i = 0; i < numIndices; i++)
            {
                Data.Indices[Data.NumIndicesUsed + i] = indices[i];
            }

            Data.NumRequests++;
            Data.NumVerticesUsed += numVertices;
            Data.NumIndicesUsed += numIndices;
        }

        private void CheckAndUpsizeSinglePropertyArrays()
        {
            if (Data.NumRequests + 1 >= Data.QueueSizeSingleProperty)
            {
                DoubleSinglePropertyArrays();
                Data.QueueSizeSingleProperty *= 2;
            }
        }

        private void DoubleSinglePropertyArrays()
        {
            Data.Ordering = ArrayFunctions.DoubleArraySizeAndKeepContents<int>(Data.Ordering);
            Data.Targets = ArrayFunctions.DoubleArraySizeAndKeepContents<CoordinateSpace>(Data.Targets);
            Data.Types = ArrayFunctions.DoubleArraySizeAndKeepContents<FillType>(Data.Types);
            Data.BaseColours = ArrayFunctions.DoubleArraySizeAndKeepContents<Colour>(Data.BaseColours);
            Data.NumVertices = ArrayFunctions.DoubleArraySizeAndKeepContents<int>(Data.NumVertices);
            Data.FirstVertexPosition = ArrayFunctions.DoubleArraySizeAndKeepContents<int>(Data.FirstVertexPosition);
            Data.NumIndices = ArrayFunctions.DoubleArraySizeAndKeepContents<int>(Data.NumIndices);
            Data.FirstIndexPosition = ArrayFunctions.DoubleArraySizeAndKeepContents<int>(Data.FirstIndexPosition);
            Data.Texture0 = ArrayFunctions.DoubleArraySizeAndKeepContents<ulong>(Data.Texture0);
            Data.Texture1 = ArrayFunctions.DoubleArraySizeAndKeepContents<ulong>(Data.Texture1);
            Data.TextureMode0 = ArrayFunctions.DoubleArraySizeAndKeepContents<TextureCoordinateMode>(Data.TextureMode0);
            Data.TextureMode1 = ArrayFunctions.DoubleArraySizeAndKeepContents<TextureCoordinateMode>(Data.TextureMode1);
            Data.Depths = ArrayFunctions.DoubleArraySizeAndKeepContents<float>(Data.Depths);
            Data.Layers = ArrayFunctions.DoubleArraySizeAndKeepContents<int>(Data.Layers);
        }

        private void CheckAndUpsizeVertexArray(int numToAdd)
        {
            while (Data.NumVerticesUsed + numToAdd >= Data.QueueSizeVertex)
            {
                Data.Vertices = ArrayFunctions.DoubleArraySizeAndKeepContents<Vertex2D>(Data.Vertices);
                Data.QueueSizeVertex *= 2;
            }
        }

        private void CheckAndUpsizeIndexArray(int numToAdd)
        {
            while (Data.NumIndicesUsed + numToAdd >= Data.QueueSizeIndex)
            {
                Data.Indices = ArrayFunctions.DoubleArraySizeAndKeepContents<int>(Data.Indices);
                Data.QueueSizeIndex *= 2;
            }
        }
    }
}