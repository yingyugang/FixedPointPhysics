using UnityEngine;
using System.Collections.Generic;

namespace BlueNoah.PhysicsEngine
{
	public enum CameraCollisionStyle
	{
		RPG,
		TopDown
	}

	public class SimpleRpgCamera : MonoBehaviour
	{
		public Transform target;

		public LayerMask collisionLayers;
		public CameraCollisionStyle collisionStyle = CameraCollisionStyle.RPG;
		public float collisionAlpha = 0.15f;
		public float collisionFadeSpeed = 10;

		public bool allowRotation = true;
		public bool lockCursor = true;
		public bool invertX;
		public bool invertY;
		public bool stayBehindTarget;

		public Vector2 targetOffset;

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

		public Vector2 sensitivity = new (1, 1);

		public float zoomSpeed = 1;
		public float zoomSmoothing = 16;
		public KeyCode zoomInAlt = KeyCode.Home;
		public KeyCode zoomOutAlt = KeyCode.End;
		public float zoomAltDelay = 0.5f;

		public float minAngle = -90;
		public float maxAngle = 90;

		private readonly List<Material> _faded_mats = new ();
		private List<Material> _current_faded_mats = new ();

		private float _zoom_in_timer;
		private float _zoom_out_timer;

		private float _wanted_distance;
		private Quaternion _rotation;
		private Vector2 _input_rotation;

		private Transform _t;

		public bool Controllable { get; set; } = true;

		public void SetCameraRotate(Vector2 angle)
		{
			_input_rotation = angle;
			originRotation = _input_rotation;
			ClampRotation();
		}

		private void Start()
		{
			_t = transform;
			_wanted_distance = distance;
			_input_rotation = originRotation;

			// If no target set, warn the user
			if (!target)
			{
				Debug.LogWarning("SimpleRpgCamera.cs: No initial target set");
			}
		}

		private void Update()
		{
			if (target)
			{
				// Fade the target according to Fade Distance (if enabled)
				foreach (var r in objectsToFade)
				{
					if (!r) continue;
					foreach (var m in r.materials)
					{
						var c = m.color;
						c.a = Mathf.Clamp(distance - fadeDistance, 0, 1);

						if (!fadeObjects)
						{
							c.a = 1;
						}

						m.color = c;
					}
				}

				if (Controllable)
				{
					if (Input.GetKey(zoomInAlt))
					{
						_zoom_in_timer += Time.deltaTime;
					}
					else
					{
						_zoom_in_timer = 0;
					}

					if (Input.GetKey(zoomOutAlt))
					{
						_zoom_out_timer += Time.deltaTime;
					}
					else
					{
						_zoom_out_timer = 0;
					}

					// Lock the cursor (if enabled) when user click-drags mouse
					if (lockCursor)
					{
						if (Input.GetMouseButtonDown(1))
						{
							Cursor.lockState = CursorLockMode.Locked;
						}

						if (Input.GetMouseButtonUp(1))
						{
							Cursor.lockState = CursorLockMode.None;
						}

						if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
						{
							if (Input.GetAxis("Mouse X") == 0 && Input.GetAxis("Mouse Y") == 0) return;
							if (Cursor.visible)
							{
								//Cursor.visible = false;
							}

							return;
						}
					}
				}
			}

			if (!Cursor.visible)
			{
				Cursor.visible = true;
			}

			// Fade back in the faded out objects that were in front of topdown camera
			if (collisionStyle == CameraCollisionStyle.TopDown)
			{
				foreach (var mat in _faded_mats)
				{
					var skip = false;

					foreach (var c_mat in _current_faded_mats)
					{
						if (mat != c_mat) continue;
						skip = true;
						break;
					}

					if (skip) continue;
					if (System.Math.Abs(mat.color.a - 1) <= 0)
					{
						_faded_mats.Remove(mat);
					}
					else
					{
						var c = mat.color;
						c.a = 1;
						mat.color = Color.Lerp(mat.color, c, Time.deltaTime * collisionFadeSpeed);
					}
				}
			}
		}

		private bool dragStartedONLeft;

