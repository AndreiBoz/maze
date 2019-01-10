﻿/**************************************************************************
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

namespace Reactive
{
    public class Utils
    {
        public static int Lines { get; private set; }
        public static int Columns { get; private set; }
        public static Directions[,] Maze { get; private set; }
        public static ExplorerDirections[,] ExplorerMaze { get; private set; }
        public static int NoExplorers = 5;
        public static int NoResources = 0;
        // Maze exit
        public static int XExit { get; private set; }
        public static int YExit { get; private set; }
        public static string ExitDirection { get; private set; }

        public static int Delay = 7;
        public static Random RandNoGen = new Random();

        public Utils(int lines, int colums, Directions[,] maze, ExplorerDirections[,] explorerMaze, int xE, int yE, string dE)
        {
            Lines = lines;
            Columns = colums;
            Maze = maze;
            XExit = xE;
            YExit = yE;
            ExitDirection = dE;
            ExplorerMaze = explorerMaze;
        }
        public static void ParseMessage(string content, out string action, out List<string> parameters)
        {
            string[] t = content.Split();

            action = t[0];

            parameters = new List<string>();
            for (int i = 1; i < t.Length; i++)
                parameters.Add(t[i]);
        }

        public static void ParseMessage(string content, out string action, out string parameters)
        {
            string[] t = content.Split();

            action = t[0];

            parameters = "";

            if (t.Length > 1)
            {
                for (int i = 1; i < t.Length - 1; i++)
                    parameters += t[i] + " ";
                parameters += t[t.Length - 1];
            }
        }

        public static string Str(object p1, object p2)
        {
            return string.Format("{0} {1}", p1, p2);
        }

        public static string Str(object p1, object p2, object p3)
        {
            return string.Format("{0} {1} {2}", p1, p2, p3);
        }

    }

}