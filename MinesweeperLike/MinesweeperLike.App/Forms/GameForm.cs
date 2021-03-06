﻿namespace MinesweeperLike.App.Forms
{
    using System;
    using System.Windows.Forms;

    using MinesweeperLike.App.Contracts;
    using MinesweeperLike.App.Core;

    public partial class GameForm : Form
    {
        private readonly IEngine engine;

        public GameForm()
        {
            this.InitializeComponent();
            this.engine = new Engine(this);
            this.engine.Run();
        }
    }
}