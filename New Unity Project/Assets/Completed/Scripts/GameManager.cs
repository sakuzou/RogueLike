using UnityEngine;
using System.Collections;
using System.Collections.Generic; //Listを使う時に宣言
using UnityEngine.UI;   //UI用に宣言
using System;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 0.5f;          //レベル表示画面で2秒待つ
    public float turnDelay = .1f;               //Enemyの動作時間(0.1秒)
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints = 100;          //プレイヤーの体力

    //HideInInspector: public変数だけどInspectorで編集させない
    //プレイヤーの順番か判定
    [HideInInspector]
    public bool playersTurn = true;

    private Text levelText; //レベルテキスト
    private GameObject levelImage; //レベルイメージ
    private int level = 10; //レベルは1にしておく
    private bool doingSetup; //levelImageの表示等で活用

    private List<Enemy> enemies;    //Enemyクラスの配列
    private bool enemiesMoving;     //Enemyのターン中true
      
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
    private void OnLevelWasLoaded(int index)
    {
        level++; //レベルを1プラスする
        InitGame();
    }

    void InitGame()
    {
        //trueの間、プレイヤーは身動きを取れない
        doingSetup = true;
        //LevelImageオブジェクト・LevelTextオブジェクトの取得
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level; //最新のレベルに更新
        levelImage.SetActive(true); //LebelImageをアクティブにし表示
        Invoke("HideLevelImage", levelStartDelay); //2秒後にメソッド呼び出し

        enemies.Clear(); //EnemyのList(配列)を初期化
        boardScript.SetupScene(level);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);    //LevelImage非アクティブ化
        doingSetup = false;             //プレイヤーが動けるようになる
    }

    public void GameOver()
    {
        //ゲームオーバーメッセージを表示
        levelText.text = "After " + level + " days, you starved.";
        levelImage.SetActive(true);

        //GameManagerを無効にする
        enabled = false;
    }

    void Update()
    {
        //プレイヤーのターンかEnemyが動いた後ならUpdateしない
        if (playersTurn || enemiesMoving || doingSetup){
            return;
        }

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