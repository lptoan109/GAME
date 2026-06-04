using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// File này có nhiệm vụ dẫn 1 object được chọn đến 1 vị trí được click trên grid, và di chuyển theo đường thẳng đến đó
public class GridMovement : MonoBehaviour
{
    public float speed = 5f; // Tốc độ di chuyển
    Vector3 targetPosition; // Vị trí đích mà object sẽ di chuyển đến
    bool isMoving = false; // Biến để kiểm tra xem object có đang di chuyển hay không
    Grid gridlayout; // Tham chiếu đến Grid layout trong scene
    SelecttableUnit selectionComponent; // Tham chiếu đến component SelecttableUnit để kiểm tra xem object có được chọn hay không 
    // Start is called before the first frame update
    void Start()
    {
        gridlayout = FindObjectOfType<Grid>(); // Tìm đối tượng Grid trong scene và gán vào biến gridlayout
        selectionComponent = GetComponent<SelecttableUnit>(); // Lấy component SelecttableUnit của object này
        if (gridlayout == null)// Kiểm tra nếu không tìm thấy Grid
        {
            Debug.LogError("Errol: Can not find grid.");
        }
        targetPosition = transform.position; // Khởi tạo vị trí đích bằng vị trí hiện tại của object
    }

    // Update is called once per frame
    void Update()
    {
        if(selectionComponent == null || !selectionComponent.isSelected) // Kiểm tra nếu object này không được chọn
        {
            HandleActualMovement(); // Tiếp tục di chuyển hết lệnh
            return; // Dừng chương trình
        }
        if (Input.GetMouseButtonDown(0)) // Kiểm tra nếu người chơi nhấp chuột trái
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Lấy vị trí chuột trong thế giới
            Vector3Int cellPos = gridlayout.WorldToCell(mouseWorldPos); // Chuyển vị trí chuột sang vị trí ô trên grid
            targetPosition = gridlayout.GetCellCenterWorld(cellPos); // Lấy vị trí trung tâm của ô đó để làm đích đến
            isMoving = true; // Bật cờ di chuyển
        }
        HandleActualMovement(); // Gọi hàm để xử lý di chuyển đến vị trí đích nếu có lệnh di chuyển
    }
    void HandleActualMovement()
    {
        if (isMoving) // Kiểm tra nếu đang di chuyển
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime); // Di chuyển object đến vị trí đích với tốc độ đã định
            if (Vector3.Distance(transform.position, targetPosition) < 0.001f) // Kiểm tra nếu đã đến gần vị trí đích
            {
                isMoving = false; // Tắt cờ di chuyển
                selectionComponent.Setselect(false); // Sau khi đến đích, bỏ chọn object
            }
        }
    }
    public void StopMoving()
    {
        isMoving = false; // Tắt cờ di chuyển
    }
    public void SetNewTarget(Vector3 newTargetPos)
    { 
        targetPosition = newTargetPos;
        isMoving = true; // Kích hoạt cờ di chuyển
    }
}
