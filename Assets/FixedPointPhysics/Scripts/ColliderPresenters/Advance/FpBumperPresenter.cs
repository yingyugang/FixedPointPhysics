/*
* Create 2023/2/2
* 応彧剛　yingyugang@gmail.com
* It's used by fixedPoint physics system.
*/
using BlueNoah.Math.FixedPoint;
using System.Collections.Generic;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    public abstract class FpBumperPresenter : FPGameObject
    {
        private readonly Dictionary<FPCharacterController, FixedPoint64> cachedAffectedCharacters = new ();
        [SerializeField]
        protected int force = 400;
        [SerializeField]
        protected bool active = true;
        //go with this function to make ensure character only be hit once per time period.
        protected bool VerifyCollisionInterval(FPCharacterController fpCharacterController)
        {
            if (cachedAffectedCharacters.TryGetValue(fpCharacterController, out var character))
            {
                if (character > FPPhysicsPresenter.Instance.TimeSinceStart)
                {
                    return false;
                }
            }
            cachedAffectedCharacters[fpCharacterController] = FPPhysicsPresenter.Instance.TimeSinceStart + 0.2;
            return true;
        }
    }
}