using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rigidBody2D;

    public float moveSpeed;
    private Transform target;

    public float damage;
    //敌人攻击间隔
    public float hitWaitTime = 1f;
    private float hitCounter;

    //初始生命值
    public float health = 5f;

    //敌人朝向
    private Vector3 PositiveVectorOne = new Vector3(1, 1, 1);
    private Vector3 NegativeVectorOne = new Vector3(-1, 1, 1);

    //被击退时间
    public float knockBackTime = .5f;
    private float knockBackCounter;

    //初始掉落经验值
    public int expToGive = 1;

    //初始掉落金币价值
    public int coinValue = 1;
    public float coinDropRate = .5f;

    private void Start()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();

        target = FindObjectOfType<PlayerController>().transform;
    }

    private void Update()
    {
        //如果玩家生命值不为0
        if (PlayerController.instance.gameObject.activeSelf == true)
        {
            //朝向目标位置移动
            Vector3 direction = target.position - transform.position;
            //改变朝向
            if (direction.x < 0)
            {
                transform.localScale = PositiveVectorOne;
            }
            else
            {
                transform.localScale = NegativeVectorOne;
            }

            ///如果需要被击退
            if (knockBackCounter > 0)
            {
           
                knockBackCounter -= Time.deltaTime;
     
                if (moveSpeed > 0)
                {
                    ///反向并减速
                    moveSpeed = -moveSpeed * 2f;
                }
                //击退结束后恢复速度朝向和大小
                if (knockBackCounter <= 0)
                {
                    moveSpeed = Mathf.Abs(moveSpeed * .5f);
                }
            }

            //设定敌人移动速度
            rigidBody2D.velocity = (target.position - transform.position).normalized * moveSpeed;
            //攻击间隔
            if (hitCounter > 0f)
            {
                hitCounter -= Time.deltaTime;
            }
        }
        //如果玩家生命值为0, 停止敌人活动
        else
        {
            rigidBody2D.velocity = Vector2.zero;
        }
    }

    //添加碰撞
    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        //检测为玩家, 并且计时器触发时候产生伤害
        if (collision.gameObject.tag == "Player" && hitCounter <= 0f)
        {
            PlayerHealthController.instance.TakeDamage(damage);
            //重置计时器的值
            hitCounter = hitWaitTime;
        }
    }

    public void TakeDamage(float damageToTake)
    {
        health -= damageToTake;

        if (health <= 0)
        {
            Destroy(gameObject);
            //掉落经验
            ExperienceLevelController.instance.SpawnExp(transform.position, expToGive);

            //如果几率合适, 掉落金币
            if (Random.value < coinDropRate)
            {
                CoinController.instance.DropCoin(transform.position, coinValue);
            }

            //死亡时发出声音
            SFXController.instance.PlaySFXPitched(1);

        }

        //在收到伤害时显示数字
        DamageNumberController.instance.SpawnDamage(damageToTake, transform.position);
    }

    //判断是否被击退
    public void TakeDamage(float damageTotake, bool shouldKnockback)
    {
        TakeDamage(damageTotake);

        if (shouldKnockback == true)
        {
            knockBackCounter = knockBackTime;
        }
    }
}
