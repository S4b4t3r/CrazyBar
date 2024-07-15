using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    GameManager gameManager;
    public float dampingRatio;
    new Rigidbody2D rigidbody2D;
    new Collider2D collider2D;
    GameObject glow;
    [SerializeField] AudioSource sfxGood;
    
    ScoreManager scoreManager;
    public bool isOnPlateau = false;
    
    void Start()
    {
        scoreManager = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        glow = transform.GetChild(0).gameObject;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        string colliderTag = col.transform.tag;

        if (glow != null)
        {
            Destroy(glow);
        }

        if (!isOnPlateau)
        {
            if (colliderTag == "Plateau" || isProp(col) && gameManager.getIsGameOver() == false)
            {
                ContactPoint2D contact = col.GetContact(0);
                float contactAngle = Vector2.Angle(contact.normal, Vector2.right);

                // If angle good enough, stick to plateau/object
                if (contactAngle > 45f && contactAngle < 135f)
                {
                    Transform oTransform = contact.collider.transform;
                    rigidbody2D.SetRotation(oTransform.rotation.eulerAngles.z);

                    FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D>(); 
                    joint.anchor = (Vector2)transform.position - contact.point;
                    joint.connectedBody = oTransform.GetComponentInParent<Rigidbody2D>();
                    joint.enableCollision = false;
                    joint.dampingRatio = dampingRatio;

                    // Prevent further collisions
                    isOnPlateau = true;

					// Combo system if collision is on props
					
                    if(isProp(col))
                    {
                        scoreManager.AddToScore(
							1, 
							ReferenceEquals(col.gameObject, scoreManager.GetLastProp()) 
							? true 
							: false
						);
                    } else {
                        scoreManager.AddToScore(1, false);
                    }
					scoreManager.AddLastProp(gameObject);
                    sfxGood.Play();
                }
            } else {
                // Object fell on ground
                gameManager.GameOver();

                // Prevent further collisions
                isOnPlateau = true;
            }
        } else if (colliderTag == "World") {
            // Object fell from plateau to ground
            gameManager.GameOver();

            // Prevent further collisions
            isOnPlateau = true;
        }
    }

    bool isProp(Collision2D col)
    {
        return col.transform.tag == "Object" && col.transform.GetComponent<Object>().isOnPlateau;
    }

    public void FlyToTable()
    {
        FixedJoint2D[] joints = GetComponents<FixedJoint2D>();
        foreach (FixedJoint2D joint in joints)
        {
            Destroy(joint);
        }
        collider2D.enabled = false;
        rigidbody2D.angularVelocity = 0f;
        rigidbody2D.velocity = Vector2.up * 6f + (Vector2.left * (4.5f + transform.position.x)* .8f); // Left force based on distance to tables
        
        StartCoroutine(Despawn());
    }

    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(1.7f);

        if (!gameManager.getIsGameOver())
        {
            Destroy(this.gameObject);
        } else {
            collider2D.enabled = true;
        }
    }
}   
