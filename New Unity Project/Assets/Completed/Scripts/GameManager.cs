using UnityEngine;
using System.Collections;
using System.Collections.Generic; //Listを使う時に宣言
using UnityEngine.UI;   //UI用に宣言
using System;

public class GameManager : MonoBehaviour
{
    public float NextStartDelay = 0.5f;          //レベル表示画面で2秒待つ
    public float turnDelay = .1f;               //Enemyの動作時間(0.1秒)
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints = 100;          //プレイヤーの体力
    
    //HideInInspector: public変数だけどInspectorで編集させない
    //プレイヤーの順番か判定
    [HideInInspector]
    public bool playersTurn = true;

    private Text NextText;          //レベルテキスト
    private GameObject NextImage;   //レベルイメージ
    private int floor = 10;         //レベルは1にしておく
    private bool doingSetup;        //NextImageの表示等で活用

    private List<Enemy> enemies;    //Enemyクラスの配列
    private bool enemiesMoving;     //Enemyのターン中true

    private Text FloorText;         //階層を表示するテキスト
      
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        //Enemyを格納する配列の作成
        enemies = new List<Enemy>();

        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    //UnityのAPIで、Sceneが呼ばれる度に実行されるメソッド
    private void OnNextWasLoaded(int index)
    {
        floor++; //レベルを1プラスする
        InitGame();
    }

    void InitGame()
    {
        //trueの間、プレイヤーは身動きを取れない
        doingSetup = true;
        //NextImageオブジェクト・NextTextオブジェクトの取得
        NextImage = GameObject.Find("NextImage");
        NextText = GameObject.Find("NextText").GetComponent<Text>();
        NextText.text = "第" + floor + "階層"; //最新のレベルに更新
        NextImage.SetActive(true); //LebelImageをアクティブにし表示
        Invoke("HideNextImage", NextStartDelay); //2秒後にメソッド呼び出し

        FloorText = GameObject.Find("FloorText").GetComponent<Text>();

        enemies.Clear(); //EnemyのList(配列)を初期化
        boardScript.SetupScene(floor);
    }

    private void HideNextImage()
    {
        NextImage.SetActive(false);    //NextImage非アクティブ化
        doingSetup = false;             //プレイヤーが動けるようになる
    }

    public void GameOver()
    {
        //ゲームオーバーメッセージを表示
        NextText.text =  floor + " 階まで到達, あなたは死にました.";
        NextImage.SetActive(true);

        //GameManagerを無効にする
        enabled = false;
    }

    void Update()
    {
        //プレイヤーのターンかEnemyが動いた後ならUpdateしない
        if (playersTurn || enemiesMoving || doingSetup){
            return;
        }
       
        FloorText.text = ""+floor;

        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    //
    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }
        //Enemyの数だけEnemyスクリプトのMoveEnemyを実行
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;
    }
}