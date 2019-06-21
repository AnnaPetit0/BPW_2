using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_Manager : MonoBehaviour
{
    [Tooltip("Only for movingItem")]
    [SerializeField] bool isMovingitem = true;
    [Tooltip("Only for AppearingItem")]
    [SerializeField] bool isAppearingitem = false;
    [Tooltip("Only for SwitchingItem")]
    [SerializeField] bool isSwitchingitem = false;
    [Tooltip("Only for movingItem")]
    [SerializeField] Transform startPoint = null;
    [Tooltip("Only for movingItem")]
    [SerializeField] Transform endPoint = null;
    [Tooltip("Only for movingItem")]
    [SerializeField] bool shouldLoop = false;
    [Tooltip("Only for movingItem")]
    [SerializeField] float moveSpeed = 0.1f;
    [Tooltip("Only for AppearingItem")]
    [SerializeField] bool Disappear = false;
    [Tooltip("Only for SwitchingItem")]
    [SerializeField] Sprite State_1 = null;
    [Tooltip("Only for SwitchingItem")]
    [SerializeField] Sprite State_2 = null;
    [Tooltip("This function is activated without a button of sorts")]
    [SerializeField] bool isActivated = false;
    //[Tooltip("If platform moves horizontally, should the player stick to it or not")]
    //[SerializeField] public bool shouldStick;

    private float moveSpeedActual = 1f;
    private bool appeared = false;
    private bool hasSwitched = false;

    private float f = 0;
    private bool hasLooped = false;
    private Rigidbody2D rb;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (isActivated)
        {
            leverActivated();
        }
    }

    public void leverActivated()
    {
        if (isMovingitem)
        {
            StartCoroutine(Moveplatform());
            //Debug.Log("Started Moveplatform");
        }
        //Debug.Log(isAppearingitem);
        if (isAppearingitem)
        {
            StartCoroutine(Appearplatform());
            //Debug.Log("Started Appearplatform");
        }
        if (isSwitchingitem)
        {
            StartCoroutine(Switchplatform());
        }
    }

    IEnumerator Switchplatform()
    {
        if (!hasSwitched)
        {
            GetComponent<SpriteRenderer>().sprite = State_2;
            hasSwitched = true;
        }
        else if (hasSwitched)
        {
            GetComponent<SpriteRenderer>().sprite = State_1;
            hasSwitched = false;
        }
        yield return null;
    }

    IEnumerator Appearplatform()
    {
        if (!Disappear)
        {
            GetComponent<SpriteRenderer>().enabled = appeared;
            GetComponent<Collider2D>().enabled = appeared;
            appeared = !appeared;
            yield return null;
        }
        else
        {
            GetComponent<SpriteRenderer>().enabled = appeared;
            GetComponent<Collider2D>().enabled = appeared;
            appeared = !appeared;
            yield return null;
        }
    }


    IEnumerator Moveplatform()
    {
        moveSpeedActual = 0.1f * moveSpeed;
        if(shouldLoop)
        {
            while(f < 1)
            {
                for(f = 0; f < 1; f += moveSpeedActual)
                {
                    transform.position = Vector3.Lerp(startPoint.position, endPoint.position, f);
                    yield return new WaitForSeconds(Time.fixedDeltaTime);
                }
                for(f = 0; f < 1 ; f += moveSpeedActual)
                {
                    transform.position = Vector3.Lerp(endPoint.position, startPoint.position, f);
                    yield return new WaitForSeconds(Time.fixedDeltaTime);
                }
                f = 0;
            }
        }
        if (!shouldLoop)
        {
            //Debug.Log("1");
            if (!hasLooped)
            {
                //Debug.Log("2");
                for (f = 0; f < 1; f += moveSpeedActual)
                {
                    //Debug.Log("f is"+ f);
                    transform.position = Vector3.Lerp(startPoint.position, endPoint.position, f);
                    yield return new WaitForSeconds(Time.fixedDeltaTime);
                }
                if (f < 1)
                {
                    hasLooped = true;
                    //Debug.Log("haslooped is" + hasLooped);
                }
            }
        }
    }
}
