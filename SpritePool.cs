using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3TZ
{
    internal static class SpritePool
    {
        private static List<Sprite> spritePool = new List<Sprite>();
        private static List<Sprite> spriteBonusPool = new List<Sprite>();

        private static Sprite spriteDestroyer;

        public static void CreatePool()
        {
            spritePool.Add(new Sprite(texture: Data.Red));
            spritePool.Add(new Sprite(texture: Data.Green));
            spritePool.Add(new Sprite(texture: Data.Blue));
            spritePool.Add(new Sprite(texture: Data.Purple));
            spritePool.Add(new Sprite(texture: Data.Orange));

            spriteBonusPool.Add(new Sprite(texture: Data.LineHorizontal));
            spriteBonusPool.Add(new Sprite(texture: Data.LineVertical));
            spriteBonusPool.Add(new Sprite(texture: Data.Bomb));

            spriteDestroyer = new Sprite(texture: Data.Destroyer);
            spriteDestroyer.Origin = new Vector2f(16, 16);

            foreach (var sp in spritePool)
            {
                sp.Origin = new Vector2f(16, 16);
            }

            foreach(var sp in spriteBonusPool)
            {
                sp.Origin = new Vector2f(16, 16);
            }
        }

        public static Sprite GetSpriteByType(CellTypes type)
        {
            return spritePool[(int)type];
        }

        public static Sprite GetBonusSpriteByType(Bonus type)
        {
            if (type == Bonus.Empty)
            {
                return null;
            }

            return spriteBonusPool[(int)type - 1];
        }

        public static Sprite GetSpriteDestroyer()
        {
            return spriteDestroyer;
        }

        public static void DestroyPool() 
        {
            spritePool.Clear();
            spriteBonusPool.Clear();
        }
    }
}
