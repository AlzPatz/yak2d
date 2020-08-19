using System;
using System.Numerics;

namespace Yak2D.Graphics
{
    public class DistortionCollection : IDistortionCollection
    {
        private int _collectionSize;
        private DistortionCollectionItem[] _collection;
        private int _numActive;

        public DistortionCollection(uint initialCollectionSize)
        {
            _collectionSize = (int)initialCollectionSize;

            CreateNewCollection(_collectionSize);

            _numActive = 0;
        }

        private void CreateNewCollection(int collectionSize)
        {
            _collection = new DistortionCollectionItem[collectionSize];

            for (var n = 0; n < collectionSize; n++)
            {
                _collection[n] = new DistortionCollectionItem
                {
                    Active = false
                };
            }
        }

        public void Add(LifeCycle cycle,
                                    CoordinateSpace coordinateSpace,
                                    float durationInSeconds,
                                    ITexture texture,
                                    Vector2 initPosition,
                                    Vector2 finalPosition,
                                    Vector2 initSize,
                                    Vector2 finalSize,
                                    float initIntensity,
                                    float finalIntensity,
                                    float initialRotation,
                                    float finalRotation)
        {
            if (_numActive == _collectionSize)
            {
                DoubleCollectionSize();
            }

            var freeIndex = FindFirstFreeIndex();

            var item = _collection[freeIndex];

            item.Active = true;
            item.Cycle = cycle;
            item.CoordinateSpace = coordinateSpace;
            item.Duration = durationInSeconds;
            item.Texture = texture;
            item.InitPosition = initPosition;
            item.FinalPosition = finalPosition;
            item.InitSize = initSize;
            item.FinalSize = finalSize;
            item.InitIntensity = initIntensity;
            item.FinalIntensity = finalIntensity;
            item.InitRotation = initialRotation;
            item.FinalRotation = finalRotation;

            item.TimeCount = 0.0f;
            item.CurrentCountDirectionIsForward = true;

            _numActive++;
        }

        private void DoubleCollectionSize()
        {
            var oldCollection = _collection;

            var newCollectionSize = 2 * _collectionSize;

            CreateNewCollection(newCollectionSize);

            for (var n = 0; n < _collectionSize; n++)
            {
                _collection[n].Active = oldCollection[n].Active;
                _collection[n].Cycle = oldCollection[n].Cycle;
                _collection[n].CoordinateSpace = oldCollection[n].CoordinateSpace;
                _collection[n].Duration = oldCollection[n].Duration;
                _collection[n].Texture = oldCollection[n].Texture;
                _collection[n].InitPosition = oldCollection[n].InitPosition;
                _collection[n].FinalPosition = oldCollection[n].FinalPosition;
                _collection[n].InitSize = oldCollection[n].InitSize;
                _collection[n].FinalSize = oldCollection[n].FinalSize;
                _collection[n].InitIntensity = oldCollection[n].InitIntensity;
                _collection[n].FinalIntensity = oldCollection[n].FinalIntensity;
                _collection[n].InitRotation = oldCollection[n].InitRotation;
                _collection[n].FinalRotation = oldCollection[n].FinalRotation;
                _collection[n].TimeCount = oldCollection[n].TimeCount;
                _collection[n].CurrentCountDirectionIsForward = oldCollection[n].CurrentCountDirectionIsForward;
            }

            _collectionSize = newCollectionSize;
        }

        private int FindFirstFreeIndex()
        {
            for (var n = 0; n < _collectionSize; n++)
            {
                if (_collection[n].Active == false)
                {
                    return n;
                }
            }
            return 0; //Shouldn't hit this. perhaps make it robust on its own. relying on caller. doesnt matter. 0 wont break (-1 would)
        }

        public void ClearCollection()
        {
            for (var n = 0; n < _collectionSize; n++)
            {
                _collection[n].Active = false;
            }
            _numActive = 0;
        }

