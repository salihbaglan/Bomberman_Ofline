using UnityEngine;

public class Explosion : MonoBehaviour
{
    public AnimatedSpriteRenderer start; // Patlama animasyonunun ba�lang�� karesini tutan sprite renderer
    public AnimatedSpriteRenderer middle; // Patlama animasyonunun orta karesini tutan sprite renderer
    public AnimatedSpriteRenderer end; // Patlama animasyonunun son karesini tutan sprite renderer

    public void SetActiveRenderer(AnimatedSpriteRenderer renderer)
    {
        start.enabled = renderer == start; // Ba�lang�� karesi sprite renderer'� aktifse, di�erlerini devre d��� b�rak�r
        middle.enabled = renderer == middle; // Orta kare sprite renderer'� aktifse, di�erlerini devre d��� b�rak�r
        end.enabled = renderer == end; // Son kare sprite renderer'� aktifse, di�erlerini devre d��� b�rak�r
    }

    public void SetDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x); // Y�n vekt�r�n�n a��s�n� hesaplar
        transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward); // Patlama objesinin rotasyonunu belirtilen y�ne ayarlar
    }

    public void DestroyAfter(float seconds)
    {
        Destroy(gameObject, seconds); // Belirtilen s�re sonunda patlama objesini yok eder
    }
}
