using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpriteFontPlus.Ttf
{
    public class FontBakerResult
    {
        public Dictionary<char, GlyphInfo> Glyphs { get; private set; }
        public byte[] Pixels { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public FontBakerResult(Dictionary<char, GlyphInfo> glyphs,
            byte[] pixels,
            int width,
            int height)
        {
            if (glyphs == null)
            {
                throw new ArgumentNullException(nameof(glyphs));
            }

            if (pixels == null)
            {
                throw new ArgumentNullException(nameof(pixels));
            }

            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width));
            }
            
            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            if (pixels.Length < width * height)
            {
                throw new ArgumentException("pixels.Length should be higher than width * height");
            }

            Glyphs = glyphs;
            Pixels = pixels;
            Width = width;
            Height = height;
        }

        public SpriteFont CreateSpriteFont(GraphicsDevice graphicsDevice)
        {
            var rgb = new Color[Width * Height];
            for (var i = 0; i < Pixels.Length; ++i)
            {
                var b = Pixels[i];
                rgb[i].R = b;
                rgb[i].G = b;
                rgb[i].B = b;
				
                rgb[i].A = b;
            }

           var  texture = new Texture2D(graphicsDevice, Width, Height);
            texture.SetData(rgb);

            var glyphBounds = new List<Rectangle>();
            var cropping = new List<Rectangle>();
            var chars = new List<char>();
            var kerning = new List<Vector3>();

            var orderedKeys = Glyphs.Keys.OrderBy(a => a);
            foreach (var key in orderedKeys)
            {
                var character = Glyphs[key];

                var bounds = new Rectangle(character.X0, character.Y0, 
                    character.X1 - character.X0,
                    character.Y1 - character.Y0);

                glyphBounds.Add(bounds);
                cropping.Add(new Rectangle((int)character.XOffset, (int)character.YOffset, bounds.Width, bounds.Height));

                chars.Add(key);

                kerning.Add(new Vector3(0, bounds.Width, character.XAdvance - bounds.Width));
            }

            var constructorInfo = typeof(SpriteFont).GetTypeInfo().DeclaredConstructors.First();
            var font = (SpriteFont) constructorInfo.Invoke(new object[]
            {
                texture, glyphBounds, cropping,
                chars, 20, 0, kerning, ' '
            });

            return font;
        }
    }
}