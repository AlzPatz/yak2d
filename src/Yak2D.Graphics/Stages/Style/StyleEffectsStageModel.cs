using System;
using System.Numerics;
using Veldrid;
using Yak2D.Internal;

namespace Yak2D.Graphics
{
    public class StyleEffectsStageModel : IStyleEffectsStageModel
    {
        public void SendToRenderStage(IRenderStageVisitor visitor, CommandList cl, RenderCommandQueueItem command) => visitor.DispatchToRenderStage(this, cl, command);
        public void CacheInstanceInVisitor(IRenderStageVisitor visitor) => visitor.CacheStageModel(this);

        public ResourceSet PixellateResourceSet { get; private set; }
        public ResourceSet EdgeDetectionResourceSet { get; private set; }
        public ResourceSet StaticResourceSet { get; private set; }
        public ResourceSet OldMovieResourceSet { get; private set; }
        public ResourceSet CrtEffectResourceSet { get; private set; }

        private readonly IFrameworkMessenger _frameworkMessenger;
        private readonly ISystemComponents _systemComponents;

        public DeviceBuffer PixellateBuffer { get; private set; }
        private DeviceBuffer _edgeDetectionBuffer;
        private DeviceBuffer _staticBuffer;
        private DeviceBuffer _oldMovieBuffer;
        private DeviceBuffer _crtEffectBuffer;

        public PixellateConfiguration PixellateCurrent { get { return _pixellateCurrent; } }
        private PixellateConfiguration _pixellateCurrent;
        private PixellateConfiguration _pixellatePrevious;
        private PixellateConfiguration _pixellateTarget;
        private bool _pixellateTransitioning;
        private float _pixellateTransitionTotalTime;
        private float _pixellateTransitionTimeCount;

        private EdgeDetectionConfiguration _edgeDetectionCurrent;
        private EdgeDetectionConfiguration _edgeDetectionPrevious;
        private EdgeDetectionConfiguration _edgeDetectionTarget;
        private bool _edgeDetectionTransitioning;
        private float _edgeDetectionTransitionTotalTime;
        private float _edgeDetectionTransitionTimeCount;

        private StaticConfiguration _staticCurrent;
        private StaticConfiguration _staticPrevious;
        private StaticConfiguration _staticTarget;
        private bool _staticTransitioning;
        private float _staticTransitionTotalTime;
        private float _staticTransitionTimeCount;

        private OldMovieConfiguration _oldMovieCurrent;
        private OldMovieConfiguration _oldMoviePrevious;
        private OldMovieConfiguration _oldMovieTarget;
        private bool _oldMovieTransitioning;
        private float _oldMovieTransitionTotalTime;
        private float _oldMovieTransitionTimeCount;

        private CrtEffectConfiguration _crtEffectCurrent;
        private CrtEffectConfiguration _crtEffectPrevious;
        private CrtEffectConfiguration _crtEffectTarget;
        private bool _crtEffectTransitioning;
        private float _crtEffectTransitionTotalTime;
        private float _crtEffectTransitionTimeCount;

        private OldMovieEffectStateProperties _oldMovieProperties;

        private Random _random;
        private float _staticTime;

        public StyleEffectsStageModel(IFrameworkMessenger frameworkMessenger, ISystemComponents systemComponents)
        {
            _frameworkMessenger = frameworkMessenger;
            _systemComponents = systemComponents;

            _random = new Random();
            _staticTime = 0.0f;

            CreateBuffers();

            CreateDefaultConfigurationAndFillBuffers();
            SetMovieStatePropertiesToDefaultValues();

            SetAllTransitionsToFalse(); //Not needed as above sets all time for transitions to 0 and that causes falses on all transitions
        }

        private void SetAllTransitionsToFalse()
        {
            _pixellateTransitioning = false;
            _edgeDetectionTransitioning = false;
            _staticTransitioning = false;
            _oldMovieTransitioning = false;
            _crtEffectTransitioning = false;
        }

