using UnityEngine;



/*制作概要
 * 2023年3月制作
 * 北海道情報専門学校ゲームクリエイタ科
 * 下田祥己
 */
/// <summary>
/// プレイヤー管理用スクリプト
/// </summary>
public class PlayerController : MonoBehaviour
{
    //変数部--------------------------------------------------------------------------------------------------------------------------------
    #region 変数
    [Header("スクリプト蘭連")]
    private string _gameControllerName = "GameController";//GameControllerタグ名
    private StageMaker _stageMaker;//ステージ生成スクリプト

    [Header("オブジェクト関連")]
    private SpriteRenderer _playerEyeRenderer = default;//プレイヤーの目のSpriteRenderer
    private string _playerEyeTag = "PlayerEye";//PlayerEyeタグ名
    private string _obstacleTag = "Obstacle";//障害物タグ名


    [Header("移動蘭連")]
    [SerializeField, Tooltip("移動速度"), Range(1, 50)] private int _moveSpeed;
    [SerializeField, Tooltip("落下速度"), Range(1, 10)] private int _fallSpeed;
    [SerializeField, Tooltip("上昇速度"), Range(1, 10)] private int _jumpSpeed;
    [SerializeField, Tooltip("1クリックで、何マス上昇するか"), Range(1, 10)] private int _jumpTrout;
    private Transform _playerTr = default;//プレイヤーのTransform
    private Vector3 _playerMovePos = default;//プレイヤーの移動座標
    private float _startPos = default;//ゲーム開始座標(x軸)
    private float _playerJumpPos = default;//ジャンプ前のPlayerのY軸を補完
    private bool _isFall = false;//落下開始
    private (bool right, bool left) _isWall = (false, false);//壁の衝突


    [Header("RayCast関連")]
    private const float _rayDistance = 0.5f;//長さ
    private const float _rayOffSetPos = 0.45f;//加算位置


    /// <summary>
    /// プレイヤーの行動enim
    /// </summary>
    private enum PlayerMotion
    {
        [InspectorName("停止")] IDLE,
        [InspectorName("移動")] MOVE,
        [InspectorName("上昇")] JUMP,
        [InspectorName("落下")] FALL,
        [InspectorName("死亡")] DEATH,
    }
    private PlayerMotion _playerMotion = default;
    #endregion



    //処理部--------------------------------------------------------------------------------------------------------------------------------
    private void Start()
    {
        //コンポーネント取得
        _playerEyeRenderer = GameObject.FindGameObjectWithTag(_playerEyeTag).GetComponent<SpriteRenderer>();//プレイヤーの目のSpriteRenderer
        _stageMaker = GameObject.FindGameObjectWithTag(_gameControllerName).GetComponent<StageMaker>();//ステージ生成スクリプトの取得

        //座標
        _playerTr = this.gameObject.transform;//プレイヤーのTransform
        _playerMovePos = _playerTr.position;//プレイヤーの移動座標
        _startPos = _playerTr.transform.position.x;//開始座標(X軸)の補完

        //プレイヤーの動きに合わせて、ステージの生成と描画の表示を自動で行う
        _stageMaker.AutoCreateStage(_startPos, _playerTr.transform.position.x);
    }



