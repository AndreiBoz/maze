using System;

namespace Reactive
{
    public struct Directions
    {
        public bool up;
        public bool down;
        public bool left;
        public bool right;

        public Directions(bool up, bool down, bool left, bool right)
        {
            this.up = up;
            this.down = down;
            this.left = left;
            this.right = right;
        }

    }

    public class MazeGenerator
    {
        private Random _gen;
        private Utils _utils;
        
        public int Lines { get; } = 21;
        public int Columns { get; } = 23;

        public int XExit { get; private set; }
        public int YExit { get; private set; }
        public string DirectionExit { get; private set; }

        public Directions[,] Maze { get; private set; }

        public MazeGenerator()
        {
            _gen = new Random();

            Init(Lines, Columns);
        }
        
        private void Init(int lines, int columns)
        {
            XExit = GenerateRand(lines);
            YExit = GenerateRand(columns);

            DirectionExit = GenerateRandDirection();

            Maze = GenerateMaze(Lines, Columns);
            _utils = new Utils(Lines, Columns, Maze,XExit,YExit,DirectionExit);
        }

        private Directions[,] GenerateMaze(int lines, int columns)
        {
            //determine orientation
            var mazeOrientation = GetOrientation(lines, columns);

            //generate initial maze
            var generate = GenerateDivision(new Directions[lines, columns], 0, 0, Lines, Columns, mazeOrientation);
            //var generate = GenerateRandomMaze();
            //attach maze borders 
            var borderedMaze = AttachBorders(generate);

            //upgrade walls
            var walledMaze = UpgradeWalls(borderedMaze);

            //create maze exit
            var finalMaze = CreateExit(walledMaze, false, false);

            // Maze
            return finalMaze;
        }

        public Directions[,] GenerateDivision(Directions[,] subdivision, int x, int y, int width, int height, string direction)
        {
            // Can`t create another subdivision
            if (width < 2 || height < 2)
            {
                return subdivision;
            }
            // set orientation depending by section width and height
            bool orientation = direction == "H";

            // set a starting point 
            int wx = orientation ? x + 0 : x + _gen.Next(width - 2);
            int wy = orientation ? y + _gen.Next(height - 2) : y + 0;

            // set length and direction section 
            int length = orientation ? width : height;
            int dx = orientation ? 1 : 0;
            int dy = orientation ? 0 : 1;
            // set random exit / make a pass 
            int removeX = orientation ? wx + _gen.Next(width) : wx;
            int removeY = orientation ? wy + _gen.Next(height) : wy;

            // "draw" the wall
            for (int i = 0; i < length; i++)
            {
                if (orientation)
                {
                    // set vertical "wall" if pass not found 
                    subdivision[wx, wy].right = (wx != removeX) ? true : false;
                    wx += dx;
                    wy += dy;
                }
                else
                {
                    // set horizontal "wall" if pass not found
                    subdivision[wx, wy].up = (wy != removeY) ? true : false;
                    wx += dx;
                    wy += dy;
                }
            }

            // set starting point, width, height for a new subdivision (fist section) 
            int nx = x;
            int ny = y;
            int w = orientation ? width : wx - x + 1;
            int h = orientation ? wy - y + 1 : height;

            GenerateDivision(subdivision, nx, ny, w, h, GetOrientation(w, h));

            // set starting point, width, height for a new subdivision (second section)
            nx = orientation ? x : wx + 1;
            ny = orientation ? wy + 1 : y;
            w = orientation ? width : x + width - wx - 1;
            h = orientation ? y + height - wy - 1 : height;

            GenerateDivision(subdivision, nx, ny, w, h, GetOrientation(w, h));
 
            return subdivision;
        }

        /**
         * Genrate random maze (secondary option)
         **/ 
        public Directions[,] GenerateRandomMaze()
        {
            Directions[,] chamber = new Directions[Lines, Columns];

            for (int i = 0; i < Lines; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    chamber[i, j] = new Directions(RandWall(), RandWall(), RandWall(), RandWall());
                }
            }
            return UpgradeWalls(AttachBorders(chamber));
        }

