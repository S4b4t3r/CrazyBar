using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarmanController : MonoBehaviour
{
    [SerializeField] GameObject thrownProp;
    [SerializeField] AudioSource throwSound;
    [SerializeField] ObjectSpawner objectSpawner;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void DoThrowAnimation()
    {
        animator.SetTrigger("Throw");
    }

    public void ThrowProp()
    {
        Instantiate(thrownProp, transform.position, Quaternion.identity);
        throwSound.Play();  
    }

    public void SpawnObject()
    {
        objectSpawner.SpawnObject();
    }

    public void MultiplyAnimatorSpeed(float speed)
    {
        animator.speed += speed;
    }
}
