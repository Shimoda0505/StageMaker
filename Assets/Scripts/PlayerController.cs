using UnityEngine;



/*����T�v
 * 2023�N3������
 * �k�C�������w�Z�Q�[���N���G�C�^��
 * ���c�ˌ�
 */
/// <summary>
/// �v���C���[�Ǘ��p�X�N���v�g
/// </summary>
public class PlayerController : MonoBehaviour
{
    //�ϐ���--------------------------------------------------------------------------------------------------------------------------------
    #region �ϐ�
    [Header("�X�N���v�g���A")]
    private string _gameControllerName = "GameController";//GameController�^�O��
    private StageMaker _stageMaker;//�X�e�[�W�����X�N���v�g

    [Header("�I�u�W�F�N�g�֘A")]
    private SpriteRenderer _playerEyeRenderer = default;//�v���C���[�̖ڂ�SpriteRenderer
    private string _playerEyeTag = "PlayerEye";//PlayerEye�^�O��
    private string _obstacleTag = "Obstacle";//��Q���^�O��


    [Header("�ړ����A")]
    [SerializeField, Tooltip("�ړ����x"), Range(1, 50)] private int _moveSpeed;
    [SerializeField, Tooltip("�������x"), Range(1, 10)] private int _fallSpeed;
    [SerializeField, Tooltip("�㏸���x"), Range(1, 10)] private int _jumpSpeed;
    [SerializeField, Tooltip("1�N���b�N�ŁA���}�X�㏸���邩"), Range(1, 10)] private int _jumpTrout;
    private Transform _playerTr = default;//�v���C���[��Transform
    private Vector3 _playerMovePos = default;//�v���C���[�̈ړ����W
    private float _startPos = default;//�Q�[���J�n���W(x��)
    private float _playerJumpPos = default;//�W�����v�O��Player��Y����⊮
    private bool _isFall = false;//�����J�n
    private (bool right, bool left) _isWall = (false, false);//�ǂ̏Փ�


    [Header("RayCast�֘A")]
    private const float _rayDistance = 0.5f;//����
    private const float _rayOffSetPos = 0.45f;//���Z�ʒu


    /// <summary>
    /// �v���C���[�̍s��enim
    /// </summary>
    private enum PlayerMotion
    {
        [InspectorName("��~")] IDLE,
        [InspectorName("�ړ�")] MOVE,
        [InspectorName("�㏸")] JUMP,
        [InspectorName("����")] FALL,
        [InspectorName("���S")] DEATH,
    }
    private PlayerMotion _playerMotion = default;
    #endregion



    //������--------------------------------------------------------------------------------------------------------------------------------
    private void Start()
    {
        //�R���|�[�l���g�擾
        _playerEyeRenderer = GameObject.FindGameObjectWithTag(_playerEyeTag).GetComponent<SpriteRenderer>();//�v���C���[�̖ڂ�SpriteRenderer
        _stageMaker = GameObject.FindGameObjectWithTag(_gameControllerName).GetComponent<StageMaker>();//�X�e�[�W�����X�N���v�g�̎擾

        //���W
        _playerTr = this.gameObject.transform;//�v���C���[��Transform
        _playerMovePos = _playerTr.position;//�v���C���[�̈ړ����W
        _startPos = _playerTr.transform.position.x;//�J�n���W(X��)�̕⊮

        //�v���C���[�̓����ɍ��킹�āA�X�e�[�W�̐����ƕ`��̕\���������ōs��
        _stageMaker.AutoCreateStage(_startPos, _playerTr.transform.position.x);
    }



