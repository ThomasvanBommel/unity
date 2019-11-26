﻿using UnityEngine;
using System.Collections;

public class GeneratePlane : MonoBehaviour {
    static Mesh Plane;
    static int Size = 32;

    static float ScaleUpdate;
    public Material PlaneMaterial;
    static Color[] colors;
    static float Scale = 3f;
    static float Amplitude = 3.5f;
    static float Frequency = 0.25f;
    static int IslandSize = 4;
    public static float maxHeight = 0, minHeight = 0;

    static int XPos = 0, ZPos = 0;
    static int UpdateX = 0, UpdateZ = 0;

    static float WaterHeight = -1.6f, LandHeight = -1.5f, MountainHeight = 12f;

    float UpdateScale,
        UpdateAmplitude, 
        UpdateFrequency, 
        UpdateWaterHeight, 
        UpdateLandHeight, 
        UpdateMountainHeight;

    void Start () {
        //GenerateMesh();
    }
    
	void Update () {
	    if (Plane) {
            //Graphics.DrawMesh(Plane, Vector3.zero, Quaternion.identity, PlaneMaterial, 0);
            if (Input.GetKeyUp(KeyCode.G)) {
                print("PRESSED 'G': Generating...");
                GenerateMesh();
                //Plane.colors = GenerateTexture(Plane.vertices);
            }

            if (UpdateX != XPos) { GenerateMesh(); }
            if (UpdateZ != ZPos) { GenerateMesh(); }
            if (UpdateScale != Scale) { UpdateScale = Scale; GenerateMesh(); }
            if (UpdateAmplitude != Amplitude) { UpdateAmplitude = Amplitude; GenerateMesh(); }
            if (UpdateFrequency != Frequency) { UpdateFrequency = Frequency; GenerateMesh(); }
            if (UpdateWaterHeight != WaterHeight) { UpdateWaterHeight = WaterHeight; GenerateMesh(); }
            if (UpdateLandHeight != LandHeight) { UpdateLandHeight = LandHeight; GenerateMesh(); }
            if (UpdateMountainHeight != MountainHeight) { UpdateMountainHeight = MountainHeight; GenerateMesh(); }
        }
        
	}

    public static Mesh GenerateChunk(int x, int z) {
        XPos = x;
        ZPos = z;

        GenerateMesh();
        return Plane;
    }

    static void GenerateMesh() {
        Plane = new Mesh();
        Plane.vertices = Verticies();
        Plane.colors = colors;
        //Plane.colors = GenerateTexture(Plane.vertices);
        Plane.triangles = Triangles();
        Plane.uv = UVs();
        
        Plane.RecalculateNormals();

        UpdateX = XPos;
        UpdateZ = ZPos;
    }

    static Vector3[] Verticies() {
        Vector3[] v = new Vector3[Size * Size];
        for (int x = 0; x < Size; x++){
            for (int z = 0; z < Size; z++){
                int n = (z * (Size)) + x;
                v[n].x = x + XPos;
                v[n].z = z + ZPos;
            }
        }

        return GenerateTexture(v) ;
    }

    static int[] Triangles() {
        int[] t = new int[((Size - 1) * (Size - 1)) * 6];
        int count = 0;
        for (int x = 0; x < Size - 1; x++) {
            for (int z = 0; z < Size - 1; z++) {
                int n = ((z * (Size)) + x);
                t[count + 0] = n;
                t[count + 1] = n + Size;
                t[count + 2] = n + (Size + 1);

                t[count + 3] = n + (Size + 1);
                t[count + 4] = n + 1;
                t[count + 5] = n;
                
                count+=6;
            }
        }
        return t;
    }

    static Vector2[] UVs() {
        Vector2[] u = new Vector2[Plane.vertices.Length];
        for (int i = 0; i < Plane.vertices.Length; i++) {
            u[i] = new Vector2(Plane.vertices[i].x, Plane.vertices[i].z);
        }
        return u;
    }

    static Vector3[] GenerateTexture(Vector3[] v) {
        if (Scale < 0) { Scale = 0.0001f; }
        float MaxHeight = float.MaxValue, MinHeight = float.MinValue; 
        Color[] c = new Color[v.Length];

        for (int x = 0; x < Size; x++) {
            for (int z = 0; z < Size; z++) {
                float amplitude = 1;
                float frequency = 1;
                float height = 0;

                for (int i = 0; i < IslandSize; i++) {
                    float sampleX = (x + XPos + 344883) / Scale * frequency;
                    float sampleZ = (z + ZPos + 324894) / Scale * frequency;

                    float heightValue = Mathf.PerlinNoise(sampleX, sampleZ) * 2 - 1;
                    height += heightValue * amplitude;

                    amplitude *= Amplitude;
                    frequency *= Frequency;
                }
                if (height > MaxHeight) {
                    MaxHeight = height;
                } else if (height < MinHeight) {
                    MinHeight = height;
                }
                v[(z * Size) + x].y = height;
                if (height > maxHeight) { maxHeight = height; }
                if (height < minHeight) { minHeight = height; }
                if (height <= WaterHeight) {
                    //c[(z * Size) + x] = Color.blue;
                    c[(z * Size) + x] = Color32.Lerp(new Color32(0, 102, 255, 1), new Color32(0, 20, 51, 1), Mathf.InverseLerp(WaterHeight, -5f, height / 3));//dark, light, t :: bottom, top, t
                } else if (height > WaterHeight && height <= LandHeight) {
                    c[(z * Size) + x] = new Color32(0, 115, 11, 1);
                    //c[(z * Size) + x] = Color32.Lerp(new Color32(0, 92, 9, 1), new Color32(0, 115, 11, 1), Mathf.InverseLerp(WaterHeight, LandHeight, height));
                } else if (height > LandHeight && height < MountainHeight) {
                    //c[(z * Size) + x] = new Color32(139, 69, 19, 1);
                    c[(z * Size) + x] = Color32.Lerp(new Color32(0, 115, 11, 1), new Color32(139, 69, 19, 1), Mathf.InverseLerp(LandHeight, MountainHeight, height * 2));
                } else if (height >= MountainHeight) {
                    c[(z * Size) + x] = Color.white;
                }
                //c[(z * Size) + x] = Color.Lerp(Color.black, Color.white, 1f);
            }
        }


        colors = c;
        return v;
    }
}
