using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cannon3
{
    class HUDItem
    {
        private Point position;
        private Texture2D texture;
        private int width;
        private int height;

        #region Properties

        public Point Position
        {
            get { return position; }
            set { position = value; }
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        #endregion

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 positionVector = new Vector2(this.position.X, this.position.Y);
            Vector2 originVector = Vector2.Zero;
            Rectangle sourceRectangle = new Rectangle(0, 0, this.width, this.height);

            spriteBatch.Draw(texture,
                             positionVector,
                             sourceRectangle,
                             Color.White,
                             0.0f,
                             originVector,
                             1.0f,
                             SpriteEffects.None,
                             0.5f);
        }
    }
}
