using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player; // Referencia al jugador
    public GameObject bulletPrefab; // Prefab de la bala
    public Transform bulletSpawnPoint; // Punto de salida de la bala
    public Transform healStation; // Estación de curación
    public Transform reloadStation; // Estación de recarga de balas
    public int maxBullets = 8; // Cantidad máxima de balas
    public float shootingRange = 10f; // Rango de disparo
    public float shootInterval = 1f; // Intervalo de tiempo entre disparos
    public float bulletSpeed = 10f; // Velocidad de la bala

    // Variable pública para la cantidad actual de balas
    public int currentBullets;

    private NavMeshAgent agent;
    private bool isReloading = false;
    private bool isHealing = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentBullets = maxBullets; // Inicializar la cantidad de balas
        InvokeRepeating("Shoot", 0f, shootInterval); // Disparar cada cierto intervalo
    }

    private void Update()
    {
        // Si el jugador está dentro del rango de disparo
        if (Vector3.Distance(transform.position, player.position) <= shootingRange)
        {
            agent.SetDestination(transform.position); // Detenerse
            transform.LookAt(player); // Apuntar al jugador
        }
        else if (!isReloading && currentBullets <= 0)
        {
            // Si no está recargando y se queda sin balas, ir a recargar
            agent.SetDestination(reloadStation.position);
        }
        else if (!isReloading && Vector3.Distance(transform.position, reloadStation.position) <= 1f)
        {
            // Si no está recargando y está cerca de la estación de recarga, ir a recargar
            agent.SetDestination(reloadStation.position);
        }
        else if (!isHealing && Vector3.Distance(transform.position, healStation.position) <= 1f)
        {
            // Si no está curándose y está cerca de la estación de curación, ir a curarse
            agent.SetDestination(healStation.position);
        }
        else
        {
            // Si no está haciendo ninguna acción, seguir al jugador
            agent.SetDestination(player.position);
        }
    }

    private void Shoot()
    {
        // Disparar si el jugador está dentro del rango de disparo y tiene balas
        if (currentBullets > 0 && Vector3.Distance(transform.position, player.position) <= shootingRange)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
            bulletRigidbody.velocity = transform.forward * bulletSpeed;

            Destroy(bullet, 3f); // Destruir la bala después de 3 segundos si no ha impactado
            currentBullets--; // Reducir la cantidad de balas
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Si colisiona con el lugar de curación, empezar a curarse
        if (collision.gameObject.CompareTag("Health"))
        {
            isHealing = true;
            // Aquí podrías agregar la lógica para la curación
        }

        // Si colisiona con el lugar de recarga, empezar a recargar
        if (collision.gameObject.CompareTag("Ammo"))
        {
            isReloading = true;
            currentBullets = maxBullets; // Recargar balas
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Si deja de colisionar con el lugar de curación, dejar de curarse
        if (collision.gameObject.CompareTag("Health"))
        {
            isHealing = false;
        }

        // Si deja de colisionar con el lugar de recarga, dejar de recargar
        if (collision.gameObject.CompareTag("Ammo"))
        {
            isReloading = false;
        }
    }
}
