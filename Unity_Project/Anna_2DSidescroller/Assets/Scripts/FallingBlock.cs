using System.Collections;
using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    [SerializeField] float TimeforFall = 1f;
    [SerializeField] LayerMask canInteract = -1;
    [SerializeField] Vector3 FalldownSpeed = new Vector3(0,1,0);
    [SerializeField] float shakeamount = 0.1f;
    [SerializeField] float fallingTimer = 1f;
    [SerializeField] bool Resetable = false;
    [SerializeField] float timeforReset = 5f;

    private Vector3 startposition = new Vector3(0, 0, 0);
    private bool shaking = false;
    private float counter = 0.0f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Collsion Detected");
        if (canInteract == (canInteract | (1 << collision.gameObject.layer)))
        {
            //Debug.Log("Player Detected");
            StartCoroutine(Falldown());
        }
    }
    IEnumerator Falldown()
    {
        startposition = transform.position;
        //Debug.Log("FalldownRoutine Started");
        StartCoroutine(ShakeBlock());
        yield return new WaitForSeconds(TimeforFall);
        //Debug.Log("WaitedForFall");
        StartCoroutine(Falling());
        yield return new WaitForSeconds(fallingTimer);
        if (!Resetable)
        {
            //Debug.Log("Destroying gameobject");
            Destroy(this.gameObject);
        }
        else
        {
            yield return new WaitForSeconds(timeforReset);
            StartCoroutine(ReturnToStart());
        }

    }
    IEnumerator Falling()
    {
        for (float fallingtime = fallingTimer; fallingtime >= 0; fallingtime -= .1f)
        {
            transform.Translate(-FalldownSpeed * Time.deltaTime);
            yield return null;
        }

    }

    IEnumerator ReturnToStart()
    {
        transform.position = startposition;
        yield return null;
    }

    IEnumerator ShakeBlock()
    {
        if (!shaking)
        {
            shaking = true;
            while (counter < TimeforFall)
            {
                counter += Time.deltaTime;
                transform.Translate(shakeamount, 0, 0);
                shakeamount = -shakeamount;
                //Debug.Log("shaking" + shakeamount);
                yield return null;
            }
            shaking = false;
            counter = 0.0f;
        }
    }
}