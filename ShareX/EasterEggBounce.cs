﻿#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using ShareX.HelpersLib;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX
{
    public class EasterEggBounce : IDisposable
    {
        public Form Form { get; private set; }
        public bool IsWorking { get; private set; }
        public int Speed { get; set; } = 20;
        public bool ApplyGravity { get; set; } = true;
        public int GravityPower { get; set; } = 3;
        public int BouncePower { get; set; } = 50;

        private Rectangle screenRectangle;
        private Timer timer;
        private Point velocity;

        public EasterEggBounce(Form form)
        {
            Form = form;

            timer = new Timer();
            timer.Interval = 20;
            timer.Tick += bounceTimer_Tick;
        }

        public void Start()
        {
            if (!IsWorking)
            {
                IsWorking = true;

                screenRectangle = CaptureHelpers.GetScreenWorkingArea();
                velocity = new Point(MathHelpers.RandomPick(-Speed, Speed), ApplyGravity ? GravityPower : MathHelpers.RandomPick(-Speed, Speed));
                timer.Start();
            }
        }

        public void Stop()
        {
            if (IsWorking)
            {
                if (timer != null)
                {
                    timer.Stop();
                }

                IsWorking = false;
            }
        }

        private void bounceTimer_Tick(object sender, EventArgs e)
        {
            if (Form != null && !Form.IsDisposed)
            {
                int x = Form.Left + velocity.X;
                int windowRight = screenRectangle.X + screenRectangle.Width - Form.Width - 1;

                if (x <= screenRectangle.X)
                {
                    x = screenRectangle.X;
                    velocity.X = Speed;
                }
                else if (x >= windowRight)
                {
                    x = windowRight;
                    velocity.X = -Speed;
                }

                int y = Form.Top + velocity.Y;
                int windowBottom = screenRectangle.Y + screenRectangle.Height - Form.Height - 1;

                if (ApplyGravity)
                {
                    if (y >= windowBottom)
                    {
                        y = windowBottom;
                        velocity.Y = -BouncePower.RandomAdd(-10, 10);
                    }
                    else
                    {
                        velocity.Y += GravityPower;
                    }
                }
                else
                {
                    if (y <= screenRectangle.Y)
                    {
                        y = screenRectangle.Y;
                        velocity.Y = Speed;
                    }
                    else if (y >= windowBottom)
                    {
                        y = windowBottom;
                        velocity.Y = -Speed;
                    }
                }

                Form.Location = new Point(x, y);
            }
        }

        public void Dispose()
        {
            Stop();

            if (timer != null)
            {
                timer.Dispose();
            }
        }
    }
}