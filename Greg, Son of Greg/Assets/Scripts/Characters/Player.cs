using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Tilemaps;

public class Player : DynamicCharacter
{
    private Vector2 wallJumpClimb = new Vector2(3.5f, 16f);
	private Vector2 wallJumpOff = new Vector2(8f, 16f);
	private Vector2 wallLeap = new Vector2(18f, 17f);
	private float wallSlideSpeedMax = 3;
	private float wallStickTime = .25f;
    private float timeToWallUnstick;
	private float maxJumpVelocity;
	private float minJumpVelocity;
	private bool wallSliding;
	private int wallDirX;

	protected override void Start() {
        base.Start();

		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
	}

	protected override void Update() {
        if (velocity.y != 0 && controller.collisions.below)
        {
            animator.SetTrigger("GroundImpact");
        }

        base.Update();

        HandleWallSliding();
    }

	public void OnJumpInputDown() {
		if (wallSliding) {
			if (wallDirX == directionalInput.x) {
				velocity.x = -wallDirX * wallJumpClimb.x;
				velocity.y = wallJumpClimb.y;
			}
			else if (directionalInput.x == 0) {
				velocity.x = -wallDirX * wallJumpOff.x;
				velocity.y = wallJumpOff.y;
			}
			else {
				velocity.x = -wallDirX * wallLeap.x;
				velocity.y = wallLeap.y;
			}
		}
		if (controller.collisions.below) {
			velocity.y = maxJumpVelocity;
		}
	}

	public void OnJumpInputUp() {
		if (velocity.y > minJumpVelocity) {
			velocity.y = minJumpVelocity;
		}
	}
		
	void HandleWallSliding() {
		wallDirX = (controller.collisions.left) ? -1 : 1;
		wallSliding = false;
		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
			wallSliding = true;

			if (velocity.y < -wallSlideSpeedMax) {
				velocity.y = -wallSlideSpeedMax;
			}

			if (timeToWallUnstick > 0) {
				velocityXSmoothing = 0;
				velocity.x = 0;

				if (directionalInput.x != wallDirX && directionalInput.x != 0) {
					timeToWallUnstick -= Time.deltaTime;
				}
				else {
					timeToWallUnstick = wallStickTime;
				}
			}
			else {
				timeToWallUnstick = wallStickTime;
			}

		}

	}

    protected override void SetPhysicsFromAbilities()
    {
        base.SetPhysicsFromAbilities();

        wallJumpClimb = new Vector2(0.3f * might, 1.5f * might);
        wallJumpOff = new Vector2(0.5f * might, 1f * might);
        wallLeap = new Vector2(1.5f * might, 1.5f * might);
        wallSlideSpeedMax = 40f / might;
        wallStickTime = .04f * wit;
}

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
