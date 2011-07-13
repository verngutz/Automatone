#region File Description
//-----------------------------------------------------------------------------
// MidiGame.cs
//
// 
// Copyright (C) Jeff Sipko. All rights reserved.
// Licensed under Microsoft Permissive License
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using Duet;
using Duet.Audio_System;
using Duet.Render_System;
using Duet.Input_System;

namespace MidiDemo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MidiGame : Microsoft.Xna.Framework.Game
    {
        // Content Objects
        ContentManager content;

        // Graphics Objects
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Audio Objects
        Synthesizer synthesizer;
        Sequencer sequencer;

        // Services
        private IAudioSystemService audioService;
        private InputComponent input;

        // Random Shit
        private float tempoChangeSpeed = 0.0f;
        private float tempo;
        private bool buttonDown = false;
        private byte instrument = 0;

        public MidiGame()
        {
            content = new ContentManager(Services);
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Start up core DUET services
            Duet.Global.Initialize(this);

            // Service Providers
            audioService = (IAudioSystemService)Services.GetService(typeof(IAudioSystemService));

            // Initialize Audio System with game specific settings
            AudioStartupOptions options = new AudioStartupOptions();
            options.audioSettingsPath = "Content\\audio\\WavetablePrograms.xgs";
            audioService.Startup(options);

            synthesizer = new Synthesizer(this);
            synthesizer.WaveBankPath = "Content\\audio\\Wave Bank.xwb";
            synthesizer.SoundBankPath = "Content\\audio\\Sound Bank.xsb";
            synthesizer.m_Patch = content.Load<XACTPatch>(@"content\audio\xact");

            sequencer = new Sequencer(this);
            
            // Setup MIDI routing
            sequencer.OutputDevice = synthesizer;

            // Load MIDI File
            sequencer.LoadMidi("Content\\audio\\moonlightsonata.mid");
            sequencer.PlayMidi();

            tempo = 54.0f;
            audioService.BeatsPerMinute = (ulong)tempo;

            input = new InputComponent(this);
            this.Components.Add(input);

            //input.LStickMoved += new GamepadEventHandler(input_LStickMoved);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            content.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            GamePadState state = GamePad.GetState(PlayerIndex.One);
            
            if (state.Buttons.A == ButtonState.Released)
            {
                buttonDown = false;
            }
            else if (state.Buttons.A == ButtonState.Pressed && !buttonDown)
            {
                buttonDown = true;
                instrument = (byte)(instrument == 0x00 ? 0x01 : 0x00);
                synthesizer.MidiProgramChange(0x00, (byte)instrument);
            }


            // Allows the game to exit
            if (state.Buttons.Back == ButtonState.Pressed)
                this.Exit();

            tempoChangeSpeed = Math.Abs(state.ThumbSticks.Left.Y) > 0.1f ? state.ThumbSticks.Left.Y : 0;

            tempo += tempoChangeSpeed;
            audioService.BeatsPerMinute = (ulong)(tempo>1?tempo:1);

            sequencer.Update(gameTime);
            synthesizer.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            //fontWriter.OutputText("This is a test", 0, 0, Color.Black);
            //fontWriter.OutputText("This is a test", 1, 1, Color.Red);
            spriteBatch.Begin();

            spriteBatch.DrawString(content.Load<SpriteFont>(@"Content\\Font"), "Tempo: " + audioService.BeatsPerMinute.ToString(),
                                        new Vector2(100, 100), Color.Black);

            if (null != synthesizer.ChannelInfo[0])
            {
                spriteBatch.DrawString(content.Load<SpriteFont>(@"Content\\Font"), "Current Instrument: " + synthesizer.ChannelInfo[0].Name,
                                            new Vector2(100, 200), Color.Black);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
