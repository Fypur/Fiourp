using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public class LevelData
    {
        public Vector2 Pos;
        public List<Entity> Entities;
        public int[,] Organisation;

        public Map ParentMap;

        public Action EnterAction;
        public Action ExitAction;

        public LevelData(List<Entity> entityData, Vector2 position, int[,] organisation, Map parentMap, Action enterAction = null, Action exitAction = null)
        {
            Pos = position;
            Entities = entityData;
            Organisation = organisation;
            EnterAction = enterAction;
            ExitAction = exitAction;
            ParentMap = parentMap;
        }
    }
}
