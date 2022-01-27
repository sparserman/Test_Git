using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float walkSpeed;    // 이동 속도
    public float jumpForce;    // 점프 높이
    public float ShotSpeed = 0.2f;    // 연사 속도

    private bool isGround = true;   // 바닥에 있는 지 체크

    // 바닥 체크용
    private CapsuleCollider capsuleCollider;

    public float lookSensitivity;  // 카메라 민감도

    public float cameraRotationLimit;  // 카메라 최대 각도
    private float currentCameraRotationX = 0;   // 현재 카메라 방향

    public Camera theCamera;   // 카메라

    private Rigidbody myRigid;

    public int numberOfBullet;     // 남은 총알 수

    public AudioClip currentSound;  // 발사 소리

    private AudioSource audioSource;

    private RaycastHit hitinfo;     // 맞은 지점 정보

    public GameObject hitEffect;  // 파티클

    public Text UIBullet;   // 총알 수 UI

    public GameObject grenadeObj;   // 수류탄
    public GameObject grenadePos;   // 발사위치

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

    public float recoil;    // 반동량
    public float recoilplus;    // 반동 증가량
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
        numberOfBullet--;   // 총알 소모
        recoil += recoilplus;   // 반동 증가
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
        // 상하 카메라 회전
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;

        // 현재 각도값이 -한계값 ~ 한계값 사이에 가두기
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX - recoil, 0f, 0f);
    }

    private void CharacterRotation()
    {
        // 좌우 캐릭터 회전
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
