﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OGLTest.Utilities;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace OGLTest
{
  // For mapping a texture onto a rectangle

  public class TextureAtlas
  {
    private Dictionary<string, TextureCoordinates> _textureDictionary = new Dictionary<string, TextureCoordinates>();

    private int[] _pixels;
    private int _pixelWidth, _pixelHeight;

    private Texture _texture;

    public TextureAtlas(string textureDirectory, ICollection<string> texturesToLoad)
    {
      string[] files = Directory.GetFiles(textureDirectory, "*.png", SearchOption.TopDirectoryOnly);

      bool filtertingTextures = texturesToLoad != null && texturesToLoad.Count > 0;
      if (filtertingTextures)
      {
        files = files.Where(file => texturesToLoad.Contains(Path.GetFileName(file))).ToArray();
      }

      int imagesPerRowColumn = (int)Math.Ceiling(Math.Sqrt(files.Length));
      int pixelsPerRowColumn = 1 + // The beginning border
                               imagesPerRowColumn * (256 + 1); // The image + ending border

      _pixelWidth = pixelsPerRowColumn;
      _pixelHeight = pixelsPerRowColumn;

      _pixels = new int[_pixelWidth * _pixelHeight];

      int currentRow = 0;
      int currentColumn = 0;

      foreach (string currentFile in files)
      {
        if (copyToAtlas(currentRow, currentColumn, currentFile) == false)
          continue;

        currentColumn++;
        if (currentColumn == imagesPerRowColumn)
        {
          currentColumn = 0;
          currentRow++;
        }
      }
      _texture = new Texture(_pixels, _pixelWidth, _pixelHeight);

      // saveTextureAtlas();
    }

    private void setPixel(int x, int y, int color)
    {
      _pixels[x + y * _pixelWidth] = color;
    }

    private bool copyToAtlas(int row, int column, string file)
    {
      int rowStart = 1 + (256 + 1) * row;
      int columnStart = 1 + (256 + 1) * column;

      var imageDataLoader = new ImageDataLoader(file);
      if (imageDataLoader.Height != 256 || imageDataLoader.Width != 256)
        return false;

      for (int i = 0; i < 256; i++)
      {
        for (int j = 0; j < 256; j++)
        {
          setPixel(columnStart + j, rowStart + i, imageDataLoader.Data[j, i]);
        }
      }

      string fileName = Path.GetFileName(file);
      Point topLeft = new Point(columnStart, rowStart);
      Point bottomRight = new Point(columnStart + 256, rowStart + 256);
      _textureDictionary[fileName] = CreateUVCoordinates(topLeft, bottomRight);

      return true;
    }

    private TextureCoordinates CreateUVCoordinates(Point topLeft, Point bottomRight)
    {
      return new TextureCoordinates()
      {
        uLeft = (float)topLeft.X / _pixelWidth,
        vTop = (float)topLeft.Y / _pixelHeight,

        uRight = (float)bottomRight.X / _pixelWidth,
        vBottom = (float)bottomRight.Y / _pixelHeight
      };
    }

    private void saveTextureAtlas()
    {
      Bitmap bm = new Bitmap(_pixelWidth, _pixelHeight);

      BitmapData bmData = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height),
          ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

      Marshal.Copy(_pixels, 0, bmData.Scan0, _pixelWidth * _pixelHeight);

      bm.UnlockBits(bmData);

      bm.Save("J:\\hello.png");
    }

    public TextureCoordinates GetTextureCoordinates(string texture)
    {
      return _textureDictionary[texture];
    }

    public void Bind(TextureUnit textureUnit)
    {
      _texture.Bind(textureUnit);
    }
  }
}
