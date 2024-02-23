using UnityEngine;
using System.Collections.Generic;

namespace BlueNoah.PhysicsEngine
{
	public class MobileSimpleRpgCamera : MonoBehaviour
	{
		public Transform target;

		public LayerMask collisionLayers;
		public CameraCollisionStyle collisionStyle = CameraCollisionStyle.RPG;
		public float collisionAlpha = 0.15f;
		public float collisionFadeSpeed = 10;

		public bool allowRotation = true;
		public bool invertX;
		public bool invertY;
		public bool stayBehindTarget;

		public Vector2 targetOffset ;

		public bool rotateObjects = true;
		public List<Transform> objectsToRotate;

		public bool fadeObjects = true;
		public float fadeDistance = 1.5f;
		public List<Renderer> objectsToFade;

		public Vector2 originRotation;
		public bool returnToOrigin = true;
		public float returnSmoothing = 3;

		public float distance = 5;
		public float minDistance;
		public float maxDistance = 10;

		public Vector2 sensitivity = new Vector2(3, 3);

		public float zoomSpeed = 1;
		public float zoomSmoothing = 16;

		public float minAngle = -90;
		public float maxAngle = 90;

		private readonly List<Material> _faded_mats = new ();
		private List<Material> _current_faded_mats = new ();

		private float _previous_distance;
		private float _wanted_distance;
		private Quaternion _rotation;
		private Vector2 _input_rotation;

		private Transform _t;

		public bool Controllable { get; set; } = true;

		private void Start()
		{
			_t = transform;
			_wanted_distance = distance;
			_input_rotation = originRotation;

			// If there isn't a target set, warn the user
			if (!target)
			{
				Debug.LogWarning("MobileSimpleRpgCamera.cs: No initial target set");
			}
		}

		private void Update()
		{
			if (target)
			{
				// Fade the target according to Fade Distance (if enabled)
				foreach (Renderer r in objectsToFade)
				{
					if (r)
					{
						foreach (Material m in r.materials)
						{
							Color c = m.color;
							c.a = Mathf.Clamp(distance - fadeDistance, 0, 1);

							if (!fadeObjects)
							{
								c.a = 1;
							}

							m.color = c;
						}
					}
				}
			}

			// Fade back in the faded out objects that were in front of topdown camera
			if (collisionStyle != CameraCollisionStyle.TopDown) return;
			{
				foreach (var mat in _faded_mats)
				{
					var skip = false;

					foreach (var c_mat in _current_faded_mats)
					{
						if (mat == c_mat)
						{
							skip = true;
							break;
						}
					}

					if (!skip)
					{
						if (System.Math.Abs(mat.color.a - 1) < 0.000001f)
						{
							_faded_mats.Remove(mat);
						}
						else
						{
							Color c = mat.color;
							c.a = 1;
							mat.color = Color.Lerp(mat.color, c, Time.deltaTime * collisionFadeSpeed);
						}
					}
				}
			}
		}

