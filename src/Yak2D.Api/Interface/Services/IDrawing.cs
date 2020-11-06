using System.Drawing;
using System.Numerics;

namespace Yak2D
{
    /// <summary>
    /// Drawing Operations
    /// Build draw queues containing lists of triangles and their properties (colouring and texturing, layer, coordinate space and depth)
    /// Draw queues exist for standard drawing (DrawStage) as well as drawing to distortion height maps (DistortionStage)
    /// Bitmap font rendering is supported
    /// </summary>
    public interface IDrawing
    {
        /// <summary>
        /// Provides access to functions to simplify common drawing operations (lines, quads, regular polygons and arrows)
        /// Includes a  fluent interface option for building flexible partial shapes and iterating / modifying 
        /// </summary>
        IDrawingHelpers Helpers { get; }

        /// <summary>
        /// Create a Texture reference object from an id
        /// DrawRequests require Texture References (currently), so if the user is storing raw ulong references instead, this method allows an easy conversion back to a reference object
        /// </summary>
        /// <param name="id">The Texture's unique ulong identifier</param>
        ITexture WrapTextureId(ulong id);

        /// <summary>
        /// Get size of texture in pixels
        /// </summary>
        /// <param name="texture">Texture reference"</param>
        Size GetSurfaceDimensions(ITexture texture);
        /// <summary>
        /// Get size of texture in pixels
        /// </summary>
        /// <param name="texture">Texture id"</param>
        Size GetSurfaceDimensions(ulong texture);

        /// <summary>
        /// A persistent draw queue has a collection of draw requests that persist even after clearing the dynamic portion of a DrawStage's draw queue
        /// </summary>
        /// <param name="stage">DrawStage reference that the persistent queue will be added too</param>
        /// <param name="requests">Draw requests that will for the persistent part of the draw queue</param>
        /// <param name="validate">Check whether the content of draw request array appears valid. If this fails, null is returned</param>
        IPersistentDrawQueue CreatePersistentDrawQueue(IDrawStage stage,
                                                       DrawRequest[] requests,
                                                       bool validate = false);

        /// <summary>
        /// A persistent draw queue has a collection of draw requests that persist even after clearing the dynamic portion of a DrawStage's draw queue
        /// </summary>
        /// <param name="stage">DrawStage id that the persistent queue will be added too</param>
        /// <param name="requests">Draw requests that will for the persistent part of the draw queue</param>
        /// <param name="validate">Check whether the content of draw request array appears valid. If this fails, null is returned</param>
        IPersistentDrawQueue CreatePersistentDrawQueue(ulong stage,
                                                       DrawRequest[] requests,
                                                       bool validate = false);

        /// <summary>
        /// Removes (destroys) a persistent draw queue from a DrawStage
        /// </summary>
        /// <param name="stage">DrawStage reference from which to remove the persistent queue (if attached)</param>                                               
        /// <param name="queue">PersistentDrawQueue reference to remove from the DrawStage</param>                                               
        void RemovePersistentDrawQueue(IDrawStage stage,
                                       IPersistentDrawQueue queue);

        /// <summary>
        /// Removes (destroys) a persistent draw queue from a DrawStage
        /// </summary>
        /// <param name="stage">DrawStage id from which to remove the persistent queue (if attached)</param>                                               
        /// <param name="queue">PersistentDrawQueue id to remove from the DrawStage</param>                                               
        void RemovePersistentDrawQueue(ulong stage,
                                       ulong queue);

        /// <summary>
        /// Clears the Dynamic (non-persistent-on-clear) Draw Queue from a DrawStage
        /// </summary>
        /// <param name="stage">DrawStage reference</param>
        void ClearDynamicDrawRequestQueue(IDrawStage stage);

        /// <summary>
        /// Clears the Dynamic (non-persistent-on-clear) Draw Queue from a DrawStage
        /// </summary>
        /// <param name="stage">DrawStage id</param>
        void ClearDynamicDrawRequestQueue(ulong stage);

        /// <summary>
        /// Adds draw request to the DrawStage's dynamic (non-persistent-on-clear) draw queue
        /// </summary>
        /// <param name="stage">DrawStage reference</param>
        /// <param name="request">Draw request to add to queue</param>
        /// <param name="validate">Check whether the content of the draw request appears valid before adding to the queue</param>
        void Draw(IDrawStage stage,
                  DrawRequest request,
                  bool validate = false);

