using UnityEngine;
using System;
using System.Collections;

public class CharacterMotorJumping
{
    // Can the character jump?
    public bool enabled = true;

    // How high do we jump when pressing jump and letting go immediately
    public float baseHeight = 1.6f;

    // We add extraHeight units (meters) on top when holding the button down longer while jumping
    public float extraHeight = 1.6f;

    // How much does the character jump out perpendicular to the surface on walkable surfaces?
    // 0 means a fully vertical jump and 1 means fully perpendicular.
    public float perpAmount = 2.0f;

    // How much does the character jump out perpendicular to the surface on too steep surfaces?
    // 0 means a fully vertical jump and 1 means fully perpendicular.
    public float steepPerpAmount = 1.5f;

    // For the next variables, @System.NonSerialized tells Unity to not serialize the variable or show it in the inspector view.
    // Very handy for organization!

    // Are we jumping? (Initiated with jump button and not grounded yet)
    // To see if we are just in the air (initiated by jumping OR falling) see the grounded variable.
    [NonSerialized]
    public bool jumping = false;

    [NonSerialized]
    public bool holdingJumpButton = false;

    // the time we jumped at (Used to determine for how long to apply extra jump power after jumping.)
    [NonSerialized]
    public float lastStartTime = 0.0f;

    [NonSerialized]
    public float lastButtonDownTime = -100f;

    [NonSerialized]
    public Vector3 jumpDir = Vector3.up;
}

public class CharacterMotorMovingPlatform
{
    public bool enabled = true;

    public MovementTransferOnJump movementTransfer = MovementTransferOnJump.PermaTransfer;

    [NonSerialized]
    public Transform hitPlatform;

    [NonSerialized]
    public Transform activePlatform;

    [NonSerialized]
    public Vector3 activeLocalPoint;

    [NonSerialized]
    public Vector3 activeGlobalPoint;

    [NonSerialized]
    public Quaternion activeLocalRotation;

    [NonSerialized]
    public Quaternion activeGlobalRotation;

    [NonSerialized]
    public Matrix4x4 lastMatrix;

    [NonSerialized]
    public Vector3 platformVelocity;

    [NonSerialized]
    public bool newPlatform;
}

public class CharacterMotorMovement
{
    // For the next variables, @System.NonSerialized tells Unity to not serialize the variable or show it in the inspector view.
    // Very handy for organization!

    // The last collision flags returned from controller.Move
    [NonSerialized]
    public CollisionFlags collisionFlags;

    // We will keep track of the character's current velocity,
    [NonSerialized]
    public Vector3 velocity;

    // This keeps track of our current velocity while we're not grounded
    [NonSerialized]
    public Vector3 frameVelocity = Vector3.zero;

    [NonSerialized]
    public Vector3 hitPoint = Vector3.zero;

    [NonSerialized]
    public Vector3 lastHitPoint = new Vector3(Mathf.Infinity, 0, 0);
}

public class CharacterMotorSliding
{
    // Does the character slide on too steep surfaces?
    public bool enabled = true;

    // How fast does the character slide on steep surfaces?
    public float slidingSpeed = 15f;

    // How much can the player control the sliding direction?
    // If the value is 0.5 the player can slide sideways with half the speed of the downwards sliding speed.
    public float sidewaysControl = 1.0f;

    // How much can the player influence the sliding speed?
    // If the value is 0.5 the player can speed the sliding up to 150% or slow it down to 50%.
    public float speedControl = 0.4f;
}

public enum MovementTransferOnJump
{
    None,
    // The jump is not affected by velocity of floor at all.
    InitTransfer,
    // Jump gets its initial velocity from the floor, then gradualy comes to a stop.
    PermaTransfer,
    // Jump gets its initial velocity from the floor, and keeps that velocity until landing.
    PermaLocked
    // Jump is relative to the movement of the last touched floor and will move together with that floor.
}

