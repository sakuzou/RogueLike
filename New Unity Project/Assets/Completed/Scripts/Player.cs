using UnityEngine;
using System.Collections;
//MovingObjectクラスを継承する
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{

    public int wallDamage = 1;              //壁へのダメージ量
    public int pointsPerFood = 10;          //フードの回復量
    public int pointsPerSoda = 20;          //ソーダの回復量
    public float restartlevelDelay = 0.1f;  //次レベルへ行く時の時間差
    public Text foodText;                   //FoodText
    private Animator m_Anim;    //歩く方向用

    private Animator animator; //PlayerChop, PlayerHit用
    private int life; //プレイヤーの体力

    //MovingObjectのStartメソッドを継承　baseで呼び出し
    protected override void Start()
    {
        //Animatorをキャッシュしておく
        animator = GetComponent<Animator>();
        m_Anim = GetComponent<Animator>();
        //シングルトンであるGameManagerのplayerFoodPointsを使うことに
        //よって、レベルを跨いでも値を保持しておける
        life = GameManager.instance.playerFoodPoints;
        foodText.text = "Food: " + life;
        //MovingObjectのStartメソッド呼び出し
        base.Start();
    }

    //Playerスクリプトが無効になる前に、体力をGameManagerへ保存
    //UnityのAPIメソッド(Unityに標準で用意された機能)
    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = life;
    }

    void Update()
    {
        //プレイヤーの順番じゃない時Updateは実行しない
        if (!GameManager.instance.playersTurn)
            return;

        int xDir = 0;   //-1: 左移動, 1: 右移動
        int yDir = 0;   //-1: 下移動, 1: 上移動

        xDir = (int)Input.GetAxisRaw("Horizontal");
        yDir = (int)Input.GetAxisRaw("Vertical");

        
        //上下左右どれかに移動する時
        if (xDir != 0 || yDir != 0)
        {
            //方向転換キーを押しているとき
            if (Input.GetKey(KeyCode.C) == true)
            {
                /*歩行アニメーション用*/
                m_Anim.SetFloat("Direction_X", xDir);
                m_Anim.SetFloat("Direction_Y", yDir);

                base.DirectionChange(xDir, yDir);   //方向転換(MovingObject)                
            }
            else
            {
                /*歩行アニメーション用*/
                m_Anim.SetFloat("Direction_X", xDir);
                m_Anim.SetFloat("Direction_Y", yDir);

                //Wall: ジェネリックパラメーター<T>に渡す型引数
                //Playerの場合はWall以外判定する必要はない
                AttemptMove<Wall>(xDir, yDir);
            }
        }

        //攻撃
        if (Input.GetKeyDown(KeyCode.Z))
        {
            base.Attack<Wall>();   //攻撃モーション再生(MovingObject)

            GameManager.instance.playersTurn = false;   //プレイヤーのターン終了
        }

        
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        //移動1回につき1ポイント失う
        life--;
        foodText.text = "食べ物: " + life;
        //MovingObjectのAttemptMove呼び出し
        base.AttemptMove<T>(xDir, yDir);

       //RaycastHit2D hit;

        CheckIfGameOver();
        //プレイヤーの順番終了
        GameManager.instance.playersTurn = false;
    }

    //MovingObjectの抽象メソッドのため必ず必要
    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;       //Wall型を定義 Wallスクリプトを表す
        hitWall.DamageWall(wallDamage);         //WallスクリプトのDamageWallメソッド呼び出し
        //animator.SetTrigger("PlayerChop");    //Wallに攻撃するアニメーションを実行
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            Invoke("Restart", restartlevelDelay);   //Invoke: 引数分遅れてメソッドを実行する
            enabled = false;                        //Playerを無効にする
        }
        else if (other.tag == "Food")
        {
            
            life += pointsPerFood;                                      //体力を回復しotherオブジェクトを削除
            foodText.text = "+" + pointsPerFood + " Food: " + life;     //
            other.gameObject.SetActive(false);                          //
        }
        else if (other.tag == "Soda")
        {
            life += pointsPerSoda;                                      //体力を回復しotherオブジェクトを削除
            foodText.text = "+" + pointsPerSoda + " Food: " + life;     //
            other.gameObject.SetActive(false);                          //
        }
    }

    private void Restart()
    {
        //同じシーンを読み込む
        SceneManager.LoadScene("Main");
        //Application.LoadLevel("Application.loadedLevelName");

    }
    //敵キャラがプレイヤーを攻撃した時のメソッド
    public void LoseLife(int loss)
    {
        //animator.SetTrigger("PlayerHit");
        life -= loss;
        foodText.text = "-" + loss + " Food: " + life;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (life <= 0)
        {
            //GameManagerのGameOverメソッド実行
            //public staticな変数なのでこのような簡単な形でメソッドを呼び出せる
            GameManager.instance.GameOver();
        }
    }
}