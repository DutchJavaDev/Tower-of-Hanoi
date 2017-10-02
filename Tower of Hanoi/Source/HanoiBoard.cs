using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tower_of_Hanoi.Source
{
    public class HanoiBoard
    {
        private readonly HanoiTower[] _hanoiTowers;
        private readonly int _diskSteps;

        internal HanoiDisk FocusHanoiDisk;
        internal HanoiTower FocusHanoiTower;

        public HanoiBoard(IReadOnlyList<Texture2D> texture2Ds,IGraphicsDeviceService graphics)
        {
            _diskSteps = GetMinimalMoves(texture2Ds.Count - 1);
            
            var xPos = graphics.GraphicsDevice.Viewport.Width / 4;
            var yPos = graphics.GraphicsDevice.Viewport.Height - texture2Ds[0].Height;
            var spacing = texture2Ds[3].Width / 4;

            // 0 = tower
            // 1 = topdisk
            // 2 = smalldisk
            // 3 = mediumdisk
            // 4 = bigdisk

            //Towers
            _hanoiTowers = new[]
            {
                new HanoiTower(texture2Ds[0],new Vector2(xPos - spacing,yPos),this),

                new HanoiTower(texture2Ds[0],new Vector2(xPos * 2,yPos),this),

                new HanoiTower(texture2Ds[0],new Vector2(xPos * 3 + spacing,yPos),this) 
            };

            //Disks
            _hanoiTowers[0].AddDisk(new HanoiDisk(texture2Ds[4]));
            _hanoiTowers[0].AddDisk(new HanoiDisk(texture2Ds[3]));
            _hanoiTowers[0].AddDisk(new HanoiDisk(texture2Ds[2]));
            _hanoiTowers[0].AddDisk(new HanoiDisk(texture2Ds[1]));
        }


        public void UpdateHanoi(MouseState mouse)
        {
            // If a disk has been selected run this to check if the player/user has clicked on a different tower
            // If the player/user has clicked on a different tower add the selected disk to that tower and remove it from the old one
            if ( FocusHanoiDisk != null && FocusHanoiTower != null )
            {
                foreach (var tower in _hanoiTowers)
                {
                    if ( !tower.InBounds(mouse.X, mouse.Y) || mouse.LeftButton != ButtonState.Pressed ||
                         FocusHanoiTower == tower ) continue;

                    var oldId = FocusHanoiDisk.IndexId;

                    tower.AddDisk(FocusHanoiDisk);
                        
                    FocusHanoiTower.RemoveDisk(oldId);

                    FocusHanoiDisk = null;
                    FocusHanoiTower = null;
                    return;
                }
            }
            else
            // If no disk has been selected update the towers to check if the player/user has clicked on a disk
            foreach (var tower in _hanoiTowers)
            {
                tower.UpdateTower(mouse);
            }

        }


        /// <summary>
        /// Renders some stuff
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="font"></param>
        public void RenderHanoi(SpriteBatch batch,SpriteFont font)
        {
            batch.Begin();
            foreach (var tower in _hanoiTowers)
            {
                tower.DrawTower(batch,font);
            }
            batch.DrawString(font,$@"Can be completed in {_diskSteps} moves.",new Vector2(), Color.AliceBlue);
            batch.DrawString(font,$@"ID: {FocusHanoiDisk?.IndexId ?? -1}",new Vector2(0,24),Color.AliceBlue);
            batch.End();
        }


        /// <summary>
        /// Calculates the minimal moves needed to complete a session
        /// </summary>
        /// <param name="disks"></param>
        /// <returns></returns>
        private static int GetMinimalMoves(int disks)
        {
            return (int)Math.Pow(2.0, disks) - 1;
        }

    }
}
