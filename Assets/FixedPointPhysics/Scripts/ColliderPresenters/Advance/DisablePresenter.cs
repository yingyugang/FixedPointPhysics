using BlueNoah.Math.FixedPoint;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    public sealed class DisablePresenter : FPGameObject
    {
        public enum Behavior {Disappear = 0 , Fall = 1,ChangeAppear = 2}
        public Behavior behavior = Behavior.Disappear;
        [Header("Active time after touched.")]
        public uint timer = 2500;
        [Header("Reset time after activated .")]
        public uint cycle = 2500;
        [Header("The object current displays.")]
        public GameObject currentObject;
        [Header("The object display after touched.")]
        public GameObject activeObject;
        [Header("The animator when active,disappear only.")]
        public Animator animator;
        private FPCollider[] colliders;
        private bool activated;
        private Coroutine fallCoroutine;
        private Vector3 startPos;
        private FPTimer activeTimer;

        private void Awake()
        {
            Init();
        }

        protected override void Init()
        {
            if (!Application.isPlaying) return;
            animator = GetComponent<Animator>();
            startPos = transform.position;
            if (currentObject != null)
            {
                currentObject.SetActive(true);
            }
            if (activeObject!=null)
            {
                activeObject.SetActive(false);
            }
            colliders = GetComponentsInChildren<FPCollider>();
            FPPhysicsPresenter.Instance.fixedPointGameObjectFastList.Add(this);
            foreach (var col in colliders)
            {
                //col.InitOrModifyCollider();
                col.onCharacterCollide += OnCollision;
            }
        }

        private void OnCollision(FPCollision fpCollision)
        {
            if (activated) return;
            if (FixedPointVector3.Dot(fpCollision.normal, FixedPointVector3.up) <= 0.95) return;
            if (behavior == Behavior.ChangeAppear)
            {
                if (currentObject != null)
                {
                    currentObject.SetActive(false);
                }
                if (activeObject != null)
                {
                    activeObject.SetActive(true);
                }
            }
            else
            {
                activated = true;
                activeTimer = new FPTimer(timer, FPPhysicsPresenter.Instance.DeltaTime, OnActive);
            }
        }

        public override void OnViewUpdate()
        {
            
        }

        private void OnReset(FPTimer fpTimer)
        {
            if (fallCoroutine != null)
            {
                StopCoroutine(fallCoroutine);
                fallCoroutine = null;
            }
            transform.position = startPos;
            foreach (var col in colliders)
            {
                col.enabled = true;
            }
            var ren = GetComponentInChildren<Renderer>();
            ren.enabled = true;
            activated = false;
            activeTimer = null;
        }

        private void OnActive(FPTimer fpTimer)
        {
            if (behavior == Behavior.Fall)
            {
                if (colliders[0].enabled)
                {
                    fallCoroutine = StartCoroutine(_Fall());
                }
            }
            else if (behavior == Behavior.Disappear)
            {
                if (animator != null)
                {
                    animator.enabled = true;
                }
                else
                {
                    var ren = GetComponentInChildren<Renderer>();
                    ren.enabled = false;
                }
            }
            if (colliders[0].enabled)
            {
                foreach (var col in colliders)
                {
                    col.enabled = false;
                }
            }
            if (cycle > 0)
            {
                activeTimer = new FPTimer(cycle, FPPhysicsPresenter.Instance.DeltaTime, OnReset );
            }
        }

        public override void OnLogicUpdate()
        {
            if (behavior == Behavior.ChangeAppear)
            {
                return;
            }
            if (activeTimer != null)
            {
                activeTimer.OnUpdate();
            }
        }

        private System.Collections.IEnumerator _Fall()
        {
            float speed = 0;
            var trans = transform;
            var t = 0f;
            while (t < 5)
            {
                t += Time.deltaTime;
                speed += 16f * Time.deltaTime;
                trans.position += Vector3.down * speed * Time.deltaTime;
                yield return null;
            }
        }
    }
}