        public void Update(float timeStep)
        {
            var numActiveAtStart = _numActive;
            var countNumberProcessed = 0;
            for (var n = 0; n < _collectionSize; n++)
            {
                if (_collection[n].Active)
                {
                    var item = _collection[n];

                    item.TimeCount += timeStep;

                    if (item.TimeCount > item.Duration)
                    {
                        switch (item.Cycle)
                        {
                            case LifeCycle.Single:
                                item.Active = false;
                                _numActive--;
                                break;
                            case LifeCycle.LoopLinear:
                                while (item.TimeCount > item.Duration)
                                {
                                    item.TimeCount -= item.Duration;
                                }
                                break;
                            case LifeCycle.LoopReverse:
                                while (item.TimeCount > item.Duration)
                                {
                                    item.TimeCount -= item.Duration;
                                }
                                item.CurrentCountDirectionIsForward = !item.CurrentCountDirectionIsForward;
                                break;
                        }
                    }

                    countNumberProcessed++;
                    if (countNumberProcessed == numActiveAtStart)
                    {
                        break;
                    }
                }
            }
        }

        public void Draw(IDrawing drawing, IDistortionStage stage)
        {
            if (_numActive == 0)
            {
                return;
            }

            var quadIndices = new int[] { 0, 1, 2, 2, 1, 3 };

            for (var n = 0; n < _collectionSize; n++)
            {
                if (_collection[n].Active)
                {
                    var item = _collection[n];

                    var fraction = item.TimeCount / item.Duration;
                    if (!item.CurrentCountDirectionIsForward)
                    {
                        fraction = 1.0f - fraction;
                    }

                    var halfSize = 0.5f * Utility.Interpolator.Interpolate(item.InitSize, item.FinalSize, ref fraction);
                    var position = Utility.Interpolator.Interpolate(item.InitPosition, item.FinalPosition, ref fraction);
                    var intensity = Utility.Interpolator.Interpolate(item.InitIntensity, item.FinalIntensity, ref fraction);
                    var rotation = Utility.Interpolator.Interpolate(item.InitRotation, item.FinalRotation, ref fraction);

                    drawing.DrawDistortion(stage, item.CoordinateSpace,
                                                                FillType.Textured,
                                                                new Vertex2D[]
                                                                {
                                                                    new Vertex2D { Position = RotateDegressClockwise(new Vector2(-halfSize.X, halfSize.Y), rotation)+ position, TexCoord0 = new Vector2(0.0f, 0.0f), TexCoord1 = new Vector2(0.0f, 0.0f), TexWeighting = 0.0f, Colour = new Colour(1.0f, 1.0f, 1.0f, 1.0f) },
                                                                    new Vertex2D { Position = RotateDegressClockwise(new Vector2(halfSize.X, halfSize.Y),  rotation)+ position, TexCoord0 = new Vector2(1.0f, 0.0f), TexCoord1 = new Vector2(1.0f, 0.0f), TexWeighting = 1.0f, Colour = new Colour(1.0f, 1.0f, 1.0f, 1.0f) },
                                                                    new Vertex2D { Position = RotateDegressClockwise(new Vector2(-halfSize.X, -halfSize.Y) , rotation)+ position, TexCoord0 = new Vector2(0.0f, 1.0f), TexCoord1 = new Vector2(0.0f, 1.0f), TexWeighting = 0.0f, Colour = new Colour(1.0f, 1.0f, 1.0f, 1.0f) },
                                                                    new Vertex2D { Position = RotateDegressClockwise(new Vector2(halfSize.X, -halfSize.Y), rotation)+ position, TexCoord0 = new Vector2(1.0f, 1.0f), TexCoord1 = new Vector2(1.0f, 1.0f), TexWeighting = 1.0f, Colour = new Colour(1.0f, 1.0f, 1.0f, 1.0f) },

                                                                },
                                                                quadIndices,
                                                                Colour.White,
                                                                item.Texture,
                                                                null,
                                                                TextureCoordinateMode.Wrap,
                                                                TextureCoordinateMode.Wrap,
                                                                intensity
                                                                );
                }
            }
        }

        private Vector2 RotateDegressClockwise(Vector2 v, double degrees)
        {
            var radians = degrees * Math.PI / 180.0f;
            var ca = (float)Math.Cos(-radians);
            var sa = (float)Math.Sin(-radians);
            return new Vector2(ca * v.X - sa * v.Y, sa * v.X + ca * v.Y);
        }
    }
}