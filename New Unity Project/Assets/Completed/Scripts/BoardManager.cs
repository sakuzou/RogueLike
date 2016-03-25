using UnityEngine;
using System;
//Listを使うため
using System.Collections.Generic;
//Randomを使うため
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    //カウント用のクラスを設定
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    //8*8のゲームボードを作るので縦の段を8、横の列を8
    public int columns = 8;
    public int rows = 8;
    //壁は5〜9の間で出現
    public Count wallCount = new Count(5, 9);
    //アイテムは1〜5の間で出現
    public Count foodCount = new Count(1, 5);
    //Exitは単体
    public GameObject exit;
    //床・内壁・アイテム・敵キャラ・外壁は複数あるため配列
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;
    //オブジェクトの位置情報を保存する変数
    private Transform boardHolder;
    //オブジェクトを配置できる範囲を表すリスト
    //Listは可変型の配列
    private List<Vector3> gridPositions = new List<Vector3>();

    //敵キャラ・アイテム・内壁を配置できる範囲を決定
    void InitialiseList()
    {
        //gridPositionをクリア
        gridPositions.Clear();
        //gridPositionにオブジェクト配置可能範囲を指定
        //x = 1〜6をループ
        for (int x = 1; x < columns - 1; x++)
        {
            //y = 1〜6をループ
            for (int y = 1; y < rows - 1; y++)
            {
                //6*6の範囲をgridPositionsに指定
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }
    //外壁、床を配置
    void BoardSetup()
    {
        //Boardというオブジェクトを作成し、transform情報をboardHolderに保存
        boardHolder = new GameObject("Board").transform;
        //x = -1〜8をループ
        for (int x = -1; x < columns + 1; x++)
        {
            //y = -1〜8をループ
            for (int y = -1; y < rows + 1; y++)
            {
                //床をランダムで選択
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                //左端or右端or最低部or最上部の時＝外壁を作る時
                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    //floorTileの時と同じように外壁をランダムで選択し、上書きする
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }
                //床or外壁を生成し、instance変数に格納
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f),Quaternion.identity) as GameObject;
                //生成したinstanceをBoardオブジェクトの子オブジェクトとする
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);     //0〜36からランダムで1つ決定し、位置情報を確定
        Vector3 randomPosition = gridPositions[randomIndex];

        gridPositions.RemoveAt(randomIndex);        //ランダムで決定した数値は削除

        return randomPosition;      //確定した位置情報を返す
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        //最低値〜最大値+1のランダム回数分だけループ
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            //gridPositionから位置情報を１つ取得
            Vector3 randomPosition = RandomPosition();
            //引数tileArrayからランダムで1つ選択
            GameObject tileChoise = tileArray[Random.Range(0, tileArray.Length)];
            //ランダムで決定した種類・位置でオブジェクトを生成
            Instantiate(tileChoise, randomPosition, Quaternion.identity);
        }
    }
    //オブジェクトを配置していくメソッド
    //このクラス内唯一のpublicメソッド 床を生成するタイミングでGameManagerから呼ばれる
    public void SetupScene(int level)
    {
        
        BoardSetup();       //床と外壁を配置
        InitialiseList();       //敵キャラ・内壁・アイテムを配置できる位置を決定
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);  //
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);  //内壁・アイテム・敵キャラをランダムで配置
        
        int enemyCount = (int)Mathf.Log(level, 2f);         //Mathf.Log : 対数で計算。level=2なら4、level=3なら8
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0F), Quaternion.identity); //Exitを7, 7の位置に配置する。
    }
}