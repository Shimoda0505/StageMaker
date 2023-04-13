using System.Collections.Generic;
using UnityEngine;
using System;



/* 制作概要
 * 
 * ーー制作者・制作時期ーー
 * 下田祥己
 * 北海道情報専門学校ゲームクリエイタ科
 * 2023年4月制作
 * 
 * 
 * ーーー作品概要ーーー
 * シャープなステージやフラットなステージどちらも作りたい。敵とギミックのどちらを沢山出そう？
 * いつも同じステージは嫌だけど、自分で作るのはめんどくさい。でも、ちゃんとクリアできるステージじゃないとダメだ...
 * StageMakerは、そんな願望を誰でも簡単に叶えられる機能を兼ね備えています。
 * 
 * 
 * ーーー設計思想ーーー
 * ・ステージのパラメーター調整に自由度を持たせつつ、ステージとしてクリアできるものが自動生成される設計
 * ・同じステージが生成されない設計
 * ・オブジェクトの配置のみを行い、使用するオブジェクトにいかなるスクリプトや振る舞いを設計しても、ステージ生成に支障をきたさない設計

 * 
 * ーー設置時の設定方法ーー
 * ①ObjDatasに好きなオブジェクトをセットし、オブジェクト毎にパラメーターを入れてください
 * ②ステージ関連のパラメーターは、生成されるステージの床部分を変更できます
 * ③ギミック・敵関連のパラメーターは、ギミック・敵の生成割合を変更できます
 * ④ステージの描画関連のパラメーターは、ステージ全体の描画の表示・非表示を変更できます
 * ⑤スクリプトで呼び出す際は、生成のみならCreateStage()を、
 * 生成と描画をオブジェクトの位置に同期して生成・表示するならAutoCreateStage()を呼び出して下さい　
 * 
 * 
 * ーーーー生成手順ーーーー
 * ①生成オブジェクトの配列(ObjDatas)番地を配列(_objNumberDatas)に補完
 * ②配列(_stageDatas)の両端に床(_objNumberDatas)番地を設定(抽選)
 * ③配列(_stageDatas)の両端以外に床(_objNumberDatas)番地を設定(抽選)
 * ④配列(_stageDatas)にギミック・敵(_objNumberDatas)番地を設定(抽選)
 * ⑤配列(_stageDatas)に設定されている配列(_objNumberDatas)番地から各オブジェクトを生成
 * ⑥生成したステージを1つのパッケージにして配列(_packageStageDatas)に補完して終了 <summary>
 * 
 * 
 * ーーーー抽選要素ーーーー
 * 下記内容にインスペクター上で幅を設定し、抽選を行っています
 * ●床
 * 　・設置されるオブジェクト
 * 　・段差の高さ
 * 　・フラットの長さ
 * ●ギミック・敵
 * 　・どちらを生成するか
 * 　・連続して生成するか
 * 　・地面と空中どちらに生成するか
 * 
 * 
 * ーーーその他概要ーーーー
 * ・デバックメソッドは、配列(_objNumberDatas)の中身を可視化したものです。消しても問題ありません
 * 
 */
///<summary>
/// ステージ生成用スクリプト
/// </summary>
public class StageMaker : MonoBehaviour
{
    //変数部--------------------------------------------------------------------------------------------------------------------------------
    #region 変数
    [Header("オブジェクト関連")]
    //オブジェクトデータ
    [SerializeField, Tooltip("生成オブジェクトの管理データ")] public List<ObjDatas> _objDatas = new List<ObjDatas>();
    [Serializable, Tooltip("ステージを構成するオブジェクトのデータ群")]
    public class ObjDatas
    {
        [SerializeField, Tooltip("オブジェクト名")] private string _name;
        [SerializeField, Tooltip("生成オブジェクト")] public GameObject _createObj;
        [SerializeField, Tooltip("同時に生成するマス数の最大値"), Range(1, 20)] public int _createMaxRange;
        [SerializeField, Tooltip("生成時の地面からの高さ"), Range(0, 10)] public int _airObstaclesHeight;

