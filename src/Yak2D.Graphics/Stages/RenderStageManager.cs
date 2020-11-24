using System;
using System.Collections.Generic;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class RenderStageManager : IRenderStageManager
    {
        public int StageCount { get { return _renderStageCollection.Count; } }

        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly IIdGenerator _idGenerator;
        private readonly ISystemComponents _systemComponents;
        private readonly IRenderStageModelFactory _renderStageModelFactory;

        private ISimpleCollection<IRenderStageModel> _renderStageCollection;

        private List<ulong> _drawStagesToAutoClearDynamicQueues;

        public RenderStageManager(IFrameworkMessenger frameworkMessenger,
                                    IIdGenerator idGenerator,
                                    ISystemComponents veldridComponents,
                                    IRenderStageModelFactory renderStageModelFactory,
                                    ISimpleCollectionFactory collectionFactory)
        {
            _frameworkMessenger = frameworkMessenger;
            _idGenerator = idGenerator;
            _systemComponents = veldridComponents;
            _renderStageModelFactory = renderStageModelFactory;

            _renderStageCollection = collectionFactory.Create<IRenderStageModel>(48);

            _drawStagesToAutoClearDynamicQueues = new List<ulong>();
        }

        public IRenderStageModel RetrieveStageModel(ulong id) => _renderStageCollection.Retrieve(id);

        public void ProcessRenderStageUpdates(float timeSinceLastDraw)
        {
            foreach (var stage in _renderStageCollection.Iterate())
            {
                stage.Update(timeSinceLastDraw);
            }
        }

        public void PrepareForDrawing()
        {
            _drawStagesToAutoClearDynamicQueues.ForEach(id =>
            {
                var stage = RetrieveStageModel(id) as IDrawStageModel;

                if (stage != null)
                {
                    stage.ClearDynamicDrawQueue();
                }
            });
        }

        public bool DestroyStage(ulong id)
        {
            var model = _renderStageCollection.Retrieve(id);

            if (model == null)
            {
                _frameworkMessenger.Report("Unable to Destroy a Render Stage as ulong does not exist in collection");
                return false;
            }
            _renderStageCollection.Remove(id);
            model.DestroyResources();

            if (_drawStagesToAutoClearDynamicQueues.Contains(id))
            {
                _drawStagesToAutoClearDynamicQueues.Remove(id);
            }

            return true;
        }

        public void DestroyAllStages(bool haveResourcesAlreadyBeenDisposed)
        {
            if (!haveResourcesAlreadyBeenDisposed)
            {
                foreach (var model in _renderStageCollection.Iterate())
                {
                    model.DestroyResources();
                }
            }
            _renderStageCollection.RemoveAll();
            _drawStagesToAutoClearDynamicQueues.Clear();
        }

        public IDrawStage CreateDrawStage(bool clearDynamicRequestQueueEachFrame, BlendState blendState)
        {
            var id = _idGenerator.New();

            var model = _renderStageModelFactory.CreateDrawStageModel(blendState);

            if (clearDynamicRequestQueueEachFrame)
            {
                _drawStagesToAutoClearDynamicQueues.Add(id);
            }

            var userReference = new DrawStage(id);

            return _renderStageCollection.Add(id, model) ? userReference : null;
        }

        public IColourEffectsStage CreateColourEffectStage()
        {
            var id = _idGenerator.New();

            var model = _renderStageModelFactory.CreateColourEffectStageModel();

            var userReference = new ColourEffectsStage(id);

            return _renderStageCollection.Add(id, model) ? userReference : null;
        }

        public IBloomStage CreateBloomEffectStage(uint sampleSurfaceWidth, uint sampleSurfaceHeight)
        {
            var id = _idGenerator.New();

            var model = _renderStageModelFactory.CreateBloomEffectStageModel(sampleSurfaceWidth, sampleSurfaceHeight);

            var userReference = new BloomStage(id);

            return _renderStageCollection.Add(id, model) ? userReference : null;
        }

        public IBlurStage CreateBlurEffect2DStage(uint sampleSurfaceWidth, uint sampleSurfaceHeight)
        {
            var id = _idGenerator.New();

            var model = _renderStageModelFactory.CreateBlur2DEffectModel(sampleSurfaceWidth, sampleSurfaceHeight);

            var userReference = new BlurStage(id);

            return _renderStageCollection.Add(id, model) ? userReference : null;
        }

        public IBlur1DStage CreateBlurEffect1DStage(uint sampleSurfaceWidth, uint sampleSurfaceHeight)
        {
            var id = _idGenerator.New();

            var model = _renderStageModelFactory.CreateBlur1DEffectModel(sampleSurfaceWidth, sampleSurfaceHeight);

            var userReference = new Blur1DStage(id);

            return _renderStageCollection.Add(id, model) ? userReference : null;
        }

        public IStyleEffectsStage CreateStyleEffectStage()
        {
            var id = _idGenerator.New();

            var model = _renderStageModelFactory.CreateStyleEffectModel();

            var userReference = new StyleEffectsStage(id);

            return _renderStageCollection.Add(id, model) ? userReference : null;
        }

        public IMeshRenderStage CreateMeshRenderStage()
        {
            var id = _idGenerator.New();

            var model = _renderStageModelFactory.CreateMeshRenderModel();

            var userReference = new MeshRenderStage(id);

            return _renderStageCollection.Add(id, model) ? userReference : null;
        }

        public IDistortionStage CreateDistortionEffectStage(bool clearDynamicRequestQueueEachFrame, uint internalSurfaceWidth, uint internalSurfaceHeight)
        {
            var id = _idGenerator.New();

            var model = _renderStageModelFactory.CreateDistortionEffectStageModel(internalSurfaceWidth, internalSurfaceHeight);

            if (clearDynamicRequestQueueEachFrame)
            {
                _drawStagesToAutoClearDynamicQueues.Add(id);
            }

            var userReference = new DistortionStage(id);

            return _renderStageCollection.Add(id, model) ? userReference : null;
        }

        public IMixStage CreateMixStage()
        {
            var id = _idGenerator.New();

            var model = _renderStageModelFactory.CreateMixStageModel();

            var userReference = new MixStage(id);

            return _renderStageCollection.Add(id, model) ? userReference : null;
        }

        public ICustomShaderStage CreateCustomShaderStage(string fragmentShaderFilename,
                                                          AssetSourceEnum assetType,
                                                          ShaderUniformDescription[] uniformDescriptions,
                                                          BlendState blendState,
                                                          bool useSpirvCompile)
        {
            var id = _idGenerator.New();

            var model = _renderStageModelFactory.CreateCustomStageModel(fragmentShaderFilename, assetType, uniformDescriptions, blendState, useSpirvCompile);

            var userReference = new CustomShaderStage(id);

            return _renderStageCollection.Add(id, model) ? userReference : null;
        }

        public ICustomVeldridStage CreateCustomVeldridStage(CustomVeldridBase stage)
        {
            var id = _idGenerator.New();

            var model = _renderStageModelFactory.CreateCustomVeldridStage(stage);

            var userReference = new CustomVeldridStage(id);

            return _renderStageCollection.Add(id, model) ? userReference : null;
        }

        public ISurfaceCopyStage CreateSurfaceCopyDataStage(uint stagingTextureWidth,
                                                            uint stagingTextureHeight,
                                                            Action<TextureData> callback,
                                                            bool useFloat32PixelFormat)
        {
            var id = _idGenerator.New();

            var model = _renderStageModelFactory.CreateSurfaceCopyDataStage(stagingTextureWidth,
                                                                            stagingTextureHeight,
                                                                            callback,
                                                                            useFloat32PixelFormat);

            var userReference = new SurfaceCopyStage(id);

            return _renderStageCollection.Add(id, model) ? userReference : null;
        }

        public void Shutdown()
        {
            DestroyAllStages(false);
        }

        public void ReInitialise()
        {
            //Duped with same name.. not really needed
            DestroyAllStages(true);
        }
    }
}