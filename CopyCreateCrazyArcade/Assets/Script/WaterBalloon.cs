using Assets.Script;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Script
{
    public class WaterBalloon : MonoBehaviour
    {
        private float elapsedTime;
        private Collider2D _collider;
        private Rigidbody2D _rigidbody;
        private const float CLEAR_TIME = 3f;
        public int currentPower = 1;

        public Explosion explosionPrefeb;
        public LayerMask NonDestroyLayer;
        public LayerMask destoryLayer;

        private const float clearTime = 0.36f;

        Collider2D[] target = new Collider2D[2];
        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _rigidbody = GetComponent<Rigidbody2D>();
        }


        void Update()
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= CLEAR_TIME)
            {
                BoomBalloon();
            }

        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {

            if (collision.gameObject.CompareTag("Explosion"))
            {
                // 시간 전에 물줄기에 풍선이 맞았을때
                BoomBalloon();
            }

        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                _collider.isTrigger = false;
            }
        }

        void BoomBalloon()
        {
            // 센터 이미지

            Explosion explod = Instantiate(explosionPrefeb, transform.position, transform.rotation);
            Animator anim = explod.GetComponent<Animator>();
            anim.SetBool("CenterExplosion", true);
            explod.DestroyAfter(clearTime);

            Destroy(gameObject);

            CrossExplodCreate();

            elapsedTime = 0;
        }

        // 진행할 방향
        void CrossExplodCreate()
        {
            Explode(transform.position, Vector2.up, currentPower);
            Explode(transform.position, Vector2.down, currentPower);
            Explode(transform.position, Vector2.left, currentPower);
            Explode(transform.position, Vector2.right, currentPower);
        }
        private void Explode(Vector2 position, Vector2 direction, int length)
        {
            if (length <= 0)
            {
                return;
            }

            //포지션으로부터의 방향 측정.
            position += direction;

            // 부술 수 없는 오브젝트 충돌시 중단
            if (Physics2D.OverlapBox(position, Vector2.one / 2f, 0f, NonDestroyLayer))
            {
                return;
            }

           

            // 부술 수 있는 오브젝트.
            if (target[0] = Physics2D.OverlapBox(position, Vector2.one / 2f, 0f, destoryLayer))
            { 
                // 오버랩 박스로 블록인경우 해당 블록삭제
                if (target[0].gameObject.layer == LayerMask.NameToLayer("Block"))
                {
                    Animator _anim = target[0].GetComponent<Animator>();

                    _anim.SetBool("BlockDestroy", true);
                    Destroy(target[0].gameObject, clearTime);
                    return;
                }
                // 블록이 아니면 지나쳐서 물줄기 생성 후 삭제
                if (target[0].gameObject.layer == LayerMask.NameToLayer("Item"))
                {
                    Destroy(target[0].gameObject);
                }
            
            }
           
            //물줄기 추가생성
            Explosion explosion = Instantiate(explosionPrefeb, position, transform.rotation);
            explosion.SetDirection(direction);

            explosion.DestroyAfter(clearTime);
            if (length == 1)
            {
                Animator _anim = explosion.GetComponent<Animator>();
                _anim.SetBool("MiddleExplosion", true);

            }
            // 최대 길이에서 재귀적으로 최대길이를 줄여나가는것.
            Explode(position, direction, length - 1);
        }


    }
}