        private void CreateDefaultConfigurationAndFillBuffers()
        {
            var initialConfig = new StyleEffectGroupConfiguration()
            {
                Pixellate = new PixellateConfiguration
                {
                    Intensity = 1.0f,
                    NumXDivisions = 120,
                    NumYDivisions = 67,
                },
                EdgeDetection = new EdgeDetectionConfiguration
                {
                    Intensity = 0.0f,
                    IsFreichen = false
                },
                Static = new StaticConfiguration
                {
                    Intensity = 0.0f,
                    TimeSpeed = 1.0f,
                    IgnoreTransparent = 1,
                    TexelScaler = 1.0f
                },
                OldMovie = new OldMovieConfiguration
                {
                    Intensity = 0.0f,
                    Scratch = 0.01f,
                    Noise = 0.009f,
                    RndShiftCutOff = 0.6f,
                    RndShiftScalar = 0.3f,
                    Dim = 0.3f,
                    ProbabilityRollStarts = 0.005f,
                    ProbabilityRollEnds = 0.04f,
                    RollSpeedMin = 0.00032f,
                    RollSpeedMax = 0.00012f,
                    RollAccelerationMin = 0.0008f,
                    RollAccelerationMax = 0.0012f,
                    RollShakeFactor = 0.003f,
                    RollOverallScale = 1.0f,
                    OverExposureProbabilityStart = 0.01f,
                    OverExposureFlickerTimeMin = 20,
                    OverExposureFlickerTimeMax = 45,
                    OverExposureIntensityMin = 1.2f,
                    OverExposureIntensityMax = 1.8f,
                    OverExposureOscillationsMin = 2,
                    OverExposureOscillationsMax = 6
                },
                CRT = new CrtEffectConfiguration
                {
                    RgbPixelFilterIntensity = 0.0f,
                    RgbPixelFilterAmount = 0.5f,
                    NumRgbFiltersHorizontally = 256,
                    NumRgbFiltersVertically = 192,
                    SimpleScanlinesIntensity = 0.0f
                }
            };

            SetEffectGroupTransition(ref initialConfig, 0.0f);
        }

        private void SetMovieStatePropertiesToDefaultValues()
        {
            _oldMovieProperties = new OldMovieEffectStateProperties
            {
                Frame = 0.2f,
                RndLine1 = 0.2f,
                RndLine2 = 0.3f,
                IsRolling = false,
                CpuShift = 0.001f,
                RollSpeed = 0.0f,
                RollAcceleration = 0.0f,
                RollPosition = 0.0f,

                IsOverExposureFlickering = false,
                OverExposureColour = new Vector4(1.0f),
                OverExposureFlickerTime = 5.0f,
                OverExposureFlickerCount = 0.0f,
                OverExposureFlickerFrac = 0.0f,
                OverExposureAngle = 0.0f,
                OverExposureOpacity = 1.0f
            };
        }

        private void CreateBuffers()
        {
            PixellateBuffer = CreateUniformBuffer(PixellateFactors.SizeInBytes);
            _edgeDetectionBuffer = CreateUniformBuffer(EdgeDetectionFactors.SizeInBytes);
            _staticBuffer = CreateUniformBuffer(StaticFactors.SizeInBytes);
            _oldMovieBuffer = CreateUniformBuffer(OldMovieFactors.SizeInBytes);
            _crtEffectBuffer = CreateUniformBuffer(CrtEffectFactors.SizeInBytes);

            PixellateResourceSet = CreateFragmentShaderUniformResourceSet(PixellateBuffer, "PixellateFactors");
            EdgeDetectionResourceSet = CreateFragmentShaderUniformResourceSet(_edgeDetectionBuffer, "EdgeDetectionFactors");
            StaticResourceSet = CreateFragmentShaderUniformResourceSet(_staticBuffer, "StaticFactors");
            OldMovieResourceSet = CreateFragmentShaderUniformResourceSet(_oldMovieBuffer, "OldMovieFactors");
            CrtEffectResourceSet = CreateFragmentShaderUniformResourceSet(_crtEffectBuffer, "CrtEffectFactors");

            FillBufferWithInitialData();
        }

