using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public bool resetable = true;
    public bool isPulled;
    public List<GameObject> itemList;
    public LayerMask canInteract = -1;
    public Sprite leverOn;
    public Sprite leverOff;

    private Collider2D _collider;

    private bool collided = false;

    void Start()
    {
        renderLever();
    }

    private void renderLever()
    {
        if (isPulled)
            GetComponent<SpriteRenderer>().sprite = leverOn;
        else
            GetComponent<SpriteRenderer>().sprite = leverOff;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _collider = collision;
        if (!collided)
        {
            if(_collider.gameObject.layer == 11)
            {
                Debug.Log("detected collision");
                StartCoroutine(TellPlatform());
                collided = true;
            }
        }
    }
    IEnumerator TellPlatform()
    {
        Debug.Log("Isinlayermask, tellingplatform" + _collider.gameObject.name);
        isPulled = !isPulled;
        renderLever();
        foreach (GameObject platform in itemList)
        {
            platform.GetComponent<Platform_Manager>().leverActivated();
        }
        yield return new WaitForSeconds(.5f);
        if (!resetable)
        {
            collided = true;
        }
        else
        {
            collided = false;
        }
    }
}
