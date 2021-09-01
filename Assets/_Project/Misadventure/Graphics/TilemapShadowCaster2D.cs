using System.Collections.Generic;
using System.Linq;
using Misadventure.Extensions;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Misadventure.Graphics
{
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [RequireComponent(typeof(CompositeCollider2D))]
    [AddComponentMenu("Rendering/2D/Tilemap Shadow Caster 2D")]
    public class TilemapShadowCaster2D : MonoBehaviour
    {
        [SerializeField] GameObject shadowCasterContainer;
        [SerializeField] bool generateOnAwake;
        [SerializeField] bool hasSelfShadows;

        CompositeCollider2D CompositeCollider { get; set; }

        void Start()
        {
            if (!generateOnAwake) return;

            GenerateShadowCaster();
        }

        [ContextMenu("Generate Shadow Caster")]
        void GenerateShadowCaster()
        {
            CompositeCollider = GetComponent<CompositeCollider2D>();

            if (shadowCasterContainer) DestroyImmediate(shadowCasterContainer);

            shadowCasterContainer = new GameObject("shadow_caster");
            shadowCasterContainer.transform.SetParent(transform);

            var polygonColliders = new List<PolygonCollider2D>();
            var polygons = new List<Vector2[]>();
            var torusPolygons = new List<(Vector2[], Vector2[])>();

            for (var i = 0; i < CompositeCollider.pathCount; i++)
            {
                var vertexCount = CompositeCollider.GetPathPointCount(i);
                var polygon = new Vector2[vertexCount];

                CompositeCollider.GetPath(i, polygon);

                var shadowCasterObject = new GameObject($"shadow_caster_{i}");
                shadowCasterObject.transform.SetParent(shadowCasterContainer.transform);

                var polygonCollider = shadowCasterObject.AddComponent<PolygonCollider2D>();
                polygonColliders.Add(polygonCollider);

                polygonCollider.points = polygon;
                polygonCollider.enabled = false;

                polygons.Add(polygon);

                var shadowCaster = shadowCasterObject.AddComponent<ShadowCaster2D>();
                shadowCaster.selfShadows = hasSelfShadows;
            }

            //find tori
            foreach (var polygon in polygons)
            {
                var containedPolygons = new List<Vector2[]>();

                foreach (var otherPolygon in polygons)
                {
                    if (otherPolygon == polygon) continue;

                    if (polygon.ContainsPolygon(otherPolygon))
                    {
                        containedPolygons.Add(otherPolygon);
                    }
                }

                foreach (var containedPolygon in containedPolygons)
                {
                    var isTorus = true;

                    foreach (var otherPolygon in containedPolygons)
                    {
                        if (otherPolygon == polygon) continue;
                        if (otherPolygon == containedPolygon) continue;

                        if (!containedPolygon.ContainsPolygon(otherPolygon)) isTorus = false;
                    }

                    if (isTorus) torusPolygons.Add((polygon, containedPolygon));
                }
            }

            //combine tori
            foreach (var (outerPolygon, innerPolygon) in torusPolygons)
            {
                var combinedPolygon = new List<Vector2>();

                var innerIndex = 0;
                var outerIndex = 0;
                var closestDistance = 0f;

                for (var i = 0; i < innerPolygon.Length; i++)
                {
                    var innerPoint = innerPolygon[i];

                    for (var j = 0; j < outerPolygon.Length; j++)
                    {
                        var outerPoint = outerPolygon[j];
                        var distance = (innerPoint - outerPoint).magnitude;

                        if (!(distance < closestDistance)) continue;

                        innerIndex = i;
                        outerIndex = j;
                        closestDistance = distance;
                    }
                }

                var firstInnerPolygon = (from point in innerPolygon
                    where innerPolygon.ToList().IndexOf(point) <= innerIndex
                    select point);
                var secondInnerPolygon = (from point in innerPolygon
                    where innerPolygon.ToList().IndexOf(point) >= innerIndex
                    select point);

                var firstOuterPolygon = (from point in outerPolygon
                    where outerPolygon.ToList().IndexOf(point) <= outerIndex
                    select point);
                var secondOuterPolygon = (from point in outerPolygon
                    where outerPolygon.ToList().IndexOf(point) >= outerIndex
                    select point);

                combinedPolygon.AddRange(firstOuterPolygon);
                combinedPolygon.AddRange(secondInnerPolygon);
                combinedPolygon.AddRange(firstInnerPolygon);
                combinedPolygon.AddRange(secondOuterPolygon);

                var shadowCasterObject = new GameObject($"shadow_caster_combined");
                shadowCasterObject.transform.SetParent(shadowCasterContainer.transform);

                var polygonCollider = shadowCasterObject.AddComponent<PolygonCollider2D>();

                polygonCollider.points = combinedPolygon.ToArray();
                polygonCollider.enabled = false;

                var shadowCaster = shadowCasterObject.AddComponent<ShadowCaster2D>();
                shadowCaster.selfShadows = hasSelfShadows;

                var indexOfInnerPolygon = polygons.IndexOf(innerPolygon);
                var innerPolygonCollider = polygonColliders[indexOfInnerPolygon];
                DestroyImmediate(innerPolygonCollider.gameObject);

                var indexOfOuterPolygon = polygons.IndexOf(outerPolygon);
                var outerPolygonCollider = polygonColliders[indexOfOuterPolygon];
                DestroyImmediate(outerPolygonCollider.gameObject);
            }
        }
    }
}