        private void FillBufferWithInitialData()
        {
            //Shared Buffer gets updated in Renderer as needs to update texel size which is related to surface size
            UpdatePixellateBuffer();
            UpdateEdgeDetectionBuffer();
            UpdateStaticBuffer();
            UpdateOldMovieBuffer();
            UpdateCrtEffectBuffer();
        }

        private void UpdatePixellateBuffer()
        {
            var factors = new PixellateFactors
            {
                PixAmount = PixellateCurrent.Intensity,
                NumXDivisions = PixellateCurrent.NumXDivisions,
                NumYDivisions = PixellateCurrent.NumYDivisions,
                Pad0 = 0,
                TexelSize = Vector2.Zero,
                Pad1 = Vector2.Zero
            };

            _systemComponents.Device.UpdateBuffer(PixellateBuffer, 0, ref factors);
        }

        private void UpdateEdgeDetectionBuffer()
        {
            var factors = new EdgeDetectionFactors
            {
                DetectEdges = _edgeDetectionCurrent.Intensity > 0.0f ? 1 : 0,
                EdgeAmount = _edgeDetectionCurrent.Intensity,
                IsFreichen = _edgeDetectionCurrent.IsFreichen ? 1 : 0,
                Pad2 = Vector4.Zero
            };

            _systemComponents.Device.UpdateBuffer(_edgeDetectionBuffer, 0, ref factors);
        }

        private void UpdateStaticBuffer()
        {
            var factors = new StaticFactors
            {
                StaticAmount = _staticCurrent.Intensity,
                Time = _staticTime,
                IgnoreTransparent = _staticCurrent.IgnoreTransparent,
                TexelScaler = _staticCurrent.TexelScaler,
                Pad3 = Vector4.Zero
            };

            _systemComponents.Device.UpdateBuffer(_staticBuffer, 0, ref factors);
        }

        private void UpdateOldMovieBuffer()
        {
            var factors = new OldMovieFactors
            {
                OldMovieAmount = _oldMovieCurrent.Intensity,
                Scratch = _oldMovieCurrent.Scratch,
                Noise = _oldMovieCurrent.Noise,
                RndLine1 = _oldMovieProperties.RndLine1,
                RndLine2 = _oldMovieProperties.RndLine2,
                Frame = _oldMovieProperties.Frame,
                CpuShift = _oldMovieProperties.CpuShift,
                RndShiftCutOff = _oldMovieCurrent.RndShiftCutOff,
                RndShiftScalar = _oldMovieCurrent.RndShiftScalar,
                Dim = _oldMovieCurrent.Dim,
                Pad4 = Vector2.Zero,
                OverExposureColour = _oldMovieProperties.OverExposureColour
            };

            _systemComponents.Device.UpdateBuffer(_oldMovieBuffer, 0, ref factors);
        }

        private void UpdateCrtEffectBuffer()
        {
            var factors = new CrtEffectFactors
            {
                RgbFilterAmount = _crtEffectCurrent.RgbPixelFilterIntensity,
                RgbFilterIntensity = _crtEffectCurrent.RgbPixelFilterAmount,
                NumRgbFiltersHorizontal = _crtEffectCurrent.NumRgbFiltersHorizontally,
                NumRgbFiltersVertical = _crtEffectCurrent.NumRgbFiltersVertically,
                ScanLineAmount = _crtEffectCurrent.SimpleScanlinesIntensity,
                Pad5 = 0.0f,
                Pad6 = 0.0f,
                Pad7 = 0.0f
            };

            _systemComponents.Device.UpdateBuffer(_crtEffectBuffer, 0, ref factors);
        }

        private DeviceBuffer CreateUniformBuffer(uint sizeInBytes)
        {
            return _systemComponents.Factory.CreateBuffer(new BufferDescription(sizeInBytes, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
        }

        private ResourceSet CreateFragmentShaderUniformResourceSet(DeviceBuffer buffer, string elementName)
        {
            var resourceLayoutElementDescription = new ResourceLayoutElementDescription(elementName, ResourceKind.UniformBuffer, ShaderStages.Fragment);

            var resourceLayout = _systemComponents.Factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    resourceLayoutElementDescription
                )
            );

            return _systemComponents.Factory.CreateResourceSet(
                new ResourceSetDescription(resourceLayout, buffer)
            );
        }