        /// <summary>
        /// Adds draw request to the DrawStage's dynamic (non-persistent-on-clear) draw queue
        /// </summary>
        /// <param name="stage">DrawStage id</param>
        /// <param name="request">Draw request to add to queue</param>
        /// <param name="validate">Check whether the content of the draw request appears valid before adding to the queue</param>
        void Draw(ulong stage,
                  DrawRequest request,
                  bool validate = false);

        /// <summary>
        /// Adds draw request to the DrawStage's dynamic (non-persistent-on-clear) draw queue
        /// </summary>
        /// <param name="stage">DrawStage reference</param>
        /// <param name="request">Draw request to add to queue</param>
        /// <param name="validate">Check whether the content of the draw request appears valid before adding to the queue</param>
        void Draw(IDrawStage stage,
                  ref DrawRequest request,
                  bool validate = false);


        /// <summary>
        /// Adds draw request to the DrawStage's dynamic (non-persistent-on-clear) draw queue
        /// </summary>
        /// <param name="stage">DrawStage id</param>
        /// <param name="request">Draw request to add to queue</param>
        /// <param name="validate">Check whether the content of the draw request appears valid before adding to the queue</param>
        void Draw(ulong stage,
                  ref DrawRequest request,
                  bool validate = false);

        /// <summary>
        /// Add draw request to the DrawStage's dynamic (non-persistent-on-clear) draw queue by specifying the request's components
        /// </summary>
        /// <param name="stage">DrawStage reference</param>
        /// <param name="target">The coordinate space (world or screen) that the vertices should be transformed by</param>
        /// <param name="type">Fill / Texturing type - Coloured, Single Textured or Dual Textured</param>
        /// <param name="vertices">The vertices that make up the triangles to draw</param>
        /// <param name="indices">The indices that reference vertices that make up the triangles to draw</param>
        /// <param name="colour">An overall colour applied to all vertices at drawing</param>
        /// <param name="texture0">Primary Texture Reference (the only texture used for single texturing)</param>
        /// <param name="texture1">Secondary Texture Reference (used for dual texturing)</param>
        /// <param name="texWrap0">Primary Texture Coordinate Wrap behaviour</param>
        /// <param name="texWrap1">Secondary Texure Coordinate Wrap behaviour</param>
        /// <param name="depth">Z depth of vertices defining ordering within a layer (0.0 [front] to 1.0 [back])</param>
        /// <param name="layer">>= 0. The layer that these vertices belong too, lower layers are drawn behind higher layers</param>
        /// <param name="validate">Check whether the content of the draw request appears valid before adding to the queue</param>
        void Draw(IDrawStage stage,
                    CoordinateSpace target,
                    FillType type,
                    Vertex2D[] vertices,
                    int[] indices,
                    Colour colour,
                    ITexture texture0,
                    ITexture texture1,
                    TextureCoordinateMode texWrap0,
                    TextureCoordinateMode texWrap1,
                    float depth,
                    int layer,
                    bool validate = false);

        /// <summary>
        /// Add draw request to the DrawStage's dynamic (non-persistent-on-clear) draw queue by specifying the request's components
        /// </summary>
        /// <param name="stage">DrawStage id</param>
        /// <param name="target">The coordinate space (world or screen) that the vertices should be transformed by</param>
        /// <param name="type">Fill / Texturing type - Coloured, Single Textured or Dual Textured</param>
        /// <param name="vertices">The vertices that make up the triangles to draw</param>
        /// <param name="indices">The indices that reference vertices that make up the triangles to draw</param>
        /// <param name="colour">An overall colour applied to all vertices at drawing</param>
        /// <param name="texture0">Primary Texture id (the only texture used for single texturing)</param>
        /// <param name="texture1">Secondary Texture id (used for dual texturing)</param>
        /// <param name="texWrap0">Primary Texture Coordinate Wrap behaviour</param>
        /// <param name="texWrap1">Secondary Texure Coordinate Wrap behaviour</param>
        /// <param name="depth">Z depth of vertices defining ordering within a layer (0.0 [front] to 1.0 [back])</param>
        /// <param name="layer">>= 0. The layer that these vertices belong too, lower layers are drawn behind higher layers</param>
        /// <param name="validate">Check whether the content of the draw request appears valid before adding to the queue</param>
        void Draw(ulong stage,
                    CoordinateSpace target,
                    FillType type,
                    Vertex2D[] vertices,
                    int[] indices,
                    Colour colour,
                    ulong texture0,
                    ulong texture1,
                    TextureCoordinateMode texWrap0,
                    TextureCoordinateMode texWrap1,
                    float depth,
                    int layer,
                    bool validate = false);

