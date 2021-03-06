﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace OGLTest
{
  class CubeMesh
  {
    private float[][] _blockPoints =
    {
            // bottom points
            new float[] {0, 0, 0},
            new float[] {1, 0, 0},
            new float[] {1, 0, 1},
            new float[] {0, 0, 1},

            // top points
            new float[] {0, 1, 0},
            new float[] {1, 1, 0},
            new float[] {1, 1, 1},
            new float[] {0, 1, 1}
        };

    public TextureCoordinates TextureCoordinates { get; set; }
        = new TextureCoordinates
        {
          uLeft = 0.0f,
          vTop = 0.0f,
          uRight = 1.0f,
          vBottom = 1.0f
        };

    public Vector3 Position { get; set; } = Vector3.Zero;
    public Vector3 Scale { get; set; } = Vector3.One;
    public Color4 Color { get; set; } = Color4.White;

    public class FacesToInclude
    {
      public bool Bottom = true;
      public Color4 BottomColor = Color4.White;

      public bool Top = true;
      public Color4 TopColor = Color4.White;

      public bool Front = true;
      public Color4 FrontColor = Color4.White;

      public bool Back = true;
      public Color4 BackColor = Color4.White;

      public bool Right = true;
      public Color4 RightColor = Color4.White;

      public bool Left = true;
      public Color4 LeftColor = Color4.White;
    }
    public FacesToInclude IncludeFaces { get; } = new FacesToInclude();


    public List<Vertex> CreateMesh()
    {
      List<Vertex> vertices = new List<Vertex>();

      if (IncludeFaces.Bottom)
        vertices.AddRange(CreateFace(new[] { 0, 1, 2, 3 }, TextureCoordinates, -Vector3.UnitY, IncludeFaces.BottomColor));

      if (IncludeFaces.Top)
        vertices.AddRange(CreateFace(new[] { 4, 7, 6, 5 }, TextureCoordinates, Vector3.UnitY, IncludeFaces.TopColor));

      if (IncludeFaces.Front)
        vertices.AddRange(CreateFace(new[] { 0, 4, 5, 1 }, TextureCoordinates, -Vector3.UnitZ, IncludeFaces.FrontColor));

      if (IncludeFaces.Back)
        vertices.AddRange(CreateFace(new[] { 2, 6, 7, 3 }, TextureCoordinates, Vector3.UnitZ, IncludeFaces.BackColor));

      if (IncludeFaces.Right)
        vertices.AddRange(CreateFace(new[] { 1, 5, 6, 2 }, TextureCoordinates, Vector3.UnitX, IncludeFaces.RightColor));

      if (IncludeFaces.Left)
        vertices.AddRange(CreateFace(new[] { 3, 7, 4, 0 }, TextureCoordinates, -Vector3.UnitX, IncludeFaces.LeftColor));


      List<Vertex> transformedVertices = new List<Vertex>();

      foreach (Vertex vertex in vertices)
      {
        Vertex transformedVertex = vertex;

        // Scale
        transformedVertex._position = Vector3.Multiply(transformedVertex._position, Scale);
        // Transform
        transformedVertex._position = Vector3.Add(transformedVertex._position, Position);

        transformedVertices.Add(transformedVertex);
      }

      return transformedVertices;
    }

    private List<Vertex> CreateFace(int[] positionHandles, TextureCoordinates textureCoordinates, Vector3 normal, Color4 color)
    {
      List<Vertex> ret = new List<Vertex>
            {
                CreateVertex(_blockPoints[positionHandles[0]], textureCoordinates.uLeft, textureCoordinates.vBottom, normal, color),
                CreateVertex(_blockPoints[positionHandles[2]], textureCoordinates.uRight, textureCoordinates.vTop, normal, color),
                CreateVertex(_blockPoints[positionHandles[1]], textureCoordinates.uLeft, textureCoordinates.vTop, normal, color),

                CreateVertex(_blockPoints[positionHandles[0]], textureCoordinates.uLeft, textureCoordinates.vBottom, normal, color),
                CreateVertex(_blockPoints[positionHandles[3]], textureCoordinates.uRight, textureCoordinates.vBottom, normal, color),
                CreateVertex(_blockPoints[positionHandles[2]], textureCoordinates.uRight, textureCoordinates.vTop, normal, color)
            };

      return ret;
    }

    private Vertex CreateVertex(float[] position, float u, float v, Vector3 normal, Color4 color)
    {
      return new Vertex(new Vector3(position[0], position[1], position[2]), normal,
          color, new Vector2(u, v));
    }
  }
}
