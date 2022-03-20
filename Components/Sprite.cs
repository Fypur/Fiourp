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
        public float Rotation = 0;

        public Color Color = Color.White;
        public Vector2 Origin = Vector2.Zero;
        public Vector2 Offset = Vector2.Zero;
        public Vector2 Scale = Vector2.One;
        public SpriteEffects Effect = SpriteEffects.None;
        public float LayerDepth = 0;

        public Rectangle? Rect = null;
        public bool Centered;

        private Dictionary<string, Animation> animations = new();
        private bool animating;
        private Animation currentAnimation;
        private int currentFrame;
        private float animTimer;

        public override string ToString()
            => $"Sprite: {Texture.Name}, {Color}, layerDepth: {LayerDepth}, Rect: {Rect}, Origin {Origin}, " +
                $"Scale: {Scale}, Rotation {Rotation}, SpriteEffects: {Effect}";

        #region Constructors

        public static Sprite None { get { Sprite s = new Sprite(Drawing.pointTexture); s.Texture = null; return s; } }

        public Sprite(Texture2D texture)
        {
            Texture = texture;
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
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
            Rect = rect;
            Color = color;
            LayerDepth = layerDepth;
        }

        public Sprite(Texture2D texture, Rectangle rect)
        {
            Texture = texture;
            Rect = rect;
        }

        public Sprite(Texture2D texture, Rectangle rect, float rotation)
        {
            Texture = texture;
            Rect = rect;
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
            if (Texture == null)
                return;

            if (Texture == Drawing.pointTexture)
                Drawing.Draw(Texture, ParentEntity.Rect, Color, Rotation, Origin, Scale, Effect, LayerDepth);
            else if (Centered)
                Drawing.Draw(Texture, ParentEntity.Pos + ParentEntity.HalfSize + Offset, null, Color.White, Rotation, Origin,
                    Vector2.One, SpriteEffects.None, 1);
            else
                Drawing.Draw(Texture, ParentEntity.Pos + Offset, null, Color, Rotation, Origin, Scale, Effect, LayerDepth);
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

            foreach(XmlElement element in doc["Sprites"])
            {
                AllAnimData[element.Name] = new();

                foreach(XmlElement anim in element)
                {
                   AllAnimData[element.Name].Animations[anim.GetAttribute("id")] = new Animation(DataManager.LoadAllGraphicsWithName(anim.GetAttribute("id")), float.Parse(anim.GetAttribute("delay"), System.Globalization.CultureInfo.InvariantCulture.NumberFormat), anim.GetAttribute("goto"));
                }
            }
        }
    }
}
