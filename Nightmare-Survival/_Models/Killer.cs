namespace Nightmare_Survival
{
    public class Killer
    {
        private Vector2 _position = new(300, 100);
        private readonly float _speed = 70f;
        private readonly AnimationManager _animations = new();

        public Killer()
        {
            var killerTexture = Globals.ContentManager.Load<Texture2D>("Killer");

            _animations.AddAnimation(new Vector2(-1, 0), new(killerTexture, 11, 1, 0.1f, 4));
            _animations.AddAnimation(new Vector2(0, 1), new(killerTexture, 11, 1, 0.1f, 2));
            _animations.AddAnimation(new Vector2(0, -1), new(killerTexture, 11, 1, 0.1f, 3));
            _animations.AddAnimation(new Vector2(1, 0), new(killerTexture, 12, 1, 0.1f, 1));
            _animations.AddAnimation(new Vector2(0, 0), new(killerTexture, 11, 1, 0.1f, 5));
        }

        public void Update()
        {
            if (KillerMovementManager.Moving)
            {
                _position += Vector2.Normalize(KillerMovementManager.Direction) * _speed * Globals.TotalSeconds;
            }
            _animations.Update(KillerMovementManager.Direction);
        }

        public void Draw()
        {
            _animations.Draw(_position);
        }
    }
}