        /// <summary>
        /// オブジェクトの分類
        /// </summary>
        public enum ObjectClassEnum
        {
            [InspectorName("床")] FLOOR,
            [InspectorName("ギミック")] GIMMICK,
            [InspectorName("敵")] ENEMY,
        }
        [SerializeField, Tooltip("オブジェクトの分類")] public ObjectClassEnum _objectClassEnum;
    }
    //オブジェクト番地(int型の配列はdefault値1の為、+1をしてenum番地に合わせて調整)
    private const int _NULL_NUMBER = 0;//空白番号
    private const int _FLOOR_NUMBER_X = (int)ObjDatas.ObjectClassEnum.FLOOR + 1;//床番号
    private const int _GIMMICK_NUMBER_X = (int)ObjDatas.ObjectClassEnum.GIMMICK + 1;//ギミック番号
    private const int _ENEMY_NUMBER_X = (int)ObjDatas.ObjectClassEnum.ENEMY + 1;//敵番号
    //オブジェクトの種類数
    private const int _OBJ_TYPE_COUNT = 4;//オブジェクトの種類数(空白,床,ギミック,敵)
    private int _floorCount = default;//床の種類数
    private int _gimmickCount = default;//ギミックの種類数
    private int _enemyCount = default;//敵の種類数
    //ソート後の_airObstaclesHeight > 0 の配列番号
    private int _gimmickAirNumber = default;//ギミックの空中の配列開始番号
    private int _enemyAirNumber = default;//敵の空中の配列開始番号
    //全オブジェクトデータ配列
    private int[,] _objNumberDatas = default;
    private string _stagePackageName = "StagePackage";//オブジェクト管理用の空オブジェクト名


    [Header("ステージ関連")]
    //フラグ
    [SerializeField, Tooltip("1マス生成を繰り返すかどうか")] private bool _isOneCellLoop;
    private bool _isLastOneCell = false;//前回1マス生成をしたか
    //ステージサイズ
    [SerializeField, Tooltip("ステージサイズ(X軸)"), Range(1, 100)] private int _stageSizeX;
    [SerializeField, Tooltip("ステージサイズ(Y軸)"), Range(1, 50)] private int _stageSizeY;
    [SerializeField, Tooltip("床下のサイズ"), Range(1, 50)] private int _underfloorSize;
    //配列
    private (int x, int y)[,] _stageDatas;//ステージデータ
    //ステージオブジェクトを配置する幅
    [SerializeField, Tooltip("段差の高さ"), Range(0, 10)] private int _floorStepHeight;
    [SerializeField, Tooltip("床の最大高さ"), Range(1, 50)] private int _floorMaxHeight;
    //生成したステージを1オブジェクトとして補完
    private List<GameObject> _packageStageDatas = new List<GameObject>();
    private enum UseCollider
    {
        [InspectorName("全てに適用")] _ALL,
        [InspectorName("外面にのみ適用")] _OUTER,
        [InspectorName("適用しない")] _NULL,
    }
    [SerializeField, Tooltip("BoxCollider2Dを床に適用する範囲")] private UseCollider _useCollider = default;




    [Header("ギミック・敵関連")]
    //障害物の生成割合
    [SerializeField, Tooltip("何割の割合で障害物を生成するか"), Range(0, 10)] private int _createRate;
    private static (int min, int max) __createRateClimb = (1, 10);
    //ギミック・敵の生成割合
    [SerializeField, Tooltip("ギミック・敵の生成割合"), Range(0, 10)] private int _gimmickOrEnemy;
    private static (int min, int max) __createObstacleClimb = (1, 10);
    //地上・空中の生成割合
    [SerializeField, Tooltip("地面・空中の生成割合"), Range(0, 10)] private int _GroundOrAir;
    private static (int min, int max) __createFieldClimb = (1, 10);
    //Enum
    /// <summary>
    /// 生成する障害物
    /// </summary>
    private enum CreateObstacleEnum
    {
        [InspectorName("ギミック・敵なし")] _NULL,
        [InspectorName("両方")] _BOTH,
        [InspectorName("ギミックのみ")] _GIMMICK_ONLY,
        [InspectorName("敵のみ")] _ENEMY_ONLY,
    }
    private CreateObstacleEnum _createObstacleEnum = default;
    /// <summary>
    /// 生成する高さ(ギミック)
    /// </summary>
    private enum CreateGimmickHeightEnum
    {
        [InspectorName("ギミックなし")] _NULL,
        [InspectorName("両方")] _BOTH,
        [InspectorName("高さ0のみ")] _GROUND_ONLY,
        [InspectorName("高さ1以上のみ")] _AIR_ONLY,
    }
    private CreateGimmickHeightEnum _createGimmickHeightEnum = default;
    /// <summary>
    /// 生成する高さ(敵)
    /// </summary>
    private enum CreateEnemyHeightEnum
    {
        [InspectorName("敵なし")] _NULL,
        [InspectorName("両方")] _BOTH,
        [InspectorName("高さ0のみ")] _GROUND_ONLY,
        [InspectorName("高さ1以上のみ")] _AIR_ONLY,
    }
    private CreateEnemyHeightEnum _createEnemyHeightEnum = default;


