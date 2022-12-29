using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Fiourp.Utility
{
    public static class SpriteBatchExt
    {
        private static readonly FieldInfo f_sortMode =          typeof(SpriteBatch).GetField("_sortMode", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo f_blendState =        typeof(SpriteBatch).GetField("_blendState", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo f_samplerState =      typeof(SpriteBatch).GetField("_samplerState", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo f_depthStencilState = typeof(SpriteBatch).GetField("_depthStencilState", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo f_rasterizerState =   typeof(SpriteBatch).GetField("_rasterizerState", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo f_effect =            typeof(SpriteBatch).GetField("_effect", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo f_spriteEffect =      typeof(SpriteBatch).GetField("_spriteEffect", BindingFlags.Instance | BindingFlags.NonPublic);


        public static SpriteSortMode GetSpriteSortMode(this SpriteBatch spriteBatch)
            => (SpriteSortMode)f_sortMode.GetValue(spriteBatch);

        public static BlendState GetBlendState(this SpriteBatch spriteBatch)
            => (BlendState)f_blendState.GetValue(spriteBatch);

        public static SamplerState GetSamplerState(this SpriteBatch spriteBatch)
            => (SamplerState)f_samplerState.GetValue(spriteBatch);

        public static DepthStencilState GetDepthStencilState(this SpriteBatch spriteBatch)
            => (DepthStencilState)f_depthStencilState.GetValue(spriteBatch);

        public static RasterizerState GetRasterizerState(this SpriteBatch spriteBatch)
            => (RasterizerState)f_rasterizerState.GetValue(spriteBatch);

        public static Effect GetEffect(this SpriteBatch spriteBatch)
            => (Effect)f_effect.GetValue(spriteBatch);

        public static SpriteEffect GetSpriteEffect(this SpriteBatch spriteBatch)
            => (SpriteEffect)f_spriteEffect.GetValue(spriteBatch);


    }
}
