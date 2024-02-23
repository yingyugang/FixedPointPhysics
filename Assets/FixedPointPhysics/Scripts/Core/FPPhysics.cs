using BlueNoah.Math.FixedPoint;
using System.Collections.Generic;
using BlueNoah.Common;

namespace BlueNoah.PhysicsEngine
{
    public class PhysicsSearch
    {
        public readonly FPFastList<FPOctreeNode> openList = new ();
    }
    
    public static class FPPhysics
    {
        public static List<FPCollider> Colliders => FPPhysicsPresenter.Instance.fpOctree.colliders;

        private static readonly ObjectPool<PhysicsSearch> searchPool = new ();
        
        public static int OverlapSphere(FixedPointVector3 position, FixedPoint64 radius, ref List<FPCollider> colliders, int layerMask = -1, bool includeTrigger = false)
        {
            return FPPhysicsPresenter.Instance.fpOctree.OverlapSphere(position, radius, ref colliders, layerMask, includeTrigger);
        }

        public static int OverlayCharacterWithCapsule(FixedPointVector3 position, FixedPoint64 height, FixedPointVector3 startPos, FixedPointVector3 endPos, FixedPoint64 radius, ref List<FPCollision> collisions)
        {
            return FPPhysicsPresenter.Instance.fpOctree.OverlayCharacterWithCapsule(position, radius, startPos, endPos, radius, ref collisions);
        }
        
        public static List<FPCollision> OverlayBoxCollision(FixedPointVector3 position, FixedPointVector3 halfSize, FixedPointMatrix orientation, int layerMask = -1, bool includeTrigger = false)
        {
            return FPPhysicsPresenter.Instance.fpOctree.OverlayBoxCollision(position, halfSize, orientation, layerMask, includeTrigger);
        }
        
        public static FixedPoint64 SqrDistanceToLine(FixedPointRay ray, FixedPointVector3 point)
        {
            return FixedPointVector3.Cross(ray.Dir, point - ray.Point).sqrMagnitude;
        }

        /// <summary>
        /// 判断Node是否可用
        /// </summary>
        /// <param name="item"></param>
        /// <param name="layerMask"></param>
        /// <param name="includeTrigger"></param>
        /// <returns></returns>
        private static bool IsNodeInValidate(FPCollider item, int layerMask, bool includeTrigger)
        {
            if (item == null)
            {
                return true;
            }
            if (!item.enabled)
            {
                return true;
            }
            if (item.isTrigger && !includeTrigger)
            {
                return true;
            }
            return layerMask != -1 && !GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer);
        }
        
        private static bool IsNodeInValidate(FPCollider item, int layerMask, bool includeTrigger,FixedPointVector3 minA, FixedPointVector3 maxA, FixedPointVector3 minB, FixedPointVector3 maxB)
        {
            if (IsNodeInValidate(item,layerMask,includeTrigger))
            {
                return true;
            }
            return !FixedPointIntersection.IntersectWithAABBAndAABBFixedPoint(minA,maxA,minB,maxB);
        }
        
