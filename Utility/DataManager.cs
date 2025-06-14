using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using MonoGame.Aseprite;
using AsepriteDotNet.Aseprite;
using AsepriteDotNet.Aseprite.Types;
using MonoGame.Aseprite.Utils;

namespace Fiourp
{
    public static class DataManager
    {
        public static ContentManager Content = Engine.Content;
        public static ContentTypeReaderManager ContentTypeReader = new();
        public static string contentDirName = new DirectoryInfo(Content.RootDirectory).FullName;

        public static Dictionary<string, Texture2D> Textures = new();
        public static Dictionary<string, Effect> PixelShaders = new();

        public static Dictionary<int, Dictionary<string, Texture2D>> Tilesets;
        public static Dictionary<string,Texture2D> Objects = new();

        public static Dictionary<string, Dictionary<string, SpriteFont>> Fonts = GetFonts();

        public static Texture2D GetTexture(string textureID)
            => Textures[textureID];

        public static void Initialize(string XMLPath)
        {
            if(XMLPath != "")
                Tilesets = GetAllTileSets(XMLPath);

            Initialize();
        }

        public static void Initialize()
        {
            GetAllGraphicsFiles("");
        }

        private static void GetAllGraphicsFiles(string folderName)
        {
            if (!Directory.Exists(Content.RootDirectory + "/Graphics/" + folderName))
                return;

            DirectoryInfo dir = new DirectoryInfo(Content.RootDirectory + "/Graphics/" + folderName);

            foreach (FileInfo file in dir.GetFiles())
            {
                string fileName = file.FullName.Replace('\\', '/');
                string name = fileName.Substring(fileName.LastIndexOf("Graphics/") + 9);
                string key = name.Substring(0, name.LastIndexOf('.'));

                object loaded = Content.Load<Object>("Graphics/" + key);
                bool isObject = fileName.Contains("Graphics/Objects");

                if (loaded is Texture2D texture)
                    AddTexture(key, texture);
                else if (loaded is AsepriteFile asepriteDoc)
                {
                    Texture2D[] textures = asepriteDoc.Load();
                    if(textures.Length == 1)
                        AddTexture(key, textures[0]);
                    for(int i = 1; i < textures.Length + 1; i++)
                        AddTexture(key + i, textures[i - 1]);
                }
                else if(loaded is Effect shader)
                {
                    PixelShaders[key.Substring(key.LastIndexOf("PixelShaders/") + 13)] = shader;
                }

                void AddTexture(string key, Texture2D texture)
                {
                    if (isObject)
                        Objects[key.Substring(key.LastIndexOf("Objects/") + 8)] = texture;
                    Textures[key] = texture;
                }
            }
            
            foreach (DirectoryInfo direct in dir.GetDirectories())
                GetAllGraphicsFiles(direct.FullName.Substring(contentDirName.Length + "Graphics/".Length));
        }

        public static Dictionary<string, T> GetAllFilesInFolder<T>(string folderName)
        {
            if (!Directory.Exists(Content.RootDirectory + "/Graphics/" + folderName))
                return null;

            DirectoryInfo dir = new DirectoryInfo(Content.RootDirectory + "/Graphics/" + folderName);

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
            DirectoryInfo dir = new DirectoryInfo(Content.RootDirectory + "/Graphics" + folderName);
            List<Texture2D> textures = new List<Texture2D>();

            foreach (FileInfo file in dir.GetFiles())
            {
                string fileName = file.FullName.Replace('\\', '/').Substring(file.FullName.Replace('\\', '/').LastIndexOf("Graphics/") + 9);
                if (!file.Name.StartsWith(name))
                    continue;

                string key = fileName.Substring(0, fileName.Length - 4);
                object loaded = Content.Load<Object>("Graphics/" + key);

                if (loaded is Texture2D texture)
                    textures.Add(texture);
                else if (loaded is AsepriteFile asepriteDoc)
                    textures.AddRange(asepriteDoc.Load());
            }

            foreach (DirectoryInfo direct in dir.GetDirectories())
                textures.AddRange(LoadAllGraphicsWithName(name, direct.FullName.Substring(contentDirName.Length + "Graphics/".Length)));

            return textures.ToArray();
        }

