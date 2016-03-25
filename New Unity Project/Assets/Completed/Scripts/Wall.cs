using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour
{
    public Sprite dmgSprite;                    //攻撃された時に表示する内壁のスプライト画像
    public int hp = 1;                          //内壁の体力

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        //SpriteRendererをキャッシュしておく
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //プレイヤーが内壁を攻撃した時に実行されるメソッド
    //PlayerクラスのOnCantMoveから呼び出し
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

            //public変数で指定しておいた画像を表示
            spriteRenderer.sprite = dmgSprite;
        }
    }

}
