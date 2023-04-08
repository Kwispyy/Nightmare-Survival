namespace Nightmare_Survival
{
    public class GameManager
    {
        private Player _player;

        public void Initialize()
        {
            _player = new();
        }

        public void Update()
        {
            InputManager.Update();
            _player.Update();
        }

        public void Draw()
        {
            _player.Draw();
        }
    }
}