		// Camera movement should be done in LateUpdate(),
		// but it is slightly choppy for some reason so changed to FixedUpdate()
		// seems to be working fine
		void LateUpdate()
		{
			if (target)
			{
				if (Controllable)
				{
					// Zoom control
					if (Input.touchCount == 2 &&
					    (Input.GetTouch(0).phase == TouchPhase.Began ||
					     Input.GetTouch(1).phase == TouchPhase.Began))
					{
						_previous_distance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
					}
					else if (Input.touchCount == 2 &&
					         (Input.GetTouch(0).phase == TouchPhase.Moved ||
					          Input.GetTouch(1).phase == TouchPhase.Moved))
					{
						var touch1 = Input.GetTouch(0).position;
						var touch2 = Input.GetTouch(1).position;

						var d = Vector2.Distance(touch1, touch2);

						_wanted_distance -= (_previous_distance - d) * zoomSpeed;
						_previous_distance = d;
					}
				}

				// Prevent wanted distance from going below or above min and max distance
				_wanted_distance = Mathf.Clamp(_wanted_distance, minDistance, maxDistance);

				// If user drags, change position based on drag direction and sensitivity
				// Stop at 90 degrees above / below object
				if (allowRotation && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
				{
					if (Controllable)
					{
						if (invertX)
						{
							_input_rotation.x -= Input.GetTouch(0).deltaPosition.x * sensitivity.x;
						}
						else
						{
							_input_rotation.x += Input.GetTouch(0).deltaPosition.x * sensitivity.x;
						}

						ClampRotation();

						if (invertY)
						{
							_input_rotation.y += Input.GetTouch(0).deltaPosition.y * sensitivity.y;
						}
						else
						{
							_input_rotation.y -= Input.GetTouch(0).deltaPosition.y * sensitivity.y;
						}

						_input_rotation.y = Mathf.Clamp(_input_rotation.y, minAngle, maxAngle);

						_rotation = Quaternion.Euler(_input_rotation.y, _input_rotation.x, 0);

						if (Input.touchCount == 1)
						{
							// Force the target's y rotation to face forward (if enabled) when dragging with one finger
							if (rotateObjects)
							{
								foreach (Transform o in objectsToRotate)
								{
									o.rotation = Quaternion.Euler(0, _input_rotation.x, 0);
								}
							}

							originRotation = _input_rotation;
							ClampRotation();
						}
					}
				}
				else if (Input.touchCount == 0 || !Controllable)
				{
					// Keeps the camera behind the target when not controlling it (if enabled)
					if (stayBehindTarget)
					{
						originRotation.x = target.eulerAngles.y;
						ClampRotation();
					}

					// If Return To Origin, move camera back to the default position
					if (returnToOrigin)
					{
						_input_rotation = Vector3.Lerp(_input_rotation, originRotation,
							returnSmoothing * Time.deltaTime);
					}

					_rotation = Quaternion.Euler(_input_rotation.y, _input_rotation.x, 0);
				}

				// Lerp from current distance to wanted distance
				distance = Mathf.Clamp(Mathf.Lerp(distance, _wanted_distance, Time.deltaTime * zoomSmoothing),
					minDistance, maxDistance);

				if (collisionStyle == CameraCollisionStyle.TopDown)
				{
					// fade out any objects in front of the top down camera
					var position = target.position;
					var ray = new Ray(position, _t.position - position);
					var hits = Physics.RaycastAll(ray, maxDistance, collisionLayers);

					_current_faded_mats = new List<Material>();

					foreach (RaycastHit hit in hits)
					{
						if (hit.transform.gameObject.GetComponent<Renderer>())
						{
							var mats = hit.transform.gameObject.GetComponent<Renderer>().materials;

							foreach (var mat in mats)
							{
								var c = mat.color;
								c.a = collisionAlpha;

								_current_faded_mats.Add(mat);
								mat.color = Color.Lerp(mat.color, c, Time.deltaTime * collisionFadeSpeed);

								var add = true;

								foreach (var f_mat in _faded_mats)
								{
									if (f_mat != mat) continue;
									add = false;
									break;
								}

								if (add)
								{
									_faded_mats.Add(mat);
								}
							}
						}
					}
				}

				// Set the position and rotation of the camera
				_t.position = _rotation * new Vector3(targetOffset.x, 0.0f, -distance) + target.position +
				              new Vector3(0, targetOffset.y, 0);
				_t.rotation = _rotation;
			}
		}

		private void ClampRotation()
		{
			if (originRotation.x < -180)
			{
				originRotation.x += 360;
			}
			else if (originRotation.x > 180)
			{
				originRotation.x -= 360;
			}

			if (_input_rotation.x - originRotation.x < -180)
			{
				_input_rotation.x += 360;
			}
			else if (_input_rotation.x - originRotation.x > 180)
			{
				_input_rotation.x -= 360;
			}
		}

		public void SetCameraRotate(Vector2 angle)
		{
			_input_rotation = angle;
			originRotation = _input_rotation;
			ClampRotation();
		}

		//TODO:追加で作成した関数なので後で拡張クラスにして対応したほうがいいと思う
		// もしくは別のクラスにするかちゃんとこれに依存しないカメラの回転の機能を作ったほうがいいかもしれない。
		public void CameraRotate(Vector2 delta)
		{
			if (Controllable)
			{
				if (invertX)
				{
					_input_rotation.x -= delta.x * sensitivity.x;
				}
				else
				{
					_input_rotation.x += delta.x * sensitivity.x;
				}

				ClampRotation();

				if (invertY)
				{
					_input_rotation.y += delta.y * sensitivity.y;
				}
				else
				{
					_input_rotation.y -= delta.y * sensitivity.y;
				}

				_input_rotation.y = Mathf.Clamp(_input_rotation.y, minAngle, maxAngle);

				_rotation = Quaternion.Euler(_input_rotation.y, _input_rotation.x, 0);

				// Force the target's y rotation to face forward (if enabled) when dragging with one finger
				if (rotateObjects)
				{
					foreach (Transform o in objectsToRotate)
					{
						o.rotation = Quaternion.Euler(0, _input_rotation.x, 0);
					}
				}

				originRotation = _input_rotation;
				ClampRotation();
			}
		}
	}
}