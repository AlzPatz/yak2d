using System.Collections.Generic;

namespace Yak2D.Internal
{
    public class SubFont
    {
        public float Size { get; private set; }
        public float LineHeight { get; private set; }
        public Dictionary<char, FontCharacter> Characters { get; private set; }
        public ITexture[] Textures { get; private set; }
        public bool HasKernings { get; private set; }
        public Dictionary<char, Dictionary<char, short>> Kernings { get; private set; }

        public SubFont(float size,
                       float lineHeight,
                       Dictionary<char, FontCharacter> characters,
                       ITexture[] textures,
                       bool hasKernings,
                       Dictionary<char, Dictionary<char, short>> kernings)
        {
            Size = size;
            LineHeight = lineHeight;
            Characters = characters;
            Textures = textures;
            HasKernings = hasKernings;
            Kernings = kernings;
        }
    }
}