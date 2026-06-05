using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarracksBuilding : Building
{
    public enum BarracksType { Infantry, Archery, Cavalry, Siege }

    [Header("Cấu hình loại nhà lính")]
    public BarracksType barracksType;
    public GameObject unitPrefab; // Prefab con lính tương ứng muốn sinh ra [cite: 69]
    public float trainTime = 3f;  // Thời gian đẻ 1 con lính

    private Queue<GameObject> trainingQueue = new Queue<GameObject>();
    private bool isTraining = false;

    protected override void Start()
    {
        [cite_start]// GDD: HP các nhà lính đồng đều là 350 ở cấp khởi đầu 
        hpPerLevel = new float[3] { 350f, 500f, 700f };
        buildingName = "Nhà lính " + barracksType.ToString();
        base.Start();
    }

    // Hàm bấm từ nút UI để đặt lệnh đẻ lính
    public void EnqueueUnit()
    {
        // Thực tế cần check thêm tiền của ResourceManager ở đây trước khi nạp vào Queue
        trainingQueue.Enqueue(unitPrefab);
        Debug.Log($"{buildingName}: Đã thêm 1 quân vào hàng đợi sản xuất ({trainingQueue.Count} đang chờ).");

        if (!isTraining)
        {
            StartCoroutine(TrainUnitRoutine());
        }
    }

    IEnumerator TrainUnitRoutine()
    {
        isTraining = true;

        while (trainingQueue.Count > 0)
        {
            yield return new WaitForSeconds(trainTime);

            // Lấy lính ra khỏi hàng đợi sau khi đếm ngược xong
            GameObject unitToSpawn = trainingQueue.Dequeue();

            // Sinh lính xuất hiện ngay cạnh vị trí nhà lính lệch sang một chút (X + 2)
            Vector3 spawnPos = transform.position + new Vector3(2f, 0f, 0f);
            Instantiate(unitToSpawn, spawnPos, Quaternion.identity);

            Debug.Log($"{buildingName}: 1 đơn vị quân đã bước ra chiến trường!");
        }

        isTraining = false;
    }
}