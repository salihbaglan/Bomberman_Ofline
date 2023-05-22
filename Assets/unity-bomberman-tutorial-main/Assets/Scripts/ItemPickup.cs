using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public enum ItemType
    {
        ExtraBomb, // Ekstra bomba almak i�in kullan�lan ��e
        BlastRadius, // Patlama yar��ap�n� art�rmak i�in kullan�lan ��e
        SpeedIncrease, // H�z� art�rmak i�in kullan�lan ��e
        PushItem, // �temleri itmek i�in kullan�lan ��e
        MultiBomb, // Birden fazla bomba koyabilmek i�in kullan�lan ��e
        Ghost, // Hayalet moduna ge�mek i�in kullan�lan ��e
        KeyItem, // Anahtar ��esi
        MaxRadius, // Maksimum patlama yar��ap�n� ayarlamak i�in kullan�lan ��e
        isActiveBombControl, // Patlama d��mesini aktif hale getiren ��e
        luckItem,
        Count,
        None,
    }
    private List<ItemType> availableItemTypes = new List<ItemType> { ItemType.SpeedIncrease, ItemType.MultiBomb, ItemType.Ghost };
    private ItemType lastSelectedItemType = ItemType.None;

    public ItemType type; // ��enin tipi

    private void OnItemPickup(GameObject player)
    {
        if (player == null)
        {
            return;
        }

        switch (type)
        {
            case ItemType.ExtraBomb:
                player.GetComponent<BombController>().AddBomb(); // Ekstra bomba ekleyen fonksiyonu �a��r�r
                break;

            case ItemType.BlastRadius:
                player.GetComponent<BombController>().ExplosionRadius(); // Patlama yar��ap�n� art�ran fonksiyonu �a��r�r
                break;

            case ItemType.SpeedIncrease:
                player.GetComponent<MovementController>().SpeedItem(); // H�z� art�ran fonksiyonu �a��r�r
                break;

            case ItemType.PushItem:
                player.GetComponent<BombController>().Push(); // �temleri iten fonksiyonu �a��r�r
                break;

            case ItemType.MultiBomb:
                player.GetComponent<BombController>().OnItemEaten(); // Birden fazla bomba koyan fonksiyonu �a��r�r
                break;

            case ItemType.Ghost:
                player.GetComponent<MovementController>().Ghost(); // Hayalet moduna ge�en fonksiyonu �a��r�r
                break;

            case ItemType.KeyItem:
                player.GetComponent<MovementController>().Key�tem(); // Anahtar ��esini alan fonksiyonu �a��r�r
                break;

            case ItemType.MaxRadius:
                player.GetComponent<BombController>().MaxExplosionRadius(); // Maksimum patlama yar��ap�n� ayarlayan fonksiyonu �a��r�r
                break;

            case ItemType.isActiveBombControl:
                player.GetComponent<BombController>().ExplosionButtonItem(); // Patlama d��mesini aktif hale getiren fonksiyonu �a��r�r
                UIManager.Instance.SetExplotionButtonState(true);
                break;

            case ItemType.luckItem:
                GameObject playerObject = player;// Burada oyuncunun referans�n� alman�z gerekiyor
                SelectRandomItem(playerObject);
                break;
        }

        Destroy(gameObject); // ��eyi yok eder
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnItemPickup(other.gameObject); // Oyuncu temas etti�inde ��eyi toplar
        }
  
    }

    // �nceki kodun devam�...

    public void SelectRandomItem(GameObject player)
    {
        if (availableItemTypes.Count == 0)
        {
            Debug.Log("T�m ��eler t�ketildi. Liste yeniden ba�lat�l�yor.");
            ResetAvailableItems();
        }

        ItemType randomItemType;
        do
        {
            randomItemType = availableItemTypes[Random.Range(0, availableItemTypes.Count)];
        } while (randomItemType == lastSelectedItemType);

        // Se�ilen ��e t�r�n� listeden ��kar
        availableItemTypes.Remove(randomItemType);
        lastSelectedItemType = randomItemType;

        // Se�ilen ��e t�r�ne g�re ilgili fonksiyonu �a��r
        switch (randomItemType)
        {
            case ItemType.SpeedIncrease:
                player.GetComponent<MovementController>().SpeedItem();
                Debug.Log("1");
                break;

            case ItemType.MultiBomb:
                player.GetComponent<BombController>().OnItemEaten();
                Debug.Log("2");
                break;

            case ItemType.Ghost:
                player.GetComponent<MovementController>().Ghost();
                Debug.Log("3");
                break;
        }
    }

    private void ResetAvailableItems()
    {
        availableItemTypes.Clear();
        availableItemTypes.AddRange(new ItemType[] { ItemType.SpeedIncrease, ItemType.MultiBomb, ItemType.Ghost });
        lastSelectedItemType = ItemType.None;
    }



}