        public static float[] GetAnimationDelays(string asepritePath)
        {
            AsepriteFile ase = Content.Load<AsepriteFile>("Graphics/" + asepritePath);

            float[] delays = new float[ase.Frames.Length];

            for(int i = 0; i < ase.Frames.Length; i++)
                delays[i] = (float)ase.Frames[i].Duration.TotalSeconds;

            return delays;
        }

        public static Dictionary<string, Dictionary<string, SpriteFont>> GetFonts()
        {
            if (!Directory.Exists(Content.RootDirectory + "/Fonts"))
                return null;

            DirectoryInfo dir = new DirectoryInfo(Content.RootDirectory + "/Fonts");

            Dictionary<string, Dictionary<string, SpriteFont>> d = new Dictionary<string, Dictionary<string, SpriteFont>>();

            foreach (DirectoryInfo directory in dir.GetDirectories())
            {
                string key = Path.GetFileNameWithoutExtension(directory.Name);
                d[key] = new Dictionary<string, SpriteFont>();

                foreach(FileInfo file in directory.GetFiles())
                {
                    string fileName = Path.GetFileNameWithoutExtension(file.Name);
                    d[key][fileName] = Content.Load<SpriteFont>("./Fonts/" + directory.Name + "/" + fileName);
                }
            }
            
            return d;
        }

        private static Dictionary<int, Dictionary<string, Texture2D>> GetAllTileSets(string XMLpath)
        {
            DirectoryInfo dir = new DirectoryInfo(Content.RootDirectory + "/Graphics/Tilesets");
            Dictionary<int, Dictionary<string, Texture2D>> d = new();

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(File.ReadAllText("Utility/SpriteData.xml"));

            foreach (System.Xml.XmlElement element in doc["Sprites"]["Tilesets"])
            {
                object loaded = Content.Load<Object>("Graphics/Tilesets" + "/" + element.GetAttribute("path"));

                Texture2D texture;
                if (loaded is Texture2D txt)
                    texture = txt;
                else if (loaded is AsepriteFile asepriteDoc)
                    texture = asepriteDoc.Load()[0];
                else
                    throw new Exception("Tileset was not found under the right format");

                if (element.Name == "OneOfEach")
                    d[stringToInt(element.GetAttribute("id"))] = GetTileSetTextures(texture, stringToInt(element.GetAttribute("tileSize")), TileSetType.OneOfEach);
                else if(element.Name == "RandomOf4")
                    d[stringToInt(element.GetAttribute("id"))] = GetTileSetTextures(texture, stringToInt(element.GetAttribute("tileSize")), TileSetType.RandomOf4);
            }

            return d;
        }

        public enum TileSetType { OneOfEach, RandomOf4 }
        public static Dictionary<string, Texture2D> GetTileSetTextures(Texture2D tileset, int tileSize, TileSetType type)
        {
            Dictionary<string, Texture2D> d = new();

            switch (type)
            {
                case TileSetType.OneOfEach:
                    string[,] tileNames = new string[,] {
                        { "top", "bottom", "left", "right" },
                        { "topLeftCorner", "topRightCorner", "bottomLeftCorner", "bottomRightCorner" },
                        { "topLeftPoint", "topRightPoint", "bottomleftPoint", "bottomRightPoint" },
                        { "upFullCorner", "downFullCorner", "leftFullCorner", "rightFullCorner" },
                        { "verticalPillar", "horizontalPillar", "complete", "inside" }
                    };
                    for (int y = 0; y < 5; y++)
                        for (int x = 0; x < 4; x++)
                        {
                            d[tileNames[y, x]] = tileset.CropTo(new Vector2(x * tileSize, y * tileSize), new Vector2(tileSize));
                            d[tileNames[y, x]].Name = tileNames[y, x];
                        }
                    break;

                case TileSetType.RandomOf4:
                    string[] tileNamesR4 = new string[] {
                        "top", "bottom", "left", "right", "topLeftCorner", "topRightCorner", "bottomLeftCorner", "bottomRightCorner", "topLeftPoint", "topRightPoint", "bottomLeftPoint", "bottomRightPoint", "upFullCorner", "downFullCorner", "leftFullCorner", "rightFullCorner", "verticalPillar", "horizontalPillar", "complete" };
                    string[] tileNamesR4Other = new string[] { "oppositePointsTopLeft", "oppositePointsTopRight", "allPoints", "inside",
                    "doublePointTop", "doublePointBottom", "doublePointLeft", "doublePointRight",
                    "triplePointTopLeft", "triplePointTopRight", "triplePointBottomLeft", "triplePointBottomRight", };

                    for(int y = 0; y < tileNamesR4.Length; y++)
                        for(int x = 0; x < 4; x++)
                        {
                            d[tileNamesR4[y] + x] = tileset.CropTo(new Vector2(x * tileSize, y * tileSize), new Vector2(tileSize));
                            d[tileNamesR4[y] + x].Name = tileNamesR4[y] + x;
                        }

                    for (int y = 0; y < tileNamesR4Other.Length / 4; y++)
                        for (int x = 0; x < 4; x++)
                        {
                            d[tileNamesR4Other[y * 4 + x]] = tileset.CropTo(new Vector2(x * tileSize, (y + tileNamesR4.Length) * tileSize), new Vector2(tileSize));
                            d[tileNamesR4Other[y * 4 + x ]].Name = tileNamesR4Other[y * 4 + x];
                        }

                    for (int y = 0; y < 2; y++)
                        for (int x = 0; x < 4; x++)
                        {
                            d["padding" + (y * 4 + x + 1)] = tileset.CropTo(new Vector2(x * tileSize, (y + tileNamesR4.Length + tileNamesR4Other.Length / 4) * tileSize), new Vector2(tileSize));
                            d["padding" + (y * 4 + x + 1)].Name = "padding" + (y * 4 + x + 1) ;
                        }

                    break;
            };

            return d;
        }

