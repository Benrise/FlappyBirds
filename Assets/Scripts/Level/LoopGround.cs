using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopGround : MonoBehaviour
{
    [SerializeField]
    private float _speed = 1f;

    [SerializeField]
    private float _width = 100f;

    private SpriteRenderer _spriteRenederer;

    private Vector2 _startSize;


    private void Start(){
        _spriteRenederer = GetComponent<SpriteRenderer>();

        _startSize = new Vector2(_spriteRenederer.size.x, _spriteRenederer.size.y);
    }

    private void Update(){
        _spriteRenederer.size = new Vector2(_spriteRenederer.size.x + _speed * Time.deltaTime, _spriteRenederer.size.y);

        if (_spriteRenederer.size.x > _width){
            _spriteRenederer.size = _startSize;
        }
    }
}