        /// <summary>
        /// A persistent distortion draw queue has a collection of distortion draw requests that persist even after clearing the dynamic portion of a DistortionStage's draw queue
        /// </summary>
        /// <param name="stage">DistortionStage Reference that the persistent queue will be added too</param>
        /// <param name="requests">Distortion Draw requests that will for the persistent part of the draw queue</param>
        /// <param name="validate">Check whether the content of distortion draw request array appears valid. If this fails, null is returned</param>
        IPersistentDistortionQueue CreatePersistentDistortQueue(IDistortionStage stage,
                                                                DistortionDrawRequest[] requests,
                                                                bool validate = false);

        /// <summary>
        /// A persistent distortion draw queue has a collection of distortion draw requests that persist even after clearing the dynamic portion of a DistortionStage's draw queue
        /// </summary>
        /// <param name="stage">DistortionStage id that the persistent queue will be added too</param>
        /// <param name="requests">Distortion Draw requests that will for the persistent part of the draw queue</param>
        /// <param name="validate">Check whether the content of distortion draw request array appears valid. If this fails, null is returned</param>
        IPersistentDistortionQueue CreatePersistentDistortQueue(ulong stage,
                                                                DistortionDrawRequest[] requests,
                                                                bool validate = false);

        /// <summary>
        /// Removes (destroys) a persistent distortion draw queue from a DistortionStage
        /// </summary>
        /// <param name="stage">DistortionStage reference from which to remove the persistent queue (if attached)</param>                                               
        /// <param name="queue">PersistentDistortionQueue reference to remove from the DistortionStage</param>       
        void RemovePersistentDistortQueue(IDistortionStage stage,
                                          IPersistentDistortionQueue queue);

        /// <summary>
        /// Removes (destroys) a persistent distortion draw queue from a DistortionStage
        /// </summary>
        /// <param name="stage">DistortionStage id from which to remove the persistent queue (if attached)</param>                                               
        /// <param name="queue">PersistentDistortionQueue id to remove from the DistortionStage</param>       
        void RemovePersistentDistortQueue(ulong stage,
                                          ulong queue);

        /// <summary>
        /// Clears the Dynamic (non-persistent-on-clear) Draw Queue from a DistortionStage
        /// </summary>
        /// <param name="stage">DistortionStage reference</param>
        void ClearDynamicDistortionRequestQueue(IDistortionStage stage);

        /// <summary>
        /// Clears the Dynamic (non-persistent-on-clear) Draw Queue from a DistortionStage
        /// </summary>
        /// <param name="stage">DistortionStage id</param>
        void ClearDynamicDistortionRequestQueue(ulong stage);

        /// <summary>
        /// Adds distortion draw request to the DistortionStage's dynamic (non-persistent-on-clear) draw queue
        /// </summary>
        /// <param name="stage">DistortionStage reference</param>
        /// <param name="request">Distortion Draw request to add to queue</param>
        /// <param name="validate">Check whether the content of the distortion draw request appears valid before adding to the queue</param>
        void DrawDistortion(IDistortionStage stage,
                            DistortionDrawRequest request,
                            bool validate = false);

        /// <summary>
        /// Adds distortion draw request to the DistortionStage's dynamic (non-persistent-on-clear) draw queue
        /// </summary>
        /// <param name="stage">DistortionStage id</param>
        /// <param name="request">Distortion Draw request to add to queue</param>
        /// <param name="validate">Check whether the content of the distortion draw request appears valid before adding to the queue</param>
        void DrawDistortion(ulong stage,
                            DistortionDrawRequest request,
                            bool validate = false);

