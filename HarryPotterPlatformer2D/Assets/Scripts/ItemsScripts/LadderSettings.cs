using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LadderSettings : MonoBehaviour
{
    [SerializeField] private float _hightOfSection;
    [SerializeField] private float _widthOfSection;
    [SerializeField]
    [Range(3,20)]
    private int _numOfSections;
    [SerializeField] private Sprite _ladder_BT;
    [SerializeField] private Sprite _ladder_M;
    [SerializeField] private string _layerName;
    [SerializeField] private int _orderInLayer;
    public void CreateLadder()
    {
        for (int k = transform.childCount - 1; k > -1; k--)
        {
            DestroyImmediate(transform.GetChild(k).gameObject);
        }
        for (int i = 0; i < _numOfSections; i++)
        {
            GameObject section = new GameObject();
            section.AddComponent<SpriteRenderer>();
            SpriteRenderer sprite = section.GetComponent<SpriteRenderer>();
            if (i == 0)
            {
                sprite.sprite = _ladder_BT;
                sprite.flipY = true;
            }
            else if (i == _numOfSections-1)
            {
                sprite.sprite = _ladder_BT;
            }
            else
            {
                sprite.sprite = _ladder_M;
            }
            sprite.sortingLayerName = _layerName;
            sprite.sortingOrder = _orderInLayer;
            sprite.drawMode = SpriteDrawMode.Sliced;
            sprite.size = new Vector2(_widthOfSection,_hightOfSection);
            section.transform.parent = transform;
            section.transform.localPosition = new Vector3(0, i * _hightOfSection, 0);
        }
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        box.size = new Vector2(0.4f, _numOfSections*_hightOfSection - 0.1f);
        box.offset = new Vector2(-0.01f, (_numOfSections-1) * _hightOfSection  / 2);
    }
}
