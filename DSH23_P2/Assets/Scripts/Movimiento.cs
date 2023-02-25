using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Movimiento : MonoBehaviour
{
    public Camera cam;
    public GameObject prefabSuelo;
    public int velocidad;

    Vector3 offset;

    float valX;
    float valZ;

    int premios;
    public Text textoPuntuacion;
    public GameObject prefabPremio;

    private Rigidbody rb;
    private Vector3 direccionActual;
    private AudioSource SonidoDeCambioDireccion;

    // Start is called before the first frame update
    void Start()
    {
        offset = cam.transform.position;

        premios = 0;

        valX = 0.0f;
        valZ = 0.0f;
        rb = GetComponent<Rigidbody>();
        direccionActual = Vector3.forward;
        SonidoDeCambioDireccion = GetComponent<AudioSource>();
        SueloInicial();
    }

    // Update is called once per frame
    void Update()
    {
        cam.transform.position = this.transform.position + offset; //para mover la camara con la bola

        // Obtener input del jugador para cambiar la dirección de la pelota
        if(Input.GetKeyUp(KeyCode.Space)){
            SonidoDeCambioDireccion.Play();
            if(direccionActual == Vector3.forward)
                direccionActual = Vector3.right;
            else
                direccionActual = Vector3.forward;
        }

        float tiempo = velocidad * Time.deltaTime;
        rb.transform.Translate(direccionActual*tiempo);
        
    }

    void SueloInicial()
    {
        for (int n = 0; n < 3; n++)
        {
            valZ += 6.0f;
            GameObject elSuelo = Instantiate(prefabSuelo, new Vector3(valX, 0.0f, valZ), Quaternion.identity) as GameObject;
        }
    }

    void OnCollisionExit(Collision other) {
        if(other.transform.tag == "Suelo"){
            StartCoroutine(CrearSuelo(other));
        }
    }

    IEnumerator CrearSuelo(Collision col){
        yield return new WaitForSeconds(0.5f); //espera de 5 s
        col.rigidbody.isKinematic = false;
        col.rigidbody.useGravity = true;
        yield return new WaitForSeconds(1f); //espera de 5 s
        Destroy(col.gameObject); // se destruye el suelo
        float ran = Random.Range(0f,1f); //creo un suelo nuevo de forma aleatoria, o hacia delante o hacia la derecha
        if(ran <= 0.5f)//creo un cubo hacia delante
            valX += 6.0f;
        else
            valZ += 6.0f;
        
        //creo un cubo nuevo
        GameObject elSuelo = Instantiate(prefabSuelo, new Vector3(valX, 0.0f, valZ), Quaternion.identity) as GameObject;

        // Generar premios: (se podría poner dentro de los if anteriores, pero por claridad he preferido ponerlo separado)
        // Generar el premio en un sitio aleatorio del suelo
        float ranSpawnZ = Random.Range(-2.5f, 2.5f);
        float ranSpawnX = Random.Range(-2.5f, 2.5f);

        // Premio delante
        if (ran <= 0.2f)
        {
            GameObject elPremio = Instantiate(prefabPremio, new Vector3(valX, 1.0f, valZ + ranSpawnZ), Quaternion.identity) as GameObject;
        }
        // Premio a la derecha
        if (ran >= 0.8f)
        {
            GameObject elPremio = Instantiate(prefabPremio, new Vector3(valX + ranSpawnX, 1.0f, valZ), Quaternion.identity) as GameObject;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Si el objeto con el que nos chocamos es un premio, destruirlo y aumentar premios
        if (other.gameObject.CompareTag("Premio"))
        {
            Debug.Log("Has conseguido un premio!");
            premios++;
            textoPuntuacion.text = "Puntuación: " + premios;
            Destroy(other.gameObject);
        }
        /*
        // Pasar al siguiente nivel o ganar, dependiendo del nivel actual y los premios conseguidos
        if (premios == 6 && escena.name == "SegundaEscena")
        {
            SceneManager.LoadScene("TerceraEscena", LoadSceneMode.Single);    
        }
        if (premios == 10 && escena.name == "TerceraEscena")
        {
            texto.text = "Felicidades, Has ganado! :D";
        }*/
    }
}