        /**
         * Attach borders to a maze
         * 
         * Add wall margins
        **/ 
        public Directions[,] AttachBorders(Directions[,] maze)
        {
            for (int i = 0; i < Lines; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    maze[0, j].up = true; // fill upper border
                    maze[i, 0].left = true; // fill left border
                    maze[Lines - 1, j].down = true; // fill bottom border
                    maze[i, Columns - 1].right = true; // fill right border
                }
            }
            return maze;
        }

        /*
         * This method complete "wall"
         * 
         * A complete "wall" is composed of two opposite directions
        **/
        public Directions[,] UpgradeWalls(Directions[,] maze)
        {
            for (int i = 0; i < Lines; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (maze[i, j].up && HasNeighbour(i, j, "up")) AddWallToNeighbour(i, j, maze, "up");
                    if (maze[i, j].down && HasNeighbour(i, j, "down")) AddWallToNeighbour(i, j, maze, "down");
                    if (maze[i, j].left && HasNeighbour(i, j, "left")) AddWallToNeighbour(i, j, maze, "left");
                    if (maze[i, j].right && HasNeighbour(i, j, "right")) AddWallToNeighbour(i, j, maze, "right");
                }
            }
            return maze;
        }

        /*
         *  This method check if a cell neighbours(up,down,left,right) do not exceed boundary limits
        **/ 
        public bool HasNeighbour(int x, int y,string direction)
        {
            switch (direction)
            {
                case "up":
                    if (x == 0 || y == 0) Console.WriteLine("Check upper neighbours for: [{0}][{1}] Result:{2}", x, y, !(x - 1 < 0));
                    return !(x - 1 < 0);
                case "down":
                    if (x == 0 || y == 0) Console.WriteLine("Check bottom neighbours for: [{0}][{1}] Result:{2}", x, y, !(x + 1 == Lines));
                    return !(x + 1 == Lines);
                case "left":
                    if (x == 0 || y == 0) Console.WriteLine("Check left neighbours for: [{0}][{1}] Result:{2}", x, y, !(y - 1 < 0));
                    return !(y - 1 < 0);
                case "right":
                    if (x == 0 || y == 0) Console.WriteLine("Check right neighbours for: [{0}][{1}] Result:{2}", x, y, !(y + 1 == Columns));
                    return !(y + 1 == Columns);
                default:
                    return false;
            }
        }

        /*
         *  Add "wall" to cell neighbour depending by selected direction
         **/ 
        public Directions[,] AddWallToNeighbour(int x, int y, Directions[,] maze, string direction)
        {
            switch (direction)
            {
                case "up":
                    maze[x - 1, y].down = true;
                    break;
                case "down":
                    maze[x + 1, y].up = true;
                    break;
                case "left":
                    maze[x, y - 1].right = true;
                    break;
                case "right":
                    maze[x, y + 1].left = true;
                    break;
            }

            return maze;
        }

        /**
         * Add wall or not
        **/
        public bool RandWall()
        {
            int prob = _gen.Next(100);
            return prob < 50;
        }

        /**
         * Select wall orientation depening by with and height
        **/
        public string GetOrientation(int w, int h)
        {
            if (w < h) return "H";
            if (w > h) return "V";

            int prob = _gen.Next(100);

            return (prob < 50) ? "H" : "V";
        }

        /**
         * Generate random number with max limit
        **/
        public int GenerateRand(int max)
        {
            int prob = _gen.Next(max);

            return prob;
        }

        /**
         * Geneate a random direction 
        **/
        public string GenerateRandDirection()
        {
            switch (_gen.Next(3))
            {
                case 0:
                    return "up";
                case 1:
                    return "down";
                case 2:
                    return "left";
                case 3:
                    return "right";
                default:
                    return "up";
            }
        }

        /**
         * Add random exit to the maze 
         **/
        public Directions[,] CreateExit(Directions[,] maze, bool genX, bool genY)
        {     
            if (genX) XExit = GenerateRand(Lines);
            if (genY) YExit = GenerateRand(Columns);
            
            switch (DirectionExit)
            {
                case "up":
                    if (maze[0, YExit].down && maze[0, YExit].left && maze[0, YExit].right) CreateExit(maze, false, true);
                    maze[0, YExit].up = false;
                    break;
                case "down":
                    if (maze[Lines - 1, YExit].up && maze[Columns - 1, YExit].left && maze[Columns - 1, YExit].right) CreateExit(maze, false, true);
                    maze[Lines - 1, YExit].down = false;
                    break;
                case "left":
                    if (maze[XExit, 0].up && maze[XExit, 0].down && maze[XExit, 0].right) CreateExit(maze, true, false);
                    maze[XExit, 0].left = false;
                    break;
                case "right":
                    if (maze[XExit, Columns - 1].left && maze[XExit, Columns - 1].up && maze[XExit, Columns - 1].down) CreateExit(maze, true, false);
                    maze[XExit, Columns - 1].right = false;
                    break;
                default:
                    Console.WriteLine("Direction null");
                    break;
            }
            return maze;
        }
    }
}
