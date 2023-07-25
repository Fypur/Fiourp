using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Fiourp
{
    public class Sprite : Renderer
    {
        public static readonly Dictionary<string, AnimData> AllAnimData = new();

        public enum DrawMode { Centered, TopLeft, ScaledTopLeft }

        public int Width { get => Texture.Width; }
        public int Height { get => Texture.Height; }

        public Action OnLastFrame;
        public Action OnLoop;
        public Action OnChange;
        public Action OnFrameChange;

        public Texture2D Texture;
        public NineSlice NineSliceSettings;
        private float rotation = 0;
        public float Rotation { get => rotation;
            set => rotation = value % ((float)Math.PI * 2); }

        public Color Color = Color.White;
        public Vector2 Origin = Vector2.Zero; //This is relative to the size of the TEXTURE not the entity
        public Vector2 Offset = Vector2.Zero;
        public Vector2 Scale = Vector2.One;
        public Effect PixelShader = null;
        public SpriteEffects SpriteEffect = SpriteEffects.None;
        public float LayerDepth = 0.5f;

        public Rectangle? DesinationRectangle = null;
        public Rectangle? SourceRectangle = null;
        public bool Centered;

        public Texture2D CurrentAnimationFrame => 
            CurrentAnimation.Frames[CurrentFrame];
        public Animation CurrentAnimation;
        public int CurrentFrame;
        private int currentLoopAmount;

        private Dictionary<string, Animation> animations = new();
        private bool animating;
        private float animTimer;

        public override string ToString()
            => $"Sprite: Texture: {Texture.Name}, {Color}, layerDepth: {LayerDepth}, Rect: {DesinationRectangle}, Origin {Origin}, " +
                $"Scale: {Scale}, Rotation {Rotation}, SpriteEffects: {SpriteEffect}";

        #region Constructors

        public static Sprite None => new Sprite();

        public Sprite() { }

        public Sprite(Texture2D texture)
        {
            Texture = texture;
        }

        public Sprite(NineSlice nineSliceSettings)
        {
            NineSliceSettings = nineSliceSettings;
        }

        public Sprite(Texture2D texture, Vector2 origin)
        {
            Texture = texture;
            Origin = origin;
        }

        public Sprite(Color color)
        {
            Texture = Drawing.PointTexture;
            Color = color;
        }

        public Sprite(Color color, Rectangle? rect, float layerDepth = 0)
        {
            Texture = Drawing.PointTexture;
            Color = color;
            LayerDepth = layerDepth;
            DesinationRectangle = rect;
        }

        public Sprite(Texture2D texture, Rectangle rect)
        {
            Texture = texture;
            DesinationRectangle = rect;
        }

        public Sprite(Texture2D texture, Rectangle rect, float rotation)
        {
            Texture = texture;
            Rotation = MathHelper.ToRadians(rotation);
            DesinationRectangle = rect;
        }

#endregion

        public override void Update()
        {
            if (NineSliceSettings != null)
                NineSliceSettings.Update();

            if (!animating)
                return;

            animTimer += Engine.Deltatime;

            if (animTimer < CurrentAnimation.Delay[CurrentFrame])
                return;

            //Next Frame

            animTimer -= CurrentAnimation.Delay[CurrentFrame];
            CurrentFrame++;
            OnFrameChange?.Invoke();

            //End
            if (CurrentFrame + 1 > CurrentAnimation.Frames.Length)
            {
                Animation was = CurrentAnimation;
                OnLastFrame?.Invoke();

                //OnLastFrame didn't change the animation
                if(CurrentAnimation == was)
                {
                    //Looping or going to next animation
                    if (CurrentAnimation.GoTo != "" && CurrentAnimation.GoTo != null)
                    {
                        //Loop
                        if((animations[CurrentAnimation.GoTo] == CurrentAnimation || CurrentAnimation.IsLoop) && currentLoopAmount < CurrentAnimation.LoopAmount)
                        {
                            CurrentFrame = 0;
                            currentLoopAmount++;
                            Texture = CurrentAnimation.Frames[CurrentFrame];
                        }
                        else
                        {
                            Play(CurrentAnimation.GoTo);
                        }
                    }
                    //Ended
                    else
                    {
                        animating = false;
                        CurrentFrame = 0;
                        currentLoopAmount = 0;
                    }
                }
                else
                    OnChange?.Invoke();
            }
            else
                Texture = CurrentAnimation.Frames[CurrentFrame];
        }

        public override void Render()
        {
            if(NineSliceSettings != null)
                NineSliceSettings.Draw(this);

            if (Texture == null)
                return;

            if (PixelShader != Drawing.GetCurrentPixelShader())
                Drawing.SwitchPixelShader(PixelShader);

            if (Texture == Drawing.PointTexture)
            {
                if (DesinationRectangle == null)
                {
                    Rectangle rect;
                    if (ParentEntity != null)
                        rect = ParentEntity.Bounds;
                    else
                        rect = Texture.Bounds;

                    rect.Location += Offset.ToPoint();
                    Drawing.Draw(Texture, ParentEntity.Pos + Offset, SourceRectangle, Color, Rotation, Origin, new Vector2(rect.Width, rect.Height) * Scale, SpriteEffect, LayerDepth);
                }
                else
                    Drawing.Draw(Texture, (Rectangle)DesinationRectangle, Color, Rotation, Origin, Scale, SpriteEffect, LayerDepth);
            }
            else if (Centered)
                Drawing.Draw(Texture, ParentEntity.Pos + ParentEntity.HalfSize + Offset, SourceRectangle, Color, Rotation, Origin,
                    Scale, SpriteEffects.None, 1);
            else
                Drawing.Draw(Texture, ParentEntity.Pos + Offset, SourceRectangle, Color, Rotation, Origin, Scale, SpriteEffect, LayerDepth);
        }


        public void Draw(Vector2 position)
            => Drawing.Draw(Texture, position, SourceRectangle, Color, Rotation, Origin, Scale, SpriteEffect, LayerDepth);

        public void Add(string id, Animation animation)
            => animations[id] = animation;

        public void Add(AnimData animData)
        {
            foreach(KeyValuePair<string, Animation> data in animData.Animations)
                animations[data.Key] = data.Value;

            if (CurrentAnimation == null && animData.StartAnimationId != "")
                 Play(animData.StartAnimationId);
        }

        public void Play(string id)
        {
#if DEBUG
            if (!animations.ContainsKey(id))
                throw new Exception("No Animation defined for Id " + id);
#endif
            if (CurrentAnimation != null && CurrentAnimation.Frames != animations[id].Frames)
                OnChange?.Invoke();

            CurrentAnimation = animations[id];
            Texture = CurrentAnimation.Frames[0];
            CurrentFrame = 0;
            animTimer = 0;
            animating = true;
            currentLoopAmount = 0;
        }

        public class Animation
        {
            public Animation(Texture2D[] frames, float[] delay, string goTo, bool isLoop = false, int loopAmount = 1)
            {
                Frames = frames;
                Delay = delay;
                GoTo = goTo;
                IsLoop = isLoop;
                LoopAmount = loopAmount;


                Slices = ((List<Slice>)((object[])Frames[0].Tag)[1]).ToArray();
                if(Slices.Length != 0)
                { }
                Frames[0].Tag = ((object[])Frames[0].Tag)[0];
            }

            public float[] Delay;
            public Texture2D[] Frames;
            public string GoTo;
            public bool IsLoop;
            public int LoopAmount;

            public Slice[] Slices;

            public class Slice
            {
                public Rectangle Rect;
                public NineSlice NiceSlice;
                public string Name;
                public Color Color;

                public Slice(string name, Rectangle rect, NineSlice niceSlice, Color color)
                {
                    Name = name;
                    Rect = rect;
                    NiceSlice = niceSlice;
                    Color = color;
                }

                public Slice(string name, Rectangle rect, Color color)
                {
                    Name = name;
                    Rect = rect;
                    Color = color;
                }
            }
        }

        public class AnimData
        {
            public readonly Dictionary<string, Animation> Animations = new();
            public string StartAnimationId;
        }

        public static void LoadAnimationXML(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(File.ReadAllText(path));
            LoadFolderXML("", doc["Sprites"]["Animations"]);
        }

        private static void LoadFolderXML(string path, XmlElement parent)
        {
            foreach(XmlElement element in parent)
            {
                if(element.Name == "Folder")
                {
                    LoadFolderXML(path + "/" + element.GetAttribute("path"), element);
                    continue;
                }

                AllAnimData[element.Name] = new();
                path += "/" + element.GetAttribute("path");

                foreach (XmlElement anim in element)
                {
                    string id = anim.GetAttribute("id");
                    string animPath = anim.GetAttribute("path");
                    string animGoto = anim.GetAttribute("goto");

                    Texture2D[] textures = DataManager.LoadAllGraphicsWithName(animPath, path);

                    string d = anim.GetAttribute("delay");
                    
                    float[] delays = new float[textures.Length];

                    if (d != "")
                    {
                        float delay = float.Parse(d, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                        Array.Fill(delays, delay);
                    }
                    else
                        delays = DataManager.GetAnimationDelays(path + '/' + animPath);
                    

                    if (anim.Name == "Anim")
                        AllAnimData[element.Name].Animations[id] = new Animation(textures, delays, animGoto);

                    else if (anim.Name == "Loop")
                    {
                        int loopAmount = 1;
                        
                        if(int.TryParse(anim.GetAttribute("loop"), out int l))
                            loopAmount = l;

                        if (animGoto == "")
                            animGoto = anim.GetAttribute("id");

                        AllAnimData[element.Name].Animations[anim.GetAttribute("id")] = new Animation(textures, delays, animGoto, true, loopAmount);
                    }
                }

                AllAnimData[element.Name].StartAnimationId = element.GetAttribute("start");
                path = path.Remove(path.LastIndexOf("/" + element.GetAttribute("path")));
            }
        }

        public Sprite Copy()
        {
            Sprite s = new Sprite();

            s.OnLastFrame = OnLastFrame;
            s.OnLoop = OnLoop ;
            s.OnChange = OnChange ;
            s.Texture = Texture ;
            s.Rotation = Rotation;
            s.Color = Color;
            s.Origin = Origin;
            s.Offset = Offset;
            s.Scale = Scale;
            s.SpriteEffect = SpriteEffect;
            s.LayerDepth = LayerDepth;
            s.DesinationRectangle = DesinationRectangle;
            s.Centered = Centered ;
            s.animations = animations;
            s.animating = animating ;
            s.CurrentAnimation = CurrentAnimation;
            s.CurrentFrame = CurrentFrame ;
            s.animTimer = animTimer;

            return s;
        }
    }
}
