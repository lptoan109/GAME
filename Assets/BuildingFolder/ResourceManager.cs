using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("Kho tài nguyên khởi đầu")]
    public int wood;
    public int stone;
    public int gold;
    public int food;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Hàm cộng tài nguyên: dùng khi lính mang tài nguyên về nhà chính
    // type truyền vào sẽ là: "Wood", "Stone", "Gold", hoặc "Food"
    public void AddResource(string type, int amount)
    {
        switch (type)
        {
            case "Wood": wood += amount; break;
            case "Stone": stone += amount; break;
            case "Gold": gold += amount; break;
            case "Food": food += amount; break;
            default: Debug.LogWarning("Loại tài nguyên không hợp lệ: " + type); return;
        }

        LogResources();
    }

    // Hàm kiểm tra và trừ tài nguyên khi xây dựng hoặc mua quân
    public bool SpendResources(int woodCost, int stoneCost, int goldCost, int foodCost)
    {
        // Kiểm tra xem người chơi có đủ cả 4 loại tài nguyên không
        if (wood >= woodCost && stone >= stoneCost && gold >= goldCost && food >= foodCost)
        {
            wood -= woodCost;
            stone -= stoneCost;
            gold -= goldCost;
            food -= foodCost;

            Debug.Log("--- ĐÃ CHI TIÊU THÀNH CÔNG ---");
            LogResources();
            return true; // Đủ tiền, cho phép thực hiện
        }

        Debug.LogWarning("Không đủ tài nguyên để thực hiện hành động này!");
        return false; // Thiếu tiền, từ chối lệnh
    }

    // Hàm in ra màn hình Console để bạn dễ theo dõi trạng thái kho
    void LogResources()
    {
        Debug.Log($"[KHO] 🪵 Gỗ: {wood} | 🪨 Đá: {stone} | 🪙 Vàng: {gold} | 🌾 Lương thực: {food}");
    }
}