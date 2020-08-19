using System.Collections.Generic;
using System.Linq;
using Veldrid;

namespace Yak2D.Utility
{
    public static class GraphicsApiConverter
    {
        public static List<GraphicsBackend> AllCurrentlySupportedFrameworkBackends { get; private set; }

        static GraphicsApiConverter()
        {
            AllCurrentlySupportedFrameworkBackends = new List<GraphicsBackend>
            {
                GraphicsBackend.Direct3D11,
                GraphicsBackend.Metal,
                GraphicsBackend.OpenGL,
                GraphicsBackend.Vulkan, 
                //GraphicsBackend.OpenGLES
            };
        }

        public static List<GraphicsApi> AllSupportedGraphicsApis()
        {
            return AllCurrentlySupportedFrameworkBackends
                            .Where(api => GraphicsDevice.IsBackendSupported(api))
                            .Select(veldrid => ConvertVeldridGraphicsBackendToApi(veldrid))
                            .ToList();
        }

        public static GraphicsBackend ConvertApiToVeldridGraphicsBackend(GraphicsApi api)
        {
            switch (api)
            {
                case GraphicsApi.Direct3D11:
                    return GraphicsBackend.Direct3D11;
                case GraphicsApi.Metal:
                    return GraphicsBackend.Metal;
                case GraphicsApi.OpenGL:
                    return GraphicsBackend.OpenGL;
                case GraphicsApi.OpenGLES:
                    return GraphicsBackend.OpenGLES;
                case GraphicsApi.Vulkan:
                    return GraphicsBackend.Vulkan;
                default:
                    return GraphicsBackend.OpenGL; //This function is not (!) run if SystemDefault enum option chosen. But defaults to opengl anyway if happens too...
            }
        }

        public static GraphicsApi ConvertVeldridGraphicsBackendToApi(GraphicsBackend backend)
        {
            switch (backend)
            {
                case GraphicsBackend.Direct3D11:
                    return GraphicsApi.Direct3D11;
                case GraphicsBackend.Metal:
                    return GraphicsApi.Metal;
                case GraphicsBackend.OpenGL:
                    return GraphicsApi.OpenGL;
                case GraphicsBackend.OpenGLES:
                    return GraphicsApi.OpenGLES;
                case GraphicsBackend.Vulkan:
                    return GraphicsApi.Vulkan;
                default:
                    return GraphicsApi.OpenGL;
            }
        }
    }
}
