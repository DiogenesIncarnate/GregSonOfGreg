using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : DynamicObject
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
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
