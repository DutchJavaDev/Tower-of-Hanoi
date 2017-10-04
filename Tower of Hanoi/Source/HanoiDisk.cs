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

        private string _indexString;
        private Vector2 _indexVector;


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
            // If the mouse is inside the region/bounds of this disk && left mouse button is down do some somthing
            if ( !(mouseX >= _position.X) || !(mouseX <= _position.X + Size.Width) ) return;
            if ( !(mouseY >= _position.Y) || !(mouseY <= _position.Y + Size.Height) ) return;

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

            _indexString = $"{IndexId}";
        }


        /// <summary>
        /// Draws some stuff
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="fonts"></param>
        public void RenderDisk(SpriteBatch batch,SpriteFont[] fonts)
        {
           batch.Draw(_diskTexture,_position,Size,Color.AliceBlue);


            // For debugging
            var size = fonts[1].MeasureString(_indexString);
            _indexVector = new Vector2
            {
                X = _position.X + Size.Width / 2f - size.X / 2f,
                Y = _position.Y + Size.Height / 2f - size.Y / 2f
            };
           batch.DrawString(fonts[1],_indexString,_indexVector, Color.DarkRed);
        }

    }
}
