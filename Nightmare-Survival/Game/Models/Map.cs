

namespace Nightmare_Survival
{
    public class Map : IDisposable
    {
        private Tile[,] tiles;

        Player player;
        public Player Player
        {
            get { return player; }
        }

        // TODO: Relevant to the task below
        //private List<Bed> beds = new();
        //private List<Enemy> enemies = new();

        private Vector2 start;
        private static readonly Point InvalidPosition = new Point(-1, -1);
        private Point exit = InvalidPosition;
        
        
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

        ContentManager contentManager;
        public ContentManager Content
        {
            get { return contentManager; }
        }

        public int Width
        {
            get { return tiles.GetLength(0); }
        }

        public int Height
        {
            get { return tiles.GetLength(1); }
        }

        //private SoundEffect exitSound;

        // TODO: !!!
        #region Initialize all methods for tile types

        //public Map(IServiceProvider serviceProvider, Stream fileStream, int mapIndex)
        //{
        //    contentManager = new ContentManager(serviceProvider, "Content");
        //    timeRemaining = TimeSpan.FromMinutes(2.0);

        //    LoadTiles(fileStream);

        //    //exitSound = Content.Load<SoundEffect>("");
        //}

        
        //private Tile LoadTiles(char tileType, int x, int y)
        //{
        //    switch(tileType)
        //    {
        //        // Blank space
        //        case '.':
        //            return new Tile(null, TileCollision.Passable);

        //        // Door
        //        case 'D':
        //            return LoadDoorTile(x, y);

        //        // Bed
        //        case 'B':
        //            return LoadBedTile(x, y);

        //        // Killer
        //        case 'A':
        //            return LoadKillerTile(x, y, "Killer");

        //        // Player 1 start point
        //        case '1':
        //            return LoadStartTile(x, y);

        //        // Wall block
        //        case '#':
        //            return LoadVarietyTile("Wall", 7, TileCollision.Wall);
        //    }
        //}

        //private Tile LoadTile(string name, TileCollision collision)
        //{
        //    return new Tile(Content.Load<Texture2D>("Tiles/" + name), collision);
        //}

        //private Tile LoadVarietyTile(string baseName, int variationCount, TileCollision collision)
        //{
        //    int index = random.Next(variationCount);
        //    return LoadTile(baseName + index, collision);
        //}


        ////private Tile LoadStartTile(int x, int y)
        ////{
            
        ////}

        //private Tile LoadDoorTile(int x, int y)
        //{

        //}

        //private Tile LoadBedTile(int x, int y)
        //{

        //}

        //private Tile LoadKillerTile(int x, int y, string spriteSet)
        //{

        //}
        

        //public void LoadTiles(Stream fileStream)
        //{
        //    int width;
        //    List<string> lines = new();
        //    using(StreamReader reader = new StreamReader(fileStream))
        //    {
        //        string line = reader.ReadLine();
        //        width = line.Length;
        //        while(line != null)
        //        {
        //            lines.Add(line);
        //            line = reader.ReadLine();
        //        }
        //    }

        //    tiles = new Tile[width, lines.Count];

        //    for(int y = 0; y < Height; y++)
        //    {
        //        for(int x = 0; x < Width; x++)
        //        {
        //            char tileType = lines[x][y];
        //            tiles[x, y] = LoadTile(tileType, x, y);
        //        }
        //    }
        //}
        #endregion

        public void Dispose()
        {
            Content.Unload();
        }

        public void Draw()
        {
            
        }
    }
}