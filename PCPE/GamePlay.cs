using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Flecs.NET.Core;
using PCPE.Component;
using PCPE.Engine;
using PCPE.Module;
using PCPE.Serialized;
using System.Xml.Schema;
using PCPE.Engine.Interface;
using PCPE.Engine.AssetManager;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace PCPE
{
    public class GamePlay : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _uiFont;
        
        public World World { get; private set; }
        public RpeChartData PlayingChart { get; private set; }
        public double CurrentTime => _lastTime;
        public readonly ObjectPool<Entity> NotePool;

        public static GamePlay Instance;
        private double _lastTime = 0f;
        private Query RendererBatchQuery;
        private bool _isCreated = false;
        private readonly SmartFramerate framerate = new(5);
        private readonly Rectangle Bound = new(-800, -450, 1600, 900);

        #region AssetManagers
        public readonly IAssetManager<Texture2D> Texture2DAssetManager;
        public readonly IAssetManager<Song> SongManager;
        private Texture2D _tapNoteTexture;
        #endregion

        public GamePlay()
        {
            Instance = this;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.PreferredBackBufferHeight = 900;
            _graphics.SynchronizeWithVerticalRetrace = true;
            _graphics.ApplyChanges();

            IsFixedTimeStep = false;
            IsMouseVisible = false;
            Texture2DAssetManager = new AssetManager<Texture2D>();
            SongManager = new AssetManager<Song>();
            NotePool = new ObjectPool<Entity>(() => World.Entity());
        }

        protected override void Initialize()
        {
            base.Initialize();
            World = World.Create();
            World.Import<JudgeLineSystem>();
            World.Import<TransformSystem>();
            World.Import<NoteSystem>();
            RendererBatchQuery = World.QueryBuilder<Transform, SpriteRenderer>().Build();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            var fileContent = File.ReadAllText("./test.json");
            PlayingChart = ChartReader.ArrangeChart(fileContent);

            Texture2DAssetManager.AddAsset("judge_line", Content.Load<Texture2D>("JudgeLine"));
            Texture2DAssetManager.AddAsset("note_tap", Content.Load<Texture2D>("Notes/click"));
            Texture2DAssetManager.AddAsset("note_drag", Content.Load<Texture2D>("Notes/drag"));
            Texture2DAssetManager.AddAsset("note_flick", Content.Load<Texture2D>("Notes/flick"));
            Texture2DAssetManager.AddAsset("note_tap_mh", Content.Load<Texture2D>("Notes/click_mh"));
            Texture2DAssetManager.AddAsset("note_drag_mh", Content.Load<Texture2D>("Notes/drag_mh"));
            Texture2DAssetManager.AddAsset("note_flick_mh", Content.Load<Texture2D>("Notes/flick_mh"));
            _tapNoteTexture = Texture2DAssetManager.GetAsset("note_tap");
            SongManager.AddAsset("bgm", Content.Load<Song>("114514"));

            _uiFont = Content.Load<SpriteFont>("UIFont");
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add a initialization function for chart generation
            if (!_isCreated)
            {
                var _judgeLineTexture = Texture2DAssetManager.GetAsset("judge_line");
                var _tapTexture = Texture2DAssetManager.GetAsset("note_tap");
                var _bgm = SongManager.GetAsset("bgm");
                foreach (var l in PlayingChart.judgeLineList)
                {
                    l.ArrangeFloorPosition();
                    var lineEntity = World.Entity();
                    lineEntity.Set<JudgeLine>(new JudgeLine(l))
                        .Set<SpriteRenderer>(new SpriteRenderer { Alpha = 1f, Color = Color.White, Texture = _judgeLineTexture })
                        .Set<Transform>(new Transform(Vector2.One, Vector2.One, 0f));
                    //for (int i = 0; i < l.notes.Count; i++)
                    //{
                    //    GenerateNote(lineEntity, l.notes, i);
                    //}
                }
                MediaPlayer.Play(_bgm);
                _isCreated = true;
            }

            World.Progress();
            _lastTime = gameTime.TotalGameTime.TotalSeconds;
            framerate.Update(gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        public void GenerateNote(Entity e, List<RpeChartData.RpeNoteSet> noteDatas, int id)
        {
            if (!NotePool.TryGetObject(out var noteEntity))
            {
                return;
            }
            noteEntity.Enable();
            noteEntity.ChildOf(e);
            var t = noteDatas[id];
            noteEntity.Set<Transform>(new Transform(new Vector2(t.positionX * 1600f, 10000f), Vector2.One * .18f, 0f))
                .Set<Note>(new Note(t))
                .Set<SpriteRenderer>(new SpriteRenderer { Alpha = 1f, Color = Color.White, Texture = _tapNoteTexture });
            noteDatas.RemoveAt(id);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullCounterClockwise);


            var drawCounter = 0;
            RendererBatchQuery.Each((ref Transform transform, ref SpriteRenderer sr) =>
            {
                /* Alpha Culling */
                if (sr.Alpha < float.Epsilon)
                {
                    return;
                }
                /* Screensize Culling */
                if (!Bound.Contains(transform.Position))
                {
                    return;
                }
                if (drawCounter >= 10000) return;
                drawCounter++;
                var bounds = sr.Texture.Bounds;
                var tPos = transform.Position;
                tPos += new Vector2(800, 450); //Half screen size
                _spriteBatch.Draw(sr.Texture,                         //texture
                    tPos,                                             //position
                    null,                                             //source rectangle
                    sr.Color * sr.Alpha,                              //color
                    MathHelper.ToRadians(transform.Rotation),         //rotation
                    new Vector2(bounds.Width / 2, bounds.Height / 2), //pivot
                    transform.LocalScale,                             //scale
                    SpriteEffects.None,                               //effect
                    0f);                                              //depth
            });

            _spriteBatch.DrawString(_uiFont, $"drawcalls: {drawCounter}\nfps: {framerate.framerate:0.00}", 
                new Vector2(100, 100), Color.White, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}