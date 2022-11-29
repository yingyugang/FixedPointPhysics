/* AsteroidController.cs */
/* Written by Kaz Crowe */
using UnityEngine;
using System.Collections;

namespace UltimateJoystickExample.Spaceship
{
	public class AsteroidController : MonoBehaviour
	{
		// Reference Variables //
		public GameManager asteroidManager;
		Rigidbody myRigidbody;

		// Controller Booleans //
		bool canDestroy = false;
		bool isDestroyed = false;
		public bool isDebris = false;
	

		public void Setup ( Vector3 force, Vector3 torque )
		{
			// Assign the rigidbody component attached to this game object.
			myRigidbody = GetComponent<Rigidbody>();

			// Add the force and torque to the rigidbody.
			myRigidbody.AddForce( force );
			myRigidbody.AddTorque( torque );

			// Delay the time that this asteroid can be destroyed for being out of the screen.
			StartCoroutine( DelayInitialDestruction( isDebris == true  ? 0.25f : 1.0f ) );
		}

		IEnumerator DelayInitialDestruction ( float delayTime )
		{
			// Wait for the designated time.
			yield return new WaitForSeconds( delayTime );

			// Allow this asteroid to be destoryed.
			canDestroy = true;
		}
	
		void Update ()
		{
			// If the asteroid is out of the screen...
			if( Mathf.Abs( transform.position.x ) > Camera.main.orthographicSize * Camera.main.aspect * 1.3f || Mathf.Abs( transform.position.z ) > Camera.main.orthographicSize * 1.3f )
			{
				// If this asteroid can be destoryed, then commence destruction!
				if( canDestroy == true )
					Destroy( gameObject );
			}
		}

		void OnCollisionEnter ( Collision theCollision )
		{
			// If the collision was from a bullet...
			if( theCollision.gameObject.name == "Bullet" )
			{
				// Destroy the bullet.
				Destroy( theCollision.gameObject );

				// Modify the score.
				asteroidManager.ModifyScore( isDebris );

				// If this object is not debris, then explode.
				if( isDebris == false )
					Explode();
				// Else just destory the debris.
				else
					Destroy( gameObject );
			}
			// Else if the collision was from the player...
			else if( theCollision.gameObject.name == "Player" )
			{
				// Spawn an explosion where the player is at.
				asteroidManager.SpawnExplosion( theCollision.transform.position );

				// Destroy the player.
				Destroy( theCollision.gameObject );

				// If this object is not debris, then explode.
				if( isDebris == false )
					Explode();
				// Else just destory the debris.
				else
					Destroy( gameObject );

				// Show the user the death screen.
				asteroidManager.ShowDeathScreen();
			}
			// Else the collision is another asteroid/debris...
			else
			{
				// If this object is not debris and it can be destroyed, then explode.
				if( isDebris == false && canDestroy == true )
					Explode();
				// Else if this object is debris and can explode, then just destroy the game object.
				else if( isDebris == true && canDestroy == true )
					Destroy( gameObject );
			}

			// Spawn an explosion particle.
			asteroidManager.SpawnExplosion( transform.position );
		}

		void Explode ()
		{
			// If this asteroid has already been destroyed, then return.
			if( isDestroyed == true )
				return;

			// Let the script know that this asteroid has already been destroyed.
			isDestroyed = true;

			// Spawn some debris from this asteroids position.
			asteroidManager.SpawnDebris( transform.position );

			// Destory this asteroid.
			Destroy( gameObject );
		}
	}
}