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

        public float Width { get => Texture.Width; }
        public float Height { get => Texture.Height; }

        public Action OnLastFrame;
        public Action OnLoop;
        public Action OnChange;
        public Action OnFrameChange;

        public Texture2D Texture;
        public NineSlice NineSliceSettings;
        public float Rotation = 0;

        public Color Color = Color.White;
        public Vector2 Origin = Vector2.Zero;
        public Vector2 Offset = Vector2.Zero;
        public Vector2 Scale = Vector2.One;
        public SpriteEffects Effect = SpriteEffects.None;
        public float LayerDepth = 0.5f;

        public Rectangle? DesinationRectangle = null;
        public Rectangle? SourceRectangle = null;
        public bool Centered;

        public Texture2D CurrentAnimationFrame => 
            CurrentAnimation.Frames[CurrentFrame];
        public Animation CurrentAnimation;
        public int CurrentFrame;

        private Dictionary<string, Animation> animations = new();
        private bool animating;
        private float animTimer;

        public override string ToString()
            => $"Texture: {Texture.Name}, {Color}, layerDepth: {LayerDepth}, Rect: {DesinationRectangle}, Origin {Origin}, " +
                $"Scale: {Scale}, Rotation {Rotation}, SpriteEffects: {Effect}";

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
        }

        public Sprite(Texture2D texture, Rectangle rect)
        {
            Texture = texture;
        }

        public Sprite(Texture2D texture, Rectangle rect, float rotation)
        {
            Texture = texture;
            Rotation = MathHelper.ToRadians(rotation);
        }

#endregion

        public override void Update()
        {
            if (!animating)
                return;

            animTimer += Engine.Deltatime;

            if (animTimer < CurrentAnimation.Delay)
                return;

            //Next Frame

            animTimer -= CurrentAnimation.Delay;
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
                        if(animations[CurrentAnimation.GoTo] == CurrentAnimation)
                        {
                            CurrentFrame = 0;
                            Texture = CurrentAnimation.Frames[CurrentFrame];
                        }
                        else
                        {
                            Play(CurrentAnimation.GoTo);
                            OnChange?.Invoke();
                        }
                    }
                    //Ended
                    else
                    {
                        animating = false;
                        CurrentFrame = 0;
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
            {
                NineSliceSettings.Draw(this);
            }

            if (Texture == null)
                return;

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
                    Drawing.Draw(Texture, ParentEntity.Pos + Offset, SourceRectangle, Color, Rotation, Origin, new Vector2(rect.Width, rect.Height) * Scale, Effect, LayerDepth);
                }
                else
                    Drawing.Draw(Texture, (Rectangle)DesinationRectangle, Color, Rotation, Origin, Scale, Effect, LayerDepth);
            }
            else if (Centered)
                Drawing.Draw(Texture, ParentEntity.Pos + ParentEntity.HalfSize + Offset, SourceRectangle, Color, Rotation, Origin,
                    Scale, SpriteEffects.None, 1);
            else
                Drawing.Draw(Texture, ParentEntity.Pos + Offset, SourceRectangle, Color, Rotation, Origin, Scale, Effect, LayerDepth);
        }


        public void Draw(Vector2 position)
            => Drawing.Draw(Texture, position, SourceRectangle, Color, Rotation, Origin, Scale, Effect, LayerDepth);

        public void Add(string id, Animation animation)
            => animations[id] = animation;

        public void Add(AnimData animData)
        {
            foreach(KeyValuePair<string, Animation> data in animData.Animations)
                animations[data.Key] = data.Value;

            if (CurrentAnimation == null)
                 Play(animData.StartAnimationId);
        }

        public void Play(string id)
        {
#if DEBUG
            if (!animations.ContainsKey(id))
                throw new Exception("No Animation defined for Id " + id);
#endif
            CurrentAnimation = animations[id];
            Texture = CurrentAnimation.Frames[0];
            CurrentFrame = 0;
            animTimer = 0;
            animating = true;
        }

        public class Animation
        {
            public Animation(Texture2D[] frames, float delay, string goTo)
            {
                Frames = frames;
                Delay = delay;
                GoTo = goTo;


                Slices = ((List<Slice>)((object[])Frames[0].Tag)[1]).ToArray();
                Frames[0].Tag = ((object[])Frames[0].Tag)[0];
            }

            public float Delay;
            public Texture2D[] Frames;
            public string GoTo;

            public Slice[] Slices;

            public class Slice
            {
                public Rectangle Rect;
                public NineSlice NiceSlice;
                public string Name;

                public Slice(string name, Rectangle rect, NineSlice niceSlice)
                {
                    Name = name;
                    Rect = rect;
                    NiceSlice = niceSlice;
                }

                public Slice(string name, Rectangle rect)
                {
                    Name = name;
                    Rect = rect;
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
                    if (anim.Name == "Anim")
                        AllAnimData[element.Name].Animations[anim.GetAttribute("id")] = 
                            new Animation(
                                DataManager.LoadAllGraphicsWithName(anim.GetAttribute("path"), path),
                                float.Parse(anim.GetAttribute("delay"),
                                System.Globalization.CultureInfo.InvariantCulture.NumberFormat),
                                anim.GetAttribute("goto"));

                    else if (anim.Name == "Loop")
                        AllAnimData[element.Name].Animations[anim.GetAttribute("id")] = new Animation(DataManager.LoadAllGraphicsWithName(anim.GetAttribute("path"), path), float.Parse(anim.GetAttribute("delay"), System.Globalization.CultureInfo.InvariantCulture.NumberFormat), anim.GetAttribute("id"));
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
            s.Effect = Effect;
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
