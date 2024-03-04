using System;
using System.Numerics;
using Friflo.Engine.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame;

public class Drones
{
    private readonly    EntityStore                         store;
    private readonly    int                                 droneCount = 10_000;
    
    private readonly    ArchetypeQuery<Transform>           transQuery;
    private readonly    ArchetypeQuery<Transform, Position> transPosQuery;
    private readonly    ArchetypeQuery<Position>            positionQuery;
    
    internal Drones() {
        store = new EntityStore(PidType.UsePidAsId);
        transQuery = store.Query<Transform>();
        positionQuery = store.Query<Position>();
        transPosQuery = store.Query<Transform, Position>();
    }

    public void Initialize()
    {
        for (int n = 0; n < droneCount; n++) {
            store.Batch()
                .Add(new Position())
                .Add(new Transform())
                .CreateEntity();
        }
    }
    
    internal void SetGrid()
    {
        int rowCount = (int)Math.Sqrt(droneCount);
        int x = 0;
        foreach (var (positions, _) in positionQuery.Chunks)
        {
            var positionSpan    = positions.Span;
            for (int n = 0; n < positions.Length; n++) {
                ref var position = ref positionSpan[n];
                position.x =  5 * x;
                position.y = -5;
                position.z =  5 * (n / rowCount);
                x = (x + 1) % rowCount;
            }
        }
    }
    
    internal void SetCube()
    {
        int edgeCount = (int)Math.Pow(droneCount, (1.0 / 3.0));
        int edgeCount2 = edgeCount * edgeCount;
        int x = 0;
        foreach (var (positions, _) in positionQuery.Chunks)
        {
            var positionSpan    = positions.Span;
            for (int n = 0; n < positions.Length; n++) {
                ref var position = ref positionSpan[n];
                position.x =  5 * x;
                position.y =  5 * ((n / edgeCount2)% edgeCount2) - 5;
                position.z =  5 * ((n / edgeCount) % edgeCount);
                x = (x + 1) % edgeCount;
            }
        }
    }
    
    internal void UpdateTransforms(double deltaTime, Matrix worldMatrix)
    {
        Matrix4x4 world = worldMatrix.AsMatrix4x4();
        foreach (var (transforms, positions, _) in transPosQuery.Chunks)
        {
            var positionSpan    = positions.Span;
            var transformSpan   = transforms.Span;
            for (int n = 0; n < positions.Length; n++) {
                transformSpan[n].value = world + Matrix4x4.CreateTranslation(positionSpan[n].value); 
            }
        }
    }

    public void Draw(Model model, Camera camera)
    {
        foreach (var (transforms, _) in transQuery.Chunks)
        {
            foreach (ref var transform in transforms.Span)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects) {
                        effect.EnableDefaultLighting();
                        effect.View			= camera.viewMatrix;
                        effect.World		= transform.value;
                        effect.Projection	= camera.projectionMatrix;
                        mesh.Draw();
                    }
                }
            }
        }
    }
}