[System.Serializable]
public class MotorPreset
{
    public Vector3 Center = new Vector3(0, 0.9f, 0);
    public float Height = 1.8f;
    public float Redius = 0.3f;
    public Vector3 FPSCamOffset = new Vector3(0, 1.5f, 0);
}

[RequireComponent(typeof(CharacterController))]
[AddComponentMenu("Character/Character Motor")]
public class CharacterMotor : MonoBehaviour
{
    public bool canControl = true;
    public bool isEnabled = true;
    public float boostMults = 1;
    public float maxForwardSpeed = 5.0f;
    public float maxSidewaysSpeed = 5.0f;
    public float maxBackwardsSpeed = 5.0f;
    public MotorPreset[] MotorPreset = { new MotorPreset() };
    public int MovementIndex = 0;

    public bool useFixedUpdate = true;
    public AnimationCurve slopeSpeedMultiplier = new AnimationCurve(new Keyframe(-90, 1), new Keyframe(0, 1), new Keyframe(90, 0));
    public float maxGroundAcceleration = 30.0f;
    public float maxAirAcceleration = 20.0f;
    public float gravity = 10.0f;
    public float maxFallSpeed = 20.0f;

    [NonSerialized]
    public Vector3 inputMoveDirection = Vector3.zero;
    [NonSerialized]
    public bool inputJump = false;
    [NonSerialized]
    public bool climb = false;

    public CharacterMotorMovement movement = new CharacterMotorMovement();
    private CharacterMotorJumping jumping = new CharacterMotorJumping();
    private CharacterMotorMovingPlatform movingPlatform = new CharacterMotorMovingPlatform();
    private CharacterMotorSliding sliding = new CharacterMotorSliding();

    //[NonSerialized]
    public bool grounded = true;
    [NonSerialized]
    public Vector3 groundNormal = Vector3.zero;
    private Vector3 lastGroundNormal = Vector3.zero;
    [HideInInspector]
    public CharacterController controller;
    public int layerMask = 8;

    public void Reset()
    {
        inputJump = false;
        inputMoveDirection = Vector3.zero;
    }

    private void Awake()
    {
        controller = gameObject.GetComponent<CharacterController>();
    }

    public void Climb(float speed)
    {
        climb = true;
        controller.transform.position += (Vector3.up * speed * Time.deltaTime);
    }