        private void ProcessOldMovieCpuFactorUpdates(float seconds)
        {
            UpdateShiftRollsAndScratches(seconds);
            UpdateOverExposureFlicker(seconds);
            UpdateOldMovieBuffer();
        }

        private void UpdateShiftRollsAndScratches(float seconds)
        {
            _oldMovieProperties.Frame += 0.05f + (float)(_random.NextDouble() * seconds);

            if (_oldMovieProperties.Frame > 50.0f)
            {
                _oldMovieProperties.Frame -= 50.0f;
            }

            _oldMovieProperties.RndLine1 = (float)_random.NextDouble();
            _oldMovieProperties.RndLine2 = (float)_random.NextDouble();

            if (_oldMovieProperties.IsRolling)
            {
                if ((float)_random.NextDouble() < _oldMovieCurrent.ProbabilityRollEnds)
                {
                    _oldMovieProperties.IsRolling = false;
                    _oldMovieProperties.CpuShift = 0.0f;
                }
                else
                {
                    _oldMovieProperties.RollSpeed += _oldMovieProperties.RollAcceleration * seconds;
                    _oldMovieProperties.RollPosition += _oldMovieProperties.RollSpeed * seconds;
                    var shake = (float)_random.NextDouble() * _oldMovieCurrent.RollShakeFactor;
                    var finalMove = _oldMovieCurrent.RollOverallScale * (shake + _oldMovieProperties.RollPosition);
                    if (finalMove >= 1.0f)
                    {
                        finalMove = 1.0f;
                        _oldMovieProperties.IsRolling = false;
                        _oldMovieProperties.CpuShift = 0.0f;
                    }
                    else
                    {
                        _oldMovieProperties.CpuShift = finalMove;
                    }
                }
            }
            else
            {
                _oldMovieProperties.CpuShift = 0.0f;
                if ((float)_random.NextDouble() < _oldMovieCurrent.ProbabilityRollStarts)
                {
                    _oldMovieProperties.IsRolling = true;
                    _oldMovieProperties.RollSpeed = _oldMovieCurrent.RollSpeedMin + ((float)_random.NextDouble() * (_oldMovieCurrent.RollSpeedMax - _oldMovieCurrent.RollSpeedMin));
                    _oldMovieProperties.RollAcceleration = _oldMovieCurrent.RollAccelerationMin = ((float)_random.NextDouble() * (_oldMovieCurrent.RollAccelerationMax - _oldMovieCurrent.RollAccelerationMin));
                    _oldMovieProperties.RollPosition = 0.0f;
                }
            }
        }

        private void UpdateOverExposureFlicker(float seconds)
        {
            if (!_oldMovieProperties.IsOverExposureFlickering)
            {
                if ((float)_random.NextDouble() < _oldMovieCurrent.OverExposureProbabilityStart)
                {
                    TriggerOverExposureFlicker();
                }
            }

            if (_oldMovieProperties.IsOverExposureFlickering)
            {
                UpdateFlicker(seconds);
            }
        }

        private void TriggerOverExposureFlicker()
        {
            _oldMovieProperties.IsOverExposureFlickering = true;
            _oldMovieProperties.OverExposureColour = new Vector4(1.0f);

            _oldMovieProperties.OverExposureFlickerTime = RandomBetween(_oldMovieCurrent.OverExposureFlickerTimeMin, _oldMovieCurrent.OverExposureFlickerTimeMax);
            _oldMovieProperties.OverExposureOscillations = (int)RandomBetween(_oldMovieCurrent.OverExposureOscillationsMin, _oldMovieCurrent.OverExposureOscillationsMax) + 1;
            _oldMovieProperties.OverExposureIntensity = RandomBetween(_oldMovieCurrent.OverExposureIntensityMin, _oldMovieCurrent.OverExposureIntensityMax);
            _oldMovieProperties.OverExposureFlickerCount = 0.0f;

            _oldMovieProperties.OverExposureTotalAngle = 2.0f * (float)Math.PI * _oldMovieProperties.OverExposureOscillations;
        }