    //メソッド部--------------------------------------------------------------------------------------------------------------------------------
    //パブリックメソッド---------------------------------------------------------------------------------------
    /*[ InputManagerで使用,通常操作 ]*/
    /// <summary>
    /// Playerの入力判定の処理
    /// </summary>
    /// <param name="isInputH">左右入力</param>
    /// <param name="isInputUp">上入力</param>
    public void PlayerInput(bool isInputH, bool isInputUp)
    {

        /*[ Nullチェック関連 ]*/
        if (!_playerTr) { print("Playerの座標データが保管されていません"); return; }


        /*[ RayCast関連 ]*/
        //RayCastの接触判定の処理
        (bool isGround, bool isCelling,  bool isHide, bool isObstacle) = PlayerRayCast(_playerTr.position);
        //プレイヤーの行動enumの変更
        if (isObstacle) { _playerMotion = PlayerMotion.DEATH; }//障害物との接触があれば死亡enum
        //Playerが隠れている時の処理
        HideMotion(isHide);


        /*[ 移動入力関連 ]*/
        //プレイヤーの動きに合わせて、ステージの生成と描画の表示を自動で行う
        _stageMaker.AutoCreateStage(_startPos, _playerTr.transform.position.x);

        //プレイヤーの行動enim
        switch (_playerMotion)
        {

            case PlayerMotion.IDLE:

                //プレイヤーの行動enumの変更
                if (isInputH) { _playerMotion = PlayerMotion.MOVE; }//移動enum
                if (isInputUp && isGround) { _playerMotion = PlayerMotion.JUMP; }//ジャンプenum
                if (!isGround) { _playerMotion = PlayerMotion.FALL; }//落下enum
                break;


            case PlayerMotion.MOVE:

                //プレイヤーの行動enumの変更
                if (!isInputH) { _playerMotion = PlayerMotion.IDLE; }//停止enum
                if (isInputUp && isGround) { _playerMotion = PlayerMotion.JUMP; }//ジャンプenum
                if (!isGround) { _playerMotion = PlayerMotion.FALL; }//落下enum
                break;


            case PlayerMotion.JUMP:

                //プレイヤーの行動enumの変更
                if(_isFall || isCelling) { _playerMotion = PlayerMotion.FALL; _isFall = false; }//落下enum
                break;


            case PlayerMotion.FALL:

                //プレイヤーの行動enumの変更
                if (isGround) { _playerMotion = PlayerMotion.IDLE; }//停止enum
                break;


            case PlayerMotion.DEATH: break; 


            default: Debug.Log("PlayerMotion(enum)のcaseなし"); break;
        }
    }
    /// <summary>
    /// Playerの移動処理
    /// </summary>
    /// <param name="fInputH">左右入力</param>
    public void PlayerMove(float fInputH)
    {

        /*[ Nullチェック関連 ]*/
        if (!_playerTr) { print("Playerの座標データが保管されていません"); return; }


        /*[ 移動処理関連 ]*/
        //プレイヤーの行動enim
        switch (_playerMotion)
        {

            case PlayerMotion.IDLE:

                //PlayerのY軸
                _playerMovePos.y = Mathf.Round(_playerMovePos.y);//四捨五入しint座標に変換
                _playerJumpPos = _playerMovePos.y + _jumpTrout;//ジャンプ時の座標計算

                //PlayerのX軸
                if(_isWall.left || _isWall.right) { _playerMovePos.x = Mathf.Round(_playerMovePos.x); }//四捨五入しint座標に変換
                break;


            case PlayerMotion.MOVE:

                //PlayerのY軸
                _playerMovePos.y = Mathf.Round(_playerMovePos.y);//四捨五入しint座標に変換
                _playerJumpPos = _playerMovePos.y + _jumpTrout;//ジャンプ時の座標計算

                //PlayerのX軸
                if (_isWall.right && fInputH > 0) { break; }//右移動ができない時に右入力があれば処理しない
                else if (_isWall.left && fInputH < 0) { break; }//左移動ができない時に左入力があれば処理しない
                if (fInputH != 0) { _playerMovePos.x += Time.deltaTime * _moveSpeed * Mathf.Sign(fInputH); } //移動
                break;


            case PlayerMotion.JUMP:

                //PlayerのY軸
                _playerMovePos.y += Time.deltaTime * _jumpSpeed;//上昇
                if(_playerMovePos.y >= _playerJumpPos) { _isFall = true; }

                //PlayerのX軸
                if (_isWall.right && fInputH > 0) { break; }//右移動ができない時に右入力があれば処理しない
                else if (_isWall.left && fInputH < 0) { break; }//左移動ができない時に左入力があれば処理しない
                if (fInputH != 0) { _playerMovePos.x += Time.deltaTime * _moveSpeed * Mathf.Sign(fInputH); } //移動
                break;


            case PlayerMotion.FALL:

                //時間ごとにPlayerのY軸を減算
                _playerMovePos.y -= Time.deltaTime * _fallSpeed;

                //PlayerのX軸
                if (_isWall.right && fInputH > 0) { break; }//右移動ができない時に右入力があれば処理しない
                else if (_isWall.left && fInputH < 0) { break; }//左移動ができない時に左入力があれば処理しない
                if (fInputH != 0) { _playerMovePos.x += Time.deltaTime * _moveSpeed * Mathf.Sign(fInputH); } //移動
                break;


            case PlayerMotion.DEATH:

                print("死亡");
                break;


            default: Debug.Log("PlayerMotion(enum)のcaseなし"); break;
        }

        //プレイヤーの移動更新
        _playerTr.position = _playerMovePos;
    }

