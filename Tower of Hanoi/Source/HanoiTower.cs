using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tower_of_Hanoi.Source
{
    internal class HanoiTower
    {
        private readonly Texture2D _texture2D;
        private readonly HanoiDisk[] _hanoiDisks = new HanoiDisk[4];
        private readonly HanoiBoard _hanoiBoard;

        private Rectangle Size { get;}
        private Vector2 Position { get;}

        public int DiskCount => _hanoiDisks.Count(disk => disk != null);
        public string Id;

        public HanoiTower(Texture2D texture,Vector2 pos,HanoiBoard board)
        {
            _hanoiBoard = board;
            _texture2D = texture;
            Position = pos;
            Size = new Rectangle(0,0,_texture2D.Width,_texture2D.Height);
        }


        /// <summary>
        /// Returns true if the mouse is inside the tower
        /// </summary>
        /// <param name="mouseX"></param>
        /// <param name="mouseY"></param>
        /// <returns>aliens</returns>
        public bool InBounds(int mouseX,int mouseY)
        {
            return mouseX >= Position.X && mouseX <= Position.X + Size.Width && mouseY >= Position.Y && mouseY <= Position.Y + Size.Height;
        }


        /// <summary>
        /// Add's a disk 
        /// </summary>
        /// <param name="disk"></param>
        public void AddDisk(HanoiDisk disk)
        {
            for (var index = 0; index < _hanoiDisks.Length; index++)
            {
                if ( _hanoiDisks[index] != null ) continue;
                _hanoiDisks[index] = disk;
                disk.IndexId = index;
                _hanoiDisks[index].SetPosition(GetPosition(_hanoiDisks[index]),this);
                break;
            }
        }


        /// <summary>
        /// Removes a disk
        /// </summary>
        /// <param name="index"></param>
        public void RemoveDisk(int index)
        {
            _hanoiDisks[index] = null;
        }


        /// <summary>
        /// Notify the main board that the player/user has selected an item
        /// </summary>
        /// <param name="diskIndex"></param>
        public void SetFocus(int diskIndex)
        {
            _hanoiBoard.FocusHanoiDisk = _hanoiDisks[diskIndex];
            _hanoiBoard.FocusHanoiTower = this;
        }


        /// <summary>
        /// Updates the child disk located in this tower
        /// </summary>
        /// <param name="state"></param>
        public void UpdateTower(MouseState state)
        {
            foreach (var disk in _hanoiDisks)
            {
                disk?.CheckBounds(state.X,state.Y,state.LeftButton == ButtonState.Pressed);
            }
        }


        /// <summary>
        /// Renders some stuff
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="fonts"></param>
        public void DrawTower(SpriteBatch batch,SpriteFont[] fonts)
        {
            batch.Draw(_texture2D,Position,Size,Color.AliceBlue);

            foreach (var disk in _hanoiDisks)
            {
                disk?.RenderDisk(batch,fonts);
            }

            batch.DrawString(fonts[0],Id,Position,Color.Blue);
        }


        /// <summary>
        /// Checks if there are no disk above the current disk
        /// <para>returns true if the are no above disk's</para>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool CanMove(int index)
        {
            // When i implement that the disk size can change i would need to re-write this methode
            // because it olny supports a max of 4 disks
            switch (index)
            {
                case 0:
                    if (_hanoiDisks[1] == null && _hanoiDisks[2] == null && _hanoiDisks[3] == null)
                        return true;
                    break;

                case 1:
                    if (_hanoiDisks[2] == null && _hanoiDisks[3] == null)
                        return true;
                    break;

                case 2:
                    if (_hanoiDisks[3] == null)
                        return true;
                    break;

                case 3:
                    // This is th top most disk
                    return true;

                default:
                    return false;
            }

            return false;
        }

        /// <summary>
        /// Checks if there is a small disk located on the tower 
        /// <para>If there is a small disk then it returns false</para>
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public bool CanAdd(int width)
        {
            // if 0 = null return true bebause there are no items in the array
            if ( _hanoiDisks[0] == null ) return true;

            for (var index = 0; index < _hanoiDisks.Length; index++)
            {
                if ( _hanoiDisks[index] != null ) continue;

                var topdisk = _hanoiDisks[index - 1];

                var topdiskWidth = topdisk.Size.Width;
                
                return topdiskWidth > width;
            }

            return false;
        }


        /// <summary>
        /// Gets a positon in wich the disk can be placed
        /// </summary>
        /// <param name="disk"></param>
        /// <returns></returns>
        private Vector2 GetPosition(HanoiDisk disk)
        {
            // 0 is the bottom because of the rendering order

            // 0 = 1/4 bottom
            // 1 = 2/4 i am not stupid, but i cant figure a word for this location: bottom-center? 
            // 2 = 3/4 i am not stupid, but i cant figure a word for this location: top-center?
            // 3 = 4/4 top
            var pos = new Vector2();

            switch (disk.IndexId)
            {
                case 0:
                    pos.X = Position.X + Size.Width / 2f - disk.Size.Width / 2f;
                    pos.Y = Position.Y + Size.Height - disk.Size.Height;
                    break;

                case 1:
                    pos.X = Position.X + Size.Width / 2f - disk.Size.Width / 2f;
                    pos.Y = Position.Y + Size.Height - disk.Size.Height * 2;
                    break;

                case 2:
                    pos.X = Position.X + Size.Width / 2f - disk.Size.Width / 2f;
                    pos.Y = Position.Y + Size.Height - disk.Size.Height * 3;
                    break;


                case 3:
                    pos.X = Position.X + Size.Width / 2f - disk.Size.Width / 2f;
                    pos.Y = Position.Y + Size.Height - disk.Size.Height * 4;
                    break;

                default:
                    pos.X = 0;
                    pos.Y = 0;
                    break;
            }

            return pos;
        }


        /// <summary>
        /// Returns the disk that is located at the top
        /// </summary>
        /// <returns></returns>
        public HanoiDisk GetTopDisk()
        {
            for (var index = _hanoiDisks.Length; index > 0; index--)
            {
                if ( _hanoiDisks[index-1] != null ) return _hanoiDisks[index-1];
            }
            return null;
        }

    }
}