        /// <summary>
        /// Adds distortion draw request to the DistortionStage's dynamic (non-persistent-on-clear) draw queue
        /// </summary>
        /// <param name="stage">DistortionStage reference</param>
        /// <param name="request">Distortion Draw request to add to queue</param>
        /// <param name="validate">Check whether the content of the distortion draw request appears valid before adding to the queue</param>
        void DrawDistortion(IDistortionStage stage,
                            ref DistortionDrawRequest request,
                            bool validate = false);

        /// <summary>
        /// Adds distortion draw request to the DistortionStage's dynamic (non-persistent-on-clear) draw queue
        /// </summary>
        /// <param name="stage">DistortionStage id</param>
        /// <param name="request">Distortion Draw request to add to queue</param>
        /// <param name="validate">Check whether the content of the distortion draw request appears valid before adding to the queue</param>
        void DrawDistortion(ulong stage,
                            ref DistortionDrawRequest request,
                            bool validate = false);

        /// <summary>
        /// Add distortion draw request to the DistortionStage's dynamic (non-persistent-on-clear) draw queue by specifying the request's components
        /// </summary>
        /// <param name="stage">DistortionStage reference</param>
        /// <param name="target">The coordinate space (world or screen) that the vertices should be transformed by</param>
        /// <param name="type">Fill / Texturing type - Coloured, Single Textured or Dual Textured</param>
        /// <param name="vertices">The vertices that make up the triangles to draw</param>
        /// <param name="indices">The indices that reference vertices that make up the triangles to draw</param>
        /// <param name="colour">An overall colour applied to all vertices at drawing</param>
        /// <param name="texture0">Primary Texture Reference (the only texture used for single texturing)</param>
        /// <param name="texture1">Secondary Texture Reference (used for dual texturing)</param>
        /// <param name="texWrap0">Primary Texture Coordinate Wrap behaviour</param>
        /// <param name="texWrap1">Secondary Texure Coordinate Wrap behaviour</param>
        /// <param name="intensity">Scalar quantity to multply the height factor of the drawn pixels</param>
        void DrawDistortion(IDistortionStage stage,
                    CoordinateSpace target,
                    FillType type,
                    Vertex2D[] vertices,
                    int[] indices,
                    Colour colour,
                    ITexture texture0,
                    ITexture texture1,
                    TextureCoordinateMode texWrap0,
                    TextureCoordinateMode texWrap1,
                    float intensity,
                    bool validate = false
                    );

        /// <summary>
        /// Add distortion draw request to the DistortionStage's dynamic (non-persistent-on-clear) draw queue by specifying the request's components
        /// </summary>
        /// <param name="stage">DistortionStage id</param>
        /// <param name="target">The coordinate space (world or screen) that the vertices should be transformed by</param>
        /// <param name="type">Fill / Texturing type - Coloured, Single Textured or Dual Textured</param>
        /// <param name="vertices">The vertices that make up the triangles to draw</param>
        /// <param name="indices">The indices that reference vertices that make up the triangles to draw</param>
        /// <param name="colour">An overall colour applied to all vertices at drawing</param>
        /// <param name="texture0">Primary Texture id (the only texture used for single texturing)</param>
        /// <param name="texture1">Secondary Texture id (used for dual texturing)</param>
        /// <param name="texWrap0">Primary Texture Coordinate Wrap behaviour</param>
        /// <param name="texWrap1">Secondary Texure Coordinate Wrap behaviour</param>
        /// <param name="intensity">Scalar quantity to multply the height factor of the drawn pixels</param>
        void DrawDistortion(ulong stage,
                    CoordinateSpace target,
                    FillType type,
                    Vertex2D[] vertices,
                    int[] indices,
                    Colour colour,
                    ulong texture0,
                    ulong texture1,
                    TextureCoordinateMode texWrap0,
                    TextureCoordinateMode texWrap1,
                    float intensity,
                    bool validate = false
                    );

