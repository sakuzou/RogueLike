using UnityEngine;
using System.Collections;
//MovingObjectを継承
public class Enemy : MovingObject
{

    public int playerDamage = 10; //プレイヤーへのダメージ量

    private Animator animator;
    private Transform target;   //プレイヤーの位置情報
    private bool skipMove;      //敵キャラが動くかどうかの判定

    private int hp = 3;   //敵の体力

    //MovingObjectのStartメソッドを継承
    protected override void Start()
    {
        //GameManagerスクリプトのEnemyの配列に格納
        GameManager.instance.AddEnemyToList(this);
        //Animatorをキャッシュしておく
        animator = GetComponent<Animator>();
        //Playerの位置情報を取得
        target = GameObject.FindGameObjectWithTag("Player").transform;
        //MovingObjectのStartメソッド呼び出し
        base.Start();
    }
    //
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }

        base.AttemptMove<T>(xDir, yDir);
        //移動が終了したらtrueにする
        skipMove = true;
    }
    //敵キャラ移動用メソッド　GameManagerから呼ばれる
    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;
        
        //同じX軸上にいるとき
        //Mathf.Abs: 絶対値をとる。-1なら1となる。
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            //プレイヤーが上にいれば+1、下に入れば-1する
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }
        //同じY軸上にいるとき
        else if (Mathf.Abs(target.position.y - transform.position.y) < float.Epsilon)
        {
            //プレイヤーが右にいれば+1、左にいれば-1する
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }
        //斜め方向にいるとき
        else
        {
            xDir = target.position.x > transform.position.x ? 1 : -1;
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }


        //ジェネリック機能　攻撃対象はPlayerのみなので、型引数はPlayer
        AttemptMove<Player>(xDir, yDir);

        /*歩行アニメーション用*/
        if (xDir != 0 || yDir != 0)
        {
            animator.SetFloat("Direction_X", xDir);
            animator.SetFloat("Direction_Y", yDir);
        }

    }
    //MovingObjectの抽象メソッドのため必ず必要
    protected override void OnCantMove<T>(T component)
    {
        //Playerクラスを取得
        Player hitPlayer = component as Player;

        //animator.SetTrigger("enemyAttack"); //攻撃アニメーション実行

        //PlayerクラスのLoseFoodメソッドを呼び出す　引数はダメージ量
        hitPlayer.LoseLife(playerDamage);
    }

    //敵キャラに攻撃が当たったときの処理
    public void LoseLife(int loss)
    {
        //animator.SetTrigger("EnemyHit");
        hp -= loss;
        if (hp < 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void DamageWall(int loss)
    {
        //体力を引数分だけ減らす
        hp -= loss;

        //ダメージをくらったときに点滅する


        //体力が0以下になった時
        if (hp <= 0)
        {
            //レイヤーを変更して(二度と)当たり判定が発生しないようにする
            gameObject.layer = LayerMask.NameToLayer("Default");


        }
    }
}