        public static Dictionary<Point, Texture2D> GetTileSetTextures(Texture2D tileset, int tileSize)
        {
            Dictionary<Point, Texture2D> tiles = new();
            for(int x = 0; x < tileset.Width; x += tileSize)
                for (int y = 0; y < tileset.Height; y += tileSize)
                {
                    tiles[new Point(x, y)] = tileset.CropTo(new Vector2(x, y), new Vector2(tileSize));
                    tiles[new Point(x, y)].Name = new Point(x, y).ToString();
                }
        return tiles;
        }

        /// <summary>
        /// Returns a random texture between four of the identified tile
        /// </summary>
        /// <param name="tileset">Tileset must be of type RandomOf4</param>
        /// <param name="identifier">The specified tile identifier</param>
        /// <returns></returns>
        public static Texture2D GetRandomTilesetTexture(Dictionary<string, Texture2D> tileset, string identifier, Random random = null)
            => tileset[identifier + (random == null ? Rand.NextInt(0, 4) : random.Next(0, 4))];

        public static Texture2D CropTo(this Texture2D texture, Vector2 position, Vector2 size)
        {
            Color[] data = new Color[(int)(size.X * size.Y)];
            Texture2D returned = new Texture2D(Engine.Graphics.GraphicsDevice, (int)size.X, (int)size.Y);
            texture.GetData(0, new Rectangle(position.ToPoint(), size.ToPoint()), data, 0, (int)(size.X * size.Y));
            data[0].A = data[0].A;
            returned.SetData(data);
            return returned;
        }

