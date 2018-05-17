using System;
using UnityEngine;

namespace Assets.Scripts.Map.Pathfinding
{
    public class AStarNode : PathfindingNode
    {
        private int h;

        public override int F{ get { return g + h; } }
        public int H { get { return h; } }

        public AStarNode(Vector2Int _point, Vector2Int target, AStarNode _parent = null, int _g = 0)
        {
            point = _point;
            parent = _parent;
            g = _g + (parent != null ? point.DistanceEstimate(parent.Point) : 0);

            h = point.DistanceEstimate(target);
        }        

        public override string ToString()
        {
            return
                "{" + point.ToString() + ", g=" + g + ", h=" + h + "}";
        }

        public override bool Equals(object obj)
        {
            AStarNode other = obj as AStarNode;

            return other != null && other.Point.Equals(this.Point);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
