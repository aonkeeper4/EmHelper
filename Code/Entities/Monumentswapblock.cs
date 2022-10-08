using Celeste.Mod.Entities;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.EmHelper.Entities
{
    [Tracked]
    [CustomEntity("EmHelper/Monumentswapblock")] //stole the code from vanilla, i hate most of those entities
    public class Monumentswapblock : Solid
    {
        public Monumentswapblock(Vector2 position, float width, float height, Vector2 node, int pattern, Color color, bool mute) : base(position, width, height, false)
        {
            this.mute = mute;
            this.Index = color;
            this.pattern = pattern;
            this.start = this.Position;
            this.end = node;
            this.maxForwardSpeed = 360f / Vector2.Distance(this.start, this.end);
            Direction.X = Math.Sign(end.X - start.X);
            Direction.Y = Math.Sign(end.Y - start.Y);
            int num = (int)MathHelper.Min(base.X, node.X);
            int num2 = (int)MathHelper.Min(base.Y, node.Y);
            int num3 = (int)MathHelper.Max(base.X + base.Width, node.X + base.Width);
            int num4 = (int)MathHelper.Max(base.Y + base.Height, node.Y + base.Height);
            this.moveRect = new Rectangle(num, num2, num3 - num, num4 - num2);
            MTexture mtexture;
            MTexture mtexture3;

            List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("objects/monumentswapblock/block");
            mtexture = atlasSubtextures[this.pattern % atlasSubtextures.Count];
            mtexture3 = GFX.Game["objects/monumentswapblock/target"];

            this.nineSliceGreen = new MTexture[3, 3];
            this.nineSliceTarget = new MTexture[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    this.nineSliceGreen[i, j] = mtexture.GetSubtexture(new Rectangle(i * 8, j * 8, 8, 8));
                    this.nineSliceTarget[i, j] = mtexture3.GetSubtexture(new Rectangle(i * 8, j * 8, 8, 8));
                }
            }



            base.Add(new LightOcclude(0.2f));
            base.Depth = -9999;
            this.SurfaceSoundIndex = 35;
        }

        public Monumentswapblock(EntityData data, Vector2 offset) : this(data.Position + offset, (float)data.Width, (float)data.Height, data.Nodes[0] + offset, data.Int("pattern", 0), data.HexColor("color", Calc.HexToColor("82d9ff")), data.Bool("mute", false))
        {
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            scene.Add(this.path = new Monumentswapblock.PathRenderer(this));
        }

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
            Audio.Stop(this.moveSfx, true);
            Audio.Stop(this.returnSfx, true);
        }

        public override void SceneEnd(Scene scene)
        {
            base.SceneEnd(scene);
            Audio.Stop(this.moveSfx, true);
            Audio.Stop(this.returnSfx, true);
        }

        public void Activated()
        {
            Swapping = true;
            target = 1 - target;
            float relativeLerp = target == 1 ? lerp : 1 - lerp;
            if (relativeLerp >= 0.2f)
            {
                speed = maxForwardSpeed;
            }
            else
            {
                speed = MathHelper.Lerp(maxForwardSpeed * 0.333f, maxForwardSpeed, relativeLerp / 0.2f);
            }
            Audio.Stop(moveSfx);
            if (!this.mute)
            {
                moveSfx = Audio.Play("event:/game/05_mirror_temple/swapblock_move", Center);
            }

        }

        public override void Update()
        {
            base.Update();
            speed = Calc.Approach(speed, maxForwardSpeed, maxForwardSpeed / 0.2f * Engine.DeltaTime);
            float num = lerp;
            lerp = Calc.Approach(lerp, target, speed * Engine.DeltaTime);
            if (lerp == 0 || lerp == 1)
                Audio.Stop(moveSfx);
            if (lerp != num)
            {
                Vector2 liftSpeed = (end - start) * speed;
                Vector2 position = Position;
                if (target == 1)
                {
                    liftSpeed = (end - start) * maxForwardSpeed;
                }
                if (lerp < num)
                {
                    liftSpeed *= -1f;
                }
                if (Scene.OnInterval(0.02f))
                {
                    // Allows move particles in both directions
                    MoveParticles((end - start) * (target - 0.5f) * 2);
                }
                Vector2 to = Vector2.Lerp(start, end, lerp);
                Vector2 diff = to - ExactPosition;
                MoveH(diff.X, liftSpeed.X);
                MoveV(diff.Y, liftSpeed.Y);
                if (position != Position)
                {
                    Audio.Position(moveSfx, Center);
                    if (Position == start || Position == end)
                    {
                        Audio.Stop(moveSfx);
                        if (!this.mute)
                        {
                            Audio.Play("event:/game/05_mirror_temple/swapblock_move_end", Center);
                        }

                    }
                }
            }
            if (Swapping && lerp >= 1f)
            {
                Swapping = false;
            }
            StopPlayerRunIntoAnimation = (lerp <= 0f || lerp >= 1f);
        }

        // particles 
        private void MoveParticles(Vector2 normal)
        {
            Vector2 position;
            Vector2 vector;
            float direction;
            float num;
            if (normal.X > 0f)
            {
                position = base.CenterLeft;
                vector = Vector2.UnitY * (base.Height - 6f);
                direction = 3.14159274f;
                num = Math.Max(2f, base.Height / 14f);
            }
            else if (normal.X < 0f)
            {
                position = base.CenterRight;
                vector = Vector2.UnitY * (base.Height - 6f);
                direction = 0f;
                num = Math.Max(2f, base.Height / 14f);
            }
            else if (normal.Y > 0f)
            {
                position = base.TopCenter;
                vector = Vector2.UnitX * (base.Width - 6f);
                direction = -1.57079637f;
                num = Math.Max(2f, base.Width / 14f);
            }
            else
            {
                position = base.BottomCenter;
                vector = Vector2.UnitX * (base.Width - 6f);
                direction = 1.57079637f;
                num = Math.Max(2f, base.Width / 14f);
            }
            this.particlesRemainder += num;
            int num2 = (int)this.particlesRemainder;
            this.particlesRemainder -= (float)num2;
            vector *= 0.5f;
            base.SceneAs<Level>().Particles.Emit(SwapBlock.P_Move, num2, position, vector, direction);
        }

        public override void Render()
        {
            Vector2 vector = this.Position + base.Shake;
            if (this.lerp != (float)this.target && this.speed > 0f)
            {
                Vector2 value = (this.end - this.start).SafeNormalize();
                if (this.target == 1)
                {
                    value *= -1f;
                }
                float num = this.speed / this.maxForwardSpeed;
                float num2 = 16f * num;
                int num3 = 2;
                while ((float)num3 < num2)
                {
                    this.DrawBlockStyle(vector + value * (float)num3, base.Width, base.Height, this.nineSliceGreen, Index * (1f - (float)num3 / num2));
                    num3 += 2;
                }
            }

            this.DrawBlockStyle(vector, base.Width, base.Height, this.nineSliceGreen, Index);
        }

        // too hard dont edit
        private void DrawBlockStyle(Vector2 pos, float width, float height, MTexture[,] ninSlice, Color color)
        {
            int num = (int)(width / 8f);
            int num2 = (int)(height / 8f);
            ninSlice[0, 0].Draw(pos + new Vector2(0f, 0f), Vector2.Zero, color);
            ninSlice[2, 0].Draw(pos + new Vector2(width - 8f, 0f), Vector2.Zero, color);
            ninSlice[0, 2].Draw(pos + new Vector2(0f, height - 8f), Vector2.Zero, color);
            ninSlice[2, 2].Draw(pos + new Vector2(width - 8f, height - 8f), Vector2.Zero, color);
            for (int i = 1; i < num - 1; i++)
            {
                ninSlice[1, 0].Draw(pos + new Vector2((float)(i * 8), 0f), Vector2.Zero, color);
                ninSlice[1, 2].Draw(pos + new Vector2((float)(i * 8), height - 8f), Vector2.Zero, color);
            }
            for (int j = 1; j < num2 - 1; j++)
            {
                ninSlice[0, 1].Draw(pos + new Vector2(0f, (float)(j * 8)), Vector2.Zero, color);
                ninSlice[2, 1].Draw(pos + new Vector2(width - 8f, (float)(j * 8)), Vector2.Zero, color);
            }
            for (int k = 1; k < num - 1; k++)
            {
                for (int l = 1; l < num2 - 1; l++)
                {
                    ninSlice[1, 1].Draw(pos + new Vector2((float)k, (float)l) * 8f, Vector2.Zero, color);
                }
            }

        }

        public Color Index;

        private bool mute = false;

        private int pattern;

        public static ParticleType P_Move;


        private const float ReturnTime = 0.8f;


        public Vector2 Direction;

        public bool Swapping;

        private Vector2 start;

        private Vector2 end;

        private float lerp;

        private int target;

        private Rectangle moveRect;

        private float speed;

        private float maxForwardSpeed;

        private MTexture[,] nineSliceGreen;

        private MTexture[,] nineSliceTarget;

        private Monumentswapblock.PathRenderer path;


        private EventInstance moveSfx;
        private EventInstance returnSfx;
        private float particlesRemainder;

        private class PathRenderer : Entity
        {
            public PathRenderer(Monumentswapblock block) : base(block.Position)
            {
                this.clipTexture = new MTexture();
                this.block = block;
                base.Depth = 8999;
                this.pathTexture = GFX.Game["objects/swapblock/path" + ((block.start.X == block.end.X) ? "V" : "H")];
                this.timer = Calc.Random.NextFloat();
            }

            public override void Update()
            {
                base.Update();
                this.timer += Engine.DeltaTime * 4f;
            }

            public override void Render()
            {
                float scale = 0.5f * (0.5f + ((float)Math.Sin((double)this.timer) + 1f) * 0.25f);
                this.block.DrawBlockStyle(new Vector2((float)this.block.moveRect.X, (float)this.block.moveRect.Y), (float)this.block.moveRect.Width, (float)this.block.moveRect.Height, this.block.nineSliceTarget, this.block.Index * scale);
            }

            private Monumentswapblock block;

            private MTexture pathTexture;

            private MTexture clipTexture;

            private float timer;
        }
    }
}
