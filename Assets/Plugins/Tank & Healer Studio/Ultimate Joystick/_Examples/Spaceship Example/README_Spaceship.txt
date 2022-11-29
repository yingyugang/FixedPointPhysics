												 /-------------------\
												// SPACESHIP EXAMPLE \\
												\\		README		 //
												 \-------------------/

	The Spaceship Example files that are included with the Ultimate Joystick package are loosly based off of the
Network Meteoroid example created and released by Unity Technologies. We have placed the classes inside a custom
namespace to avoid any conflicts with any of your scripts that are used. This means that you will not be able to
reference any of these scripts without using the correct namespace. All networking funcitonality has been removed
with the scripts to allow use within versions 5.0.0 and 5.3.0 of Unity.

------------
| MOVEMENT |
------------
		First, the Ultimate Joystick is used for the input to move the spaceship around. We are using a Vector2 variable
	and storing the Ultimate Joystick's position within the Update function.

		movePosition = UltimateJoystick.GetPosition( "Movement" );

	In the FixedUpdate function we are creating a Vector3 to store a position that the player's transform can look at.
	We do this by taking the current position of the player, plus the new lookRot Vector3.

		Vector3 lookRot = new Vector3( movePosition.x, 0, movePosition.y );
		transform.LookAt( transform.position + lookRot );

	Then, after the player is facing the correct direction, we will add force to the player's rigidbody to make it move.
	We will do this by adding force in the player's forward direction and multiplying it by the distance that the joystick
	is from the center of the base.

		myRigidbody.AddForce( transform.forward * UltimateJoystick.GetDistance( "Movement" ) * 1000.0f * accelerationSpeed * Time.deltaTime );

	This will make the player move in the direction that the joystick is.

------------
| SHOOTING |
------------
		Second, we are using the Ultimate Joystick to get the input from the user to aim and shoot the gun that is located
	on the spaceship. This is done in much the same way as the movement of the spaceship. We are catching the users input
	from the joystick in the Update function.

		shootPosition = UltimateJoystick.GetPosition( "Shooting" );

	The process next is the exact same as with the movement. We will be creating a Vector3 variable to store the position
	that the joystick should be looking at. After getting the position of the joystick in the right axis, the we apply the
	gunRot variable, modified with the gunTrans' position in world space.

		Vector3 gunRot = new Vector3( shootPosition.x, 0, shootPosition.y );
		gunTrans.LookAt( gunTrans.position + gunRot );

	With this code, the gun of the spaceship is aiming towards the position that the user if making the right joystick face.
	Now, in order to make the player shoot the gun on the spaceship, we need to get the state of the joystick that is being
	used for shooting. We do this by calling the UltimateJoystick.GetJoystickState() function, and pass in the correct joystick
	name.

		if( UltimateJoystick.GetJoystickState( "Shooting" ) && shootingTimer <= 0 )
		{
			shootingTimer = shootingCooldown;
			CreateBullets();
		}

	The above example is implementing a cooldown to the speed at which the player can shoot bullets, but the Ultimate Joystick's
	function is being used to check if the user is currently using the joystick used for aiming the gun of the spaceship.

--------------
| CONCLUSION |
--------------
	If you are having any problems with the code, or need any assistance getting the Ultimate Joystick implemented into your
	project, then please contact us at TankAndHealerStudio@outlook.com and we will try to help you out as much as we can!

----------------
| HELPFUL INFO |
----------------
	Support Email:
		TankAndHealerStudio@outlook.com

	Network Meteoroid Package:
		https://www.assetstore.unity3d.com/en/#!/content/62227

	Support Website:
		http://www.tankandhealerstudio.com/assets.html