        private float RandomBetween(float v0, float v1)
        {
            return v0 + ((float)_random.NextDouble() * (v1 - v0));
        }

        private void UpdateFlicker(float seconds)
        {
            if (_oldMovieProperties.OverExposureFlickerCount > _oldMovieProperties.OverExposureFlickerTime)
            {
                _oldMovieProperties.IsOverExposureFlickering = false;
                _oldMovieProperties.OverExposureColour = new Vector4(1.0f);
            }

            if (_oldMovieProperties.IsOverExposureFlickering)
            {
                _oldMovieProperties.OverExposureFlickerFrac = _oldMovieProperties.OverExposureFlickerCount / _oldMovieProperties.OverExposureFlickerTime;
                _oldMovieProperties.OverExposureAngle = _oldMovieProperties.OverExposureTotalAngle * _oldMovieProperties.OverExposureFlickerFrac;
                _oldMovieProperties.OverExposureOpacity = 1.0f + ((1.0f - _oldMovieProperties.OverExposureFlickerFrac) * (_oldMovieProperties.OverExposureIntensity * (0.5f * (float)(Math.Sin(_oldMovieProperties.OverExposureAngle) + 1.0f))));

                _oldMovieProperties.OverExposureColour = new Vector4(_oldMovieProperties.OverExposureOpacity);

                _oldMovieProperties.OverExposureFlickerCount += 60.0f * seconds;
            }
        }

        public void SetEffectGroupTransition(ref StyleEffectGroupConfiguration config, float transitionSeconds)
        {
            SetPixellateTransition(ref config.Pixellate, transitionSeconds);
            SetEdgeDetectionTransition(ref config.EdgeDetection, transitionSeconds);
            SetStaticTransition(ref config.Static, transitionSeconds);
            SetOldMovieTransition(ref config.OldMovie, transitionSeconds);
            SetCrtEffectTransition(ref config.CRT, transitionSeconds);
        }

        public void SetPixellateTransition(ref PixellateConfiguration config, float transitionSeconds)
        {
            transitionSeconds = Utility.Clamper.Clamp(transitionSeconds, 0.0f, float.MaxValue);

            if (transitionSeconds == 0.0f)
            {
                _pixellateCurrent = config;
                _pixellateTransitioning = false;
                UpdatePixellateBuffer();
                return;
            }

            _pixellatePrevious = PixellateCurrent;
            _pixellateTarget = config;
            _pixellateTransitionTotalTime = transitionSeconds;
            _pixellateTransitionTimeCount = 0.0f;
            _pixellateTransitioning = true;
        }

        public void SetEdgeDetectionTransition(ref EdgeDetectionConfiguration config, float transitionSeconds)
        {
            transitionSeconds = Utility.Clamper.Clamp(transitionSeconds, 0.0f, float.MaxValue);

            if (transitionSeconds == 0.0f)
            {
                _edgeDetectionCurrent = config;
                _edgeDetectionTransitioning = false;
                UpdateEdgeDetectionBuffer();
                return;
            }

            _edgeDetectionPrevious = _edgeDetectionCurrent;
            _edgeDetectionTarget = config;
            _edgeDetectionTransitionTotalTime = transitionSeconds;
            _edgeDetectionTransitionTimeCount = 0.0f;
            _edgeDetectionTransitioning = true;
        }

        public void SetStaticTransition(ref StaticConfiguration config, float transitionSeconds)
        {
            transitionSeconds = Utility.Clamper.Clamp(transitionSeconds, 0.0f, float.MaxValue);

            if (transitionSeconds == 0.0f)
            {
                _staticCurrent = config;
                _staticTransitioning = false;
                UpdateStaticBuffer();
                return;
            }

            _staticPrevious = _staticCurrent;
            _staticTarget = config;
            _staticTransitionTotalTime = transitionSeconds;
            _staticTransitionTimeCount = 0.0f;
            _staticTransitioning = true;
        }

