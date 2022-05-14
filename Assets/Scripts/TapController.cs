using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TapController : MonoBehaviour
{
    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float tapForce = 10;
    public float tiltSmooth = 5;
    public Vector3 startPos;

    public AudioSource tapAudio;
    public AudioSource scoreAudio;
    public AudioSource dieAudio;

    Rigidbody2D rigid;
    Quaternion downRotation;
    Quaternion forwardRotation;

    GameManager game;
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0, 0, -90);
        forwardRotation = Quaternion.Euler(0, 0, 35);
        game = GameManager.Instance;
        rigid.simulated = false;
    }

    void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameStarted()
    {
        rigid.velocity = Vector2.zero;
        rigid.simulated = true;
    }

    void OnGameOverConfirmed()
    {
        transform.localPosition = startPos;
        transform.rotation = Quaternion.identity;
    }
    // Update is called once per frame
    void Update()
    {
        if (game.GameOver)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            tapAudio.Play();
            transform.rotation = forwardRotation;
            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.up * tapForce, ForceMode2D.Force);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "ScoreZone")
        {
            OnPlayerScored();
            scoreAudio.Play();
            if(collision.gameObject.name == "ScoreItem")
            {
                collision.gameObject.SetActive(false);
            }
        }
        if(collision.gameObject.tag == "DeadZone")
        {
            rigid.simulated = false;
            OnPlayerDied();
            dieAudio.Play();
        }
    }
}
