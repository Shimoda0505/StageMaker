using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



/*
 * 2023年3月制作
 * 北海道情報専門学校ゲームクリエイタ科
 * 下田祥己
 */
/// <summary>
/// 落下オブジェクトのギミック用スクリプト
/// </summary>
public class GimmickFallBox : MonoBehaviour
{

    [Header("オブジェクト関連")]
    [SerializeField, Tooltip("落とすオブジェクト")] private GameObject _fallObj;
    private BoxCollider2D _objCollider;//落とすオブジェクトのCollider
    private float _fallObjSize = 0;//落とすオブジェクトの変化サイズ
    private float _fallObjPos = default;
    private float _defaultSize = default;//落とすオブジェクトの初期サイズ
    private float _defaultPos = default;//落とすオブジェクトの初期位置

    [Header("速度関連")]
    [SerializeField, Tooltip("落下速度"), Range(0.1f, 5f)] private float _fallSpeed;
    [SerializeField, Tooltip("落とすオブジェクトの大小する速度"), Range(0.1f, 5f)] private float _changeSizeSpeed;


    [Header("時間関連")]
    [SerializeField, Tooltip("落とす間隔"), Range(0.1f, 5f)] private float _fallIntervalTime;
    private float _fallIntervalCount = 0;//落とす間隔の計測


    [Header("RayCast関連")]
    private const float _rayDistance = 0.55f;//長さ
    private string _stageName = "Stage";//Stageタグ名
    private int _stageLayerMask = default;//Stageレイヤーの番地


    /// <summary>
    /// 状態Enum
    /// </summary>
    private enum MotionEnum
    {
        [InspectorName("待機")]IDLE,
        [InspectorName("生成")]MAKE,
        [InspectorName("移動")]MOVE,
        [InspectorName("停止")]STOP,
    }
    private MotionEnum _motionEnum = MotionEnum.IDLE;



    //処理部--------------------------------------------------------------------------------------------------------------------------------

    private void Start()
    {
        //落とすオブジェクト
        _defaultSize = _fallObj.transform.localScale.x;//初期サイズを補完
        _defaultPos = _fallObj.transform.localPosition.y;//初期位置
        _fallObjPos = _defaultPos;//初期位置の複製
        _fallObj.transform.localScale = new Vector3(0, 0, 1);//サイズを初期化

        //Colliderの初期化
        _objCollider = _fallObj.GetComponent<BoxCollider2D>();//取得
        _objCollider.enabled = false;//非アクティブ

        //レイヤーの管理番号を取得しマスクへの変換
        _stageLayerMask = 1 << LayerMask.NameToLayer(_stageName);
    }

    private void Update()
    {

        switch (_motionEnum)
        {
            case MotionEnum.IDLE:

                //時間計測後生成開始
                _fallIntervalCount += Time.deltaTime;
                if(_fallIntervalCount >= _fallIntervalTime) { _fallIntervalCount = 0; _motionEnum = MotionEnum.MAKE; }
                break;


            case MotionEnum.MAKE:

                //サイズ更新
                _fallObjSize += Time.deltaTime * _changeSizeSpeed;
                //Colliderをアクティブ
                if(_fallObjSize >= _defaultSize)
                {

                    //サイズをdefaultに変更
                    _fallObjSize = _defaultSize;

                    //Colliderをアクティブ
                    _objCollider.enabled = true;
                    _motionEnum = MotionEnum.MOVE; 
                }
                break;


            case MotionEnum.MOVE:

                //落下
                _fallObjPos -= Time.deltaTime * _fallSpeed;

                //RayCastの接触判定の処理、trueで移動終了
                if (RayCast(_fallObj.transform.position)) { _motionEnum = MotionEnum.STOP; }
                break;


            case MotionEnum.STOP:

                //サイズ更新
                _fallObjSize -= Time.deltaTime * _changeSizeSpeed;
                if (_fallObjSize <= 0)
                {

                    //初期k化
                    _fallObjSize = 0;//サイズ
                    _fallObjPos = _defaultPos;//座標

                    //初期位置に移動
                    //Colliderを非アクティブ
                    _objCollider.enabled = false;
                    _motionEnum = MotionEnum.IDLE;
                }
                break;


            default:print("_motionEnumのcaseなし"); break;
        }

        //サイズ
        _fallObj.transform.localScale = new Vector3(_fallObjSize, _fallObjSize, 1);
        //座標
        _fallObj.transform.localPosition = new Vector3(_fallObj.transform.localPosition.x, _fallObjPos, _fallObj.transform.localPosition.z);
    }



    //メソッド部--------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// RayCastの接触判定の処理
    /// </summary>
    /// <param name="rayPos">RayCastの原点</param>
    /// <returns>地面の接触,周囲2方向以上で壁と接触</returns>
    private bool RayCast(Vector2 rayPos)
    {

        //RayCastを各方向に飛ばす
        RaycastHit2D hitDownRight = Physics2D.Raycast(rayPos, Vector2.down, _rayDistance, _stageLayerMask);
        //RayCastの可視化
        Debug.DrawRay(rayPos, Vector2.down * _rayDistance, Color.red);
        return (hitDownRight);
    }

}
