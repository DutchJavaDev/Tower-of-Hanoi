using System;
using System.Collections.Generic;
using System.Security.Policy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tower_of_Hanoi.Source
{
    public class HanoiBoard
    {
        private readonly AiSolver _aiSolver;
        private readonly HanoiTower[] _hanoiTowers;
        public string StepMessage;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly GraphicsDevice _graphicsDevice;
        public readonly Viewport DefaultViewport;

        internal HanoiDisk FocusHanoiDisk;
        internal HanoiTower FocusHanoiTower;

        public int MovesMade;

        public HanoiBoard(IReadOnlyList<Texture2D> texture2Ds,IGraphicsDeviceService graphics)
        {
            StepMessage = $@"Can be completed in {GetMinimalMoves(texture2Ds.Count-1)} moves.";
            _graphicsDevice = graphics.GraphicsDevice;
            DefaultViewport = _graphicsDevice.Viewport;
            

            // Tower placing
            var xPos = DefaultViewport.Width / 4;
            var yPos = DefaultViewport.Height - texture2Ds[0].Height;
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

            _aiSolver = new AiSolver(_hanoiTowers,this);
        }


        /// <summary>
        /// Update's some stuff
        /// </summary>
        /// <param name="mouse"></param>
        /// <param name="deltaTime"></param>
        public void UpdateHanoi(MouseState mouse,float deltaTime)
        {
            AiVersion(deltaTime);
        }


        /// <summary>
        /// Version for making the computer run the logic
        /// </summary>
        private void AiVersion(float delta)
        {
            _aiSolver.UpdateAi(delta);
        }


        /// <summary>
        /// Game mode for the player
        /// </summary>
        /// <param name="mouse"></param>
        // ReSharper disable once UnusedMember.Local
        private void PlayerVersion(MouseState mouse)
        {
            // If a disk has been selected run this to check if the player/user has clicked on a different tower
            // If the player/user has clicked on a different tower add the selected disk to that tower and remove it from the old one
            if (FocusHanoiDisk != null && FocusHanoiTower != null)
            {
                foreach (var tower in _hanoiTowers)
                {
                    if (!tower.InBounds(mouse.X, mouse.Y) || mouse.LeftButton != ButtonState.Pressed || FocusHanoiTower == tower) continue;

                    if (!tower.CanAdd(FocusHanoiDisk.Size.Width))
                    {
                        FocusHanoiDisk = null;
                        FocusHanoiTower = null;
                        break;
                    }

                    var oldId = FocusHanoiDisk.IndexId;

                    tower.AddDisk(FocusHanoiDisk);

                    FocusHanoiTower.RemoveDisk(oldId);

                    // Keeps track of the moves the player/user has made
                    MovesMade++;

                    FocusHanoiDisk = null;
                    FocusHanoiTower = null;
                    break;
                }
            }

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
        /// <param name="fonts"></param>
        public void RenderHanoi(SpriteBatch batch,SpriteFont[] fonts)
        {
            batch.Begin();
            foreach (var tower in _hanoiTowers)
            {
                tower.DrawTower(batch,fonts);
            }
            var stepmessageSize = fonts[0].MeasureString(StepMessage);

            batch.DrawString(fonts[0],StepMessage,new Vector2
            {
                X = DefaultViewport.Width / 2f  - stepmessageSize.X / 2f,
                Y = DefaultViewport.Height / 10f
            }, Color.AliceBlue);

            var movesMade = $@"Moves made: {MovesMade}";
            var movesMadeSize = fonts[0].MeasureString(movesMade);

            batch.DrawString(fonts[0], movesMade,new Vector2
            {
                X = DefaultViewport.Width / 2f - movesMadeSize.X / 2f,
                Y = DefaultViewport.Height / 10f * 2
            }, Color.AliceBlue);


            // Enable on user-version for debugging
            //batch.DrawString(fonts[1], $"ID:{FocusHanoiDisk?.IndexId ?? -1}",new Vector2(), Color.Red);

            _aiSolver.RenderAi(batch,fonts[0]);
            batch.End();
        }


        /// <summary>
        /// Calculates the minimal moves needed to complete a session
        /// </summary>
        /// <param name="disks"></param>
        /// <returns></returns>
        public static int GetMinimalMoves(int disks)
        {
            return (int)Math.Pow(2.0, disks) - 1;
        }

    }
}
