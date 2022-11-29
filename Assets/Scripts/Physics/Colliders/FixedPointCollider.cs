using BlueNoah.Math.FixedPoint;
using System.Collections.Generic;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    public enum ColliderType
    {
        Sphere,
        AABB,
        OBB,
        Triangle
    }
    public abstract class FixedPointCollider : FastListItem
    {
        public GameObject actorPresenter { get; set; }
        public FixedPointTransform fixedPointTransform { get; protected set; }
        public FixedPointRigidbody fixedPointRigidbody { get; set; }
        public FixedPointVector3 min { get;protected  set; }
        public FixedPointVector3 max { get; protected set; }
        public FixedPointVector3 offset;
        public FixedPointVector3 position {
            get {
                return fixedPointTransform.fixedPointPosition + offset;    
            }
        }
        public int layer { get; set; }
        public bool enabled { get; set; } = true;
        public bool isTrigger { get; set; }
        public int castIndex { get; set; }
        public ColliderType colliderType { get;protected set; }
        public HashSet<FixedPointOctreeNode> impactNodes { get; set; } = new HashSet<FixedPointOctreeNode>();
        public abstract void UpdateCollider();

    }
}