		// Camera movement should be done in LateUpdate(),
		// but it is slightly choppy for some reason so changed to FixedUpdate()
		// seems to be working fine
		private void LateUpdate()
		{
			if (!target) return;
			if (Controllable)
			{
				// Zoom control
				if (Input.GetAxis("Mouse ScrollWheel") < 0 || Input.GetKeyDown(zoomOutAlt) ||
				    _zoom_out_timer > zoomAltDelay)
				{
					_wanted_distance += zoomSpeed;
				}
				else if (Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetKeyDown(zoomInAlt) ||
				         _zoom_in_timer > zoomAltDelay)
				{
					_wanted_distance -= zoomSpeed;
				}
			}

			// Prevent wanted distance from going below or above min and max distance
			_wanted_distance = Mathf.Clamp(_wanted_distance, minDistance, maxDistance);



			if (Input.GetMouseButtonDown(0)) // Check for left mouse button click
			{
				var mousePosition = Input.mousePosition;

				// Check if the mouse click started in the left half of the screen
				dragStartedONLeft = mousePosition.x <= Screen.width / 2;
			}
			else if (Input.GetMouseButtonUp(0))
			{
				dragStartedONLeft = false;
			}

			if (allowRotation && (!dragStartedONLeft && (Input.GetMouseButton(0)) || Input.GetMouseButton(1)))
			{
				if (Controllable)
				{

					if (invertX)
					{
						_input_rotation.x -= Input.GetAxis("Mouse X") * sensitivity.x;
					}
					else
					{
						_input_rotation.x += Input.GetAxis("Mouse X") * sensitivity.x;
					}

					ClampRotation();

					if (invertY)
					{
						_input_rotation.y += Input.GetAxis("Mouse Y") * sensitivity.y;
					}
					else
					{
						_input_rotation.y -= Input.GetAxis("Mouse Y") * sensitivity.y;
					}

					_input_rotation.y = Mathf.Clamp(_input_rotation.y, minAngle, maxAngle);

					_rotation = Quaternion.Euler(_input_rotation.y, _input_rotation.x, 0);

					// Force the target's y rotation to face forward (if enabled) when right clicking
					if (rotateObjects)
					{
						if (Input.GetMouseButton(1))
						{
							foreach (var o in objectsToRotate)
							{
								o.rotation = Quaternion.Euler(0, _input_rotation.x, 0);
							}
						}
					}

					// If user is right clicking, set the default position to the current position
					if (Input.GetMouseButton(1))
					{
						originRotation = _input_rotation;
						ClampRotation();
					}
				}
			}
			else
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

			// Set wanted position based off rotation and distance
			var position1 = target.position;
			Vector3 wanted_position = _rotation * new Vector3(targetOffset.x, 0, -_wanted_distance - 0.2f) +
			                          position1 + new Vector3(0, targetOffset.y, 0);
			Vector3 current_position = _rotation * new Vector3(targetOffset.x, 0, 0) + position1 +
			                           new Vector3(0, targetOffset.y, 0);

			if (collisionStyle == CameraCollisionStyle.RPG)
			{
				// Linecast to test if there are objects between the camera and the target using collision layers

				if (Physics.Linecast(current_position, wanted_position, out var hit, collisionLayers))
				{
					distance = Vector3.Distance(current_position, hit.point) - 0.2f;
				}
			}
			else if (collisionStyle == CameraCollisionStyle.TopDown)
			{
				// fade out any objects in front of the top down camera
				var position = target.position;
				var ray = new Ray(position, _t.position - position);
				var hits = new RaycastHit[100];
				var size = Physics.RaycastNonAlloc(ray, hits, maxDistance, collisionLayers);

				_current_faded_mats = new List<Material>();

				for (var i = 0; i < size; i++)
				{
					var hit = hits[i];
					if (!hit.transform.gameObject.GetComponent<Renderer>()) continue;
					var mats = hit.transform.gameObject.GetComponent<Renderer>().materials;

					foreach (var mat in mats)
					{
						Color c = mat.color;
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

			// Set the position and rotation of the camera
			_t.position = _rotation * new Vector3(targetOffset.x, 0.0f, -distance) + target.position +
			              new Vector3(0, targetOffset.y, 0);
			_t.rotation = _rotation;
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

		//TODO:追加で作成した関数なので後で拡張クラスにして対応したほうがいいと思う
		// もしくは別のクラスにするかちゃんとこれに依存しないカメラの回転の機能を作ったほうがいいかもしれない。
		// これはMobileSimpleRpgCameraにも書いてある話……
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