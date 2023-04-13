using System.Collections.Generic;
using UnityEngine;
using System;



/* ����T�v
 * 
 * �[�[����ҁE���쎞���[�[
 * ���c�ˌ�
 * �k�C�������w�Z�Q�[���N���G�C�^��
 * 2023�N4������
 * 
 * 
 * �[�[�[��i�T�v�[�[�[
 * �V���[�v�ȃX�e�[�W��t���b�g�ȃX�e�[�W�ǂ������肽���B�G�ƃM�~�b�N�̂ǂ�����R�o�����H
 * ���������X�e�[�W�͌������ǁA�����ō��̂͂߂�ǂ������B�ł��A�����ƃN���A�ł���X�e�[�W����Ȃ��ƃ_����...
 * StageMaker�́A����Ȋ�]��N�ł��ȒP�Ɋ�������@�\�����˔����Ă��܂��B
 * 
 * 
 * �[�[�[�݌v�v�z�[�[�[
 * �E�X�e�[�W�̃p�����[�^�[�����Ɏ��R�x���������A�X�e�[�W�Ƃ��ăN���A�ł�����̂��������������݌v
 * �E�����X�e�[�W����������Ȃ��݌v
 * �E�I�u�W�F�N�g�̔z�u�݂̂��s���A�g�p����I�u�W�F�N�g�ɂ����Ȃ�X�N���v�g��U�镑����݌v���Ă��A�X�e�[�W�����Ɏx����������Ȃ��݌v

 * 
 * �[�[�ݒu���̐ݒ���@�[�[
 * �@ObjDatas�ɍD���ȃI�u�W�F�N�g���Z�b�g���A�I�u�W�F�N�g���Ƀp�����[�^�[�����Ă�������
 * �A�X�e�[�W�֘A�̃p�����[�^�[�́A���������X�e�[�W�̏�������ύX�ł��܂�
 * �B�M�~�b�N�E�G�֘A�̃p�����[�^�[�́A�M�~�b�N�E�G�̐���������ύX�ł��܂�
 * �C�X�e�[�W�̕`��֘A�̃p�����[�^�[�́A�X�e�[�W�S�̂̕`��̕\���E��\����ύX�ł��܂�
 * �D�X�N���v�g�ŌĂяo���ۂ́A�����݂̂Ȃ�CreateStage()���A
 * �����ƕ`����I�u�W�F�N�g�̈ʒu�ɓ������Đ����E�\������Ȃ�AutoCreateStage()���Ăяo���ĉ������@
 * 
 * 
 * �[�[�[�[�����菇�[�[�[�[
 * �@�����I�u�W�F�N�g�̔z��(ObjDatas)�Ԓn��z��(_objNumberDatas)�ɕ⊮
 * �A�z��(_stageDatas)�̗��[�ɏ�(_objNumberDatas)�Ԓn��ݒ�(���I)
 * �B�z��(_stageDatas)�̗��[�ȊO�ɏ�(_objNumberDatas)�Ԓn��ݒ�(���I)
 * �C�z��(_stageDatas)�ɃM�~�b�N�E�G(_objNumberDatas)�Ԓn��ݒ�(���I)
 * �D�z��(_stageDatas)�ɐݒ肳��Ă���z��(_objNumberDatas)�Ԓn����e�I�u�W�F�N�g�𐶐�
 * �E���������X�e�[�W��1�̃p�b�P�[�W�ɂ��Ĕz��(_packageStageDatas)�ɕ⊮���ďI�� <summary>
 * 
 * 
 * �[�[�[�[���I�v�f�[�[�[�[
 * ���L���e�ɃC���X�y�N�^�[��ŕ���ݒ肵�A���I���s���Ă��܂�
 * ����
 * �@�E�ݒu�����I�u�W�F�N�g
 * �@�E�i���̍���
 * �@�E�t���b�g�̒���
 * ���M�~�b�N�E�G
 * �@�E�ǂ���𐶐����邩
 * �@�E�A�����Đ������邩
 * �@�E�n�ʂƋ󒆂ǂ���ɐ������邩
 * 
 * 
 * �[�[�[���̑��T�v�[�[�[�[
 * �E�f�o�b�N���\�b�h�́A�z��(_objNumberDatas)�̒��g�������������̂ł��B�����Ă���肠��܂���
 * 
 */
///<summary>
/// �X�e�[�W�����p�X�N���v�g
/// </summary>
public class StageMaker : MonoBehaviour
{
    //�ϐ���--------------------------------------------------------------------------------------------------------------------------------
    #region �ϐ�
    [Header("�I�u�W�F�N�g�֘A")]
    //�I�u�W�F�N�g�f�[�^
    [SerializeField, Tooltip("�����I�u�W�F�N�g�̊Ǘ��f�[�^")] public List<ObjDatas> _objDatas = new List<ObjDatas>();
    [Serializable, Tooltip("�X�e�[�W���\������I�u�W�F�N�g�̃f�[�^�Q")]
    public class ObjDatas
    {
        [SerializeField, Tooltip("�I�u�W�F�N�g��")] private string _name;
        [SerializeField, Tooltip("�����I�u�W�F�N�g")] public GameObject _createObj;
        [SerializeField, Tooltip("�����ɐ�������}�X���̍ő�l"), Range(1, 20)] public int _createMaxRange;
        [SerializeField, Tooltip("�������̒n�ʂ���̍���"), Range(0, 10)] public int _airObstaclesHeight;

