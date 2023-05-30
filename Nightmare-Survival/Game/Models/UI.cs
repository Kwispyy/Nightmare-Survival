namespace Nightmare_Survival
{
    public class UI
    {
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private List<Upgrade> upgrades;

        public UI(SpriteBatch spriteBatch, SpriteFont font)
        {
            this.spriteBatch = spriteBatch;
            this.font = font;

            upgrades = new List<Upgrade>
            {
                new Upgrade("Upgrade bed LVL1", 10),
                new Upgrade("Updgrade bed LVL2", 20),
            };
        }

        public void Update()
        {

        }

        public void Draw()
        {
            int yOffset = 0;

            for(int i = 0;  i < upgrades.Count; i++)
            {
                string upgradeName = upgrades[i].Name;
                int upgradeCost = upgrades[i].Cost;

                spriteBatch.DrawString(font, upgradeName + " - Cost: " + upgradeCost, new Vector2(580, yOffset), Color.White);
                DrawShadowedString(font, upgradeName + " - Cost: " + upgradeCost, new Vector2(580, yOffset), Color.White);

                yOffset += 25;
            }
        }

        private void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
            spriteBatch.DrawString(font, value, position, color);
        }
    }
}