using BlueNoah.Math.FixedPoint;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlueNoah.PhysicsEngine
{
    public class PhysicsOverlapExample : MonoBehaviour
    {
        public Slider sliderRadius;
        public TextMeshProUGUI txtRadius;
        public Slider sliderX;
        public TextMeshProUGUI txtX;
        public Slider sliderY;
        public TextMeshProUGUI txtY;
        public Slider sliderZ;
        public TextMeshProUGUI txtZ;
        public TextMeshProUGUI txtHit;
        public GameObject hitGo;

        public GameObject sphere;

        FixedPoint64 radius = 1;
        FixedPointVector3 position = new FixedPointVector3(0, 3, 0);

        private void Awake()
        {
            sliderRadius.minValue = 1;
            sliderRadius.maxValue = 10;
            sliderRadius.value = radius.AsFloat();
            txtRadius.text = $"Radius:{radius.AsFloat()}";
            sliderRadius.onValueChanged.AddListener((val) => {
                radius = val;
                txtRadius.text = $"Radius:{radius.AsFloat()}";
            });

            sliderX.minValue = -10;
            sliderX.maxValue = 10;
            sliderX.value = 0;
            txtX.text = $"X:{position.x.AsFloat()}";
            sliderX.onValueChanged.AddListener((val) => {
                position.x = val;
                txtX.text = $"X:{position.x.AsFloat()}";
            });

            sliderY.minValue = -10;
            sliderY.maxValue = 10;
            sliderY.value = 0;
            txtY.text = $"Y:{position.y.AsFloat()}";
            sliderY.onValueChanged.AddListener((val) => {
                position.y = val;
                txtY.text = $"Y:{position.y.AsFloat()}";
            });

            sliderZ.minValue = -10;
            sliderZ.maxValue = 10;
            sliderZ.value = 0;
            txtZ.text = $"Z:{position.z.AsFloat()}";
            sliderZ.onValueChanged.AddListener((val) => {
                position.z = val;
                txtZ.text = $"Z:{position.z.AsFloat()}";
            });
        }
        // Update is called once per frame
        void Update()
        {
            position = new FixedPointVector3(sphere.transform.position);
            var colliders = FixedPointPhysicsPresenter.OverlapSphere(position, sphere.transform.localScale.x / 2);
            hitGo.SetActive(false);
            foreach (var item in colliders)
            {
                switch (item.colliderType)
                {
                    case ColliderType.AABB:
                        { 
                            var hit = FixedPointIntersection.IntersectWithSphereAndAABB(position, radius, item.min, item.max);
                            if (hit.hit)
                            {
                                hitGo.SetActive(true);
                                hitGo.transform.position = hit.closestPoint.ToVector3();
                                hitGo.transform.forward = hit.normal.ToVector3();
                            }
                        }
                        break;
                    case ColliderType.Sphere:
                        {
                            var sphere = (FixedPointSphereCollider)item;
                            var hit = FixedPointIntersection.IntersectWithSphereAndSphere(position, radius, item.position, sphere.radius);
                            if (hit.hit)
                            {
                                hitGo.SetActive(true);
                                hitGo.transform.position = hit.closestPoint.ToVector3();
                                hitGo.transform.forward = hit.normal.ToVector3();
                            }
                        }
                        break;
                    case ColliderType.OBB:
                        {
                            var obb = (FixedPointOBBCollider)item;
                            var hit = FixedPointIntersection.IntersectWithSphereAndOBB(position, radius, item.position, obb.halfSize,obb.fixedPointTransform.fixedPointMatrix);
                            if (hit.hit)
                            {
                                hitGo.SetActive(true);
                                hitGo.transform.position = hit.closestPoint.ToVector3();
                                hitGo.transform.forward = hit.normal.ToVector3();
                            }
                        }
                        break;
                    case ColliderType.Triangle:
                        {
                            var obb = (FixedPointTriangleCollider)item;
                            var hit = FixedPointIntersection.IntersectWithSphereAndTriangle(position, radius, obb);
                            if (hit.hit)
                            {
                                hitGo.SetActive(true);
                                hitGo.transform.position = hit.closestPoint.ToVector3();
                                hitGo.transform.forward = hit.normal.ToVector3();
                            }
                        }
                        break;
                }
               
            }
        }
    }
}