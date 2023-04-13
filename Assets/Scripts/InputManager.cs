using UnityEngine;
using System;



/*����T�v
 * 2023�N3������
 * �k�C�������w�Z�Q�[���N���G�C�^��
 * ���c�ˌ�
 */
/// <summary>
/// ���͊Ǘ��p�X�N���v�g
/// </summary>
public class InputManager : MonoBehaviour
{
    //�ϐ���--------------------------------------------------------------------------------------------------------------------------------
    #region �ϐ�
    [Header("�X�N���v�g�֘A")]
    private PlayerController _playerController;//�v���C���[�Ǘ��p�X�N���v�g
    private CameraController _cameraController;//�J�����Ǘ��p�X�N���v�g


    [Header("�f�o�b�N���A")]
    [SerializeField, Tooltip("�f�o�b�N���[�h")] private bool _isPlayerDebugMode;
    /// <summary>
    /// �Q�[���̃V�X�e�����[�h
    /// </summary>
    private enum SystemMode
    {
        [InspectorName("�ʏ탂�[�h")] NOMAL_MODE,
        [InspectorName("�f�o�b�N���[�h")] DEBUG_MODE,
    }
    private SystemMode _systemMode = default;


    [Header("���͊֘A")]
    [SerializeField, Tooltip("�f�b�g�]�[��"), Range(0.1f, 0.9f)] private float _deadZone;
    private string _h = "Horizontal";//���͖�
    private string _v = "Vertical";//���͖�


    [Header("Player���A")]
    private string _playerTag = "Player";//�v���C���[��tag��
    private Transform _playerTr = default;//�v���C���[��Transform


    [Header("Camera�֘A")]
    private string _camTag = "MainCamera";//�J������tag��
    #endregion


    //������--------------------------------------------------------------------------------------------------------------------------------

    private void Start()
    {

        //�R���|�[�l���g�擾
        GameObject playerObj = GameObject.FindGameObjectWithTag(_playerTag).gameObject;//Player�I�u�W�F�N�g
        if (!playerObj) { print("playerObj���擾�ł��܂���"); }
        else 
        {
            _playerController = playerObj.GetComponent<PlayerController>();//PlayerController�X�N���v�g
            _playerTr = playerObj.transform;//�v���C���[��Transform
        }
        _cameraController = GameObject.FindGameObjectWithTag(_camTag).GetComponent<CameraController>();//CameraController�̃X�N���v�g
    }

    private void Update()
    {

        //PlayerController��Null�Ȃ珈�����Ȃ�
        if (!_playerController) { print("PlayerController���擾�ł��܂���"); return; }

        //�f�o�b�N���[�h�̐؂�ւ�
        _systemMode = _isPlayerDebugMode ? SystemMode.DEBUG_MODE : SystemMode.NOMAL_MODE;

        switch (_systemMode)
        {
            case SystemMode.NOMAL_MODE:

                //�v���C���[�̓��͏���
                _playerController.PlayerInput(Mathf.Abs(Input.GetAxis(_h)) >= _deadZone, Input.GetKeyDown(KeyCode.W));
                break;


            case SystemMode.DEBUG_MODE:

                //�v���C���[�̃f�o�b�N����
                _playerController.PlayerDebugMode(Input.GetAxis(_h), Input.GetAxis(_v));
                break;


            default: print("SystemMode��case������܂���"); break;
        }
    }

    private void FixedUpdate()
    {

        //PlayerController��Null�Ȃ珈�����Ȃ�
        if (!_playerController) { print("PlayerController���擾�ł��܂���"); return; }

        switch (_systemMode)
        {
            case SystemMode.NOMAL_MODE:

                //�v���C���[�̈ړ�����
                _playerController.PlayerMove(Input.GetAxis(_h));
                break;


            case SystemMode.DEBUG_MODE: break;


            default: print("SystemMode��case������܂���"); break;
        }
    }

    private void LateUpdate()
    {

        if (!_cameraController) { print("CameraController���擾�ł��܂���"); return; }

        //�J�����̈ړ�����
        _cameraController.CameraMove(_playerTr);
    }
}
