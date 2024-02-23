using BlueNoah.Common;
using BlueNoah.Math.FixedPoint;
using System.Collections.Generic;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    internal class FPOctree
    {
        internal FPOctreeNode root;
        internal readonly List<FPCollider> colliders = new ();
        private readonly List<FPOctreeNode> nodes = new ();
        private readonly List<FPOctreeNode> openList = new ();
        private int castIndex;
        public int size { get;private set; }
        private int openListIndex;
        public static FPOctree Initialize(int size)
        {
            var fixedPointOctree = new FPOctree
            {
                size = size
            };
            var exp = (int)System.Math.Log(size, 2);
            var root = new FPOctreeNode((int)System.Math.Pow(2, exp - 1), new FixedPointVector3(0, 0, 0), size);
            var openList = new List<FPOctreeNode> { root };
            const int minHalfSize = 8;
            while (openList.Count > 0)
            {
                var node = openList[0];
                openList.RemoveAt(0);
                node.nodes = new FPOctreeNode[8];
                for (var i = 0; i < 8; i++)
                {
                    var x = i % 2 == 0 ? node.halfSize / 2 : -node.halfSize / 2;
                    var y = i / 2 % 2 == 0 ? node.halfSize / 2 : -node.halfSize / 2;
                    var z = i / 4 % 2 == 0 ? node.halfSize / 2 : -node.halfSize / 2;
                    var subNode = new FPOctreeNode( node.halfSize / 2, node.pos + new FixedPointVector3(x, y, z), node.size / 2)
                        {
                            parentNode = node
                        };
                    if (node.halfSize / 2 > minHalfSize)
                    {
                        openList.Add(subNode);
                    }
                    fixedPointOctree.nodes.Add(subNode);
                    node.nodes[i] = subNode;
                }
            }
            fixedPointOctree.root = root;
            return fixedPointOctree;
        }
        public bool IsOutOfBound(FixedPointVector3 position)
        {
            return position.x > size || position.x < -size || position.y > size || position.y < -size || position.z > size || position.z < -size;
        }
        public void Reset()
        {
            colliders?.Clear();
            foreach (var item in nodes)
            {
                item.FpSphereStack?.Clear();
                item.FpObbStack?.Clear();
                item.FpCapsuleStack?.Clear();
                item.FpCylinderStack?.Clear();
                item.FpAABBStack?.Clear();
                item.FpAACapsuleStack?.Clear();
                item.FpCharacterStack?.Clear();
            }
        }
        public void AddCollider(FPCollider fpCollider)
        {
            colliders.Add(fpCollider);
        }
        public void UpdateColliders()
        {
            openList.Clear();
            openListIndex = 0;
            foreach (var item in colliders)
            {
                if (!item.fpTransform.colliderUpdateFlag) continue;
                item.UpdateCollider();
                item.fpTransform.colliderUpdateFlag = false;
            }
        }
        public List<FPCollision> OverlayBoxCollision(FixedPointVector3 position, FixedPointVector3 halfSize, FixedPointMatrix orientation, int layerMask = -1, bool includeTrigger = false)
        {
            var collisions = new List<FPCollision>();
            openList.Add(root);
            castIndex++;
            if (castIndex == int.MaxValue)
            {
                castIndex = 0;
            }
            while (openList.Count > openListIndex)
            {
                var node = openList[openListIndex];
                openListIndex++;
                if (node.FpSphereStack != null)
                {
                    for (var i = 0; i < node.FpSphereStack.Count; i++)
                    {
                        var item = node.FpSphereStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled)
                        {
                            continue;
                        }
                        if (item.isTrigger && !includeTrigger)
                        {
                            continue;
                        }
                        if (layerMask != -1 && !GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        var fixedPointCollision = FixedPointIntersection.IntersectWithSphereAndOBB(item.position, item.radius, position, halfSize, orientation);
                        if (!fixedPointCollision.hit) continue;
                        fixedPointCollision.normal = -fixedPointCollision.normal;
                        collisions.Add(fixedPointCollision);
                    }
                }

                if (node.nodes == null) continue;
                {
                    foreach (var item in node.nodes)
                    {
                        if(item.colliderCount <= 0) continue;
                        if (FixedPointIntersection.IntersectWithAABBAndOBBFixedPoint(item.fixedPointAABB.Min, item.fixedPointAABB.Max, position, halfSize, orientation))
                        {
                            openList.Add(item);
                            //Debug.Log(node.nodes[i]);
                        }
                    }
                }
            }
            return collisions;
        }
        public int OverlaySphereCollision(FixedPointVector3 position, FixedPoint64 radius,ref List<FPCollision> collisions, int layerMask = -1, bool includeTrigger = false,bool dynamic= false)
        {
            var count = 0;
            openList.Add(root);
            castIndex++;
            if (castIndex == int.MaxValue)
            {
                castIndex = 0;
            }
            while (openList.Count > openListIndex)
            {
                var node = openList[openListIndex];
                openListIndex++;
                FPCollision fpCollision;
                if (node.FpSphereStack != null)
                {
                    for (var i = 0; i < node.FpSphereStack.Count; i++)
                    {
                        var item = node.FpSphereStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled)
                        {
                            continue;
                        }
                        if (item.isTrigger && !includeTrigger)
                        {
                            continue;
                        }
                        if (item.isDynamic != dynamic)
                        {
                            continue;
                        }
                        if (layerMask != -1 && !GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        fpCollision = FixedPointIntersection.IntersectWithSphereAndSphere(position, radius, item.position, item.radius);
                        if (fpCollision.hit)
                        {
                            fpCollision.collider = item;
                            if (collisions.Count == count)
                            {
                                collisions.Add(fpCollision);
                            }
                            else
                            {
                                collisions[count] = fpCollision;
                            }
                            count++;
                        }
                    }
                }
                if (node.FpAABBStack != null)
                {
                    for (var i = 0; i < node.FpAABBStack.Count; i++)
                    {
                        var item = node.FpAABBStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled)
                        {
                            continue;
                        }
                        if (item.isTrigger && !includeTrigger)
                        {
                            continue;
                        }
                        if (item.isDynamic != dynamic)
                        {
                            continue;
                        }
                        if (layerMask != -1 && !GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        fpCollision = FixedPointIntersection.IntersectWithSphereAndAABB(position, radius, item.min, item.max);
                        if (fpCollision.hit)
                        {
                            fpCollision.collider = item;
                            if (collisions.Count == count)
                            {
                                collisions.Add(fpCollision);
                            }
                            else
                            {
                                collisions[count] = fpCollision;
                            }
                            count++;
                        }
                    }
                }
                if (node.FpObbStack != null)
                {
                    for (var i = 0; i < node.FpObbStack.Count; i++)
                    {
                        var item = node.FpObbStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled)
                        {
                            continue;
                        }
                        if (item.isTrigger && !includeTrigger)
                        {
                            continue;
                        }
                        if (item.isDynamic != dynamic)
                        {
                            continue;
                        }
                        if (!GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        fpCollision = FixedPointIntersection.IntersectWithSphereAndOBB(position, radius, item.position, item.halfSize, item.fpTransform.fixedPointMatrix);
                        if (fpCollision.hit)
                        {
                            fpCollision.collider = item;
                            if (collisions.Count == count)
                            {
                                collisions.Add(fpCollision);
                            }
                            else
                            {
                                collisions[count] = fpCollision;
                            }
                            count++;
                        }
                    }
                }
                if (node.FpCapsuleStack != null)
                {
                    for (var i = 0; i < node.FpCapsuleStack.Count; i++)
                    {
                        var item = node.FpCapsuleStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled)
                        {
                            continue;
                        }
                        if (item.isTrigger && !includeTrigger)
                        {
                            continue;
                        }
                        if (item.isDynamic != dynamic)
                        {
                            continue;
                        }
                        if (!GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        fpCollision = FixedPointIntersection.IntersectWithSphereAndCapsule(position, radius, item.startPos, item.endPos, item.scaledRadius);
                        if (fpCollision.hit)
                        {
                            fpCollision.collider = item;
                            if (collisions.Count == count)
                            {
                                collisions.Add(fpCollision);
                            }
                            else
                            {
                                collisions[count] = fpCollision;
                            }
                            count++;
                        }
                    }
                }
                if (node.FpCylinderStack != null)
                {
                    for (var i = 0; i < node.FpCylinderStack.Count; i++)
                    {
                        var item = node.FpCylinderStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled)
                        {
                            continue;
                        }
                        if (item.isTrigger && !includeTrigger)
                        {
                            continue;
                        }
                        if (item.isDynamic != dynamic)
                        {
                            continue;
                        }
                        if (!GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        fpCollision = FixedPointIntersection.IntersectWithSphereAndCylinder(position, radius, item.startPos, item.endPos, item.radius);
                        if (fpCollision.hit)
                        {
                            fpCollision.collider = item;
                            if (collisions.Count == count)
                            {
                                collisions.Add(fpCollision);
                            }
                            else
                            {
                                collisions[count] = fpCollision;
                            }
                            count++;
                        }
                    }
                }
                if (node.FpAACapsuleStack != null)
                {
                    for (var i = 0; i < node.FpAACapsuleStack.Count; i++)
                    {
                        var item = node.FpAACapsuleStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled)
                        {
                            continue;
                        }
                        if (item.isTrigger && !includeTrigger)
                        {
                            continue;
                        }
                        if (item.isDynamic != dynamic)
                        {
                            continue;
                        }
                        if (!GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        fpCollision = FixedPointIntersection.IntersectWithSphereAndAACapsule(position, radius, item.startPos, item.endPos, item.scaledRadius);
                        if (fpCollision.hit)
                        {
                            fpCollision.collider = item;
                            if (collisions.Count == count)
                            {
                                collisions.Add(fpCollision);
                            }
                            else
                            {
                                collisions[count] = fpCollision;
                            }
                            count++;
                        }
                    }
                }
                
                if (node.nodes != null)
                {
                    foreach (var item in node.nodes)
                    {
                        if(item.colliderCount <= 0) continue;
                        if (FixedPointIntersection.IntersectWithAABBAndSphere(item.fixedPointAABB.Min, item.fixedPointAABB.Max, position, radius))
                        {
                            openList.Add(item);
                        }
                    }
                }
            }
            return count;
        }
        public int OverlayAABBCollisionCount(FixedPointVector3 min, FixedPointVector3 max, int layerMask = -1, bool includeTrigger = false) 
        {
            var count = 0;
            openList.Add(root);
            castIndex++;
            if (castIndex == int.MaxValue)
            {
                castIndex = 0;
            }
            while (openList.Count > openListIndex)
            {
                var node = openList[openListIndex];
                openListIndex++;
                if (node.FpSphereStack != null)
                {
                    for (var i = 0; i < node.FpSphereStack.Count; i++)
                    {
                        var item = node.FpSphereStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled)
                        {
                            continue;
                        }
                        if (item.isTrigger && !includeTrigger)
                        {
                            continue;
                        }
                        if (layerMask != -1 && !GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        if (FixedPointIntersection.IntersectWithAABBAndSphere(min, max, item.position, item.radius))
                        {
                            count++;
                        }
                    }
                }
                if (node.FpAABBStack != null)
                {
                    for (var i = 0; i < node.FpAABBStack.Count; i++)
                    {
                        var item = node.FpAABBStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled)
                        {
                            continue;
                        }
                        if (item.isTrigger && !includeTrigger)
                        {
                            continue;
                        }
                        if (layerMask != -1 && !GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        if (FixedPointIntersection.IntersectWithAABBAndAABBFixedPoint(item.min, item.max, min, max))
                        {
                            count++;
                        }
                    }
                }
                if (node.FpObbStack != null)
                {
                    for (var i = 0; i < node.FpObbStack.Count; i++)
                    {
                        var item = node.FpObbStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled)
                        {
                            continue;
                        }
                        if (item.isTrigger && !includeTrigger)
                        {
                            continue;
                        }
                        if (layerMask != -1 && !GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        if (FixedPointIntersection.IntersectWithAABBAndOBBFixedPoint(min, max, item))
                        {
                            count++;
                        }
                    }
                }
                if (node.nodes != null)
                {
                    foreach (var item in node.nodes)
                    {
                        if(item.colliderCount <= 0) continue;
                        if (FixedPointIntersection.IntersectWithAABBAndAABBFixedPoint(item.fixedPointAABB.Min, item.fixedPointAABB.Max, min, max))
                        {
                            openList.Add(item);
                        }
                    }
                }
            }
            return count;
        }
        public int OverlapSphere(FixedPointVector3 position, FixedPoint64 radius,ref List<FPCollider> colliders ,int layerMask = -1, bool includeTrigger = false)
        {
            int count = 0;
            openList.Add(root);
            castIndex++;
            var min = position - new FixedPointVector3(radius, radius, radius);
            var max = position + new FixedPointVector3(radius, radius, radius);
            if (castIndex == int.MaxValue)
            {
                castIndex = 0;
            }
            while (openList.Count > openListIndex)
            {
                var node = openList[openListIndex];
                openListIndex++;
                if (node.FpSphereStack != null)
                {
                    for (var i = 0; i < node.FpSphereStack.Count; i++)
                    {
                        var item = node.FpSphereStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled)
                        {
                            continue;
                        }
                        if (item.isTrigger && !includeTrigger)
                        {
                            continue;
                        }
                        if (layerMask != -1 && !GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        if ((radius + item.radius) * (radius + item.radius) > (item.position - position).sqrMagnitude)
                        {
                            if (colliders.Count == count)
                            {
                                colliders.Add(item);
                            }
                            else
                            {
                                colliders[count] = item;
                            }
                            count++;
                        }
                    }
                }
                if (node.FpAABBStack != null)
                {
                    for (var i = 0; i < node.FpAABBStack.Count; i++)
                    {
                        var item = node.FpAABBStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled)
                        {
                            continue;
                        }
                        if (item.isTrigger && !includeTrigger)
                        {
                            continue;
                        }
                        if (layerMask != -1 && !GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        if (FixedPointIntersection.IntersectWithAABBAndSphere(item.min, item.max, position, radius))
                        {
                            if (colliders.Count == count)
                            {
                                colliders.Add(item);
                            }
                            else
                            {
                                colliders[count] = item;
                            }
                            count++;
                        }
                    }
                }
                FPCollision fpCollision;
                if (node.FpObbStack != null)
                {
                    for (var i = 0; i < node.FpObbStack.Count; i++)
                    {
                        var item = node.FpObbStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled)
                        {
                            continue;
                        }
                        if (item.isTrigger && !includeTrigger)
                        {
                            continue;
                        }
                        if (!GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        fpCollision = FixedPointIntersection.IntersectWithSphereAndOBB(position, radius, item.position, item.halfSize, item.fpTransform.fixedPointMatrix);
                        if (fpCollision.hit)
                        {
                            if (colliders.Count == count)
                            {
                                colliders.Add(item);
                            }
                            else
                            {
                                colliders[count] = item;
                            }
                            count++;
                        }
                    }
                }
                if (node.FpCapsuleStack != null)
                {
                    for (var i = 0; i < node.FpCapsuleStack.Count; i++)
                    {
                        var item = node.FpCapsuleStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled)
                        {
                            continue;
                        }
                        if (item.isTrigger && !includeTrigger)
                        {
                            continue;
                        }
                        if (!GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        fpCollision = FixedPointIntersection.IntersectWithSphereAndCapsule(position, radius, item.startPos, item.endPos, item.scaledRadius);
                        if (fpCollision.hit)
                        {
                            if (colliders.Count == count)
                            {
                                colliders.Add(item);
                            }
                            else
                            {
                                colliders[count] = item;
                            }
                            count++;
                        }
                    }
                }
                if (node.FpCylinderStack != null)
                {
                    for (var i = 0; i < node.FpCylinderStack.Count; i++)
                    {
                        var item = node.FpCylinderStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled)
                        {
                            continue;
                        }
                        if (item.isTrigger && !includeTrigger)
                        {
                            continue;
                        }
                        if (!GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        fpCollision = FixedPointIntersection.IntersectWithSphereAndCylinder(position, radius, item.startPos, item.endPos, item.radius);
                        if (fpCollision.hit)
                        {
                            if (colliders.Count == count)
                            {
                                colliders.Add(item);
                            }
                            else
                            {
                                colliders[count] = item;
                            }
                            count++;
                        }
                    }
                }
                if (node.FpAACapsuleStack != null)
                {
                    for (var i = 0; i < node.FpAACapsuleStack.Count; i++)
                    {
                        var item = node.FpAACapsuleStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled)
                        {
                            continue;
                        }
                        if (item.isTrigger && !includeTrigger)
                        {
                            continue;
                        }
                        if (!GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        fpCollision = FixedPointIntersection.IntersectWithSphereAndAACapsule(position, radius, item.startPos, item.endPos, item.scaledRadius);
                        if (fpCollision.hit)
                        {
                            if (colliders.Count == count)
                            {
                                colliders.Add(item);
                            }
                            else
                            {
                                colliders[count] = item;
                            }
                            count++;
                        }
                    }
                }
                if (node.nodes != null)
                {
                    foreach (var item in node.nodes)
                    {
                        if(item.colliderCount <= 0) continue;
                        if (FixedPointIntersection.IntersectWithAABBAndAABBFixedPoint(item.fixedPointAABB.Min, item.fixedPointAABB.Max, min, max))
                        {
                            openList.Add(item);
                        }
                    }
                }
            }
            return count;
        }
        public int OverlayAACapsuleCollision(FixedPointVector3 startPos, FixedPointVector3 endPos, FixedPoint64 radius,ref List<FPCollision> collisions, int layerMask = -1, bool includeTrigger = false,bool dynamic= false)
        {
            var count = 0;
            openList.Add(root);
            castIndex++;
            var min = startPos - new FixedPointVector3(radius,radius,radius);
            var max = endPos + new FixedPointVector3(radius, radius, radius);
            if (castIndex == int.MaxValue)
            {
                castIndex = 0;
            }
            while (openList.Count > openListIndex)
            {
                var node = openList[openListIndex];
                openListIndex++;
                FPCollision fpCollision;
                if (node.FpSphereStack != null)
                {
                    for (var i = 0; i < node.FpSphereStack.Count; i++)
                    {
                        var item = node.FpSphereStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled)
                        {
                            continue;
                        }
                        if (item.isTrigger && !includeTrigger)
                        {
                            continue;
                        }
                        if (item.isDynamic != dynamic)
                        {
                            continue;
                        }
                        if (layerMask != -1 && !GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        fpCollision = FixedPointIntersection.IntersectWithAACapsuleAndSphere(startPos, endPos, radius, item.position, item.scaledRadius);
                        if (!fpCollision.hit) continue;
                        fpCollision.collider = item;
                        if (collisions.Count == count)
                        {
                            collisions.Add(fpCollision);
                        }
                        else
                        {
                            collisions[count] = fpCollision;
                        }
                        count++;
                    }
                }
                if (node.FpAABBStack != null)
                {
                    for (var i = 0; i < node.FpAABBStack.Count; i++)
                    {
                        var item = node.FpAABBStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled)
                        {
                            continue;
                        }
                        if (item.isTrigger && !includeTrigger)
                        {
                            continue;
                        }
                        if (item.isDynamic != dynamic)
                        {
                            continue;
                        }
                        if (layerMask != -1 && !GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        if (!FixedPointIntersection.IntersectWithAABBAndAABBFixedPoint(min, max, item.min, item.max))
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        fpCollision = FixedPointIntersection.IntersectWithAACapsuleAndAABB(startPos, endPos, radius, item.min, item.max);
                        if (!fpCollision.hit) continue;
                        fpCollision.collider = item;
                        if (collisions.Count == count)
                        {
                            collisions.Add(fpCollision);
                        }
                        else
                        {
                            collisions[count] = fpCollision;
                        }
                        count++;
                    }
                }
                if (node.FpObbStack != null)
                {
                    for (var i = 0; i < node.FpObbStack.Count; i++)
                    {
                        var item = node.FpObbStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled)
                        {
                            continue;
                        }
                        if (item.isTrigger && !includeTrigger)
                        {
                            continue;
                        }
                        if (item.isDynamic != dynamic)
                        {
                            continue;
                        }
                        if (!GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        if (!FixedPointIntersection.IntersectWithAABBAndAABBFixedPoint(min, max, item.min, item.max))
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        var matrix = FixedPointMatrix.CreateFromQuaternion(item.fpTransform.rotation);
                        fpCollision = FixedPointIntersection.IntersectWithAACapsuleAndOBB(startPos, endPos, radius, item.position, item.halfSize, matrix, item.min, item.max);
                        //fixedPointCollision = FixedPointIntersection.IntersectWithSphereAndOBB(startPos,  radius, item.position, item.halfSize, matrix);
                        if (!fpCollision.hit) continue;
                        fpCollision.collider = item;
                        if (collisions.Count == count)
                        {
                            collisions.Add(fpCollision);
                        }
                        else
                        {
                            collisions[count] = fpCollision;
                        }
                        count++;
                    }
                }
                if (node.FpCapsuleStack != null)
                {
                    for (var i = 0; i < node.FpCapsuleStack.Count; i++)
                    {
                        var item = node.FpCapsuleStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled)
                        {
                            continue;
                        }
                        if (item.isTrigger && !includeTrigger)
                        {
                            continue;
                        }
                        if (item.isDynamic != dynamic)
                        {
                            continue;
                        }
                        if (!GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        if (!FixedPointIntersection.IntersectWithAABBAndAABBFixedPoint(min, max, item.min, item.max))
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        fpCollision = FixedPointIntersection.IntersectWithCapsuleAndCapsule(startPos, endPos, radius, item.startPos, item.endPos, item.scaledRadius);
                        if (!fpCollision.hit) continue;
                        fpCollision.collider = item;
                        if (collisions.Count == count)
                        {
                            collisions.Add(fpCollision);
                        }
                        else
                        {
                            collisions[count] = fpCollision;
                        }
                        count++;
                    }
                }
                if (node.FpCylinderStack != null)
                {
                    for (var i = 0; i < node.FpCylinderStack.Count; i++)
                    {
                        var item = node.FpCylinderStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled)
                        {
                            continue;
                        }
                        if (item.isTrigger && !includeTrigger)
                        {
                            continue;
                        }
                        if (item.isDynamic != dynamic)
                        {
                            continue;
                        }
                        if (!GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        if (!FixedPointIntersection.IntersectWithAABBAndAABBFixedPoint(min, max, item.min, item.max))
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        fpCollision = FixedPointIntersection.IntersectWithCapsuleAndCylinder(startPos, endPos, radius, item.startPos, item.endPos, item.radius);
                        if (!fpCollision.hit) continue;
                        fpCollision.collider = item;
                        if (collisions.Count == count)
                        {
                            collisions.Add(fpCollision);
                        }
                        else
                        {
                            collisions[count] = fpCollision;
                        }
                        count++;
                    }
                }
                if (node.FpAACapsuleStack != null)
                {
                    for (var i = 0; i < node.FpAACapsuleStack.Count; i++)
                    {
                        var item = node.FpAACapsuleStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled)
                        {
                            continue;
                        }
                        if (item.isTrigger && !includeTrigger)
                        {
                            continue;
                        }
                        if (item.isDynamic != dynamic)
                        {
                            continue;
                        }
                        if (!GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        if (!FixedPointIntersection.IntersectWithAABBAndAABBFixedPoint(min, max, item.min, item.max))
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        fpCollision = FixedPointIntersection.IntersectWithAACapsuleAndAACapsule(startPos, endPos, radius, item.startPos, item.endPos, item.scaledRadius);
                        if (!fpCollision.hit) continue;
                        fpCollision.collider = item;
                        if (collisions.Count == count)
                        {
                            collisions.Add(fpCollision);
                        }
                        else
                        {
                            collisions[count] = fpCollision;
                        }
                        count++;
                    }
                }
                if (node.nodes != null)
                {
                    foreach (var item in node.nodes)
                    {
                        //if(item.subColliderCount <= 0) continue;
                        if (FixedPointIntersection.IntersectWithAABBAndAABBFixedPoint(item.fixedPointAABB.Min, item.fixedPointAABB.Max, min, max))
                        {
                            openList.Add(item);
                        }
                    }
                }
            }
            return count;
        }
        public int OverlapSphereNonAlloc(FPCollider[] colliders, FixedPointVector3 center, FixedPoint64 radius, int layerMask)
        {
            int count = 0;
            openList.Add(root);
            var min = center - new FixedPointVector3(radius, radius, radius);
            var max = center + new FixedPointVector3(radius, radius, radius);
            castIndex++;
            if (castIndex == int.MaxValue)
            {
                castIndex = 0;
            }
            while (openList.Count > openListIndex && count < colliders.Length)
            {
                var node = openList[openListIndex];
                openListIndex++;
                if (node.FpSphereStack != null)
                {
                    for (var i = 0; i < node.FpSphereStack.Count; i++)
                    {
                        var item = node.FpSphereStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled || item.isTrigger)
                        {
                            continue;
                        }
                        if (layerMask != -1 && !GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        if (FixedPointIntersection.IntersectWithAABBAndAABBFixedPoint(item.min, item.max, min, max))
                        {
                            if ((radius + item.radius) * (radius + item.radius) > (item.position - center).sqrMagnitude)
                            {
                                colliders[count] = item;
                                count++;
                            }
                        }
                    }
                }
                if (node.FpAABBStack != null)
                {
                    for (var i = 0; i < node.FpAABBStack.Count; i++)
                    {
                        var item = node.FpAABBStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled || item.isTrigger)
                        {
                            continue;
                        }
                        if (layerMask != -1 && !GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        if (FixedPointIntersection.IntersectWithAABBAndAABBFixedPoint(item.min, item.max, min, max))
                        {
                            if (FixedPointIntersection.IntersectWithAABBAndSphere(item.min, item.max, center, radius))
                            {
                                colliders[count] = item;
                                count++;
                            }
                        }
                    }
                }
                if (node.nodes != null)
                {
                    foreach (var item in node.nodes)
                    {
                        if(item.colliderCount <= 0) continue;
                        if (FixedPointIntersection.IntersectWithAABBAndAABBFixedPoint(item.fixedPointAABB.Min, item.fixedPointAABB.Max, min, max))
                            //if (FixedPointIntersection.IntersectWithAABBAndSphere(node.nodes[i].min, node.nodes[i].max, center, radius))
                        {
                            openList.Add(item);
                        }
                    }
                }
            }
            return count;
        }
        public List<FPCollider> OverlapAABB(FixedPointVector3 center, FixedPointVector3 size, int layerMask, bool includeTrigger = false)
        {
            var intersectColliders = new List<FPCollider>();
            var calMin = center - size / 2;
            var calMax = center + size / 2;
            var min = new FixedPointVector3(FixedPointMath.Min(calMin.x, calMax.x), FixedPointMath.Min(calMin.y, calMax.y), FixedPointMath.Min(calMin.z, calMax.z));
            var max = new FixedPointVector3(FixedPointMath.Max(calMin.x, calMax.x), FixedPointMath.Max(calMin.y, calMax.y), FixedPointMath.Max(calMin.z, calMax.z));
            openList.Add(root);
            castIndex++;
            if (castIndex == int.MaxValue)
            {
                castIndex = 0;
            }
            while (openList.Count > openListIndex)
            {
                var node = openList[openListIndex];
                openListIndex++;
                if (node.FpSphereStack != null)
                {
                    for (var i = 0; i < node.FpSphereStack.Count; i++)
                    {
                        var item = node.FpSphereStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled)
                        {
                            continue;
                        }
                        if (item.isTrigger && !includeTrigger)
                        {
                            continue;
                        }
                        if (layerMask != -1 && !GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        if (FixedPointIntersection.IntersectWithAABBAndSphere(min, max, item.position, item.radius))
                        {
                            intersectColliders.Add(item);
                        }
                    }
                }
                if (node.FpAABBStack != null)
                {
                    for (var i = 0; i < node.FpAABBStack.Count; i++)
                    {
                        var item = node.FpAABBStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled)
                        {
                            continue;
                        }
                        if (item.isTrigger && !includeTrigger)
                        {
                            continue;
                        }
                        if (layerMask != -1 && !GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        if (FixedPointIntersection.IntersectWithAABBAndAABBFixedPoint(item.min, item.max, min, max))
                        {
                            intersectColliders.Add(item);
                        }
                    }
                }
                if (node.FpObbStack != null)
                {
                    for (var i = 0; i < node.FpObbStack.Count; i++)
                    {
                        var item = node.FpObbStack[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (!item.enabled)
                        {
                            continue;
                        }
                        if (item.isTrigger && !includeTrigger)
                        {
                            continue;
                        }
                        if (layerMask != -1 && !GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
                        {
                            continue;
                        }
                        if (item.castIndex == castIndex)
                        {
                            continue;
                        }
                        item.castIndex = castIndex;
                        if (FixedPointIntersection.IntersectWithAABBAndOBBFixedPoint(min, max, item))
                        {
                            intersectColliders.Add(item);
                        }
                    }
                }
                if (node.nodes != null)
                {
                    foreach (var item in node.nodes)
                    {
                        if(item.colliderCount <= 0) continue;
                        if (FixedPointIntersection.IntersectWithAABBAndAABBFixedPoint(item.fixedPointAABB.Min, item.fixedPointAABB.Max, min, max))
                        {
                            openList.Add(item);
                        }
                    }
                }
            }
            return intersectColliders;
        }
        public int OverlayCharacterWithCapsule(FixedPointVector3 position, FixedPoint64 height, FixedPointVector3 startPos, FixedPointVector3 endPos, FixedPoint64 radius,ref List<FPCollision> collisions)
        {
            var count = 0;
            openList.Add(root);
            castIndex++;
            if (castIndex == int.MaxValue)
            {
                castIndex = 0;
            }
            var bound = height * 0.5 + radius;
            while (openList.Count > openListIndex)
            {
                var node = openList[openListIndex];
                openListIndex++;
                //foreach (var item in node.intersectCharacterControllers)
                for (var i = 0; i < node.FpCharacterStack.Count; i++)
                {
                    var item = node.FpCharacterStack[i];
                    if (item == null)
                    {
                        continue;
                    }
                    if (!item.enabled)
                    {
                        continue;
                    }
                    if (item.castIndex == castIndex)
                    {
                        continue;
                    }
                    item.castIndex = castIndex;
                    var fixedPointCollision = FixedPointIntersection.IntersectWithSphereAndCapsule(item.position, item.scaledRadius, startPos, endPos, radius);
                    if (fixedPointCollision.hit)
                    {
                        fixedPointCollision.collider = item;
                        if (collisions.Count == count)
                        {
                            collisions.Add(fixedPointCollision);
                        }
                        else
                        {
                            collisions[count] = fixedPointCollision;
                        }
                        count++;
                    }
                }
                foreach (var item in node.nodes)
                {
                    if(item.colliderCount <= 0) continue;
                    if (FixedPointIntersection.IsIntersectWithSphereAndAABB(position, bound, item.fixedPointAABB.Min, item.fixedPointAABB.Max))
                    {
                        openList.Add(item);
                    }
                }
            }
            return count;
        }
        public int OverlayCharacterWithSphere(FPSphereCollider fixedPointFpSphereCollider, ref List<FPCollision> collisions)
        {
            var count = 0;
            var radius = fixedPointFpSphereCollider.radius;
            fixedPointFpSphereCollider.UpdateAABB();
            var min = fixedPointFpSphereCollider.min;
            var max = fixedPointFpSphereCollider.max;
            openList.Add(root);
            castIndex++;
            if (castIndex == int.MaxValue)
            {
                castIndex = 0;
            }
            while (openList.Count > openListIndex)
            {
                var node = openList[openListIndex];
                openListIndex++;
                for (var i = 0; i < node.FpCharacterStack.Count; i++)
                {
                    var item = node.FpCharacterStack[i];
                    if (item == null)
                    {
                        continue;
                    }
                    if (!item.enabled)
                    {
                        continue;
                    }
                    if (item.castIndex == castIndex)
                    {
                        continue;
                    }
                    item.castIndex = castIndex;
                    if (!FixedPointIntersection.IntersectWithAABBAndAABBFixedPoint(min, max, item.min, item.max))
                    {
                        var fixedPointCollision = FixedPointIntersection.IntersectWithSphereAndAACapsule(fixedPointFpSphereCollider.position, radius, item.startPos, item.endPos, item.scaledRadius);
                        if (fixedPointCollision.hit)
                        {
                            fixedPointCollision.collider = item;
                            if (collisions.Count == count)
                            {
                                collisions.Add(fixedPointCollision);
                            }
                            else
                            {
                                collisions[count] = fixedPointCollision;
                            }
                            count++;
                        }
                    }
                }
                if (node.nodes != null)
                {
                    foreach (var item in node.nodes)
                    {
                        if(item.colliderCount <= 0) continue;
                        if (FixedPointIntersection.IntersectWithAABBAndAABBFixedPoint(min, max, item.fixedPointAABB.Min, item.fixedPointAABB.Max))
                        {
                            openList.Add(item);
                        }
                    }
                }
            }
            return count;
        }
    }
}