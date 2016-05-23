﻿namespace MinesweeperLike.App.Core
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using MinesweeperLike.App.Constants;
    using MinesweeperLike.App.Contracts;
    using MinesweeperLike.App.Core.Factories;

    public class FieldController : IFieldController
    {
        private readonly IFieldFactory gameFieldFactory;

        private readonly IFieldGenerator gameFieldGenerator;

        public FieldController(IDatabase database, IFieldGenerator gameFieldGenerator)
        {
            this.Database = database;
            this.gameFieldGenerator = gameFieldGenerator;
            this.gameFieldFactory = new FieldFactory();
        }

        public IDatabase Database { get; }

        public IFieldGenerator FieldGenerator { get; }


        public void ClickedOnMine(int buttonCoordinateX, int buttonCoordinateY)
        {
            this.Database.Labels[buttonCoordinateX, buttonCoordinateY].ForeColor = Color.Black;
            this.Database.Labels[buttonCoordinateX, buttonCoordinateY].BackColor = Color.Red;

            int width = this.Database.Buttons.GetLength(0);
            int height = this.Database.Buttons.GetLength(1);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (this.Database.GameField[i, j] == -1)
                    {
                        this.Database.Buttons[i, j].Visible = false;
                    }
                }
            }
        }

        public void ClickedOnEmpty(int buttonCoordinateX, int buttonCoordinateY)
        {
            if (!this.Database.Buttons[buttonCoordinateX, buttonCoordinateY].Visible)
            {
                return;
            }

            var currentButton = this.Database.Buttons[buttonCoordinateX, buttonCoordinateY];
            int currentPosition = this.Database.GameField[buttonCoordinateX, buttonCoordinateY];

            int width = this.Database.GameField.GetLength(0);
            int height = this.Database.GameField.GetLength(1);
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (!this.InBounds(width, height, buttonCoordinateX, i, buttonCoordinateY, j))
                    {
                        continue;
                    }

                    currentButton.Visible = false;
                    if (currentPosition == 0)
                    {
                        this.ClickedOnEmpty(buttonCoordinateX + i, buttonCoordinateY + j);
                    }
                }
            }
        }

        public void CreateGameField(
            Form form,
            Panel panel,
            MouseEventHandler mouseEventHandler,
            int minesCount,
            int gameFieldWidth,
            int gameFieldHeight)
        {
            if (panel == null)
            {
                throw new ArgumentNullException(nameof(panel));
            }

            panel = this.gameFieldFactory.CreateGameField(gameFieldWidth, gameFieldHeight);
            this.gameFieldGenerator.GameField = panel;
            this.gameFieldGenerator.CreateButtons(panel, mouseEventHandler, gameFieldWidth, gameFieldHeight);
            this.gameFieldGenerator.CreateLabels(panel, gameFieldWidth, gameFieldHeight);
            this.gameFieldGenerator.CreateMines(minesCount, gameFieldWidth, gameFieldHeight);
            this.gameFieldGenerator.CreateNumbers(gameFieldWidth, gameFieldHeight);

            form.Controls.Add(panel);
        }

        private bool InBounds(int width, int height, int row, int k, int col, int l)
        {
            return row + k >= 0 && col + l >= 0 && row + k < width && col + l < height;
        }
    }
}