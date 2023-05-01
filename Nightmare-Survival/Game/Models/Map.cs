namespace Nightmare_Survival
{
    public class Map : IDisposable
    {
        //Struct on the map
        private Tile[,] tiles;

        public Player Player
        {
            get { return player; }
        }
        Player player;

        // TODO: Relevant to the task below
        private List<Bed> beds = new();
        //private List<Killer> killers = new();

        //Key location
        private Vector2 start;
        private static readonly Point InvalidPosition = new Point(-1, -1);
        private Point door = InvalidPosition;
        
        private Random random = new Random(354668);

        int value;
        public int Value
        {
            get { return value; }
        }

        //bool reachedExit;
        //public bool ReachedExit
        //{
        //    get { return reachedExit; }
        //}

        TimeSpan timeRemaining;
        public TimeSpan TimeRemaining
        {
            get { return timeRemaining; }
        }

        ContentManager content;
        public ContentManager Content
        {
            get { return content; }
        }

        #region Initialize all methods for tile types
        public Map(IServiceProvider serviceProvider, Stream fileStream, int mapIndex)
        {
            content = new ContentManager(serviceProvider, "Content");
            timeRemaining = TimeSpan.FromMinutes(2.0);

            LoadTiles(fileStream);

            //exitSound = Content.Load<SoundEffect>("");
        }

        private void LoadTiles(Stream fileStream)
        {
            // Load the level and ensure all of the lines are the same length.
            int width;
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line = reader.ReadLine();
                width = line.Length;
                while (line != null)
                {
                    lines.Add(line);
                    if (line.Length != width)
                        throw new Exception(String.Format("The length of line {0} is different from all preceeding lines.", lines.Count));
                    line = reader.ReadLine();
                }
            }

            // Allocate the tile grid.
            tiles = new Tile[width, lines.Count];

            // Loop over every tile position,
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // to load each tile.
                    char tileType = lines[y][x];
                    tiles[x, y] = LoadTile(tileType, x, y);
                }
            }

            //if(Player == null)
            //{
            //    throw new NotSupportedException("Need a start point for player!");
            //}
        }

        private Tile LoadTile(char tileType, int x, int y)
        {
            switch (tileType)
            {
                // Blank space
                case '.':
                    return LoadVarietyTile("tile", 5, TileCollision.Passable);

                // Door
                case 'D':
                    return LoadDoorTile(x, y, TileCollision.Door);

                // Bed
                case 'B':
                    return LoadBedTile(x, y, TileCollision.Bed);

                // Killer
                case 'K':
                    return LoadKillerTile(x, y, "Killer");

                // Player start point
                case 'S':
                    return LoadStartTile(x, y);

                // Wall block
                case '#':
                    return LoadWallTile("wall", TileCollision.Wall);
                default:
                    throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", tileType, x, y));
            }
        }

        private Tile LoadTile(string name, TileCollision collision)
        {
            return new Tile(Content.Load<Texture2D>("Tiles/" + name), collision);
        }

        private Tile LoadWallTile(string name, TileCollision collision)
        {
            return new Tile(Content.Load<Texture2D>("Tiles/wall"), collision);
        }

        private Tile LoadVarietyTile(string baseName, int variationCount, TileCollision collision)
        {
            int index = random.Next(variationCount);
            return LoadTile(baseName + index, collision);
        }


        private Tile LoadStartTile(int x, int y)
        {
            if (Player != null)
                throw new NotSupportedException("A level may only have one starting point.");

            start = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            player = new Player(this, start);

            return new Tile(null, TileCollision.Passable);
        }

        private Tile LoadDoorTile(int x, int y, TileCollision collision)
        {
            Point position = GetBounds(x, y).Center;
            return new Tile(Content.Load<Texture2D>("Tiles/door"), collision);
        }

        private Tile LoadBedTile(int x, int y, TileCollision collision)
        {
            Point position = GetBounds(x, y).Center;
            beds.Add(new Bed(this, new Vector2(position.X, position.Y)));

            return new Tile(Content.Load<Texture2D>("Tiles/bed"), collision);
        }

        private Tile LoadKillerTile(int x, int y, string spriteSet)
        {
            Vector2 position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            //killers.Add(new Killer(this, position, spriteSet));

            return new Tile(null, TileCollision.Passable);
        }
        #endregion

        public void Dispose()
        {
            Content.Unload();
        }

        public TileCollision GetCollision(int x, int y)
        {
            if (x < 0 || x >= Width)
                return TileCollision.Wall;

            if (y < 0 || y >= Height)
                return TileCollision.Passable;

            return tiles[x, y].Collision;
        }

        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }

        public int Width
        {
            get { return tiles.GetLength(0); }
        }

        public int Height
        {
            get { return tiles.GetLength(1); }
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            timeRemaining -= gameTime.ElapsedGameTime;

            if(Player.Position.X > 575 && Player.Position.X < 605 && Player.Position.Y > 280 && Player.Position.Y < 320)
            {
                value += 1;
            }

            Player.Update(gameTime, keyboardState);
        }

        public void Draw(GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            DrawTiles(spriteBatch);
            Player.Draw(gameTime, spriteBatch);
        }

        private void DrawTiles(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    Texture2D texture = tiles[x, y].Texture;
                    if (texture != null)
                    {
                        Vector2 position = new Vector2(x, y) * Tile.Size;
                        spriteBatch.Draw(texture, position, Color.White);
                    }
                }
            }
        }
    }
}