        public void SetOldMovieTransition(ref OldMovieConfiguration config, float transitionSeconds)
        {
            transitionSeconds = Utility.Clamper.Clamp(transitionSeconds, 0.0f, float.MaxValue);

            if (transitionSeconds == 0.0f)
            {
                _oldMovieCurrent = config;
                _oldMovieTransitioning = false;
                UpdateOldMovieBuffer();
                return;
            }

            _oldMoviePrevious = _oldMovieCurrent;
            _oldMovieTarget = config;
            _oldMovieTransitionTotalTime = transitionSeconds;
            _oldMovieTransitionTimeCount = 0.0f;
            _oldMovieTransitioning = true;
        }

        public void SetCrtEffectTransition(ref CrtEffectConfiguration config, float transitionSeconds)
        {
            transitionSeconds = Utility.Clamper.Clamp(transitionSeconds, 0.0f, float.MaxValue);

            if (transitionSeconds == 0.0f)
            {
                _crtEffectCurrent = config;
                _crtEffectTransitioning = false;
                UpdateCrtEffectBuffer();
                return;
            }

            _crtEffectPrevious = _crtEffectCurrent;
            _crtEffectTarget = config;
            _crtEffectTransitionTotalTime = transitionSeconds;
            _crtEffectTransitionTimeCount = 0.0f;
            _crtEffectTransitioning = true;
        }

        public void Update(float seconds)
        {
            if (_oldMovieCurrent.Intensity > 0.0f)
            {
                ProcessOldMovieCpuFactorUpdates(seconds);
            }

            if (_pixellateTransitioning)
            {
                ProcessPixellateTransition(seconds);
                UpdatePixellateBuffer();
            }

            if (_edgeDetectionTransitioning)
            {
                ProcessEdgeDetectionTransition(seconds);
                UpdateEdgeDetectionBuffer();
            }

            if (_staticTransitioning)
            {
                ProcessStaticFactors(seconds);
                ProcessStaticTransition(seconds);
                UpdateStaticBuffer();
            }
            else
            {
                if (_staticCurrent.Intensity > 0.0f)
                {
                    ProcessStaticFactors(seconds);
                    UpdateStaticBuffer();
                }
            }

            if (_oldMovieTransitioning)
            {
                ProcessOldMovieTransition(seconds);
                UpdateOldMovieBuffer();
            }

            if (_crtEffectTransitioning)
            {
                ProcessCrtEffectTransition(seconds);
                UpdateCrtEffectBuffer();
            }
        }

        private void ProcessPixellateTransition(float seconds)
        {
            _pixellateTransitionTimeCount += seconds;
            var fraction = _pixellateTransitionTimeCount / _pixellateTransitionTotalTime;

            if (fraction >= 1.0f)
            {
                fraction = 1.0f;
                _pixellateTransitioning = false;
            }

            _pixellateCurrent.Intensity = Utility.Interpolator.Interpolate(_pixellatePrevious.Intensity, _pixellateTarget.Intensity, ref fraction);
            _pixellateCurrent.NumXDivisions = Utility.Interpolator.Interpolate(_pixellatePrevious.NumXDivisions, _pixellateTarget.NumXDivisions, ref fraction);
            _pixellateCurrent.NumYDivisions = Utility.Interpolator.Interpolate(_pixellatePrevious.NumYDivisions, _pixellateTarget.NumYDivisions, ref fraction);
        }

        private void ProcessEdgeDetectionTransition(float seconds)
        {
            _edgeDetectionTransitionTimeCount += seconds;
            var fraction = _edgeDetectionTransitionTimeCount / _edgeDetectionTransitionTotalTime;

            if (fraction >= 1.0f)
            {
                fraction = 1.0f;
                _edgeDetectionTransitioning = false;
            }

            _edgeDetectionCurrent.Intensity = Utility.Interpolator.Interpolate(_edgeDetectionPrevious.Intensity, _edgeDetectionTarget.Intensity, ref fraction);
            _edgeDetectionCurrent.IsFreichen = _edgeDetectionTarget.IsFreichen;
        }

