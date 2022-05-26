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

        private Dictionary<string, Animation> animations = new();
        private bool animating;
        private Animation currentAnimation;
        private int currentFrame;
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
            Texture = Drawing.pointTexture;
            Color = color;
        }

        public Sprite(Color color, Rectangle? rect, float layerDepth = 0)
        {
            Texture = Drawing.pointTexture;
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

            if (animTimer < currentAnimation.Delay)
                return;

            //Next Frame

            animTimer -= currentAnimation.Delay;
            currentFrame++;

            //End
            if(currentFrame + 1 > currentAnimation.Frames.Length)
            {
                Animation was = currentAnimation;
                OnLastFrame?.Invoke();

                //OnLastFrame didn't change the animation
                if(currentAnimation == was)
                {
                    //Looping or going to next animation
                    if (currentAnimation.GoTo != "" && currentAnimation.GoTo != null)
                    {
                        //Loop
                        if(animations[currentAnimation.GoTo] == currentAnimation)
                        {
                            currentFrame = 0;
                            Texture = currentAnimation.Frames[currentFrame];
                        }
                        else
                        {
                            Play(currentAnimation.GoTo);
                        }
                    }
                    //Ended
                    else
                    {
                        animating = false;
                        currentFrame = 0;
                    }
                }
            }
            else
                Texture = currentAnimation.Frames[currentFrame];
        }

        public override void Render()
        {
            if(NineSliceSettings != NineSliceSettings.Empty)
            {
                NineSliceSettings n = NineSliceSettings;

                DrawNineSlice(n.TopLeft, ParentEntity.Pos, Size(n.TopLeft));
                DrawNineSlice(n.TopRight, ParentEntity.Pos + new Vector2(ParentEntity.Width - n.TopRight.Width, 0), Size(n.TopRight));
                DrawNineSlice(n.BottomLeft, ParentEntity.Pos + new Vector2(0, ParentEntity.Height - n.BottomLeft.Height), Size(n.BottomLeft));
                DrawNineSlice(n.BottomRight, ParentEntity.Pos + new Vector2(ParentEntity.Width - n.BottomRight.Width, ParentEntity.Height - n.BottomRight.Height), Size(n.BottomRight));

                if (n.Repeat)
                {
                    for (int i = 0; i < ParentEntity.Width - n.TopLeft.Width - n.TopRight.Width; i += n.Top.Width)
                        DrawNineSlice(n.Top, ParentEntity.Pos + new Vector2(n.TopLeft.Width + i, 0), Size(n.Top));

                    for (int i = 0; i < ParentEntity.Width - n.BottomLeft.Width - n.BottomRight.Width; i += n.Bottom.Width)
                        DrawNineSlice(n.Bottom, ParentEntity.Pos + new Vector2(n.BottomLeft.Width + i, ParentEntity.Height - n.Bottom.Height), Size(n.Bottom));

                    for (int i = 0; i < ParentEntity.Height - n.TopRight.Height - n.BottomRight.Height; i += n.Right.Height)
                        DrawNineSlice(n.Right, ParentEntity.Pos + new Vector2(ParentEntity.Width - n.TopRight.Width, n.TopRight.Height + i), Size(n.Right));

                    for (int i = 0; i < ParentEntity.Height - n.TopLeft.Height - n.BottomLeft.Height; i += n.Left.Height)
                        DrawNineSlice(n.Left, ParentEntity.Pos + new Vector2(0, n.TopLeft.Height + i), Size(n.Left));

                    for (int x = n.TopLeft.Width; x <= ParentEntity.Width - n.TopLeft.Width - n.BottomRight.Width; x += n.Fill.Width)
                        for (int y = n.TopLeft.Height; y <= ParentEntity.Height - n.TopLeft.Height - n.BottomRight.Height; y += n.Fill.Height)
                            DrawNineSlice(n.Fill, ParentEntity.Pos + new Vector2(x, y), 
                                new Vector2(Math.Min(n.Fill.Width, ParentEntity.Width - n.Right.Width - x), Math.Min(n.Fill.Height, ParentEntity.Height - n.Bottom.Height - y)));

                }
                else
                {
                    DrawNineSlice(n.Top, ParentEntity.Pos + new Vector2(n.TopLeft.Width, 0), new Vector2(ParentEntity.Width - n.TopLeft.Width - n.TopRight.Width, n.Top.Height));
                    DrawNineSlice(n.Bottom, ParentEntity.Pos + new Vector2(n.BottomLeft.Width, ParentEntity.Height - n.Bottom.Height), new Vector2(ParentEntity.Width - n.BottomLeft.Width - n.BottomRight.Width, n.Bottom.Height));
                    DrawNineSlice(n.Right, ParentEntity.Pos + new Vector2(ParentEntity.Width - n.TopRight.Width, n.TopRight.Height), new Vector2(n.Right.Width, ParentEntity.Height - n.TopRight.Height - n.BottomRight.Height));
                    DrawNineSlice(n.Left, ParentEntity.Pos + new Vector2(0, n.TopLeft.Height), new Vector2(n.Left.Width, ParentEntity.Height - n.TopLeft.Height - n.BottomLeft.Height));

                    DrawNineSlice(n.Fill, ParentEntity.Pos + Size(n.TopLeft), ParentEntity.Size - Size(n.TopLeft) - Size(n.BottomRight));
                }

                return;

                static Vector2 Size(Texture2D texture) => new Vector2(texture.Width, texture.Height);

                void DrawNineSlice(Texture2D texture, Vector2 position, Vector2 size)
                {
                    Drawing.Draw(texture, position + Offset, size * Scale, Color, Rotation, Origin, Effect, LayerDepth);
                    /*Drawing.DrawEdge(new Rectangle(position.ToPoint(), size.ToPoint()), 1, new Color(){ R = 255, G = 255, B = 255, A = 50 });*/
                }
            }

            if (Texture == null)
                return;

            if (Texture == Drawing.pointTexture)
            {
                if (desinationRectangle == null)
                {
                    Rectangle rect = ParentEntity.Bounds;
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
            currentAnimation = animations[id];
            Texture = currentAnimation.Frames[0];
            currentFrame = 0;
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
            }

            public float Delay;
            public Texture2D[] Frames;
            public string GoTo;
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
                        AllAnimData[element.Name].Animations[anim.GetAttribute("id")] = new Animation(DataManager.LoadAllGraphicsWithName(anim.GetAttribute("path"), path), float.Parse(anim.GetAttribute("delay"), System.Globalization.CultureInfo.InvariantCulture.NumberFormat), anim.GetAttribute("goto"));
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
            s.currentAnimation = currentAnimation;
            s.currentFrame = currentFrame ;
            s.animTimer = animTimer;

            return s;
    }
    }
}
