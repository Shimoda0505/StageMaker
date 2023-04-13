using UnityEngine;



/*����T�v
 * 2023�N3������
 * �k�C�������w�Z�Q�[���N���G�C�^��
 * ���c�ˌ�
 */
/// <summary>
/// �J�����Ǘ��p�X�N���v�g
/// </summary>
public class CameraController : MonoBehaviour
{
    //�ϐ���--------------------------------------------------------------------------------------------------------------------------------
    [Header("Camera���A")]
    [SerializeField, Tooltip("�Ǐ]���x"), Range(1, 10)] private int _camSpeed;
    private Vector3 _offSetPos = new Vector3(0, 0, -10);//�I�t�Z�b�g���W



    //���\�b�h��--------------------------------------------------------------------------------------------------------------------------------
    //�p�u���b�N���\�b�h--------------------------------------------------------------------
    /*[ InputManager�Ŏg�p ]*/
    /// <summary>
    /// �J�����̈ړ�
    /// </summary>
    /// <param name="playerTr">�v���C���[��Transform</param>
    public void CameraMove(Transform playerTr)
    {
        //�J�����̍��W���v���C���[�ɒǏ]
        this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, playerTr.position + _offSetPos, Time.deltaTime * _camSpeed);
    }
}