        /// <summary>
        /// �I�u�W�F�N�g�̕���
        /// </summary>
        public enum ObjectClassEnum
        {
            [InspectorName("��")] FLOOR,
            [InspectorName("�M�~�b�N")] GIMMICK,
            [InspectorName("�G")] ENEMY,
        }
        [SerializeField, Tooltip("�I�u�W�F�N�g�̕���")] public ObjectClassEnum _objectClassEnum;
    }
    //�I�u�W�F�N�g�Ԓn(int�^�̔z���default�l1�ׁ̈A+1������enum�Ԓn�ɍ��킹�Ē���)
    private const int _NULL_NUMBER = 0;//�󔒔ԍ�
    private const int _FLOOR_NUMBER_X = (int)ObjDatas.ObjectClassEnum.FLOOR + 1;//���ԍ�
    private const int _GIMMICK_NUMBER_X = (int)ObjDatas.ObjectClassEnum.GIMMICK + 1;//�M�~�b�N�ԍ�
    private const int _ENEMY_NUMBER_X = (int)ObjDatas.ObjectClassEnum.ENEMY + 1;//�G�ԍ�
    //�I�u�W�F�N�g�̎�ސ�
    private const int _OBJ_TYPE_COUNT = 4;//�I�u�W�F�N�g�̎�ސ�(��,��,�M�~�b�N,�G)
    private int _floorCount = default;//���̎�ސ�
    private int _gimmickCount = default;//�M�~�b�N�̎�ސ�
    private int _enemyCount = default;//�G�̎�ސ�
    //�\�[�g���_airObstaclesHeight > 0 �̔z��ԍ�
    private int _gimmickAirNumber = default;//�M�~�b�N�̋󒆂̔z��J�n�ԍ�
    private int _enemyAirNumber = default;//�G�̋󒆂̔z��J�n�ԍ�
    //�S�I�u�W�F�N�g�f�[�^�z��
    private int[,] _objNumberDatas = default;
    private string _stagePackageName = "StagePackage";//�I�u�W�F�N�g�Ǘ��p�̋�I�u�W�F�N�g��


    [Header("�X�e�[�W�֘A")]
    //�t���O
    [SerializeField, Tooltip("1�}�X�������J��Ԃ����ǂ���")] private bool _isOneCellLoop;
    private bool _isLastOneCell = false;//�O��1�}�X������������
    //�X�e�[�W�T�C�Y
    [SerializeField, Tooltip("�X�e�[�W�T�C�Y(X��)"), Range(1, 100)] private int _stageSizeX;
    [SerializeField, Tooltip("�X�e�[�W�T�C�Y(Y��)"), Range(1, 50)] private int _stageSizeY;
    [SerializeField, Tooltip("�����̃T�C�Y"), Range(1, 50)] private int _underfloorSize;
    //�z��
    private (int x, int y)[,] _stageDatas;//�X�e�[�W�f�[�^
    //�X�e�[�W�I�u�W�F�N�g��z�u���镝
    [SerializeField, Tooltip("�i���̍���"), Range(0, 10)] private int _floorStepHeight;
    [SerializeField, Tooltip("���̍ő卂��"), Range(1, 50)] private int _floorMaxHeight;
    //���������X�e�[�W��1�I�u�W�F�N�g�Ƃ��ĕ⊮
    private List<GameObject> _packageStageDatas = new List<GameObject>();
    private enum UseCollider
    {
        [InspectorName("�S�ĂɓK�p")] _ALL,
        [InspectorName("�O�ʂɂ̂ݓK�p")] _OUTER,
        [InspectorName("�K�p���Ȃ�")] _NULL,
    }
    [SerializeField, Tooltip("BoxCollider2D�����ɓK�p����͈�")] private UseCollider _useCollider = default;




    [Header("�M�~�b�N�E�G�֘A")]
    //��Q���̐�������
    [SerializeField, Tooltip("�����̊����ŏ�Q���𐶐����邩"), Range(0, 10)] private int _createRate;
    private static (int min, int max) __createRateClimb = (1, 10);
    //�M�~�b�N�E�G�̐�������
    [SerializeField, Tooltip("�M�~�b�N�E�G�̐�������"), Range(0, 10)] private int _gimmickOrEnemy;
    private static (int min, int max) __createObstacleClimb = (1, 10);
    //�n��E�󒆂̐�������
    [SerializeField, Tooltip("�n�ʁE�󒆂̐�������"), Range(0, 10)] private int _GroundOrAir;
    private static (int min, int max) __createFieldClimb = (1, 10);
    //Enum
    /// <summary>
    /// ���������Q��
    /// </summary>
    private enum CreateObstacleEnum
    {
        [InspectorName("�M�~�b�N�E�G�Ȃ�")] _NULL,
        [InspectorName("����")] _BOTH,
        [InspectorName("�M�~�b�N�̂�")] _GIMMICK_ONLY,
        [InspectorName("�G�̂�")] _ENEMY_ONLY,
    }
    private CreateObstacleEnum _createObstacleEnum = default;
    /// <summary>
    /// �������鍂��(�M�~�b�N)
    /// </summary>
    private enum CreateGimmickHeightEnum
    {
        [InspectorName("�M�~�b�N�Ȃ�")] _NULL,
        [InspectorName("����")] _BOTH,
        [InspectorName("����0�̂�")] _GROUND_ONLY,
        [InspectorName("����1�ȏ�̂�")] _AIR_ONLY,
    }
    private CreateGimmickHeightEnum _createGimmickHeightEnum = default;
    /// <summary>
    /// �������鍂��(�G)
    /// </summary>
    private enum CreateEnemyHeightEnum
    {
        [InspectorName("�G�Ȃ�")] _NULL,
        [InspectorName("����")] _BOTH,
        [InspectorName("����0�̂�")] _GROUND_ONLY,
        [InspectorName("����1�ȏ�̂�")] _AIR_ONLY,
    }
    private CreateEnemyHeightEnum _createEnemyHeightEnum = default;


