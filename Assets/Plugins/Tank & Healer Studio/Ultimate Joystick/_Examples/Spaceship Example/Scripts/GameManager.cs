/* GameManager.cs */
/* Written by Kaz Crowe */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UltimateJoystickExample.Spaceship
{
	public class GameManager : MonoBehaviour
	{
		// Static Refs //
		private static GameManager instance;
		public static GameManager Instance
		{
			get { return instance; }
		}

		// Prefabs //
		[Header( "Prefabs" )]
		public GameObject astroidPrefab;
		public GameObject debrisPrefab;
		public GameObject explosionPrefab;
		public GameObject playerExplosionPrefab;

		// Spawning Info //
		bool spawning = true;
		[Header( "Spawning" )]
		public float spawnTimeMin = 5.0f;
		public float spawnTimeMax = 10.0f;
		public int startingAsteroids = 2;

		// Score //
		[Header( "Score" )]
		public Text scoreText;
		int score = 0;
		public int asteroidPoints = 50;
		public int debrisPoints = 10;

		// Game Over //
		[Header( "Game Over" )]
		public Image gameOverScreen;
		public Text gameOverText;

	
		void Awake ()
		{
			// If the instance variable is already assigned...
			if( instance != null )
			{
				// If the instance is currently active...
				if( instance.gameObject.activeInHierarchy == true )
				{
					// Warn the user that there are multiple Game Managers within the scene and destroy the old manager.
					Debug.LogWarning( "There are multiple instances of the Game Manager script. Removing the old manager from the scene." );
					Destroy( instance.gameObject );
				}
				
				// Remove the old manager.
				instance = null;
			}

			// Assign the instance variable as the Game Manager script on this object.
			instance = GetComponent<GameManager>();
		}

		void Start ()
		{
			// Start spawning the asteroids.
			StartCoroutine( "SpawnTimer" );

			// Update the score text to reflect the current score on start.
			UpdateScoreText();
		}
	
		IEnumerator SpawnTimer ()
		{
			// Wait for a bit before the initial spawn.
			yield return new WaitForSeconds( 0.5f );

			// For as many times as the startingAsteroids variable dictates, spawn an asteroid.
			for( int i = 0; i < startingAsteroids; i++ )
				SpawnAsteroid();

			// While spawning is true...
			while( spawning )
			{
				// Wait for a range of seconds determined my the min and max variables.
				yield return new WaitForSeconds( Random.Range( spawnTimeMin, spawnTimeMax ) );

				// Spawn an asteroid.
				SpawnAsteroid();
			}
		}

		void SpawnAsteroid ()
		{
			// Get a random point within a circle area.
			Vector2 dir = Random.insideUnitCircle;

			// Create a Vector3 varaible to store the spawn position.
			Vector3 pos = Vector3.zero;

			// If the X value of the spawn direction is greater than the Y, then spawn the asteroid to the left or right of the screen, determined by the value of dir.
			if( Mathf.Abs( dir.x ) > Mathf.Abs( dir.y ) )
				pos = new Vector3( Mathf.Sign( dir.x ) * Camera.main.orthographicSize * Camera.main.aspect * 1.3f, 0, dir.y * Camera.main.orthographicSize );
			// Else the Y value is greater than X, so spawn the asteroid up or down, determined by the value of dir.
			else
				pos = new Vector3( dir.x * Camera.main.orthographicSize * Camera.main.aspect * 1.3f, 0, Mathf.Sign( dir.y ) * Camera.main.orthographicSize );

			// Create the asteroid game object at the position( determined above ), and at a random rotation.
			GameObject ast = Instantiate( astroidPrefab, pos, Quaternion.Euler( Random.value * 360.0f, Random.value * 360.0f, Random.value * 360.0f ) ) as GameObject;

			// Call the setup function on the asteroid with the desired force and torque.
			ast.GetComponent<AsteroidController>().Setup( -pos.normalized * 1000.0f, Random.insideUnitSphere * Random.Range( 500.0f, 1500.0f ) );

			// Assign the manager component to this instance of the Game Manager.
			ast.GetComponent<AsteroidController>().asteroidManager = instance;
		}

		public void SpawnDebris( Vector3 pos )
		{
			// Determine how many debris should be created.
			int numberToSpawn = Random.Range( 3, 6 );

			// For each number to spawn...
			for( int i = 0; i < numberToSpawn; i++ )
			{
				// Create a force to make the debris fly away from eachother.
				Vector3 force = Quaternion.Euler( 0, i * 360f / numberToSpawn, 0 ) * Vector3.forward * 5.0f * 300f;

				// Create the new debris game object at the asteroid's position, plus the forces position to make sure that the debris is positioned correctly. Random rotation as well.
				GameObject newGO = Instantiate( debrisPrefab, pos + force.normalized * Random.Range( 0.0f, 5.0f ), Quaternion.Euler( 0, Random.value * 180f, 0 ) ) as GameObject;

				// Apply a random scale factor to make all the debris different.
				newGO.transform.localScale = new Vector3( Random.Range( 0.25f, 0.5f ), Random.Range( 0.25f, 0.5f ), Random.Range( 0.25f, 0.5f ) );

				// Setup the needed force and torque.
				newGO.GetComponent<AsteroidController>().Setup( force / 2, Random.insideUnitSphere * Random.Range( 500f, 1500f ) );

				// Assign the Game Manager component to this instance.
				newGO.GetComponent<AsteroidController>().asteroidManager = instance;
			}
		}

		public void SpawnExplosion ( Vector3 pos )
		{
			// Create a new explosion prefab game object at the desired position, with default rotation.
			GameObject newParticles = Instantiate( explosionPrefab, pos, Quaternion.identity ) as GameObject;

			// Destory the particles after one second.
			Destroy( newParticles, 1 );
		}

		public void ShowDeathScreen ()
		{
			// Enable the game over screen game object.
			gameOverScreen.gameObject.SetActive( true );

			GameObject expl = ( GameObject )Instantiate( playerExplosionPrefab, FindObjectOfType<PlayerController>().transform.position, Quaternion.identity );

			Destroy( expl, 2 );

			// Start the ShakeCamera coroutine for a dynamic effect.
			StartCoroutine( "ShakeCamera" );

			// Start the Fade coroutine so that the death screen will fade in.
			StartCoroutine( "FadeDeathScreen" );

			// Set spawning to false so that no more asteroids get spawned.
			spawning = false;

			UltimateJoystick.GetUltimateJoystick( "Movement" ).UpdatePositioning();
		}

		IEnumerator FadeDeathScreen ()
		{
			// Wait for half a second for a little bit more dynamic effect.
			yield return new WaitForSeconds( 0.5f );

			// Set the text to the final score text plus the user's score.
			scoreText.text = "Final Score\n" + score.ToString();

			// Create temporary colors to be able to apply a fade to the image and text.
			Color imageColor = gameOverScreen.color;
			Color textColor = gameOverText.color;

			for( float t = 0.0f; t < 1.0f; t += Time.deltaTime * 3f )
			{
				// Lerp the alpha of the temp colors from 0 to 0.75 by the amount of t. 
				imageColor.a = Mathf.Lerp( 0.0f, 0.75f, t );
				textColor.a = Mathf.Lerp( 0.0f, 1.0f, t );

				// Apply the temp color to the image and text.
				gameOverScreen.color = imageColor;
				gameOverText.color = textColor;

				// Also lerp the font size from 50 to 100 by t.
				scoreText.fontSize = ( int )Mathf.Lerp( 50, 100, t );

				// Wait for next frame.
				yield return null;
			}

			// Apply a finalized amount to the alpha channels.
			imageColor.a = 0.75f;
			textColor.a = 1.0f;

			// Apply the final color values to the image and text.
			gameOverScreen.color = imageColor;
			gameOverText.color = textColor;
		}
		
		public void ModifyScore ( bool isDebris )
		{
			// Increase the score by the appropriate amount.
			score += isDebris == true ? debrisPoints : asteroidPoints;

			// Update the score text to reflect the current score.
			UpdateScoreText();
		}

		void UpdateScoreText ()
		{
			// Set the visual score amount to reflect the current score value.
			scoreText.text = score.ToString();
		}

		IEnumerator ShakeCamera ()
		{
			// Store the original position of the camera.
			Vector2 origPos = Camera.main.transform.position;
			for( float t = 0.0f; t < 1.0f; t += Time.deltaTime * 2.0f )
			{
				// Create a temporary vector2 with the camera's original position modified by a random distance from the origin.
				Vector2 tempVec = origPos + Random.insideUnitCircle;

				// Apply the temporary vector.
				Camera.main.transform.position = tempVec;

				// Yield until next frame.
				yield return null;
			}

			// Return back to the original position.
			Camera.main.transform.position = origPos;
		}
	}
}