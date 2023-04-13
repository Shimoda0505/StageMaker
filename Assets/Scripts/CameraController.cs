using UnityEngine;



/*制作概要
 * 2023年3月制作
 * 北海道情報専門学校ゲームクリエイタ科
 * 下田祥己
 */
/// <summary>
/// カメラ管理用スクリプト
/// </summary>
public class CameraController : MonoBehaviour
{
    //変数部--------------------------------------------------------------------------------------------------------------------------------
    [Header("Camera蘭連")]
    [SerializeField, Tooltip("追従速度"), Range(1, 10)] private int _camSpeed;
    private Vector3 _offSetPos = new Vector3(0, 0, -10);//オフセット座標



    //メソッド部--------------------------------------------------------------------------------------------------------------------------------
    //パブリックメソッド--------------------------------------------------------------------
    /*[ InputManagerで使用 ]*/
    /// <summary>
    /// カメラの移動
    /// </summary>
    /// <param name="playerTr">プレイヤーのTransform</param>
    public void CameraMove(Transform playerTr)
    {
        //カメラの座標をプレイヤーに追従
        this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, playerTr.position + _offSetPos, Time.deltaTime * _camSpeed);
    }
}