    //���\�b�h��--------------------------------------------------------------------------------------------------------------------------------
    //�p�u���b�N���\�b�h---------------------------------------------------------------------------------------
    /*[ InputManager�Ŏg�p,�ʏ푀�� ]*/
    /// <summary>
    /// Player�̓��͔���̏���
    /// </summary>
    /// <param name="isInputH">���E����</param>
    /// <param name="isInputUp">�����</param>
    public void PlayerInput(bool isInputH, bool isInputUp)
    {

        /*[ Null�`�F�b�N�֘A ]*/
        if (!_playerTr) { print("Player�̍��W�f�[�^���ۊǂ���Ă��܂���"); return; }


        /*[ RayCast�֘A ]*/
        //RayCast�̐ڐG����̏���
        (bool isGround, bool isCelling,  bool isHide, bool isObstacle) = PlayerRayCast(_playerTr.position);
        //�v���C���[�̍s��enum�̕ύX
        if (isObstacle) { _playerMotion = PlayerMotion.DEATH; }//��Q���Ƃ̐ڐG������Ύ��Senum
        //Player���B��Ă��鎞�̏���
        HideMotion(isHide);


        /*[ �ړ����͊֘A ]*/
        //�v���C���[�̓����ɍ��킹�āA�X�e�[�W�̐����ƕ`��̕\���������ōs��
        _stageMaker.AutoCreateStage(_startPos, _playerTr.transform.position.x);

        //�v���C���[�̍s��enim
        switch (_playerMotion)
        {

            case PlayerMotion.IDLE:

                //�v���C���[�̍s��enum�̕ύX
                if (isInputH) { _playerMotion = PlayerMotion.MOVE; }//�ړ�enum
                if (isInputUp && isGround) { _playerMotion = PlayerMotion.JUMP; }//�W�����venum
                if (!isGround) { _playerMotion = PlayerMotion.FALL; }//����enum
                break;


            case PlayerMotion.MOVE:

                //�v���C���[�̍s��enum�̕ύX
                if (!isInputH) { _playerMotion = PlayerMotion.IDLE; }//��~enum
                if (isInputUp && isGround) { _playerMotion = PlayerMotion.JUMP; }//�W�����venum
                if (!isGround) { _playerMotion = PlayerMotion.FALL; }//����enum
                break;


            case PlayerMotion.JUMP:

                //�v���C���[�̍s��enum�̕ύX
                if(_isFall || isCelling) { _playerMotion = PlayerMotion.FALL; _isFall = false; }//����enum
                break;


            case PlayerMotion.FALL:

                //�v���C���[�̍s��enum�̕ύX
                if (isGround) { _playerMotion = PlayerMotion.IDLE; }//��~enum
                break;


            case PlayerMotion.DEATH: break; 


            default: Debug.Log("PlayerMotion(enum)��case�Ȃ�"); break;
        }
    }
    /// <summary>
    /// Player�̈ړ�����
    /// </summary>
    /// <param name="fInputH">���E����</param>
    public void PlayerMove(float fInputH)
    {

        /*[ Null�`�F�b�N�֘A ]*/
        if (!_playerTr) { print("Player�̍��W�f�[�^���ۊǂ���Ă��܂���"); return; }


        /*[ �ړ������֘A ]*/
        //�v���C���[�̍s��enim
        switch (_playerMotion)
        {

            case PlayerMotion.IDLE:

                //Player��Y��
                _playerMovePos.y = Mathf.Round(_playerMovePos.y);//�l�̌ܓ���int���W�ɕϊ�
                _playerJumpPos = _playerMovePos.y + _jumpTrout;//�W�����v���̍��W�v�Z

                //Player��X��
                if(_isWall.left || _isWall.right) { _playerMovePos.x = Mathf.Round(_playerMovePos.x); }//�l�̌ܓ���int���W�ɕϊ�
                break;


            case PlayerMotion.MOVE:

                //Player��Y��
                _playerMovePos.y = Mathf.Round(_playerMovePos.y);//�l�̌ܓ���int���W�ɕϊ�
                _playerJumpPos = _playerMovePos.y + _jumpTrout;//�W�����v���̍��W�v�Z

                //Player��X��
                if (_isWall.right && fInputH > 0) { break; }//�E�ړ����ł��Ȃ����ɉE���͂�����Ώ������Ȃ�
                else if (_isWall.left && fInputH < 0) { break; }//���ړ����ł��Ȃ����ɍ����͂�����Ώ������Ȃ�
                if (fInputH != 0) { _playerMovePos.x += Time.deltaTime * _moveSpeed * Mathf.Sign(fInputH); } //�ړ�
                break;


            case PlayerMotion.JUMP:

                //Player��Y��
                _playerMovePos.y += Time.deltaTime * _jumpSpeed;//�㏸
                if(_playerMovePos.y >= _playerJumpPos) { _isFall = true; }

                //Player��X��
                if (_isWall.right && fInputH > 0) { break; }//�E�ړ����ł��Ȃ����ɉE���͂�����Ώ������Ȃ�
                else if (_isWall.left && fInputH < 0) { break; }//���ړ����ł��Ȃ����ɍ����͂�����Ώ������Ȃ�
                if (fInputH != 0) { _playerMovePos.x += Time.deltaTime * _moveSpeed * Mathf.Sign(fInputH); } //�ړ�
                break;


            case PlayerMotion.FALL:

                //���Ԃ��Ƃ�Player��Y�������Z
                _playerMovePos.y -= Time.deltaTime * _fallSpeed;

                //Player��X��
                if (_isWall.right && fInputH > 0) { break; }//�E�ړ����ł��Ȃ����ɉE���͂�����Ώ������Ȃ�
                else if (_isWall.left && fInputH < 0) { break; }//���ړ����ł��Ȃ����ɍ����͂�����Ώ������Ȃ�
                if (fInputH != 0) { _playerMovePos.x += Time.deltaTime * _moveSpeed * Mathf.Sign(fInputH); } //�ړ�
                break;


            case PlayerMotion.DEATH:

                print("���S");
                break;


            default: Debug.Log("PlayerMotion(enum)��case�Ȃ�"); break;
        }

        //�v���C���[�̈ړ��X�V
        _playerTr.position = _playerMovePos;
    }

