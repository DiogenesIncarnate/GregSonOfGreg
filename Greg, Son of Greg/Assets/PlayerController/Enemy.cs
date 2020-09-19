using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : DynamicObject
{
    public Player player;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        player = GameObject.FindObjectOfType<Player>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        FacePlayer();
    }

    public void FacePlayer()
    {
        Vector2 distFromPlayer = player.transform.position - transform.position;
        if(Mathf.Abs(distFromPlayer.y) < 1.0f && Mathf.Abs(distFromPlayer.x) < 10.0f)
        {
            Vector3 tempScale = transform.localScale;
            tempScale.x = Mathf.Abs(tempScale.x) * Mathf.Sign(distFromPlayer.x);
            transform.localScale = tempScale;
        }
    }

    public void Hit(GameObject source)
    {
        Vector2 hitDir = new Vector2(Mathf.Sign(transform.position.x - source.transform.position.x) * 5.0f, 5.0f);
        velocity += new Vector3(hitDir.x, hitDir.y, 0);
        Damage(5);
    }

    void Damage(float dmg)
    {
        currentHP -= dmg;
        if(currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }
}
