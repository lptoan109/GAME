using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    [Header("Cấu hình Prefab công trình")]
    public GameObject buildingPrefab;  // File Prefab nhà thật để sinh ra khi đủ tiền
    public GameObject previewPrefab;   // File bóng ma tòa nhà (màu mờ/trong suốt) để xem trước

    [Header("Chi phí xây dựng (Bộ 4 tài nguyên)")]
    public int woodCost;         // Chi phí Gỗ
    public int stoneCost;         // Chi phí Đá
    public int goldCost;           // Chi phí Vàng
    public int foodCost;           // Chi phí Lương thực

    private GameObject currentPreview; // Lưu trữ cái bóng ma đang bám theo chuột
    private Grid gridLayout;           // Tham chiếu đến Grid layout trong Scene
    private bool isPlacing = false;    // Cờ kiểm tra xem có đang trong chế độ chọn vị trí xây không

    void Start()
    {
        // Tự động tìm đối tượng Grid có trên Map khi game bắt đầu
        gridLayout = FindObjectOfType<Grid>();

        if (gridLayout == null)
        {
            Debug.LogError("Không tìm thấy đối tượng Grid nào trong Scene! Vui lòng tạo một Grid.");
        }
    }

    void Update()
    {
        // 1. NGƯỜI CHƠI NHẤN PHÍM 'B': Bật chế độ xây nhà (B = Build)
        if (Input.GetKeyDown(KeyCode.B) && !isPlacing)
        {
            StartPlacement();
        }

        // 2. LOGIC KHI ĐANG TRONG CHẾ ĐỘ CHỌN VỊ TRÍ XÂY
        if (isPlacing && currentPreview != null)
        {
            // Lấy vị trí của chuột trong thế giới 2D
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Ép vị trí chuột về tọa độ ô lưới (Cell) trên Grid
            Vector3Int cellPos = gridLayout.WorldToCell(mousePos);

            // Lấy vị trí tâm (Center) của ô lưới đó để đặt nhà cho thẳng hàng
            Vector3 gridPos = gridLayout.GetCellCenterWorld(cellPos);
            gridPos.z = 0f; // Khóa trục Z về 0 để làm việc chuẩn trong môi trường 2D

            // Cập nhật vị trí của bóng ma tòa nhà liên tục bám theo ô lưới của chuột
            currentPreview.transform.position = gridPos;

            // 3. NGƯỜI CHƠI CLICK CHUỘT TRÁI (0): Chốt hạ xây dựng tại ô đang chọn
            if (Input.GetMouseButtonDown(0))
            {
                PlaceBuilding(gridPos);
            }

            // 4. NGƯỜI CHƠI CLICK CHUỘT PHẢI (1): Hủy bỏ lệnh xây dựng hiện tại
            if (Input.GetMouseButtonDown(1))
            {
                CancelPlacement();
            }
        }
    }

    // Hàm bắt đầu kích hoạt chế độ xem trước vị trí xây nhà
    void StartPlacement()
    {
        if (previewPrefab == null)
        {
            Debug.LogError("Chưa kéo thả Preview Prefab vào script BuildingSystem!");
            return;
        }

        isPlacing = true;
        // Tạo ra bóng ma tòa nhà tại vị trí chuột hiện tại
        currentPreview = Instantiate(previewPrefab);
    }

    // Hàm chốt hạ xây nhà thật và trừ tiền
    void PlaceBuilding(Vector3 position)
    {
        if (ResourceManager.Instance == null)
        {
            Debug.LogError("Không tìm thấy ResourceManager trong Scene!");
            return;
        }

        // Gọi tổng đài ResourceManager kiểm tra xem người chơi có đủ cả 4 loại tiền không
        if (ResourceManager.Instance.SpendResources(woodCost, stoneCost, goldCost, foodCost))
        {
            if (buildingPrefab == null)
            {
                Debug.LogError("Chưa kéo thả Building Prefab vào script BuildingSystem!");
                return;
            }

            // ĐỦ TÀI NGUYÊN -> Sinh ra tòa nhà thật vững chắc tại ô lưới đã chọn
            Instantiate(buildingPrefab, position, Quaternion.identity);

            // Xóa cái bóng ma xem trước đi để dọn dẹp bộ nhớ
            Destroy(currentPreview);
            isPlacing = false;

            Debug.Log("Xây dựng công trình thành công!");
        }
        else
        {
            // THIẾU TÀI NGUYÊN -> Báo lỗi và hủy lệnh đặt nhà
            Debug.LogWarning("Xây nhà thất bại do thiếu tài nguyên!");
            CancelPlacement();
        }
    }

    // Hàm hủy bỏ lệnh xây dựng, trả lại trạng thái bình thường
    void CancelPlacement()
    {
        if (currentPreview != null)
        {
            Destroy(currentPreview);
        }
        isPlacing = false;
        Debug.Log("Đã hủy chế độ xây dựng.");
    }
}