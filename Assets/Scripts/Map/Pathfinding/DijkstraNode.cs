using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Map.Pathfinding
{
    class DijkstraNode : AStarNode
    {
        public DijkstraNode(Vector2Int _point, AStarNode _parent = null, int _g = 0) : base(_point, _point, _parent, _g)
        {
        }
    }
}