    [Header("�X�e�[�W�̕`��֘A")]
    [SerializeField, Tooltip("�X�e�[�W�̕`����������ǂ���")] private bool _isDrawingStage;
    [SerializeField, Tooltip("�X�e�[�W�̕`�悷��͈�(1�ł���΁A���݂̒n�_+1�܂ł͈͕̔͂`��)"), Range(1, 10)] private int _drawingClamp;
    private int _createPosOffSet = default;//�������W�̃I�t�Z�b�g
    private float _stageCreatePos = 0;//�X�e�[�W���X�V����v���C���[���W(x��)
    private float _nowPackageStagePos = 0;//���݂���X�e�[�W�p�b�P�[�W�̃v���C���[���W(x��)
    private int _nowPackageStageNumber = -1;//���݂���X�e�[�W�p�b�P�[�W�̔z��ԍ�(-1�Ԓn����J�n���邱�ƂŁA�X�V���ɉ��Z�����0�Ԓn����J�n�ł���)
    #endregion



    //������--------------------------------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        //�����ݒ�
        StartSetting();

        //�z����̐���
        ArraySort();

        //�I�u�W�F�N�g�Ǘ��̔z���`��
        DebugArray();
    }



    //���\�b�h��----------------------------------------------------------------------------------------------------------------------------
    //�p�u���b�N���\�b�h--------------------------------------------------------------------
    /*[ �ݒu�������^�C�~���O�Ŏ��R�ɌĂяo�� ]*/
    /// <summary>
    /// �X�e�[�W�̐���
    /// �����݂̂��������ꍇ�������ݒu���Ă�������
    /// </summary>
    /// <param name="createPos">����������W</param>
    public void CreateStage((float x, float y) createPos)
    {
        try
        {
            #region �z��̏����ݒ�
            int[] floorHeights = new int[_stageSizeX];//_stageDatas.x���̏�����
            for (int i = 0; i < floorHeights.Length; i++) { floorHeights[i] = 0; }//floorHeights�̏�����
            (int x, int y) floorNumber = (_FLOOR_NUMBER_X, 0);//����ݒu���鏰�̎�ޔԒn
            for (int i = 0; i < _stageDatas.GetLength(0); i++)
            {
                for (int j = 0; j < _stageDatas.GetLength(1); j++) { _stageDatas[i, j] = (_NULL_NUMBER, _NULL_NUMBER); }
            }//_stageDatas�̏�����
            #endregion


            #region ���[�̏���ݒu
            //���̔ԍ��𒊑I
            floorNumber.y = UnityEngine.Random.Range(0, _floorCount);//���I
            floorNumber = (floorNumber.x, floorNumber.y);//�X�V

            //�ǂ̗��[�����𒊑I
            int floorHeight = UnityEngine.Random.Range(0, _floorStepHeight);
            //0�Ԓn(X��)�ƍŏI�Ԓn(X��)�ɏ��ԍ���ݒ�
            for (int j = 0; j <= floorHeight; j++) { _stageDatas[0, j] = floorNumber; }//���[
            for (int j = 0; j <= floorHeight; j++) { _stageDatas[_stageDatas.GetLength(0) - 1, j] = floorNumber; }//�E�[

            //���������i�[
            floorHeights[0] = floorHeight;
            floorHeights[floorHeights.Length - 1] = floorHeight;
            #endregion


            #region ����ݒu
            for (int i = 1; i < _stageDatas.GetLength(0) - 1;)
            {
                /*[ ���̍�����ݒ� ]*/
                //�O��̏��������猸�Z�����l��0�ȉ��Ȃ�A�ŏ��l��O��̏������ɐݒ�
                int floorHeightRandamX = floorHeight - _floorStepHeight < 0 ? floorHeight : floorHeight - _floorStepHeight;
                //���̍ő卂���ȏ�Ȃ�A�ő�l�����̍ő卂���ɐݒ�
                int floorHeightRandamY = floorHeight + _floorStepHeight >= _floorMaxHeight ? _floorMaxHeight : floorHeight + _floorStepHeight;
                //���̍�����O��̍���+-���璊�I
                floorHeight = UnityEngine.Random.Range(floorHeightRandamX, floorHeightRandamY);


                /*[ ���̕���ݒ� ]*/
                //�����_�����I�̍ŏ��l�̏�����
                int randomMin = 1;
                //1�}�X�������J��Ԃ��Ȃ��ݒ�Ȃ�
                if (!_isOneCellLoop) { randomMin = _isLastOneCell == true ? 2 : randomMin; }
                //���̕��ʂɂȂ钷���𒊑I
                int floorWidthRange = UnityEngine.Random.Range(randomMin, _objDatas[_objNumberDatas[floorNumber.x, floorNumber.y]]._createMaxRange);
                //���I�l��1�Ȃ�true��ݒ�
                _isLastOneCell = floorWidthRange == 1 ? true : false;
                //���̒�����_stageDatas.x�̍ő�l�ȏ�Ȃ�A���̒�����_stageDatas.x�ɐݒ�
                int floorWidth = i + floorWidthRange <= _stageDatas.GetLength(0) - 1 ? i + floorWidthRange : _stageDatas.GetLength(0) - 1;


                /*[ ��L2����(floorWidth,floorHeight)���̃}�X�����ɐݒ� ]*/
                //�O��̐ݒ�ԍ�~+floorWidth���̕�
                for (int x = i; x < floorWidth; x++)
                {
                    //1~floorHeight���̍���
                    for (int y = 0; y <= floorHeight; y++) { _stageDatas[x, y] = floorNumber; }

                    //���������i�[
                    floorHeights[x] = floorHeight;
                }
                //���ɐݒ肵�����Ai�����Z
                i += floorWidthRange;
            }
            #endregion


            #region �M�~�b�N�ƓG��ݒu
            //�Ō�̏�������ݒ�
            int lastFloorHeight = floorHeights[0];
            //�������̕ς�����ŏ��̔z��ԍ�
            int firstChangeNumber = 0;

            //�i�[����Ă��鏰�̍���������
            for (int i = 0; i < floorHeights.Length; i++)
            {

                //�ݒu�ł���M�~�b�N�E�G���Ȃ��ꍇ�����I��
                if (_createObstacleEnum == CreateObstacleEnum._NULL) { break; }

                //���̍������ς������
                if (lastFloorHeight != floorHeights[i])
                {
                    for (int j = firstChangeNumber; j < i;)
                    {
                        //�ݒu����I�u�W�F�N�g�̔z��ԍ��̔Ԓn
                        (int x, int y) objNumber = default;


                        /*[ �ݒu����I�u�W�F�N�g�̎�ނ𒊑I ]*/
                        //�M�~�b�N�ƓG�ǂ���𐶐����邩���I(0 = �M�~�b�N, 1 = �G)
                        //�ǂ���̃I�u�W�F�N�g������Ȃ�
                        if (_createObstacleEnum == CreateObstacleEnum._BOTH)
                        {
                            objNumber.x = UnityEngine.Random.Range(__createObstacleClimb.min, __createObstacleClimb.max);//���I
                            objNumber.x = objNumber.x >= _gimmickOrEnemy ? 0 : 1;//�d�ݕt�����璊�I�ԍ���0,1�ϊ�
                        }
                        //�Е��̃I�u�W�F�N�g�����Ȃ��ꍇ�A�l���Œ艻
                        else { objNumber.x = _createObstacleEnum == CreateObstacleEnum._GIMMICK_ONLY ? 0 : 1; }
                        //�ݒu����I�u�W�F�N�g��z��ԍ��ɕϊ�
                        (objNumber.x, objNumber.y) = objNumber.x == 0 ? (_GIMMICK_NUMBER_X, _gimmickCount) : (_ENEMY_NUMBER_X, _enemyCount);


                        /*[ �ݒu����I�u�W�F�N�g�̐������W�𒊑I ]*/
                        //�n��Ƌ󒆂ǂ���ɐ������邩���I(0 = �n��, 1 = ��)
                        //�T������z��͈̔͂�������
                        (int min, int max) groundOrAirClimp = default;
                        //�ݒu����I�u�W�F�N�g�̎�ނ��M�~�b�N�Ȃ�
                        if (objNumber.x == _GIMMICK_NUMBER_X)
                        {
                            //�ǂ���̃t�B�[�h������Ȃ�
                            if (_createGimmickHeightEnum == CreateGimmickHeightEnum._BOTH)
                            {
                                groundOrAirClimp.min = UnityEngine.Random.Range(__createFieldClimb.min, __createFieldClimb.max);//���I
                                groundOrAirClimp.min = groundOrAirClimp.min >= _GroundOrAir ? 0 : 1;//�d�ݕt�����璊�I�ԍ���0,1�ϊ�
                            }
                            //�Е��̃t�B�[�h�����Ȃ��ꍇ�A�l���Œ艻
                            else { groundOrAirClimp.min = _createGimmickHeightEnum == CreateGimmickHeightEnum._GROUND_ONLY ? 0 : 1; }
                            //�T������z��͈̔͂�ݒ�
                            //�n��(0~�󒆔z��̊J�n�Ԓn),��(�󒆔z��̊J�n�Ԓn~�z���Length)
                            groundOrAirClimp = groundOrAirClimp.min == 0 ? (0, _gimmickAirNumber) : (_gimmickAirNumber, objNumber.y);
                        }
                        //�ݒu����I�u�W�F�N�g�̎�ނ��G�Ȃ�
                        else if (objNumber.x == _ENEMY_NUMBER_X)
                        {
                            //�ǂ���̃t�B�[�h������Ȃ�
                            if (_createEnemyHeightEnum == CreateEnemyHeightEnum._BOTH)
                            {
                                groundOrAirClimp.min = UnityEngine.Random.Range(__createFieldClimb.min, __createFieldClimb.max);//���I
                                groundOrAirClimp.min = groundOrAirClimp.min >= _GroundOrAir ? 0 : 1;//�d�ݕt�����璊�I�ԍ���0,1�ϊ�
                            }
                            //�Е��̃t�B�[�h�����Ȃ��ꍇ�A�l���Œ艻
                            else { groundOrAirClimp.min = _createEnemyHeightEnum == CreateEnemyHeightEnum._GROUND_ONLY ? 0 : 1; }
                            //�T������z��͈̔͂�ݒ�
                            //�n��(0~�󒆔z��̊J�n�Ԓn),��(�󒆔z��̊J�n�Ԓn~�z���Length)
                            groundOrAirClimp = groundOrAirClimp.min == 0 ? (0, _enemyAirNumber) : (_enemyAirNumber, objNumber.y);
                        }
                        else { print("�ݒu����I�u�W�F�N�g�̐������W������܂���"); }


                        /*[ �ݒu����I�u�W�F�N�g�𒊑I ]*/
                        //gimmickOrEnemy�Ō��肳�ꂽ��ނ���ǂ̃I�u�W�F�N�g��ݒu���邩���I
                        objNumber.y = UnityEngine.Random.Range(groundOrAirClimp.min, groundOrAirClimp.max);


                        /*[ �ݒu����I�u�W�F�N�g���m�� ]*/
                        //�ݒu����I�u�W�F�N�g
                        objNumber = (objNumber.x, objNumber.y);//�ݒu����I�u�W�F�N�g�̔z��ԍ�
                        ObjDatas objDatas = _objDatas[_objNumberDatas[objNumber.x, objNumber.y]];//�ݒu����I�u�W�F�N�g�̃f�[�^
                        int objHeight = objDatas._airObstaclesHeight;//�ݒu����I�u�W�F�N�g�̐������鍂����ݒ�
                        int createCell = UnityEngine.Random.Range(1, objDatas._createMaxRange);//�ݒu����I�u�W�F�N�g�����}�X�������邩���I
                        createCell = createCell <= i - j ? createCell : i - j;//�����}�X���������͈͂𒴂��Ă���Ȃ�A�����͈͂ɐ����}�X����ύX


                        /*[ ���I�����I�u�W�F�N�g��z�u���邩���I ]*/
                        //�������邩���I(0 = ����, 1 =�@���Ȃ�)
                        int createRate = UnityEngine.Random.Range(__createRateClimb.min, __createRateClimb.max);//���I
                        createRate = createRate <= _createRate ? 0 : 1;//�d�ݕt�����璊�I�ԍ���0,1�ϊ�
                        if (createRate == 0)
                        {
                            //���I�����I�u�W�F�N�g�̐ݒu
                            //�O��̍X�V�ʒu����A���݂̈ʒu�܂ł���Ԃ�����
                            //_stageDatas�̔z��ɐݒu���A������i�̏����������Z
                            for (int x = 0; x < createCell; x++) { _stageDatas[j + x, lastFloorHeight + 1 + objHeight] = objNumber; }
                        }


                        /*[ �T���͈͂̍X�V ]*/
                        //���ݔz�u�����ꏊ��+1���邱�Ƃ�1�}�X�󂯂Ď���z�u
                        j += createCell + 1;
                        if (j >= i - 1) { break; }
                    }

                    //�������̕ς�����ŏ��̔z��ԍ����X�V
                    firstChangeNumber = i + 1;
                    //�Ō�̏��������X�V,�z��T�C�Y�𒴂�����I��
                    if (firstChangeNumber >= floorHeights.Length) { break; }
                    lastFloorHeight = floorHeights[firstChangeNumber];
                }
            }
            #endregion


            #region �X�e�[�W�I�u�W�F�N�g�̐���
            //�X�e�[�W���p�b�P�[�W�ɂ��ĊǗ�
            //�I�u�W�F�N�g�Ǘ��p�̋�I�u�W�F�N�g���C���X�^���X
            GameObject stagePackages = new GameObject(_stagePackageName + _packageStageDatas.Count);
            //List�ɕ⊮
            _packageStageDatas.Add(stagePackages);

            /*[ �X�e�[�W�̐��� ]*/
            //�e�X��_objNumberDatas�ԍ��ɉ����āA�I�u�W�F�N�g��z�u
            //�X�e�[�W�ɃI�u�W�F�N�g���C���X�^���X
            for (int i = 0; i < _stageDatas.GetLength(0); i++)
            {
                for (int j = 0; j < _stageDatas.GetLength(1); j++)
                {
                    //�X�e�[�W�f�[�^�z��Ɋi�[����Ă���l���擾
                    (int x, int y) objNumber = _stageDatas[i, j];
                    int objClassNumber = _objNumberDatas[objNumber.x, objNumber.y];

                    //�󔒔ԍ��ȊO�Ȃ珈��
                    if (objNumber.x != _NULL_NUMBER)
                    {
                        //�I�u�W�F�N�g�̃C���X�^���X
                        GameObject newObj = Instantiate(_objDatas[objClassNumber]._createObj, new Vector2(i + createPos.x, j + createPos.y), Quaternion.identity);

                        //���������I�u�W�F�N�g���q�I�u�W�F�N�g��
                        newObj.transform.parent = stagePackages.transform;

                        //���������I�u�W�F�N�g�����Ȃ�
                        if (objNumber.x == _FLOOR_NUMBER_X)
                        {
                            //Collider�̓K�p
                            switch (_useCollider)
                            {
                                case UseCollider._ALL:

                                    //BoxCollider2D��K�p
                                    newObj.AddComponent<BoxCollider2D>();
                                    break;


                                case UseCollider._OUTER:

                                    //�ŏ�ʂȂ�ABoxCollider2D��K�p
                                    if (floorHeights[i] == j) { newObj.AddComponent<BoxCollider2D>(); }
                                    break;


                                case UseCollider._NULL: break;

                                default: print("UseCollider��case������܂���"); break;
                            }
                        }
                    }
                }
            }

            /*[ �����̐��� ]*/
            //����ݒu�������Ԓn��ݒ�
            int floorClassNumber = _objNumberDatas[floorNumber.x, floorNumber.y];
            //�����̋󔒂ɃI�u�W�F�N�g���C���X�^���X
            for (int i = 0; i < _stageSizeX; i++)
            {
                for (int j = 1; j < _underfloorSize; j++)
                {
                    //�I�u�W�F�N�g�̃C���X�^���X
                    GameObject newObj = Instantiate(_objDatas[floorClassNumber]._createObj, new Vector2(i + createPos.x, -j + createPos.y), Quaternion.identity);

                    //���������I�u�W�F�N�g���q�I�u�W�F�N�g��
                    newObj.transform.parent = stagePackages.transform;

                    //Collider�̓K�p
                    switch (_useCollider)
                    {
                        case UseCollider._ALL:

                            //BoxCollider2D��K�p
                            newObj.AddComponent<BoxCollider2D>();
                            break;


                        case UseCollider._OUTER: break;

                        case UseCollider._NULL: break;

                        default: print("UseCollider��case������܂���"); break;
                    }
                }
            }
            #endregion
        }
        catch { print("CreateStage�ɃG���[������܂�"); throw; }
    }
    /// <summary>
    /// �v���C���[�̓����ɍ��킹�āA�X�e�[�W�̐����ƕ`��̕\���E��\���������ōs��
    /// ���������ƕ`��̕\���E��\�����Z�b�g�ōs�������ꍇ�͂������ݒu���Ă�������
    /// </summary>
    /// <param name="startPos">�Q�[���J�n���W</param>
    /// <param name="nowPos">���݂̈ʒu</param>
    public void AutoCreateStage(float startPos, float nowPos)
    {

        /* [ �X�e�[�W�̐��� ] */
        //�ړ������̍X�V
        float moveRange = nowPos - startPos;

        //�ړ����������̐������W - �I�t�Z�b�g�ɂ����珈��
        if (moveRange >= _stageCreatePos - _createPosOffSet)
        {
            //�X�e�[�W����
            CreateStage((_stageCreatePos, 0f));//�X�e�[�W�̔z�u
            _stageCreatePos += _stageSizeX;//�X�e�[�W���X�V����v���C���[���W(x��)�̍X�V
        }


        /* [ �X�e�[�W�̕`�� ] */
        //�X�e�[�W�̕`��������Ȃ珈��
        if (_isDrawingStage)
        {
            //�O�i���ɏ��������
            //�ړ����������̕`����W - �I�t�Z�b�g�ɂ����珈��
            if (moveRange >= _nowPackageStagePos - _createPosOffSet)
            {
                //�l�̍X�V
                _nowPackageStageNumber++;//���݂���X�e�[�W�p�b�P�[�W�̔z��ԍ�
                _nowPackageStagePos += _stageSizeX;//���݂���X�e�[�W�p�b�P�[�W�̃v���C���[���W(x��)

                //�\���E��\������z��ԍ�
                (int min, int max) activeClimp = (_nowPackageStageNumber - _drawingClamp, _nowPackageStageNumber);
                if (activeClimp.min >= 0) { _packageStageDatas[activeClimp.min].SetActive(false); }//�z��ԍ����z��ŏ��l�����傫���Ȃ��\��
                if (activeClimp.max < _packageStageDatas.Count) { _packageStageDatas[activeClimp.max].SetActive(true); }//�z��ԍ����z��ő�l�����������Ȃ�\��
            }
            //��ޒ��ɏ��������
            //�ړ����������̕`����W - �X�e�[�W�T�C�Y(X��)- �I�t�Z�b�g�ɂ����珈��
            else if (moveRange < _nowPackageStagePos - _stageSizeX + _createPosOffSet)
            {
                //�l�̍X�V
                _nowPackageStageNumber--;//���݂���X�e�[�W�p�b�P�[�W�̔z��ԍ�
                _nowPackageStagePos -= _stageSizeX;//���݂���X�e�[�W�p�b�P�[�W�̃v���C���[���W(x��)

                //�\���E��\������z��ԍ�
                (int min, int max) activeClimp = (_nowPackageStageNumber, _nowPackageStageNumber + _drawingClamp);
                if (activeClimp.min >= 0) { _packageStageDatas[activeClimp.min].SetActive(true); }//�z��ԍ����z��ŏ��l�����傫���Ȃ�\��
                if (activeClimp.max < _packageStageDatas.Count) { _packageStageDatas[activeClimp.max].SetActive(false); }//�z��ԍ����z��ő�l�����������Ȃ��\��
            }
        }
    }


    //�v���C�x�[�g���\�b�h------------------------------------------------------------------
    /*[ Awake�Őݒu ]*/
    /// <summary>
    /// �����ݒ�
    /// </summary>
    private void StartSetting()
    {
        //�X�e�[�W�̕`��������Ȃ猻�݂̒n�_+1�܂ł͈̔͂ɐݒ�
        if (_isDrawingStage) { _drawingClamp++; }

        //�������W�̃I�t�Z�b�g���X�e�[�W�T�C�Y(X��)�̔����ɐݒ�
        _createPosOffSet = _stageSizeX / 2;
    }
    /// <summary>
    /// �z����̐���
    /// </summary>
    private void ArraySort()
    {
        try
        {
            /*[ �I�u�W�F�N�g���ɔz���U�蕪�� ]*/
            //�z��̃T�C�Y��ݒ�
            _objNumberDatas = new int[_OBJ_TYPE_COUNT, _objDatas.Count];//�S�I�u�W�F�N�g�f�[�^�z��
            _stageDatas = new (int, int)[_stageSizeX, _stageSizeY];//�X�e�[�W�f�[�^�z��
                                                                   //�S�I�u�W�F�N�g�f�[�^�z��ɐݒu���Ȃ��Ԓn�ԍ��ŏ�����
            for (int i = 0; i < _objNumberDatas.GetLength(0); i++)
            {
                for (int j = 0; j < _objNumberDatas.GetLength(1); j++) { _objNumberDatas[i, j] = -1; }
            }

            //�i���̍������X�e�[�W�T�C�Y(Y��)�����傫���Ȃ�X�V
            _floorStepHeight = _floorStepHeight > _stageSizeY ? _stageSizeY : _floorStepHeight;
            //���̍ő卂�����X�e�[�W�T�C�Y(Y��)�����傫���Ȃ�X�V
            _floorMaxHeight = _floorMaxHeight > _stageSizeY ? _stageSizeY - 1 : _floorMaxHeight;

            //�e���ނ��Ƃɔz��֊i�[���A��ސ����J�E���g
            for (int i = 0; i < _objDatas.Count; i++)
            {
                if (_objDatas[i]._objectClassEnum == ObjDatas.ObjectClassEnum.FLOOR) { _objNumberDatas[_FLOOR_NUMBER_X, _floorCount] = i; _floorCount++; }//��
                if (_objDatas[i]._objectClassEnum == ObjDatas.ObjectClassEnum.GIMMICK) { _objNumberDatas[_GIMMICK_NUMBER_X, _gimmickCount] = i; _gimmickCount++; }//�M�~�b�N
                if (_objDatas[i]._objectClassEnum == ObjDatas.ObjectClassEnum.ENEMY) { _objNumberDatas[_ENEMY_NUMBER_X, _enemyCount] = i; _enemyCount++; }//�G
            }


            /*[ �����ł����ʂɂ���ă\�[�g���Aenum��ύX ]*/
            //�M�~�b�N�E�G�̂ǂ��炩���Ȃ��Ȃ�A�Ȃ����Ƃ͕ʂ̎�ނ݂̂�enum��ύX
            if (_gimmickCount == 0 && _enemyCount == 0) { _createObstacleEnum = CreateObstacleEnum._NULL; print("�ݒu�ł���M�~�b�N�E�G������܂���"); }
            else if (_gimmickCount == 0) { _createObstacleEnum = CreateObstacleEnum._ENEMY_ONLY; }//�G�̂�
            else if (_enemyCount == 0) { _createObstacleEnum = CreateObstacleEnum._GIMMICK_ONLY; }//�M�~�b�N�̂�
            else { _createObstacleEnum = CreateObstacleEnum._BOTH; }//����

            //�M�~�b�N�E�G�̐������鍂���ɂ����enum�ύX
            GimmickSort(_createObstacleEnum);
            EnemySort(_createObstacleEnum);
        }
        catch { print("ArraySort�ɃG���[������܂�"); throw; }
    }

    /*[ ArraySort�Őݒu ]*/
    /// <summary>
    /// �M�~�b�N�̃\�[�g�ƃf�[�^�̗L���`�F�b�N
    /// </summary>
    /// <param name="createObstacleEnum">���������Q��enum</param>
    private void GimmickSort(CreateObstacleEnum createObstacleEnum)
    {
        try
        {
            if (createObstacleEnum != CreateObstacleEnum._ENEMY_ONLY)
            {
                //�i�[����Ă���I�u�W�F�N�g���������Ƀ\�[�g���A�\�[�g���0��0�ȊO�̋��ڔԒn��ԋp
                _gimmickAirNumber = SortObjArray(_gimmickCount, _GIMMICK_NUMBER_X);//�M�~�b�N

                //����������0��0�ȊO�ŁA�Ȃ����Ƃ͕ʂ̎�ނ݂̂�enum��ύX
                if (_gimmickAirNumber == _gimmickCount) { _createGimmickHeightEnum = CreateGimmickHeightEnum._GROUND_ONLY; }//�n��̂�
                else if (_gimmickAirNumber == 0) { _createGimmickHeightEnum = CreateGimmickHeightEnum._AIR_ONLY; }//�󒆂̂�
                else { _createGimmickHeightEnum = CreateGimmickHeightEnum._BOTH; }//����
            }
            else { _createGimmickHeightEnum = CreateGimmickHeightEnum._NULL; }
        }
        catch { print("GimmickSort�ɃG���[������܂�"); throw; }
    }
    /// <summary>
    /// �G�̃\�[�g�ƃf�[�^�̗L���`�F�b�N
    /// </summary>
    /// <param name="createObstacleEnum">���������Q��enum</param>
    private void EnemySort(CreateObstacleEnum createObstacleEnum)
    {
        try
        {
            if (createObstacleEnum != CreateObstacleEnum._GIMMICK_ONLY)
            {
                //�i�[����Ă���I�u�W�F�N�g���������Ƀ\�[�g���A�\�[�g���0��0�ȊO�̋��ڔԒn��ԋp
                _enemyAirNumber = SortObjArray(_enemyCount, _ENEMY_NUMBER_X);

                //����������0��0�ȊO�ŁA�Ȃ����Ƃ͕ʂ̎�ނ݂̂�enum��ύX
                if (_enemyAirNumber == _enemyCount) { _createEnemyHeightEnum = CreateEnemyHeightEnum._GROUND_ONLY; }//�n��̂�
                else if (_enemyAirNumber == 0) { _createEnemyHeightEnum = CreateEnemyHeightEnum._AIR_ONLY; }//�󒆂̂�
                else { _createEnemyHeightEnum = CreateEnemyHeightEnum._BOTH; }//����
            }
            else { _createEnemyHeightEnum = CreateEnemyHeightEnum._NULL; }

        }
        catch { print("EnemySort�ɃG���[������܂�"); throw; }
    }

    /*[ GimmickSort�EEnemySort�Őݒu ]*/
    /// <summary>
    /// �i�[����Ă���I�u�W�F�N�g���������Ƀ\�[�g
    /// </summary>
    /// <param name="arraySize">�i�[�z��̃T�C�Y</param>
    /// <param name="sortObjNumber">�\�[�g����I�u�W�F�N�g�̎�ޔԒn</param>
    /// <returns>�\�[�g���0��0�ȊO�̋��ڔԒn��ԋp</returns>
    private int SortObjArray(int arraySize, int sortObjNumber)
    {
        try
        {
            //�\�[�g�p�z���������
            int[] sortArray = new int[arraySize];
            //�i�[�ԍ�
            (int head, int hip) sortNumber = (0, 1);
            //�z��̃R�s�[
            for (int i = 0; i < arraySize; i++) { sortArray[i] = _objNumberDatas[sortObjNumber, i]; }

            //����0�̃I�u�W�F�N�g��T�����Ċi�[
            for (int i = 0; i < sortArray.Length; i++)
            {
                //�i�[����Ă���l
                int storedNumber = sortArray[i];
                //storedNumber�Ԓn�̃I�u�W�F�N�g�̐���������ݒ�
                int airHeight = _objDatas[storedNumber]._airObstaclesHeight;

                //airHeight��0(�n��)�Ȃ瓪����A0�ȊO(��)�Ȃ�K����i�[
                //������̔Ԓn���X�V���Ă����A����������0��0�ȊO�Ń\�[�g
                if (airHeight == 0) { _objNumberDatas[sortObjNumber, sortNumber.head] = storedNumber; sortNumber.head++; }
                else if (airHeight != 0) { _objNumberDatas[sortObjNumber, sortArray.Length - sortNumber.hip] = storedNumber; sortNumber.hip++; }
            }

            //�\�[�g���0��0�ȊO�̋��ڔԒn��ԋp
            return sortNumber.head;
        }
        catch { print("SortObjArray�ɃG���[������܂�"); throw; }
    }


    //�f�o�b�N���\�b�h-----------------------------------------------------------------------
    /*[ �f�o�b�N ]*/
    /// <summary>
    /// �I�u�W�F�N�g�Ǘ��̔z�������
    /// </summary>
    private void DebugArray()
    {
        try
        {
            //�f�o�b�N�p�Ő��������I�u�W�F�N�g���p�b�P�[�W��
            GameObject stagePackages = new GameObject(_stagePackageName + "Debug");

            for (int i = 0; i < _objNumberDatas.GetLength(0); i++)
            {
                for (int j = 0; j < _objNumberDatas.GetLength(1); j++)
                {
                    //�z��ԍ�-1(Null)�ȊO�Ȃ�A�ݒ肳��Ă���ԍ��̃I�u�W�F�N�g�𐶐�
                    if (_objNumberDatas[i, j] == -1) { break; }
                    GameObject newObj = Instantiate(_objDatas[_objNumberDatas[i, j]]._createObj, new Vector2(i * 2 - 20, j * 2 + 20), Quaternion.identity);

                    //���������I�u�W�F�N�g���q�I�u�W�F�N�g��
                    newObj.transform.parent = stagePackages.transform;

                }
            }
        }
        catch { print("DebugArray�ɃG���[������܂�"); throw; }
    }
}