        private static int stringToInt(string str)
            => int.Parse(str, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

        public static Texture2D Load(string path)
        {
            Textures.TryGetValue(path, out Texture2D texture);
            if(texture != null)
                return texture;

            object loaded = Content.Load<Object>(path);
            if (loaded is Texture2D tex) 
            {
                Textures[path] = tex;
                return tex;
            }
            else if (loaded is AsepriteFile asepriteDoc)
            {
                Textures[path] = asepriteDoc.Load()[0];
                return Textures[path];
            }
            else
                throw new Exception("File was not found under the right format");
        }

        public static Texture2D[] Load(this AsepriteFile doc) 
        {
            Texture2D[] result = new Texture2D[doc.Frames.Length];
            for (int i = 0; i < doc.Frames.Length; i++)
            {
                result[i] = doc.CreateSprite(Engine.Graphics.GraphicsDevice, i, true, false, false).TextureRegion.Texture;//  .Texture.CropTo(new Vector2(doc.Frames[i].X, doc.Frames[i].Y), new Vector2(doc.Frames[i].Width, doc.Frames[i].Height));
            }

            result[0].Tag = new object[2];
            List<Sprite.Animation.Slice> slices = new();

            foreach(AsepriteSlice slice in doc.Slices)
            {
                slices.Add(new Sprite.Animation.Slice(slice.Name, new Rectangle(slice.Keys[0].Bounds.X, slice.Keys[0].Bounds.Y, slice.Keys[0].Bounds.Width, slice.Keys[0].Bounds.Height), slice.UserData.Color.Value.ToXnaColor()));
            }

            ((object[])result[0].Tag)[1] = slices;

            foreach(AsepriteTag tag in doc.Tags)
            {
                for (int i = tag.From; i <= tag.To; i++)
                {
                    if (i != 0)
                        result[i].Tag = tag.Name;
                    else
                        ((object[])result[0].Tag)[0] = tag.Name;
                }
            }

            return result;
        }

        //TODO: Refractor all of this to have a class that encapsulates both a simple Texture2D and an Aseprite Document to have a lot more data easily
        /*public struct TextureData
        {
            public string Tag = "";
            public float Duration = 0f;
            public List<Sprite.Animation.Slice> Slices = null;

            public TextureData() { }
        }*/

        public static string[] GetAllFMODBanksPaths()
        {
            if (!Directory.Exists(Content.RootDirectory + "/Audio/Desktop"))
                return new string[] {};

            DirectoryInfo dir = new DirectoryInfo(Content.RootDirectory + "/Audio/Desktop");
            List<string> paths = new();

            foreach(FileInfo f in dir.GetFiles())
            {
                if (f.Extension == ".bank")
                    paths.Add(f.FullName);
            }

            return paths.ToArray();
        }

        public static Texture2D FlipXAndY(this Texture2D texture)
        {
            Texture2D flipped = new Texture2D(Engine.Graphics.GraphicsDevice, texture.Width, texture.Height);
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);

            Color[] reversed = new Color[texture.Width * texture.Height];

            for(int i = 0; i < data.Length; i++)
                reversed[data.Length - 1 - i] = data[i];

            flipped.Name = texture.Name + " FlippedXAndY";
            flipped.SetData(reversed);
            return flipped;
        }

        public static Texture2D FlipX(this Texture2D texture)
        {
            Texture2D flipped = new Texture2D(Engine.Graphics.GraphicsDevice, texture.Width, texture.Height);
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);

            Color[] reversed = new Color[texture.Width * texture.Height];

            for(int y = 0; y < texture.Height; y++)
            {
                for(int x = 0; x < texture.Width; x++)
                {
                    int coords = x + y * texture.Width;
                    int reversedCoords = Math.Abs(x + 1 - texture.Width) + y * texture.Width;
                    reversed[reversedCoords] = data[coords];
                }
            }

            flipped.Name = texture.Name + " FlippedX";
            flipped.SetData(reversed);
            return flipped;
        }

        public static Texture2D FlipY(this Texture2D texture)
        {
            Texture2D flipped = new Texture2D(Engine.Graphics.GraphicsDevice, texture.Width, texture.Height);
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);

            Color[] reversed = new Color[texture.Width * texture.Height];

            for (int y = 0; y < texture.Height; y++)
            {
                for (int x = 0; x < texture.Width; x++)
                {
                    int coords = x + y * texture.Width;
                    int reversedCoords = (texture.Height - 1 - y) * texture.Width + x;
                    reversed[reversedCoords] = data[coords];
                }
            }

            flipped.Name = texture.Name + " FlippedY";
            flipped.SetData(reversed);
            return flipped;
        }

        public static Texture2D Rotate90(this Texture2D texture)
        {
            Texture2D flipped = new Texture2D(Engine.Graphics.GraphicsDevice, texture.Width, texture.Height);
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);

            Color[] reversed = new Color[texture.Width * texture.Height];

            for (int y = 0; y < texture.Height; y++)
            {
                for (int x = 0; x < texture.Width; x++)
                {
                    int coords = x + y * texture.Width;
                    int rotated = y - (x + 1 - texture.Width) * texture.Width;
                    reversed[coords] = data[rotated];
                }
            }

            flipped.Name = texture.Name + " Rotated90";
            flipped.SetData(reversed);
            return flipped;
        }
    }
}
