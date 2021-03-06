﻿namespace MinesweeperLike.App.Core
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    using MinesweeperLike.App.Constants;
    using MinesweeperLike.App.Contracts;
    using MinesweeperLike.App.Properties;

    public class FieldController : IFieldController
    {
        private readonly Image notAMineImage;

        private readonly Image flagImage;

        public FieldController(IDatabase database)
        {
            this.Database = database;
            this.flagImage = Resources.flag;
            this.notAMineImage = Resources.not_a_mine;
        }

        public IDatabase Database { get; }

        public int MarketButtonsCounter { get; set; }

        public Image Flag => this.flagImage;

        public void ClickedOnMine(int buttonCoordinateX, int buttonCoordinateY)
        {
            this.Database.Labels[buttonCoordinateX, buttonCoordinateY].BackColor = LabelSettings.LabelClickedOnMineColor;

            int width = this.Database.Buttons.GetLength(0);
            int height = this.Database.Buttons.GetLength(1);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (this.Database.GameField[i, j] == MineSettings.Mine && this.Database.Buttons[i, j].Image == null)
                    {
                        this.Database.Buttons[i, j].Visible = false;
                    }

                    if (this.Database.GameField[i, j] != MineSettings.Mine && this.Database.Buttons[i, j].Image != null)
                    {
                        this.Database.Buttons[i, j].Image = this.notAMineImage;
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

            if (currentButton.Image != null)
            {
                this.MarketButtonsCounter--;
            }

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

                    this.Database.MarketButtons[buttonCoordinateX, buttonCoordinateY] = true;
                    currentButton.Visible = false;
                    if (currentPosition == 0)
                    {
                        this.ClickedOnEmpty(buttonCoordinateX + i, buttonCoordinateY + j);
                    }
                }
            }
        }

        public List<bool> GetMarketButtons()
        {
            List<bool> markedButtons = new List<bool>();

            for (int i = 0; i < this.Database.MarketButtons.GetLength(0); i++)
            {
                for (int j = 0; j < this.Database.MarketButtons.GetLength(1); j++)
                {
                    markedButtons.Add(this.Database.MarketButtons[i, j]);
                }
            }

            return markedButtons;
        }

        public void MarkAllMinesWithFlag()
        {
            int width = this.Database.MarketButtons.GetLength(0);
            int height = this.Database.MarketButtons.GetLength(1);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (this.Database.MarketButtons[i, j])
                    {
                        this.Database.Buttons[i, j].Image = this.Flag;
                    }
                }
            }
        }

        public void TimerConfiguration(Timer timer, EventHandler e)
        {
            timer.Enabled = true;
            timer.Interval = 1000;
            timer.Tick += e;
        }

        public void RestartGameField(int gameFieldWidth, int gameFieldHeight)
        {
            for (int i = 0; i < gameFieldWidth; i++)
            {
                for (int j = 0; j < gameFieldHeight; j++)
                {
                    if (this.Database.Buttons[i, j].Image != null)
                    {
                        this.Database.Buttons[i, j].Image = null;
                    }

                    if (!this.Database.Buttons[i, j].Visible)
                    {
                        this.Database.Buttons[i, j].Visible = true;
                    }

                    if (this.Database.Labels[i, j].Image == null)
                    {
                        this.Database.MarketButtons[i, j] = false;
                    }

                    this.Database.Labels[i, j].BackColor = LabelSettings.LabelBackColor;
                }
            }
        }

        public void SolveGameField(int gameFieldWidth, int gameFieldHeight)
        {
            for (int i = 0; i < gameFieldWidth; i++)
            {
                for (int j = 0; j < gameFieldHeight; j++)
                {
                    var currentButton = this.Database.Buttons[i, j];
                    var currentGameFieldPosition = this.Database.GameField[i, j];

                    if (currentGameFieldPosition == -1)
                    {
                        currentButton.Image = this.Flag;
                    }
                    else
                    {
                        currentButton.Visible = false;
                    }
                }
            }
        }

        private bool InBounds(int width, int height, int row, int k, int col, int l)
        {
            return row + k >= 0 && col + l >= 0 && row + k < width && col + l < height;
        }
    }
}