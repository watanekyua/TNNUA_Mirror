﻿using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class HoriCameraController : MonoBehaviour
{
    [Header("UI Ref")]
    public CanvasGroup PNL_Option;
    public Text TXT_Pos;
    public Text TXT_Rot;
    public Button ResetCam;

    [Header("Control Param")]

    [SerializeField]
    private float lookSpeedH = 2f;

    [SerializeField]
    private float lookSpeedV = 2f;

    [SerializeField]
    private float zoomSpeed = 2f;

    [SerializeField]
    private float dragSpeed = 3f;

    float yaw = 0f;
    float pitch = 0f;

    [SerializeField]
    Vector3 defaultPos =  new Vector3(0, 1, -10);

    [SerializeField]
    Vector3 defaultElur = new Vector3(10, 0, 0);

    Quaternion defaultRot;

    public void Start()
    {
        defaultRot = Quaternion.Euler(defaultElur);

        transform.localPosition = SystemConfig.Instance.GetData<Vector3>("Cam_Pos", defaultPos);
        transform.localRotation = SystemConfig.Instance.GetData<Quaternion>("Cam_Rot", defaultRot);

        // Initialize the correct initial rotation
        this.yaw = this.transform.localEulerAngles.y;
        this.pitch = this.transform.localEulerAngles.x;

        SaveTransform();

        ResetCam.onClick.AddListener(delegate {
            transform.localPosition = defaultPos;
            transform.localRotation = defaultRot;
        });
    }

    async void SaveTransform(){
        await Task.Delay(1000);
        while(true){
            SystemConfig.Instance.SaveData("Cam_Pos", transform.localPosition);
            SystemConfig.Instance.SaveData("Cam_Rot", transform.localRotation);
            await Task.Delay(1000);

            if(this == null)
                break;
        }
    }

    private void Update()
    {
        // Only work not in Setting Panel
        if (PNL_Option.blocksRaycasts && Input.GetKey(KeyCode.LeftAlt))
        {
            //Look around with Left Mouse
            if (Input.GetMouseButton(0))
            {
                this.yaw += this.lookSpeedH * Input.GetAxis("Mouse X");
                this.pitch -= this.lookSpeedV * Input.GetAxis("Mouse Y");

                //this.transform.eulerAngles = new Vector3(this.pitch, this.yaw, 0f);
                //Lock X
                this.transform.localEulerAngles = new Vector3(this.pitch, 0f, 0f);
            }

            //drag camera around with Middle Mouse
            if (Input.GetMouseButton(2))
            {
                //transform.Translate(-Input.GetAxisRaw("Mouse X") * Time.deltaTime * dragSpeed, -Input.GetAxisRaw("Mouse Y") * Time.deltaTime * dragSpeed, 0);
                //Lock X
                transform.Translate(0, -Input.GetAxisRaw("Mouse Y") * Time.deltaTime * dragSpeed, 0);
            }

            if (Input.GetMouseButton(1))
            {
                //Zoom in and out with Right Mouse
                this.transform.Translate(0, 0, Input.GetAxisRaw("Mouse X") * this.zoomSpeed * .07f, Space.Self);
            }

            //Zoom in and out with Mouse Wheel
            this.transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * this.zoomSpeed, Space.Self);

            TXT_Pos.text = transform.localPosition.ToString();
            TXT_Rot.text = transform.localEulerAngles.ToString();
        }
    }
}