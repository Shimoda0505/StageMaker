using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



/*
 * 2023�N3������
 * �k�C�������w�Z�Q�[���N���G�C�^��
 * ���c�ˌ�
 */
/// <summary>
/// �����I�u�W�F�N�g�̃M�~�b�N�p�X�N���v�g
/// </summary>
public class GimmickFallBox : MonoBehaviour
{

    [Header("�I�u�W�F�N�g�֘A")]
    [SerializeField, Tooltip("���Ƃ��I�u�W�F�N�g")] private GameObject _fallObj;
    private BoxCollider2D _objCollider;//���Ƃ��I�u�W�F�N�g��Collider
    private float _fallObjSize = 0;//���Ƃ��I�u�W�F�N�g�̕ω��T�C�Y
    private float _fallObjPos = default;
    private float _defaultSize = default;//���Ƃ��I�u�W�F�N�g�̏����T�C�Y
    private float _defaultPos = default;//���Ƃ��I�u�W�F�N�g�̏����ʒu

    [Header("���x�֘A")]
    [SerializeField, Tooltip("�������x"), Range(0.1f, 5f)] private float _fallSpeed;
    [SerializeField, Tooltip("���Ƃ��I�u�W�F�N�g�̑召���鑬�x"), Range(0.1f, 5f)] private float _changeSizeSpeed;


    [Header("���Ԋ֘A")]
    [SerializeField, Tooltip("���Ƃ��Ԋu"), Range(0.1f, 5f)] private float _fallIntervalTime;
    private float _fallIntervalCount = 0;//���Ƃ��Ԋu�̌v��


    [Header("RayCast�֘A")]
    private const float _rayDistance = 0.55f;//����
    private string _stageName = "Stage";//Stage�^�O��
    private int _stageLayerMask = default;//Stage���C���[�̔Ԓn


    /// <summary>
    /// ���Enum
    /// </summary>
    private enum MotionEnum
    {
        [InspectorName("�ҋ@")]IDLE,
        [InspectorName("����")]MAKE,
        [InspectorName("�ړ�")]MOVE,
        [InspectorName("��~")]STOP,
    }
    private MotionEnum _motionEnum = MotionEnum.IDLE;



    //������--------------------------------------------------------------------------------------------------------------------------------

    private void Start()
    {
        //���Ƃ��I�u�W�F�N�g
        _defaultSize = _fallObj.transform.localScale.x;//�����T�C�Y��⊮
        _defaultPos = _fallObj.transform.localPosition.y;//�����ʒu
        _fallObjPos = _defaultPos;//�����ʒu�̕���
        _fallObj.transform.localScale = new Vector3(0, 0, 1);//�T�C�Y��������

        //Collider�̏�����
        _objCollider = _fallObj.GetComponent<BoxCollider2D>();//�擾
        _objCollider.enabled = false;//��A�N�e�B�u

        //���C���[�̊Ǘ��ԍ����擾���}�X�N�ւ̕ϊ�
        _stageLayerMask = 1 << LayerMask.NameToLayer(_stageName);
    }

    private void Update()
    {

        switch (_motionEnum)
        {
            case MotionEnum.IDLE:

                //���Ԍv���㐶���J�n
                _fallIntervalCount += Time.deltaTime;
                if(_fallIntervalCount >= _fallIntervalTime) { _fallIntervalCount = 0; _motionEnum = MotionEnum.MAKE; }
                break;


            case MotionEnum.MAKE:

                //�T�C�Y�X�V
                _fallObjSize += Time.deltaTime * _changeSizeSpeed;
                //Collider���A�N�e�B�u
                if(_fallObjSize >= _defaultSize)
                {

                    //�T�C�Y��default�ɕύX
                    _fallObjSize = _defaultSize;

                    //Collider���A�N�e�B�u
                    _objCollider.enabled = true;
                    _motionEnum = MotionEnum.MOVE; 
                }
                break;


            case MotionEnum.MOVE:

                //����
                _fallObjPos -= Time.deltaTime * _fallSpeed;

                //RayCast�̐ڐG����̏����Atrue�ňړ��I��
                if (RayCast(_fallObj.transform.position)) { _motionEnum = MotionEnum.STOP; }
                break;


            case MotionEnum.STOP:

                //�T�C�Y�X�V
                _fallObjSize -= Time.deltaTime * _changeSizeSpeed;
                if (_fallObjSize <= 0)
                {

                    //����k��
                    _fallObjSize = 0;//�T�C�Y
                    _fallObjPos = _defaultPos;//���W

                    //�����ʒu�Ɉړ�
                    //Collider���A�N�e�B�u
                    _objCollider.enabled = false;
                    _motionEnum = MotionEnum.IDLE;
                }
                break;


            default:print("_motionEnum��case�Ȃ�"); break;
        }

        //�T�C�Y
        _fallObj.transform.localScale = new Vector3(_fallObjSize, _fallObjSize, 1);
        //���W
        _fallObj.transform.localPosition = new Vector3(_fallObj.transform.localPosition.x, _fallObjPos, _fallObj.transform.localPosition.z);
    }



    //���\�b�h��--------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// RayCast�̐ڐG����̏���
    /// </summary>
    /// <param name="rayPos">RayCast�̌��_</param>
    /// <returns>�n�ʂ̐ڐG,����2�����ȏ�ŕǂƐڐG</returns>
    private bool RayCast(Vector2 rayPos)
    {

        //RayCast���e�����ɔ�΂�
        RaycastHit2D hitDownRight = Physics2D.Raycast(rayPos, Vector2.down, _rayDistance, _stageLayerMask);
        //RayCast�̉���
        Debug.DrawRay(rayPos, Vector2.down * _rayDistance, Color.red);
        return (hitDownRight);
    }

}