    private void UpdateFunction()
    {
        if (!this.enabled)
            return;

        var lastPosition = this.transform.position;
        if (MotorPreset.Length > MovementIndex)
        {
            if (controller.enabled)
            {
                controller.height = MotorPreset[MovementIndex].Height;
                controller.center = MotorPreset[MovementIndex].Center;
                controller.radius = MotorPreset[MovementIndex].Redius;
            }
        }

        if (isEnabled)
        {
            if (controller.enabled)
            {
                var velocity = movement.velocity;
                // Update velocity based on input
                velocity = ApplyInputVelocityChange(velocity);

                // Apply gravity and jumping force
                velocity = ApplyGravityAndJumping(velocity);

                // Moving platform support
                var moveDistance = Vector3.zero;
                if (MoveWithPlatform())
                {
                    var newGlobalPoint = movingPlatform.activePlatform.TransformPoint(movingPlatform.activeLocalPoint);
                    moveDistance = (newGlobalPoint - movingPlatform.activeGlobalPoint);
                    if (moveDistance != Vector3.zero)
                    {
                        controller.Move(moveDistance);
                    }
                    // Support moving platform rotation as well:
                    var newGlobalRotation = movingPlatform.activePlatform.rotation * movingPlatform.activeLocalRotation;
                    var rotationDiff = newGlobalRotation * Quaternion.Inverse(movingPlatform.activeGlobalRotation);

                    var yRotation = rotationDiff.eulerAngles.y;
                    if (yRotation != 0)
                    {
                        // Prevent rotation of the local up vector
                        this.transform.Rotate(0, yRotation, 0);
                    }
                }

                // Save lastPosition for velocity calculation.

                // We always want the movement to be framerate independent.  Multiplying by Time.deltaTime does this.
                var currentMovementOffset = velocity * Time.deltaTime;
                // Find out how much we need to push towards the ground to avoid loosing grouning
                // when walking down a step or over a sharp change in slope.
                var pushDownOffset = Mathf.Max(controller.stepOffset, new Vector3(currentMovementOffset.x, 0, currentMovementOffset.z).magnitude);

                if (grounded)
                {
                    currentMovementOffset -= pushDownOffset * Vector3.up;
                }

                if (climb)
                {
                    currentMovementOffset.y = 0f;
                    float moveDir = Vector3.Dot(this.transform.forward, velocity.normalized);
                    if (moveDir < 0 && !ongroundtest)
                    {
                        currentMovementOffset.x = 0;
                        currentMovementOffset.z = 0;
                    }
                }

                // Reset variables that will be set by collision function
                movingPlatform.hitPlatform = null;
                groundNormal = Vector3.zero;
                movement.collisionFlags = controller.Move(currentMovementOffset);
                movement.lastHitPoint = movement.hitPoint;
                lastGroundNormal = groundNormal;

                if (movingPlatform.enabled && movingPlatform.activePlatform != movingPlatform.hitPlatform)
                {
                    if (movingPlatform.hitPlatform != null)
                    {
                        movingPlatform.activePlatform = movingPlatform.hitPlatform;
                        movingPlatform.lastMatrix = movingPlatform.hitPlatform.localToWorldMatrix;
                        movingPlatform.newPlatform = true;
                    }
                }

                // Calculate the velocity based on the current and previous position.  
                // This means our velocity will only be the amount the character actually moved as a result of collisions.
                var oldHVelocity = new Vector3(velocity.x, 0, velocity.z);
                movement.velocity = (this.transform.position - lastPosition) / Time.deltaTime;
                var newHVelocity = new Vector3(movement.velocity.x, 0, movement.velocity.z);

                // The CharacterController can be moved in unwanted directions when colliding with things.
                // We want to prevent this from influencing the recorded velocity.
                if (oldHVelocity == Vector3.zero)
                {
                    movement.velocity = new Vector3(0, movement.velocity.y, 0);
                }
                else
                {
                    var projectedNewVelocity = Vector3.Dot(newHVelocity, oldHVelocity) / oldHVelocity.sqrMagnitude;
                    movement.velocity = oldHVelocity * Mathf.Clamp01(projectedNewVelocity) + movement.velocity.y * Vector3.up;
                }

                if (movement.velocity.y < velocity.y - 0.001)
                {
                    if (movement.velocity.y < 0)
                    {
                        // Something is forcing the CharacterController down faster than it should.
                        // Ignore this
                        movement.velocity.y = velocity.y;
                    }
                    else
                    {
                        // The upwards movement of the CharacterController has been blocked.
                        // This is treated like a ceiling collision - stop further jumping here.
                        jumping.holdingJumpButton = false;
                    }
                }

                // We were grounded but just loosed grounding
                if (grounded && !IsGroundedTest())
                {
                    grounded = false;
                    // Apply inertia from platform
                    if (movingPlatform.enabled &&
                        (movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer ||
                        movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer))
                    {
                        movement.frameVelocity = movingPlatform.platformVelocity;
                        movement.velocity += movingPlatform.platformVelocity;
                    }

                    SendMessage("OnFall", SendMessageOptions.DontRequireReceiver);
                    // We pushed the character down to ensure it would stay on the ground if there was any.
                    // But there wasn't so now we cancel the downwards offset to make the fall smoother.
                    this.transform.position += pushDownOffset * Vector3.up;
                }
                // We were not grounded but just landed on something
                else if (!grounded && IsGroundedTest())
                {

                    grounded = true;
                    jumping.jumping = false;
                    SubtractNewPlatformVelocity();
                    SendMessage("OnLand", SendMessageOptions.DontRequireReceiver);
                }

                // Moving platforms support
                if (MoveWithPlatform())
                {
                    // Use the center of the lower half sphere of the capsule as reference point.
                    // This works best when the character is standing on moving tilting platforms. 
                    movingPlatform.activeGlobalPoint = this.transform.position + Vector3.up * (controller.center.y - (controller.height * 0.5f) + controller.radius);
                    movingPlatform.activeLocalPoint = movingPlatform.activePlatform.InverseTransformPoint(movingPlatform.activeGlobalPoint);

                    // Support moving platform rotation as well:
                    movingPlatform.activeGlobalRotation = this.transform.rotation;
                    movingPlatform.activeLocalRotation = Quaternion.Inverse(movingPlatform.activePlatform.rotation) * movingPlatform.activeGlobalRotation;
                }
                ongroundtest = false;
                climb = false;
            }
        }
        else
        {
            grounded = false;
            RaycastHit hit;
            int mask = (1 << layerMask);
            if (Physics.Raycast(this.transform.position + controller.center + (Vector3.up * 0.01f), -Vector3.up, out hit, ((controller.height / 2) + controller.stepOffset + 0.01f), mask))
            {
                if (hit.collider && hit.collider.gameObject.transform.root != this.gameObject.transform.root)
                    grounded = true;
            }
        }
    }


