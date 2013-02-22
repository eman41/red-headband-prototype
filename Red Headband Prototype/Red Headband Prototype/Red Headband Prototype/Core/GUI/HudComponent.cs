namespace Red_Headband_Prototype.Core.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using TileEngine.Engine;

    public class HudComponent : GameComponent, IResetable
    {
        private PlayerObject _player;
        private Texture2D _sheet;
        private Vector2 _healthPosition;
        private Rectangle _healthFrameRect;
        private Rectangle _healthFillRect;

        private const int HP_BAR_WIDTH = 300;
        private const int COLOR_MAX = 255;

        private Color _hpColor;
        private int _hpR = COLOR_MAX;
        private int _hpG = COLOR_MAX;
        private int _hpB = COLOR_MAX;


        public HudComponent(Game game, String dataPath, PlayerObject player)
            : base(game)
        {
            _player = player;
            LoadAssets(dataPath);
            _healthPosition = new Vector2(
                Game.GraphicsDevice.Viewport.Width / 2 - _healthFrameRect.Width / 2,
                Game.GraphicsDevice.Viewport.Height - _healthFrameRect.Height - 10);
        }

        public Color HpBarColor
        {
            set
            {
                _hpColor = value;
            }
            get
            {
                return _hpColor;
            }
        }

        public void Update()
        {
            UpdateHealth();
        }

        public void Reset()
        {
            _healthFillRect.Width = PlayerObject.PLAYER_MAX_HP;
        }

        private void UpdateHealth()
        {
            if (_player.IsAlive)
            {
                if (_healthFillRect.Width > _player.CurrentHelth)
                {
                    _healthFillRect.Width--;
                }
                else if (_healthFillRect.Width < _player.CurrentHelth)
                {
                    _healthFillRect.Width++;
                }

                // Turn up the red as health decreases
                float f = (float)COLOR_MAX * ((float)_healthFillRect.Width / (float)HP_BAR_WIDTH);
                _hpB = _hpG = Convert.ToInt32(f);
            }
            else
            {
                _hpB = _hpG = 0;
                _healthFillRect.Width = 0;
            }

            _hpColor = new Color(_hpR, _hpG, _hpB);
        }

        public void Draw(GameTime gameTime, SpriteBatch batch)
        {
            batch.Draw(_sheet, _healthPosition, _healthFillRect, _hpColor);
            batch.Draw(_sheet, _healthPosition, _healthFrameRect, Color.White);
        }

        private float HealPercent()
        {
            return (float)_healthFillRect.Width / (float)PlayerObject.PLAYER_MAX_HP;
        }

        private void LoadAssets(string datapath)
        {
            XElement decoded = XElement.Load(datapath);
            IEnumerable<string> sheetPath = from path in decoded.Elements("sheet") select (string)path.Attribute("path");
            _sheet = Game.Content.Load<Texture2D>(sheetPath.ElementAt<string>(0));

            _healthFrameRect = LoadXmlRectangle(decoded, "healthbar-frame");
            _healthFillRect = LoadXmlRectangle(decoded, "healthbar-fill");
        }

        private Rectangle LoadXmlRectangle(XElement xmlElement, string parentTag)
        {
            var rectangle = from path in xmlElement.Elements(parentTag)
                            select new Rectangle(
                                (int)path.Element("rectangle").Attribute("x"),
                                (int)path.Element("rectangle").Attribute("y"),
                                (int)path.Element("rectangle").Attribute("w"),
                                (int)path.Element("rectangle").Attribute("h"));

            
            return rectangle.ElementAt<Rectangle>(0);
        }
    }
}
