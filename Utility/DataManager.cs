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

        public static Dictionary<string, Texture2D> GrassTileSet = GetAllGraphicsFilesInFolder<Texture2D>("GrassTileSet");
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
                name = name.Replace('\\', '/');
                string key = name.Substring(0, name.Length - 4);
                d[key] = Content.Load<Texture2D>("Graphics/" + key);
            }
            
            foreach (DirectoryInfo direct in dir.GetDirectories())
                GetAllGraphicsFiles(direct.FullName.Substring(contentDirName.Length + "Graphics\\".Length), d);
        }

        public static Dictionary<string, T> GetAllGraphicsFilesInFolder<T>(string folderName)
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

        /// <summary>
        /// Get all graphics files that start with the specified name
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Texture2D[] LoadAllGraphicsWithName(string name, string folderName = "")
        {
            DirectoryInfo dir = new DirectoryInfo(Content.RootDirectory + "\\Graphics\\" + folderName);
            List<Texture2D> textures = new List<Texture2D>();

            foreach (FileInfo file in dir.GetFiles())
            {
                string fileName = file.FullName.Substring(file.FullName.LastIndexOf("Graphics\\") + 9);
                if (!file.Name.StartsWith(name))
                    continue;

                string key = fileName.Substring(0, fileName.Length - 4);
                textures.Add(Content.Load<Texture2D>("Graphics/" + key));
            }

            foreach (DirectoryInfo direct in dir.GetDirectories())
                textures.AddRange(LoadAllGraphicsWithName(name, direct.FullName.Substring(contentDirName.Length + "Graphics\\".Length)));

            return textures.ToArray();
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
