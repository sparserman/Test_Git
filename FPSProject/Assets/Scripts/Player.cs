using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float walkSpeed;    // �̵� �ӵ�
    public float jumpForce;    // ���� ����
    public float ShotSpeed = 0.2f;    // ���� �ӵ�

    private bool isGround = true;   // �ٴڿ� �ִ� �� üũ

    // �ٴ� üũ��
    private CapsuleCollider capsuleCollider;

    public float lookSensitivity;  // ī�޶� �ΰ���

    public float cameraRotationLimit;  // ī�޶� �ִ� ����
    private float currentCameraRotationX = 0;   // ���� ī�޶� ����

    public Camera theCamera;   // ī�޶�

    private Rigidbody myRigid;

    public int numberOfBullet;     // ���� �Ѿ� ��

    public AudioClip currentSound;  // �߻� �Ҹ�

    private AudioSource audioSource;

    private RaycastHit hitinfo;     // ���� ���� ����

    public GameObject hitEffect;  // ��ƼŬ

    public Text UIBullet;   // �Ѿ� �� UI

    public GameObject grenadeObj;   // ����ź
    public GameObject grenadePos;   // �߻���ġ

    void Start()
    {
        myRigid = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        IsGround();
        Move();
        CameraRotation();
        CharacterRotation();
        TryJump();
        Shot();
        Reload();
        UpdateUIBullet();
        GrenadeShot();
    }

    public float grenadePower;
    private void GrenadeShot()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameObject copyGrenade = Instantiate(grenadeObj);
            copyGrenade.SetActive(true);

            Vector3 temppos = new Vector3();
            temppos.x = grenadePos.transform.position.x;
            temppos.y = grenadePos.transform.position.y;
            temppos.z = grenadePos.transform.position.z;
            copyGrenade.transform.position = temppos;

            Rigidbody rigidGrenade = copyGrenade.GetComponent<Rigidbody>();
            rigidGrenade.AddForce(grenadePos.transform.up * grenadePower, ForceMode.Impulse);
            rigidGrenade.AddTorque(grenadePos.transform.up * -10, ForceMode.Impulse);
        }
    }
    
    private void UpdateUIBullet()
    {
        UIBullet.text = numberOfBullet.ToString();
    }

    private void Reload()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            numberOfBullet = 30;
        }
    }

    private void Hit()
    {
        if(Physics.Raycast(theCamera.transform.position, theCamera.transform.forward, out hitinfo))
        {
            GameObject copyeffect = Instantiate(hitEffect, hitinfo.point, Quaternion.LookRotation(hitinfo.normal));
            GameObject.Destroy(copyeffect, 1f);
        }
    }

    private float m_CurrentTime = 0;

    public float recoil;    // �ݵ���
    public float recoilplus;    // �ݵ� ������
    private void Shot()
    {
        if (numberOfBullet != 0)
        {
            if(Input.GetMouseButton(0))
            {
                if (Time.time >= m_CurrentTime)
                {
                    m_CurrentTime = Time.time + ShotSpeed;
                    PlaySE(currentSound);
                    Fire();
                }
            }
        }
    }

    private void Fire()
    {
        Hit();
        numberOfBullet--;   // �Ѿ� �Ҹ�
        recoil += recoilplus;   // �ݵ� ����
    }

    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
    }

    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
        }
    }

    private void Jump()
    {
        myRigid.velocity = transform.up * jumpForce;
    }

    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * walkSpeed;

        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    private void CameraRotation()
    {
        // ���� ī�޶� ȸ��
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;

        // ���� �������� -�Ѱ谪 ~ �Ѱ谪 ���̿� ���α�
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX - recoil, 0f, 0f);
    }

    private void CharacterRotation()
    {
        // �¿� ĳ���� ȸ��
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
    }

    private void PlaySE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }
}
