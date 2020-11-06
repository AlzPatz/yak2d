namespace Yak2D
{
    /// <summary>
    /// The interface that user apps must implement. The framework calls the methods which define the broad program flow
    /// </summary>
    public interface IApplication
    {
        /// <summary>
        /// The user should return the desired configuration properties. Called once
        /// </summary>
        StartupConfig Configure();

        /// <summary>
        /// Called once on startup, prior to the intial resource creation call. The application may wish to implement one time, independent operations here
        /// </summary>
        void OnStartup();

        /// <summary>
        /// If there are framework messages in the queue, ahead of an update call, this method is called once per message
        /// </summary>
        /// <param name="msg">The framework message</param>
        /// <param name="yak">Collection of framework user services</param>
        void ProcessMessage(FrameworkMessage msg, IServices yak);

        /// <summary>
        /// Called whenever framework related resources should be created, including once at start up and whenever resources are lost (such as graphics device change). An application should be able to restore all required resources at any moment this is called
        /// </summary>
        /// <param name="yak">Collection of framework user services</param>
        bool CreateResources(IServices yak);

        /// <summary>
        /// Called when the user application should execute an update iteration. Return false to shutdown the application
        /// </summary>
        /// <param name="yak">Collection of framework user services</param>
        /// <param name="secondsSinceLastUpdate">Seconds since last update iteration</param>
        bool Update(IServices yak, float secondsSinceLastUpdate);

        /// <summary>
        /// Called ahead of Drawing. The user may wish to include preparatory steps here before drawing begins. Both time since last draw and update are provided. Time since last update could be used to interpolate positions to smooth motion
        /// </summary>
        /// <param name="yak">Collection of framework user services</param>
        /// <param name="secondsSinceLastDraw">Seconds since last draw iteration</param>
        /// <param name="secondsSinceLastupdate">Seconds since last update iteration</param>
        void PreDrawing(IServices yak, float secondsSinceLastDraw, float secondsSinceLastupdate);

        /// <summary>
        /// The user should populate Draw Request Queues here
        /// </summary>
        /// <param name="draw">Exposes methods used for updating draw queues</param>
        /// <param name="fps">Provides update and draw iteration FPS</param>
        /// <param name="input">Exposes framework input services. Whilst best practice input processing should be done in an update iteration (you will not get correct behaviour regarding whether a button or key was release or pressed 'this frame' for example, as these are tied to updates). Access to input here can be useful in some cases</param>
        /// <param name="transforms"> Provides functions to transform positions between World, Screen and Window coordinates</param>
        /// <param name="secondsSinceLastDraw">Seconds since last draw iteration</param>
        /// <param name="secondsSinceLastUpdate">Seconds since last update iteration</param>
        void Drawing(IDrawing draw, IFps fps, IInput input, ICoordinateTransforms transforms, float secondsSinceLastDraw, float secondsSinceLastUpdate);

        /// <summary>
        /// The user should build the frame's render pipeline here
        /// </summary>
        /// <param name="q">Exposes methods used for building the render pipeline</param>
        /// <param name="windowRenderTarget">The RenderTarget reference of the current window's main rendering surface</param>
        void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget);

        /// <summary>
        /// Called when application is shutting down. User should release non-framework related resources here
        /// </summary>
        void Shutdown();
    }
}