    void FixedUpdate()
    {
        if (movingPlatform.enabled)
        {
            if (movingPlatform.activePlatform != null)
            {
                if (!movingPlatform.newPlatform)
                {
                    movingPlatform.platformVelocity = (
                        movingPlatform.activePlatform.localToWorldMatrix.MultiplyPoint3x4(movingPlatform.activeLocalPoint)
                        - movingPlatform.lastMatrix.MultiplyPoint3x4(movingPlatform.activeLocalPoint)
                    ) / Time.deltaTime;
                }
                movingPlatform.lastMatrix = movingPlatform.activePlatform.localToWorldMatrix;
                movingPlatform.newPlatform = false;
            }
            else
            {
                movingPlatform.platformVelocity = Vector3.zero;
            }
        }

        if (useFixedUpdate)
        {
            UpdateFunction();
        }
    }

    void Update()
    {
        if (!useFixedUpdate)
        {
            UpdateFunction();
        }
    }

    Vector3 ApplyInputVelocityChange(Vector3 velocity)
    {
        if (!canControl)
            inputMoveDirection = Vector3.zero;

        // Find desired velocity
        Vector3 desiredVelocity;
        if (grounded && TooSteep())
        {
            // The direction we're sliding in
            desiredVelocity = new Vector3(groundNormal.x, 0, groundNormal.z).normalized;
            // Find the input movement direction projected onto the sliding direction
            var projectedMoveDir = Vector3.Project(inputMoveDirection, desiredVelocity);
            // Add the sliding direction, the spped control, and the sideways control vectors
            desiredVelocity = desiredVelocity + projectedMoveDir * sliding.speedControl + (inputMoveDirection - projectedMoveDir) * sliding.sidewaysControl;
            // Multiply with the sliding speed
            desiredVelocity *= sliding.slidingSpeed;
        }
        else
            desiredVelocity = GetDesiredHorizontalVelocity();

        if (movingPlatform.enabled && movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
        {
            desiredVelocity += movement.frameVelocity * 10;
            desiredVelocity.y = 0;
        }

        if (grounded)
            desiredVelocity = AdjustGroundVelocityToNormal(desiredVelocity, groundNormal);
        else
            velocity.y = 0;

        // Enforce max velocity change
        var maxVelocityChange = GetMaxAcceleration(grounded) * Time.deltaTime;
        var velocityChangeVector = (desiredVelocity - velocity);
        if (velocityChangeVector.sqrMagnitude > maxVelocityChange * maxVelocityChange)
        {
            velocityChangeVector = velocityChangeVector.normalized * maxVelocityChange;
        }
        // If we're in the air and don't have control, don't apply any velocity change at all.
        // If we're on the ground and don't have control we do apply it - it will correspond to friction.

        if (Vector3.Dot(desiredVelocity.normalized, velocity.normalized) < 0.9)
        {
            velocity = Vector3.Lerp(velocity, desiredVelocity, 0.5f);
        }

        if (grounded || canControl)
            velocity += velocityChangeVector;

        if (grounded)
        {
            // When going uphill, the CharacterController will automatically move up by the needed amount.
            // Not moving it upwards manually prevent risk of lifting off from the ground.
            // When going downhill, DO move down manually, as gravity is not enough on steep hills.
            velocity.y = Mathf.Min(velocity.y, 0);
        }

        return velocity;
    }

    Vector3 ApplyGravityAndJumping(Vector3 velocity)
    {
        if (climb)
            return velocity;

        if (!inputJump || !canControl)
        {
            jumping.holdingJumpButton = false;
            jumping.lastButtonDownTime = -100;
        }

        if (inputJump && jumping.lastButtonDownTime < 0 && canControl)
            jumping.lastButtonDownTime = Time.time;

        if (grounded)
            velocity.y = Mathf.Min(0, velocity.y) - gravity * Time.deltaTime;
        else
        {
            velocity.y = movement.velocity.y - gravity * Time.deltaTime * 2;

            // When jumping up we don't apply gravity for some time when the user is holding the jump button.
            // This gives more control over jump height by pressing the button longer.
            if (jumping.jumping && jumping.holdingJumpButton)
            {
                // Calculate the duration that the extra jump force should have effect.
                // If we're still less than that duration after the jumping time, apply the force.
                if (Time.time < jumping.lastStartTime + jumping.extraHeight / CalculateJumpVerticalSpeed(jumping.baseHeight))
                {
                    // Negate the gravity we just applied, except we push in jumpDir rather than jump upwards.
                    velocity += jumping.jumpDir * gravity * Time.deltaTime;
                }
            }

            // Make sure we don't fall any faster than maxFallSpeed. This gives our character a terminal velocity.
            velocity.y = Mathf.Max(velocity.y, -maxFallSpeed);
        }


        if (grounded)
        {
            // Jump only if the jump button was pressed down in the last 0.2 seconds.
            // We use this check instead of checking if it's pressed down right now
            // because players will often try to jump in the exact moment when hitting the ground after a jump
            // and if they hit the button a fraction of a second too soon and no new jump happens as a consequence,
            // it's confusing and it feels like the game is buggy.
            if (jumping.enabled && canControl && (Time.time - jumping.lastButtonDownTime < 0.2))
            {
                grounded = false;
                jumping.jumping = true;
                jumping.lastStartTime = Time.time;
                jumping.lastButtonDownTime = -100;
                jumping.holdingJumpButton = true;

                // Calculate the jumping direction
                if (TooSteep())
                    jumping.jumpDir = Vector3.Slerp(Vector3.up, groundNormal, jumping.steepPerpAmount);
                else
                    jumping.jumpDir = Vector3.Slerp(Vector3.up, groundNormal, jumping.perpAmount);

                // Apply the jumping force to the velocity. Cancel any vertical velocity first.
                velocity.y = 0;
                velocity += jumping.jumpDir * CalculateJumpVerticalSpeed(jumping.baseHeight);

                // Apply inertia from platform
                if (movingPlatform.enabled &&
                    (movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer ||
                    movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer))
                {
                    movement.frameVelocity = movingPlatform.platformVelocity;
                    velocity += movingPlatform.platformVelocity;
                }

                SendMessage("OnJump", SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                jumping.holdingJumpButton = false;
            }
        }

        return velocity;
    }

    private bool ongroundtest;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.y > 0 && hit.normal.y > groundNormal.y && hit.moveDirection.y < 0)
        {
            if ((hit.point - movement.lastHitPoint).sqrMagnitude > 0.001 || lastGroundNormal == Vector3.zero)
                groundNormal = hit.normal;
            else
                groundNormal = lastGroundNormal;

            movingPlatform.hitPlatform = hit.collider.transform;
            movement.hitPoint = hit.point;
            movement.frameVelocity = Vector3.zero;
            ongroundtest = true;
        }

    }

