using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Đây là file để  đánh dấu 1 object có được chọn hay không
public class SelecttableUnit : MonoBehaviour
{
    public bool isSelected = false;
    SpriteRenderer sRenderer;
    Vector3 startMousePos;
    Vector3 endMousepos;
    // Start is called before the first frame update
    void Start()
    {
        sRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if(Input.GetMouseButtonDown(1)) // Khi nhấp chuột phải
        {
            startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Lấy vị trí chuột khi bắt đầu nhấp
            startMousePos.z = 0; // Đặt z về 0 để làm việc trong 2D
        }
        if(Input.GetMouseButtonUp(1)) // Khi thả chuột phải
        {
            endMousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Lấy vị trí chuột khi thả
            endMousepos.z = 0; // Đặt z về 0 để làm việc trong 2D
            CheckIfSelected(); // Kiểm tra xem object có được chọn hay không
        }
    }
    void CheckIfSelected()
    {
        // Tạo một vùng hình chữ nhật từ vị trí bắt đầu đến vị trí kết thúc của chuột
        Rect selectionRect = new Rect(
            Mathf.Min(startMousePos.x, endMousepos.x),
            Mathf.Min(startMousePos.y, endMousepos.y),
            Mathf.Abs(startMousePos.x - endMousepos.x),
            Mathf.Abs(startMousePos.y - endMousepos.y)
        );
        // Kiểm tra nếu vị trí của object nằm trong vùng chọn
        if (selectionRect.Contains(transform.position))
        {
            isSelected = true; // Đánh dấu object này là được chọn
            sRenderer.color = Color.green; // Thay đổi màu sắc để dễ nhận biết
        }
        else
        {
            isSelected = false; // Đánh dấu object này là không được chọn
            sRenderer.color = Color.white; // Trả về màu sắc ban đầu
        }
    }
    public void Setselect(bool status)
    {
        isSelected = status; // Đánh dấu object này là không được chọn
        sRenderer.color = isSelected?Color.green:Color.white; // Trả về màu sắc ban đầu
    }
    public bool isUnitSelected()
    {
        return isSelected; // Trả về trạng thái được chọn của object này
    }
}
