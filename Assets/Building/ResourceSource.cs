using UnityEngine;

public class ResourceSource : MonoBehaviour
{
    public enum ResourceType { Wood, Stone, Gold, Food }

    [Header("Cấu hình tài nguyên")]
    public ResourceType type;       // Loại tài nguyên (Chọn trong Dropdown ở Inspector)
    public int maxTrulung;    // Trữ lượng tối đa của mỏ
    public int currentTrulung;      // Trữ lượng hiện tại

    void Start()
    {
        currentTrulung = maxTrulung;
    }

    // Hàm gọi từ lính khi lính khai thác mỏ này
    public int Gather(int amount)
    {
        // Nếu mỏ sắp hết, chỉ cho phép lấy phần còn lại
        int gatherAmount = Mathf.Min(amount, currentTrulung);

        currentTrulung -= gatherAmount;

        // Nếu mỏ cạn kiệt tài nguyên -> Biến mất (Cây bị chặt hạ, mỏ đá bị san phẳng)
        if (currentTrulung <= 0)
        {
            Debug.Log(gameObject.name + " đã bị khai thác cạn kiệt!");
            Destroy(gameObject);
        }

        return gatherAmount; // Trả về số lượng thực tế lính lấy được
    }
}