using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Map.Pathfinding
{
    public abstract class PathfindingNode
    {
        protected Vector2Int point;
        protected PathfindingNode parent;
        protected int g;        

        public Vector2Int Point { get { return point; } }
        public PathfindingNode Parent { get { return parent; } }
        public virtual int F { get { return g; } }
        public int G { get { return g; } }

        public override string ToString()
        {
            return
                "{" + point.ToString() + ", g=" + g + "}";
        }

        public override bool Equals(object obj)
        {
            PathfindingNode other = obj as PathfindingNode;

            return other != null && other.Point.Equals(this.Point);
        }

        public override int GetHashCode()
        {
            return Point.GetHashCode();
        }
    }
}
