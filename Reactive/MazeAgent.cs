/**************************************************************************
 *                                                                        *
 *  Website:     https://github.com/florinleon/ActressMas                 *
 *  Description: The reactive architecture using the ActressMas framework *
 *  Copyright:   (c) 2018, Florin Leon                                    *
 *                                                                        *
 *  This program is free software; you can redistribute it and/or modify  *
 *  it under the terms of the GNU General Public License as published by  *
 *  the Free Software Foundation. This program is distributed in the      *
 *  hope that it will be useful, but WITHOUT ANY WARRANTY; without even   *
 *  the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR   *
 *  PURPOSE. See the GNU General Public License for more details.         *
 *                                                                        *
 **************************************************************************/

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace Reactive
{
    public class MazeAgent : ActressMas.Agent
    {
        private MazeForm _formGui;
        public Dictionary<string, string> ExplorerPositions { get; set; }
       
        public MazeAgent()
        {
            ExplorerPositions = new Dictionary<string, string>();

            Thread t = new Thread(new ThreadStart(GUIThread));
            t.Start();
        
        }

        private void GUIThread()
        {
            _formGui = new MazeForm();
            _formGui.SetOwner(this);
            _formGui.ShowDialog();
            Application.Run();
        }

        public override void Setup()
        {
            Console.WriteLine("Starting " + Name);
        }

        public override void Act(ActressMas.Message message)
        {
            try
            {
                Console.WriteLine("\t[{1} -> {0}]: {2}", this.Name, message.Sender, message.Content);

                string action; string parameters;
                Utils.ParseMessage(message.Content, out action, out parameters);

               
                HandleDirection(message.Sender, parameters, action);

                _formGui.UpdateMazeGUI();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void HandleDirection(string sender, string position, string action)
        {
            ExplorerPositions[sender] = position;
            // Check for exit
            if (ExitFound(position, action))
            {
                Send(sender, "exit");
            }
            
            // Check walls
            if (WallFound(position,action))
            {
                Send(sender, "wall");
            }
            // Pass
            Send(sender, "pass");
        }

        private bool WallFound(string position, string direction)
        {
            string[] pos = position.Split();

            int x = Convert.ToInt32(pos[1]);
            int y = Convert.ToInt32(pos[0]);
            
            switch(direction)
            {
                case "up":
                    return Utils.Maze[x, y].up;
                case "down":
                    return Utils.Maze[x, y].down;
                case "left":
                    return Utils.Maze[x, y].left;
                case "right":
                    return Utils.Maze[x, y].right;
                default:
                    return false;
            }
        }

        private bool ExitFound(string position, string direction)
        {
            string[] pos = position.Split();

            int x = Convert.ToInt32(pos[0]);
            int y = Convert.ToInt32(pos[1]);

            return x == Utils.XExit && y == Utils.YExit && direction == Utils.ExitDirection;  
        }

    }
}