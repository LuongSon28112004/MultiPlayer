using Unity.Netcode;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;

    public static InputManager Instance { get => instance; }

    private bool isKeyE;
    public bool IsKeyE { get => isKeyE; }
    float horizonrtalInput;
    [SerializeField] public float HorizonrtalInput { get => horizonrtalInput; }
    float verticalInput;
    public float VerticalInput { get => verticalInput; }
    private bool isSpace;
    public bool IsSpace { get => isSpace; }

    void Start()
    {
        if (instance != null) Debug.Log("Khong the ton tai 2 inputmanager");
        else instance = this;
    }

    void Update()
    {
        //if(!IsOwner) return; // Chỉ cho phép InputManager hoạt động trên client sở hữu nó
        this.getKey();
        this.getKeyUp();
        this.getHorizontal();
        this.getVertical();
        this.getIsSpace();
    }

    private void getKey()
    {
        if (Input.GetKey(KeyCode.E))
        {
            isKeyE = true;
        }
    }

    private void getKeyUp()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            isKeyE = false;
        }
    }

    private void getHorizontal()
    {
        horizonrtalInput = Input.GetAxisRaw("Horizontal");
    }

    private void getVertical()
    {
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void getIsSpace()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isSpace = true;
        }
        else
        {
            isSpace = false; // Reset ngay sau frame đó
        }
    }

}
