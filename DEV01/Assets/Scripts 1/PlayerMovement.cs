using System.Security.Cryptography;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpforce = 5f;
    [SerializeField] public Animator anim;
    private bool isJumping;
    private float direcao;
    private Rigidbody2D rb;

    public float KBForce;
    public float KBCount;
    public float KBTime;

    public bool isKonock;

    public AudioSource audios;
    public AudioClip[] sounds;


    private GameObject attackArea = default;
    public bool attacking = false;
    private float timeToAttack = 0.25f;
    private float timer = 0f;

     void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isJumping = false;
       attackArea = transform.GetChild(0).gameObject;
    }

    private void Atack()
    {
           anim.SetTrigger("Atack");
           attacking = true;
           attackArea.SetActive(attacking);
        audios.clip = sounds[0];
        audios.Play();
    }

    void MoveAnim()
    {
        anim.SetFloat("HorizontalAnim", rb.linearVelocity.x);
    }

    void JumpAnim()
    {
        anim.SetFloat("VerticalAnim", rb.linearVelocity.y);
        anim.SetBool("groundCheck", isJumping);
      
    }

     void Update ()
    {
        
        KonockLogic();
        MoveAnim();
        JumpAnim();
        if(Input.GetMouseButtonDown(0))
        {
            Atack();
            
        }
        if(attacking)
        {
            timer += Time.deltaTime;

            if(timer >= timeToAttack)
            {
                timer = 0f;
                attacking = false;
                attackArea.SetActive(attacking);
            }
        }
    }

    public void MoveLogic()
    {

       direcao = Input.GetAxisRaw("Horizontal");
        

        rb.linearVelocity = new Vector2(direcao *  speed, rb.linearVelocity.y);

        if (direcao > 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }

        else if (direcao < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        if (Input.GetButtonDown("Jump") && isJumping == false)
        {
            rb.linearVelocityY = jumpforce;
            isJumping = true;
            audios.clip = sounds[1];
            audios.Play();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Chão")) 
        {
            isJumping = false;
        }

    }

    void KonockLogic()
    {
        if(KBCount < 0)
        {
            MoveLogic();
        }
        else
        {
            if (isKonock == true)
            {
                rb.linearVelocity = new Vector3(-KBForce, KBForce);
            }
            if (isKonock == false)
            {
                rb.linearVelocity = new Vector3(KBForce, KBForce);
            }
        }
        KBCount -= Time.deltaTime;
    }
}