        /// <summary>
        /// Create a draw request from a Text String and Font and add to the DrawStage's dynamic (non-persistent-on-clear) draw queue
        /// </summary>
        /// <param name="stage">DrawStage reference</param>
        /// <param name="target">The coordinate space (world or screen) that the vertices should be transformed by</param>
        /// <param name="text">Text string to draw</param>
        /// <param name="colour">Text colour</param>
        /// <param name="fontSize">Font size in pixels</param>
        /// <param name="position">Position of text string (x dependent on justify, y is top)</param>
        /// <param name="justify">Horizontal positioning of text relative to position</param>
        /// <param name="depth">Z depth of vertices defining ordering within a layer (0.0 [front] to 1.0 [back])</param>
        /// <param name="layer">>= 0. The layer that these vertices belong too, lower layers are drawn behind higher layers</param>
        /// <param name="font">Font reference to use, null uses a default font</param>
        /// <param name="useKerningsWhereAvaliable">Use font kerning data for spacing adjustments if font contains this data</param>
        void DrawString(IDrawStage stage,
                        CoordinateSpace target,
                        string text,
                        Colour colour,
                        float fontSize,
                        Vector2 position,
                        TextJustify justify,
                        float depth,
                        int layer,
                        IFont font = null,
                        bool useKerningsWhereAvaliable = true);

        /// <summary>
        /// Create a draw request from a Text String and Font and add to the DrawStage's dynamic (non-persistent-on-clear) draw queue
        /// </summary>
        /// <param name="stage">DrawStage id</param>
        /// <param name="target">The coordinate space (world or screen) that the vertices should be transformed by</param>
        /// <param name="text">Text string to draw</param>
        /// <param name="colour">Text colour</param>
        /// <param name="fontSize">Font size in pixels</param>
        /// <param name="position">Position of text string (x dependent on justify, y is top)</param>
        /// <param name="justify">Horizontal positioning of text relative to position</param>
        /// <param name="depth">Z depth of vertices defining ordering within a layer (0.0 [front] to 1.0 [back])</param>
        /// <param name="layer">>= 0. The layer that these vertices belong too, lower layers are drawn behind higher layers</param>
        /// <param name="font">Font id to use</param>
        /// <param name="useKerningsWhereAvaliable">Use font kerning data for spacing adjustments if font contains this data</param>
        void DrawString(ulong stage,
                        CoordinateSpace target,
                        string text,
                        Colour colour,
                        float fontSize,
                        Vector2 position,
                        TextJustify justify,
                        float depth,
                        int layer,
                        ulong font,
                        bool useKerningsWhereAvaliable = true);

        /// <summary>
        /// Create a draw request from a Text String and Font and add to the DrawStage's dynamic (non-persistent-on-clear) draw queue
        /// Uses default font
        /// </summary>
        /// <param name="stage">DrawStage id</param>
        /// <param name="target">The coordinate space (world or screen) that the vertices should be transformed by</param>
        /// <param name="text">Text string to draw</param>
        /// <param name="colour">Text colour</param>
        /// <param name="fontSize">Font size in pixels</param>
        /// <param name="position">Position of text string (x dependent on justify, y is top)</param>
        /// <param name="justify">Horizontal positioning of text relative to position</param>
        /// <param name="depth">Z depth of vertices defining ordering within a layer (0.0 [front] to 1.0 [back])</param>
        /// <param name="layer">>= 0. The layer that these vertices belong too, lower layers are drawn behind higher layers</param>
        /// <param name="useKerningsWhereAvaliable">Use font kerning data for spacing adjustments if font contains this data</param>
        void DrawString(ulong stage,
                        CoordinateSpace target,
                        string text,
                        Colour colour,
                        float fontSize,
                        Vector2 position,
                        TextJustify justify,
                        float depth,
                        int layer,
                        bool useKerningsWhereAvaliable = true);

        /// <summary>
        /// Measure pixel length (width) of a string
        /// </summary>
        /// <param name="text">Text string</param>
        /// <param name="fontSize">Size of font in pixels</param>
        /// <param name="font">Font reference to use, null uses a default font</param>
        float MeasureStringLength(string text,
                                  float fontSize,
                                  IFont font = null);

        /// <summary>
        /// Measure pixel length (width) of a string
        /// </summary>
        /// <param name="text">Text string</param>
        /// <param name="fontSize">Size of font in pixels</param>
        /// <param name="font">Font id to use, null uses a default font</param>
        float MeasureStringLength(string text,
                                  float fontSize,
                                  ulong font);

        /// <summary>
        /// Measure pixel length (width) of a string
        /// Uses default font
        /// </summary>
        /// <param name="text">Text string</param>
        /// <param name="fontSize">Size of font in pixels</param>
        float MeasureStringLength(string text,
                                  float fontSize);
    }
}