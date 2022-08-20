using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Casino
{
    public class Camera
    {
        public Matrix Transform { get; private set; }
        public Vector2 Position;

        public void Follow(Player target)
        {
            var position = Matrix.CreateTranslation(-target.position.X - (target.storage.width / 2), -target.position.Y - (target.storage.heigth / 2), 0);
            var offset = Matrix.CreateTranslation(Game1.UserScreenWidth / 2, Game1.UserScreenHeight / 2, 0);
            Position = target.position;
            Transform = position * offset;
        }
    }
}
