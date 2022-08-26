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
        public NineSliceSettings NineSliceSettings;
        public float Rotation = 0;

        public Color Color = Color.White;
        public Vector2 Origin = Vector2.Zero;
        public Vector2 Offset = Vector2.Zero;
        public Vector2 Scale = Vector2.One;
        public SpriteEffects Effect = SpriteEffects.None;
        public float LayerDepth = 0.5f;

        public Rectangle? desinationRectangle = null;
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
            => $"Texture: {Texture.Name}, {Color}, layerDepth: {LayerDepth}, Rect: {desinationRectangle}, Origin {Origin}, " +
                $"Scale: {Scale}, Rotation {Rotation}, SpriteEffects: {Effect}";

        #region Constructors

        public static Sprite None => new Sprite();

        public Sprite() { }

        public Sprite(Texture2D texture)
        {
            Texture = texture;
        }

        public Sprite(NineSliceSettings nineSliceSettings)
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
            if(NineSliceSettings != NineSliceSettings.Empty)
            {
                DrawNineSlice(NineSliceSettings);
            }

            if (Texture == null)
                return;

            if (Texture == Drawing.PointTexture)
            {
                if (desinationRectangle == null)
                {
                    Rectangle rect;
                    if (ParentEntity != null)
                        rect = ParentEntity.Bounds;
                    else
                        rect = Texture.Bounds;

                    rect.Location += Offset.ToPoint();
                    Drawing.Draw(Texture, rect, Color, Rotation, Origin, Scale, Effect, LayerDepth);
                }
                else
                    Drawing.Draw(Texture, (Rectangle)desinationRectangle, Color, Rotation, Origin, Scale, Effect, LayerDepth);
            }
            else if (Centered)
                Drawing.Draw(Texture, ParentEntity.Pos + ParentEntity.HalfSize + Offset, SourceRectangle, Color, Rotation, Origin,
                    Vector2.One, SpriteEffects.None, 1);
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
                public NineSliceSettings NiceSlice;
                public string Name;

                public Slice(string name, Rectangle rect, NineSliceSettings niceSlice)
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
            s.desinationRectangle = desinationRectangle;
            s.Centered = Centered ;
            s.animations = animations;
            s.animating = animating ;
            s.CurrentAnimation = CurrentAnimation;
            s.CurrentFrame = CurrentFrame ;
            s.animTimer = animTimer;

            return s;
        }

        private void DrawNineSlice(NineSliceSettings nineSliceSettings)
        {
            NineSliceSettings n = NineSliceSettings;

            DrawSlice(n.TopLeft, ParentEntity.Pos, Size(n.TopLeft));
            DrawSlice(n.TopRight, ParentEntity.Pos + new Vector2(ParentEntity.Width - Size(n.TopRight).X, 0), Size(n.TopRight));
            DrawSlice(n.BottomLeft, ParentEntity.Pos + new Vector2(0, ParentEntity.Height - Size(n.BottomLeft).Y), Size(n.BottomLeft));
            DrawSlice(n.BottomRight, ParentEntity.Pos + new Vector2(ParentEntity.Width - Size(n.BottomRight).X, ParentEntity.Height - Size(n.BottomRight).Y), Size(n.BottomRight));

            if (n.Repeat)
            {
                if(n.Top != null)
                    for (int i = 0; i < ParentEntity.Width - Size(n.TopLeft).X - Size(n.TopRight).X; i += Size(n.Top).X)
                        DrawSlice(n.Top, ParentEntity.Pos + new Vector2(Size(n.TopLeft).X + i, 0), Size(n.Top));

                if (n.Bottom != null)
                    for (int i = 0; i < ParentEntity.Width - Size(n.BottomLeft).X - Size(n.BottomRight).X; i += Size(n.Bottom).X)
                        DrawSlice(n.Bottom, ParentEntity.Pos + new Vector2(Size(n.BottomLeft).X + i, ParentEntity.Height - Size(n.Bottom).Y), Size(n.Bottom));

                if (n.Right != null)
                    for (int i = 0; i < ParentEntity.Height - Size(n.TopRight).Y - Size(n.BottomRight).Y; i += Size(n.Right).Y)
                        DrawSlice(n.Right, ParentEntity.Pos + new Vector2(ParentEntity.Width - Size(n.TopRight).X, Size(n.TopRight).Y + i), Size(n.Right));

                if (n.Left != null)
                    for (int i = 0; i < ParentEntity.Height - Size(n.TopLeft).Y - Size(n.BottomLeft).Y; i += Size(n.Left).Y)
                        DrawSlice(n.Left, ParentEntity.Pos + new Vector2(0, Size(n.TopLeft).Y + i), Size(n.Left));

                if (n.Fill != null)
                    for (int x = Size(n.TopLeft).X; x <= ParentEntity.Width - Size(n.TopLeft).X - Size(n.BottomRight).X; x += Size(n.Fill).X)
                        for (int y = Size(n.TopLeft).Y; y <= ParentEntity.Height - Size(n.TopLeft).Y - Size(n.BottomRight).Y; y += Size(n.Fill).Y)
                            DrawSlice(n.Fill, ParentEntity.Pos + new Vector2(x, y),
                                new Point(Math.Min(Size(n.Fill).X, ParentEntity.Width - Size(n.Right).X - x), Math.Min(Size(n.Fill).Y, ParentEntity.Height - Size(n.Bottom).Y - y)));

            }
            else
            {
                DrawSlice(n.Top, ParentEntity.Pos + new Vector2(Size(n.TopLeft).X, 0), new Point(ParentEntity.Width - Size(n.TopLeft).X - Size(n.TopRight).X, Size(n.Top).Y));
                DrawSlice(n.Bottom, ParentEntity.Pos + new Vector2(Size(n.BottomLeft).X, ParentEntity.Height - Size(n.Bottom).Y), new Point(ParentEntity.Width - Size(n.BottomLeft).X - Size(n.BottomRight).X, Size(n.Bottom).Y));
                DrawSlice(n.Right, ParentEntity.Pos + new Vector2(ParentEntity.Width - Size(n.TopRight).X, Size(n.TopRight).Y), new Point(Size(n.Right).X, ParentEntity.Height - Size(n.TopRight).Y - Size(n.BottomRight).Y));
                DrawSlice(n.Left, ParentEntity.Pos + new Vector2(0, Size(n.TopLeft).Y), new Point(Size(n.Left).X, ParentEntity.Height - Size(n.TopLeft).Y - Size(n.BottomLeft).Y));

                DrawSlice(n.Fill, ParentEntity.Pos + Size(n.TopLeft).ToVector2(), ParentEntity.Size.ToPoint() - Size(n.TopLeft) - Size(n.BottomRight));
            }

            return;

            static Point Size(Texture2D texture) => texture != null ? new Point(texture.Width, texture.Height) : Point.Zero;

            void DrawSlice(Texture2D texture, Vector2 position, Point size)
            {
                if(texture != null)
                    Drawing.Draw(texture, position + Offset, size.ToVector2() * Scale, Color, Rotation, Origin, Effect, LayerDepth);
                /*Drawing.DrawEdge(new Rectangle(position.ToPoint(), size.ToPoint()), 1, new Color(){ R = 255, G = 255, B = 255, A = 50 });*/
            }
        }
    }
}
