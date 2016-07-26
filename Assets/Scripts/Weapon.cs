using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

    public float fireRate = 0;
    public int damage = 10;
    public LayerMask whatToHit;

    public Transform BulletTrailPrefab;
    public Transform HitPrefab;
    public Transform MuzzleFlashPrefab;
    float timeToSpawnEffect = 0;
    public float effectSpawnRate = 10;

    float timeToFire = 0;
    Transform firePoint;

	// Use this for initialization
	void Awake () {
        firePoint = transform.FindChild("FirePoint");

        if (firePoint == null) {
            Debug.LogError("No firePoint? WHAT?!");
        }
	}  
	
	// Update is called once per frame
	void Update () {
       if (fireRate == 0) {
            if (Input.GetButtonDown("Fire1")) {
                Shoot();
            }
        }

        else {
            if (Input.GetButton("Fire1") && Time.time > timeToFire) {
                timeToFire = Time.time + 1 / fireRate;
                Shoot();
            }
        }
	}

    void Shoot () {
        Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);
        RaycastHit2D hit = Physics2D.Raycast(firePointPosition, mousePosition - firePointPosition,100,whatToHit);
       
        Debug.DrawLine(firePointPosition, (mousePosition-firePointPosition) * 100, Color.cyan);

        if (hit.collider != null) {
            Debug.DrawLine(firePointPosition, hit.point, Color.red);
            EnemyStatus enemy = hit.collider.GetComponent<EnemyStatus>();

            if (enemy != null) {
                enemy.DamageEnemy(damage);
                // Debug.Log("We hit " + hit.collider.name + " and did " + damage + " damage.");
            }
        }

        if (Time.time >= timeToSpawnEffect) {
            Vector3 hitPosition;
            Vector3 hitNormal;

            if (hit.collider == null) {
                hitPosition = (mousePosition - firePointPosition) * 30;
                hitNormal = new Vector3(9999, 9999, 9999);
            }

            else {
                hitPosition = hit.point;
                hitNormal = hit.normal;
            }
            Effect(hitPosition, hitNormal);
            timeToSpawnEffect = Time.time + 1 / effectSpawnRate;
        }
    }

    void Effect (Vector3 hitPosition, Vector3 hitNormal) {
        Transform trail = Instantiate(BulletTrailPrefab, firePoint.position, firePoint.rotation) as Transform;
        LineRenderer lineRendererVariable = trail.GetComponent<LineRenderer>();

        if (lineRendererVariable != null) {
            lineRendererVariable.SetPosition(0, firePoint.position);
            lineRendererVariable.SetPosition(1, hitPosition);
        }

        Destroy(trail.gameObject, 0.04f);
        if (hitNormal != new Vector3(9999, 9999, 9999)) {
            Transform hitParticle = Instantiate(HitPrefab, hitPosition, Quaternion.FromToRotation(Vector3.right, hitNormal)) as Transform;
            Destroy(hitParticle.gameObject, 1f);
        }

        Transform clone = Instantiate(MuzzleFlashPrefab, firePoint.position, firePoint.rotation) as Transform;
        clone.parent = firePoint;
        float size = Random.Range(0.6f, 0.9f);
        clone.localScale = new Vector3(size, size, size);
        Destroy (clone.gameObject, 0.02f);
    }
}
