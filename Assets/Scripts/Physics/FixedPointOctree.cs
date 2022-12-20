using BlueNoah.Common;
using BlueNoah.Math.FixedPoint;
using System.Collections.Generic;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    public class FixedPointOctree
    {
        public FixedPointOctreeNode root;
        public List<FixedPointCollider> colliders = new List<FixedPointCollider>();
        public List<FixedPointOctreeNode> nodes = new List<FixedPointOctreeNode>();
        public List<FixedPointOctreeNode> unitNodes = new List<FixedPointOctreeNode>();
        int castIndex;
        public int size { get; private set; }
        public bool IsOutOfBound(FixedPointVector3 position)
        {
            if (position.x > size || position.x < -size || position.y > size || position.y < -size || position.z > size || position.z < -size)
            {
                return true;
            }
            return false;
        }
        public void Reset()
        {
            colliders.Clear();
            foreach (var item in nodes)
            {
                item.intersectedSphereColliders.Clear();
                item.intersectedAABBColliders.Clear();
                item.intersectedOBBColliders.Clear();
                item.intersectedTriangleColliders.Clear();
            }
        }
        public void AddCollider(FixedPointCollider fixedPointCollider)
        {
            colliders.Add(fixedPointCollider);
        }
        //Fast AABB checking
        //Just in cases of updown check. Only for test AABB.
        public FixedPointCollider Raycast2DAABB(FixedPoint64 x, FixedPoint64 z, int layerMask)
        {
            FixedPointCollider collider = null;
            openList.Add(root);
            castIndex++;
            if (castIndex == int.MaxValue)
            {
                castIndex = 0;
            }
            var y = -FixedPoint64.MaxValue;
            while (openList.Count > openListIndex)
            {
                var node = openList[openListIndex];
                openListIndex++;
                foreach (var item in node.intersectedAABBColliders)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    if (!item.enabled || item.isTrigger)
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
                    if (item.max.y > y)
                    {
                        if (x >= item.min.x && x <= item.max.x && z >= item.min.z && z <= item.max.z)
                        {
                            collider = item;
                            y = item.position.y;
                        }
                    }
                }
                if (node.nodes!=null)
                {
                    for (int i = 0; i < node.nodes.Length; i++)
                    {
                        if (y < node.nodes[i].fixedPointAABB.Max.y)
                        {
                            if (x >= node.nodes[i].fixedPointAABB.Min.x && x <= node.nodes[i].fixedPointAABB.Max.x && z >= node.nodes[i].fixedPointAABB.Min.z && z <= node.nodes[i].fixedPointAABB.Max.z)
                            {
                                openList.Add(node.nodes[i]);
                            }
                        }
                    }
                }
            }
            return collider;
        }
        public List<FixedPointCollider> RaycastAll(FixedPointVector3 origin, FixedPointVector3 direct, FixedPoint64 length, int layerMask, bool includeTrigger = false)
        {
            var fixedPointRay = new FixedPointRay(origin, direct);
            var colliders = new List<FixedPointCollider>();
            FixedPointCollision fixedPointCollision;
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
                foreach (var item in node.intersectedSphereColliders)
                {
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
                    if (FixedPointIntersection.IntersetWithRayAndSphereFixedPoint(origin, direct, length, item.position, item.radius,out fixedPointCollision))
                    {
                        colliders.Add(item);
                    }
                }
                foreach (var item in node.intersectedAABBColliders)
                {
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
                    if (FixedPointIntersection.IntersectWithRayAndAABBFixedPoint(fixedPointRay.Point, fixedPointRay.Dir * length, item.min, item.max, out fixedPointCollision) != FixedPoint64.MaxValue)
                    {
                        colliders.Add(item);
                    }
                }
                foreach (var item in node.intersectedOBBColliders)
                {
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
                    if (FixedPointIntersection.IntersectWithRayAndOBBFixedPoint(fixedPointRay.Point, fixedPointRay.Dir ,length, item.position, item.halfSize,item.fixedPointTransform.fixedPointMatrix, out fixedPointCollision) > 0)
                    {
                        colliders.Add(item);
                    }
                }
                foreach (var item in node.intersectedTriangleColliders)
                {
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
                    if (FixedPointIntersection.IntersectWithRayAndTriangleFixedPoint(fixedPointRay.Point, fixedPointRay.Dir, item, out fixedPointCollision))
                    {
                        colliders.Add(item);
                    }
                }
                if (node.nodes != null)
                {
                    for (int i = 0; i < node.nodes.Length; i++)
                    {
                        if (FixedPointIntersection.IntersectWithRayAndAABBFixedPoint(fixedPointRay.Point, fixedPointRay.Dir * length, node.nodes[i].fixedPointAABB.Min, node.nodes[i].fixedPointAABB.Max, out fixedPointCollision) != FixedPoint64.MaxValue)
                        {
                            openList.Add(node.nodes[i]);
                        }
                    }
                }
            }
            return colliders;
        }
        public bool Raycast(FixedPointVector3 origin, FixedPointVector3 direct, FixedPoint64 length, out FixedPointRaycastHit fixedPointRaycastHit, int layerMask, bool includeTrigger = false)
        {
            var fixedPointRay = new FixedPointRay(origin, direct);
            FixedPointCollider collider = null;
            fixedPointRaycastHit = null;
            var intersection = FixedPointVector3.zero;
            var normal = FixedPointVector3.zero;
            FixedPointCollision fixedPointCollision;
            FixedPoint64 currentDistance;
            var sqrDistance = FixedPoint64.MaxValue;
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
               
                    foreach (var item in node.intersectedSphereColliders)
                    {
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
                        if (FixedPointIntersection.IntersetWithRayAndSphereFixedPoint(origin, direct, length, item.position, item.radius, out fixedPointCollision))
                        {
                            currentDistance = fixedPointCollision.t * fixedPointCollision.t;
                            if (currentDistance < sqrDistance)
                            {
                                sqrDistance = currentDistance;
                                intersection = fixedPointCollision.closestPoint;
                                normal = fixedPointCollision.normal;
                                collider = item;
                            }
                        }
                    }
                    foreach (var item in node.intersectedAABBColliders)
                    {
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
                        /*
                        if (FixedPointIntersection.IntersectionWithRayAndAABBFixedPointA(fixedPointRay.Point, fixedPointRay.Dir, length, item.min, item.max, out fixedPointCollision))
                        {
                            currentDistance = (fixedPointCollision.point - origin).sqrMagnitude;
                            if (currentDistance < sqrDistance)
                            {
                                sqrDistance = currentDistance;
                                intersection = fixedPointCollision.point;
                                normal = fixedPointCollision.normal;
                                collider = item;
                            }
                        }*/
                        if (FixedPointIntersection.IntersectWithRayAndAABBFixedPoint(fixedPointRay.Point, fixedPointRay.Dir * length, item.min, item.max, out fixedPointCollision) != FixedPoint64.MaxValue)
                        {
                            currentDistance = (fixedPointCollision.closestPoint - origin).sqrMagnitude;
                            if (currentDistance < sqrDistance)
                            {
                                sqrDistance = currentDistance;
                                intersection = fixedPointCollision.closestPoint;
                                normal = fixedPointCollision.normal;
                                collider = item;
                            }
                        }
                        /*
                        if (FixedPointIntersection.IntersectionWithRayAndAABBFixedPointA(fixedPointRay.Point, fixedPointRay.Dir,length, item.min, item.max, out fixedPointCollision))
                        {
                            currentDistance = (fixedPointCollision.point - origin).sqrMagnitude;
                            if (currentDistance < sqrDistance)
                            {
                                sqrDistance = currentDistance;
                                intersection = fixedPointCollision.point;
                                normal = fixedPointCollision.normal;
                                collider = item;
                            }
                        }*/
                    }

                    foreach (var item in node.intersectedOBBColliders)
                    {
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
                        if (FixedPointIntersection.IntersectWithRayAndOBBFixedPoint(fixedPointRay.Point, fixedPointRay.Dir, length, item.position, item.halfSize, item.fixedPointTransform.fixedPointMatrix, out fixedPointCollision) > 0)
                        {
                            currentDistance = (fixedPointCollision.closestPoint - origin).sqrMagnitude;
                            if (currentDistance < sqrDistance)
                            {
                                sqrDistance = currentDistance;
                                intersection = fixedPointCollision.closestPoint;
                                normal = fixedPointCollision.normal;
                                collider = item;
                            }
                        }
                    }
                    foreach (var item in node.intersectedTriangleColliders)
                    {
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
                        if (FixedPointIntersection.IntersectWithRayAndTriangleFixedPoint(fixedPointRay.Point, fixedPointRay.Dir, item,  out fixedPointCollision))
                        {
                            currentDistance = (fixedPointCollision.closestPoint - origin).sqrMagnitude;
                            if (currentDistance < sqrDistance)
                            {
                                sqrDistance = currentDistance;
                                intersection = fixedPointCollision.closestPoint;
                                normal = fixedPointCollision.normal;
                                collider = item;
                            }
                        }
                }
                if (node.nodes!=null)
                {
                    for (int i = 0; i < node.nodes.Length; i++)
                    {
                        if (FixedPointIntersection.PointInAABB(fixedPointRay.Point, node.nodes[i].fixedPointAABB.Min, node.nodes[i].fixedPointAABB.Max) || FixedPointIntersection.IntersectWithRayAndAABBFixedPoint(fixedPointRay.Point, fixedPointRay.Dir * length, node.nodes[i].fixedPointAABB.Min, node.nodes[i].fixedPointAABB.Max, out fixedPointCollision) != FixedPoint64.MaxValue)
                        {
                            openList.Add(node.nodes[i]);
                        }
                    }
                }
            }
            if (collider!=null)
            {
                fixedPointRaycastHit = new FixedPointRaycastHit(collider,intersection, normal);
                return true;
            }else
            {
                return false;
            }
        }
        public int RaycastNonAlloc(FixedPointVector3 origin, FixedPointVector3 direct, FixedPoint64 length,  FixedPointRaycastHit[] fixedPointRaycastHit, int layerMask, bool includeTrigger = false)
        {
            var fixedPointRay = new FixedPointRay(origin, direct);
            int count = 0;
            FixedPointCollision fixedPointCollision;
            openList.Add(root);
            castIndex++;
            if (castIndex == int.MaxValue)
            {
                castIndex = 0;
            }
            while (openList.Count > openListIndex && count < fixedPointRaycastHit.Length)
            {
                var node = openList[openListIndex];
                openListIndex++;
                
                    foreach (var item in node.intersectedSphereColliders)
                    {
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
                        if (FixedPointIntersection.IntersetWithRayAndSphereFixedPoint(origin, direct, length, item.position, item.radius, out fixedPointCollision))
                        {
                            fixedPointRaycastHit[count] = new FixedPointRaycastHit(item, fixedPointCollision.closestPoint,fixedPointCollision.normal);
                            count++;
                        }
                    }
                    foreach (var item in node.intersectedAABBColliders)
                    {
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

                        if (FixedPointIntersection.IntersectWithRayAndAABBFixedPoint(fixedPointRay.Point, fixedPointRay.Dir * length, item.min, item.max, out fixedPointCollision) != FixedPoint64.MaxValue)
                        {
                            fixedPointRaycastHit[count] = new FixedPointRaycastHit(item, fixedPointCollision.closestPoint, fixedPointCollision.normal);
                            count++;
                        }
                        /*
                        if (FixedPointIntersection.IntersectionWithRayAndAABBFixedPointA(fixedPointRay.Point, fixedPointRay.Dir,length, item.min, item.max, out fixedPointCollision))
                        {
                            fixedPointRaycastHit[count] = new FixedPointRaycastHit(item, fixedPointCollision.point, fixedPointCollision.normal);
                            count++;
                        }*/
                    }
                    foreach (var item in node.intersectedOBBColliders)
                    {
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
                        if (FixedPointIntersection.IntersectWithRayAndOBBFixedPoint(fixedPointRay.Point, fixedPointRay.Dir, length, item.position, item.halfSize, item.fixedPointTransform.fixedPointMatrix, out fixedPointCollision) > 0)
                        {
                            fixedPointRaycastHit[count] = new FixedPointRaycastHit(item, fixedPointCollision.closestPoint, fixedPointCollision.normal);
                            count++;
                        }
                    }
                    foreach (var item in node.intersectedTriangleColliders)
                    {
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
                        if (FixedPointIntersection.IntersectWithRayAndTriangleFixedPoint(fixedPointRay.Point, fixedPointRay.Dir, item, out fixedPointCollision))
                        {
                            fixedPointRaycastHit[count] = new FixedPointRaycastHit(item, fixedPointCollision.closestPoint, fixedPointCollision.normal);
                            count++;
                        }
                    }
                if (node.nodes != null)
                {
                    for (int i = 0; i < node.nodes.Length; i++)
                    {
                        if (FixedPointIntersection.IntersectWithRayAndAABBFixedPoint(fixedPointRay.Point, fixedPointRay.Dir * length, node.nodes[i].fixedPointAABB.Min, node.nodes[i].fixedPointAABB.Max, out fixedPointCollision) != FixedPoint64.MaxValue)
                        {
                            openList.Add(node.nodes[i]);
                        }
                    }
                }
            }
            return count;
        }
        public static FixedPoint64 SqrDistanceToLine(FixedPointRay ray, FixedPointVector3 point)
        {
            return FixedPointVector3.Cross(ray.Dir,point - ray.Point).sqrMagnitude;
        }
        public List<FixedPointCollision> OverlayBoxCollision(FixedPointVector3 position, FixedPointVector3 halfSize, FixedPointMatrix orientation, int layerMask = -1, bool includeTrigger = false)
        {
            var collisions = new List<FixedPointCollision>();
            FixedPointCollision fixedPointCollision;
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
                
                foreach (var item in node.intersectedSphereColliders)
                {
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
                    fixedPointCollision = FixedPointIntersection.IntersectWithSphereAndOBB(item.position, item.radius, position, halfSize, orientation);
                    if (fixedPointCollision.hit)
                    {
                        fixedPointCollision.normal = - fixedPointCollision.normal;
                        collisions.Add(fixedPointCollision);
                    }
                }
                /* Only affect to Sphere now.
                foreach (var item in node.intersectedAABBColliders)
                {
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
                    fixedPointCollision = FixedPointIntersection.IntersectWithAABBAndOBBFixedPoint(item.min, item.max, position, halfSize, orientation);
                    if (fixedPointCollision.hit)
                    {
                        collisions.Add(fixedPointCollision);
                    }
                }
                foreach (var item in node.intersectedOBBColliders)
                {
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
                    fixedPointCollision = FixedPointIntersection.IntersectWithSphereAndOBB(position, radius, item.position, item.halfSize, item.fixedPointTransform.fixedPointMatrix);
                    if (fixedPointCollision.hit)
                    {
                        collisions.Add(fixedPointCollision);
                    }
                }
                foreach (var item in node.intersectedTriangleColliders)
                {
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
                    fixedPointCollision = FixedPointIntersection.IntersectWithSphereAndTriangle(position, radius, item);
                    if (fixedPointCollision.hit)
                    {
                        collisions.Add(fixedPointCollision);
                    }
                }*/
                if (node.nodes!=null)
                {
                    for (int i = 0; i < node.nodes.Length; i++)
                    {
                        if (FixedPointIntersection.IntersectWithAABBAndOBBFixedPoint(node.nodes[i].min, node.nodes[i].max, position, halfSize, orientation))
                        {
                            openList.Add(node.nodes[i]);
                            //Debug.Log(node.nodes[i]);
                        }
                    }
                }
            }
            return collisions;
        }
        public List<FixedPointCollision> OverlaySphereCollision(FixedPointVector3 position, FixedPoint64 radius, int layerMask = -1, bool includeTrigger = false)
        {
            var collisions = new List<FixedPointCollision>();
            FixedPointCollision fixedPointCollision;
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
                    foreach (var item in node.intersectedSphereColliders)
                    {
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
                        fixedPointCollision = FixedPointIntersection.IntersectWithSphereAndSphere(position, radius, item.position, item.radius);
                        if (fixedPointCollision.hit)
                        {
                            collisions.Add(fixedPointCollision);
                        }
                    }
                    foreach (var item in node.intersectedAABBColliders)
                    {
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
                        fixedPointCollision = FixedPointIntersection.IntersectWithSphereAndAABB(position, radius, item.min, item.max);
                        if (fixedPointCollision.hit)
                        {
                            collisions.Add(fixedPointCollision);
                        }
                    }
                    foreach (var item in node.intersectedOBBColliders)
                    {
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
                        fixedPointCollision = FixedPointIntersection.IntersectWithSphereAndOBB(position, radius, item.position, item.halfSize, item.fixedPointTransform.fixedPointMatrix);
                        if (fixedPointCollision.hit)
                        {
                            collisions.Add(fixedPointCollision);
                        }
                    }
                    foreach (var item in node.intersectedTriangleColliders)
                    {
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
                        fixedPointCollision = FixedPointIntersection.IntersectWithSphereAndTriangle(position, radius, item);
                        if (fixedPointCollision.hit)
                        {
                            collisions.Add(fixedPointCollision);
                        }
                    }
                if (node.nodes != null)
                {
                    for (int i = 0; i < node.nodes.Length; i++)
                    {
                        if (FixedPointIntersection.IntersectWithAABBAndSphere(node.nodes[i].min, node.nodes[i].max, position, radius))
                        {
                            openList.Add(node.nodes[i]);
                        }
                    }
                }
            }
            return collisions;
        }
        public List<FixedPointCollider> OverlapSphere(FixedPointVector3 position, FixedPoint64 radius, int layerMask = -1,bool includeTrigger = false)
        {
            var colliders = new List<FixedPointCollider>();
            FixedPointCollision fixedPointCollision;
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
                foreach (var item in node.intersectedSphereColliders)
                {
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
                    if (layerMask!= -1 && !GridLayerMask.ValidateLayerMask(layerMask, 1 << item.layer))
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
                        colliders.Add(item);
                    }
                }
                foreach (var item in node.intersectedAABBColliders)
                {
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
                        colliders.Add(item);
                    }
                }
                foreach (var item in node.intersectedOBBColliders)
                {
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
                    fixedPointCollision = FixedPointIntersection.IntersectWithSphereAndOBB(position, radius, item.position, item.halfSize, item.fixedPointTransform.fixedPointMatrix);
                    if (fixedPointCollision.hit)
                    {
                        colliders.Add(item);
                    }
                }
                foreach (var item in node.intersectedTriangleColliders)
                {
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
                    fixedPointCollision = FixedPointIntersection.IntersectWithSphereAndTriangle(position, radius,item);
                    if (fixedPointCollision.hit)
                    {
                        colliders.Add(item);
                    }
                }
                if (node.nodes != null)
                {
                    for (int i = 0; i < node.nodes.Length; i++)
                    {
                        if (FixedPointIntersection.IntersectWithAABBAndAABBFixedPoint(node.nodes[i].min, node.nodes[i].max, min, max))
                        {
                            openList.Add(node.nodes[i]);
                        }
                    }
                }
            }
            return colliders;
        }
        public int OverlapSphereNonAlloc(FixedPointCollider[] colliders, FixedPointVector3 center, FixedPoint64 radius, int layerMask)
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
                    foreach (var item in node.intersectedSphereColliders)
                    {
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
                    foreach (var item in node.intersectedAABBColliders)
                    {
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
                if (node.nodes != null)
                {
                    for (int i = 0; i < node.nodes.Length; i++)
                    {
                        if (FixedPointIntersection.IntersectWithAABBAndAABBFixedPoint(node.nodes[i].min, node.nodes[i].max, min, max))
                        //if (FixedPointIntersection.IntersectWithAABBAndSphere(node.nodes[i].min, node.nodes[i].max, center, radius))
                        {
                            openList.Add(node.nodes[i]);
                        }
                    }
                }
            }
            return count;
        }
        public List<FixedPointCollider> OverlapAABB(FixedPointVector3 center, FixedPointVector3 size,int layerMask, bool includeTrigger = false)
        {
            List<FixedPointCollider> colliders = new List<FixedPointCollider>();
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
                foreach (var item in node.intersectedSphereColliders)
                {
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
                        colliders.Add(item);
                    }
                }
                foreach (var item in node.intersectedAABBColliders)
                {
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
                        colliders.Add(item);
                    }
                }
                foreach (var item in node.intersectedOBBColliders)
                {
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
                    if (FixedPointIntersection.IntersectWithAABBAndOBBFixedPoint( min, max,item))
                    {
                        colliders.Add(item);
                    }
                }
                if (node.nodes != null)
                {
                    for (int i = 0; i < node.nodes.Length; i++)
                    {
                        if (FixedPointIntersection.IntersectWithAABBAndAABBFixedPoint(node.nodes[i].min, node.nodes[i].max, min, max))
                        {
                            openList.Add(node.nodes[i]);
                        }
                    }
                }
            }
            return colliders;
        }
        public int OverlapAABBNonAlloc(FixedPointCollider[] colliders , FixedPointVector3 center, FixedPointVector3 size, int layerMask)
        {
            int count = 0;
            var min = center - size / 2;
            var max = center + size / 2;
            openList.Add(root);
            castIndex++;
            if (castIndex == int.MaxValue)
            {
                castIndex = 0;
            }
            while (openList.Count > openListIndex && count < colliders.Length)
            {
                var node = openList[openListIndex];
                openListIndex++;
                foreach (var item in node.intersectedSphereColliders)
                {
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
                    if (FixedPointIntersection.IntersectWithAABBAndSphere(min, max, item.position, item.radius))
                    {
                        colliders[count] = item;
                        count++;
                    }
                }
                foreach (var item in node.intersectedAABBColliders)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    if (!item.enabled)
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
                        colliders[count] = item;
                        count++;
                    }
                }
                foreach (var item in node.intersectedOBBColliders)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    if (!item.enabled)
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
                    if (FixedPointIntersection.IntersectWithAABBAndOBBFixedPoint(min, max, item))
                    {
                        colliders[count] = item;
                        count++;
                    }
                }
                if (node.nodes != null)
                {
                    for (int i = 0; i < node.nodes.Length; i++)
                    {
                        if (FixedPointIntersection.IntersectWithAABBAndAABBFixedPoint(node.nodes[i].min, node.nodes[i].max, min, max))
                        {
                            openList.Add(node.nodes[i]);
                        }
                    }
                }
            }
            return count;
        }
        List<FixedPointOctreeNode> openList = new List<FixedPointOctreeNode>();
        int openListIndex = 0;
        public void UpdateColliders() {
            openList.Clear();
            openListIndex = 0;
            foreach (var item in colliders)
            {
                if (item.fixedPointTransform.isColliderDirty)
                {
                    item.UpdateCollider();
                    item.fixedPointTransform.isColliderDirty = false;
                }
            }
        }
        public void UpdateCollider(FixedPointSphereCollider fixedPointSphereCollider)
        {
            foreach (var item in fixedPointSphereCollider.impactNodes)
            {
                item.intersectedSphereColliders.Remove(fixedPointSphereCollider);
            }
            fixedPointSphereCollider.impactNodes.Clear();
            var size = fixedPointSphereCollider.radius * 2;
            var doubleSize = size * 2;
            openList.AddRange(root.nodes);
            while (openList.Count > openListIndex)
            {
                var node = openList[openListIndex];
                openListIndex++;
                if (node.nodes == null || node.size <= doubleSize)
                {
                    node.intersectedSphereColliders.Add(fixedPointSphereCollider);
                    fixedPointSphereCollider.impactNodes.Add(node);
                    continue;
                }
                if (node.nodes!=null)
                {
                    for (int i = 0; i < node.nodes.Length; i++)
                    {
                        if (node.size > size)
                        {
                            if (FixedPointIntersection.IntersectWithAABBAndSphere(node.nodes[i].min, node.nodes[i].max, fixedPointSphereCollider.fixedPointTransform.fixedPointPosition, fixedPointSphereCollider.radius))
                            {
                                openList.Add(node.nodes[i]);
                            }
                        }
                    }
                }
            }
        }
        public void UpdateCollider(FixedPointAABBCollider fixedPointAABBCollider)
        {
            foreach (var item in fixedPointAABBCollider.impactNodes)
            {
                item.intersectedAABBColliders.Remove(fixedPointAABBCollider);
            }
            fixedPointAABBCollider.impactNodes.Clear();
            var size = FixedPointMath.Max(FixedPointMath.Max(fixedPointAABBCollider.size.x, fixedPointAABBCollider.size.y), fixedPointAABBCollider.size.z);
            var doubleSize = size * 2;
            openList.AddRange(root.nodes);
            while (openList.Count > openListIndex)
            {
                var node = openList[openListIndex];
                openListIndex++;
                if (node.nodes == null || node.size <= doubleSize)
                {
                    node.intersectedAABBColliders.Add(fixedPointAABBCollider);
                    fixedPointAABBCollider.impactNodes.Add(node);
                    continue;
                }
                if (node.nodes != null)
                {
                    for (int i = 0; i < node.nodes.Length; i++)
                    {
                        if (node.size > size)
                        {
                            if (FixedPointIntersection.IntersectWithAABBAndAABBFixedPoint(node.nodes[i].min, node.nodes[i].max, fixedPointAABBCollider.min, fixedPointAABBCollider.max))
                            {
                                openList.Add(node.nodes[i]);
                            }
                        }
                    }
                }
            }
        }
        public void UpdateCollider(FixedPointOBBCollider fixedPointOBBCollider)
        {
            foreach (var item in fixedPointOBBCollider.impactNodes)
            {
                item.intersectedOBBColliders.Remove(fixedPointOBBCollider);
            }
            fixedPointOBBCollider.impactNodes.Clear();

            var size = FixedPointMath.Max(FixedPointMath.Max(fixedPointOBBCollider.size.x, fixedPointOBBCollider.size.y), fixedPointOBBCollider.size.z);
            var doubleSize = size * 2;
            openList.Add(root);
            //Debug.Log($"#{size}#{doubleSize}");
            while (openList.Count > openListIndex)
            {
                var node = openList[openListIndex];
                openListIndex++;
                //Debug.Log($"{node.size}#{size}#{doubleSize}");
                if (node.nodes == null || node.size <= doubleSize)
                {
                    //Debug.Log(doubleSize);
                    node.intersectedOBBColliders.Add(fixedPointOBBCollider);
                    fixedPointOBBCollider.impactNodes.Add(node);
                    continue;
                }
                if (node.nodes != null)
                {
                    for (int i = 0; i < node.nodes.Length; i++)
                    {
                        if (node.size > size)
                        {
                            if (FixedPointIntersection.IntersectWithAABBAndSphere(node.nodes[i].min, node.nodes[i].max,fixedPointOBBCollider.position,fixedPointOBBCollider.radius))
                            {
                                //if (FixedPointIntersection.IntersectWithAABBAndOBBFixedPoint(node.nodes[i].min, node.nodes[i].max, fixedPointOBBCollider))
                                {
                                    openList.Add(node.nodes[i]);
                                }
                            }
                        }
                    }
                }
            }
        }
        public void UpdateCollider(FixedPointTriangleCollider fixedPointTriangleCollider)
        {
            foreach (var item in fixedPointTriangleCollider.impactNodes)
            {
                item.intersectedTriangleColliders.Remove(fixedPointTriangleCollider);
            }
            fixedPointTriangleCollider.impactNodes.Clear();
            openList.AddRange(root.nodes);
            while (openList.Count > openListIndex)
            {
                var node = openList[openListIndex];
                openListIndex++;
                if (node.nodes == null || node.nodes.Length <= 0)
                {
                    node.intersectedTriangleColliders.Add(fixedPointTriangleCollider);
                    fixedPointTriangleCollider.impactNodes.Add(node);
                    continue;
                }
                for (int i = 0; i < node.nodes.Length; i++)
                {
                    if (FixedPointIntersection.IntersectWithAABBAndAABBFixedPoint(node.nodes[i].min, node.nodes[i].max, fixedPointTriangleCollider.min, fixedPointTriangleCollider.max))
                    {
                        //TODO Intersection AABB triangle.
                        openList.Add(node.nodes[i]);
                    }
                }
            }
        }
        public static FixedPointOctree Initialize(int size)
        {
            var fixedPointOctree = new FixedPointOctree();
            fixedPointOctree.size = size;
            var exp = (int)System.Math.Log(size, 2);
            var root = new FixedPointOctreeNode(exp, (int)System.Math.Pow(2, exp - 1), new FixedPointVector3(0, 0, 0), size);
            var openList = new List<FixedPointOctreeNode>();
            openList.Add(root);
            int count = 0;
            int minHalfSize = 4;
            while (openList.Count > 0)
            {
                count++;
                var node = openList[0];
                openList.RemoveAt(0);
                node.nodes = new FixedPointOctreeNode[8];
                for (int i = 0; i < 8; i++)
                {
                    int x = i % 2 == 0 ? node.halfSize / 2 : -node.halfSize / 2;
                    int y = i / 2 % 2 == 0 ? node.halfSize / 2 : -node.halfSize / 2;
                    int z = i / 4 % 2 == 0 ? node.halfSize / 2 : -node.halfSize / 2;
                    var subNode = new FixedPointOctreeNode(node.exp - 1, node.halfSize / 2, node.pos + new FixedPointVector3(x, y, z), node.size / 2);
                    subNode.parentNode = node;
                    if (node.halfSize / 2 > minHalfSize)
                    {
                        openList.Add(subNode);
                    }
                    else
                    {
                        fixedPointOctree.unitNodes.Add(subNode);
                    }
                    fixedPointOctree.nodes.Add(subNode);
                    /*
                    switch (i)
                    {
                        case 0:
                            subNode.pos = node.pos + new FixedPointVector3(node.halfSize / 2, node.halfSize / 2, node.halfSize / 2);
                            break;
                        case 1:
                            subNode.pos = node.pos + new FixedPointVector3(node.halfSize / 2, -node.halfSize / 2, node.halfSize / 2);
                            break;
                        case 2:
                            subNode.pos = node.pos + new FixedPointVector3(-node.halfSize / 2, -node.halfSize / 2, node.halfSize / 2);
                            break;
                        case 3:
                            subNode.pos = node.pos + new FixedPointVector3(-node.halfSize / 2, node.halfSize / 2, node.halfSize / 2);
                            break;
                        case 4:
                            subNode.pos = node.pos + new FixedPointVector3(node.halfSize / 2, node.halfSize / 2, -node.halfSize / 2);
                            break;
                        case 5:
                            subNode.pos = node.pos + new FixedPointVector3(-node.halfSize / 2, -node.halfSize / 2, -node.halfSize / 2);
                            break;
                        case 6:
                            subNode.pos = node.pos + new FixedPointVector3(-node.halfSize / 2, node.halfSize / 2, -node.halfSize / 2);
                            break;
                        case 7:
                            subNode.pos = node.pos + new FixedPointVector3(node.halfSize / 2, node.halfSize / 2, -node.halfSize / 2);
                            break;
                    }*/
                    node.nodes[i] = subNode;
                }
            }
            /*
            var sideSize = size / minHalfSize * 2;
            List<List<List<FixedPointOctreeNode>>> arrangedNodes = new List<List<List<FixedPointOctreeNode>>>(sideSize);
            for (int i = 0; i < arrangedNodes.Count; i++)
            {
                arrangedNodes[i] = new List<List<FixedPointOctreeNode>>(sideSize);
                for (int j = 0; j < arrangedNodes[i].Count; j++)
                {
                    arrangedNodes[i][j] = new List<FixedPointOctreeNode>(sideSize);
                }
            }
            foreach (var item in fixedPointOctree.unitNodes)
            {
                var x = (item.pos.x + size / 2 - minHalfSize) / minHalfSize * 2;
                var y = (item.pos.y + size / 2 - minHalfSize) / minHalfSize * 2;
                var z = (item.pos.z + size / 2 - minHalfSize) / minHalfSize * 2;
                arrangedNodes[x.AsInt()][y.AsInt()][z.AsInt()] = item;
            }
            foreach (var item in fixedPointOctree.unitNodes)
            {}
            */
            fixedPointOctree.root = root;
            return fixedPointOctree;
        }
    }
}