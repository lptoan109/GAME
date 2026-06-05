using UnityEngine;

public class Building : MonoBehaviour
{
    [Header("Cấu hình cấp độ & Máu")]
    public string buildingName;
    public int currentLevel = 1; // Mặc định khởi đầu ở cấp 1
    public int maxLevel = 3;

    // Mảng chứa HP cho từng cấp (Cấp 1 = phần tử 0, Cấp 2 = phần tử 1, Cấp 3 = phần tử 2)
    public float[] hpPerLevel = new float[3] { 100f, 200f, 300f };

    [HideInInspector] public float currentHP;

    protected virtual void Start()
    {
        // Khởi tạo máu ban đầu dựa theo cấp độ hiện tại
        currentHP = hpPerLevel[Mathf.Clamp(currentLevel - 1, 0, maxLevel - 1)];
    }

    // Hàm nhận sát thương khi bị lính địch hoặc xe bắn đá công phá
    public virtual void TakeDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log($"{buildingName} (Cấp {currentLevel}) bị đánh! Máu còn: {currentHP}/{hpPerLevel[currentLevel - 1]}");

        if (currentHP <= 0)
        {
            OnBuildingDestroyed();
        }
    }

    // Hàm xử lý việc nâng cấp công trình
    public virtual bool UpgradeBuilding(int townCenterLevel)
    {
        if (currentLevel >= maxLevel)
        {
            Debug.LogWarning($"{buildingName} đã đạt cấp tối đa!");
            return false;
        }

        [cite_start]// Kiểm tra điều kiện: Cấp của nhà này không được vượt quá cấp Nhà chính 
        if (currentLevel + 1 > townCenterLevel)
        {
            Debug.LogError($"Không thể nâng cấp! Cấp độ của {buildingName} phải nhỏ hơn hoặc bằng cấp độ Nhà chính.");
            return false;
        }

        currentLevel++;
        // Hồi đầy máu theo mốc HP mới sau khi nâng cấp thành công
        currentHP = hpPerLevel[currentLevel - 1];
        Debug.Log($"Chúc mừng! {buildingName} đã nâng cấp lên Cấp {currentLevel}!");
        return true;
    }

    protected virtual void OnBuildingDestroyed()
    {
        Debug.Log($"{buildingName} đã bị phá hủy hoàn toàn!");
        Destroy(gameObject);
    }
}