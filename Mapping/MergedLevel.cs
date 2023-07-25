using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp
{
    public class MergedLevel : Level
    {
        public Level[] MergedLevels;

        public MergedLevel(params Level[] levelsMerged) : base(MakeLevelData(levelsMerged))
        {
            MergedLevels = levelsMerged;
        }

        private static LevelData MakeLevelData(Level[] levelsMerged)
        {
            List<Entity> entities = new List<Entity>();
            Vector2 pos = levelsMerged[0].Pos;
            Vector2 size = levelsMerged[0].Size;
            Action enterAction = null;

            foreach (Level level in levelsMerged)
            {
                entities.AddRange(level.EntityData);

                if (level.Pos.X < pos.X)
                {
                    size.X += pos.X - level.Pos.X;
                    pos.X = level.Pos.X;
                }
                if (level.Pos.Y < pos.Y)
                {
                    size.Y += pos.Y - level.Pos.Y;
                    pos.Y = level.Pos.Y;
                }
                if(level.Pos.X + level.Size.X > pos.X + size.X)
                    size.X += level.Pos.X + level.Size.X - pos.X;
                if (level.Pos.Y + level.Size.Y > pos.Y + size.Y)
                    size.Y += level.Pos.Y + level.Size.Y - pos.Y;

                enterAction += level.EnterAction;
            }

            int[,] organisation = new int[(int)Math.Ceiling(size.Y / levelsMerged[0].TileHeight), (int)Math.Ceiling(size.X / levelsMerged[0].TileWidth)];

            foreach (Level level in levelsMerged)
            {
                int offsetX = (int)(level.Pos.X - pos.X) / level.TileWidth;
                int offsetY = (int)(level.Pos.Y - pos.Y) / level.TileHeight;

                for (int y = 0; y < level.Organisation.GetLength(0); y++)
                {
                    for (int x = 0; x < level.Organisation.GetLength(1); x++)
                    {
                        organisation[offsetY + y, offsetX + x] = level.Organisation[y, x];
                    }
                }
            }

            

            return new LevelData(entities, pos, size, organisation, levelsMerged[0].ParentMap, enterAction);
        }
    }
}
