using Sandbox;
using Sandbox.Citizen;
using Framework;

namespace Framework;
public partial class Player : Component, IPlayer
{
	// Components

	private CharacterController characterController;

	private CitizenAnimationHelper animationHelper;

	private CameraComponent Camera;

	private SkinnedModelRenderer modelRenderer;

	// Game Objects

	[Property] public GameObject Head { get; set; }

	[Property] public GameObject Body { get; set; }

	// Movement Properties

	[Property] public float GroundControl { get; set; } = 4.0f;

	[Property] public float AirControl { get; set; } = 0.1f;

	[Property] public float MaxForce { get; set; } = 50f;

	[Property] public float Speed { get; set; } = 160f;

	[Property] public float RunSpeed { get; set; } = 290f;

	[Property] public float CrouchSpeed { get; set; } = 90f;

	[Property] public float JumpForce { get; set; } = 400f;

	public GameObject PickedUp { get; set; }

	// Movement Variables

	public Vector3 WishVelocity = Vector3.Zero;
	public bool IsCrouching = false;
	public bool IsRunning = false;

	// Camera Properties

	[Property] public float CamDistance { get; set; } = 0f;

	// Camera Variables

	private bool IsFirstPerson => CamDistance == 0f;
	public Angles eyeAngles;

	protected override void OnAwake()
	{
		characterController = Components.GetInChildrenOrSelf<CharacterController>();
		animationHelper = Components.GetInChildrenOrSelf<CitizenAnimationHelper>();
		Camera = Components.GetInChildrenOrSelf<CameraComponent>();
		modelRenderer = Components.GetInChildrenOrSelf<SkinnedModelRenderer>();
	}

	protected override void OnUpdate() // Every frame update
	{
		if ( IsProxy ) return;
		if ( Input.Pressed( "Use" ) ) Use();
		if ( Input.Pressed( "Attack1" ) ) Throw();
		if ( Input.Pressed( "Attack2" ) ) Pickup();
		IsCrouching = Input.Down( "Crouch" );
		IsRunning = Input.Down( "Run" );

		// Rotate head based on mouse movement
		
		eyeAngles = Head.WorldRotation.Angles();
		eyeAngles.pitch += Input.MouseDelta.y * 0.1f;
		eyeAngles.yaw += Input.MouseDelta.x * -0.1f;
		eyeAngles.roll = 0f;
		eyeAngles.pitch = eyeAngles.pitch.Clamp( -89.9f, 89.9f ); // Clamp pitch to screen bounds to prevent flipping
		Head.WorldRotation = eyeAngles.ToRotation();

		// Set camera position

		var camPos = Head.WorldPosition;
		if ( Camera is not null )
		{
			if ( !IsFirstPerson )
			{
				// Trace backwards to see where camera could safely be
				var camForward = eyeAngles.ToRotation().Forward;
				var camTr = Scene.Trace.Ray( camPos, camPos - ( camForward * CamDistance ) ) // Trace backwards the distance of the camera to where the head is looking to prevent clipping
					.WithoutTags( "player" )
					.Run();

				if ( camTr.Hit ) camPos = camTr.HitPosition + camTr.Normal; // Move camera slightly from surface of hit position
			}
		}

		Camera.WorldPosition = camPos;
		Camera.WorldRotation = eyeAngles.ToRotation();

		RenderLegsNoBody();
	}

	protected override void OnFixedUpdate() // Every physics update
	{
		RotateBody();
		UpdateAnimations();
		
		if ( PickedUp is not null )
		{
			PickedUp.WorldPosition = Head.WorldPosition + ( Head.WorldRotation.Forward * 50f );
			PickedUp.WorldRotation = Head.WorldRotation;
		}

		if ( IsProxy ) return;
		BuildWishVelocity();
		Move();
	}

