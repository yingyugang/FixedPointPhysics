namespace BlueNoah.PhysicsEngine
{
    using Math.FixedPoint;
    using System;
    using UnityEngine;

    internal class FPTimer : FastListItem
    {
        FixedPoint64 activeTime;
        FixedPoint64 currentTime;
        FixedPoint64 deltaTime;
        Action<FPTimer> onActive;
        public GameObject gameObject { get; private set; }
        public bool disposed { get; private set; }
        public FPTimer(uint activeTimeInt, FixedPoint64 deltaTime, Action<FPTimer> onActive, GameObject gameObject = null)
        {
            activeTime = activeTimeInt * 0.001;
            this.deltaTime = deltaTime;
            this.onActive = onActive;
            this.gameObject = gameObject;
        }
        public void OnUpdate()
        {
            if (disposed)
            {
                return;
            }
            currentTime += deltaTime;
            if (currentTime >= activeTime)
            {
                disposed = true;
                onActive?.Invoke(this);
            }
        }

        public int index { get; set; }
    }
}