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
        /// <param name="services">Collection of framework user services</param>
        void ProcessMessage(FrameworkMessage msg, IServices services);

        /// <summary>
        /// Called whenever framework related resources should be created, including once at start up and whenever resources are lost (such as graphics device change). An application should be able to restore all required resources at any moment this is called
        /// </summary>
        /// <param name="services">Collection of framework user services</param>
        bool CreateResources(IServices services);

        /// <summary>
        /// Called when the user application should execute an update iteration. Return false to shutdown the application
        /// </summary>
        /// <param name="services">Collection of framework user services</param>
        /// <param name="timeSinceLastUpdateSeconds">Seconds since last update iteration</param>
        bool Update(IServices services, float timeSinceLastUpdateSeconds);

        /// <summary>
        /// Called ahead of Drawing. The user may wish to include preparatory steps here before drawing begins. Both time since last draw and update are provided. Time since last update could be used to interpolate positions to smooth motion
        /// </summary>
        /// <param name="services">Collection of framework user services</param>
        /// <param name="timeSinceLastDrawSeconds">Seconds since last draw iteration</param>
        /// <param name="timeSinceLastUpdateSeconds">Seconds since last update iteration</param>
        void PreDrawing(IServices services, float timeSinceLastDrawSeconds, float timeSinceLastUpdateSeconds);

        /// <summary>
        /// The user should populate Draw Request Queues here
        /// </summary>
        /// <param name="drawing">Exposes methods used for updating draw queues</param>
        /// <param name="fps">Provides update and draw iteration FPS</param>
        /// <param name="input">Exposes framework input services. Whilst best practice input processing should be done in an update iteration. Access to input here can be useful in some cases</param>
        /// <param name="timeSinceLastDrawSeconds">Seconds since last draw iteration</param>
        /// <param name="timeSinceLastUpdateSeconds">Seconds since last update iteration</param>
        void Drawing(IDrawing drawing, IFps fps, IInput input, float timeSinceLastDrawSeconds, float timeSinceLastUpdateSeconds);

        /// <summary>
        /// The user should build the frame's render pipeline here
        /// </summary>
        /// <param name="queue">Exposes methods used for building the render pipeline</param>
        void Rendering(IRenderQueue queue);

        /// <summary>
        /// Called when application is shutting down. User should release non-framework related resources here
        /// </summary>
        void Shutdown();
    }
}