using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Map.Pathfinding
{
    public class DijkstraNode : PathfindingNode
    {
        public DijkstraNode(Vector2Int _point, DijkstraNode _parent = null, int _g = 0)
        {
            point = _point;
            parent = _parent;
            g = _g + (parent != null ? point.DistanceEstimate(parent.Point) : 0);
        }

        public override bool Equals(object obj)
        {
            DijkstraNode other = obj as DijkstraNode;

            return other != null && other.Point.Equals(this.Point);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
