using System;
using System.Numerics;

namespace Yak2D
{
    /// <summary>
    /// Functions to transform positions between World, Screen and Window coordinates
    /// </summary>
    public interface ICoordinateTransforms
    {
        /// <summary>
        /// Converts a Camera's World position to it's Screen Position 
        /// </summary>
        /// <param name="position">World Position To Transform</param>
        /// <param name="camera">Camera through which the world is viewed</param>
        Vector2 ScreenFromWorld(Vector2 position, ICamera2D camera);
        /// <summary>
        /// Converts a Camera's World position to it's Screen Position 
        /// </summary>
        /// <param name="position">World Position To Transform</param>
        /// <param name="camera">Camera id key through which the world is viewed</param>
        Vector2 ScreenFromWorld(Vector2 position, ulong camera);

        /// <summary>
        /// Converts a Camera's Screen position to it's World Position 
        /// </summary>
        /// <param name="position">Screen Position To Transform</param>
        /// <param name="camera">Camera through which the world is viewed</param>
        Vector2 WorldFromScreen(Vector2 position, ICamera2D camera);
        /// <summary>
        /// Converts a Camera's Screen position to it's World Position 
        /// </summary>
        /// <param name="position">Screen Position To Transform</param>
        /// <param name="camera">Camera id key through which the world is viewed</param>
        Vector2 WorldFromScreen(Vector2 position, ulong camera);

        /// <summary>
        /// Converts a Window Position (Origin Top-Left) to a Camera's Screen Position
        /// </summary>
        /// <param name="position">Window Position To Transform (Origin Top-Left)</param>
        /// <param name="camera">Camera through which the world is viewed</param>
        /// <param name="viewport">The viewport which defines a camera's window area (default null is whole window)</param>
        TransformResult ScreenFromWindow(Vector2 position, ICamera2D camera, IViewport viewport = null);
        /// <summary>
        /// Converts a Window Position (Origin Top-Left) to a Camera's Screen Position
        /// </summary>
        /// <param name="position">Window Position To Transform (Origin Top-Left)</param>
        /// <param name="camera">Camera id key through which the world is viewed</param>
        /// <param name="viewport">The viewport id key which defines a camera's window area (default null is whole window)</param>
        TransformResult ScreenFromWindow(Vector2 position, ulong camera, ulong? viewport = null);

        /// <summary>
        /// Converts a Screen Position to a Window Position (Origin Top-Left)
        /// </summary>
        /// <param name="position">Screen Position To Transform</param>
        /// <param name="camera">Camera through which the world is viewed</param>
        /// <param name="viewport">The viewport which defines a camera's window area (default null is whole window)</param>
        TransformResult WindowFromScreen(Vector2 position, ICamera2D camera, IViewport viewport = null);
        /// <summary>
        /// Converts a Screen Position to a Window Position (Origin Top-Left)
        /// </summary>
        /// <param name="position">Screen Position To Transform</param>
        /// <param name="camera">Camera id key through which the world is viewed</param>
        /// <param name="viewport">The viewport id key which defines a camera's window area (default null is whole window)</param>
        TransformResult WindowFromScreen(Vector2 position, ulong camera, ulong? viewport = null);

        /// <summary>
        /// Converts a Window Position (Origin Top-Left) to a Camera's World Position
        /// </summary>
        /// <param name="position">Window Position To Transform (Origin Top-Left)</param>
        /// <param name="camera">Camera through which the world is viewed</param>
        /// <param name="viewport">The viewport which defines a camera's window area (default null is whole window)</param>
        TransformResult WorldFromWindow(Vector2 position, ICamera2D camera, IViewport viewport = null);
        /// <summary>
        /// Converts a Window Position (Origin Top-Left) to a Camera's World Position
        /// </summary>
        /// <param name="position">Window Position To Transform (Origin Top-Left)</param>
        /// <param name="camera">Camera through which the world is viewed</param>
        /// <param name="viewport">The viewport which defines a camera's window area (default null is whole window)</param>
        TransformResult WorldFromWindow(Vector2 position, ulong camera, ulong? viewport = null);

        /// <summary>
        /// Converts a World Position to a Window Position (Origin Top-Left)
        /// </summary>
        /// <param name="position">World Position To Transform</param>
        /// <param name="camera">Camera through which the world is viewed</param>
        /// <param name="viewport">The viewport which defines a camera's window area (default null is whole window)</param>
        TransformResult WindowFromWorld(Vector2 position, ICamera2D camera, IViewport viewport = null);

        /// <summary>
        /// Converts a World Position to a Window Position (Origin Top-Left)
        /// </summary>
        /// <param name="position">World Position To Transform</param>
        /// <param name="camera">Camera through which the world is viewed</param>
        /// <param name="viewport">The viewport which defines a camera's window area (default null is whole window)</param>
        TransformResult WindowFromWorld(Vector2 position, ulong camera, ulong? viewport = null);
    }
}