using NSubstitute;
using System.Numerics;
using Xunit;
using Yak2D.Graphics;
using Yak2D.Internal;

namespace Yak2D.Tests
{
    public class DistortionCollectionTest
    {
        [Fact]
        public void DistortionCollection_InitialisingAdding_CorrectCallsToDraw()
        {
            var collection = new DistortionCollection(4);

            collection.Add(LifeCycle.LoopLinear,
                           CoordinateSpace.Screen,
                           1.0f,
                           new TextureReference(0UL),
                           Vector2.Zero,
                           Vector2.One,
                           Vector2.Zero,
                           Vector2.One,
                           1.0f,
                           0.0f,
                           0.0f,
                           1.0f);

            collection.Add(LifeCycle.Single,
                           CoordinateSpace.Screen,
                           1.0f,
                           new TextureReference(0UL),
                           Vector2.Zero,
                           Vector2.One,
                           Vector2.Zero,
                           Vector2.One,
                           1.0f,
                           0.0f,
                           0.0f,
                           1.0f);

            var drawing = Substitute.For<IDrawing>();
            var stage = Substitute.For<IDistortionStage>();

            collection.Draw(drawing, stage);

            drawing.Received(2).DrawDistortion(stage,
                                              Arg.Any<CoordinateSpace>(),
                                              Arg.Any<FillType>(),
                                              Arg.Any<Vertex2D[]>(),
                                              Arg.Any<int[]>(),
                                              Arg.Any<Colour>(),
                                              Arg.Any<ITexture>(),
                                              Arg.Any<ITexture>(),
                                              Arg.Any<TextureCoordinateMode>(),
                                              Arg.Any<TextureCoordinateMode>(),
                                              Arg.Any<float>());
        }

        [Fact]
        public void DistortionCollection_Updating_CorrectlyRemovedSingleAfterUpdateTimeLapsed()
        {
            var collection = new DistortionCollection(4);

            collection.Add(LifeCycle.LoopLinear,
                           CoordinateSpace.Screen,
                           1.0f,
                           new TextureReference(0UL),
                           Vector2.Zero,
                           Vector2.One,
                           Vector2.Zero,
                           Vector2.One,
                           1.0f,
                           0.0f,
                           0.0f,
                           1.0f);

            collection.Add(LifeCycle.Single,
                           CoordinateSpace.Screen,
                           1.0f,
                           new TextureReference(0UL),
                           Vector2.Zero,
                           Vector2.One,
                           Vector2.Zero,
                           Vector2.One,
                           1.0f,
                           0.0f,
                           0.0f,
                           1.0f);

            var drawing = Substitute.For<IDrawing>();
            var stage = Substitute.For<IDistortionStage>();

            collection.Update(2.0f);

            collection.Draw(drawing, stage);

            drawing.Received(1).DrawDistortion(stage,
                                              Arg.Any<CoordinateSpace>(),
                                              Arg.Any<FillType>(),
                                              Arg.Any<Vertex2D[]>(),
                                              Arg.Any<int[]>(),
                                              Arg.Any<Colour>(),
                                              Arg.Any<ITexture>(),
                                              Arg.Any<ITexture>(),
                                              Arg.Any<TextureCoordinateMode>(),
                                              Arg.Any<TextureCoordinateMode>(),
                                              Arg.Any<float>());
        }

        [Fact]
        public void DistortionCollection_Removal_CorrectlyRemoveAllOnClear()
        {
            var collection = new DistortionCollection(4);

            collection.Add(LifeCycle.LoopLinear,
                           CoordinateSpace.Screen,
                           1.0f,
                           new TextureReference(0UL),
                           Vector2.Zero,
                           Vector2.One,
                           Vector2.Zero,
                           Vector2.One,
                           1.0f,
                           0.0f,
                           0.0f,
                           1.0f);

            collection.Add(LifeCycle.Single,
                           CoordinateSpace.Screen,
                           1.0f,
                           new TextureReference(0UL),
                           Vector2.Zero,
                           Vector2.One,
                           Vector2.Zero,
                           Vector2.One,
                           1.0f,
                           0.0f,
                           0.0f,
                           1.0f);

            var drawing = Substitute.For<IDrawing>();
            var stage = Substitute.For<IDistortionStage>();

            collection.ClearCollection();
            
            collection.Draw(drawing, stage);

            drawing.Received(0).DrawDistortion(stage,
                                              Arg.Any<CoordinateSpace>(),
                                              Arg.Any<FillType>(),
                                              Arg.Any<Vertex2D[]>(),
                                              Arg.Any<int[]>(),
                                              Arg.Any<Colour>(),
                                              Arg.Any<ITexture>(),
                                              Arg.Any<ITexture>(),
                                              Arg.Any<TextureCoordinateMode>(),
                                              Arg.Any<TextureCoordinateMode>(),
                                              Arg.Any<float>());
        }
    }
}