    /*[ InputManager�Ŏg�p,�f�o�b�N���� ]*/
    /// <summary>
    /// �v���C���[�̃f�o�b�N���[�h
    /// </summary>
    /// <param name="fInputH">���E����</param>
    /// <param name="fInputV">�㉺����</param>
    public void PlayerDebugMode(float fInputH, float fInputV)
    {

        //���͏���
        if(Mathf.Abs(fInputH) >= 0.1f) { _playerMovePos.x += Time.deltaTime * _moveSpeed * Mathf.Sign(fInputH); }
        if(Mathf.Abs(fInputV) >= 0.1f) { _playerMovePos.y += Time.deltaTime * _moveSpeed * Mathf.Sign(fInputV); }

        //�v���C���[�̈ړ��X�V
        _playerTr.position = _playerMovePos;

        //�v���C���[�̓����ɍ��킹�āA�X�e�[�W�̐����ƕ`��̕\���������ōs��
        _stageMaker.AutoCreateStage(_startPos, _playerTr.transform.position.x);
    }


    //�v���C�x�[�g���\�b�h---------------------------------------------------------------------------------------
    /*[ RayCast ]*/
    /// <summary>
    /// RayCast�̐ڐG�����𔻕�
    /// </summary>
    /// <param name="rayPos">RayCast�̌��_</param>
    /// <breaks>�n�ʂ̐ڐG,�V��̐ڐG,����2�����ȏ�ŕǂƐڐG</breaks>
    private (bool isGround, bool isCeiling, bool isHide, bool isObstacle)  PlayerRayCast(Vector2 rayPos)
    {

        //RayCast�̐ڐG������擾
        (bool isHit, bool isObstacle) isRight = HitRayCast(true, rayPos, Vector2.right);//�E�ǂ̐ڐG
        (bool isHit, bool isObstacle) isLeft = HitRayCast(true, rayPos, Vector2.left);//���ǂ̐ڐG
        (bool isHit, bool isObstacle) isGround = HitRayCast(false, rayPos, Vector2.down);//�n�ʂ̐ڐG
        (bool isHit, bool isObstacle) isCeiling = HitRayCast(false, rayPos, Vector2.up);//�V��̐ڐG

        //Raycast�̐ڐG�����]�����⊮
        bool isHide = isGround.isHit && (isRight.isHit || isLeft.isHit) ? true : false;//����2�����ȏ�ŕǂƐڐG
        bool isObstacle = isRight.isObstacle || isLeft.isObstacle || isGround.isObstacle || isCeiling.isObstacle ? true : false;//��Q���ƐڐG�������ǂ���
        _isWall = (isRight.isHit, isLeft.isHit);//���E�̐ڐG�����⊮

        //�l�̕ԋp
        return (isGround.isHit, isCeiling.isHit, isHide, isObstacle);
    }
    /// <summary>
    /// RayCast�̐ڐG����̏���
    /// </summary>
    /// <param name="HorizontalOrVertical">�c���ǂ���ɂƂ΂���</param>
    /// <param name="rayPos">���_</param>
    /// <param name="throwDirection">�ǂ̕����ɔ�΂���</param>
    /// <returns>RayCast���ڐG������</returns>
    private (bool isHit, bool isObstacle) HitRayCast(bool HorizontalOrVertical, Vector2 rayPos, Vector2 throwDirection)
    {
        //���Əc�ǂ���ɔ�΂����ɂ��A�I�t�Z�b�g�ʒu��ύX
        Vector2 rayOffSetPos = HorizontalOrVertical ? new Vector2(0, _rayOffSetPos) : new Vector2(_rayOffSetPos, 0);

        //RayCast���e�����ɔ�΂�
        RaycastHit2D hitPlus = Physics2D.Raycast(rayPos + rayOffSetPos, throwDirection, _rayDistance);//�v���X�ʒu
        RaycastHit2D hitMinus = Physics2D.Raycast(rayPos - rayOffSetPos, throwDirection, _rayDistance);//�}�C�i�X�ʒu

        //RayCast�̉���
        Debug.DrawRay(rayPos + rayOffSetPos, throwDirection * _rayDistance, Color.red);//�v���X�ʒu
        Debug.DrawRay(rayPos - rayOffSetPos, throwDirection * _rayDistance, Color.red);//�}�C�i�X�ʒu

        //Raycast�̐ڐG�����]�����ԋp
        bool isHit = hitPlus || hitMinus ? true : false;
        //�ڐG�����I�u�W�F�N�g����Q���̏ꍇ���S�t���Otrue
        bool isObstacle = false;
        if (hitPlus) { isObstacle = hitPlus.collider.tag == _obstacleTag ? true : false; }//�v���X�ʒu
        if (hitMinus) { isObstacle = hitMinus.collider.tag == _obstacleTag ? true : false; }//�}�C�i�X�ʒu
        return (isHit, isObstacle);
    }

    /*[ �v���C���[���f���̃A�j���[�V���� ]*/
    /// <summary>
    /// Player���B��Ă��鎞(����2�����ɕ�or��)�̏���
    /// </summary>
    /// <param name="isHide">isHide�t���O</param>
    private void HideMotion(bool isHide)
    {

        //Null�̏ꍇ�������Ȃ�
        if (_playerEyeRenderer == null) { return; }

        if (isHide) { _playerEyeRenderer.color = Color.black; }
        else if (!isHide) { _playerEyeRenderer.color = Color.white; }
    }
}