        private void ProcessStaticFactors(float seconds)
        {
            _staticTime += _staticCurrent.TimeSpeed * seconds;
            if (_staticTime > 10000.0f)
            {
                _staticTime -= 10000.0f;
            }
        }

        private void ProcessStaticTransition(float seconds)
        {
            _staticTransitionTimeCount += seconds;
            var fraction = _staticTransitionTimeCount / _staticTransitionTotalTime;

            if (fraction >= 1.0f)
            {
                fraction = 1.0f;
                _staticTransitioning = false;
            }

            _staticCurrent.Intensity = Utility.Interpolator.Interpolate(_staticPrevious.Intensity, _staticTarget.Intensity, ref fraction);
            _staticCurrent.TimeSpeed = Utility.Interpolator.Interpolate(_staticPrevious.TimeSpeed, _staticTarget.TimeSpeed, ref fraction);
            _staticCurrent.IgnoreTransparent = Utility.Interpolator.Interpolate(_staticPrevious.IgnoreTransparent, _staticTarget.IgnoreTransparent, ref fraction);
            _staticCurrent.TexelScaler = Utility.Interpolator.Interpolate(_staticPrevious.TexelScaler, _staticTarget.TexelScaler, ref fraction);
        }

        private void ProcessOldMovieTransition(float seconds)
        {
            _oldMovieTransitionTimeCount += seconds;
            var fraction = _oldMovieTransitionTimeCount / _oldMovieTransitionTotalTime;

            if (fraction >= 1.0f)
            {
                fraction = 1.0f;
                _oldMovieTransitioning = false;
            }

            _oldMovieCurrent.Intensity = Utility.Interpolator.Interpolate(_oldMoviePrevious.Intensity, _oldMovieTarget.Intensity, ref fraction);
            _oldMovieCurrent.Noise = Utility.Interpolator.Interpolate(_oldMoviePrevious.Noise, _oldMovieTarget.Noise, ref fraction);
            _oldMovieCurrent.OverExposureFlickerTimeMax = Utility.Interpolator.Interpolate(_oldMoviePrevious.OverExposureFlickerTimeMax, _oldMovieTarget.OverExposureFlickerTimeMax, ref fraction);
            _oldMovieCurrent.OverExposureFlickerTimeMin = Utility.Interpolator.Interpolate(_oldMoviePrevious.OverExposureFlickerTimeMin, _oldMovieTarget.OverExposureFlickerTimeMin, ref fraction);
            _oldMovieCurrent.OverExposureIntensityMax = Utility.Interpolator.Interpolate(_oldMoviePrevious.OverExposureIntensityMax, _oldMovieTarget.OverExposureIntensityMax, ref fraction);
            _oldMovieCurrent.OverExposureIntensityMin = Utility.Interpolator.Interpolate(_oldMoviePrevious.OverExposureIntensityMin, _oldMovieTarget.OverExposureIntensityMin, ref fraction);
            _oldMovieCurrent.OverExposureOscillationsMax = Utility.Interpolator.Interpolate(_oldMoviePrevious.OverExposureOscillationsMax, _oldMovieTarget.OverExposureOscillationsMax, ref fraction);
            _oldMovieCurrent.OverExposureOscillationsMin = Utility.Interpolator.Interpolate(_oldMoviePrevious.OverExposureOscillationsMin, _oldMovieTarget.OverExposureOscillationsMin, ref fraction);
            _oldMovieCurrent.OverExposureProbabilityStart = Utility.Interpolator.Interpolate(_oldMoviePrevious.OverExposureProbabilityStart, _oldMovieTarget.OverExposureProbabilityStart, ref fraction);
            _oldMovieCurrent.ProbabilityRollEnds = Utility.Interpolator.Interpolate(_oldMoviePrevious.ProbabilityRollEnds, _oldMovieTarget.ProbabilityRollEnds, ref fraction);
            _oldMovieCurrent.ProbabilityRollStarts = Utility.Interpolator.Interpolate(_oldMoviePrevious.ProbabilityRollStarts, _oldMovieTarget.ProbabilityRollStarts, ref fraction);
            _oldMovieCurrent.RndShiftCutOff = Utility.Interpolator.Interpolate(_oldMoviePrevious.RndShiftCutOff, _oldMovieTarget.RndShiftCutOff, ref fraction);
            _oldMovieCurrent.RndShiftScalar = Utility.Interpolator.Interpolate(_oldMoviePrevious.RndShiftScalar, _oldMovieTarget.RndShiftScalar, ref fraction);
            _oldMovieCurrent.RollAccelerationMax = Utility.Interpolator.Interpolate(_oldMoviePrevious.RollAccelerationMax, _oldMovieTarget.RollAccelerationMax, ref fraction);
            _oldMovieCurrent.RollAccelerationMin = Utility.Interpolator.Interpolate(_oldMoviePrevious.RollAccelerationMin, _oldMovieTarget.RollAccelerationMin, ref fraction);
            _oldMovieCurrent.RollOverallScale = Utility.Interpolator.Interpolate(_oldMoviePrevious.RollOverallScale, _oldMovieTarget.RollOverallScale, ref fraction);
            _oldMovieCurrent.RollShakeFactor = Utility.Interpolator.Interpolate(_oldMoviePrevious.RollShakeFactor, _oldMovieTarget.RollShakeFactor, ref fraction);
            _oldMovieCurrent.RollSpeedMax = Utility.Interpolator.Interpolate(_oldMoviePrevious.RollSpeedMax, _oldMovieTarget.RollSpeedMax, ref fraction);
            _oldMovieCurrent.RollSpeedMin = Utility.Interpolator.Interpolate(_oldMoviePrevious.RollSpeedMin, _oldMovieTarget.RollSpeedMin, ref fraction);
            _oldMovieCurrent.Scratch = Utility.Interpolator.Interpolate(_oldMoviePrevious.Scratch, _oldMovieTarget.Scratch, ref fraction);
        }