	void BuildWishVelocity()
	{
		WishVelocity = 0;

		var rot = Head.WorldRotation;
		if ( Input.Down( "Forward" ) ) WishVelocity += rot.Forward;
		if ( Input.Down( "Backward" ) ) WishVelocity += rot.Backward;
		if ( Input.Down( "Left" ) ) WishVelocity += rot.Left;
		if ( Input.Down( "Right" ) ) WishVelocity += rot.Right;

		WishVelocity = WishVelocity.WithZ( 0 ); // Remove vertical velocity
		if ( !WishVelocity.IsNearZeroLength ) WishVelocity = WishVelocity.Normal;

		if ( IsCrouching ) WishVelocity *= CrouchSpeed;
		else if ( IsRunning ) WishVelocity *= RunSpeed;
		else WishVelocity *= Speed;
	}

	void Move()
	{
		var gravity = Scene.PhysicsWorld.Gravity;

		if ( characterController.IsOnGround )
		{
			characterController.Velocity = characterController.Velocity.WithZ( 0 );
			characterController.Accelerate( WishVelocity );
			characterController.ApplyFriction( GroundControl );
		} else
		{
			characterController.Velocity += gravity * Time.Delta * 0.5f; // Apply half gravity before movement
			characterController.Accelerate( WishVelocity.ClampLength( MaxForce ) ); // Only move as fast as max force
			characterController.ApplyFriction( AirControl );
		}

		if ( !( characterController.Velocity.IsNearZeroLength && WishVelocity.IsNearZeroLength ) ) characterController.Move();
		if ( !characterController.IsOnGround ) characterController.Velocity += gravity * Time.Delta * 0.5f; // Apply half gravity after movement
	}

	void RenderLegsNoBody()
	{
		modelRenderer.SetBodyGroup( "head", 2 );
		modelRenderer.SetBodyGroup( "chest", 1 );
		modelRenderer.SetBodyGroup( "hands", 1 );

		var headRot = Head.WorldRotation.Angles().WithPitch( 0 ).ToRotation();
		var offset = characterController.Velocity.Length > 0f ? headRot.Forward * 16f : headRot.Forward * 13f;
        Body.WorldPosition = ( Head.WorldPosition - offset ).WithZ( 0 ); // // Set body position relative to head
	}

	void RotateBody()
	{
		var targetRot = new Angles( 0, eyeAngles.ToRotation().Yaw(), 0 ).ToRotation();
		float rotateDiff = Body.WorldRotation.Distance( targetRot );

		if ( rotateDiff > 20f || characterController.Velocity.Length > 10f )
		{
			Body.WorldRotation = Rotation.Lerp( Body.WorldRotation, targetRot, Time.Delta * 2f );
		}
	}

	void UpdateAnimations()
	{
		if ( animationHelper is null ) return;
		animationHelper.WithWishVelocity( WishVelocity );
		animationHelper.WithVelocity( characterController.Velocity );
		animationHelper.AimAngle = eyeAngles.ToRotation();
		animationHelper.IsGrounded = characterController.IsOnGround;
		animationHelper.WithLook( eyeAngles.ToRotation().Forward, 1f, 0.75f, 0.5f );
		animationHelper.MoveStyle = IsRunning ? CitizenAnimationHelper.MoveStyles.Run : CitizenAnimationHelper.MoveStyles.Walk;
		animationHelper.DuckLevel = IsCrouching ? 1 : 0;
	}

	void Use()
	{

	}

	void Throw()
	{
		if ( PickedUp is null ) return;
	}

	void Pickup()
	{
		if ( PickedUp is not null )
		{
			PickedUp.Components.Get<Rigidbody>().Gravity = true;
			PickedUp = null;
			return;
		}

		var tr = Scene.Trace.Ray( Head.WorldPosition, Head.WorldPosition + ( eyeAngles.ToRotation().Forward * 150f ) )
			.WithTag( "pickup" )
			.Run();

		if ( !tr.Hit ) return;

		PickedUp = tr.GameObject;
		PickedUp.Components.Get<Rigidbody>().Gravity = false;
	}
}
