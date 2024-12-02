using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoTopDown : MonoBehaviour
{
    [SerializeField] private float moveVel;
    [SerializeField] private float fuerzaFlecha;
    [SerializeField] private GameObject _flecha;
    [SerializeField] private Transform posFlecha;
    private Vector2 pointerInput, movementInput;

    //Ataque
    [SerializeField] private Transform attackPont;
    [SerializeField] private float attackRadius;
    //[SerializeField] private GameObject particulas;
    [SerializeField] private float foerzaAtaque;
    //Ataque

    private PlayerInput _playerinput;
    private Rigidbody2D _rb;
    private Vector2 _pos;
    private Transform _transform;
    private Animator _animator;
    private SpriteRenderer _sprite;
    private bool _girar;
    private bool _left;
    private bool _atacando;
    private bool _disparando;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerinput = GetComponent<PlayerInput>();
        _transform = GetComponent<Transform>();
        _animator = GetComponent<Animator>();
        _left = true;
        _sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!_atacando)
        {
            //pointerInput = GetPointerInput();

            _pos = _playerinput.actions["Mover"].ReadValue<Vector2>();

        //Para mover el personaje
        _transform.position += new Vector3(_pos.x * moveVel * Time.deltaTime, _pos.y * moveVel * Time.deltaTime, 0);
        }
        //Para rotar el personaje
        //_animator.SetFloat("velX", Mathf.Abs(_pos.x));
        //_animator.SetFloat("velY", Mathf.Abs(_pos.y));
        if (_pos.x > 0 && !_girar) { transform.localScale = new Vector3(-1,1,1); _girar = true; }
        if (_pos.x < 0 && _girar) { transform.localScale = new Vector3(1,1,1); _girar = false; }
        if (_pos != Vector2.zero)
        {
            _animator.SetBool("1_Move", true);
        }
        else
        {
            _animator.SetBool("1_Move", false);
        }
    }
 
    public void Flecha(InputAction.CallbackContext context)
    {
       
        if (context.started)
        {
            Vector2 dirFlecha =  (pointerInput - (Vector2)transform.position).normalized;
            GameObject flecha = Instantiate(_flecha, posFlecha.position, posFlecha.rotation);
            flecha.GetComponent<Rigidbody2D>().AddForce(dirFlecha * fuerzaFlecha, ForceMode2D.Impulse);
            Destroy(flecha, 3f);
        }

    }

    //public void Disparando()
   // {
        
        
        //_disparando = true;
   // }
    //public void NoDisparando()
    
    
    public void Disparar(InputAction.CallbackContext context)
    {
        //Debug.Log(context.phase);
        if (context.started && !_atacando)
        {
            _rb.velocity = Vector2.zero;
            //_animator.SetBool("Disparar", true);
            _atacando = true;
        }
    }
    public void Disparando() { _atacando = true; _animator.SetBool("Disparar", true); }
    public void NoDisparando() { _atacando = false; _animator.SetBool("Disparar", false); }
    // todo aqui es de Ataque
    public void Melee(InputAction.CallbackContext context)
    {
        if (context.started && !_atacando)
        {
            _rb.velocity = Vector2.zero;
            Golpe();
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPont.position, attackRadius);
    }
    public void Golpe() { _atacando = true; _animator.SetBool("Melee", true); }
    public void NoGolpe() { _atacando = false; _animator.SetBool("Melee", false); }

    public void EventoAnimAtaque()
    {
        //Necesario para el circulo
        ContactFilter2D aux = new ContactFilter2D();
        List<RaycastHit2D> results = new List<RaycastHit2D>();

        // se crea el circulo
        Physics2D.CircleCast(attackPont.position, attackRadius, Vector2.zero, aux, results);

        //Lee la lista
        foreach (RaycastHit2D hit in results)
        {
            if (hit.collider.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("Golpeado");
                //Instantiate(particulasAtaque, hit.collider.transform.position, hit.collider.transform.rotation); genera particulas
                // hit.collider.gameObject.GetComponent<IA_Enemigo>().Golpear(); tiene el quitar vida en el enemigo
            }
            else
            {
                //Instantiate(particulasAtaque, attackPoint.position, attackPoint.rotation); Genera particulas si golpea otra cosa
            }
        }
    }
}
