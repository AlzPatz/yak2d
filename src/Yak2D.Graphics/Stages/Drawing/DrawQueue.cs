using System;
using System.Collections.Generic;
using Yak2D.Utility;

namespace Yak2D.Graphics
{
    public class DrawQueue : IDrawQueue
    {
        public QueueData Data { get; private set; }

        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly bool _skipDepthsAndLayersSort;

        public DrawQueue(IFrameworkMessenger frameworkMessenger,
                         int initialRequestQueueSize,
                         int scalarForPerElementArraySizes,
                         bool skipDrawQueueSortingByDepthsAndLayers)
        {
            if (initialRequestQueueSize < 1)
            {
                initialRequestQueueSize = 512;
            }

            if (scalarForPerElementArraySizes < 1)
            {
                scalarForPerElementArraySizes = 8;
            }

            _frameworkMessenger = frameworkMessenger;
            _skipDepthsAndLayersSort = skipDrawQueueSortingByDepthsAndLayers;

            var perElementArraySize = initialRequestQueueSize * scalarForPerElementArraySizes;

            InitialiseDataObject(initialRequestQueueSize, perElementArraySize);

            Clear();
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
            var numRequests = Data.NumRequests;

            if (numRequests <= 1)
                return;

            // Capture local array references for comparer closure consistency
            // and to avoid repeated property lookups during sort
            var layers = Data.Layers;
            var depths = Data.Depths;
            var texture0 = Data.Texture0;
            var texture1 = Data.Texture1;
            var types = Data.Types;
            var texMode0 = Data.TextureMode0;
            var texMode1 = Data.TextureMode1;
            var skipDepthAndLayerSort = _skipDepthsAndLayersSort;

            // Array.Sort is stable from .NET 7 onwards - no need for HPCSharp
            // Data.Ordering holds draw request indices [0..n-1], sorted here into
            // the order downstream rendering should consume them
            //
            // Priority (high to low):
            //   Layer (asc), Depth (desc), Texture0, Texture1, DrawType, TexMode0, TexMode1
            Array.Sort(Data.Ordering, 0, numRequests, Comparer<int>.Create((left, right) =>
            {
                int result;

                if (!skipDepthAndLayerSort)
                {
                    result = layers[left].CompareTo(layers[right]);
                    if (result != 0)
                        return result;

                    // Depth sorted descending (back to front), so right.CompareTo(left)
                    result = depths[right].CompareTo(depths[left]);
                    if (result != 0)
                        return result;
                }

                result = texture0[left].CompareTo(texture0[right]);
                if (result != 0)
                    return result;

                result = texture1[left].CompareTo(texture1[right]);
                if (result != 0)
                    return result;

                result = ((int)types[left]).CompareTo((int)types[right]);
                if (result != 0)
                    return result;

                result = ((int)texMode0[left]).CompareTo((int)texMode0[right]);
                if (result != 0)
                    return result;

                return ((int)texMode1[left]).CompareTo((int)texMode1[right]);
            }));
        }

        public bool AddIfValid(ref CoordinateSpace target,
                                ref FillType type,
                                ref Colour colour,
                                ref Vertex2D[] vertices,
                                ref int[] indices,
                                ref ulong texture0,
                                ref ulong texture1,
                                ref TextureCoordinateMode texMode0,
                                ref TextureCoordinateMode texMode1,
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
                texMode0 = TextureCoordinateMode.None;
                texMode1 = TextureCoordinateMode.None;
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

                if (texMode0 == TextureCoordinateMode.None)
                    texMode0 = TextureCoordinateMode.Wrap;

                if (texMode1 == TextureCoordinateMode.None)
                    texMode1 = TextureCoordinateMode.Wrap;
            }

            if (type == FillType.Textured)
            {
                if (texMode0 == TextureCoordinateMode.None)
                    texMode0 = TextureCoordinateMode.Wrap;

                texture1 = 0UL;
                texMode1 = TextureCoordinateMode.None;
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
            }

            Add(ref target, ref type, ref colour, ref vertices, ref indices,
                ref texture0, ref texture1, ref texMode0, ref texMode1, ref depth, ref layer);

            return true;
        }

        public void Add(ref CoordinateSpace target,
                        ref FillType type,
                        ref Colour colour,
                        ref Vertex2D[] vertices,
                        ref int[] indices,
                        ref ulong texture0,
                        ref ulong texture1,
                        ref TextureCoordinateMode texMode0,
                        ref TextureCoordinateMode texMode1,
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
            Data.TextureMode0[Data.NumRequests] = texMode0;
            Data.TextureMode1[Data.NumRequests] = texMode1;
            Data.Depths[Data.NumRequests] = depth;
            Data.Layers[Data.NumRequests] = layer;

            for (var v = 0; v < numVertices; v++)
                Data.Vertices[Data.NumVerticesUsed + v] = vertices[v];

            for (var i = 0; i < numIndices; i++)
                Data.Indices[Data.NumIndicesUsed + i] = indices[i];

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