        /// <summary>
        /// TODO 缺少Mesh的Raycast检测
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direct"></param>
        /// <param name="length"></param>
        /// <param name="fpRaycastHit"></param>
        /// <param name="layerMask"></param>
        /// <param name="includeTrigger"></param>
        /// <returns></returns>
        public static bool Raycast(FixedPointVector3 origin, FixedPointVector3 direct, FixedPoint64 length, out FPRaycastHit fpRaycastHit, int layerMask, bool includeTrigger)
        {
            var fixedPointRay = new FixedPointRay(origin, direct);
            FPCollider fpCollider = null;
            fpRaycastHit = null;
            var intersection = FixedPointVector3.zero;
            var outPoint = FixedPointVector3.zero;
            var normal = FixedPointVector3.zero;
            var sqrDistance = FixedPoint64.MaxValue;
            var fpOctree = FPPhysicsPresenter.Instance.fpOctree;
            var physicsSearch = searchPool.Pull();
            physicsSearch.openList.Clear();
            physicsSearch.openList.Add(fpOctree.root);
            while (physicsSearch.openList.Count > 0)
            {
                var node = physicsSearch.openList[0];
                physicsSearch.openList.Remove(node);
                FPCollision fpCollision;
                FixedPoint64 currentDistance;
                if (node.FpSphereStack != null)
                {
                    for (var i = 0; i < node.FpSphereStack.Count; i++)
                    {
                        var item = node.FpSphereStack[i];
                        if (IsNodeInValidate(item, layerMask, includeTrigger))
                        {
                            continue;
                        }
                        if (FixedPointIntersection.IntersetWithRayAndSphereFixedPoint(origin, direct, length, item.position, item.radius, out fpCollision))
                        {
                            currentDistance = fpCollision.t * fpCollision.t;
                            if (currentDistance < sqrDistance)
                            {
                                sqrDistance = currentDistance;
                                intersection = fpCollision.closestPoint;
                                normal = fpCollision.normal;
                                fpCollider = item;
                            }
                        }
                    }
                }
                if (node.FpAABBStack != null)
                {
                    for (var i = 0; i < node.FpAABBStack.Count; i++)
                    {
                        var item = node.FpAABBStack[i];
                        if (IsNodeInValidate(item, layerMask, includeTrigger))
                        {
                            continue;
                        }
                        if (FixedPointIntersection.IntersectWithRayAndAABBFixedPoint(fixedPointRay.Point, fixedPointRay.Dir * length, item.min, item.max, out fpCollision) != FixedPoint64.MaxValue)
                        {
                            currentDistance = (fpCollision.closestPoint - origin).sqrMagnitude;
                            if (currentDistance < sqrDistance)
                            {
                                sqrDistance = currentDistance;
                                intersection = fpCollision.closestPoint;
                                normal = fpCollision.normal;
                                fpCollider = item;
                            }
                        }
                    }
                }
                if (node.FpObbStack != null)
                {
                    for (var i = 0; i < node.FpObbStack.Count; i++)
                    {
                        var item = node.FpObbStack[i];
                        if (IsNodeInValidate(item, layerMask, includeTrigger))
                        {
                            continue;
                        }
                        if (FixedPointIntersection.IntersectWithRayAndAABBFixedPoint(fixedPointRay.Point, fixedPointRay.Dir * length, item.min, item.max, out fpCollision) == FixedPoint64.MaxValue)
                        {
                            continue;
                        }
                        if (FixedPointIntersection.IntersectWithRayAndOBBFixedPoint(fixedPointRay.Point, fixedPointRay.Dir, length, item.position, item.halfSize, item.fpTransform.fixedPointMatrix, out fpCollision) > 0)
                        {
                            currentDistance = (fpCollision.closestPoint - origin).sqrMagnitude;
                            if (currentDistance < sqrDistance)
                            {
                                sqrDistance = currentDistance;
                                intersection = fpCollision.closestPoint;
                                normal = fpCollision.normal;
                                outPoint = fpCollision.outsidePoint;
                                fpCollider = item;
                            }
                        }
                    }
                }
                if (node.nodes != null)
                {
                    foreach (var item in node.nodes)
                    {
                        if(item.colliderCount <= 0) continue;
                        if (FixedPointIntersection.PointInAABB(fixedPointRay.Point, item.fixedPointAABB.Min, item.fixedPointAABB.Max) 
                            || FixedPointIntersection.IntersectWithRayAndAABBFixedPoint(fixedPointRay.Point, fixedPointRay.Dir * length, item.fixedPointAABB.Min, item.fixedPointAABB.Max, out fpCollision) != FixedPoint64.MaxValue)
                        {
                            physicsSearch.openList.Add(item);
                        }
                    }
                }
            }
            searchPool.Push(physicsSearch);
            if (fpCollider == null) return false;
            fpRaycastHit = new FPRaycastHit(fpCollider, intersection, normal, outPoint);
            return true;
        }
        
        public static int OverlaySphereCollision(FixedPointVector3 position, FixedPoint64 radius,ref List<FPCollision> collisions, int layerMask = -1, bool includeTrigger = false)
        {
            var count = 0;
            var fpOctree = FPPhysicsPresenter.Instance.fpOctree;
            var physicsSearch = searchPool.Pull();
            physicsSearch.openList.Clear();
            physicsSearch.openList.Add(fpOctree.root);
            var min = position - new FixedPointVector3(radius,radius,radius);
            var max = position + new FixedPointVector3(radius, radius, radius);
            while (physicsSearch.openList.Count > 0)
            {
                var node =  physicsSearch.openList[0];
                physicsSearch.openList.Remove(node);
                FPCollision fpCollision;
                if (node.FpSphereStack != null)
                {
                    for (var i = 0; i < node.FpSphereStack.Count; i++)
                    {
                        var item = node.FpSphereStack[i];
                        if (IsNodeInValidate(item, layerMask, includeTrigger))
                        {
                            continue;
                        }
                        fpCollision = FixedPointIntersection.IntersectWithSphereAndSphere(position, radius, item.position, item.scaledRadius);
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
                        if (IsNodeInValidate(item, layerMask, includeTrigger))
                        {
                            continue;
                        }
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
                        if (IsNodeInValidate(item, layerMask, includeTrigger,min,max,item.min,item.max))
                        {
                            continue;
                        }
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
                        if (IsNodeInValidate(item, layerMask, includeTrigger,min,max,item.min,item.max))
                        {
                            continue;
                        }
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
                        if (IsNodeInValidate(item, layerMask, includeTrigger,min,max,item.min,item.max))
                        {
                            continue;
                        }
                        fpCollision = FixedPointIntersection.IntersectWithSphereAndCylinder(position, radius, item.startPos, item.endPos, item.scaledRadius);
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
                        if (IsNodeInValidate(item, layerMask, includeTrigger,min,max,item.min,item.max))
                        {
                            continue;
                        }
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
                if (node.FpMeshStack != null)
                {
                    for (var i = 0; i < node.FpMeshStack.Count; i++)
                    {
                        var item = node.FpMeshStack[i];
                        if (IsNodeInValidate(item, layerMask, includeTrigger,min,max,item.min,item.max))
                        {
                            continue;
                        }
                        fpCollision = FixedPointIntersection.IntersectWithSphereAndMesh(position, radius, item);
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
                if (node.nodes == null) continue;
                {
                    foreach (var item in node.nodes)
                    {
                        if(item.colliderCount <= 0) continue;
                        if (FixedPointIntersection.IntersectWithAABBAndSphere(item.fixedPointAABB.Min, item.fixedPointAABB.Max, position, radius))
                        {
                            physicsSearch.openList.Add(item);
                        }
                    }
                }
            }
            searchPool.Push(physicsSearch);
            return count;
        }
        
    }
}