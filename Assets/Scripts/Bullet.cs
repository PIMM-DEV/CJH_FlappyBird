using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    AudioSource boom;

    void Awake()
    {
        boom = GetComponent<AudioSource>();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "DeadZone" && !(collision.gameObject.name == "ground"))
        {
            collision.gameObject.SetActive(false);
            boom.Play();
            transform.position = Vector3.right * 1000; 
            StartCoroutine(Reset(collision));
        }
    }

    IEnumerator Reset(Collider2D collision)
    {
        yield return new WaitForSeconds((1 / GameManager.Instance.difficulty * 2));
        collision.gameObject.SetActive(true);
        Destroy(this);
    }
}
