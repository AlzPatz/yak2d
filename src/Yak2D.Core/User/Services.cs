namespace Yak2D.Core
{
    public class Services : IServices
    {
        public IBackend Backend { get; private set; }
        public IDisplay Display { get; private set; }
        public IFps FPS { get; private set; }
        public IStages Stages { get; private set; }
        public IInput Input { get; private set; }
        public ISurfaces Surfaces { get; private set; }
        public ICameras Cameras { get; private set; }
        public IFonts Fonts { get; private set; }
        public IHelpers Helpers { get; private set; }

        public Services(IBackend backend,
                        IDisplay display,
                        IFps fps,
                        IStages stages,
                        IInput input,
                        ISurfaces surfaces,
                        ICameras cameras,
                        IFonts fonts,
                        IHelpers helpers)
        {
            Backend = backend;
            Display = display;
            FPS = fps;
            Stages = stages;
            Input = input;
            Surfaces = surfaces;
            Cameras = cameras;
            Fonts = fonts;
            Helpers = helpers;
        }
    }
}