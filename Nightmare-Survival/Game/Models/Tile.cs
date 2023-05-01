namespace Nightmare_Survival
{
    public enum TileCollision
    {
        // A passable tile is one which does not hinder player motion at all.
        Passable = 0,

        // An impassable tile is one which does not allow the player to move through.
        // This's wall.
        Wall = 1,

        //This tile is a "door" type of tile.
        //The player can pass through it freely, but the killer cannot and must break it.
        Door = 2,

        // As long as the player is on this tile he will save currency
        Bed = 3
    }

    struct Tile
    {
        public Texture2D Texture;
        public TileCollision Collision;

        public const int Width = 32;
        public const int Height = 32;

        public static readonly Vector2 Size = new Vector2(Width, Height);

        public Tile(Texture2D texture, TileCollision collision)
        {
            Texture = texture;
            Collision = collision;
        }
    }
}