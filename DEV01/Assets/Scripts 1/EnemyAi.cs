using UnityEngine;

public class EnemyIA : MonoBehaviour
{
    [Header("Alvos e Componentes")]
    public Transform player;
    [SerializeField] private Animator anim;
    private Rigidbody2D rb;
    private GameObject attackArea;

    [Header("Combate")]
    public bool attacking = false;
    public float timeToAttackDuration = 0.25f; // Quanto tempo a hitbox fica ativa
    public float attackCooldown = 1.5f; // Tempo entre um ataque e outro
    private float attackTimer = 0f;
    private float cooldownTimer = 0f;
    private bool canAttack = true;

    [Header("Distâncias")]
    public float detectionDistance = 8f;
    public float attackDistance = 2f;

    [Header("Movimentação")]
    public float speed = 2f;
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    // Estados do Inimigo
    private enum EnemyState { Patrol, Chase, Attack }
    [SerializeField] private EnemyState currentState = EnemyState.Patrol;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Garante que pega o filho corretamente (verifica se existe)
        if (transform.childCount > 0)
            attackArea = transform.GetChild(0).gameObject;

        if (attackArea != null)
            attackArea.SetActive(false);
    }

    void Update()
    {
        // Se o player não foi definido, não faz nada para evitar erros
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // --- 1. Controle de Cooldown de Ataque ---
        if (!canAttack)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= attackCooldown)
            {
                canAttack = true;
                cooldownTimer = 7f;
            }
        }

        // --- 2. Controle da Duração do Ataque (Hitbox) ---
        if (attacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= timeToAttackDuration)
            {
                StopAttack(); // Desativa a hitbox
            }
        }

        // --- 3. Máquina de Estados ---
        // Só troca de estado se não estiver no meio da animação de ataque
        if (!attacking)
        {
            if (distance <= attackDistance)
                currentState = EnemyState.Attack;
            else if (distance <= detectionDistance)
                currentState = EnemyState.Chase;
            else
                currentState = EnemyState.Patrol;
        }

        // Execução dos comportamentos
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;

            case EnemyState.Chase:
                ChasePlayer();
                break;

            case EnemyState.Attack:
                AttackLogic();
                break;
        }
    }

    void Patrol()
    {
        // Se não houver pontos de patrulha, fica parado (Idle)
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        MoveTowards(targetPoint.position);

        if (Vector3.Distance(transform.position, targetPoint.position) < 1.5f)
        {
            // Vai para o próximo ponto (loop)
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    void ChasePlayer()
    {
        MoveTowards(player.position);
    }

    void AttackLogic()
    {
        // Para o inimigo para atacar
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("Walk", false);

        // Garante que ele olhe para o player antes de atacar
        LookAtTarget(player.position);

        if (canAttack && !attacking)
        {
            StartAttack();
        }
    }

    void StartAttack()
    {
        Debug.Log("Inimigo atacando!");
        attacking = true;
        canAttack = false;

        // Ativa hitbox
        if (attackArea != null) attackArea.SetActive(true);

        // Ativa animação (Trigger é melhor que Bool para ataques únicos)
        anim.SetTrigger("Attack");
    }

    void StopAttack()
    {
        attacking = false;
        attackTimer = 2f;
        if (attackArea != null) attackArea.SetActive(false);
    }

    void MoveTowards(Vector3 target)
    {
        // 1. Calcula a direção
        Vector2 direction = (target - transform.position).normalized;
        direction.y = 0;

        // 2. Move usando Rigidbody (Mantém física)
        // Nota: MovePosition é teleporte suave, não gera velocity automática para anims
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);

        // 3. Atualiza animação de andar
        anim.SetBool("Walk", true);
        anim.SetFloat("HorizontalAnimE", direction.x); // Usa a direção calculada, não a velocidade física

        // 4. Espelha o sprite
        LookAtTarget(target);
    }

    void LookAtTarget(Vector3 target)
    {
        // Se o alvo está à direita
        if (target.x > transform.position.x)
            transform.localScale = new Vector3(1f, 1f, 1f);
        // Se o alvo está à esquerda
        else if (target.x < transform.position.x)
            transform.localScale = new Vector3(-1f, 1f, 1f);
    }

    // Para visualizar as áreas no Editor da Unity
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}

