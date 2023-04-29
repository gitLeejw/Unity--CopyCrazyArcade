using Assets.Script;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class BossMonster : MonoBehaviour
{
    private int bossHP = 10;
    private Vector3[,] moves = new Vector3[3, 4];
    private Vector3 saveTransformPosition;
    private Vector2 attackDirection;

    private int movesRandomPositionX = 0;
    private int movesRandomPositionY = 0;
    private int savePositionX;
    private int savePositionY;
    private int randomPattern;
    private int explosionLength = 5;
    private float moveSpeed;
    private float speed = 2f;

    private Animator _anim;
    private Collider2D _collider;

    public WaterBalloon _Balloon;

    public Explosion _Explosion;
    private Vector3[] explodePosition = new Vector3[4];

    IEnumerator state;
    IEnumerator AttackCoroutine;
    private WaitForSeconds attackCoolTime = new WaitForSeconds(0.5f);
    private WaitForSeconds stateCoolTime = new WaitForSeconds(3);

    private bool waitDie;
    private bool IsOnMove = true;
    private bool IsOnAttack = false;
    private bool _isHit = false;

    private void Awake()
    {

        _anim = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();

        explodePosition[0] = new Vector3(3f, -3.5f, 0);
        explodePosition[1] = new Vector3(3, -4.5f, 0);
        explodePosition[2] = new Vector3(3f, 3.5f, 0);
        explodePosition[3] = new Vector3(3f, 2.5f, 0);


        moves[0, 0] = new Vector3(-8, 6.5f, 0);
        moves[0, 1] = new Vector3(-4, 6.5f, 0);
        moves[0, 2] = new Vector3(0, 6.5f, 0);
        moves[0, 3] = new Vector3(4, 6.5f, 0);

        moves[1, 0] = new Vector3(-8, 2.5f, 0);
        moves[1, 1] = new Vector3(-4, 2.5f, 0);
        moves[1, 2] = new Vector3(0, 2.5f, 0);
        moves[1, 3] = new Vector3(4, 2.5f, 0);

        moves[2, 0] = new Vector3(-8, -2.5f, 0);
        moves[2, 1] = new Vector3(-4, -2.5f, 0);
        moves[2, 2] = new Vector3(0, -2.5f, 0);
        moves[2, 3] = new Vector3(4, -2.5f, 0);

        randomPattern = Random.Range(0, 3);

    }

    private void Update()
    {
        moveSpeed = speed * Time.deltaTime;

        if (transform.position == moves[movesRandomPositionY, movesRandomPositionX])
        {
            if (IsOnAttack)
            {
                state = StateHelper();
                StartCoroutine(state);
            }
        }
        if (IsOnMove)
        {
            BossMovement();
        }

        _isHit = false;
    }


    IEnumerator StateHelper()
    {
        saveTransformPosition = -saveTransformPosition;


        IsOnMove = false;
        randomPattern = Random.Range(0, 2);
        SetAttackDirection();
        _anim.SetFloat("PositionX", attackDirection.x);
        _anim.SetFloat("PositionY", attackDirection.y);



        IsOnAttack = false;

        SetRandomPosition();

        yield return stateCoolTime;

        IsOnMove = true;
    }

    private void PhaseUpdate()
    {
        speed += 2;
    }
    private void SetAttackDirection()
    {
        if (transform.position == moves[1, 1])
        {
            int i = Random.Range(0, 2);

            if (i == 0)
            {
                AttackCoroutine = Attack(Vector2.right);
                StartCoroutine(AttackCoroutine);
            }
            else
            {
                BoxExplosionAttack();
            }
        }

        if (transform.position == moves[1, 2])
        {
            int i = Random.Range(0, 2);
            if (i == 0)
            {
                AttackCoroutine = Attack(Vector2.left);
                StartCoroutine(AttackCoroutine);
            }
            else
            {
                BoxExplosionAttack();
            }
        }
        if (transform.position == moves[0, 0] || transform.position == moves[0, 1])
        {
            int i = Random.Range(0, 2);
            if (i == 0)
            {
                AttackCoroutine = Attack(Vector2.right);
                StartCoroutine(AttackCoroutine);
            }
            else
            {
                AttackCoroutine = Attack(Vector2.down);
                StartCoroutine(AttackCoroutine);

            }
        }

        if (transform.position == moves[0, 2] || transform.position == moves[0, 3])
        {
            int i = Random.Range(0, 2);
            if (i == 0)
            {
                AttackCoroutine = Attack(Vector2.left);
                StartCoroutine(AttackCoroutine);
            }
            else
            {
                AttackCoroutine = Attack(Vector2.down);
                StartCoroutine(AttackCoroutine);

            }
        }
        if (transform.position == moves[1, 0])
        {
            AttackCoroutine = Attack(Vector2.right);
            StartCoroutine(AttackCoroutine);
        }

        if (transform.position == moves[1, 3])
        {
            AttackCoroutine = Attack(Vector2.left);
            StartCoroutine(AttackCoroutine);
        }

        if (transform.position == moves[2, 0] || transform.position == moves[2, 1])
        {
            int i = Random.Range(0, 2);
            if (i == 0)
            {
                AttackCoroutine = Attack(Vector2.up);
                StartCoroutine(AttackCoroutine);
            }
            else
            {
                AttackCoroutine = Attack(Vector2.right);
                StartCoroutine(AttackCoroutine);
            }
        }

        if (transform.position == moves[2, 2] || transform.position == moves[2, 3])
        {
            int i = Random.Range(0, 2);
            if (i == 0)
            {
                AttackCoroutine = Attack(Vector2.left);
                StartCoroutine(AttackCoroutine);
            }
            else
            {
                AttackCoroutine = Attack(Vector2.up);
                StartCoroutine(AttackCoroutine);
            }
        }
    }
    private void SetRandomPosition()
    {
        if (randomPattern == 0)
        {
            movesRandomPositionX = Random.Range(0, 4);
            if (savePositionX == movesRandomPositionX)
            {
                movesRandomPositionX = Random.Range(0, 4);
            }

        }

        if (randomPattern == 1)
        {
            movesRandomPositionY = Random.Range(0, 3);

            if (savePositionY == movesRandomPositionY)
            {
                movesRandomPositionY = Random.Range(0, 3);

            }

        }
    }
    private void BossMovement()
    {
        savePositionX = movesRandomPositionX;
        savePositionY = movesRandomPositionY;

        saveTransformPosition = transform.position;


        saveTransformPosition = moves[movesRandomPositionY, movesRandomPositionX] - saveTransformPosition;
        _anim.SetFloat("PositionX", saveTransformPosition.x);
        _anim.SetFloat("PositionY", saveTransformPosition.y);

        transform.position = Vector3.MoveTowards(transform.position, moves[movesRandomPositionY, movesRandomPositionX], moveSpeed);

        IsOnAttack = true;


    }

    private void BoxExplosionAttack()
    {

        Explode(transform.GetChild(0).position + explodePosition[0], Vector2.up, explosionLength + 2);
        Explode(transform.GetChild(0).position - explodePosition[1], Vector2.down, explosionLength + 2);
        Explode(transform.GetChild(0).position + explodePosition[2], Vector2.left, explosionLength);
        Explode(transform.GetChild(0).position - explodePosition[3], Vector2.right, explosionLength);
    }
    private void Explode(Vector2 position, Vector2 direction, int length)
    {
        if (length <= 0)
        {
            return;
        }

        //포지션으로부터의 방향 측정.
        position += direction;


        //물줄기 추가생성
        Explosion explosion = Instantiate(_Explosion, position, transform.rotation);
        explosion.SetDirection(direction);

        Animator _anim = explosion.GetComponent<Animator>();
        _anim.SetBool("CenterExplosion", true);

        // 최대 길이에서 재귀적으로 최대길이를 줄여나가는것.
        Explode(position, direction, length - 1);
    }
    IEnumerator Attack(Vector2 direction)
    {
        CanAttack(direction);
        yield return attackCoolTime;
        CanAttack(direction);
        yield return attackCoolTime;
        CanAttack(direction);

    }

    private void OnDamage()
    {
        _anim.SetBool("Hit", true);
        _collider.enabled = false;

        transform.GetChild(1).GetChild(2).GetChild(bossHP - 1).gameObject.SetActive(false);
        --bossHP;
        if (bossHP == 3)
            PhaseUpdate();

        if (bossHP == 8)
        {
            
            IsBossWaitDie();
        }
    }
    private void IsBossWaitDie()
    {
        _collider.enabled = true;

        IsOnMove = false;
        IsOnAttack= false;
        StopCoroutine(state);
        StopCoroutine(AttackCoroutine);

        waitDie = true;

        _anim.SetTrigger("Die");
    }

    private void IsBossDieConfirm()
    {
        _anim.SetTrigger("DieConfirm");
    }

    private void SetFalse()
    {
        gameObject.SetActive(false);
    }

    private void OffDamage()
    {
        _collider.enabled = true;
        _anim.SetBool("Hit", false);
    }
    private Vector2 CanAttack(Vector2 direction)
    {

        WaterBalloon balloon = Instantiate(_Balloon, transform.GetChild(0).position, Quaternion.identity);
        balloon.currentPower = 3;
        balloon.tag = "BossBalloon";
        balloon._collider.isTrigger = false;
        balloon._rigidbody.constraints = RigidbodyConstraints2D.None;
        balloon._rigidbody.velocity = direction * 10;
        return attackDirection = direction;
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Explosion") && _isHit == false)
        {
            if (bossHP > 0)
            {
                OnDamage();
                _isHit = true;
            }
        }

        if (collision.CompareTag("Balloon"))
        {
            WaterBalloon balloon = collision.GetComponent<WaterBalloon>();
            balloon.BoomBalloon();
        }

        if (collision.CompareTag("Player"))
        {
            if (waitDie)
            {
                IsBossDieConfirm();
            }
            else
            {
                PlayerStatus status = collision.gameObject.GetComponent<PlayerStatus>();
                status.DieConfirmation();
            }
        }
    }
}
