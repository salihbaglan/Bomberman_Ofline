using UnityEngine;

public class Destructible : MonoBehaviour
{
    public float destructionTime = 1f; // Nesnenin yok edilmesi i�in gerekli s�re

    [Range(0f, 1f)]
    public float itemSpawnChance = 0.2f; // Yarat�klar�n yeni bir nesne yaratma olas�l���

    public GameObject[] spawnableItems; // Yarat�labilecek nesnelerin listesi

    private void Start()
    {
        // Nesnenin belirtilen s�re sonra yok edilmesi
        Destroy(gameObject, destructionTime);
    }

    private void OnDestroy()
    {
        // Yarat�klar�n yeni bir nesne yaratma olas�l���n� kontrol etme ve nesneyi yaratma
        if (spawnableItems.Length > 0 && Random.value < itemSpawnChance)
        {
            // Yarat�labilecek nesnelerden rastgele birini se�me
            int randomIndex = Random.Range(0, spawnableItems.Length);

            // Se�ilen nesneyi mevcut nesnenin konumunda ve varsay�lan d�n���mde yaratma
            Instantiate(spawnableItems[randomIndex], transform.position, Quaternion.identity);
        }
    }
}
