using Veldrid;

namespace Yak2D.Graphics
{
    public class BlendStateConverter : IBlendStateConverter
    {
        public BlendStateDescription Convert(BlendState state)
        {
            switch (state)
            {
                case BlendState.Override:
                    return BlendStateDescription.SingleOverrideBlend;
                case BlendState.Alpha:
                    return BlendStateDescription.SingleAlphaBlend;
                case BlendState.AdditiveAlpha:
                    return BlendStateDescription.SingleAdditiveBlend;
                case BlendState.AdditiveComponentWise:
                    return new BlendStateDescription(
                        RgbaFloat.Black,
                        new BlendAttachmentDescription(
                            true,
                            BlendFactor.One,
                            BlendFactor.One,
                            BlendFunction.Add,
                            BlendFactor.One,
                            BlendFactor.One,
                            BlendFunction.Add
                        ));
            }

            return BlendStateDescription.SingleAlphaBlend;
        }
    }
}