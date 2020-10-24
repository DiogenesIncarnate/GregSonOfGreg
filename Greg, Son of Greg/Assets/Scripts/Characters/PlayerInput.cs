using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Pathfinding;

[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour {

	Player player;

	void Start () {
		player = GetComponent<Player> ();
	}

	void Update () {
		Vector2 directionalInput = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		player.SetDirectionalInput (directionalInput);

		if (Input.GetKeyDown (KeyCode.W)) {
			player.OnJumpInputDown ();
		}
		if (Input.GetKeyUp (KeyCode.W)) {
			player.OnJumpInputUp ();
		}
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.OnAttackInput();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("ProceduralLevel");
        }
    }
}
