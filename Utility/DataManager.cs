using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fiourp
{
    public static class DataManager
    {
        public static ContentManager Content = Engine.Content;
        public static string contentDirName = new DirectoryInfo(Content.RootDirectory).FullName;


        public static Dictionary<string, Texture2D> Textures = GetAllTextures();

        public static Dictionary<string, Texture2D> GrassTileSet = LoadAllGraphicsFilesInFolder<Texture2D>("GrassTileSet");
        public static readonly Dictionary<int, Dictionary<string, Texture2D>> TileSets = new Dictionary<int, Dictionary<string, Texture2D>>()
        {
            { 1, GrassTileSet }
        };

        public static Dictionary<string, Dictionary<string, SpriteFont>> Fonts = GetFonts();

        public static Texture2D GetTexture(string textureID)
            => Textures[textureID];

        public static void Initialize() { }

        public static Dictionary<string, Texture2D> GetAllTextures()
        {
            Dictionary<string, Texture2D> dict = new Dictionary<string, Texture2D>();
            GetAllGraphicsFiles("", dict);
            return dict;
        }

        private static void GetAllGraphicsFiles(string folderName, Dictionary<string, Texture2D> d)
        {
            DirectoryInfo dir = new DirectoryInfo(Content.RootDirectory + "\\Graphics\\" + folderName);

            foreach (FileInfo file in dir.GetFiles())
            {
                string name = file.FullName.Substring(file.FullName.LastIndexOf("Graphics\\") + 9);
                string key = name.Substring(0, name.Length - 4);
                d[key] = Content.Load<Texture2D>("Graphics/" + key);
            }
            
            foreach (DirectoryInfo direct in dir.GetDirectories())
                GetAllGraphicsFiles(direct.FullName.Substring(contentDirName.Length + "Graphics\\".Length), d);
        }

        public static Dictionary<string, T> LoadAllGraphicsFilesInFolder<T>(string folderName)
        {
            DirectoryInfo dir = new DirectoryInfo(Content.RootDirectory + "\\Graphics\\" + folderName);

            Dictionary<string, T> d = new Dictionary<string, T>();

            foreach (FileInfo file in dir.GetFiles())
            {
                string key = Path.GetFileNameWithoutExtension(file.Name);
                d[key] = Content.Load<T>(dir.FullName + '/' + key);
            }

            return d;
        }

        public static Dictionary<string, Dictionary<string, SpriteFont>> GetFonts()
        {
            DirectoryInfo dir = new DirectoryInfo(Content.RootDirectory + "\\Fonts");

            Dictionary<string, Dictionary<string, SpriteFont>> d = new Dictionary<string, Dictionary<string, SpriteFont>>();

            foreach (DirectoryInfo directory in dir.GetDirectories())
            {
                string key = Path.GetFileNameWithoutExtension(directory.Name);
                d[key] = new Dictionary<string, SpriteFont>();

                foreach(FileInfo file in directory.GetFiles())
                {
                    string fileName = Path.GetFileNameWithoutExtension(file.Name);
                    d[key][fileName] = Content.Load<SpriteFont>(directory.FullName + "/" + fileName);
                }
            }
            
            return d;
        }
    }
}