    private IEnumerable SubtractNewPlatformVelocity()
    {
        // When landing, subtract the velocity of the new ground from the character's velocity
        // since movement in ground is relative to the movement of the ground.
        if (movingPlatform.enabled &&
            (movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer ||
            movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer))
        {
            // If we landed on a new platform, we have to wait for two FixedUpdates
            // before we know the velocity of the platform under the character
            if (movingPlatform.newPlatform)
            {
                var platform = movingPlatform.activePlatform;
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                if (grounded && platform == movingPlatform.activePlatform)
                    yield return 1;
            }
            movement.velocity -= movingPlatform.platformVelocity;
        }
    }

    private bool MoveWithPlatform()
    {
        return movingPlatform.enabled
        && (grounded || movingPlatform.movementTransfer == MovementTransferOnJump.PermaLocked)
        && movingPlatform.activePlatform != null;
    }

    private Vector3 GetDesiredHorizontalVelocity()
    {
        // Find desired velocity
        var desiredLocalDirection = this.transform.InverseTransformDirection(inputMoveDirection);
        var maxSpeed = MaxSpeedInDirection(desiredLocalDirection);
        if (grounded)
        {
            // Modify max speed on slopes based on slope speed multiplier curve
            var movementSlopeAngle = Mathf.Asin(movement.velocity.normalized.y) * Mathf.Rad2Deg;
            maxSpeed *= slopeSpeedMultiplier.Evaluate(movementSlopeAngle);
        }
        return this.transform.TransformDirection(desiredLocalDirection * maxSpeed);
    }

