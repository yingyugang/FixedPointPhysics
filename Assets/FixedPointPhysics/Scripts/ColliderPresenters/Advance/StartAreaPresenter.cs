using BlueNoah.Math.FixedPoint;
using System.Collections.Generic;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    public sealed class StartAreaPresenter : FPGameObject
    {
        public int count = 32;
        private const int MaxPerRow = 8;
        private const float RowInterval = 1.6f;
        private const float ColumnInterval = 2.2f;

        public List<FixedPointVector3> GetPositions(int count)
        {
            var positions = new List<FixedPointVector3>();
            var row = FixedPointMath.Ceiling(count / new FixedPoint64(MaxPerRow)).AsInt();
            for (var i = 0; i < count; i++)
            {
                positions.Add(fpTransform.rotation *
                              new FixedPointVector3(((i % MaxPerRow - MaxPerRow * 0.5f) + 0.5f) * ColumnInterval,
                                  0,
                                  ((i / MaxPerRow - row * 0.5f) + 0.5f) * RowInterval) +
                              fpTransform.position);
            }
            return positions;
        }

        public override void OnLogicUpdate()
        {
        }

        public override void OnViewUpdate()
        {
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                var positions = GetPositions(count);
                foreach (var item in positions)
                {
                    Gizmos.DrawWireSphere(item.ToVector3(),0.5f);
                }
            }
            else
            {
                var row = Mathf.CeilToInt(count / (float)MaxPerRow);
                var trans = transform;
                for (var i = 0; i < count; i++)
                {
                    Gizmos.DrawWireSphere(transform.rotation * new Vector3(((i % MaxPerRow - MaxPerRow / 2f) + 0.5f) * ColumnInterval, 0, ((i / MaxPerRow - row / 2f) + 0.5f) * RowInterval) + trans.position, 0.5f);
                }
            }
        }

    }
}