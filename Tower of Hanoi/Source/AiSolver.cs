﻿
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tower_of_Hanoi.Source
{
    internal class AiSolver
    {
        private readonly HanoiBoard _hanoiBoard;
        private readonly HanoiTower _a; // left tower
        private readonly HanoiTower _b; // center tower
        private readonly HanoiTower _c; // right tower
        private readonly bool _oddAmount; // Desides wich type of logic we are running

        private const float UpdateOn = 1.5f; // Make a move every 1.5 seconds
        private float _updateCounter; // Keeps count how much move-updates have passed
        private bool _move1, _move2, _move3;
        

        public AiSolver(IReadOnlyList<HanoiTower> towers,HanoiBoard board)
        {
            _hanoiBoard = board;

            _a = towers[0];
            _b = towers[1];
            _c = towers[2];

            _a.Id = "A";
            _b.Id = "B";
            _c.Id = "C";

            // Check if the diskount is even or oneven
            // if true amount is odd: 1,3,5,7,9
            // if false amount is even: 2,4,8,10
            _oddAmount = _a.DiskCount % 2 != 0;

            board.StepMessage = $"Can be solved in: {HanoiBoard.GetMinimalMoves(_a.DiskCount)} moves || {(_oddAmount ? "Odd" : "Even")} amount of disk's";
        }


        /// <summary>
        /// Checks if it needs to update the ai
        /// </summary>
        /// <param name="deltaTime"></param>
        public void UpdateAi(float deltaTime)
        {
            if ( _updateCounter >= UpdateOn )
            {
                _updateCounter = 0;

                // Run a single update
                if (_oddAmount)
                {
                    OddLogic();
                }
                else
                {
                    EvenLogic();
                }
            }
            else
            {
                _updateCounter += 1f * deltaTime;
            }
        }


        /// <summary>
        /// <para>Logic for odd numbers like 3,5,7,9,11 and so on</para>
        /// </summary>
        private void OddLogic()
        {

            if (_move1 && _move2 && _move3) // All moves have been done time to reset
            {
                _move1 = false;
                _move2 = false;
                _move3 = false;
            }

            if (!_move1)
            {
                // make the legal move between tower A and C (in either direction),
                LegalMove(_a, _c);
                _move1 = !_move1;
                return;
            }

            if (!_move2)
            {
                // make the legal move between tower A and B (in either direction),
                LegalMove(_a, _b);
                _move2 = !_move2;
                return;
            }

            if ( !_move3 )
            {
                // make the legal move between tower B and C (in either direction),
                LegalMove(_b, _c);
                _move3 = !_move3;
            }
        }


        /// <summary>
        /// <para>Logic for even numbers like 2,4,6,8,10 and so on</para>
        /// </summary>
        private void EvenLogic()
        {

            if ( _move1 && _move2 && _move3 )
            {
                _move1 = false;
                _move2 = false;
                _move3 = false;
            }

            if ( !_move1 )
            {
                // make the legal move between tower A and B (in either direction),
                LegalMove(_a, _b);
                _move1 = !_move1;
                return;
            }

            if ( !_move2 )
            {
                // make the legal move between tower A and C (in either direction),
                LegalMove(_a, _c);
                _move2 = !_move2;
                return;
            }

            if ( _move3 ) return;
            // make the legal move between tower B and C (in either direction),
            LegalMove(_b, _c);
            _move3 = !_move3;
        }


        /// <summary>
        /// Renders some stuff
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="font"></param>
        public void RenderAi(SpriteBatch batch,SpriteFont font)
        {
            var stepmessageSize = font.MeasureString(_hanoiBoard.StepMessage);

            batch.DrawString(font, _hanoiBoard.StepMessage, new Vector2
            {
                X = _hanoiBoard.DefaultViewport.Width / 2f - stepmessageSize.X / 2f,
                Y = _hanoiBoard.DefaultViewport.Height / 10f
            }, Color.AliceBlue);
        }


        /// <summary>
        /// <para>Checks for legal moves between source-tower and the target-tower.</para>
        /// <para>If it fails to make a move from source-tower to target-tower it will try to make a move from target-tower to source-tower</para>
        /// </summary>
        /// <param name="sourceTower"></param>
        /// <param name="targetTower"></param>
        private void LegalMove(HanoiTower sourceTower, HanoiTower targetTower)
        {
            HanoiDisk topDisk;
            int id;

            // Check if source-tower has disks
            if ( sourceTower.DiskCount > 0 )
            {
              
                // get the top disk from the source-tower and its id
                topDisk = sourceTower.GetTopDisk();
                id = topDisk.IndexId;

                // check if the disk can be added to the target tower
                if ( targetTower.CanAdd(topDisk.Size.Width) )
                {
                    sourceTower.RemoveDisk(id);
                    targetTower.AddDisk(topDisk);

                    // Keeps track of the moves the ai has used
                    _hanoiBoard.MovesMade++;
                    Console.WriteLine($"Added: {id} from {sourceTower.Id} to {targetTower.Id}");
                }
                else
                {
                    // Move from target-tower to the source-tower
                    topDisk = targetTower.GetTopDisk();
                    id = topDisk.IndexId;
                    targetTower.RemoveDisk(id);
                    sourceTower.AddDisk(topDisk);

                    // Keeps track of the moves the ai has used
                    _hanoiBoard.MovesMade++;
                    Console.WriteLine($"Added: {id} from {sourceTower.Id} to {targetTower.Id}");
                }

            }
            // If the source tower does nat have any disk then move fro target to source
            else if ( targetTower.DiskCount > 0 )
            {
                topDisk = targetTower.GetTopDisk();
                id = topDisk.IndexId;

                targetTower.RemoveDisk(id);
                sourceTower.AddDisk(topDisk);

                // Keeps track of the moves the ai has used
                _hanoiBoard.MovesMade++;
                Console.WriteLine($"Added: {id} from {sourceTower.Id} to {targetTower.Id}");
            }
            else
            {
                // Unknown error
                Console.WriteLine("Completed");
            }
        }
    }
}