    /*[ InputManagerで使用,デバック操作 ]*/
    /// <summary>
    /// プレイヤーのデバックモード
    /// </summary>
    /// <param name="fInputH">左右入力</param>
    /// <param name="fInputV">上下入力</param>
    public void PlayerDebugMode(float fInputH, float fInputV)
    {

        //入力処理
        if(Mathf.Abs(fInputH) >= 0.1f) { _playerMovePos.x += Time.deltaTime * _moveSpeed * Mathf.Sign(fInputH); }
        if(Mathf.Abs(fInputV) >= 0.1f) { _playerMovePos.y += Time.deltaTime * _moveSpeed * Mathf.Sign(fInputV); }

        //プレイヤーの移動更新
        _playerTr.position = _playerMovePos;

        //プレイヤーの動きに合わせて、ステージの生成と描画の表示を自動で行う
        _stageMaker.AutoCreateStage(_startPos, _playerTr.transform.position.x);
    }


    //プライベートメソッド---------------------------------------------------------------------------------------
    /*[ RayCast ]*/
    /// <summary>
    /// RayCastの接触方向を判別
    /// </summary>
    /// <param name="rayPos">RayCastの原点</param>
    /// <breaks>地面の接触,天井の接触,周囲2方向以上で壁と接触</breaks>
    private (bool isGround, bool isCeiling, bool isHide, bool isObstacle)  PlayerRayCast(Vector2 rayPos)
    {

        //RayCastの接触判定を取得
        (bool isHit, bool isObstacle) isRight = HitRayCast(true, rayPos, Vector2.right);//右壁の接触
        (bool isHit, bool isObstacle) isLeft = HitRayCast(true, rayPos, Vector2.left);//左壁の接触
        (bool isHit, bool isObstacle) isGround = HitRayCast(false, rayPos, Vector2.down);//地面の接触
        (bool isHit, bool isObstacle) isCeiling = HitRayCast(false, rayPos, Vector2.up);//天井の接触

        //Raycastの接触判定を評価し補完
        bool isHide = isGround.isHit && (isRight.isHit || isLeft.isHit) ? true : false;//周囲2方向以上で壁と接触
        bool isObstacle = isRight.isObstacle || isLeft.isObstacle || isGround.isObstacle || isCeiling.isObstacle ? true : false;//障害物と接触したかどうか
        _isWall = (isRight.isHit, isLeft.isHit);//左右の接触判定を補完

        //値の返却
        return (isGround.isHit, isCeiling.isHit, isHide, isObstacle);
    }
    /// <summary>
    /// RayCastの接触判定の処理
    /// </summary>
    /// <param name="HorizontalOrVertical">縦横どちらにとばすか</param>
    /// <param name="rayPos">原点</param>
    /// <param name="throwDirection">どの方向に飛ばすか</param>
    /// <returns>RayCastが接触したか</returns>
    private (bool isHit, bool isObstacle) HitRayCast(bool HorizontalOrVertical, Vector2 rayPos, Vector2 throwDirection)
    {
        //横と縦どちらに飛ばすかにより、オフセット位置を変更
        Vector2 rayOffSetPos = HorizontalOrVertical ? new Vector2(0, _rayOffSetPos) : new Vector2(_rayOffSetPos, 0);

        //RayCastを各方向に飛ばす
        RaycastHit2D hitPlus = Physics2D.Raycast(rayPos + rayOffSetPos, throwDirection, _rayDistance);//プラス位置
        RaycastHit2D hitMinus = Physics2D.Raycast(rayPos - rayOffSetPos, throwDirection, _rayDistance);//マイナス位置

        //RayCastの可視化
        Debug.DrawRay(rayPos + rayOffSetPos, throwDirection * _rayDistance, Color.red);//プラス位置
        Debug.DrawRay(rayPos - rayOffSetPos, throwDirection * _rayDistance, Color.red);//マイナス位置

        //Raycastの接触判定を評価し返却
        bool isHit = hitPlus || hitMinus ? true : false;
        //接触したオブジェクトが障害物の場合死亡フラグtrue
        bool isObstacle = false;
        if (hitPlus) { isObstacle = hitPlus.collider.tag == _obstacleTag ? true : false; }//プラス位置
        if (hitMinus) { isObstacle = hitMinus.collider.tag == _obstacleTag ? true : false; }//マイナス位置
        return (isHit, isObstacle);
    }

    /*[ プレイヤーモデルのアニメーション ]*/
    /// <summary>
    /// Playerが隠れている時(周囲2方向に壁or床)の処理
    /// </summary>
    /// <param name="isHide">isHideフラグ</param>
    private void HideMotion(bool isHide)
    {

        //Nullの場合処理しない
        if (_playerEyeRenderer == null) { return; }

        if (isHide) { _playerEyeRenderer.color = Color.black; }
        else if (!isHide) { _playerEyeRenderer.color = Color.white; }
    }
}
