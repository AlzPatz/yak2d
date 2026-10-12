using NeoVeldrid;

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
                    //Not BlendStateDescription.SingleAlphaBlend: that also multiplies the alpha channel by
                    //source alpha, so semi-transparent pixels drawn onto a transparent target end up with alpha²
                    return new BlendStateDescription(
                        RgbaFloat.Black,
                        new BlendAttachmentDescription(
                            true,
                            BlendFactor.SourceAlpha,
                            BlendFactor.InverseSourceAlpha,
                            BlendFunction.Add,
                            BlendFactor.One,
                            BlendFactor.InverseSourceAlpha,
                            BlendFunction.Add
                        ));
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
                case BlendState.Premultiplied:
                    return new BlendStateDescription(
                        RgbaFloat.Black,
                        new BlendAttachmentDescription(
                            true,
                            BlendFactor.One,
                            BlendFactor.InverseSourceAlpha,
                            BlendFunction.Add,
                            BlendFactor.One,
                            BlendFactor.InverseSourceAlpha,
                            BlendFunction.Add
                        ));
            }

            return BlendStateDescription.SingleAlphaBlend;
        }
    }
}