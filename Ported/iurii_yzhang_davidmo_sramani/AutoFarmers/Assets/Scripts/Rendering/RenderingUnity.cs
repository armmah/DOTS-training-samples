﻿using System.Collections;
using System.Collections.Generic;
using GameAI;
using Rendering;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public class RenderingUnity : MonoBehaviour
{
    public static readonly int2 WorldSize = new int2(WorldCreatorSystem.worldSizeN, WorldCreatorSystem.worldSizeM);
    public static readonly int2 WorldSizeHalf = WorldSize / 2;

    public const float scale = 2.1f;

    public static int2 World2TilePosition(float3 pos)
    {
        return new int2((int) (pos.x * scale), (int) (pos.y * scale)) + WorldSizeHalf;
    }
    
    public static float3 Tile2WorldPosition(int2 pos)
    {
        pos -= WorldSizeHalf; 
        return new float3(pos.x, 0.0f, pos.y) * scale;
    }
    
    public static float3 Tile2WorldSize(int2 reqSize)
    {
        return new float3(reqSize.x, 1.0f/scale, reqSize.y) * scale;
    }
    
    public MeshRenderer drone;
    public MeshRenderer farmer;
    public MeshRenderer ground;
    public MeshRenderer plant;
    public MeshRenderer rock;
    public MeshRenderer store;
    
    static RenderingUnity m_instance;
    public static RenderingUnity instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindObjectOfType<RenderingUnity>();
            return m_instance;
        }
    }

    public Entity CreateDrone(EntityManager em)
    {
        var atype = em.CreateArchetype(typeof(Translation),
            typeof(NonUniformScale), typeof(LocalToWorld), typeof(RenderMesh), typeof(RenderingAnimationComponent),
            typeof(RenderingAnimationDroneFlyComponent));
        return Create(em, drone, atype);
    }
    
    public Entity CreateFarmer(EntityManager em)
    {
        var atype = em.CreateArchetype(typeof(Translation),
            typeof(NonUniformScale), typeof(LocalToWorld), typeof(RenderMesh), typeof(RenderingAnimationComponent));
        return Create(em, farmer, atype);
    }
    
    public Entity CreateGround(EntityManager em)
    {
        var atype = em.CreateArchetype(typeof(Translation), typeof(LocalToWorld), typeof(NonUniformScale), typeof(RenderMesh));
        return Create(em, ground, atype);
    }

    public Entity CreateStone(EntityManager em)
    {
        var atype = em.CreateArchetype(typeof(Translation),
            typeof(NonUniformScale), typeof(LocalToWorld), typeof(RenderMesh));
        return Create(em, rock, atype);
    }

    public Entity CreatePlant(EntityManager em)
    {
        var atype = em.CreateArchetype(typeof(Translation),
            typeof(NonUniformScale), typeof(LocalToWorld), typeof(RenderMesh));
        return Create(em, plant, atype);
    }
    
    public Entity CreateStore(EntityManager em)
    {
        var atype = em.CreateArchetype(typeof(Translation),
            typeof(NonUniformScale), typeof(LocalToWorld), typeof(RenderMesh));
        return Create(em, store, atype);
    }

    private Entity Create(EntityManager em, MeshRenderer meshRenderer, EntityArchetype atype)
    {
        var e = em.CreateEntity(atype);
        em.SetComponentData(e, new Translation {Value = new float3(0, meshRenderer.transform.position.y, 0)});
        em.SetSharedComponentData(e, new RenderMesh
        {
            mesh = meshRenderer.GetComponent<MeshFilter>().sharedMesh,
            material = meshRenderer.sharedMaterial,
            castShadows = ShadowCastingMode.On,
            receiveShadows = true
        });
        
        if (em.HasComponent<NonUniformScale>(e))
            em.SetComponentData(e, new NonUniformScale {Value = meshRenderer.transform.localScale});

        if (em.HasComponent<RenderingAnimationDroneFlyComponent>(e))
            em.SetComponentData(e, new RenderingAnimationDroneFlyComponent {offset = Random.Range(-Mathf.PI, Mathf.PI)});
        
        //TODO: make it disabled (how?)
        
        return e;
    }
    
    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}