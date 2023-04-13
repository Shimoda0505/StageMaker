using UnityEngine;
using System;



/*制作概要
 * 2023年3月制作
 * 北海道情報専門学校ゲームクリエイタ科
 * 下田祥己
 */
/// <summary>
/// 入力管理用スクリプト
/// </summary>
public class InputManager : MonoBehaviour
{
    //変数部--------------------------------------------------------------------------------------------------------------------------------
    #region 変数
    [Header("スクリプト関連")]
    private PlayerController _playerController;//プレイヤー管理用スクリプト
    private CameraController _cameraController;//カメラ管理用スクリプト


    [Header("デバック蘭連")]
    [SerializeField, Tooltip("デバックモード")] private bool _isPlayerDebugMode;
    /// <summary>
    /// ゲームのシステムモード
    /// </summary>
    private enum SystemMode
    {
        [InspectorName("通常モード")] NOMAL_MODE,
        [InspectorName("デバックモード")] DEBUG_MODE,
    }
    private SystemMode _systemMode = default;


    [Header("入力関連")]
    [SerializeField, Tooltip("デットゾーン"), Range(0.1f, 0.9f)] private float _deadZone;
    private string _h = "Horizontal";//入力名
    private string _v = "Vertical";//入力名


    [Header("Player蘭連")]
    private string _playerTag = "Player";//プレイヤーのtag名
    private Transform _playerTr = default;//プレイヤーのTransform


    [Header("Camera関連")]
    private string _camTag = "MainCamera";//カメラのtag名
    #endregion


    //処理部--------------------------------------------------------------------------------------------------------------------------------

    private void Start()
    {

        //コンポーネント取得
        GameObject playerObj = GameObject.FindGameObjectWithTag(_playerTag).gameObject;//Playerオブジェクト
        if (!playerObj) { print("playerObjが取得できません"); }
        else 
        {
            _playerController = playerObj.GetComponent<PlayerController>();//PlayerControllerスクリプト
            _playerTr = playerObj.transform;//プレイヤーのTransform
        }
        _cameraController = GameObject.FindGameObjectWithTag(_camTag).GetComponent<CameraController>();//CameraControllerのスクリプト
    }

    private void Update()
    {

        //PlayerControllerがNullなら処理しない
        if (!_playerController) { print("PlayerControllerが取得できません"); return; }

        //デバックモードの切り替え
        _systemMode = _isPlayerDebugMode ? SystemMode.DEBUG_MODE : SystemMode.NOMAL_MODE;

        switch (_systemMode)
        {
            case SystemMode.NOMAL_MODE:

                //プレイヤーの入力処理
                _playerController.PlayerInput(Mathf.Abs(Input.GetAxis(_h)) >= _deadZone, Input.GetKeyDown(KeyCode.W));
                break;


            case SystemMode.DEBUG_MODE:

                //プレイヤーのデバック処理
                _playerController.PlayerDebugMode(Input.GetAxis(_h), Input.GetAxis(_v));
                break;


            default: print("SystemModeのcaseがありません"); break;
        }
    }

    private void FixedUpdate()
    {

        //PlayerControllerがNullなら処理しない
        if (!_playerController) { print("PlayerControllerが取得できません"); return; }

        switch (_systemMode)
        {
            case SystemMode.NOMAL_MODE:

                //プレイヤーの移動処理
                _playerController.PlayerMove(Input.GetAxis(_h));
                break;


            case SystemMode.DEBUG_MODE: break;


            default: print("SystemModeのcaseがありません"); break;
        }
    }

    private void LateUpdate()
    {

        if (!_cameraController) { print("CameraControllerが取得できません"); return; }

        //カメラの移動処理
        _cameraController.CameraMove(_playerTr);
    }
}
