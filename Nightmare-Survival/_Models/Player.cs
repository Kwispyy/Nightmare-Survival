namespace Nightmare_Survival
{
    public class Player
    {
        private Vector2 _position = new(100, 100);
        private readonly float _speed = 70f;
        private readonly AnimationManager _animations = new();

        public Player()
        {
            var playerTexture = Globals.ContentManager.Load<Texture2D>("Sprites/Player/idlePlayer");

            _animations.AddAnimation(new Vector2(-1, 0), new(playerTexture, 8, 4, 0.1f, 1));
            _animations.AddAnimation(new Vector2(0, 1), new(playerTexture, 8, 4, 0.1f, 2));
            _animations.AddAnimation(new Vector2(0, -1), new(playerTexture, 8, 4, 0.1f, 3));
            _animations.AddAnimation(new Vector2(1, 0), new(playerTexture, 8, 4, 0.1f, 4));
        }

        public void Update()
        {
            if (InputManager.Moving)
            {
                _position += Vector2.Normalize(InputManager.Direction) * _speed * Globals.TotalSeconds;
            }

            _animations.Update(InputManager.Direction);
        }

        public void Draw()
        {
            _animations.Draw(_position);
        }
    }
}