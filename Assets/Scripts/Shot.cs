using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Shot : MonoBehaviour
{
    public float maxShotDelay;
    public float curShotDelay;

    public GameObject bulletPrefab;
    public Vector3 bulletGroup;

    public Image Boom;
    public Image Image;
    // Update is called once per frame
    void Update()
    {
        Fire();
        Reload();
        ReloadImage();
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    void ReloadImage()
    {
        Boom.fillAmount = Mathf.Min(1, curShotDelay / maxShotDelay);
        Image.fillAmount = Mathf.Min(1, curShotDelay / maxShotDelay);
    }

    void Fire()
    {
        if (curShotDelay < maxShotDelay)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            rigid.AddForce(Vector2.right * 200, ForceMode2D.Force);

            curShotDelay = 0;
        }
    } 

    public void ClickShot()
    {
        if (curShotDelay < maxShotDelay)
            return;

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.AddForce(Vector2.right * 200, ForceMode2D.Force);

        curShotDelay = 0;
    }
}
