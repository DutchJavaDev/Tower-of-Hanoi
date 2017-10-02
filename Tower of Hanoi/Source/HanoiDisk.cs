using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Tower_of_Hanoi.Source
{
    internal class HanoiDisk
    {
        public Rectangle Size { get; }
        public int IndexId { get; set; }

        private readonly Texture2D _diskTexture;
        private Vector2 _position;
        private HanoiTower _parenTower;


        public HanoiDisk(Texture2D texture)
        {
            _diskTexture = texture;
            _position = new Vector2();
            Size = new Rectangle(0,0,_diskTexture.Width,_diskTexture.Height);
        }


        /// <summary>
        /// Checks if the mouse is inside this disk and also if the left mouse button is pressed
        /// </summary>
        /// <param name="mouseX"></param>
        /// <param name="mouseY"></param>
        /// <param name="leftClick"></param>
        public void CheckBounds(int mouseX,int mouseY,bool leftClick)
        {
            var x = _position.X;
            var y = _position.Y;
            var width = Size.Width;
            var height = Size.Height;

            // If the mouse is inside the region/bounds of this disk && left mouse button is down do some somthing
            if ( !(mouseX >= x) || !(mouseX <= x + width) ) return;
            if ( !(mouseY >= y) || !(mouseY <= y + height) ) return;

            if ( leftClick && _parenTower.CanMove(IndexId))
            {
               _parenTower.SetFocus(IndexId);
            }
        }

        
        /// <summary>
        /// Set the position lol
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="parent"></param>
        public void SetPosition(Vector2 pos,HanoiTower parent)
        {
            _position = pos;
            _parenTower = parent;
        }


        /// <summary>
        /// Draws some stuff
        /// </summary>
        /// <param name="batch"></param>
        public void RenderDisk(SpriteBatch batch,SpriteFont font)
        {
           batch.Draw(_diskTexture,_position,Size,Color.AliceBlue);
           batch.DrawString(font,$@"{IndexId}",_position,Color.Beige);
        }

    }
}