    [Header("ステージの描画関連")]
    [SerializeField, Tooltip("ステージの描画を消すかどうか")] private bool _isDrawingStage;
    [SerializeField, Tooltip("ステージの描画する範囲(1であれば、現在の地点+1までの範囲は描画)"), Range(1, 10)] private int _drawingClamp;
    private int _createPosOffSet = default;//生成座標のオフセット
    private float _stageCreatePos = 0;//ステージを更新するプレイヤー座標(x軸)
    private float _nowPackageStagePos = 0;//現在いるステージパッケージのプレイヤー座標(x軸)
    private int _nowPackageStageNumber = -1;//現在いるステージパッケージの配列番号(-1番地から開始することで、更新時に加算すると0番地から開始できる)
    #endregion



    //処理部--------------------------------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        //初期設定
        StartSetting();

        //配列内の整理
        ArraySort();

        //オブジェクト管理の配列を描画
        DebugArray();
    }



    //メソッド部----------------------------------------------------------------------------------------------------------------------------
    //パブリックメソッド--------------------------------------------------------------------
    /*[ 設置したいタイミングで自由に呼び出し ]*/
    /// <summary>
    /// ステージの生成
    /// 生成のみをしたい場合こちらを設置してください
    /// </summary>
    /// <param name="createPos">生成する座標</param>
    public void CreateStage((float x, float y) createPos)
    {
        try
        {
            #region 配列の初期設定
            int[] floorHeights = new int[_stageSizeX];//_stageDatas.x毎の床高さ
            for (int i = 0; i < floorHeights.Length; i++) { floorHeights[i] = 0; }//floorHeightsの初期化
            (int x, int y) floorNumber = (_FLOOR_NUMBER_X, 0);//今回設置する床の種類番地
            for (int i = 0; i < _stageDatas.GetLength(0); i++)
            {
                for (int j = 0; j < _stageDatas.GetLength(1); j++) { _stageDatas[i, j] = (_NULL_NUMBER, _NULL_NUMBER); }
            }//_stageDatasの初期化
            #endregion


            #region 両端の床を設置
            //床の番号を抽選
            floorNumber.y = UnityEngine.Random.Range(0, _floorCount);//抽選
            floorNumber = (floorNumber.x, floorNumber.y);//更新

            //壁の両端高さを抽選
            int floorHeight = UnityEngine.Random.Range(0, _floorStepHeight);
            //0番地(X軸)と最終番地(X軸)に床番号を設定
            for (int j = 0; j <= floorHeight; j++) { _stageDatas[0, j] = floorNumber; }//左端
            for (int j = 0; j <= floorHeight; j++) { _stageDatas[_stageDatas.GetLength(0) - 1, j] = floorNumber; }//右端

            //床高さを格納
            floorHeights[0] = floorHeight;
            floorHeights[floorHeights.Length - 1] = floorHeight;
            #endregion


            #region 床を設置
            for (int i = 1; i < _stageDatas.GetLength(0) - 1;)
            {
                /*[ 床の高さを設定 ]*/
                //前回の床高さから減算した値が0以下なら、最小値を前回の床高さに設定
                int floorHeightRandamX = floorHeight - _floorStepHeight < 0 ? floorHeight : floorHeight - _floorStepHeight;
                //床の最大高さ以上なら、最大値を床の最大高さに設定
                int floorHeightRandamY = floorHeight + _floorStepHeight >= _floorMaxHeight ? _floorMaxHeight : floorHeight + _floorStepHeight;
                //床の高さを前回の高さ+-から抽選
                floorHeight = UnityEngine.Random.Range(floorHeightRandamX, floorHeightRandamY);


                /*[ 床の幅を設定 ]*/
                //ランダム抽選の最小値の初期化
                int randomMin = 1;
                //1マス生成を繰り返さない設定なら
                if (!_isOneCellLoop) { randomMin = _isLastOneCell == true ? 2 : randomMin; }
                //床の平面になる長さを抽選
                int floorWidthRange = UnityEngine.Random.Range(randomMin, _objDatas[_objNumberDatas[floorNumber.x, floorNumber.y]]._createMaxRange);
                //抽選値が1ならtrueを設定
                _isLastOneCell = floorWidthRange == 1 ? true : false;
                //床の長さが_stageDatas.xの最大値以上なら、床の長さを_stageDatas.xに設定
                int floorWidth = i + floorWidthRange <= _stageDatas.GetLength(0) - 1 ? i + floorWidthRange : _stageDatas.GetLength(0) - 1;


                /*[ 上記2項目(floorWidth,floorHeight)分のマスを床に設定 ]*/
                //前回の設定番号~+floorWidth分の幅
                for (int x = i; x < floorWidth; x++)
                {
                    //1~floorHeight分の高さ
                    for (int y = 0; y <= floorHeight; y++) { _stageDatas[x, y] = floorNumber; }

                    //床高さを格納
                    floorHeights[x] = floorHeight;
                }
                //床に設定した分、iを加算
                i += floorWidthRange;
            }
            #endregion


            #region ギミックと敵を設置
            //最後の床高さを設定
            int lastFloorHeight = floorHeights[0];
            //床高さの変わった最初の配列番号
            int firstChangeNumber = 0;

            //格納されている床の高さ分処理
            for (int i = 0; i < floorHeights.Length; i++)
            {

                //設置できるギミック・敵がない場合処理終了
                if (_createObstacleEnum == CreateObstacleEnum._NULL) { break; }

                //床の高さが変わったら
                if (lastFloorHeight != floorHeights[i])
                {
                    for (int j = firstChangeNumber; j < i;)
                    {
                        //設置するオブジェクトの配列番号の番地
                        (int x, int y) objNumber = default;


                        /*[ 設置するオブジェクトの種類を抽選 ]*/
                        //ギミックと敵どちらを生成するか抽選(0 = ギミック, 1 = 敵)
                        //どちらのオブジェクトもあるなら
                        if (_createObstacleEnum == CreateObstacleEnum._BOTH)
                        {
                            objNumber.x = UnityEngine.Random.Range(__createObstacleClimb.min, __createObstacleClimb.max);//抽選
                            objNumber.x = objNumber.x >= _gimmickOrEnemy ? 0 : 1;//重み付けから抽選番号を0,1変換
                        }
                        //片方のオブジェクトしかない場合、値を固定化
                        else { objNumber.x = _createObstacleEnum == CreateObstacleEnum._GIMMICK_ONLY ? 0 : 1; }
                        //設置するオブジェクトを配列番号に変換
                        (objNumber.x, objNumber.y) = objNumber.x == 0 ? (_GIMMICK_NUMBER_X, _gimmickCount) : (_ENEMY_NUMBER_X, _enemyCount);


                        /*[ 設置するオブジェクトの生成座標を抽選 ]*/
                        //地上と空中どちらに生成するか抽選(0 = 地上, 1 = 空中)
                        //探索する配列の範囲を初期化
                        (int min, int max) groundOrAirClimp = default;
                        //設置するオブジェクトの種類がギミックなら
                        if (objNumber.x == _GIMMICK_NUMBER_X)
                        {
                            //どちらのフィードもあるなら
                            if (_createGimmickHeightEnum == CreateGimmickHeightEnum._BOTH)
                            {
                                groundOrAirClimp.min = UnityEngine.Random.Range(__createFieldClimb.min, __createFieldClimb.max);//抽選
                                groundOrAirClimp.min = groundOrAirClimp.min >= _GroundOrAir ? 0 : 1;//重み付けから抽選番号を0,1変換
                            }
                            //片方のフィードしかない場合、値を固定化
                            else { groundOrAirClimp.min = _createGimmickHeightEnum == CreateGimmickHeightEnum._GROUND_ONLY ? 0 : 1; }
                            //探索する配列の範囲を設定
                            //地上(0~空中配列の開始番地),空中(空中配列の開始番地~配列のLength)
                            groundOrAirClimp = groundOrAirClimp.min == 0 ? (0, _gimmickAirNumber) : (_gimmickAirNumber, objNumber.y);
                        }
                        //設置するオブジェクトの種類が敵なら
                        else if (objNumber.x == _ENEMY_NUMBER_X)
                        {
                            //どちらのフィードもあるなら
                            if (_createEnemyHeightEnum == CreateEnemyHeightEnum._BOTH)
                            {
                                groundOrAirClimp.min = UnityEngine.Random.Range(__createFieldClimb.min, __createFieldClimb.max);//抽選
                                groundOrAirClimp.min = groundOrAirClimp.min >= _GroundOrAir ? 0 : 1;//重み付けから抽選番号を0,1変換
                            }
                            //片方のフィードしかない場合、値を固定化
                            else { groundOrAirClimp.min = _createEnemyHeightEnum == CreateEnemyHeightEnum._GROUND_ONLY ? 0 : 1; }
                            //探索する配列の範囲を設定
                            //地上(0~空中配列の開始番地),空中(空中配列の開始番地~配列のLength)
                            groundOrAirClimp = groundOrAirClimp.min == 0 ? (0, _enemyAirNumber) : (_enemyAirNumber, objNumber.y);
                        }
                        else { print("設置するオブジェクトの生成座標がありません"); }


                        /*[ 設置するオブジェクトを抽選 ]*/
                        //gimmickOrEnemyで決定された種類からどのオブジェクトを設置するか抽選
                        objNumber.y = UnityEngine.Random.Range(groundOrAirClimp.min, groundOrAirClimp.max);


                        /*[ 設置するオブジェクトを確定 ]*/
                        //設置するオブジェクト
                        objNumber = (objNumber.x, objNumber.y);//設置するオブジェクトの配列番号
                        ObjDatas objDatas = _objDatas[_objNumberDatas[objNumber.x, objNumber.y]];//設置するオブジェクトのデータ
                        int objHeight = objDatas._airObstaclesHeight;//設置するオブジェクトの生成する高さを設定
                        int createCell = UnityEngine.Random.Range(1, objDatas._createMaxRange);//設置するオブジェクトを何マス生成するか抽選
                        createCell = createCell <= i - j ? createCell : i - j;//生成マス数が生成範囲を超えているなら、生成範囲に生成マス数を変更


                        /*[ 抽選したオブジェクトを配置するか抽選 ]*/
                        //生成するか抽選(0 = する, 1 =　しない)
                        int createRate = UnityEngine.Random.Range(__createRateClimb.min, __createRateClimb.max);//抽選
                        createRate = createRate <= _createRate ? 0 : 1;//重み付けから抽選番号を0,1変換
                        if (createRate == 0)
                        {
                            //抽選したオブジェクトの設置
                            //前回の更新位置から、現在の位置までくり返し処理
                            //_stageDatasの配列に設置し、処理分iの処理数を加算
                            for (int x = 0; x < createCell; x++) { _stageDatas[j + x, lastFloorHeight + 1 + objHeight] = objNumber; }
                        }


                        /*[ 探索範囲の更新 ]*/
                        //現在配置した場所に+1することで1マス空けて次を配置
                        j += createCell + 1;
                        if (j >= i - 1) { break; }
                    }

                    //床高さの変わった最初の配列番号を更新
                    firstChangeNumber = i + 1;
                    //最後の床高さを更新,配列サイズを超えたら終了
                    if (firstChangeNumber >= floorHeights.Length) { break; }
                    lastFloorHeight = floorHeights[firstChangeNumber];
                }
            }
            #endregion


            #region ステージオブジェクトの生成
            //ステージをパッケージにして管理
            //オブジェクト管理用の空オブジェクトをインスタンス
            GameObject stagePackages = new GameObject(_stagePackageName + _packageStageDatas.Count);
            //Listに補完
            _packageStageDatas.Add(stagePackages);

            /*[ ステージの生成 ]*/
            //各々の_objNumberDatas番号に応じて、オブジェクトを配置
            //ステージにオブジェクトをインスタンス
            for (int i = 0; i < _stageDatas.GetLength(0); i++)
            {
                for (int j = 0; j < _stageDatas.GetLength(1); j++)
                {
                    //ステージデータ配列に格納されている値を取得
                    (int x, int y) objNumber = _stageDatas[i, j];
                    int objClassNumber = _objNumberDatas[objNumber.x, objNumber.y];

                    //空白番号以外なら処理
                    if (objNumber.x != _NULL_NUMBER)
                    {
                        //オブジェクトのインスタンス
                        GameObject newObj = Instantiate(_objDatas[objClassNumber]._createObj, new Vector2(i + createPos.x, j + createPos.y), Quaternion.identity);

                        //生成したオブジェクトを子オブジェクト化
                        newObj.transform.parent = stagePackages.transform;

                        //生成したオブジェクトが床なら
                        if (objNumber.x == _FLOOR_NUMBER_X)
                        {
                            //Colliderの適用
                            switch (_useCollider)
                            {
                                case UseCollider._ALL:

                                    //BoxCollider2Dを適用
                                    newObj.AddComponent<BoxCollider2D>();
                                    break;


                                case UseCollider._OUTER:

                                    //最上面なら、BoxCollider2Dを適用
                                    if (floorHeights[i] == j) { newObj.AddComponent<BoxCollider2D>(); }
                                    break;


                                case UseCollider._NULL: break;

                                default: print("UseColliderのcaseがありません"); break;
                            }
                        }
                    }
                }
            }

            /*[ 床下の生成 ]*/
            //今回設置した床番地を設定
            int floorClassNumber = _objNumberDatas[floorNumber.x, floorNumber.y];
            //床下の空白にオブジェクトをインスタンス
            for (int i = 0; i < _stageSizeX; i++)
            {
                for (int j = 1; j < _underfloorSize; j++)
                {
                    //オブジェクトのインスタンス
                    GameObject newObj = Instantiate(_objDatas[floorClassNumber]._createObj, new Vector2(i + createPos.x, -j + createPos.y), Quaternion.identity);

                    //生成したオブジェクトを子オブジェクト化
                    newObj.transform.parent = stagePackages.transform;

                    //Colliderの適用
                    switch (_useCollider)
                    {
                        case UseCollider._ALL:

                            //BoxCollider2Dを適用
                            newObj.AddComponent<BoxCollider2D>();
                            break;


                        case UseCollider._OUTER: break;

                        case UseCollider._NULL: break;

                        default: print("UseColliderのcaseがありません"); break;
                    }
                }
            }
            #endregion
        }
        catch { print("CreateStageにエラーがあります"); throw; }
    }
    /// <summary>
    /// プレイヤーの動きに合わせて、ステージの生成と描画の表示・非表示を自動で行う
    /// 自動生成と描画の表示・非表示をセットで行いたい場合はこちらを設置してください
    /// </summary>
    /// <param name="startPos">ゲーム開始座標</param>
    /// <param name="nowPos">現在の位置</param>
    public void AutoCreateStage(float startPos, float nowPos)
    {

        /* [ ステージの生成 ] */
        //移動距離の更新
        float moveRange = nowPos - startPos;

        //移動距離が次の生成座標 - オフセットにきたら処理
        if (moveRange >= _stageCreatePos - _createPosOffSet)
        {
            //ステージ生成
            CreateStage((_stageCreatePos, 0f));//ステージの配置
            _stageCreatePos += _stageSizeX;//ステージを更新するプレイヤー座標(x軸)の更新
        }


        /* [ ステージの描画 ] */
        //ステージの描画を消すなら処理
        if (_isDrawingStage)
        {
            //前進中に処理される
            //移動距離が次の描画座標 - オフセットにきたら処理
            if (moveRange >= _nowPackageStagePos - _createPosOffSet)
            {
                //値の更新
                _nowPackageStageNumber++;//現在いるステージパッケージの配列番号
                _nowPackageStagePos += _stageSizeX;//現在いるステージパッケージのプレイヤー座標(x軸)

                //表示・非表示する配列番号
                (int min, int max) activeClimp = (_nowPackageStageNumber - _drawingClamp, _nowPackageStageNumber);
                if (activeClimp.min >= 0) { _packageStageDatas[activeClimp.min].SetActive(false); }//配列番号が配列最小値よりも大きいなら非表示
                if (activeClimp.max < _packageStageDatas.Count) { _packageStageDatas[activeClimp.max].SetActive(true); }//配列番号が配列最大値よりも小さいなら表示
            }
            //後退中に処理される
            //移動距離が次の描画座標 - ステージサイズ(X軸)- オフセットにきたら処理
            else if (moveRange < _nowPackageStagePos - _stageSizeX + _createPosOffSet)
            {
                //値の更新
                _nowPackageStageNumber--;//現在いるステージパッケージの配列番号
                _nowPackageStagePos -= _stageSizeX;//現在いるステージパッケージのプレイヤー座標(x軸)

                //表示・非表示する配列番号
                (int min, int max) activeClimp = (_nowPackageStageNumber, _nowPackageStageNumber + _drawingClamp);
                if (activeClimp.min >= 0) { _packageStageDatas[activeClimp.min].SetActive(true); }//配列番号が配列最小値よりも大きいなら表示
                if (activeClimp.max < _packageStageDatas.Count) { _packageStageDatas[activeClimp.max].SetActive(false); }//配列番号が配列最大値よりも小さいなら非表示
            }
        }
    }


    //プライベートメソッド------------------------------------------------------------------
    /*[ Awakeで設置 ]*/
    /// <summary>
    /// 初期設定
    /// </summary>
    private void StartSetting()
    {
        //ステージの描画を消すなら現在の地点+1までの範囲に設定
        if (_isDrawingStage) { _drawingClamp++; }

        //生成座標のオフセットをステージサイズ(X軸)の半分に設定
        _createPosOffSet = _stageSizeX / 2;
    }
    /// <summary>
    /// 配列内の整理
    /// </summary>
    private void ArraySort()
    {
        try
        {
            /*[ オブジェクト毎に配列を振り分け ]*/
            //配列のサイズを設定
            _objNumberDatas = new int[_OBJ_TYPE_COUNT, _objDatas.Count];//全オブジェクトデータ配列
            _stageDatas = new (int, int)[_stageSizeX, _stageSizeY];//ステージデータ配列
                                                                   //全オブジェクトデータ配列に設置しない番地番号で初期化
            for (int i = 0; i < _objNumberDatas.GetLength(0); i++)
            {
                for (int j = 0; j < _objNumberDatas.GetLength(1); j++) { _objNumberDatas[i, j] = -1; }
            }

            //段差の高さがステージサイズ(Y軸)よりも大きいなら更新
            _floorStepHeight = _floorStepHeight > _stageSizeY ? _stageSizeY : _floorStepHeight;
            //床の最大高さがステージサイズ(Y軸)よりも大きいなら更新
            _floorMaxHeight = _floorMaxHeight > _stageSizeY ? _stageSizeY - 1 : _floorMaxHeight;

            //各部類ごとに配列へ格納し、種類数をカウント
            for (int i = 0; i < _objDatas.Count; i++)
            {
                if (_objDatas[i]._objectClassEnum == ObjDatas.ObjectClassEnum.FLOOR) { _objNumberDatas[_FLOOR_NUMBER_X, _floorCount] = i; _floorCount++; }//床
                if (_objDatas[i]._objectClassEnum == ObjDatas.ObjectClassEnum.GIMMICK) { _objNumberDatas[_GIMMICK_NUMBER_X, _gimmickCount] = i; _gimmickCount++; }//ギミック
                if (_objDatas[i]._objectClassEnum == ObjDatas.ObjectClassEnum.ENEMY) { _objNumberDatas[_ENEMY_NUMBER_X, _enemyCount] = i; _enemyCount++; }//敵
            }


            /*[ 生成できる種別によってソートし、enumを変更 ]*/
            //ギミック・敵のどちらかがないなら、ない方とは別の種類のみにenumを変更
            if (_gimmickCount == 0 && _enemyCount == 0) { _createObstacleEnum = CreateObstacleEnum._NULL; print("設置できるギミック・敵がありません"); }
            else if (_gimmickCount == 0) { _createObstacleEnum = CreateObstacleEnum._ENEMY_ONLY; }//敵のみ
            else if (_enemyCount == 0) { _createObstacleEnum = CreateObstacleEnum._GIMMICK_ONLY; }//ギミックのみ
            else { _createObstacleEnum = CreateObstacleEnum._BOTH; }//両方

            //ギミック・敵の生成する高さによってenum変更
            GimmickSort(_createObstacleEnum);
            EnemySort(_createObstacleEnum);
        }
        catch { print("ArraySortにエラーがあります"); throw; }
    }

    /*[ ArraySortで設置 ]*/
    /// <summary>
    /// ギミックのソートとデータの有無チェック
    /// </summary>
    /// <param name="createObstacleEnum">生成する障害物enum</param>
    private void GimmickSort(CreateObstacleEnum createObstacleEnum)
    {
        try
        {
            if (createObstacleEnum != CreateObstacleEnum._ENEMY_ONLY)
            {
                //格納されているオブジェクトを高さ毎にソートし、ソート後の0と0以外の境目番地を返却
                _gimmickAirNumber = SortObjArray(_gimmickCount, _GIMMICK_NUMBER_X);//ギミック

                //生成高さが0と0以外で、ない方とは別の種類のみにenumを変更
                if (_gimmickAirNumber == _gimmickCount) { _createGimmickHeightEnum = CreateGimmickHeightEnum._GROUND_ONLY; }//地上のみ
                else if (_gimmickAirNumber == 0) { _createGimmickHeightEnum = CreateGimmickHeightEnum._AIR_ONLY; }//空中のみ
                else { _createGimmickHeightEnum = CreateGimmickHeightEnum._BOTH; }//両方
            }
            else { _createGimmickHeightEnum = CreateGimmickHeightEnum._NULL; }
        }
        catch { print("GimmickSortにエラーがあります"); throw; }
    }
    /// <summary>
    /// 敵のソートとデータの有無チェック
    /// </summary>
    /// <param name="createObstacleEnum">生成する障害物enum</param>
    private void EnemySort(CreateObstacleEnum createObstacleEnum)
    {
        try
        {
            if (createObstacleEnum != CreateObstacleEnum._GIMMICK_ONLY)
            {
                //格納されているオブジェクトを高さ毎にソートし、ソート後の0と0以外の境目番地を返却
                _enemyAirNumber = SortObjArray(_enemyCount, _ENEMY_NUMBER_X);

                //生成高さが0と0以外で、ない方とは別の種類のみにenumを変更
                if (_enemyAirNumber == _enemyCount) { _createEnemyHeightEnum = CreateEnemyHeightEnum._GROUND_ONLY; }//地上のみ
                else if (_enemyAirNumber == 0) { _createEnemyHeightEnum = CreateEnemyHeightEnum._AIR_ONLY; }//空中のみ
                else { _createEnemyHeightEnum = CreateEnemyHeightEnum._BOTH; }//両方
            }
            else { _createEnemyHeightEnum = CreateEnemyHeightEnum._NULL; }

        }
        catch { print("EnemySortにエラーがあります"); throw; }
    }

    /*[ GimmickSort・EnemySortで設置 ]*/
    /// <summary>
    /// 格納されているオブジェクトを高さ毎にソート
    /// </summary>
    /// <param name="arraySize">格納配列のサイズ</param>
    /// <param name="sortObjNumber">ソートするオブジェクトの種類番地</param>
    /// <returns>ソート後の0と0以外の境目番地を返却</returns>
    private int SortObjArray(int arraySize, int sortObjNumber)
    {
        try
        {
            //ソート用配列を初期化
            int[] sortArray = new int[arraySize];
            //格納番号
            (int head, int hip) sortNumber = (0, 1);
            //配列のコピー
            for (int i = 0; i < arraySize; i++) { sortArray[i] = _objNumberDatas[sortObjNumber, i]; }

            //高さ0のオブジェクトを探索して格納
            for (int i = 0; i < sortArray.Length; i++)
            {
                //格納されている値
                int storedNumber = sortArray[i];
                //storedNumber番地のオブジェクトの生成高さを設定
                int airHeight = _objDatas[storedNumber]._airObstaclesHeight;

                //airHeightが0(地上)なら頭から、0以外(空中)なら尻から格納
                //頭からの番地を更新していき、生成高さを0と0以外でソート
                if (airHeight == 0) { _objNumberDatas[sortObjNumber, sortNumber.head] = storedNumber; sortNumber.head++; }
                else if (airHeight != 0) { _objNumberDatas[sortObjNumber, sortArray.Length - sortNumber.hip] = storedNumber; sortNumber.hip++; }
            }

            //ソート後の0と0以外の境目番地を返却
            return sortNumber.head;
        }
        catch { print("SortObjArrayにエラーがあります"); throw; }
    }


    //デバックメソッド-----------------------------------------------------------------------
    /*[ デバック ]*/
    /// <summary>
    /// オブジェクト管理の配列を可視化
    /// </summary>
    private void DebugArray()
    {
        try
        {
            //デバック用で生成したオブジェクトをパッケージ化
            GameObject stagePackages = new GameObject(_stagePackageName + "Debug");

            for (int i = 0; i < _objNumberDatas.GetLength(0); i++)
            {
                for (int j = 0; j < _objNumberDatas.GetLength(1); j++)
                {
                    //配列番号-1(Null)以外なら、設定されている番号のオブジェクトを生成
                    if (_objNumberDatas[i, j] == -1) { break; }
                    GameObject newObj = Instantiate(_objDatas[_objNumberDatas[i, j]]._createObj, new Vector2(i * 2 - 20, j * 2 + 20), Quaternion.identity);

                    //生成したオブジェクトを子オブジェクト化
                    newObj.transform.parent = stagePackages.transform;

                }
            }
        }
        catch { print("DebugArrayにエラーがあります"); throw; }
    }
}
