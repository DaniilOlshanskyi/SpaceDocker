using BEPUphysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceDocker
{
    internal class Skybox : DrawableGameComponent
    {
        private Model skybox;
        private Texture2D skyboxTexture;
        float aspectRatio;

        public Skybox(Game game, float aspectRatio) : base(game)
        {
            game.Components.Add(this);
            this.aspectRatio = aspectRatio;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            skyboxTexture = Game.Content.Load<Texture2D>("Textures\\starsky");
            skybox = Game.Content.Load<Model>("Models\\cube");
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            Matrix[] transforms = new Matrix[skybox.Bones.Count];
            skybox.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in skybox.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Alpha = 1f;
                    effect.LightingEnabled = false;
                    effect.TextureEnabled = true;
                    effect.Texture = skyboxTexture;
                    effect.World = Matrix.CreateScale(100f) * transforms[mesh.ParentBone.Index];
                    effect.View = Matrix.CreateLookAt(MathConverter.Convert(Game1.cameraPositionBepu),
                        MathConverter.Convert(Game1.physCapsule.Position), MathConverter.Convert(Game1.physCapsule.OrientationMatrix.Up));
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f), aspectRatio,
                        1.0f, 100000000.0f);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
            base.Draw(gameTime);
        }


    }
}
