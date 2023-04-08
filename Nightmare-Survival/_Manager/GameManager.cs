namespace Nightmare_Survival
{
    public class GameManager
    {
        private Player _player;
        private Killer _killer;
        private readonly Map _map;

        public GameManager()
        {
            _map = new();
        }

        public void Initialize()
        {
            _killer = new();
            _player = new();
        }

        public void Update()
        {
            InputManager.Update();
            KillerMovementManager.Update();
            _player.Update();
            _killer.Update();
        }

        public void Draw()
        {
            _map.Draw();
            _player.Draw();
            _killer.Draw();
        }
    }
}