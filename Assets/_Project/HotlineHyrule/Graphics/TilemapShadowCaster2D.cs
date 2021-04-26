using System;
using HotlineHyrule.Extensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HotlineHyrule.Graphics
{
    public class TilemapShadowCaster2D : MonoBehaviour
    {
        [SerializeField] GameObject shadowCasterPrefab;
        [SerializeField] GameObject shadowCasterContainer;
        
        Tilemap Tilemap { get; set; }

        void Awake()
        {
            Tilemap = GetComponent<Tilemap>();
            if (!shadowCasterPrefab) shadowCasterPrefab = Resources.Load<GameObject>("Prefabs/Graphics/shadow_caster");
            if (!shadowCasterContainer)
            {
                shadowCasterContainer = new GameObject("shadow_casters");
                shadowCasterContainer.transform.SetParent(Tilemap.transform);
            }
        }
        
        void Start()
        {
            if (!Tilemap) return;

            foreach (var cellPosition in Tilemap.cellBounds.allPositionsWithin)
            {
                if (!Tilemap.HasTile(cellPosition)) continue;
                var shadowCasterObject = Instantiate(shadowCasterPrefab, (Vector2)cellPosition.ToWorld(), Quaternion.identity);
                shadowCasterObject.name = $"shadow_caster_{cellPosition.x}_{cellPosition.y}";
                shadowCasterObject.transform.SetParent(shadowCasterContainer.transform);
            }
        }
    }
}