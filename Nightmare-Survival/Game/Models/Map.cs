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

        public Killer Killer
        {
            get { return killer; }
        }
        Killer killer;

        //Map key location
        private Vector2 playerStart;
        private Vector2 killerStart;

        private Random random = new Random(354668);

        Vector2[] points = new Vector2[]
        {
            new Vector2(0, 0),  // Левый верхний угол
            new Vector2(100, 380),  // Правый верхний угол
            new Vector2(592, 380),  // Правый нижний угол
            new Vector2(20, 20)   // Левый нижний угол
        };

        public int Value;

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
        public Map(IServiceProvider serviceProvider, Stream fileStream, int mapIndex)
        {
            content = new ContentManager(serviceProvider, "Content");
            timeRemaining = TimeSpan.FromMinutes(0.5);

            LoadTiles(fileStream);
        }
        #region Initialize all methods for tile types
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

            if (Player == null)
            {
                throw new NotSupportedException("Need a start point for player!");
            }
        }

        private Tile LoadTile(char tileType, int x, int y)
        {
            return tileType switch
            {
                // Blank space
                '.' => LoadVarietyTile("tile", 5, TileCollision.Passable),
                // Door
                'D' => LoadDoorTile(x, y, TileCollision.Door),
                // Bed
                'B' => LoadBedTile(x, y, TileCollision.Bed),
                // Killer
                'K' => LoadKillerTile(x, y),
                // Player start point
                'S' => LoadStartTile(x, y),
                // Wall block
                '#' => LoadWallTile("wall", TileCollision.Wall),
                // Floor (wood)
                'F' => LoadTile("wood", TileCollision.Passable),
                _ => throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", tileType, x, y)),
            };
        }

        private Tile LoadTile(string name, TileCollision collision)
        {
            return new Tile(Content.Load<Texture2D>("Tiles/" + name), collision);
        }

        private Tile LoadWallTile(string name, TileCollision collision)
        {
            return new Tile(Content.Load<Texture2D>("Tiles/" + name), collision);
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

            playerStart = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            player = new Player(this, playerStart);

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
            return new Tile(Content.Load<Texture2D>("Tiles/bed"), collision);
        }

        private Tile LoadKillerTile(int x, int y)
        {
            if(Killer != null)
            {
                throw new NotSupportedException("A level may have killer");
            }

            killerStart = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            killer = new Killer(this, killerStart);

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

            if(!Player.IsAlive)
            {
                Player.Reset(playerStart);
                Value = 0;
                timeRemaining = TimeSpan.FromMinutes(2.0);
            }

            Player.Update(gameTime);
            KillerUpdate(gameTime);
        }

        public void KillerUpdate(GameTime gameTime)
        {
            Killer.Update(Player.Position, killer.Position, gameTime);

            if (killer.BoundingRectangle.Intersects(Player.BoundingRectangle))
            {
                PlayerKilled();
            }
        }

        private void PlayerKilled()
        {
            Player.OnKilled();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawTiles(spriteBatch);
            Player.Draw(gameTime, spriteBatch);
            Killer.Draw(gameTime, spriteBatch);
        }

        private void DrawTiles(SpriteBatch spriteBatch)
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