        private void ProcessCrtEffectTransition(float seconds)
        {
            _crtEffectTransitionTimeCount += seconds;
            var fraction = _crtEffectTransitionTimeCount / _crtEffectTransitionTotalTime;

            if (fraction >= 1.0f)
            {
                fraction = 1.0f;
                _crtEffectTransitioning = false;
            }

            _crtEffectCurrent.RgbPixelFilterIntensity = Utility.Interpolator.Interpolate(_crtEffectPrevious.RgbPixelFilterIntensity, _crtEffectTarget.RgbPixelFilterIntensity, ref fraction);
            _crtEffectCurrent.RgbPixelFilterAmount = Utility.Interpolator.Interpolate(_crtEffectPrevious.RgbPixelFilterAmount, _crtEffectTarget.RgbPixelFilterAmount, ref fraction);
            _crtEffectCurrent.NumRgbFiltersHorizontally = Utility.Interpolator.Interpolate(_crtEffectPrevious.NumRgbFiltersHorizontally, _crtEffectTarget.NumRgbFiltersHorizontally, ref fraction);
            _crtEffectCurrent.NumRgbFiltersVertically = Utility.Interpolator.Interpolate(_crtEffectPrevious.NumRgbFiltersVertically, _crtEffectTarget.NumRgbFiltersVertically, ref fraction);
            _crtEffectCurrent.SimpleScanlinesIntensity = Utility.Interpolator.Interpolate(_crtEffectPrevious.SimpleScanlinesIntensity, _crtEffectTarget.SimpleScanlinesIntensity, ref fraction);
        }

        public void DestroyResources()
        {
            PixellateResourceSet?.Dispose();
            EdgeDetectionResourceSet?.Dispose();
            StaticResourceSet?.Dispose();
            OldMovieResourceSet?.Dispose();
            CrtEffectResourceSet?.Dispose();

            PixellateBuffer?.Dispose();
            _edgeDetectionBuffer?.Dispose();
            _staticBuffer?.Dispose();
            _oldMovieBuffer?.Dispose();
            _crtEffectBuffer?.Dispose();
        }
    }
}