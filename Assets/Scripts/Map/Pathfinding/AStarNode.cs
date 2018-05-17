using System;
using UnityEngine;

namespace Assets.Scripts.Map.Pathfinding
{
    public class AStarNode
    {
        protected Vector2Int point;
        protected AStarNode parent;
        protected int g;
        private int h;
        
        public Vector2Int Point { get { return point; } }
        public AStarNode Parent { get { return parent; } }
        public int F { get { return g + h; } }
        public int G { get { return g; } }
        public int H { get { return h; } }                

        public AStarNode(Vector2Int _point, Vector2Int target, AStarNode _parent = null, int _g = 0)
        {
            point = _point;
            parent = _parent;
            g = _g + (parent != null ? point.DistanceEstimate(parent.point) : 0);

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
            return Point.GetHashCode();
        }
    }
}
