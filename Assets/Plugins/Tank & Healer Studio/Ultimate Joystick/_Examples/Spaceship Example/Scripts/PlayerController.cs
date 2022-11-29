/* PlayerController.cs */
/* Written by Kaz Crowe */
using UnityEngine;

namespace UltimateJoystickExample.Spaceship
{
	public class PlayerController : MonoBehaviour
	{
		// Speeds //
		[Header( "Speeds" )]
		public float rotationSpeed = 45.0f;
		public float accelerationSpeed = 2.0f;
		public float maxSpeed = 3.0f;
		public float shootingCooldown = 0.2f;

		// Prefabs //
		[Header( "Assigned Variables" )]
		public GameObject bulletPrefab;

		// Player Components //
		Rigidbody myRigidbody;
		public Transform gunTrans;
		public Transform bulletSpawnPos;
		public GameObject playerVisuals;

		// Timers //
		float shootingTimer = 0;

		// Controls //
		bool canControl = true;

		// Input Positions //
		Vector3 movePosition;
		Vector3 shootPosition;

		
		void Start ()
		{
			// Store the player's rigidbody.
			myRigidbody = GetComponent<Rigidbody>();
		}

		void Update ()
		{
			// Store the input positions
			movePosition = new Vector3( UltimateJoystick.GetHorizontalAxis( "Movement" ), UltimateJoystick.GetVerticalAxis( "Movement" ), 0 );
			shootPosition = new Vector3( UltimateJoystick.GetHorizontalAxis( "Shooting" ), UltimateJoystick.GetVerticalAxis( "Shooting" ), 0 );

			// If the user cannot control the player, then return.
			if( canControl == false )
				return;

			// If the shooting joystick is being used and the shooting timer is ready...
			if( UltimateJoystick.GetJoystickState( "Shooting" ) && shootingTimer <= 0 )
			{
				// Then reset the timer and shoot a bullet.
				shootingTimer = shootingCooldown;
				CreateBullets();
			}

			// If the shoot timer is above zero, reduce it.
			if( shootingTimer > 0 )
				shootingTimer -= Time.deltaTime;
		}

		void FixedUpdate ()
		{
			// If the user cannot control the player...
			if( canControl == false )
			{
				// Then reset the player's rotation, position, velocity and angular vel.
				myRigidbody.rotation = Quaternion.identity;
				myRigidbody.position = Vector3.zero;
				myRigidbody.velocity = Vector3.zero;
				myRigidbody.angularVelocity = Vector3.zero;
			}
			else
			{
				// Figure out the rotation that the player should be facing and apply it.
				Vector3 lookRot = new Vector3( movePosition.x, 0, movePosition.y );
				transform.LookAt( transform.position + lookRot );

				// Also figure out the rotation of the player's gun and apply it.
				Vector3 gunRot = new Vector3( shootPosition.x, 0, shootPosition.y );
				gunTrans.LookAt( gunTrans.position + gunRot );

				// Apply the input force to the player.
				myRigidbody.AddForce( transform.forward * UltimateJoystick.GetDistance( "Movement" ) * 1000.0f * accelerationSpeed * Time.deltaTime );

				// If the player's force is greater than the max speed, then normalize it.
				if( myRigidbody.velocity.magnitude > maxSpeed )
					myRigidbody.velocity = myRigidbody.velocity.normalized * maxSpeed;

				// Run the CheckExitScreen function to see if the player has left the screen.
				CheckExitScreen();
			}
		}

		void CheckExitScreen ()
		{
			// If the main camera is not assigned, then return.
			if( Camera.main == null )
				return;
			
			// If the absolute value of the player's X position is greater than the ortho size of the camera multiplied by the camera's aspect ratio, then reset the player on the other side.
			if( Mathf.Abs( myRigidbody.position.x ) > Camera.main.orthographicSize * Camera.main.aspect )
				myRigidbody.position = new Vector3( -Mathf.Sign( myRigidbody.position.x ) * Camera.main.orthographicSize * Camera.main.aspect, 0, myRigidbody.position.z );
			
			// If the absolute value of the player's Z position is greater than the ortho size, then reset the Z position to the other side.
			if( Mathf.Abs( myRigidbody.position.z ) > Camera.main.orthographicSize )
				myRigidbody.position = new Vector3( myRigidbody.position.x, myRigidbody.position.y, -Mathf.Sign( myRigidbody.position.z ) * Camera.main.orthographicSize );
		}

		void CreateBullets ()
		{
			// Create a new bulletPrefab game object at the barrel's position and rotation.
			GameObject bullet = Instantiate( bulletPrefab, bulletSpawnPos.position, bulletSpawnPos.rotation ) as GameObject;

			// Rename the bullet for reference within the asteroid script.
			bullet.name = bulletPrefab.name;
			
			// Apply a speed to the bullet's velocity.
			bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 200.0f;

			// Destroy the bullet after 3 seconds.
			Destroy( bullet, 3.0f );
		}
	}
}