    private Vector3 AdjustGroundVelocityToNormal(Vector3 hVelocity, Vector3 groundNormal)
    {
        var sideways = Vector3.Cross(Vector3.up, hVelocity);
        return Vector3.Cross(sideways, groundNormal).normalized * hVelocity.magnitude;
    }

    public bool IsGroundedTest()
    {
        return groundNormal.y > 0.01;
    }

    float GetMaxAcceleration(bool grounded)
    {
        return grounded ? maxGroundAcceleration : maxAirAcceleration;
    }

    float CalculateJumpVerticalSpeed(float targetJumpHeight)
    {
        return Mathf.Sqrt(2 * targetJumpHeight * gravity);
    }

    bool IsJumping()
    {
        return jumping.jumping;
    }

    bool IsSliding()
    {
        return (grounded && sliding.enabled && TooSteep());
    }

    bool IsTouchingCeiling()
    {
        return (movement.collisionFlags & CollisionFlags.CollidedAbove) != 0;
    }

    public bool IsGrounded()
    {
        return grounded;
    }

    bool TooSteep()
    {
        return (groundNormal.y <= Mathf.Cos(controller.slopeLimit * Mathf.Deg2Rad));
    }

    Vector3 GetDirection()
    {
        return inputMoveDirection;
    }

    void SetControllable(bool controllable)
    {
        canControl = controllable;
    }

    float MaxSpeedInDirection(Vector3 desiredMovementDirection)
    {
        if (desiredMovementDirection == Vector3.zero)
            return 0;
        else
        {
            var zAxisEllipseMultiplier = (desiredMovementDirection.z > 0 ? maxForwardSpeed * boostMults : maxBackwardsSpeed * boostMults) / maxSidewaysSpeed * boostMults;
            var temp = new Vector3(desiredMovementDirection.x, 0, desiredMovementDirection.z / zAxisEllipseMultiplier).normalized;
            var length = new Vector3(temp.x, 0, temp.z * zAxisEllipseMultiplier).magnitude * maxSidewaysSpeed * boostMults;
            return length;
        }
    }

    void SetVelocitys(Vector3 velocity)
    {
        grounded = false;
        movement.velocity = velocity;
        movement.frameVelocity = Vector3.zero;
        SendMessage("OnExternalVelocity");
    }
}

